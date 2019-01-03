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
         Double length = (c?.length?.text ?? 1).Limit(0);
         Double startDiameter = (c?.diameter?.text ?? 1).Limit(0);
         Double endDiameter = (c?.topdiameter?.text ?? 1).Limit(0);
         Int32 longitude = (Int32)(c?.longlat?.longitude ?? 12).Limit(3, 100);
         Int32 latitude = (Int32)(c?.longlat?.latitude ?? 8).Limit(1, 100);
         Boolean bottomFace = c?.capstart != null && startDiameter > 0;
         Boolean topFace = c?.capend != null && endDiameter > 0;

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

         //
         //   5*----3*
         //   /       \
         // 7*        1*
         //  |\       /|
         //  |9*---11* |
         //  | |     | |          Y
         //  |4|----2| |          |
         //  |/|     |\|          |
         // 6* |     |0*          *------X
         //   \|     |/          /
         //   8*---10*          Z
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

         // Create the vertical faces
         for ( Int32 ix = 0; ix < longitude; ix++ )
         {
            // This builds the faces from bottom to top.
            //
            //  *-----*
            //  |     |
            //  |  1  |
            //  *-----*
            //  |     |
            //  |  0  |
            //  *-----*
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

               if ( ix >= longitude - 1 )
               {
                  // When building the last vertical slice of faces, make sure
                  // the right side connects to the first vertical slice of
                  // points. The tex coords do not need to be adjusted, since
                  // they have an extra vertical slice.
                  rightBottom.pointindex = iy;
                  rightTop.pointindex = iy + 1;
               }

               pointdata[] pointdata;

               if ( endDiameter <= startDiameter )
               {
                  // Build the vertices clockwise starting at the right bottom
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
               else
               {
                  // Builds the vertices clockwise starting at the left bottom
                  //
                  // 1*----2*
                  //  |     |
                  //  |     |
                  // 0*----3*
                  pointdata = new pointdata[]
                  {
                     leftBottom,
                     leftTop,
                     rightTop,
                     rightBottom,
                  };
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

         // Create the top faces
         if ( topFace )
         {
            pointdata[] pointdata = new pointdata[longitude];

            for ( Int32 ix = 0; ix < longitude; ix++ )
            {
               pointdata data = new pointdata();
               data.pointindex =
                  (longitude - 1 - ix) * (latitude + 1) + latitude;
               data.normalindex = 0;
               data.texcoordindex = data.pointindex;

               pointdata[ix] = data;
            }

            facedata facedata = new facedata();
            facedata.numpoints = pointdata.Length;
            facedata.flags = flags;
            facedata.matno = 0;
            facedata.flatnormalno = -1;
            facedata.pointdata = pointdata;

            facedatas.Add(facedata);
         }

         // Create the bottom faces
         if ( bottomFace )
         {
            pointdata[] pointdata = new pointdata[longitude];

            for ( Int32 ix = 0; ix < longitude; ix++ )
            {
               pointdata data = new pointdata();
               data.pointindex = ix * (latitude + 1);
               data.normalindex = 0;
               data.texcoordindex = data.pointindex;

               pointdata[ix] = data;
            }

            facedata facedata = new facedata();
            facedata.numpoints = pointdata.Length;
            facedata.flags = flags;
            facedata.matno = 0;
            facedata.flatnormalno = -1;
            facedata.pointdata = pointdata;

            facedatas.Add(facedata);
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
