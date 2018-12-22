using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   internal static class An8Cylinder
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="c">the cylinder</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the converted tmesh</returns>
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
         Double length = (c?.length?.text ?? 1).LimitTo(0);
         Double startDiameter = (c?.diameter?.text ?? 1).LimitTo(0);
         Double endDiameter = (c?.topdiameter?.text ?? 1).LimitTo(0);
         Int32 longitude = (Int32)(c?.longlat?.longitude ?? 12).LimitTo(3, 100);
         Int32 latitude = (Int32)(c?.longlat?.latitude ?? 8).LimitTo(1, 100);
         Boolean bottomFace = c?.capstart != null && startDiameter > 0;
         Boolean topFace = c?.capend != null && endDiameter > 0;

         if ( callback != null )
         {
            if ( c?.length != null && c.length.text != length )
            {
               callback($"The \"{c.name?.text}\" cylinders's length of {c.length.text} has been limited to {length}.");
            }

            if ( c?.diameter != null && c.diameter.text != startDiameter )
            {
               callback($"The \"{c.name?.text}\" cylinders's diameter of {c.diameter.text} has been limited to {startDiameter}.");
            }

            if ( c?.topdiameter != null && c.topdiameter.text != endDiameter )
            {
               callback($"The \"{c.name?.text}\" cylinders's top diameter of {c.topdiameter.text} has been limited to {endDiameter}.");
            }

            if ( c?.longlat != null )
            {
               if ( c.longlat.longitude != longitude )
               {
                  callback($"The \"{c.name?.text}\" cylinders's longitude of {c.longlat.longitude} has been limited to {longitude}.");
               }

               if ( c.longlat.latitude != latitude )
               {
                  callback($"The \"{c.name?.text}\" cylinders's latitude of {c.longlat.latitude} has been limited to {latitude}.");
               }
            }
         }

         Double stepY = length / latitude;
         Double startRadius = startDiameter / 2;
         Double stepRadius = (endDiameter - startDiameter) / (2 * latitude);
         facedataenum flags = facedataenum.hastexture;

         if ( c?.capstart == null || c?.capend == null )
         {
            // Show the back if one or more end caps are missing.
            // Note: Presumably, you only need to show the back face when one
            // of the cylinder ends is "open". Anim8or v1.00 is smart enough to
            // detect that it does not need to draw a zero-sized face when an
            // end diameter is zero, but for some reason, it still wants to
            // draw the back face.
            flags |= facedataenum.showback;
         }

         //
         //   5*---3*
         //   /      \
         // 7*       1*
         //  |\      /|
         //  |9*--11* |
         //  | |    | |          Y
         //  |4|---2| |          |
         //  |/|    |\|          |
         // 6* |    |0*          *------X
         //   \|    |/          /
         //   8*--10*          Z
         //
         for ( Int32 ix = 0; ix <= longitude; ix++ )
         {
            Double x = Math.Cos(ix * 2 * Math.PI / longitude);
            Double z = -Math.Sin(ix * 2 * Math.PI / longitude);
            Double u = (Double)ix / longitude;

            for ( Int32 iy = 0; iy <= latitude; iy++ )
            {
               Double y = stepY * iy;
               Double radius = startRadius + stepRadius * iy;
               Double v = (Double)iy / latitude;

               // Don't create the last column of points, since they are the
               // same as the first column of points.
               if ( ix < longitude )
               {
                  points.Add(new point(x * radius, y, z * radius));
               }

               texcoords.Add(new texcoord(u, v));
            }
         }

         // Create the vertical faces
         for ( Int32 ix = 0; ix < longitude; ix++ )
         {
            // This builds the faces from bottom to top.
            //
            //  *------*
            //  |      |
            //  |   1  |
            //  *------*
            //  |      |
            //  |   0  |
            //  *------*
            for ( Int32 iy = 0; iy < latitude; iy++ )
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

               pointdata[] pointdata;

               if ( ix >= longitude - 1 )
               {
                  // When building the last column of faces, make sure the
                  // right side connects to the first column of points. The tex
                  // coords do not need to be adjusted, since they have an
                  // extra column.
                  rightBottom.pointindex = iy;
                  rightTop.pointindex = iy + 1;
               }

               if ( endDiameter <= startDiameter )
               {
                  // Build the vertices clockwise starting at the right bottom
                  //
                  // 2*-----3*
                  //  |      |
                  //  |      |
                  // 1*-----0*
                  pointdata = new pointdata[]
                  {
                     rightBottom,
                     leftBottom,
                     leftTop,
                     rightTop,
                  };
               }
               else
               {
                  // Builds the vertices clockwise starting at the left bottom
                  //
                  // 1*-----2*
                  //  |      |
                  //  |      |
                  // 0*-----3*
                  pointdata = new pointdata[]
                  {
                     leftBottom,
                     leftTop,
                     rightTop,
                     rightBottom,
                  };
               }

               facedata f = new facedata();
               f.numpoints = pointdata.Length;
               f.flags = flags;
               f.matno = 0;
               f.flatnormalno = -1;
               f.pointdata = pointdata;

               facedatas.Add(f);
            }
         }

         // Create the top faces
         if ( topFace )
         {
            // Disc top
            facedata f = new facedata();
            f.numpoints = longitude;
            f.flags = flags;
            f.matno = 0;
            f.flatnormalno = -1;
            f.pointdata = new pointdata[f.numpoints];

            for ( Int64 i = 0; i < longitude; i++ )
            {
               pointdata index = new pointdata();
               index.pointindex = (longitude - 1 - i) * (latitude + 1) + latitude;//indices[longitude - 1 - i, latitude];
               index.texcoordindex = index.pointindex;//texcoords.Count - longitude - i * (latitude + 1);
               f.pointdata[i] = index;
            }

            facedatas.Add(f);
         }

         // Create the bottom faces
         if ( bottomFace )
         {
            // Disc base
            facedata f = new facedata();
            f.numpoints = longitude;
            f.flags = flags;
            f.matno = 0;
            f.flatnormalno = -1;
            f.pointdata = new pointdata[f.numpoints];

            for ( Int64 i = 0; i < longitude; i++ )
            {
               pointdata index = new pointdata();
               index.pointindex = i * (latitude + 1);//indices[i, 0];
               index.texcoordindex = index.pointindex;//i * (latitude + 1);
               f.pointdata[i] = index;
            }

            facedatas.Add(f);
         }

         m.points = new points();
         m.points.point = points.ToArray();
         m.texcoords = new texcoords();
         m.texcoords.texcoord = texcoords.ToArray();
         m.faces = new faces();
         m.faces.facedata = facedatas.ToArray();

         return m;
      }
   }
}
