using Anim8orTransl8or.An8;
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
            point vec3Ref = new point(startDiameter / 2, 0);
            point vec3RefTop = new point(endDiameter / 2, length);
            Double heightStep = length / latitude;
            Int32[,] indices = new Int32[longitude, latitude + 1];

            for ( Int64 i = 0; i < longitude; i++ )
            {
               quaternion q = new quaternion();
               q.y = Math.Sin(angle * i / 2);
               q.w = Math.Cos(angle * i / 2);

               point vec4 = q.Rotate(vec3Ref);
               point vec4Top = q.Rotate(vec3RefTop);
               point vecCone = vec4Top - vec4;
               vecCone = vecCone.Normalize();

               for ( Int64 j = 0; j <= latitude; j++ )
               {
                  point p = vec4 + vecCone * (j * heightStep);

                  indices[i, j] = points.Count;
                  points.Add(p);

                  texcoord t = new texcoord(
                     (Double)i / longitude,
                     (Double)j / latitude);

                  texcoords.Add(t);
               }
            }

            for ( Int64 j = 0; j <= latitude; j++ )
            {
               texcoord t = new texcoord(1, (Double)j / latitude);
               texcoords.Add(t);
            }

            // Faces
            if ( c.capstart != null )
            {
               // Disc base
               facedata f = new facedata();
               f.numpoints = longitude;
               f.flags = facedataenum.hastexture;
               f.matno = 0;
               f.flatnormalno = -1;
               f.pointdata = new pointdata[f.numpoints];

               for ( Int64 i = 0; i < longitude; i++ )
               {
                  pointdata index = new pointdata();
                  index.pointindex = indices[i, 0];
                  index.texcoordindex = i * (latitude + 1);
                  f.pointdata[i] = index;
               }

               facedatas.Add(f);
            }

            if ( c.capend != null )
            {
               // Disc top
               facedata f = new facedata();
               f.numpoints = longitude;
               f.flags = facedataenum.hastexture;
               f.matno = 0;
               f.flatnormalno = -1;
               f.pointdata = new pointdata[f.numpoints];

               for ( Int64 i = 0; i < longitude; i++ )
               {
                  pointdata index = new pointdata();
                  index.pointindex = indices[longitude - 1 - i, latitude];
                  index.texcoordindex = i * (latitude + 1);
                  f.pointdata[i] = index;
               }

               facedatas.Add(f);
            }

            // Faces of the "tunnel"
            for ( Int64 i = 0; i < longitude; i++ )
            {
               for ( Int64 j = 0; j < latitude; j++ )
               {
                  facedata f = new facedata();
                  f.numpoints = 4;
                  f.flags = facedataenum.hastexture;
                  f.matno = 0;
                  f.flatnormalno = -1;
                  f.pointdata = new pointdata[f.numpoints];

                  if ( i != longitude - 1 )
                  {
                     pointdata p = new pointdata();
                     p.pointindex = indices[i, j];
                     p.texcoordindex = j + i * (latitude + 1);
                     f.pointdata[0] = p;

                     p = new pointdata();
                     p.pointindex = indices[i, j + 1];
                     p.texcoordindex = j + 1 + i * (latitude + 1);
                     f.pointdata[1] = p;

                     p = new pointdata();
                     p.pointindex = indices[i + 1, j + 1];
                     p.texcoordindex = j + 1 + (i + 1) * (latitude + 1);
                     f.pointdata[2] = p;

                     p = new pointdata();
                     p.pointindex = indices[i + 1, j];
                     p.texcoordindex = j + (i + 1) * (latitude + 1);
                     f.pointdata[3] = p;
                  }
                  else
                  {
                     pointdata p = new pointdata();
                     p.pointindex = indices[i, j];
                     p.texcoordindex = j + i * (latitude + 1);
                     f.pointdata[0] = p;

                     p = new pointdata();
                     p.pointindex = indices[i, j + 1];
                     p.texcoordindex = j + 1 + i * (latitude + 1);
                     f.pointdata[1] = p;

                     p = new pointdata();
                     p.pointindex = indices[0, j + 1];
                     p.texcoordindex = j + 1 + (i + 1) * (latitude + 1);
                     f.pointdata[2] = p;

                     p = new pointdata();
                     p.pointindex = indices[0, j];
                     p.texcoordindex = j + (i + 1) * (latitude + 1);
                     f.pointdata[3] = p;
                  }

                  facedatas.Add(f);
               }
            }
         }

         if ( points.Count > 0 )
         {
            m.points = new points();
            m.points.point = points.ToArray();
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
