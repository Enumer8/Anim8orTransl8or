using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Normals
   {
      internal static mesh Calculate(mesh m)
      {
         // Return if normals are already present or information is missing
         if ( m.normals?.point != null ||
              m.points?.point == null ||
              m.faces?.facedata == null )
         {
            return m;
         }

         Double smoothAngle = m.smoothangle?.text ?? 45.0;
         List<point> normals = new List<point>();

         // Calculate facet normals
         List<point> facetNormals = new List<point>(m.faces.facedata.Length);

         foreach ( facedata facedata in m.faces.facedata )
         {
            facedata.flags |= facedataenum.hasnormals;
            facedata.flatnormalno = 0;

            point a = m.points.point[facedata.pointdata[0].pointindex];
            point b = m.points.point[facedata.pointdata[1].pointindex];
            point c = m.points.point[facedata.pointdata[2].pointindex];

            facetNormals.Add((c - a).Cross(b - a).Normalize());
         }

         Double cosAngle = Math.Cos(smoothAngle * Math.PI / 180);
         LinkedNode[] members = new LinkedNode[m.points.point.Length];

         for ( Int32 i = 0; i < m.faces.facedata.Length; i++ )
         {
            facedata f = m.faces.facedata[i];

            foreach ( pointdata p in f.pointdata )
            {
               LinkedNode l = new LinkedNode();
               l.FaceIndex = i;
               l.Next = members[p.pointindex];
               members[p.pointindex] = l;
            }
         }

         for ( Int32 i = 0; i < m.points.point.Length; i++ )
         {
            LinkedNode node = members[i];

            point average = new point();
            Int32 avg = 0;

            while ( node != null )
            {
               // Only average if the angle between the two faces is less than
               // the threshold angle
               Double dot = facetNormals[node.FaceIndex].Dot(
                  facetNormals[members[i].FaceIndex]);

               if ( dot > cosAngle )
               {
                  node.Averaged = true;
                  average = average + facetNormals[node.FaceIndex];
                  avg = 1;
               }

               node = node.Next;
            }

            if ( avg > 0 )
            {
               average = average.Normalize();
               avg = normals.Count;
               normals.Add(average);
            }

            node = members[i];

            while ( node != null )
            {
               pointdata[] pointdata =
                  m.faces.facedata[node.FaceIndex].pointdata;

               if ( node.Averaged )
               {
                  for ( Int32 j = 0; j < pointdata.Length; j++ )
                  {
                     // If this node was averaged, use the average normal
                     if ( pointdata[j].pointindex == i )
                     {
                        pointdata[j].normalindex = avg;
                        break;
                     }
                  }
               }
               else
               {
                  // If this node wasn't averaged, use the facet normal
                  for ( Int32 j = 0; j < pointdata.Length; j++ )
                  {
                     // If this node was averaged, use the average normal
                     if ( pointdata[j].pointindex == i )
                     {
                        pointdata[j].normalindex = normals.Count;
                        break;
                     }
                  }

                  normals.Add(facetNormals[(Int32)node.FaceIndex]);
               }

               node = node.Next;
            }
         }

         m.normals = new normals();
         m.normals.point = normals.ToArray();
         return m;
      }

      class LinkedNode
      {
         public Int32 FaceIndex;
         public Boolean Averaged; // or Smoothed?
         public LinkedNode Next;
      }
   }
}
