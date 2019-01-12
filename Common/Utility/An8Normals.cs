using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Normals
   {
      /// <summary>
      /// This will produce the same normals as if the user had clicked
      /// Options->Debug->Output Normals in Anim8or v1.00.
      /// </summary>
      /// <param name="m">the mesh</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the mesh with normals</returns>
      internal static mesh Calculate(mesh m, Action<String> callback = null)
      {
         mesh m2 = new mesh();
         m2.name = m?.name;
         m2.@base = m?.@base;
         m2.pivot = m?.pivot;
         m2.material = m?.material;
         m2.smoothangle = m?.smoothangle;
         m2.materiallist = m?.materiallist;
         m2.points = m?.points;
         m2.normals = null;
         m2.edges = m?.edges;
         m2.texcoords = m?.texcoords;
         m2.faces = RemoveNormals(m?.faces);

         // The face normals are stored first, then normals for each point in
         // each face. This is done so the face index can be used on this list.
         List<point> normals = new List<point>();

         // Note: These defaults and limits were reversed engineered.
         Double smoothAngle = (m2.smoothangle?.text ?? 45).Limit(0, 180);

         Double cosAngle = Math.Cos(smoothAngle * Math.PI / 180);
         Int32 faceCount = m2.faces?.facedata?.Length ?? 0;
         Int32 pointCount = m2.points?.point?.Length ?? 0;
         PointLink[] pointLinks = new PointLink[pointCount];

         if ( callback != null )
         {
            callback($"The \"{m2.name?.text}\" mesh's calculated normals will not exactly match what Anim8or would generate, though they will be close. Please output normals from Anim8or if they need to be exact.");

            if ( m2.points?.point == null || m2.points.point.Length == 0 )
            {
               callback($"The \"{m2.name?.text}\" mesh does not have any points, so no normals can be calculated.");
            }

            if ( m2.faces?.facedata == null || m2.faces.facedata.Length == 0 )
            {
               callback($"The \"{m2.name?.text}\" mesh does not have any faces, so no normals can be calculated.");
            }
         }

         // Calculate the normals of faces and link points to faces
         for ( Int32 i = 0; i < faceCount; i++ )
         {
            facedata f = m2.faces.facedata[i];
            Int64 indexA = -1;
            Int64 indexB = -1;
            Int64 indexC = -1;

            if ( f?.pointdata?.Length >= 3 )
            {
               indexA = f.pointdata[0].pointindex;
               indexB = f.pointdata[1].pointindex;
               indexC = f.pointdata[2].pointindex;
            }

            if ( indexA >= 0 &&
                 indexA < pointCount &&
                 indexB >= 0 &&
                 indexB < pointCount &&
                 indexC >= 0 &&
                 indexC < pointCount )
            {
               f.flags |= facedataenum.hasnormals;
               f.flatnormalno = normals.Count;

               point a = m2.points.point[indexA];
               point b = m2.points.point[indexB];
               point c = m2.points.point[indexC];

               normals.Add((c - a).Cross(b - a).Normalize());
            }
            else
            {
               callback?.Invoke($"The \"{m2.name?.text}\" mesh's face {i} is invalid.");
               normals.Add(new point(0, 1, 0));
            }

            foreach ( pointdata p in m2.faces.facedata[i]?.pointdata ??
               new pointdata[0] )
            {
               PointLink l = new PointLink();
               l.Next = pointLinks[p.pointindex];
               l.FaceIndex = i;
               l.SmoothGroup = -1;
               pointLinks[p.pointindex] = l;
            }
         }

         // Create the minimum number of smooth groups
         for ( Int32 i = 0; i < pointCount; i++ )
         {
            PointLink thisPointLink = pointLinks[i];
            Int32 maxSmoothGroup = -1;

            while ( thisPointLink != null )
            {
               if ( thisPointLink.SmoothGroup < 0 )
               {
                  thisPointLink.SmoothGroup = ++maxSmoothGroup;

                  List<Int32> facesInGroup = new List<Int32>();
                  facesInGroup.Add(thisPointLink.FaceIndex);

                  Boolean addedNewMember = true;

                  while ( addedNewMember )
                  {
                     addedNewMember = false;

                     PointLink thatPointLink = thisPointLink.Next;

                     while ( thatPointLink != null )
                     {
                        if ( thatPointLink.SmoothGroup !=
                           thisPointLink.SmoothGroup )
                        {
                           Boolean inGroup = false;

                           foreach ( Int32 faceIndex in facesInGroup )
                           {
                              Double dot = normals[faceIndex].Dot(
                                 normals[thatPointLink.FaceIndex]);

                              if ( dot >= cosAngle )
                              {
                                 inGroup = true;
                                 break;
                              }
                           }

                           if ( inGroup )
                           {
                              facesInGroup.Add(thatPointLink.FaceIndex);
                              addedNewMember = true;
                              thatPointLink.SmoothGroup =
                                 thisPointLink.SmoothGroup;
                           }
                        }

                        thatPointLink = thatPointLink.Next;
                     }
                  }
               }

               thisPointLink = thisPointLink.Next;
            }
         }

         // Calculate the normals for each point in each face
         for ( Int32 i = 0; i < faceCount; i++ )
         {
            foreach ( pointdata p in m2.faces.facedata[i]?.pointdata ??
               new pointdata[0] )
            {
               // Find the smooth group of this point in this face
               Int32 smoothGroup = -1;
               PointLink pointLink = pointLinks[p.pointindex];

               while ( pointLink != null )
               {
                  if ( pointLink.FaceIndex == i )
                  {
                     smoothGroup = pointLink.SmoothGroup;
                     pointLink = null;
                  }
                  else
                  {
                     pointLink = pointLink.Next;
                  }
               }

               point smoothNormal = new point();
               pointLink = pointLinks[p.pointindex];

               while ( pointLink != null )
               {
                  if ( pointLink.SmoothGroup == smoothGroup )
                  {
                     smoothNormal += normals[pointLink.FaceIndex];
                  }

                  pointLink = pointLink.Next;
               }

               p.normalindex = normals.Count;
               normals.Add(smoothNormal.Normalize());
            }
         }

         if ( normals.Count > 0 )
         {
            m2.normals = new normals();
            m2.normals.point = normals.ToArray();
         }

         return m2;
      }

      internal static faces RemoveNormals(faces faces)
      {
         faces f;

         if ( faces != null )
         {
            f = new faces();

            if ( faces.facedata != null )
            {
               f.facedata = new facedata[faces.facedata.Length];

               for ( Int32 i = 0; i < faces.facedata.Length; i++ )
               {
                  f.facedata[i] = RemoveNormals(faces.facedata[i]);
               }
            }
         }
         else
         {
            f = null;
         }

         return f;
      }

      internal static facedata RemoveNormals(facedata facedata)
      {
         facedata fd;

         if ( facedata != null )
         {
            fd = new facedata();

            fd.numpoints = facedata.numpoints;
            fd.flags = facedata.flags & ~facedataenum.hasnormals;
            fd.matno = facedata.matno;
            fd.flatnormalno = -1;

            if ( facedata.pointdata != null )
            {
               fd.pointdata = new pointdata[facedata.pointdata.Length];

               for ( Int32 i = 0; i < facedata.pointdata.Length; i++ )
               {
                  fd.pointdata[i] = RemoveNormals(facedata.pointdata[i]);
               }
            }
         }
         else
         {
            fd = null;
         }

         return fd;
      }

      internal static pointdata RemoveNormals(pointdata pointdata)
      {
         pointdata pd;

         if ( pointdata != null )
         {
            pd = new pointdata();

            pd.pointindex = pointdata.pointindex;
            pd.normalindex = 0;
            pd.texcoordindex = pointdata.texcoordindex;
         }
         else
         {
            pd = null;
         }

         return pd;
      }

      class PointLink
      {
         public PointLink Next;
         public Int32 FaceIndex;
         public Int32 SmoothGroup;
      }
   }
}
