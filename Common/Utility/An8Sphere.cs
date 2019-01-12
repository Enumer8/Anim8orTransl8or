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

         // Note: Anim8or v1.00 prioritizes longlat over geodesic
         if ( s?.longlat != null || s?.geodesic == null )
         {
            // This creates all the points.
            //
            //       -4*--                  Y
            //   7*        3*               |
            //   /           \              |
            // 6*            2*             *------X
            //   \           /             /
            //   5*        1*             Z
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

                  // Don't create the last vertical slice of points, since they
                  // are the same as the first
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

            // Note: Anim8or v1.00 does some special handling of edge cases. It
            // is easiest just to handle them individually.
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

            // This creates all the faces.
            AddLatLongFaces(facedatas, longitude, latitude);
         }
         else // if ( s?.geodesic != null )
         {
            // These contain the indices of the seam between the right bottom
            // back octant and the right bottom front octant.
            //
            //       [0]
            //      --*-- [1]
            //   *         *
            //  /           [2]
            // *             *       X------*
            //  \           /              /|
            //   *         *              Z |
            //      --*--                   Y
            //
            Int32[] rightBottomIndices = new Int32[geodesic + 1];
            Int32[] uvRightBottomIndicesA = new Int32[geodesic + 1];
            Int32[] uvRightBottomIndicesB = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right top back
            // octant and the right top front octant.
            //
            //       [0]
            //      --*-- [1]               Y
            //   *         *                | Z
            //  /           [2]             |/
            // *             *       X------*
            //  \           /
            //   *         *
            //      --*--
            //
            Int32[] rightTopIndices = new Int32[geodesic + 1];
            Int32[] uvRightTopIndicesA = new Int32[geodesic + 1];
            Int32[] uvRightTopIndicesB = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right bottom
            // back octant and the right top back octant.
            //
            //       [0]
            //      --*-- [1]
            //   *         *                  Y
            //  /           [2]              /
            // *             *       X------*
            //  \           /               |
            //   *         *                |
            //      --*--                   Z
            //
            Int32[] rightBackIndices = new Int32[geodesic + 1];
            Int32[] uvRightBackIndices = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right bottom
            // front octant and the right top front octant.
            //
            //       [0]
            //      --*-- [1]
            //   *         *                  Y
            //  /           [2]              /
            // *             *              *------Z
            //  \           /               |
            //   *         *                |
            //      --*--                   X
            //
            Int32[] rightFrontIndices = new Int32[geodesic + 1];
            Int32[] uvRightFrontIndices = new Int32[geodesic + 1];

            // These contain the indices of the seam between the left bottom
            // back octant and the left bottom front octant.
            //
            //       [0]
            //      --*-- [1]
            //   *         *                  Z
            //  /           [2]              /
            // *             *              *------X
            //  \           /               |
            //   *         *                |
            //      --*--                   Y
            //
            Int32[] leftBottomIndices = new Int32[geodesic + 1];
            Int32[] uvLeftBottomIndicesB = new Int32[geodesic + 1];
            Int32[] uvLeftBottomIndicesA = new Int32[geodesic + 1];

            // These contain the indices of the seam between the left top back
            // octant and the left top front octant.
            //
            //       [0]
            //      --*-- [1]               Y
            //   *         *                |
            //  /           [2]             |
            // *             *              *------X
            //  \           /              /
            //   *         *              Z
            //      --*--
            //
            Int32[] leftTopIndices = new Int32[geodesic + 1];
            Int32[] uvLeftTopIndicesA = new Int32[geodesic + 1];
            Int32[] uvLeftTopIndicesB = new Int32[geodesic + 1];

            // These contain the indices of the seam between the left bottom
            // back octant and the left top back octant.
            //
            //       [0]
            //      --*-- [1]               X
            //   *         *                | Y
            //  /           [2]             |/
            // *             *       Z------*
            //  \           /
            //   *         *
            //      --*--
            //
            Int32[] leftBackIndices = new Int32[geodesic + 1];
            Int32[] uvLeftBackIndices = new Int32[geodesic + 1];

            // These contain the indices of the seam between the left bottom
            // front octant and the left top front octant.
            //
            //       [0]
            //      --*-- [1]               Z
            //   *         *                | Y
            //  /           [2]             |/
            // *             *              *------X
            //  \           /
            //   *         *
            //      --*--
            //
            Int32[] leftFrontIndices = new Int32[geodesic + 1];
            Int32[] uvLeftFrontIndices = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right bottom
            // back octant and the left bottom back octant.
            //
            //       [0]
            //      --*-- [1]
            //   *         *                  X
            //  /           [2]              /
            // *             *       Z------*
            //  \           /               |
            //   *         *                |
            //      --*--                   Y
            //
            Int32[] bottomBackIndices = new Int32[geodesic + 1];
            Int32[] uvBottomBackIndicesA = new Int32[geodesic + 1];
            Int32[] uvBottomBackIndicesB = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right bottom
            // front octant and the left bottom front octant.
            //
            //       [0]
            //      --*-- [1]
            //   *         *
            //  /           [2]
            // *             *              *------Z
            //  \           /              /|
            //   *         *              X |
            //      --*--                   Y
            //
            Int32[] bottomFrontIndices = new Int32[geodesic + 1];
            Int32[] uvBottomFrontIndicesA = new Int32[geodesic + 1];
            Int32[] uvBottomFrontIndicesB = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right top back
            // octant and the left top back octant.
            //
            //       [0]
            //      --*-- [1]               Y
            //   *         *                |
            //  /           [2]             |
            // *             *       Z------*
            //  \           /              /
            //   *         *              X
            //      --*--
            //
            Int32[] topBackIndices = new Int32[geodesic + 1];
            Int32[] uvTopBackIndicesA = new Int32[geodesic + 1];
            Int32[] uvTopBackIndicesB = new Int32[geodesic + 1];

            // These contain the indices of the seam between the right top
            // front octant and the left top front octant.
            //
            //       [0]
            //      --*-- [1]               Y
            //   *         *                | X
            //  /           [2]             |/
            // *             *              *------Z
            //  \           /
            //   *         *
            //      --*--
            //
            Int32[] topFrontIndices = new Int32[geodesic + 1];
            Int32[] uvTopFrontIndicesA = new Int32[geodesic + 1];
            Int32[] uvTopFrontIndicesB = new Int32[geodesic + 1];

            // This creates the initial octahedron.
            //
            //        0*                    Y
            //      / / \ \                 |
            //    /  /   \  \               |
            // 5*---2*--3*---4*             *------X
            //    \  \   /  /              /
            //      \ \ / /               Z
            //        1*
            //
            point p;
            texcoord t;

            // This creates the top point of the octahedron.
            p = new point(0, diameter / 2, 0);
            rightTopIndices[0] = points.Count;
            leftTopIndices[0] = points.Count;
            topBackIndices[0] = points.Count;
            topFrontIndices[0] = points.Count;
            points.Add(p);

            // Note: Anim8or v1.00 creates this even though it is unused.
            t = new texcoord(0, 1);
            texcoords.Add(t);

            t = new texcoord(0.125, 1);
            uvLeftTopIndicesB[0] = texcoords.Count;
            uvTopBackIndicesA[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.375, 1);
            uvRightTopIndicesA[0] = texcoords.Count;
            uvTopBackIndicesB[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.625, 1);
            uvRightTopIndicesB[0] = texcoords.Count;
            uvTopFrontIndicesB[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.875, 1);
            uvLeftTopIndicesA[0] = texcoords.Count;
            uvTopFrontIndicesA[0] = texcoords.Count;
            texcoords.Add(t);

            // Note: Anim8or v1.00 creates this even though it is unused.
            t = new texcoord(0, 0);
            texcoords.Add(t);

            // This creates the bottom point of the octahedron.
            p = new point(0, -diameter / 2, 0);
            rightBottomIndices[0] = points.Count;
            leftBottomIndices[0] = points.Count;
            bottomBackIndices[0] = points.Count;
            bottomFrontIndices[0] = points.Count;
            points.Add(p);

            t = new texcoord(0.125, 0);
            uvLeftBottomIndicesB[0] = texcoords.Count;
            uvBottomBackIndicesA[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.375, 0);
            uvRightBottomIndicesA[0] = texcoords.Count;
            uvBottomBackIndicesB[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.625, 0);
            uvRightBottomIndicesB[0] = texcoords.Count;
            uvBottomFrontIndicesB[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(0.875, 0);
            uvLeftBottomIndicesA[0] = texcoords.Count;
            uvBottomFrontIndicesA[0] = texcoords.Count;
            texcoords.Add(t);

            // This creates the front point of the octahedron.
            p = new point(0, 0, diameter / 2);
            rightFrontIndices[geodesic] = points.Count;
            leftFrontIndices[0] = points.Count;
            bottomFrontIndices[geodesic] = points.Count;
            topFrontIndices[geodesic] = points.Count;
            points.Add(p);

            t = new texcoord(0.75, 0.5);
            uvRightFrontIndices[geodesic] = texcoords.Count;
            uvLeftFrontIndices[0] = texcoords.Count;
            uvBottomFrontIndicesA[geodesic] = texcoords.Count;
            uvBottomFrontIndicesB[geodesic] = texcoords.Count;
            uvTopFrontIndicesA[geodesic] = texcoords.Count;
            uvTopFrontIndicesB[geodesic] = texcoords.Count;
            texcoords.Add(t);

            // This creates the back point of the octahedron.
            p = new point(0, 0, -diameter / 2);
            rightBackIndices[0] = points.Count;
            leftBackIndices[geodesic] = points.Count;
            bottomBackIndices[geodesic] = points.Count;
            topBackIndices[geodesic] = points.Count;
            points.Add(p);

            t = new texcoord(0.25, 0.5);
            uvRightBackIndices[0] = texcoords.Count;
            uvLeftBackIndices[geodesic] = texcoords.Count;
            uvBottomBackIndicesA[geodesic] = texcoords.Count;
            uvBottomBackIndicesB[geodesic] = texcoords.Count;
            uvTopBackIndicesA[geodesic] = texcoords.Count;
            uvTopBackIndicesB[geodesic] = texcoords.Count;
            texcoords.Add(t);

            // This creates the left point of the octahedron.
            p = new point(diameter / 2, 0, 0);
            leftBottomIndices[geodesic] = points.Count;
            leftTopIndices[geodesic] = points.Count;
            leftBackIndices[0] = points.Count;
            leftFrontIndices[geodesic] = points.Count;
            points.Add(p);

            t = new texcoord(0, 0.5);
            uvLeftBottomIndicesB[geodesic] = texcoords.Count;
            uvLeftTopIndicesB[geodesic] = texcoords.Count;
            uvLeftBackIndices[0] = texcoords.Count;
            texcoords.Add(t);

            t = new texcoord(1, 0.5);
            uvLeftBottomIndicesA[geodesic] = texcoords.Count;
            uvLeftTopIndicesA[geodesic] = texcoords.Count;
            uvLeftFrontIndices[geodesic] = texcoords.Count;
            texcoords.Add(t);

            // This creates the right point of the octahedron.
            p = new point(-diameter / 2, 0, 0);
            rightBottomIndices[geodesic] = points.Count;
            rightTopIndices[geodesic] = points.Count;
            rightBackIndices[geodesic] = points.Count;
            rightFrontIndices[0] = points.Count;
            points.Add(p);

            t = new texcoord(0.5, 0.5);
            uvRightBottomIndicesA[geodesic] = texcoords.Count;
            uvRightBottomIndicesB[geodesic] = texcoords.Count;
            uvRightTopIndicesA[geodesic] = texcoords.Count;
            uvRightTopIndicesB[geodesic] = texcoords.Count;
            uvRightBackIndices[geodesic] = texcoords.Count;
            uvRightFrontIndices[0] = texcoords.Count;
            texcoords.Add(t);

            // This creates the missing points of the seams.
            for ( Int32 i = 1; i < geodesic; i++ )
            {
               Double t0 = (Double)i / geodesic;
               Double t1 = 1 - t0;
               Double scale = diameter / 2 / Math.Sqrt(t0 * t0 + t1 * t1);
               t0 *= scale;
               t1 *= scale;

               // Top front
               p = new point(0, t1, t0);
               topFrontIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvTopFrontIndicesA[i] = texcoords.Count;
               uvTopFrontIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               // Top left
               p = new point(t0, t1, 0);
               leftTopIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvLeftTopIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               t.u = 1;
               uvLeftTopIndicesA[i] = texcoords.Count;
               texcoords.Add(t);

               // Top back
               p = new point(0, t1, -t0);
               topBackIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvTopBackIndicesA[i] = texcoords.Count;
               uvTopBackIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               // Top right
               p = new point(-t0, t1, 0);
               rightTopIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvRightTopIndicesA[i] = texcoords.Count;
               uvRightTopIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom front
               p = new point(0, -t1, t0);
               bottomFrontIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvBottomFrontIndicesA[i] = texcoords.Count;
               uvBottomFrontIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom left
               p = new point(t0, -t1, 0);
               leftBottomIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvLeftBottomIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               t.u = 1;
               uvLeftBottomIndicesA[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom back
               p = new point(0, -t1, -t0);
               bottomBackIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvBottomBackIndicesA[i] = texcoords.Count;
               uvBottomBackIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               // Bottom right
               p = new point(-t0, -t1, 0);
               rightBottomIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvRightBottomIndicesA[i] = texcoords.Count;
               uvRightBottomIndicesB[i] = texcoords.Count;
               texcoords.Add(t);

               // Left front
               p = new point(t0, 0, t1);
               leftFrontIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvLeftFrontIndices[i] = texcoords.Count;
               texcoords.Add(t);

               // Left back
               p = new point(t1, 0, -t0);
               leftBackIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvLeftBackIndices[i] = texcoords.Count;
               texcoords.Add(t);

               // Right back
               p = new point(-t0, 0, -t1);
               rightBackIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvRightBackIndices[i] = texcoords.Count;
               texcoords.Add(t);

               // Right front
               p = new point(-t1, 0, t0);
               rightFrontIndices[i] = points.Count;
               points.Add(p);

               t = SphericalTexCoord(p);
               uvRightFrontIndices[i] = texcoords.Count;
               texcoords.Add(t);
            }

            // This creates the faces in the left top front octant and creates
            // the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               topFrontIndices,
               leftTopIndices,
               leftFrontIndices,
               uvTopFrontIndicesA,
               uvLeftTopIndicesA,
               uvLeftFrontIndices,
               true);

            // This creates the faces in the left top back octant and creates
            // the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               leftTopIndices,
               topBackIndices,
               leftBackIndices,
               uvLeftTopIndicesB,
               uvTopBackIndicesA,
               uvLeftBackIndices,
               true);

            // This creates the faces in the right top back octant and creates
            // the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               topBackIndices,
               rightTopIndices,
               rightBackIndices,
               uvTopBackIndicesB,
               uvRightTopIndicesA,
               uvRightBackIndices,
               true);

            // This creates the faces in the right top front octant and creates
            // the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               rightTopIndices,
               topFrontIndices,
               rightFrontIndices,
               uvRightTopIndicesB,
               uvTopFrontIndicesB,
               uvRightFrontIndices,
               true);

            // This creates the faces in the left bottom front octant and
            // creates the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               bottomFrontIndices,
               leftBottomIndices,
               leftFrontIndices,
               uvBottomFrontIndicesA,
               uvLeftBottomIndicesA,
               uvLeftFrontIndices,
               false);

            // This creates the faces in the left bottom back octant and
            // creates the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               leftBottomIndices,
               bottomBackIndices,
               leftBackIndices,
               uvLeftBottomIndicesB,
               uvBottomBackIndicesA,
               uvLeftBackIndices,
               false);

            // This creates the faces in the right bottom back octant and
            // creates the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               bottomBackIndices,
               rightBottomIndices,
               rightBackIndices,
               uvBottomBackIndicesB,
               uvRightBottomIndicesA,
               uvRightBackIndices,
               false);

            // This creates the faces in the right bottom front octant and
            // creates the missing points.
            AddGeodesicFaces(
               diameter,
               geodesic,
               points,
               texcoords,
               facedatas,
               rightBottomIndices,
               bottomFrontIndices,
               rightFrontIndices,
               uvRightBottomIndicesB,
               uvBottomFrontIndicesB,
               uvRightFrontIndices,
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
         p = p.Normalize();
         texcoord t = new texcoord();
         t.u = Math.Atan2(p.z, p.x) / (-2 * Math.PI);
         t.u = t.u >= 0 ? t.u : t.u + 1;
         t.v = Math.Asin(p.y) / Math.PI + 0.5;
         return t;
      }

      static void AddLatLongFaces(
         List<facedata> facedatas,
         Int32 longitude,
         Int32 latitude)
      {
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
                  // This builds the vertices starting at the left bottom.
                  //
                  // 1*-----*2
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
                  // This builds the vertices starting at the right bottom.
                  //
                  // 2*---*3
                  //  |   |
                  // 1*---*0
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
                  // This builds the vertices starting at the right bottom.
                  //
                  // 2*
                  //  | \
                  //  |   \
                  // 1*-----*0
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

      static void AddGeodesicFaces(
         Double diameter,
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
         // These contain the indices in the octant.
         // Note: Only the lower left half of the array is needed.
         //
         //  *
         //  | \
         //  |   \
         //  *-----*
         Int32[,] level = new Int32[geodesic + 1, geodesic + 1];
         Int32[,] uvLevel = new Int32[geodesic + 1, geodesic + 1];

         // This adds the left indices.
         for ( Int32 v = 0; v <= geodesic; v++ )
         {
            level[0, v] = left[v];
            uvLevel[0, v] = uvLeft[v];
         }

         // This adds the right indices.
         for ( Int32 v = 1; v <= geodesic; v++ )
         {
            level[v, v] = right[v];
            uvLevel[v, v] = uvRight[v];
         }

         // This adds the bottom indices.
         for ( Int32 h = 1; h < geodesic; h++ )
         {
            level[h, geodesic] = bottom[h];
            uvLevel[h, geodesic] = uvBottom[h];
         }

         // This adds the inner indices.
         // This creates the missing points in the octant.
         for ( Int32 v = 2; v < geodesic; v++ )
         {
            point end = points[level[v, v]].Normalize();
            point start = points[level[0, v]].Normalize();

            for ( Int32 h = 1; h < v; h++ )
            {
               point p = end * ((Double)h / v) + start * (1.0 - (Double)h / v);
               p = p.Normalize() * diameter / 2;

               level[h, v] = points.Count;
               points.Add(p);

               texcoord t = SphericalTexCoord(p);
               uvLevel[h, v] = texcoords.Count;
               texcoords.Add(t);
            }
         }

         // This builds the faces left to right from top to bottom.
         //
         //    *
         //    | \
         //    | 0 \
         //    *-----*
         //    | \ 2 | \
         //    | 1 \ | 3 \
         //    *-----*-----*
         //
         for ( Int32 j = 0; j < geodesic; j++ )
         {
            for ( Int32 i = 0; i <= j; i++ )
            {
               facedata f;
               pointdata leftTop;
               pointdata rightTop;
               pointdata rightBottom;
               pointdata leftBottom;

               // This creates the downward facing triangle.
               // *-----*
               //   \   |
               //     \ |
               //       *
               if ( i > 0 )
               {
                  f = new facedata();
                  f.numpoints = 3;
                  f.flags = facedataenum.hastexture;
                  f.matno = 0;
                  f.flatnormalno = -1;
                  f.pointdata = new pointdata[f.numpoints];

                  leftTop = new pointdata();
                  leftTop.pointindex = level[i - 1, j];
                  leftTop.texcoordindex = uvLevel[i - 1, j];

                  rightTop = new pointdata();
                  rightTop.pointindex = level[i, j];
                  rightTop.texcoordindex = uvLevel[i, j];

                  rightBottom = new pointdata();
                  rightBottom.pointindex = level[i, j + 1];
                  rightBottom.texcoordindex = uvLevel[i, j + 1];

                  if ( faceOrder )
                  {
                     f.pointdata[0] = leftTop;
                     f.pointdata[1] = rightTop;
                     f.pointdata[2] = rightBottom;
                  }
                  else
                  {
                     f.pointdata[0] = leftTop;
                     f.pointdata[1] = rightBottom;
                     f.pointdata[2] = rightTop;
                  }

                  facedatas.Add(f);
               }

               // This creates the upward facing triangle.
               // *
               // | \
               // |   \
               // *-----*
               f = new facedata();
               f.numpoints = 3;
               f.flags = facedataenum.hastexture;
               f.matno = 0;
               f.flatnormalno = -1;
               f.pointdata = new pointdata[f.numpoints];

               leftTop = new pointdata();
               leftTop.pointindex = level[i, j];
               leftTop.texcoordindex = uvLevel[i, j];

               rightBottom = new pointdata();
               rightBottom.pointindex = level[i + 1, j + 1];
               rightBottom.texcoordindex = uvLevel[i + 1, j + 1];

               leftBottom = new pointdata();
               leftBottom.pointindex = level[i, j + 1];
               leftBottom.texcoordindex = uvLevel[i, j + 1];

               if ( faceOrder )
               {
                  f.pointdata[0] = leftTop;
                  f.pointdata[1] = rightBottom;
                  f.pointdata[2] = leftBottom;
               }
               else
               {
                  f.pointdata[0] = leftTop;
                  f.pointdata[1] = leftBottom;
                  f.pointdata[2] = rightBottom;
               }

               facedatas.Add(f);
            }
         }
      }
   }
}
