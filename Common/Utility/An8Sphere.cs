using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Sphere
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="s">the sphere</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the converted mesh</returns>
      internal static mesh Calculate(sphere s, Action<String> callback = null)
      {
         mesh m = new mesh();
         m.name = s?.name;
         m.@base = s?.@base;
         m.pivot = s?.pivot;
         m.material = s?.material;

         if ( s?.material?.name != null )
         {
            m.materiallist = new materiallist();
            m.materiallist.materialname = new @string[1];
            m.materiallist.materialname[0] = new @string();
            m.materiallist.materialname[0].text = s.material.name;
         }

         List<point> points = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // Note: These defaults and limits were reversed engineered.
         Double diameter = (s?.diameter?.text ?? 10).Limit(0);
         Int32 longitude = (Int32)(s?.longlat?.longitude ?? 12).Limit(0, 32);
         Int32 latitude = (Int32)(s?.longlat?.latitude ?? 8).Limit(0, 16);
         // Note: Anim8or v1.00's editor limits geodesic from 1 to 6, but
         // the actual mesh in the object editor can be between 0 and 32.
         Int32 geodesic = (Int32)(s?.geodesic?.text ?? 0).Limit(0, 32);

         if ( callback != null )
         {
            if ( s?.diameter != null && s.diameter.text != diameter )
            {
               callback($"The \"{s.name?.text}\" sphere's diameter of {s.diameter.text} has been limited to {diameter}.");
            }

            if ( s?.longlat != null )
            {
               if ( s.longlat.longitude != longitude )
               {
                  callback($"The \"{s.name?.text}\" sphere's longitude of {s.longlat.longitude} has been limited to {longitude}.");
               }

               if ( s.longlat.latitude != latitude )
               {
                  callback($"The \"{s.name?.text}\" sphere's latitude of {s.longlat.latitude} has been limited to {latitude}.");
               }
            }

            if ( s?.geodesic != null && s.geodesic.text != geodesic )
            {
               callback($"The \"{s.name?.text}\" sphere's geodesic of {s.geodesic.text} has been limited to {geodesic}.");
            }
         }

         // Anim8or prioritizes longlat over geodesic
         if ( s?.longlat != null || s?.geodesic == null )
         {
            //
            //       -4*--               Y
            //   7*        3*            |
            //   /           \           |
            // 6*            2*          *------X
            //   \           /          /
            //   5*        1*          Z
            //       -0*--
            //
            for ( Int32 ix = 0; ix <= longitude; ix++ )
            {
               Double cosX = Math.Cos(ix * 2 * Math.PI / longitude);
               Double sinX = -Math.Sin(ix * 2 * Math.PI / longitude);

               for ( Int32 iy = 0; iy <= latitude; iy++ )
               {
                  Double cosY = -Math.Cos(iy * Math.PI / latitude);
                  Double sinY = Math.Sin(iy * Math.PI / latitude);

                  // Don't create the last vertical slice of points, since
                  // they are the same as the first
                  if ( ix < longitude )
                  {
                     // Don't create the bottom and top points more than once
                     if ( ix == 0 || iy != 0 && iy != latitude )
                     {
                        points.Add(new point(
                           cosX * sinY * diameter / 2,
                           cosY * diameter / 2,
                           sinX * sinY * diameter / 2));
                     }
                  }

                  texcoords.Add(new texcoord(
                     (Double)ix / longitude,
                     (Double)iy / latitude));
               }
            }

            // Note: Anim8or v1.00 does some special handling of edge cases.
            if ( longitude == 0 || latitude == 0 )
            {
               points.Clear();
               texcoords.Clear();

               // If both are 0, create a point at the center
               if ( longitude == 0 && latitude == 0 )
               {
                  points.Add(new point(0, 0, 0));
                  texcoords.Add(new texcoord(0, 0));
               }
               else if ( latitude == 0 )
               {
                  points.Add(new point(0, 0, 0));

                  for ( Int32 ix = 0; ix <= longitude; ix++ )
                  {
                     texcoords.Add(new texcoord((Double)ix / longitude, 0));
                  }
               }
               else // if ( longitude == 0 )
               {
                  points.Add(new point(0, -diameter / 2, 0));
                  points.Add(new point(0, diameter / 2, 0));

                  for ( Int32 iy = 0; iy <= latitude; iy++ )
                  {
                     texcoords.Add(new texcoord(0, (Double)iy / latitude));
                  }
               }
            }

            // Create the faces
            for ( Int32 iy = 0; iy < latitude; iy++ )
            {
               for ( Int32 ix = 0; ix < longitude; ix++ )
               {
                  pointdata leftTop = new pointdata();
                  leftTop.pointindex = ix * (latitude + 1) + iy + 1;
                  leftTop.normalindex = 0;
                  leftTop.texcoordindex = leftTop.pointindex;

                  pointdata rightTop = new pointdata();
                  rightTop.pointindex = (ix + 1) * (latitude + 1) + iy + 1;
                  rightTop.normalindex = 0;
                  rightTop.texcoordindex = rightTop.pointindex;

                  pointdata rightBottom = new pointdata();
                  rightBottom.pointindex = (ix + 1) * (latitude + 1) + iy;
                  rightBottom.normalindex = 0;
                  rightBottom.texcoordindex = rightBottom.pointindex;

                  pointdata leftBottom = new pointdata();
                  leftBottom.pointindex = ix * (latitude + 1) + iy;
                  leftBottom.normalindex = 0;
                  leftBottom.texcoordindex = leftBottom.pointindex;

                  Int64 FixIndex(Int64 index)
                  {
                     // If the first vertical slice
                     if ( index <= latitude )
                     {
                        // Return the first vertical slice unchanged
                        return index;
                     }
                     // If the middle vertical slices
                     else if ( index < (latitude + 1) * longitude )
                     {
                        Int64 mod = index % (latitude + 1);

                        if ( mod == 0 || mod == latitude )
                        {
                           // Return the original bottom or top point
                           return mod;
                        }
                        else
                        {
                           // Shift the indices by removing the duplicate
                           // bottom and top points
                           Int64 div = index / (latitude + 1);
                           return index - (div - 1) * 2 - 1;
                        }
                     }
                     // If the last vertical slice
                     else
                     {
                        // Return the first vertical slice instead
                        return index % (latitude + 1);
                     }
                  }

                  leftTop.pointindex = FixIndex(leftTop.pointindex);
                  rightTop.pointindex = FixIndex(rightTop.pointindex);
                  rightBottom.pointindex = FixIndex(rightBottom.pointindex);
                  leftBottom.pointindex = FixIndex(leftBottom.pointindex);

                  pointdata[] pointdata;

                  if ( iy == 0 ) // bottom
                  {
                     // Builds the vertices starting at the left bottom
                     //
                     // 1*----2*
                     //  |   /
                     //  | /
                     // 0*
                     pointdata = new pointdata[]
                     {
                        leftBottom,
                        leftTop,
                        rightTop,
                     };
                  }
                  else if ( iy < latitude - 1 ) // middle
                  {
                     // Build the vertices starting at the right bottom
                     //
                     // 2*----3*
                     //  |     |
                     //  |     |
                     // 1*----0*
                     pointdata = new pointdata[]
                     {
                        rightBottom,
                        leftBottom,
                        leftTop,
                        rightTop,
                     };
                  }
                  else // top
                  {
                     // Builds the vertices starting at the right bottom
                     //
                     // 2*
                     //  | \
                     //  |   \
                     // 1*----0*
                     pointdata = new pointdata[]
                     {
                        rightBottom,
                        leftBottom,
                        leftTop,
                     };
                  }

                  facedata facedata = new facedata();
                  facedata.numpoints = pointdata.Length;
                  facedata.flags = facedataenum.hastexture;
                  facedata.matno = 0;
                  facedata.flatnormalno = -1;
                  facedata.pointdata = pointdata;

                  facedatas.Add(facedata);
               }
            }
         }
         else // if ( s?.geodesic != null )
         {
            Int32[] leftBottomIndices = new Int32[geodesic + 1];
            Int32[] leftTopIndices = new Int32[geodesic + 1];
            Int32[] leftBackIndices = new Int32[geodesic + 1];
            Int32[] leftFrontIndices = new Int32[geodesic + 1];
            Int32[] rightBottomIndices = new Int32[geodesic + 1];
            Int32[] rightTopIndices = new Int32[geodesic + 1];
            Int32[] rightBackIndices = new Int32[geodesic + 1];
            Int32[] rightFrontIndices = new Int32[geodesic + 1];
            Int32[] bottomBackIndices = new Int32[geodesic + 1];
            Int32[] bottomFrontIndices = new Int32[geodesic + 1];
            Int32[] topBackIndices = new Int32[geodesic + 1];
            Int32[] topFrontIndices = new Int32[geodesic + 1];
            Int32[] uvRightTop0 = new Int32[geodesic + 1];
            Int32[] uvRightFront0 = new Int32[geodesic + 1];
            Int32[] uvTopFront0 = new Int32[geodesic + 1];
            Int32[] uvRightTop1 = new Int32[geodesic + 1];
            Int32[] uvRightBack1 = new Int32[geodesic + 1];
            Int32[] uvTopBack1 = new Int32[geodesic + 1];
            Int32[] uvLeftTop2 = new Int32[geodesic + 1];
            Int32[] uvLeftBack2 = new Int32[geodesic + 1];
            Int32[] uvTopBack2 = new Int32[geodesic + 1];
            Int32[] uvLeftFront3 = new Int32[geodesic + 1];
            Int32[] uvLeftTop3 = new Int32[geodesic + 1];
            Int32[] uvTopFront3 = new Int32[geodesic + 1];
            Int32[] uvBottomFront4 = new Int32[geodesic + 1];
            Int32[] uvBottomRight4 = new Int32[geodesic + 1];
            Int32[] uvRightBottom5 = new Int32[geodesic + 1];
            Int32[] uvBottomBack5 = new Int32[geodesic + 1];
            Int32[] uvLeftBottom6 = new Int32[geodesic + 1];
            Int32[] uvBottomBack6 = new Int32[geodesic + 1];
            Int32[] uvLeftBottom7 = new Int32[geodesic + 1];
            Int32[] uvBottomFront7 = new Int32[geodesic + 1];

            // Points of level 0
            point p;
            texcoord t;

            // Point top
            p = new point(0, diameter / 2, 0); // point0
            topFrontIndices[0] = points.Count;
            topBackIndices[0] = points.Count;
            rightTopIndices[0] = points.Count;
            leftTopIndices[0] = points.Count;
            points.Add(p);

            // Note: Anim8or v1.00 creates this even though it is unused.
            t = new texcoord(0, 1); // texcoord0
            texcoords.Add(t);

            t = new texcoord(0.125, 1); // texcoord1
            uvRightTop1[0] = texcoords.Count;
            uvTopBack1[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.375, 1); // texcoord2
            uvLeftTop2[0] = texcoords.Count;
            uvTopBack2[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.625, 1); // texcoord3
            uvLeftTop3[0] = texcoords.Count;
            uvTopFront3[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.875, 1); // texcoord4
            uvTopFront0[0] = texcoords.Count;
            uvRightTop0[0] = texcoords.Count;
            texcoords.Add(t);

            // Note: Anim8or v1.00 creates this even though it is unused.
            t = new texcoord(0, 0); // texcoord5
            texcoords.Add(t);

            // Point bottom
            p = new point(0, -diameter / 2, 0); // point1
            bottomFrontIndices[0] = points.Count;
            bottomBackIndices[0] = points.Count;
            rightBottomIndices[0] = points.Count;
            leftBottomIndices[0] = points.Count;
            points.Add(p);

            t = new texcoord(0.125, 0); // texcoord6
            uvRightBottom5[0] = texcoords.Count;
            uvBottomBack5[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.375, 0); // texcoord7
            uvLeftBottom6[0] = texcoords.Count;
            uvBottomBack6[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.625, 0); // texcoord8
            uvLeftBottom7[0] = texcoords.Count;
            uvBottomFront7[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.875, 0); // texcoord9
            uvBottomFront4[0] = texcoords.Count;
            uvBottomRight4[0] = texcoords.Count;
            texcoords.Add(t);

            // Point front
            p = new point(0, 0, diameter / 2); // point2
            topFrontIndices[geodesic] = points.Count;
            bottomFrontIndices[geodesic] = points.Count;
            leftFrontIndices[geodesic] = points.Count;
            rightFrontIndices[0] = points.Count;
            points.Add(p);

            t = new texcoord(0.75, 0.5); // texcoord10
            uvTopFront0[geodesic] = texcoords.Count;
            uvTopFront3[geodesic] = texcoords.Count;
            uvBottomFront4[geodesic] = texcoords.Count;
            uvBottomFront7[geodesic] = texcoords.Count;
            uvRightFront0[0] = texcoords.Count;
            uvLeftFront3[geodesic] = texcoords.Count;
            texcoords.Add(t);

            // Point back
            p = new point(0, 0, -diameter / 2); // point3
            topBackIndices[geodesic] = points.Count;
            rightBackIndices[geodesic] = points.Count;
            leftBackIndices[0] = points.Count;
            bottomBackIndices[geodesic] = points.Count;
            points.Add(p);

            t = new texcoord(0.25, 0.5); // texcoord11
            uvTopBack1[geodesic] = texcoords.Count;
            uvTopBack2[geodesic] = texcoords.Count;
            uvBottomBack5[geodesic] = texcoords.Count;
            uvBottomBack6[geodesic] = texcoords.Count;
            uvRightBack1[geodesic] = texcoords.Count;
            uvLeftBack2[0] = texcoords.Count;
            texcoords.Add(t);

            // Point right
            p = new point(diameter / 2, 0, 0); // point4
            rightTopIndices[geodesic] = points.Count;
            rightBottomIndices[geodesic] = points.Count;
            rightBackIndices[0] = points.Count;
            rightFrontIndices[geodesic] = points.Count;
            points.Add(p);

            t = new texcoord(0, 0.5); // texcoord12
            uvRightTop1[geodesic] = texcoords.Count;
            uvRightBack1[0] = texcoords.Count;
            uvRightBottom5[geodesic] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(1, 0.5); // texcoord13
            uvRightTop0[geodesic] = texcoords.Count;
            uvRightFront0[geodesic] = texcoords.Count;
            uvBottomRight4[geodesic] = texcoords.Count;
            texcoords.Add(t);

            // Point left
            p = new point(-diameter / 2, 0, 0); // point5
            leftTopIndices[geodesic] = points.Count;
            leftBottomIndices[geodesic] = points.Count;
            leftBackIndices[geodesic] = points.Count;
            leftFrontIndices[0] = points.Count;
            points.Add(p);

            t = new texcoord(0.5, 0.5); // texcoord14
            uvLeftTop2[geodesic] = texcoords.Count;
            uvLeftTop3[geodesic] = texcoords.Count;
            uvLeftBack2[geodesic] = texcoords.Count;
            uvLeftFront3[0] = texcoords.Count;
            uvLeftBottom6[geodesic] = texcoords.Count;
            uvLeftBottom7[geodesic] = texcoords.Count;
            texcoords.Add(t);

            for ( Int32 i = 1; i < geodesic; i++ )
            {
               Double t0 = (Double)i / geodesic;
               Double t1 = 1 - t0;
               Double scale = diameter / 2 / Math.Sqrt(t0 * t0 + t1 * t1);
               t0 *= scale;
               t1 *= scale;

               // Top front
               p = new point(0, t1, t0); // point6
               topFrontIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord15
               uvTopFront0[i] = texcoords.Count;
               uvTopFront3[i] = texcoords.Count;
               texcoords.Add(t);

               // Top right
               p = new point(t0, t1, 0); // point7
               rightTopIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord16
               uvRightTop0[i] = texcoords.Count;
               texcoords.Add(t);
               t.u = 1;// texcoord17
               uvRightTop1[i] = texcoords.Count;
               texcoords.Add(t);

               // Top back
               p = new point(0, t1, -t0); // point8
               topBackIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord18
               uvTopBack1[i] = texcoords.Count;
               uvTopBack2[i] = texcoords.Count;
               texcoords.Add(t);

               // Top left
               p = new point(-t0, t1, 0); // point9
               leftTopIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord19
               uvLeftTop2[i] = texcoords.Count;
               uvLeftTop3[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom front
               p = new point(0, -t1, t0); // point10
               bottomFrontIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord20
               uvBottomFront4[i] = texcoords.Count;
               uvBottomFront7[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom right
               p = new point(t0, -t1, 0); // point11
               rightBottomIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord21
               uvBottomRight4[i] = texcoords.Count;
               texcoords.Add(t);
               t.u = 1;// texcoord22
               uvRightBottom5[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom back
               p = new point(0, -t1, -t0); // point12
               bottomBackIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord23
               uvBottomBack5[i] = texcoords.Count;
               uvBottomBack6[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom left
               p = new point(-t0, -t1, 0); // point13
               leftBottomIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord24
               uvLeftBottom6[i] = texcoords.Count;
               uvLeftBottom7[i] = texcoords.Count;
               texcoords.Add(t);

               // Right front
               p = new point(t0, 0, t1); // point14
               rightFrontIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord25
               uvRightFront0[i] = texcoords.Count;
               texcoords.Add(t);

               // Right back
               p = new point(t1, 0, -t0); // point15
               rightBackIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord26
               uvRightBack1[i] = texcoords.Count;
               texcoords.Add(t);

               // Left back
               p = new point(-t0, 0, -t1); // point16
               leftBackIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord27
               uvLeftBack2[i] = texcoords.Count;
               texcoords.Add(t);

               // Left front
               p = new point(-t1, 0, t0); // point17
               leftFrontIndices[i] = points.Count;
               points.Add(p);
               t = SphericalTexCoord(p); // texcoord28
               uvLeftFront3[i] = texcoords.Count;
               texcoords.Add(t);
            }

            // face0
            // LeftTopFront faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               topFrontIndices,
               rightTopIndices,
               rightFrontIndices,
               uvTopFront0,
               uvRightTop0,
               uvRightFront0,
               true);

            // face1
            // LeftTopBack faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               rightTopIndices,
               topBackIndices,
               rightBackIndices,
               uvRightTop1,
               uvTopBack1,
               uvRightBack1,
               true);

            // face2
            // RightTopBack faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               topBackIndices,
               leftTopIndices,
               leftBackIndices,
               uvTopBack2,
               uvLeftTop2,
               uvLeftBack2,
               true);

            // face3
            // RightTopFront faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               leftTopIndices,
               topFrontIndices,
               leftFrontIndices,
               uvLeftTop3,
               uvTopFront3,
               uvLeftFront3,
               true);

            // face4
            // LeftBottomFront faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               bottomFrontIndices,
               rightBottomIndices,
               rightFrontIndices,
               uvBottomFront4,
               uvBottomRight4,
               uvRightFront0,
               false);

            // face5
            // LeftBottomBack faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               rightBottomIndices,
               bottomBackIndices,
               rightBackIndices,
               uvRightBottom5,
               uvBottomBack5,
               uvRightBack1,
               false);

            // face6
            // RightBottomBack faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               bottomBackIndices,
               leftBottomIndices,
               leftBackIndices,
               uvBottomBack6,
               uvLeftBottom6,
               uvLeftBack2,
               false);

            // face7
            // RightBottomFront faces?
            AddGeodesicFaces(
               geodesic,
               points,
               texcoords,
               facedatas,
               leftBottomIndices,
               bottomFrontIndices,
               leftFrontIndices,
               uvLeftBottom7,
               uvBottomFront7,
               uvLeftFront3,
               false);
         }

         m.points = new points();
         m.points.point = points.ToArray();
         m.texcoords = new texcoords();
         m.texcoords.texcoord = texcoords.ToArray();

         // Note: It is possible for there to be no faces.
         if ( facedatas.Count > 0 )
         {
            m.faces = new faces();
            m.faces.facedata = facedatas.ToArray();
         }

         return m;
      }

      static texcoord SphericalTexCoord(point p)
      {
         texcoord t = new texcoord();
         t.u = Math.Atan2(p.z, p.x) / (-2 * Math.PI);
         t.u = t.u >= 0 ? t.u : t.u + 1;
         t.v = Math.Asin(p.y) / Math.PI + 0.5;
         return t;
      }

      static void AddGeodesicFaces(
         Int32 geodesic,
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
         for ( Int32 v = 0; v <= geodesic; v++ )
         {
            level[v, 0] = left[v];
            uvlevel[v, 0] = uvLeft[v];
         }

         for ( Int32 v = 0; v <= geodesic; v++ )
         {
            level[v, v] = right[v];
            uvlevel[v, v] = uvRight[v];
         }

         for ( Int32 h = 1; h < geodesic; h++ )
         {
            level[geodesic, h] = bottom[h];
            uvlevel[geodesic, h] = uvBottom[h];
         }

         for ( Int32 v = 2; v < geodesic; ++v )
         {
            point vec = points[level[v, v]] - points[level[v, 0]];

            Double length = vec.GetLength();
            vec = vec / length; // vec.Normalize();

            texcoord t = texcoords[uvlevel[v, v]] - texcoords[uvlevel[v, 0]];

            Double lengthUV = t.GetLength();
            t = t / lengthUV; // t.Normalize();

            for ( Int32 h = 1; h < v; h++ )
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

         for ( Int32 v = 0; v < geodesic; v++ )
         {
            for ( Int32 i = 0; i < v + 1; i++ )
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

            for ( Int32 i = 0; i < v; i++ )
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
