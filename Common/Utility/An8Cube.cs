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

namespace Anim8orTransl8or.Utility
{
   static class An8Cube
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="c">the cube</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the calculated mesh</returns>
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

         List<point> points = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // Note: These defaults and limits were reversed engineered.
         Double scaleX = (c?.scale?.x ?? 10).Limit(0);
         Double scaleY = (c?.scale?.y ?? 10).Limit(0);
         Double scaleZ = (c?.scale?.z ?? 10).Limit(0);
         Int32 divisionsX = (Int32)(c?.divisions?.x ?? 1).Limit(1, 100);
         Int32 divisionsY = (Int32)(c?.divisions?.y ?? 1).Limit(1, 100);
         Int32 divisionsZ = (Int32)(c?.divisions?.z ?? 1).Limit(1, 100);

         if ( callback != null )
         {
            if ( c?.scale != null )
            {
               if ( c.scale.x != scaleX )
               {
                  callback($"The \"{c.name?.text}\" cube's X scale of {c.scale.x} has been limited to {scaleX}.");
               }

               if ( c.scale.y != scaleY )
               {
                  callback($"The \"{c.name?.text}\" cube's Y scale of {c.scale.y} has been limited to {scaleY}.");
               }

               if ( c.scale.z != scaleZ )
               {
                  callback($"The \"{c.name?.text}\" cube's Z scale of {c.scale.z} has been limited to {scaleZ}.");
               }
            }

            if ( c?.divisions != null )
            {
               if ( c.divisions.x != divisionsX )
               {
                  callback($"The \"{c.name?.text}\" cube's X divisions of {c.divisions.x} has been limited to {divisionsX}.");
               }

               if ( c.divisions.y != divisionsY )
               {
                  callback($"The \"{c.name?.text}\" cube's Y divisions of {c.divisions.y} has been limited to {divisionsY}.");
               }

               if ( c.divisions.z != divisionsZ )
               {
                  callback($"The \"{c.name?.text}\" cube's Z divisions of {c.divisions.z} has been limited to {divisionsZ}.");
               }
            }
         }

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
         Int32[,] rightIndices = new Int32[divisionsZ + 1, divisionsY + 1];

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
         Int32[,] leftIndices = new Int32[divisionsZ + 1, divisionsY + 1];

         // This contains the indices of the bottom side.
         //
         // [0,0]  [1,0]  [2,0]
         //   *------*------*               Z
         //   |      |      |               | Y
         // [0,1]  [1,1]  [2,1]             |/
         //   *------*------*               *------X
         //   |      |      |
         // [0,2]  [1,2]  [2,2]
         //   *------*------*
         //
         Int32[,] bottomIndices = new Int32[divisionsX + 1, divisionsZ + 1];

         // This contains the indices of the top side.
         //
         // [0,0]  [1,0]  [2,0]
         //   *------*------*
         //   |      |      |
         // [0,1]  [1,1]  [2,1]
         //   *------*------*               *------X
         //   |      |      |              /|
         // [0,2]  [1,2]  [2,2]           Y |
         //   *------*------*               Z
         //
         Int32[,] topIndices = new Int32[divisionsX + 1, divisionsZ + 1];

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
         Int32[,] backIndices = new Int32[divisionsX + 1, divisionsY + 1];

         // This contains the indices of the front side.
         //
         // [0,0]  [1,0]  [2,0]
         //   *------*------*               Y
         //   |      |      |               |
         // [0,1]  [1,1]  [2,1]             |
         //   *------*------*               *------X
         //   |      |      |              /
         // [0,2]  [1,2]  [2,2]           Z
         //   *------*------*
         //
         Int32[,] frontIndices = new Int32[divisionsX + 1, divisionsY + 1];

         // This creates all back side and front side points (interleaved).
         //
         //     4*----10*----16* (back)
         //      |      |      |
         //      |      |      |            Y
         //     2*-----8*----14*            |
         // 5*----11*----17*   |            |
         //  |   |  |   |  |   |            *------X
         //  |  0*--|--6*--|-12*           /
         // 3*-----9*----15*              Z
         //  |      |      |
         //  |      |      |
         // 1*-----7*----13* (front)
         //
         for ( Int32 ix = 0; ix <= divisionsX; ix++ )
         {
            Double x = ((Double)ix / divisionsX - 0.5) * scaleX;
            Double u = (Double)ix / divisionsX;

            for ( Int32 iy = 0; iy <= divisionsY; iy++ )
            {
               Double y = ((Double)iy / divisionsY - 0.5) * scaleY;
               Double v = (Double)iy / divisionsY;

               for ( Int32 iz = 0; iz <= divisionsZ; iz += divisionsZ )
               {
                  Double z = ((Double)iz / divisionsZ - 0.5) * scaleZ;

                  #region Save indices for later
                  if ( ix == 0 )
                  {
                     rightIndices[iz, divisionsY - iy] = points.Count;
                  }
                  else if ( ix == divisionsX )
                  {
                     leftIndices[divisionsZ - iz, divisionsY - iy] =
                        points.Count;
                  }

                  if ( iy == 0 )
                  {
                     bottomIndices[ix, divisionsZ - iz] = points.Count;
                  }
                  else if ( iy == divisionsY )
                  {
                     topIndices[ix, iz] = points.Count;
                  }

                  if ( iz == 0 )
                  {
                     backIndices[divisionsX - ix, divisionsY - iy] =
                        points.Count;
                  }
                  else if ( iz == divisionsZ )
                  {
                     frontIndices[ix, divisionsY - iy] = points.Count;
                  }
                  #endregion

                  points.Add(new point(x, y, z));
                  texcoords.Add(new texcoord(u, v));
               }
            }
         }

         // This creates the right side and left side points except for the
         // back and front side points already created.
         //
         //      * (right)     * (left)
         //     /|            /|
         //   4* |          5* |            Y
         //   /| *          /| *            |
         //  * |/|         * |/|            |
         //  |2* |         |3* |            *------X
         //  |/| *         |/| *           /
         //  * |/          * |/           Z
         //  |0*           |1*
         //  |/            |/
         //  *             *
         //
         for ( Int32 iy = 0; iy <= divisionsY; iy++ )
         {
            Double y = ((Double)iy / divisionsY - 0.5) * scaleY;
            Double v = (Double)iy / divisionsY;

            for ( Int32 iz = 1; iz < divisionsZ; iz++ )
            {
               Double z = ((Double)iz / divisionsZ - 0.5) * scaleZ;

               for ( Int32 ix = 0; ix <= divisionsX; ix += divisionsX )
               {
                  Double x = ((Double)ix / divisionsX - 0.5) * scaleX;
                  Double u = (Double)ix / divisionsX;

                  #region Save indices for later
                  if ( ix == 0 )
                  {
                     rightIndices[iz, divisionsY - iy] = points.Count;
                  }
                  else if ( ix == divisionsX )
                  {
                     leftIndices[divisionsZ - iz, divisionsY - iy] =
                        points.Count;
                  }

                  if ( iy == 0 )
                  {
                     bottomIndices[ix, divisionsZ - iz] = points.Count;
                  }
                  else if ( iy == divisionsY )
                  {
                     topIndices[ix, iz] = points.Count;
                  }
                  #endregion

                  points.Add(new point(x, y, z));
                  texcoords.Add(new texcoord(u, v));
               }
            }
         }

         // This creates the bottom side and top side points except for right,
         // left, back, and front side points already created.
         //
         //      *------*------* (top)
         //     /      /      /
         //    *-----1*------*              Y
         //   /      /      /               |
         //  *------*------*                |
         //                                 *------X
         //      *------*------*           /
         //     /      /      /           Z
         //    *-----0*------*
         //   /      /      /
         //  *------*------* (bottom)
         //
         for ( Int32 ix = 1; ix < divisionsX; ix++ )
         {
            Double x = ((Double)ix / divisionsX - 0.5) * scaleX;
            Double u = (Double)ix / divisionsX;

            for ( Int32 iz = 1; iz < divisionsZ; iz++ )
            {
               Double z = ((Double)iz / divisionsZ - 0.5) * scaleZ;

               for ( Int32 iy = 0; iy <= divisionsY; iy += divisionsY )
               {
                  Double y = ((Double)iy / divisionsY - 0.5) * scaleY;
                  Double v = (Double)iy / divisionsY;

                  #region Save indices for later
                  if ( iy == 0 )
                  {
                     bottomIndices[ix, divisionsZ - iz] = points.Count;
                  }
                  else if ( iy == divisionsY )
                  {
                     topIndices[ix, iz] = points.Count;
                  }
                  #endregion

                  points.Add(new point(x, y, z));
                  texcoords.Add(new texcoord(u, v));
               }
            }
         }

         // This creates the back side faces.
         AddFaces(
            facedatas,
            backIndices,
            BuildSideFrom.BottomToTopRightToLeft,
            BuildFaceStartingAt.RightBottom);

         // This creates the front side faces.
         AddFaces(
            facedatas,
            frontIndices,
            BuildSideFrom.BottomToTopLeftToRight,
            BuildFaceStartingAt.LeftBottom);

         // This creates the right side faces.
         AddFaces(
            facedatas,
            rightIndices,
            BuildSideFrom.LeftToRightBottomToTop,
            BuildFaceStartingAt.LeftBottom);

         // This creates the left side faces.
         AddFaces(
            facedatas,
            leftIndices,
            BuildSideFrom.RightToLeftBottomToTop,
            BuildFaceStartingAt.RightBottom);

         // This creates the top side faces.
         AddFaces(
            facedatas,
            topIndices,
            BuildSideFrom.TopToBottomLeftToRight,
            BuildFaceStartingAt.LeftTop);

         // This creates the bottom side faces.
         AddFaces(
            facedatas,
            bottomIndices,
            BuildSideFrom.BottomToTopLeftToRight,
            BuildFaceStartingAt.LeftBottom);

         m.points = new points();
         m.points.point = points.ToArray();
         m.texcoords = new texcoords();
         m.texcoords.texcoord = texcoords.ToArray();
         m.faces = new faces();
         m.faces.facedata = facedatas.ToArray();

         return m;
      }

      enum BuildSideFrom
      {
         // This builds the faces of the side left to right from top to bottom.
         //
         //  *---*---*
         //  | 0 | 1 |
         //  *---*---*
         //  | 2 | 3 |
         //  *---*---*
         LeftToRightTopToBottom,

         // This builds the faces of the side left to right from bottom to top.
         //
         //  *---*---*
         //  | 2 | 3 |
         //  *---*---*
         //  | 0 | 1 |
         //  *---*---*
         LeftToRightBottomToTop,

         // This builds the faces of the side right to left from top to bottom.
         //
         //  *---*---*
         //  | 1 | 0 |
         //  *---*---*
         //  | 3 | 2 |
         //  *---*---*
         RightToLeftTopToBottom,

         // This builds the faces of the side right to left from bottom to top.
         //
         //  *---*---*
         //  | 3 | 2 |
         //  *---*---*
         //  | 1 | 0 |
         //  *---*---*
         RightToLeftBottomToTop,

         // This builds the faces of the side top to bottom from left to right.
         //
         //  *---*---*
         //  | 0 | 2 |
         //  *---*---*
         //  | 1 | 3 |
         //  *---*---*
         TopToBottomLeftToRight,

         // This builds the faces of the side top to bottom from right to left.
         //
         //  *---*---*
         //  | 2 | 0 |
         //  *---*---*
         //  | 3 | 1 |
         //  *---*---*
         TopToBottomRightToLeft,

         // This builds the faces of the side bottom to top from left to right.
         //
         //  *---*---*
         //  | 1 | 3 |
         //  *---*---*
         //  | 0 | 2 |
         //  *---*---*
         BottomToTopLeftToRight,

         // This builds the faces of the side bottom to top from right to left.
         //
         //  *---*---*
         //  | 3 | 1 |
         //  *---*---*
         //  | 2 | 0 |
         //  *---*---*
         BottomToTopRightToLeft,
      }

      enum BuildFaceStartingAt
      {
         // This builds the vertices clockwise starting at the left top.
         //
         // 0*---*1
         //  |   |
         // 3*---*2
         LeftTop,

         // This builds the vertices clockwise starting at the right top.
         //
         // 3*---*0
         //  |   |
         // 2*---*1
         RightTop,

         // This builds the vertices clockwise starting at the right bottom.
         //
         // 2*---*3
         //  |   |
         // 1*---*0
         RightBottom,

         // This builds the vertices clockwise starting at the left bottom.
         //
         // 1*---*2
         //  |   |
         // 0*---*3
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
         case BuildSideFrom.LeftToRightTopToBottom:
         default:
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
         case BuildSideFrom.TopToBottomLeftToRight:
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
