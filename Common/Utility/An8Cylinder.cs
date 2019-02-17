// Copyright © 2018 Contingent Games.
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
   static class An8Cylinder
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="c">the cylinder</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the calculated mesh</returns>
      internal static mesh Calculate(
         cylinder c,
         Action<String> callback = null)
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
         Double length = (c?.length?.text ?? 1).Limit(0);
         Double startDiameter = (c?.diameter?.text ?? 1).Limit(0);
         Double endDiameter = (c?.topdiameter?.text ?? 1).Limit(0);
         Int32 longitude = (Int32)(c?.longlat?.longitude ?? 12).Limit(3, 100);
         Int32 latitude = (Int32)(c?.longlat?.latitude ?? 8).Limit(1, 100);

         if ( callback != null )
         {
            if ( c?.length != null && c.length.text != length )
            {
               callback($"The \"{c.name?.text}\" cylinder's length of {c.length.text} has been limited to {length}.");
            }

            if ( c?.diameter != null && c.diameter.text != startDiameter )
            {
               callback($"The \"{c.name?.text}\" cylinder's diameter of {c.diameter.text} has been limited to {startDiameter}.");
            }

            if ( c?.topdiameter != null && c.topdiameter.text != endDiameter )
            {
               callback($"The \"{c.name?.text}\" cylinder's top diameter of {c.topdiameter.text} has been limited to {endDiameter}.");
            }

            if ( c?.longlat != null )
            {
               if ( c.longlat.longitude != longitude )
               {
                  callback($"The \"{c.name?.text}\" cylinder's longitude of {c.longlat.longitude} has been limited to {longitude}.");
               }

               if ( c.longlat.latitude != latitude )
               {
                  callback($"The \"{c.name?.text}\" cylinder's latitude of {c.longlat.latitude} has been limited to {latitude}.");
               }
            }
         }

         // This creates all the points.
         //
         //   5*----3*
         //   /       \
         // 7*        1*
         //  |\       /|
         //  |9*---11* |
         //  | |     | |                    Y
         //  |4|----2| |                    |
         //  |/|     |\|                    |
         // 6* |     |0*                    *------X
         //   \|     |/                    /
         //   8*---10*                    Z
         //
         for ( Int32 ix = 0; ix <= longitude; ix++ )
         {
            Double x = Math.Cos(ix * 2 * Math.PI / longitude);
            Double z = -Math.Sin(ix * 2 * Math.PI / longitude);
            Double u = (Double)ix / longitude;

            for ( Int32 iy = 0; iy <= latitude; iy++ )
            {
               Double y = (Double)iy / latitude * length;
               Double v = (Double)iy / latitude;
               Double radius = (endDiameter * v + startDiameter * (1 - v)) / 2;

               // Don't create the last vertical slice of points, since they
               // are the same as the first
               if ( ix < longitude )
               {
                  points.Add(new point(x * radius, y, z * radius));
               }

               texcoords.Add(new texcoord(u, v));
            }
         }

         facedataenum flags = facedataenum.hastexture;

         if ( c?.capstart == null || c?.capend == null )
         {
            // Show the back face if at least one end cap is missing
            // Note: Presumably, you only need to show the back face when one
            // of the cylinder ends is "open". Anim8or is smart enough to
            // detect that it does not need to draw a zero-sized face when an
            // end diameter is zero, but for some reason, it still wants to
            // draw the back face.
            flags |= facedataenum.showback;
         }

         // This creates the vertical faces.
         AddVerticalFaces(
            facedatas,
            longitude,
            latitude,
            flags,
            BuildSideFrom.BottomToTop,
            endDiameter <= startDiameter ?
               BuildFaceStartingAt.RightBottom :
               BuildFaceStartingAt.LeftBottom);

         // This creates the top face.
         if ( c?.capend != null && endDiameter > 0 )
         {
            // Note: Anim8or detects that it does not need to create the top
            // face when the end diameter is 0.
            AddTopFace(facedatas, longitude, latitude, flags);
         }

         // This creates the bottom face.
         if ( c?.capstart != null && startDiameter > 0 )
         {
            // Note: Anim8or detects that it does not need to create the bottom
            // face when the start diameter is 0.
            AddBottomFace(facedatas, longitude, latitude, flags);
         }

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
         // This builds the faces of the side top to bottom.
         //
         //  *---*
         //  | 0 |
         //  *---*
         //  | 1 |
         //  *---*
         TopToBottom,

         // This builds the faces of the side bottom to top.
         //
         //  *---*
         //  | 1 |
         //  *---*
         //  | 0 |
         //  *---*
         BottomToTop,
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

      static void AddVerticalFaces(
         List<facedata> facedatas,
         Int32 longitude,
         Int32 latitude,
         facedataenum flags,
         BuildSideFrom buildSideFrom,
         BuildFaceStartingAt buildFaceStartingAt)
      {
         switch ( buildSideFrom )
         {
         case BuildSideFrom.TopToBottom:
         default:
            for ( Int32 i = 0; i < longitude; i++ )
            {
               for ( Int32 j = 0; j < latitude; j++ )
               {
                  AddVerticalFace(
                     facedatas,
                     longitude,
                     latitude,
                     flags,
                     buildFaceStartingAt,
                     i,
                     j);
               }
            }
            break;
         case BuildSideFrom.BottomToTop:
            for ( Int32 i = 0; i < longitude; i++ )
            {
               for ( Int32 j = latitude - 1; j >= 0; j-- )
               {
                  AddVerticalFace(
                     facedatas,
                     longitude,
                     latitude,
                     flags,
                     buildFaceStartingAt,
                     i,
                     j);
               }
            }
            break;
         }
      }

      static void AddVerticalFace(
         List<facedata> facedatas,
         Int32 longitude,
         Int32 latitude,
         facedataenum flags,
         BuildFaceStartingAt buildFaceStartingAt,
         Int32 i,
         Int32 j)
      {
         pointdata leftTop = new pointdata();
         leftTop.pointindex = i * (latitude + 1) + latitude - j;
         leftTop.normalindex = 0;
         leftTop.texcoordindex = leftTop.pointindex;

         pointdata rightTop = new pointdata();
         rightTop.pointindex = (i + 1) * (latitude + 1) + latitude - j;
         rightTop.normalindex = 0;
         rightTop.texcoordindex = rightTop.pointindex;

         pointdata rightBottom = new pointdata();
         rightBottom.pointindex = (i + 1) * (latitude + 1) + latitude - j - 1;
         rightBottom.normalindex = 0;
         rightBottom.texcoordindex = rightBottom.pointindex;

         pointdata leftBottom = new pointdata();
         leftBottom.pointindex = i * (latitude + 1) + latitude - j - 1;
         leftBottom.normalindex = 0;
         leftBottom.texcoordindex = leftBottom.pointindex;

         Int64 FixIndex(Int64 index)
         {
            // If the first or middle vertical slices
            if ( index < (latitude + 1) * longitude )
            {
               // Return the vertical slice unchanged
               return index;
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
         facedata.flags = flags;
         facedata.matno = 0;
         facedata.flatnormalno = -1;
         facedata.pointdata = pointdata;

         facedatas.Add(facedata);
      }

      static void AddTopFace(
         List<facedata> facedatas,
         Int32 longitude,
         Int32 latitude,
         facedataenum flags)
      {
         pointdata[] pointdata = new pointdata[longitude];

         for ( Int32 i = 0; i < longitude; i++ )
         {
            pointdata data = new pointdata();
            data.pointindex = (longitude - 1 - i) * (latitude + 1) + latitude;
            data.normalindex = 0;
            data.texcoordindex = data.pointindex;

            pointdata[i] = data;
         }

         facedata facedata = new facedata();
         facedata.numpoints = pointdata.Length;
         facedata.flags = flags;
         facedata.matno = 0;
         facedata.flatnormalno = -1;
         facedata.pointdata = pointdata;

         facedatas.Add(facedata);
      }

      static void AddBottomFace(
         List<facedata> facedatas,
         Int32 longitude,
         Int32 latitude,
         facedataenum flags)
      {
         pointdata[] pointdata = new pointdata[longitude];

         for ( Int32 i = 0; i < longitude; i++ )
         {
            pointdata data = new pointdata();
            data.pointindex = i * (latitude + 1);
            data.normalindex = 0;
            data.texcoordindex = data.pointindex;

            pointdata[i] = data;
         }

         facedata facedata = new facedata();
         facedata.numpoints = pointdata.Length;
         facedata.flags = flags;
         facedata.matno = 0;
         facedata.flatnormalno = -1;
         facedata.pointdata = pointdata;

         facedatas.Add(facedata);
      }
   }
}
