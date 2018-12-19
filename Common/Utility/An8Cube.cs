using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Cube
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00. The
      /// degenerate cases will produce an empty mesh.
      /// </summary>
      /// <param name="c">the cube</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the cube converted to a mesh</returns>
      internal static mesh Calculate(cube c, Action<String> callback = null)
      {
         mesh m = new mesh();
         m.name = c?.name;
         m.@base = c?.@base;
         m.pivot = c?.pivot;
         m.material = c?.material;

         if ( c?.material?.name != null )
         {
            m.materiallist = new materiallist();
            m.materiallist.materialname = new @string[1];
            m.materiallist.materialname[0] = new @string();
            m.materiallist.materialname[0].text = c.material.name;
         }

         // Note: We will treat non-positive scales and non-positive divisions
         // as degenerate cases that shouldn't create any points (since they
         // do not create a 3D shape). Technically Anim8or v1.00 may still draw
         // lines or some faces.
         if ( c?.scale?.x > 0 &&
              c?.scale?.y > 0 &&
              c?.scale?.z > 0 &&
              c?.divisions?.x > 0 &&
              c?.divisions?.y > 0 &&
              c?.divisions?.z > 0 )
         {
            List<point> points = new List<point>();
            List<texcoord> texcoords = new List<texcoord>();
            List<facedata> facedatas = new List<facedata>();

            Double scaleX = c.scale.x;
            Double scaleY = c.scale.y;
            Double scaleZ = c.scale.z;

            // Note: Anim8or v1.00 limits divisions to 100
            Int32 divisionX = Math.Min((Int32)c.divisions.x, 100);
            Int32 divisionY = Math.Min((Int32)c.divisions.y, 100);
            Int32 divisionZ = Math.Min((Int32)c.divisions.z, 100);

            Double minX = scaleX / -2;
            Double minY = scaleY / -2;
            Double minZ = scaleZ / -2;
            Double stepX = scaleX / divisionX;
            Double stepY = scaleY / divisionY;
            Double stepZ = scaleZ / divisionZ;
            Double stepU = 1.0 / divisionX;
            Double stepV = 1.0 / divisionY;

            // This contains the indices of the right side.
            //
            // [0,0]  [1,0]  [2,0]
            //   *------*------*               Y
            //   |      |      |               | X
            // [0,1]  [1,1]  [2,1]             |/
            //   *------*------*               *------Z
            //   |      |      |
            // [0,2]  [1,2]  [2,2]
            //   *------*------*
            //
            Int32[,] rightIndices = new Int32[divisionZ + 1, divisionY + 1];

            // This contains the indices of the left side.
            //
            // [0,0]  [1,0]  [2,0]
            //   *------*------*               Y
            //   |      |      |               |
            // [0,1]  [1,1]  [2,1]             |
            //   *------*------*        Z------*
            //   |      |      |              /
            // [0,2]  [1,2]  [2,2]           X
            //   *------*------*
            //
            Int32[,] leftIndices = new Int32[divisionZ + 1, divisionY + 1];

            // This contains the indices of the bottom side.
            //
            // [0,0]  [1,0]  [2,0]
            //   *------*------*          Z
            //   |      |      |          | Y
            // [0,1]  [1,1]  [2,1]        |/
            //   *------*------*          *------X
            //   |      |      |
            // [0,2]  [1,2]  [2,2]
            //   *------*------*
            //
            Int32[,] bottomIndices = new Int32[divisionX + 1, divisionZ + 1];

            // This contains the indices of the top side.
            //
            // [0,0]  [1,0]  [2,0]
            //   *------*------*
            //   |      |      |
            // [0,1]  [1,1]  [2,1]
            //   *------*------*          *------X
            //   |      |      |         /|
            // [0,2]  [1,2]  [2,2]      Y |
            //   *------*------*          Z
            //
            Int32[,] topIndices = new Int32[divisionX + 1, divisionZ + 1];

            // This contains the indices of the back side.
            //
            // [0,0]  [1,0]  [2,0]
            //   *------*------*               Y
            //   |      |      |               | Z
            // [0,1]  [1,1]  [2,1]             |/
            //   *------*------*        X------*
            //   |      |      |
            // [0,2]  [1,2]  [2,2]
            //   *------*------*
            //
            Int32[,] backIndices = new Int32[divisionX + 1, divisionY + 1];

            // This contains the indices of the front side.
            //
            // [0,0]  [1,0]  [2,0]
            //   *------*------*          Y
            //   |      |      |          |
            // [0,1]  [1,1]  [2,1]        |
            //   *------*------*          *------X
            //   |      |      |         /
            // [0,2]  [1,2]  [2,2]      Z
            //   *------*------*
            //
            Int32[,] frontIndices = new Int32[divisionX + 1, divisionY + 1];

            // Create all back side and front side points (interleaved).
            //
            //     4*----10*----16* (back)
            //      |      |      |
            //      |      |      |          Y
            //     2*-----8*----14*          |
            // 5*----11*----17*   |          |
            //  |   |  |   |  |   |          *------X
            //  |  0*--|--6*--|-12*         /
            // 3*-----9*----15*            Z
            //  |      |      |
            //  |      |      |
            // 1*-----7*----13* (front)
            //
            for ( Int32 ix = 0; ix <= divisionX; ix++ )
            {
               Double x = minX + stepX * ix;
               Double u = stepU * ix;

               for ( Int32 iy = 0; iy <= divisionY; iy++ )
               {
                  Double y = minY + stepY * iy;
                  Double v = stepV * iy;

                  for ( Int32 iz = 0; iz <= divisionZ; iz += divisionZ )
                  {
                     Double z = minZ + stepZ * iz;

                     #region Save indices for later
                     if ( ix == 0 )
                     {
                        rightIndices[iz, divisionY - iy] = points.Count;
                     }
                     else if ( ix == divisionX )
                     {
                        leftIndices[divisionZ - iz, divisionY - iy] =
                           points.Count;
                     }

                     if ( iy == 0 )
                     {
                        bottomIndices[ix, divisionZ - iz] = points.Count;
                     }
                     else if ( iy == divisionY )
                     {
                        topIndices[ix, iz] = points.Count;
                     }

                     if ( iz == 0 )
                     {
                        backIndices[divisionX - ix, divisionY - iy] =
                           points.Count;
                     }
                     else if ( iz == divisionZ )
                     {
                        frontIndices[ix, divisionY - iy] = points.Count;
                     }
                     #endregion

                     points.Add(new point(x, y, z));
                     texcoords.Add(new texcoord(u, v));
                  }
               }
            }

            // Create the right side and left side points (interleaved) except
            // for the back and front side points already created.
            //
            //      * (right)     * (left)
            //     /|            /|
            //   4* |          5* |          Y
            //   /| *          /| *          |
            //  * |/|         * |/|          |
            //  |2* |         |3* |          *------X
            //  |/| *         |/| *         /
            //  * |/          * |/         Z
            //  |0*           |1*
            //  |/            |/
            //  *             *
            //
            for ( Int32 iy = 0; iy <= divisionY; iy++ )
            {
               Double y = minY + stepY * iy;
               Double v = stepV * iy;

               for ( Int32 iz = 1; iz < divisionZ; iz++ )
               {
                  Double z = minZ + stepZ * iz;

                  for ( Int32 ix = 0; ix <= divisionX; ix += divisionX )
                  {
                     Double x = minX + stepX * ix;
                     Double u = stepU * ix;

                     #region Save indices for later
                     if ( ix == 0 )
                     {
                        rightIndices[iz, divisionY - iy] = points.Count;
                     }
                     else if ( ix == divisionX )
                     {
                        leftIndices[divisionZ - iz, divisionY - iy] =
                           points.Count;
                     }

                     if ( iy == 0 )
                     {
                        bottomIndices[ix, divisionZ - iz] = points.Count;
                     }
                     else if ( iy == divisionY )
                     {
                        topIndices[ix, iz] = points.Count;
                     }
                     #endregion

                     points.Add(new point(x, y, z));
                     texcoords.Add(new texcoord(u, v));
                  }
               }
            }

            // Create the bottom side and top side points (interleaved) except
            // for right, left, back, and front side points already created.
            //
            //      *------*------* (top)
            //     /      /      /
            //    *-----1*------*            Y
            //   /      /      /             |
            //  *------*------*              |
            //                               *------X
            //      *------*------*         /
            //     /      /      /         Z
            //    *-----0*------*
            //   /      /      /
            //  *------*------* (bottom)
            //
            for ( Int32 ix = 1; ix < divisionX; ix++ )
            {
               Double x = minX + stepX * ix;
               Double u = stepU * ix;

               for ( Int32 iz = 1; iz < divisionZ; iz++ )
               {
                  Double z = minZ + stepZ * iz;

                  for ( Int32 iy = 0; iy <= divisionY; iy += divisionY )
                  {
                     Double y = minY + stepY * iy;
                     Double v = stepV * iy;

                     #region Save indices for later
                     if ( iy == 0 )
                     {
                        bottomIndices[ix, divisionZ - iz] = points.Count;
                     }
                     else if ( iy == divisionY )
                     {
                        topIndices[ix, iz] = points.Count;
                     }
                     #endregion

                     points.Add(new point(x, y, z));
                     texcoords.Add(new texcoord(u, v));
                  }
               }
            }

            // Create the back side faces
            AddFaces(
               facedatas,
               backIndices,
               BuildSideFrom.BottomToTopRightToLeft,
               BuildFaceStartingAt.RightBottom);

            // Create the front side faces
            AddFaces(
               facedatas,
               frontIndices,
               BuildSideFrom.BottomToTopLeftToRight,
               BuildFaceStartingAt.LeftBottom);

            // Create the right side faces
            AddFaces(
               facedatas,
               rightIndices,
               BuildSideFrom.LeftToRightBottomToTop,
               BuildFaceStartingAt.LeftBottom);

            // Create the left side faces
            AddFaces(
               facedatas,
               leftIndices,
               BuildSideFrom.RightToLeftBottomToTop,
               BuildFaceStartingAt.RightBottom);

            // Create the top side faces
            AddFaces(
               facedatas,
               topIndices,
               BuildSideFrom.TopToBottomLeftToRight,
               BuildFaceStartingAt.LeftTop);

            // Create the bottom side faces
            AddFaces(
               facedatas,
               bottomIndices,
               BuildSideFrom.BottomToTopLeftToRight,
               BuildFaceStartingAt.LeftBottom);

            // Add the points, texcoords, and faces to the mesh
            m.points = new points();
            m.points.point = points.ToArray();
            m.texcoords = new texcoords();
            m.texcoords.texcoord = texcoords.ToArray();
            m.faces = new faces();
            m.faces.facedata = facedatas.ToArray();
         }
         else if ( callback != null )
         {
            if ( c == null )
            {
               callback("The cube is null. No points will be created.");
            }
            else
            {
               if ( c.scale == null )
               {
                  callback($"The \"{c.name?.text}\" cube's scale is null. No points will be created.");
               }
               else
               {
                  if ( c.scale.x <= 0 )
                  {
                     callback($"The \"{c.name?.text}\" cube's X scale (i.e. {c.scale.x}) is not positive. No points will be created.");
                  }

                  if ( c.scale.y <= 0 )
                  {
                     callback($"The \"{c.name?.text}\" cube's Y scale (i.e. {c.scale.y}) is not positive. No points will be created.");
                  }

                  if ( c.scale.z <= 0 )
                  {
                     callback($"The \"{c.name?.text}\" cube's Z scale (i.e. {c.scale.z}) is not positive. No points will be created.");
                  }
               }

               if ( c.divisions == null )
               {
                  callback($"The \"{c.name?.text}\" cube's divisions is null. No points will be created.");
               }
               else
               {
                  if ( c.divisions.x <= 0 )
                  {
                     callback($"The \"{c.name?.text}\" cube's X divisions (i.e. {c.divisions.x}) is not positive. No points will be created.");
                  }

                  if ( c.divisions.y <= 0 )
                  {
                     callback($"The \"{c.name?.text}\" cube's Y divisions (i.e. {c.divisions.y}) is not positive. No points will be created.");
                  }

                  if ( c.divisions.z <= 0 )
                  {
                     callback($"The \"{c.name?.text}\" cube's Z divisions (i.e. {c.divisions.z}) is not positive. No points will be created.");
                  }
               }
            }
         }

         return m;
      }

      enum BuildSideFrom
      {
         // This builds the faces top to bottom from left to right.
         //
         //  *------*------*
         //  |      |      |
         //  |   0  |   2  |
         //  *------*------*
         //  |      |      |
         //  |   1  |   3  |
         //  *------*------*
         TopToBottomLeftToRight,

         // This builds the faces top to bottom from right to left.
         //
         //  *------*------*
         //  |      |      |
         //  |   2  |   0  |
         //  *------*------*
         //  |      |      |
         //  |   3  |   1  |
         //  *------*------*
         TopToBottomRightToLeft,

         // This builds the faces bottom to top from left to right.
         //
         //  *------*------*
         //  |      |      |
         //  |   1  |   3  |
         //  *------*------*
         //  |      |      |
         //  |   0  |   2  |
         //  *------*------*
         BottomToTopLeftToRight,

         // This builds the faces bottom to top from right to left.
         //
         //  *------*------*
         //  |      |      |
         //  |   3  |   1  |
         //  *------*------*
         //  |      |      |
         //  |   2  |   0  |
         //  *------*------*
         BottomToTopRightToLeft,

         // This builds the faces left to right from top to bottom.
         //
         //  *------*------*
         //  |      |      |
         //  |   0  |   1  |
         //  *------*------*
         //  |      |      |
         //  |   2  |   3  |
         //  *------*------*
         LeftToRightTopToBottom,

         // This builds the faces left to right from bottom to top.
         //
         //  *------*------*
         //  |      |      |
         //  |   2  |   3  |
         //  *------*------*
         //  |      |      |
         //  |   0  |   1  |
         //  *------*------*
         LeftToRightBottomToTop,

         // This builds the faces right to left from top to bottom.
         //
         //  *------*------*
         //  |      |      |
         //  |   1  |   0  |
         //  *------*------*
         //  |      |      |
         //  |   3  |   2  |
         //  *------*------*
         RightToLeftTopToBottom,

         // This builds the faces right to left from bottom to top.
         //
         //  *------*------*
         //  |      |      |
         //  |   3  |   2  |
         //  *------*------*
         //  |      |      |
         //  |   1  |   0  |
         //  *------*------*
         RightToLeftBottomToTop,
      }

      enum BuildFaceStartingAt
      {
         // This builds the vertices clockwise starting at the left top.
         //
         // 0*-----1*
         //  |      |
         //  |      |
         // 3*-----2*
         LeftTop,

         // This builds the vertices clockwise starting at the right top.
         //
         // 3*-----0*
         //  |      |
         //  |      |
         // 2*-----1*
         RightTop,

         // This builds the vertices clockwise starting at the right bottom.
         //
         // 2*-----3*
         //  |      |
         //  |      |
         // 1*-----0*
         RightBottom,

         // This builds the vertices clockwise starting at the left bottom.
         //
         // 1*-----2*
         //  |      |
         //  |      |
         // 0*-----3*
         LeftBottom,
      }

      static void AddFaces(
         List<facedata> facedatas,
         Int32[,] indices,
         BuildSideFrom buildSideFrom,
         BuildFaceStartingAt buildFaceStartingAt)
      {
         switch ( buildSideFrom )
         {
         case BuildSideFrom.TopToBottomLeftToRight:
         default:
            for ( Int32 i = 0; i < indices.GetLength(0) - 1; i++ )
            {
               for ( Int32 j = 0; j < indices.GetLength(1) - 1; j++ )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.TopToBottomRightToLeft:
            for ( Int32 i = indices.GetLength(0) - 2; i >= 0; i-- )
            {
               for ( Int32 j = 0; j < indices.GetLength(1) - 1; j++ )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.BottomToTopLeftToRight:
            for ( Int32 i = 0; i < indices.GetLength(0) - 1; i++ )
            {
               for ( Int32 j = indices.GetLength(1) - 2; j >= 0; j-- )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.BottomToTopRightToLeft:
            for ( Int32 i = indices.GetLength(0) - 2; i >= 0; i-- )
            {
               for ( Int32 j = indices.GetLength(1) - 2; j >= 0; j-- )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.LeftToRightTopToBottom:
            for ( Int32 j = 0; j < indices.GetLength(1) - 1; j++ )
            {
               for ( Int32 i = 0; i < indices.GetLength(0) - 1; i++ )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.LeftToRightBottomToTop:
            for ( Int32 j = indices.GetLength(1) - 2; j >= 0; j-- )
            {
               for ( Int32 i = 0; i < indices.GetLength(0) - 1; i++ )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.RightToLeftTopToBottom:
            for ( Int32 j = 0; j < indices.GetLength(1) - 1; j++ )
            {
               for ( Int32 i = indices.GetLength(0) - 2; i >= 0; i-- )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         case BuildSideFrom.RightToLeftBottomToTop:
            for ( Int32 j = indices.GetLength(1) - 2; j >= 0; j-- )
            {
               for ( Int32 i = indices.GetLength(0) - 2; i >= 0; i-- )
               {
                  AddFace(facedatas, indices, buildFaceStartingAt, i, j);
               }
            }
            break;
         }
      }

      static void AddFace(
         List<facedata> facedatas,
         Int32[,] indices,
         BuildFaceStartingAt buildFaceStartingAt,
         Int32 i,
         Int32 j)
      {
         pointdata leftTop = new pointdata();
         leftTop.pointindex = indices[i, j];
         leftTop.normalindex = 0;
         leftTop.texcoordindex = leftTop.pointindex;

         pointdata rightTop = new pointdata();
         rightTop.pointindex = indices[i + 1, j];
         rightTop.normalindex = 0;
         rightTop.texcoordindex = rightTop.pointindex;

         pointdata rightBottom = new pointdata();
         rightBottom.pointindex = indices[i + 1, j + 1];
         rightBottom.normalindex = 0;
         rightBottom.texcoordindex = rightBottom.pointindex;

         pointdata leftBottom = new pointdata();
         leftBottom.pointindex = indices[i, j + 1];
         leftBottom.normalindex = 0;
         leftBottom.texcoordindex = leftBottom.pointindex;

         pointdata[] pointdata;

         switch ( buildFaceStartingAt )
         {
         case BuildFaceStartingAt.LeftTop:
         default:
            pointdata = new pointdata[]
            {
               leftTop,
               rightTop,
               rightBottom,
               leftBottom,
            };
            break;
         case BuildFaceStartingAt.RightTop:
            pointdata = new pointdata[]
            {
               rightTop,
               rightBottom,
               leftBottom,
               leftTop,
            };
            break;
         case BuildFaceStartingAt.RightBottom:
            pointdata = new pointdata[]
            {
               rightBottom,
               leftBottom,
               leftTop,
               rightTop,
            };
            break;
         case BuildFaceStartingAt.LeftBottom:
            pointdata = new pointdata[]
            {
               leftBottom,
               leftTop,
               rightTop,
               rightBottom,
            };
            break;
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
