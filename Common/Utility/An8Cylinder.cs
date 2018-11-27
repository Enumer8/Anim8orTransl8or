using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Cylinder
   {
      internal static mesh Convert(cylinder c)
      {
         // Convert the sphere to a mesh
         mesh m = new mesh();
         m.name = c.name;
         m.@base = c.@base;
         m.pivot = c.pivot;
         m.material = c.material;

         if ( c.material?.name != null )
         {
            m.materiallist = new materiallist();
            m.materiallist.materialname = new @string[1];
            m.materiallist.materialname[0] = new @string();
            m.materiallist.materialname[0].text = c.material.name;
         }

         List<point> points = new List<point>();
         List<point> normals = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // Note: If diameter and topdiameter are both 0, then there's nothing
         // to draw. Technically Anim8or v1.00 draws a line. If length is 0,
         // Anim8or v1.00 still draws the cylinder as if it was 0.001. If
         // longitude is less than 3, Anim8or v1.00 still draws the cylinder as
         // if it was 3. If latitude is less than 1, Anim8or v1.00 still draws
         // the cylinder as id it was 1.
         if ( c.diameter?.text > 0 || c.topdiameter?.text > 0 )
         {
            // Note: Anim8or v1.00 limits length to 0.001
            Double length = Math.Max(c.length?.text ?? 0, 0.001);
            Double startDiameter = Math.Max(c.diameter?.text ?? 0, 0);
            Double endDiameter = Math.Max(c.topdiameter?.text ?? 0, 0);
            // Note: Anim8or v1.00 limits longitude between 3 and 128.
            Int64 longitude = Math.Max(c.longlat?.longitude ?? 0, 3);
            longitude = Math.Min(longitude, 128);
            // Note: Anim8or v1.00 limits latitude between 1 and 100.
            Int64 latitude = Math.Max(c.longlat?.latitude ?? 0, 1);
            latitude = Math.Min(latitude, 100);

            Double angle = 2 * Math.PI / longitude;
            point vec3Ref = new point() { x = startDiameter / 2 };
            point vec3RefTop = new point() { x = endDiameter / 2, y = length };
            Double heightStep = length / latitude;
            Int64[,] vertexIndices = new Int64[longitude, latitude + 1];

            for ( Int64 i = 0; i < longitude; i++ )
            {
               quaternion quat = new quaternion();
               quat.y = Math.Sin(angle * i / 2);
               quat.w = Math.Cos(angle * i / 2);

               point vec4 = An8Math.Rotate(quat, vec3Ref);
               point vec4Top = An8Math.Rotate(quat, vec3RefTop);
               point vecCone = new point();
               vecCone.x = vec4Top.x - vec4.x;
               vecCone.y = vec4Top.y - vec4.y;
               vecCone.z = vec4Top.z - vec4.z;
               vecCone = An8Math.Normalize(vecCone);

               for ( Int64 j = 0; j <= latitude; j++ )
               {
                  // point
                  point newPoint = new point();
                  newPoint.x = vec4.x + vecCone.x * (j * heightStep);
                  newPoint.y = vec4.y + vecCone.y * (j * heightStep);
                  newPoint.z = vec4.z + vecCone.z * (j * heightStep);
                  points.Add(newPoint);

                  // Normal
                  point newNormal = new point() { x = newPoint.x, z = newPoint.z };
                  newNormal = An8Math.Normalize(newNormal);
                  normals.Add(newNormal);

                  // texcoord
                  texcoord newTexCoord = new texcoord() { u = (Double)i / longitude, v = (Double)j / latitude };
                  texcoords.Add(newTexCoord);

                  vertexIndices[i, j] = points.Count - 1;
               }
            }

            for ( Int64 j = 0; j <= latitude; j++ )
            {
               // texcoord
               texcoord newTexCoord = new texcoord() { u = 1, v = (Double)j / latitude };
               texcoords.Add(newTexCoord);
            }

            // Faces //////////////////////////////////////////////////////////
            if ( c.capstart != null )
            {
               // Disc base
               facedata newFaceBottom = new facedata();
               newFaceBottom.numpoints = longitude;
               newFaceBottom.flags = facedataenum.hasnormals | facedataenum.hastexture;
               newFaceBottom.matno = 0;
               newFaceBottom.flatnormalno = 0;
               newFaceBottom.pointdata = new pointdata[newFaceBottom.numpoints];

               for ( Int64 i = 0; i < longitude; i++ )
               {
                  pointdata index = new pointdata();
                  index.pointindex = vertexIndices[i, 0];
                  index.normalindex = normals.Count;
                  index.texcoordindex = i * (latitude + 1);
                  newFaceBottom.pointdata[i] = index;
               }

               facedatas.Add(newFaceBottom);

               point newNormal = new point() { y = -1 };
               normals.Add(newNormal);
            }

            if ( c.capend != null )
            {
               // Disc Top
               facedata newFaceTop = new facedata();
               newFaceTop.numpoints = longitude;
               newFaceTop.flags = facedataenum.hasnormals | facedataenum.hastexture;
               newFaceTop.matno = 0;
               newFaceTop.flatnormalno = 0;
               newFaceTop.pointdata = new pointdata[newFaceTop.numpoints];

               for ( Int64 i = 0; i < longitude; i++ )
               {
                  pointdata index = new pointdata();
                  index.pointindex = vertexIndices[longitude - 1 - i, latitude];
                  index.normalindex = normals.Count;
                  index.texcoordindex = i * (latitude + 1);
                  //newFaceTop.pointdata[i] = index;
                  newFaceTop.pointdata[i] = index;
               }

               facedatas.Add(newFaceTop);

               point newNormal = new point() { y = 1 };
               normals.Add(newNormal);
            }

            // Faces of the "tunnel"
            for ( Int64 i = 0; i < longitude; i++ )
            {
               for ( Int64 j = 0; j < latitude; j++ )
               {
                  facedata newFaceTunnel = new facedata();
                  newFaceTunnel.numpoints = 4;
                  newFaceTunnel.flags = facedataenum.hasnormals | facedataenum.hastexture;
                  newFaceTunnel.matno = 0;
                  newFaceTunnel.flatnormalno = 0;
                  newFaceTunnel.pointdata = new pointdata[newFaceTunnel.numpoints];

                  if ( i != longitude - 1 )
                  {
                     pointdata index = new pointdata();
                     index.pointindex = vertexIndices[i, j];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + i * (latitude + 1);
                     newFaceTunnel.pointdata[0] = index;

                     index = new pointdata();
                     index.pointindex = vertexIndices[i, j + 1];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + 1 + i * (latitude + 1);
                     newFaceTunnel.pointdata[1] = index;

                     index = new pointdata();
                     index.pointindex = vertexIndices[i + 1, j + 1];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + 1 + (i + 1) * (latitude + 1);
                     newFaceTunnel.pointdata[2] = index;

                     index = new pointdata();
                     index.pointindex = vertexIndices[i + 1, j];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + (i + 1) * (latitude + 1);
                     newFaceTunnel.pointdata[3] = index;
                  }
                  else
                  {
                     pointdata index = new pointdata();
                     index.pointindex = vertexIndices[i, j];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + i * (latitude + 1);
                     newFaceTunnel.pointdata[0] = index;

                     index = new pointdata();
                     index.pointindex = vertexIndices[i, j + 1];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + 1 + i * (latitude + 1);
                     newFaceTunnel.pointdata[1] = index;

                     index = new pointdata();
                     index.pointindex = vertexIndices[0, j + 1];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + 1 + (i + 1) * (latitude + 1);
                     newFaceTunnel.pointdata[2] = index;

                     index = new pointdata();
                     index.pointindex = vertexIndices[0, j];
                     index.normalindex = index.pointindex;
                     index.texcoordindex = j + (i + 1) * (latitude + 1);
                     newFaceTunnel.pointdata[3] = index;
                  }

                  facedatas.Add(newFaceTunnel);
               }
            }
         }

         if ( points.Count > 0 )
         {
            m.points = new points();
            m.points.point = points.ToArray();
         }

         if ( normals.Count > 0 )
         {
            m.normals = new normals();
            m.normals.point = normals.ToArray();
         }

         if ( texcoords.Count > 0 )
         {
            m.texcoords = new texcoords();
            m.texcoords.texcoord = texcoords.ToArray();
         }

         if ( facedatas.Count > 0 )
         {
            m.faces = new faces();
            m.faces.facedata = facedatas.ToArray();
         }

         return m;
      }
   }
}
