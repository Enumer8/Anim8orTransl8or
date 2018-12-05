using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Sphere
   {
      internal static mesh Convert(sphere s)
      {
         // Convert the sphere to a mesh
         mesh m = new mesh();
         m.name = s.name;
         m.@base = s.@base;
         m.pivot = s.pivot;
         m.material = s.material;

         if ( s.material?.name != null )
         {
            m.materiallist = new materiallist();
            m.materiallist.materialname = new @string[1];
            m.materiallist.materialname[0] = new @string();
            m.materiallist.materialname[0].text = s.material.name;
         }

         List<point> points = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // Note: Anim8or v1.00 prioritizes longlat over geodesic.
         if ( s.longlat != null )
         {
            // Note: If diameter is 0 or longitude is 1 or less or latitude is
            // 1 or less, then there's nothing to draw. Technically Anim8or
            // v1.00 may draw lines.
            if ( s.diameter?.text > 0 &&
                 s.longlat.longitude > 1 &&
                 s.longlat.latitude > 1 )
            {
               Double diameter = s.diameter.text;
               // Note: Anim8or v1.00 limits longitude to 32.
               Int64 longitude = Math.Min(s.longlat.longitude, 32);
               // Note: Anim8or v1.00 limits latitude to 16.
               Int64 latitude = Math.Min(s.longlat.latitude, 16);
               Double stepAngleX = -Math.PI / latitude;
               Double stepAngleY = 2 * Math.PI / longitude;
               point startPoint = new point(0, diameter / 2);

               for ( Int64 i = 1; i < latitude; i++ )
               {
                  quaternion q = new quaternion(
                     0,
                     0,
                     Math.Sin(stepAngleX * i / 2),
                     Math.Cos(stepAngleX * i / 2));

                  point p = q.Rotate(startPoint);

                  for ( Int64 j = 0; j < longitude; j++ )
                  {
                     q.y = Math.Sin(stepAngleY * j / 2);
                     q.z = 0;
                     q.w = Math.Cos(stepAngleY * j / 2);

                     point p2 = q.Rotate(p);
                     points.Add(p2);

                     texcoord t = new texcoord(
                        (Double)j / longitude,
                        1 - (Double)i / latitude);

                     texcoords.Add(t);
                  }
               }

               // Top point
               point topPoint = new point(0, diameter / 2);
               points.Add(topPoint);

               // Bottom point
               point bottomPoint = new point(0, -diameter / 2);
               points.Add(bottomPoint);

               for ( Int64 i = 0; i < latitude - 2; i++ )
               {
                  for ( Int64 j = 0; j < longitude; j++ )
                  {
                     facedata f = new facedata();
                     f.numpoints = 4;
                     f.flags = facedataenum.hastexture;
                     f.matno = 0;
                     f.flatnormalno = -1;
                     f.pointdata = new pointdata[f.numpoints];

                     // pointdata0
                     f.pointdata[0] = new pointdata();
                     f.pointdata[0].pointindex = i * longitude + j;
                     f.pointdata[0].texcoordindex = f.pointdata[0].pointindex;

                     // pointdata1
                     if ( j == longitude - 1 )
                     {
                        f.pointdata[1] = new pointdata();
                        f.pointdata[1].pointindex = i * longitude;

                        // TexCoord
                        texcoord t = texcoords[
                           (Int32)f.pointdata[1].pointindex];
                        t.u = 1;

                        f.pointdata[1].texcoordindex = texcoords.Count;
                        texcoords.Add(t);
                     }
                     else
                     {
                        f.pointdata[1] = new pointdata();
                        f.pointdata[1].pointindex = i * longitude + j + 1;
                        f.pointdata[1].texcoordindex =
                           f.pointdata[1].pointindex;
                     }

                     // pointdata2
                     if ( j == longitude - 1 )
                     {
                        f.pointdata[2] = new pointdata();
                        f.pointdata[2].pointindex = (i + 1) * longitude;

                        // TexCoord
                        texcoord t = texcoords[
                           (Int32)f.pointdata[2].pointindex];
                        t.u = 1;

                        f.pointdata[2].texcoordindex = texcoords.Count;
                        texcoords.Add(t);
                     }
                     else
                     {
                        f.pointdata[2] = new pointdata();
                        f.pointdata[2].pointindex =
                           (i + 1) * longitude + j + 1;
                        f.pointdata[2].texcoordindex =
                           f.pointdata[2].pointindex;
                     }

                     // pointdata3
                     f.pointdata[3] = new pointdata();
                     f.pointdata[3].pointindex = (i + 1) * longitude + j;
                     f.pointdata[3].texcoordindex =
                        f.pointdata[3].pointindex;

                     facedatas.Add(f);
                  }
               }

               // Top and bottom faces
               for ( Int64 i = 0; i < 2; i++ )
               {
                  for ( Int64 j = 0; j < longitude; j++ )
                  {
                     facedata f = new facedata();
                     f.numpoints = 3;
                     f.flags = facedataenum.hastexture;
                     f.matno = 0;
                     f.flatnormalno = -1;
                     f.pointdata = new pointdata[f.numpoints];

                     // pointdata0
                     pointdata p0 = new pointdata();
                     p0.pointindex = i * longitude * (latitude - 2) + j;
                     p0.texcoordindex = p0.pointindex;

                     // pointdata1
                     pointdata p1 = new pointdata();

                     if ( j == longitude - 1 )
                     {
                        p1.pointindex = i * longitude * (latitude - 2);

                        // TexCoord
                        texcoord t = texcoords[
                           (Int32)p1.pointindex];
                        t.u = 1;

                        p1.texcoordindex = texcoords.Count;
                        texcoords.Add(t);
                     }
                     else
                     {
                        p1.pointindex = i * longitude * (latitude - 2) + j + 1;
                        p1.texcoordindex = p1.pointindex;
                     }

                     // pointdata2
                     pointdata p2 = new pointdata();
                     p2.pointindex = longitude * (latitude - 1) + i;

                     // Top TexCoord
                     texcoord topTexCoord = new texcoord(
                        (Double)j / longitude,
                        1 - i);

                     p2.texcoordindex = texcoords.Count;
                     texcoords.Add(topTexCoord);

                     if ( i != 0 )
                     {
                        f.pointdata[0] = p0;
                        f.pointdata[1] = p1;
                        f.pointdata[2] = p2;
                     }
                     else
                     {
                        f.pointdata[0] = p0;
                        f.pointdata[1] = p2;
                        f.pointdata[2] = p1;
                     }

                     facedatas.Add(f);
                  }
               }
            }
         }
         else if ( s.geodesic != null )
         {
            // Note: If diameter is 0 or geodesic is 0, then there's nothing to
            // draw.
            if ( s.diameter?.text > 0 && s.geodesic.text > 0 )
            {
               Double diameter = s.diameter.text;
               // Note: Anim8or v1.00 limits geodesic to 32 (the editor limits
               // to 6).
               Int64 geodesic = Math.Min(s.geodesic.text, 32);

               // Indices
               Int32[] topFront = new Int32[geodesic + 1];
               Int32[] topBack = new Int32[geodesic + 1];
               Int32[] topRight = new Int32[geodesic + 1];
               Int32[] topLeft = new Int32[geodesic + 1];

               Int32[] bottomFront = new Int32[geodesic + 1];
               Int32[] bottomBack = new Int32[geodesic + 1];
               Int32[] bottomRight = new Int32[geodesic + 1];
               Int32[] bottomLeft = new Int32[geodesic + 1];

               Int32[] rightFront = new Int32[geodesic + 1];
               Int32[] rightBack = new Int32[geodesic + 1];
               Int32[] leftFront = new Int32[geodesic + 1];
               Int32[] leftBack = new Int32[geodesic + 1];

               // UV top
               Int32[] uvTopFront0 = new Int32[geodesic + 1];
               Int32[] uvTopRight0 = new Int32[geodesic + 1];
               Int32[] uvRightFront0 = new Int32[geodesic + 1];

               Int32[] uvTopRight1 = new Int32[geodesic + 1];
               Int32[] uvTopBack1 = new Int32[geodesic + 1];
               Int32[] uvRightBack1 = new Int32[geodesic + 1];

               Int32[] uvTopLeft2 = new Int32[geodesic + 1];
               Int32[] uvTopBack2 = new Int32[geodesic + 1];
               Int32[] uvLeftBack2 = new Int32[geodesic + 1];

               Int32[] uvTopLeft3 = new Int32[geodesic + 1];
               Int32[] uvTopFront3 = new Int32[geodesic + 1];
               Int32[] uvLeftFront3 = new Int32[geodesic + 1];

               // Uv bottom
               Int32[] uvBottomFront4 = new Int32[geodesic + 1];
               Int32[] uvBottomRight4 = new Int32[geodesic + 1];
               //Int32[] uvRightFront4 = new Int32[geodesic + 1];

               Int32[] uvBottomRight5 = new Int32[geodesic + 1];
               Int32[] uvBottomBack5 = new Int32[geodesic + 1];
               //Int32[] uvRightBack5 = new Int32[geodesic + 1];

               Int32[] uvBottomLeft6 = new Int32[geodesic + 1];
               Int32[] uvBottomBack6 = new Int32[geodesic + 1];
               //Int32[] uvLeftFront6 = new Int32[geodesic + 1];

               Int32[] uvBottomLeft7 = new Int32[geodesic + 1];
               Int32[] uvBottomFront7 = new Int32[geodesic + 1];
               //Int32[] uvLeftBack7 = new Int32[geodesic + 1];

               Double radius = 1 /*diameter / 2*/;

               // Points of level 0

               // Point top
               point p = new point(0, radius, 0);

               topFront[0] = points.Count;
               topBack[0] = points.Count;
               topRight[0] = points.Count;
               topLeft[0] = points.Count;
               points.Add(p);

               texcoord t = new texcoord(0.875, 1);

               uvTopFront0[0] = texcoords.Count;
               uvTopRight0[0] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(0.125, 1);

               uvTopRight1[0] = texcoords.Count;
               uvTopBack1[0] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(0.375, 1);

               uvTopLeft2[0] = texcoords.Count;
               uvTopBack2[0] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(0.625, 1);

               uvTopLeft3[0] = texcoords.Count;
               uvTopFront3[0] = texcoords.Count;
               texcoords.Add(t);

               // Point bottom
               p = new point(0, -radius, 0);

               bottomFront[0] = points.Count;
               bottomBack[0] = points.Count;
               bottomRight[0] = points.Count;
               bottomLeft[0] = points.Count;
               points.Add(p);

               t = new texcoord(0.875, 0);

               uvBottomFront4[0] = texcoords.Count;
               uvBottomRight4[0] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(0.125, 0);

               uvBottomRight5[0] = texcoords.Count;
               uvBottomBack5[0] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(0.375, 0);

               uvBottomLeft6[0] = texcoords.Count;
               uvBottomBack6[0] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(0.625, 0);

               uvBottomLeft7[0] = texcoords.Count;
               uvBottomFront7[0] = texcoords.Count;
               texcoords.Add(t);

               // Point back
               p = new point(0, 0, -radius);

               topBack[geodesic] = points.Count;
               rightBack[geodesic] = points.Count;
               leftBack[0] = points.Count;
               bottomBack[geodesic] = points.Count;
               points.Add(p);

               t = new texcoord(0.25, 0.5);

               uvTopBack1[geodesic] = texcoords.Count;
               uvTopBack2[geodesic] = texcoords.Count;
               uvBottomBack5[geodesic] = texcoords.Count;
               uvBottomBack6[geodesic] = texcoords.Count;
               uvRightBack1[geodesic] = texcoords.Count;
               uvLeftBack2[0] = texcoords.Count;
               texcoords.Add(t);

               // Point front
               p = new point(0, 0, radius);

               topFront[geodesic] = points.Count;
               bottomFront[geodesic] = points.Count;
               leftFront[geodesic] = points.Count;
               rightFront[0] = points.Count;
               points.Add(p);

               t = new texcoord(0.75, 0.5);

               uvTopFront0[geodesic] = texcoords.Count;
               uvTopFront3[geodesic] = texcoords.Count;
               uvBottomFront4[geodesic] = texcoords.Count;
               uvBottomFront7[geodesic] = texcoords.Count;
               uvRightFront0[0] = texcoords.Count;
               uvLeftFront3[geodesic] = texcoords.Count;
               texcoords.Add(t);

               // Point right
               p = new point(radius, 0, 0);

               topRight[geodesic] = points.Count;
               bottomRight[geodesic] = points.Count;
               rightBack[0] = points.Count;
               rightFront[geodesic] = points.Count;
               points.Add(p);

               t = new texcoord(0, 0.5);

               uvTopRight1[geodesic] = texcoords.Count;
               uvRightBack1[0] = texcoords.Count;
               uvBottomRight5[geodesic] = texcoords.Count;
               texcoords.Add(t);

               t = new texcoord(1, 0.5);

               uvTopRight0[geodesic] = texcoords.Count;
               uvRightFront0[geodesic] = texcoords.Count;
               uvBottomRight4[geodesic] = texcoords.Count;
               texcoords.Add(t);

               // Point left
               p = new point(-radius, 0, 0);

               topLeft[geodesic] = points.Count;
               bottomLeft[geodesic] = points.Count;
               leftBack[geodesic] = points.Count;
               leftFront[0] = points.Count;
               points.Add(p);

               t = new texcoord(0.5, 0.5);

               uvTopLeft2[geodesic] = texcoords.Count;
               uvTopLeft3[geodesic] = texcoords.Count;
               uvLeftBack2[geodesic] = texcoords.Count;
               uvLeftFront3[0] = texcoords.Count;
               uvBottomLeft6[geodesic] = texcoords.Count;
               uvBottomLeft7[geodesic] = texcoords.Count;
               texcoords.Add(t);

               // Create points on the edges of the initial octahedron
               for ( Int64 i = 1; i < geodesic; i++ )
               {
                  Double t0 = (Double)i / geodesic;
                  Double t1 = 1 - t0;

                  // Top front
                  p = new point(0, t1, t0);

                  topFront[i] = points.Count;
                  points.Add(p);

                  point vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvTopFront0[i] = texcoords.Count;
                  uvTopFront3[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Top right
                  p = new point(t0, t1, 0);

                  topRight[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);
                  t.u = 1;

                  uvTopRight0[i] = texcoords.Count;
                  texcoords.Add(t);

                  t.v = 0;

                  uvTopRight1[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Top back
                  p = new point(0, t1, -t0);

                  topBack[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvTopBack1[i] = texcoords.Count;
                  uvTopBack2[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Top left
                  p = new point(-t0, t1, 0);

                  topLeft[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();

                  t = SphericalTexCoordFromNormal(vec3);

                  uvTopLeft2[i] = texcoords.Count;
                  uvTopLeft3[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Bottom front
                  p = new point(0, -t1, t0);

                  bottomFront[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvBottomFront4[i] = texcoords.Count;
                  uvBottomFront7[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Bottom right
                  p = new point(t0, -t1, 0);

                  bottomRight[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);
                  t.u = 1;

                  uvBottomRight4[i] = texcoords.Count;
                  texcoords.Add(t);

                  t.v = 0;

                  uvBottomRight5[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Bottom back
                  p = new point(0, -t1, -t0);

                  bottomBack[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvBottomBack5[i] = texcoords.Count;
                  uvBottomBack6[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Bottom left
                  p = new point(-t0, -t1, 0);

                  bottomLeft[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvBottomLeft6[i] = texcoords.Count;
                  uvBottomLeft7[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Right front
                  p = new point(t0, 0, t1);

                  rightFront[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvRightFront0[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Right back
                  p = new point(t1, 0, -t0);

                  rightBack[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvRightBack1[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Left back
                  p = new point(-t0, 0, -t1);

                  leftBack[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvLeftBack2[i] = texcoords.Count;
                  texcoords.Add(t);

                  // Left front
                  p = new point(-t1, 0, t0);

                  leftFront[i] = points.Count;
                  points.Add(p);

                  vec3 = points[points.Count - 1].Normalize();
                  t = SphericalTexCoordFromNormal(vec3);

                  uvLeftFront3[i] = texcoords.Count;
                  texcoords.Add(t);
               }

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  topFront,
                  topRight,
                  rightFront,
                  uvTopFront0,
                  uvTopRight0,
                  uvRightFront0,
                  true);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  topLeft,
                  topFront,
                  leftFront,
                  uvTopLeft3,
                  uvTopFront3,
                  uvLeftFront3,
                  true);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  topRight,
                  topBack,
                  rightBack,
                  uvTopRight1,
                  uvTopBack1,
                  uvRightBack1,
                  true);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  topBack,
                  topLeft,
                  leftBack,
                  uvTopBack2,
                  uvTopLeft2,
                  uvLeftBack2,
                  true);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  bottomFront,
                  bottomRight,
                  rightFront,
                  uvBottomFront4,
                  uvBottomRight4,
                  uvRightFront0,
                  false);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  bottomLeft,
                  bottomFront,
                  leftFront,
                  uvBottomLeft7,
                  uvBottomFront7,
                  uvLeftFront3,
                  false);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  bottomRight,
                  bottomBack,
                  rightBack,
                  uvBottomRight5,
                  uvBottomBack5,
                  uvRightBack1,
                  false);

               AddGeodesicFaces(
                  geodesic,
                  points,
                  texcoords,
                  facedatas,
                  bottomBack,
                  bottomLeft,
                  leftBack,
                  uvBottomBack6,
                  uvBottomLeft6,
                  uvLeftBack2,
                  false);

               radius = diameter / 2;

               for ( Int64 i = 0; i < points.Count; ++i )
               {
                  points[(Int32)i] = points[(Int32)i].Normalize() * radius;
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

      static texcoord SphericalTexCoordFromNormal(point p)
      {
         point vecA = new point(1, 0, 0);
         point vecB = new point(p.x, 0, p.z).Normalize();

         Double cosAngle = vecB.x;
         Double angle = Math.Acos(cosAngle);

         texcoord t = new texcoord();

         if ( p.z < 0 )
         {
            t.u = angle / (2 * Math.PI);
         }
         else
         {
            t.u = 1 - angle / (2 * Math.PI);
         }

         t.v = p.y / 2 + 0.5;

         return t;
      }

      static void AddGeodesicFaces(
         Int64 geodesic,
         List<point> points,
         List<texcoord> texcoords,
         List<facedata> facedatas,
         Int32[] left,
         Int32[] right,
         Int32[] bottom,
         Int32[] uvLeft,
         Int32[] uvRight,
         Int32[] uvBottom,
         Boolean faceOrder)
      {
         Int32[,] level = new Int32[geodesic + 1, geodesic + 1];
         Int32[,] uvlevel = new Int32[geodesic + 1, geodesic + 1];

         // Create new vertices + copy indices in level
         for ( Int64 v = 0; v <= geodesic; v++ )
         {
            level[v, 0] = left[v];
            uvlevel[v, 0] = uvLeft[v];
         }

         for ( Int64 v = 0; v <= geodesic; v++ )
         {
            level[v, v] = right[v];
            uvlevel[v, v] = uvRight[v];
         }

         for ( Int64 h = 1; h < geodesic; h++ )
         {
            level[geodesic, h] = bottom[h];
            uvlevel[geodesic, h] = uvBottom[h];
         }

         for ( Int64 v = 2; v < geodesic; ++v )
         {
            point vec = points[level[v, v]] - points[level[v, 0]];

            Double length = vec.GetLength();
            vec = vec / length; // vec.Normalize();

            texcoord t = texcoords[uvlevel[v, v]] - texcoords[uvlevel[v, 0]];

            Double lengthUV = t.GetLength();
            t = t / lengthUV; // t.Normalize();

            for ( Int64 h = 1; h < v; h++ )
            {
               point p = points[level[v, 0]] + vec * (h * length / v);

               level[v, h] = points.Count;
               points.Add(p);

               texcoord newTexcoord = texcoords[uvlevel[v, 0]] +
                  t * (h * lengthUV / v);

               uvlevel[v, h] = texcoords.Count - 1;
               texcoords.Add(newTexcoord);
            }
         }

         for ( Int64 v = 0; v < geodesic; v++ )
         {
            for ( Int64 i = 0; i < v + 1; i++ )
            {
               facedata f = new facedata();
               f.numpoints = 3;
               f.flags = facedataenum.hastexture;
               f.matno = 0;
               f.flatnormalno = -1;
               f.pointdata = new pointdata[f.numpoints];

               // pointdata0
               pointdata p0 = new pointdata();
               p0.pointindex = level[v, i];
               p0.texcoordindex = uvlevel[v, i];

               // pointdata1
               pointdata p1 = new pointdata();
               p1.pointindex = level[v + 1, i];
               p1.texcoordindex = uvlevel[v + 1, i];

               // pointdata2
               pointdata p2 = new pointdata();
               p2.pointindex = level[v + 1, i + 1];
               p2.texcoordindex = uvlevel[v + 1, i + 1];

               if ( faceOrder == false )
               {
                  f.pointdata[0] = p0;
                  f.pointdata[1] = p1;
                  f.pointdata[2] = p2;
               }
               else
               {
                  f.pointdata[0] = p0;
                  f.pointdata[1] = p2;
                  f.pointdata[2] = p1;
               }

               facedatas.Add(f);
            }

            for ( Int64 i = 0; i < v; i++ )
            {
               facedata f = new facedata();
               f.numpoints = 3;
               f.flags = facedataenum.hastexture;
               f.matno = 0;
               f.flatnormalno = -1;
               f.pointdata = new pointdata[f.numpoints];

               // pointdata0
               pointdata p0 = new pointdata();
               p0.pointindex = level[v, i];
               p0.texcoordindex = uvlevel[v, i];

               // pointdata1
               pointdata p1 = new pointdata();
               p1.pointindex = level[v + 1, i + 1];
               p1.texcoordindex = uvlevel[v + 1, i + 1];

               // pointdata2
               pointdata p2 = new pointdata();
               p2.pointindex = level[v, i + 1];
               p2.texcoordindex = uvlevel[v, i + 1];

               if ( faceOrder == false )
               {
                  f.pointdata[0] = p0;
                  f.pointdata[1] = p1;
                  f.pointdata[2] = p2;
               }
               else
               {
                  f.pointdata[0] = p0;
                  f.pointdata[1] = p2;
                  f.pointdata[2] = p1;
               }

               facedatas.Add(f);
            }
         }
      }
   }
}
