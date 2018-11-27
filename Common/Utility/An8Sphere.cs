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
         List<point> normals = new List<point>();
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
               quaternion quaternion = new quaternion();
               point startPoint = new point() { y = diameter / 2 };

               for ( Int64 i = 1; i < latitude; i++ )
               {
                  quaternion.y = 0;
                  quaternion.z = Math.Sin(stepAngleX * i / 2);
                  quaternion.w = Math.Cos(stepAngleX * i / 2);

                  point point = An8Math.Rotate(quaternion, startPoint);

                  for ( Int64 j = 0; j < longitude; j++ )
                  {
                     quaternion.y = Math.Sin(stepAngleY * j / 2);
                     quaternion.z = 0;
                     quaternion.w = Math.Cos(stepAngleY * j / 2);

                     // Point
                     point newPoint = An8Math.Rotate(quaternion, point);
                     points.Add(newPoint);

                     // Normal
                     point newNormal = An8Math.Normalize(newPoint);
                     normals.Add(newNormal);

                     // TexCoord
                     texcoord newTexCoord = new texcoord();
                     newTexCoord.u = (Double)j / longitude;
                     newTexCoord.v = 1 - (Double)i / latitude;
                     texcoords.Add(newTexCoord);
                  }
               }

               // Top point
               point topPoint = new point();
               topPoint.y = diameter / 2;
               points.Add(topPoint);

               // Top normal
               point topNormal = An8Math.Normalize(topPoint);
               normals.Add(topNormal);

               // Bottom point
               point bottomPoint = new point();
               bottomPoint.y = -diameter / 2;
               points.Add(bottomPoint);

               // Bottom normal
               point bottomNormal = An8Math.Normalize(bottomPoint);
               normals.Add(bottomNormal);

               for ( Int64 i = 0; i < latitude - 2; i++ )
               {
                  for ( Int64 j = 0; j < longitude; j++ )
                  {
                     facedata newFace = new facedata();
                     newFace.numpoints = 4;
                     newFace.flags =
                        facedataenum.hasnormals | facedataenum.hastexture;
                     newFace.matno = 0;
                     newFace.flatnormalno = 0;
                     newFace.pointdata = new pointdata[newFace.numpoints];

                     // pointdata0
                     newFace.pointdata[0] = new pointdata();
                     newFace.pointdata[0].pointindex = i * longitude + j;
                     newFace.pointdata[0].normalindex =
                        newFace.pointdata[0].pointindex;
                     newFace.pointdata[0].texcoordindex =
                        newFace.pointdata[0].pointindex;

                     // pointdata1
                     if ( j == longitude - 1 )
                     {
                        newFace.pointdata[1] = new pointdata();
                        newFace.pointdata[1].pointindex = i * longitude;
                        newFace.pointdata[1].normalindex =
                           newFace.pointdata[1].pointindex;

                        // TexCoord
                        texcoord newTexCoord = texcoords[
                           (Int32)newFace.pointdata[1].pointindex];
                        newTexCoord.u = 1;
                        texcoords.Add(newTexCoord);
                        newFace.pointdata[1].texcoordindex =
                           texcoords.Count - 1;
                     }
                     else
                     {
                        newFace.pointdata[1] = new pointdata();
                        newFace.pointdata[1].pointindex =
                           i * longitude + j + 1;
                        newFace.pointdata[1].normalindex =
                           newFace.pointdata[1].pointindex;
                        newFace.pointdata[1].texcoordindex =
                           newFace.pointdata[1].pointindex;
                     }

                     // pointdata2
                     if ( j == longitude - 1 )
                     {
                        newFace.pointdata[2] = new pointdata();
                        newFace.pointdata[2].pointindex = (i + 1) * longitude;
                        newFace.pointdata[2].normalindex =
                           newFace.pointdata[2].pointindex;

                        // TexCoord
                        texcoord newTexCoord = texcoords[
                           (Int32)newFace.pointdata[2].pointindex];
                        newTexCoord.u = 1;
                        texcoords.Add(newTexCoord);
                        newFace.pointdata[2].texcoordindex =
                           texcoords.Count - 1;
                     }
                     else
                     {
                        newFace.pointdata[2] = new pointdata();
                        newFace.pointdata[2].pointindex =
                           (i + 1) * longitude + j + 1;
                        newFace.pointdata[2].normalindex =
                           newFace.pointdata[2].pointindex;
                        newFace.pointdata[2].texcoordindex =
                           newFace.pointdata[2].pointindex;
                     }

                     // pointdata3
                     newFace.pointdata[3] = new pointdata();
                     newFace.pointdata[3].pointindex = (i + 1) * longitude + j;
                     newFace.pointdata[3].normalindex =
                        newFace.pointdata[3].pointindex;
                     newFace.pointdata[3].texcoordindex =
                        newFace.pointdata[3].pointindex;

                     facedatas.Add(newFace);
                  }
               }

               // Top and bottom faces
               for ( Int64 i = 0; i < 2; i++ )
               {
                  for ( Int64 j = 0; j < longitude; j++ )
                  {
                     facedata newFace = new facedata();
                     newFace.numpoints = 3;
                     newFace.flags =
                        facedataenum.hasnormals | facedataenum.hastexture;
                     newFace.matno = 0;
                     newFace.flatnormalno = 0;
                     newFace.pointdata = new pointdata[newFace.numpoints];

                     // pointdata0
                     pointdata pointdata0 = new pointdata();
                     pointdata0.pointindex =
                        i * longitude * (latitude - 2) + j;
                     pointdata0.normalindex = pointdata0.pointindex;
                     pointdata0.texcoordindex = pointdata0.pointindex;

                     // pointdata1
                     pointdata pointdata1 = new pointdata();

                     if ( j == longitude - 1 )
                     {
                        pointdata1.pointindex = i * longitude * (latitude - 2);
                        pointdata1.normalindex = pointdata1.pointindex;

                        // TexCoord
                        texcoord newTexCoord = texcoords[
                           (Int32)pointdata1.pointindex];
                        newTexCoord.u = 1;
                        texcoords.Add(newTexCoord);
                        pointdata1.texcoordindex = texcoords.Count - 1;
                     }
                     else
                     {
                        pointdata1.pointindex =
                           i * longitude * (latitude - 2) + j + 1;
                        pointdata1.normalindex = pointdata1.pointindex;
                        pointdata1.texcoordindex = pointdata1.pointindex;
                     }

                     // pointdata2
                     pointdata pointdata2 = new pointdata();
                     pointdata2.pointindex = longitude * (latitude - 1) + i;
                     pointdata2.normalindex = pointdata2.pointindex;

                     // Top TexCoord
                     texcoord topTexCoord = new texcoord();
                     topTexCoord.u = (Double)j / longitude;
                     topTexCoord.v = 1 - i;
                     texcoords.Add(topTexCoord);
                     pointdata2.texcoordindex = texcoords.Count - 1;

                     if ( i != 0 )
                     {
                        newFace.pointdata[0] = pointdata0;
                        newFace.pointdata[1] = pointdata1;
                        newFace.pointdata[2] = pointdata2;
                     }
                     else
                     {
                        newFace.pointdata[0] = pointdata0;
                        newFace.pointdata[1] = pointdata2;
                        newFace.pointdata[2] = pointdata1;
                     }

                     facedatas.Add(newFace);
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

               // Vertex indices
               Int64[] TopFront = new Int64[geodesic + 1];
               Int64[] TopBack = new Int64[geodesic + 1];
               Int64[] TopRight = new Int64[geodesic + 1];
               Int64[] TopLeft = new Int64[geodesic + 1];

               Int64[] BottomFront = new Int64[geodesic + 1];
               Int64[] BottomBack = new Int64[geodesic + 1];
               Int64[] BottomRight = new Int64[geodesic + 1];
               Int64[] BottomLeft = new Int64[geodesic + 1];

               Int64[] RightFront = new Int64[geodesic + 1];
               Int64[] RightBack = new Int64[geodesic + 1];
               Int64[] LeftFront = new Int64[geodesic + 1];
               Int64[] LeftBack = new Int64[geodesic + 1];

               // UV top
               Int64[] uvTopFront0 = new Int64[geodesic + 1];
               Int64[] uvTopRight0 = new Int64[geodesic + 1];
               Int64[] uvRightFront0 = new Int64[geodesic + 1];

               Int64[] uvTopRight1 = new Int64[geodesic + 1];
               Int64[] uvTopBack1 = new Int64[geodesic + 1];
               Int64[] uvRightBack1 = new Int64[geodesic + 1];

               Int64[] uvTopLeft2 = new Int64[geodesic + 1];
               Int64[] uvTopBack2 = new Int64[geodesic + 1];
               Int64[] uvLeftBack2 = new Int64[geodesic + 1];

               Int64[] uvTopLeft3 = new Int64[geodesic + 1];
               Int64[] uvTopFront3 = new Int64[geodesic + 1];
               Int64[] uvLeftFront3 = new Int64[geodesic + 1];

               // Uv bottom
               Int64[] uvBottomFront4 = new Int64[geodesic + 1];
               Int64[] uvBottomRight4 = new Int64[geodesic + 1];
               //Int64[] uvRightFront4 = new Int64[geodesic + 1];

               Int64[] uvBottomRight5 = new Int64[geodesic + 1];
               Int64[] uvBottomBack5 = new Int64[geodesic + 1];
               //Int64[] uvRightBack5 = new Int64[geodesic + 1];

               Int64[] uvBottomLeft6 = new Int64[geodesic + 1];
               Int64[] uvBottomBack6 = new Int64[geodesic + 1];
               //Int64[] uvLeftFront6 = new Int64[geodesic + 1];

               Int64[] uvBottomLeft7 = new Int64[geodesic + 1];
               Int64[] uvBottomFront7 = new Int64[geodesic + 1];
               //Int64[] uvLeftBack7 = new Int64[geodesic + 1];

               Double radius = 1 /*diameter / 2*/;

               // Points of Level 0 ///////////////////////////////////////////

               // Point Top ///////////////////////////////////////////////////
               point newPoint = new point() { y = radius };
               points.Add(newPoint);

               TopFront[0] = points.Count - 1;
               TopBack[0] = points.Count - 1;
               TopRight[0] = points.Count - 1;
               TopLeft[0] = points.Count - 1;

               texcoord newUV = new texcoord() { u = 0.875, v = 1 };
               texcoords.Add(newUV);
               uvTopFront0[0] = texcoords.Count - 1;
               uvTopRight0[0] = texcoords.Count - 1;

               newUV = new texcoord() { u = 0.125, v = 1 };
               texcoords.Add(newUV);
               uvTopRight1[0] = texcoords.Count - 1;
               uvTopBack1[0] = texcoords.Count - 1;

               newUV = new texcoord() { u = 0.375, v = 1 };
               texcoords.Add(newUV);
               uvTopLeft2[0] = texcoords.Count - 1;
               uvTopBack2[0] = texcoords.Count - 1;

               newUV = new texcoord() { u = 0.625, v = 1 };
               texcoords.Add(newUV);
               uvTopLeft3[0] = texcoords.Count - 1;
               uvTopFront3[0] = texcoords.Count - 1;

               // Point Bottom ////////////////////////////////////////////////
               newPoint = new point() { y = -radius };
               points.Add(newPoint);

               BottomFront[0] = points.Count - 1;
               BottomBack[0] = points.Count - 1;
               BottomRight[0] = points.Count - 1;
               BottomLeft[0] = points.Count - 1;

               newUV = new texcoord() { u = 0.875, v = 0 };
               texcoords.Add(newUV);
               uvBottomFront4[0] = texcoords.Count - 1;
               uvBottomRight4[0] = texcoords.Count - 1;

               newUV = new texcoord() { u = 0.125, v = 0 };
               texcoords.Add(newUV);
               uvBottomRight5[0] = texcoords.Count - 1;
               uvBottomBack5[0] = texcoords.Count - 1;

               newUV = new texcoord() { u = 0.375, v = 0 };
               texcoords.Add(newUV);
               uvBottomLeft6[0] = texcoords.Count - 1;
               uvBottomBack6[0] = texcoords.Count - 1;

               newUV = new texcoord() { u = 0.625, v = 0 };
               texcoords.Add(newUV);
               uvBottomLeft7[0] = texcoords.Count - 1;
               uvBottomFront7[0] = texcoords.Count - 1;

               // Point Back //////////////////////////////////////////////////
               newPoint = new point() { z = -radius };
               points.Add(newPoint);

               TopBack[geodesic] = points.Count - 1;
               RightBack[geodesic] = points.Count - 1;
               LeftBack[0] = points.Count - 1;
               BottomBack[geodesic] = points.Count - 1;

               newUV = new texcoord() { u = 0.25, v = 0.5 };
               texcoords.Add(newUV);
               uvTopBack1[geodesic] = texcoords.Count - 1;
               uvTopBack2[geodesic] = texcoords.Count - 1;

               uvBottomBack5[geodesic] = texcoords.Count - 1;
               uvBottomBack6[geodesic] = texcoords.Count - 1;

               uvRightBack1[geodesic] = texcoords.Count - 1;
               uvLeftBack2[0] = texcoords.Count - 1;

               // Point Front /////////////////////////////////////////////////
               newPoint = new point() { z = radius };
               points.Add(newPoint);

               TopFront[geodesic] = points.Count - 1;
               BottomFront[geodesic] = points.Count - 1;
               LeftFront[geodesic] = points.Count - 1;
               RightFront[0] = points.Count - 1;

               newUV = new texcoord() { u = 0.75, v = 0.5 };
               texcoords.Add(newUV);
               uvTopFront0[geodesic] = texcoords.Count - 1;
               uvTopFront3[geodesic] = texcoords.Count - 1;

               uvBottomFront4[geodesic] = texcoords.Count - 1;
               uvBottomFront7[geodesic] = texcoords.Count - 1;

               uvRightFront0[0] = texcoords.Count - 1;
               uvLeftFront3[geodesic] = texcoords.Count - 1;

               // Point Right /////////////////////////////////////////////////
               newPoint = new point() { x = radius };
               points.Add(newPoint);

               TopRight[geodesic] = points.Count - 1;
               BottomRight[geodesic] = points.Count - 1;
               RightBack[0] = points.Count - 1;
               RightFront[geodesic] = points.Count - 1;

               newUV = new texcoord() { v = 0.5 };
               texcoords.Add(newUV);
               uvTopRight1[geodesic] = texcoords.Count - 1;
               uvRightBack1[0] = texcoords.Count - 1;
               uvBottomRight5[geodesic] = texcoords.Count - 1;

               newUV = new texcoord() { u = 1, v = 0.5 };
               texcoords.Add(newUV);
               uvTopRight0[geodesic] = texcoords.Count - 1;
               uvRightFront0[geodesic] = texcoords.Count - 1;
               uvBottomRight4[geodesic] = texcoords.Count - 1;

               // Point Left //////////////////////////////////////////////////
               newPoint = new point() { x = -radius };
               points.Add(newPoint);

               TopLeft[geodesic] = points.Count - 1;
               BottomLeft[geodesic] = points.Count - 1;
               LeftBack[geodesic] = points.Count - 1;
               LeftFront[0] = points.Count - 1;

               newUV = new texcoord() { u = 0.5, v = 0.5 };
               texcoords.Add(newUV);
               uvTopLeft2[geodesic] = texcoords.Count - 1;
               uvTopLeft3[geodesic] = texcoords.Count - 1;

               uvLeftBack2[geodesic] = texcoords.Count - 1;
               uvLeftFront3[0] = texcoords.Count - 1;

               uvBottomLeft6[geodesic] = texcoords.Count - 1;
               uvBottomLeft7[geodesic] = texcoords.Count - 1;

               ////////////////////////////////////////////////////////////////
               // Create points on the edges of the initial octahedron
               for ( Int64 i = 1; i < geodesic; i++ )
               {
                  texcoord SphericalTexCoordFromNormal(point p)
                  {
                     point @ref = new point() { x = 1 };
                     point vec3b = new point() { x = p.x, z = p.z };

                     point vecA = @ref;
                     point vecB = An8Math.Normalize(vec3b);

                     Double cosAngle = vecA.x * vecB.x + vecA.y * vecB.y + vecA.z * vecB.z;
                     Double angle = Math.Acos(cosAngle);

                     texcoord texcoord = new texcoord();

                     if ( p.z < 0 )
                     {
                        texcoord.u = angle / (2 * Math.PI);
                     }
                     else
                     {
                        texcoord.u = 1 - angle / (2 * Math.PI);
                     }

                     texcoord.v = p.y / 2 + 0.5;
                     return texcoord;
                  }

                  Double t = (Double)i / geodesic;
                  Double t1 = 1 - t;

                  // TopFront /////////////////////////////////////////////////
                  newPoint = new point() { y = t1, z = t };
                  points.Add(newPoint);

                  TopFront[i] = points.Count - 1;

                  point vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvTopFront0[i] = texcoords.Count - 1;
                  uvTopFront3[i] = texcoords.Count - 1;

                  // TopRight /////////////////////////////////////////////////
                  newPoint = new point() { x = t, y = t1 };
                  points.Add(newPoint);

                  TopRight[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  newUV.u = 1;
                  texcoords.Add(newUV);
                  uvTopRight0[i] = texcoords.Count - 1;
                  newUV.v = 0;
                  texcoords.Add(newUV);
                  uvTopRight1[i] = texcoords.Count - 1;

                  // TopBack //////////////////////////////////////////////////
                  newPoint = new point() { y = t1, z = -t };
                  points.Add(newPoint);

                  TopBack[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvTopBack1[i] = texcoords.Count - 1;
                  uvTopBack2[i] = texcoords.Count - 1;

                  // TopLeft //////////////////////////////////////////////////
                  newPoint = new point() { x = -t, y = t1 };
                  points.Add(newPoint);

                  TopLeft[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvTopLeft2[i] = texcoords.Count - 1;
                  uvTopLeft3[i] = texcoords.Count - 1;

                  // BottomFront //////////////////////////////////////////////
                  newPoint = new point() { y = -t1, z = t };
                  points.Add(newPoint);

                  BottomFront[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvBottomFront4[i] = texcoords.Count - 1;
                  uvBottomFront7[i] = texcoords.Count - 1;

                  // BottomRight //////////////////////////////////////////////
                  newPoint = new point() { x = t, y = -t1 };
                  points.Add(newPoint);

                  BottomRight[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  newUV.u = 1;
                  texcoords.Add(newUV);
                  uvBottomRight4[i] = texcoords.Count - 1;
                  newUV.v = 0;
                  texcoords.Add(newUV);
                  uvBottomRight5[i] = texcoords.Count - 1;

                  // BottomBack ///////////////////////////////////////////////
                  newPoint = new point() { y = -t1, z = -t };
                  points.Add(newPoint);

                  BottomBack[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvBottomBack5[i] = texcoords.Count - 1;
                  uvBottomBack6[i] = texcoords.Count - 1;

                  // BottomLeft ///////////////////////////////////////////////
                  newPoint = new point() { x = -t, y = -t1 };
                  points.Add(newPoint);

                  BottomLeft[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvBottomLeft6[i] = texcoords.Count - 1;
                  uvBottomLeft7[i] = texcoords.Count - 1;

                  // RightFront ///////////////////////////////////////////////
                  newPoint = new point() { x = t, z = t1 };
                  points.Add(newPoint);

                  RightFront[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvRightFront0[i] = texcoords.Count - 1;


                  // RightBack ////////////////////////////////////////////////
                  newPoint = new point() { x = t1, z = -t };
                  points.Add(newPoint);

                  RightBack[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvRightBack1[i] = texcoords.Count - 1;

                  // LeftBack /////////////////////////////////////////////////
                  newPoint = new point() { x = -t, z = -t1 };
                  points.Add(newPoint);

                  LeftBack[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvLeftBack2[i] = texcoords.Count - 1;

                  // LeftFront ////////////////////////////////////////////////
                  newPoint = new point() { x = -t1, z = t };
                  points.Add(newPoint);

                  LeftFront[i] = points.Count - 1;

                  vec3 = An8Math.Normalize(points[points.Count - 1]);
                  newUV = SphericalTexCoordFromNormal(vec3);
                  texcoords.Add(newUV);
                  uvLeftFront3[i] = texcoords.Count - 1;
               }

               void AddGeodesicFaces(
                  //Int64 geodesic,
                  //List<point> points,
                  //List<texcoord> texcoords,
                  //List<facedata> facedatas,
                  Int64[] i_Left,
                  Int64[] i_Right,
                  Int64[] i_Bottom,
                  Int64[] i_uvLeft,
                  Int64[] i_uvRight,
                  Int64[] i_uvBottom,
                  Boolean i_FaceOrder)
               {
                  Int64[,] level = new Int64[geodesic + 1, geodesic + 1];
                  Int64[,] uvlevel = new Int64[geodesic + 1, geodesic + 1];

                  // Create new vertices + copy indices in level
                  for ( Int64 v = 0; v <= geodesic; v++ )
                  {
                     level[v, 0] = i_Left[v];
                     uvlevel[v, 0] = i_uvLeft[v];
                  }

                  for ( Int64 v = 0; v <= geodesic; v++ )
                  {
                     level[v, v] = i_Right[v];
                     uvlevel[v, v] = i_uvRight[v];
                  }

                  for ( Int64 h = 1; h < geodesic; h++ )
                  {
                     level[geodesic, h] = i_Bottom[h];
                     uvlevel[geodesic, h] = i_uvBottom[h];
                  }

                  for ( Int64 v = 2; v < geodesic; ++v )
                  {
                     point vec = new point();
                     vec.x = points[(Int32)level[v, v]].x - points[(Int32)level[v, 0]].x;
                     vec.y = points[(Int32)level[v, v]].y - points[(Int32)level[v, 0]].y;
                     vec.z = points[(Int32)level[v, v]].z - points[(Int32)level[v, 0]].z;

                     Double length = Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
                     vec = An8Math.Normalize(vec);

                     texcoord vecUV = new texcoord();
                     vecUV.u = texcoords[(Int32)uvlevel[v, v]].u - texcoords[(Int32)uvlevel[v, 0]].u;
                     vecUV.v = texcoords[(Int32)uvlevel[v, v]].v - texcoords[(Int32)uvlevel[v, 0]].v;

                     Double lengthUV = Math.Sqrt(vecUV.u * vecUV.u + vecUV.v * vecUV.v);
                     vecUV.u = vecUV.u / lengthUV;
                     vecUV.v = vecUV.v / lengthUV;

                     for ( Int64 h = 1; h < v; h++ )
                     {
                        point newPoint2 = new point();
                        newPoint2.x = points[(Int32)level[v, 0]].x + vec.x * (h * length / v);
                        newPoint2.y = points[(Int32)level[v, 0]].y + vec.y * (h * length / v);
                        newPoint2.z = points[(Int32)level[v, 0]].z + vec.z * (h * length / v);
                        points.Add(newPoint2);
                        level[v, h] = points.Count - 1;

                        texcoord newTexcoord = new texcoord();
                        newTexcoord.u = texcoords[(Int32)uvlevel[v, 0]].u + vecUV.u * (h * lengthUV / v);
                        newTexcoord.v = texcoords[(Int32)uvlevel[v, 0]].v + vecUV.v * (h * lengthUV / v);
                        texcoords.Add(newTexcoord);
                        uvlevel[v, h] = texcoords.Count - 1;
                     }
                  }

                  for ( Int64 v = 0; v < geodesic; v++ )
                  {
                     for ( Int64 i = 0; i < v + 1; i++ )
                     {
                        facedata newFace = new facedata();
                        newFace.numpoints = 3;
                        newFace.flags = facedataenum.hasnormals | facedataenum.hastexture;
                        newFace.matno = 0;
                        newFace.flatnormalno = 0;
                        newFace.pointdata = new pointdata[newFace.numpoints];

                        // 0
                        pointdata index0 = new pointdata();
                        index0.pointindex = level[v, i];
                        index0.normalindex = index0.pointindex;
                        index0.texcoordindex = uvlevel[v, i];

                        // 1
                        pointdata index1 = new pointdata();
                        index1.pointindex = level[v + 1, i];
                        index1.normalindex = index1.pointindex;
                        index1.texcoordindex = uvlevel[v + 1, i];

                        // 2
                        pointdata index2 = new pointdata();
                        index2.pointindex = level[v + 1, i + 1];
                        index2.normalindex = index2.pointindex;
                        index2.texcoordindex = uvlevel[v + 1, i + 1];

                        if ( i_FaceOrder == false )
                        {
                           newFace.pointdata[0] = index0;
                           newFace.pointdata[1] = index1;
                           newFace.pointdata[2] = index2;
                        }
                        else
                        {
                           newFace.pointdata[0] = index0;
                           newFace.pointdata[1] = index2;
                           newFace.pointdata[2] = index1;
                        }

                        facedatas.Add(newFace);
                     }

                     for ( Int64 i = 0; i < v; i++ )
                     {
                        facedata newFace = new facedata();
                        newFace.numpoints = 3;
                        newFace.flags = facedataenum.hasnormals | facedataenum.hasnormals;
                        newFace.matno = 0;
                        newFace.flatnormalno = 0;
                        newFace.pointdata = new pointdata[newFace.numpoints];

                        // 0
                        pointdata index0 = new pointdata();
                        index0.pointindex = level[v, i];
                        index0.normalindex = index0.pointindex;
                        index0.texcoordindex = uvlevel[v, i];

                        // 1
                        pointdata index1 = new pointdata();
                        index1.pointindex = level[v + 1, i + 1];
                        index1.normalindex = index1.pointindex;
                        index1.texcoordindex = uvlevel[v + 1, i + 1];

                        // 2
                        pointdata index2 = new pointdata();
                        index2.pointindex = level[v, i + 1];
                        index2.normalindex = index2.pointindex;
                        index2.texcoordindex = uvlevel[v, i + 1];

                        if ( i_FaceOrder == false )
                        {
                           newFace.pointdata[0] = index0;
                           newFace.pointdata[1] = index1;
                           newFace.pointdata[2] = index2;
                        }
                        else
                        {
                           newFace.pointdata[0] = index0;
                           newFace.pointdata[1] = index2;
                           newFace.pointdata[2] = index1;
                        }

                        facedatas.Add(newFace);
                     }
                  }
               }

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  TopFront, TopRight, RightFront,
                  uvTopFront0, uvTopRight0, uvRightFront0,
                  true);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  TopLeft, TopFront, LeftFront,
                  uvTopLeft3, uvTopFront3, uvLeftFront3,
                  true);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  TopRight, TopBack, RightBack,
                  uvTopRight1, uvTopBack1, uvRightBack1,
                  true);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  TopBack, TopLeft, LeftBack,
                  uvTopBack2, uvTopLeft2, uvLeftBack2,
                  true);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  BottomFront, BottomRight, RightFront,
                  uvBottomFront4, uvBottomRight4, uvRightFront0,
                  false);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  BottomLeft, BottomFront, LeftFront,
                  uvBottomLeft7, uvBottomFront7, uvLeftFront3,
                  false);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  BottomRight, BottomBack, RightBack,
                  uvBottomRight5, uvBottomBack5, uvRightBack1,
                  false);

               AddGeodesicFaces(
                  //geodesic, points, texcoords, facedatas,
                  BottomBack, BottomLeft, LeftBack,
                  uvBottomBack6, uvBottomLeft6, uvLeftBack2,
                  false);

               radius = diameter / 2;

               for ( Int64 i = 0; i < points.Count; ++i )
               {
                  point normal = An8Math.Normalize(points[(Int32)i]);
                  normals.Add(normal);

                  points[(Int32)i].x = normal.x * radius;
                  points[(Int32)i].y = normal.y * radius;
                  points[(Int32)i].z = normal.z * radius;
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
