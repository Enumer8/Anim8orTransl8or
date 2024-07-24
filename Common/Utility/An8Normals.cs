// Copyright © 2024 Contingent Games.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;
using System.Linq;

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
               indexA = f.pointdata[0]?.pointindex ?? -1;
               indexB = f.pointdata[1]?.pointindex ?? -1;
               indexC = f.pointdata[2]?.pointindex ?? -1;
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

            foreach ( pointdata p in f?.pointdata ?? new pointdata[0] )
            {
               Int64 index = p?.pointindex ?? -1;

               if ( index >= 0 && index < pointCount )
               {
                  PointLink pointLink = pointLinks[index];

                  if ( pointLink == null )
                  {
                     pointLinks[index] = pointLink = new PointLink();
                  }

                  pointLink.SmoothingGroups.Add(new SmoothingGroup
                  {
                     FaceIndices = { i }
                  });
               }
            }
         }

         // Merge groups
         foreach ( PointLink pointLink in pointLinks )
         {
            Int32 count = pointLink?.SmoothingGroups.Count ?? 0;

            for ( Int32 i = count - 1; i >= 0; i-- )
            {
               SmoothingGroup groupA = pointLink.SmoothingGroups[i];

               for ( Int32 j = i - 1; j >= 0; j-- )
               {
                  SmoothingGroup groupB = pointLink.SmoothingGroups[j];

                  // If any face in groupA should smooth with any face in
                  // groupB, then combine the groups.
                  Boolean shouldMerge = false;

                  foreach ( Int32 faceAIndex in groupA.FaceIndices )
                  {
                     foreach ( Int32 faceBIndex in groupB.FaceIndices )
                     {
                        if ( IsSmooth(
                           m2.edges?.edge,
                           m2.faces?.facedata,
                           normals,
                           cosAngle,
                           faceAIndex,
                           faceBIndex) )
                        {
                           shouldMerge = true;
                           break;
                        }
                     }

                     if ( shouldMerge )
                     {
                        break;
                     }
                  }

                  if ( shouldMerge )
                  {
                     groupB.FaceIndices.AddRange(groupA.FaceIndices);
                     pointLink.SmoothingGroups.RemoveAt(i);
                     break;
                  }
               }
            }
         }

         // Note: Rearrange the groups to match Anim8or v1.00's order.
         foreach ( PointLink pointLink in pointLinks )
         {
            if ( pointLink?.SmoothingGroups.Count > 2 )
            {
               pointLink.SmoothingGroups.Reverse(
                  1,
                  pointLink.SmoothingGroups.Count - 1);
            }
         }

         // Calculate the smooth normals for each group
         for ( Int32 i = 0; i < pointCount; i++ )
         {
            PointLink pointLink = pointLinks[i];

            foreach ( SmoothingGroup smoothingGroup in
               pointLink?.SmoothingGroups ??
               new List<SmoothingGroup>() )
            {
               point smoothNormal = new point();

               foreach ( Int32 faceIndex in smoothingGroup.FaceIndices )
               {
                  foreach ( pointdata p in
                     m2.faces.facedata[faceIndex].pointdata )
                  {
                     if ( p.pointindex == i )
                     {
                        p.normalindex = normals.Count;
                     }
                  }

                  smoothNormal += normals[faceIndex];
               }

               smoothNormal = smoothNormal.Normalize();
               normals.Add(smoothNormal);
            }
         }

         if ( normals.Count > 0 )
         {
            m2.normals = new normals();
            m2.normals.point = normals.ToArray();
         }

         return m2;
      }

      static faces RemoveNormals(faces faces)
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

      static facedata RemoveNormals(facedata facedata)
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

      static pointdata RemoveNormals(pointdata pointdata)
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

      static Boolean IsSmooth(
         edge[] edges,
         facedata[] faces,
         List<point> normals,
         Double cosAngle,
         Int32 faceAIndex,
         Int32 faceBIndex)
      {
         Int32 facesLength = faces?.Length ?? 0;

         if ( faceAIndex >= 0 && faceAIndex < facesLength &&
              faceBIndex >= 0 && faceBIndex < facesLength )
         {
            facedata faceA = faces[faceAIndex];
            facedata faceB = faces[faceBIndex];

            List<Int64> indicesA = new List<Int64>();
            List<Int64> indicesB = new List<Int64>();

            foreach ( pointdata p in faceA?.pointdata ?? new pointdata[0] )
            {
               if ( p != null )
               {
                  indicesA.Add(p.pointindex);
               }
            }

            foreach ( pointdata p in faceB?.pointdata ?? new pointdata[0] )
            {
               if ( p != null )
               {
                  indicesB.Add(p.pointindex);
               }
            }

            List<Int64> indices = indicesA.Intersect(indicesB).ToList();

            if ( indices.Count == 2 )
            {
               indices.Sort();

               Boolean creasedEdge = false;

               foreach ( edge edge in edges ?? new edge[0] )
               {
                  if ( indices[0] == edge?.startpointindex &&
                       indices[1] == edge?.endpointindex )
                  {
                     if ( edge?.sharpness < 0 )
                     {
                        creasedEdge = true;
                     }
                     break;
                  }
               }

               if ( !creasedEdge )
               {
                  Double dot = normals[faceAIndex].Dot(normals[faceBIndex]);

                  if ( dot >= cosAngle )
                  {
                     return true;
                  }
               }
            }
         }

         return false;
      }

      class PointLink
      {
         public List<SmoothingGroup> SmoothingGroups =
            new List<SmoothingGroup>();
      }

      class SmoothingGroup
      {
         public List<Int32> FaceIndices = new List<Int32>();
      }
   }
}
