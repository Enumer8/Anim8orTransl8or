using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Cube
   {
      internal static mesh Convert(cube c)
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

         // Note: If two out of three 'scale's are 0, then there's nothing to
         // draw. Technically Anim8or v1.00 draws a line. If any division is 0,
         // Anim8or v1.00 still draws the cube as if it was 1.
         if ( c.scale?.x > 0 && c.scale?.y > 0 ||
              c.scale?.x > 0 && c.scale?.z > 0 ||
              c.scale?.y > 0 && c.scale?.z > 0 )
         {
            Double scaleX = Math.Max(c.scale?.x ?? 0.0, 0.0);
            Double scaleY = Math.Max(c.scale?.y ?? 0.0, 0.0);
            Double scaleZ = Math.Max(c.scale?.z ?? 0.0, 0.0);
            Int64 divisionX = Math.Max(c.divisions?.x ?? 0, 1);
            Int64 divisionY = Math.Max(c.divisions?.y ?? 0, 1);
            Int64 divisionZ = Math.Max(c.divisions?.z ?? 0, 1);

            Int32[][] verticalIndices = new Int32[4][];
            verticalIndices[0] = new Int32[divisionY + 1];
            verticalIndices[1] = new Int32[divisionY + 1];
            verticalIndices[2] = new Int32[divisionY + 1];
            verticalIndices[3] = new Int32[divisionY + 1];

            Double step = scaleY / divisionY;

            // Left front
            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point p = new point(
                  -scaleX / 2,
                  scaleY / 2 - step * i,
                  scaleZ / 2);

               verticalIndices[0][i] = points.Count;
               points.Add(p);
            }

            // Right front
            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point p = new point(
                  scaleX / 2,
                  scaleY / 2 - step * i,
                  scaleZ / 2);

               verticalIndices[1][i] = points.Count;
               points.Add(p);
            }

            // Right back
            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point p = new point(
                  scaleX / 2,
                  scaleY / 2 - step * i,
                  -scaleZ / 2);

               verticalIndices[2][i] = points.Count;
               points.Add(p);
            }

            // Left back
            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point p = new point(
                  -scaleX / 2,
                  scaleY / 2 - step * i,
                  -scaleZ / 2);

               verticalIndices[3][i] = points.Count;
               points.Add(p);
            }

            // Top
            Int32[][] topIndices = new Int32[4][];
            topIndices[0] = new Int32[divisionX + 1]; // Front
            topIndices[1] = new Int32[divisionZ + 1]; // Right
            topIndices[2] = new Int32[divisionX + 1]; // Back
            topIndices[3] = new Int32[divisionZ + 1]; // Left

            // Bottom
            Int32[][] bottomIndices = new Int32[4][];
            bottomIndices[0] = new Int32[divisionX + 1]; // Front
            bottomIndices[1] = new Int32[divisionZ + 1]; // Right
            bottomIndices[2] = new Int32[divisionX + 1]; // Back
            bottomIndices[3] = new Int32[divisionZ + 1]; // Left

            step = scaleX / divisionX;

            // Top/bottom front
            for ( Int64 i = 1; i < divisionX; i++ )
            {
               point p = new point(
                  -scaleX / 2 + step * i,
                  scaleY / 2,
                  scaleZ / 2);

               topIndices[0][i] = points.Count;
               points.Add(p);

               point p2 = new point(p.x, p.y - scaleY, p.z);

               bottomIndices[0][i] = points.Count;
               points.Add(p2);
            }

            topIndices[0][0] = verticalIndices[0][0];
            bottomIndices[0][0] = verticalIndices[0][divisionY];

            topIndices[0][divisionX] = verticalIndices[1][0];
            bottomIndices[0][divisionX] = verticalIndices[1][divisionY];

            // Top/bottom back
            for ( Int64 i = 1; i < divisionX; i++ )
            {
               point p = new point(
                  -scaleX / 2 + step * i,
                  scaleY / 2,
                  -scaleZ / 2);

               topIndices[2][divisionX - i] = points.Count;
               points.Add(p);

               point p2 = new point(p.x, p.y - scaleY, p.z);

               bottomIndices[2][divisionX - i] = points.Count;
               points.Add(p2);
            }

            topIndices[2][0] = verticalIndices[2][0];
            bottomIndices[2][0] = verticalIndices[2][divisionY];

            topIndices[2][divisionX] = verticalIndices[3][0];
            bottomIndices[2][divisionX] = verticalIndices[3][divisionY];

            step = scaleZ / divisionZ;

            // Right top/bottom
            for ( Int64 i = 1; i < divisionZ; i++ )
            {
               point p = new point(
                  scaleX / 2,
                  scaleY / 2,
                  scaleZ / 2 - step * i);

               topIndices[1][i] = points.Count;
               points.Add(p);

               point p2 = new point(p.x, p.y - scaleY, p.z);

               bottomIndices[1][i] = points.Count;
               points.Add(p2);
            }

            topIndices[1][0] = verticalIndices[1][0];
            bottomIndices[1][0] = verticalIndices[1][divisionY];

            topIndices[1][divisionZ] = verticalIndices[2][0];
            bottomIndices[1][divisionZ] = verticalIndices[2][divisionY];

            // Left top/bottom
            for ( Int64 i = 1; i < divisionZ; i++ )
            {
               point p = new point(
                  -scaleX / 2,
                  scaleY / 2,
                  scaleZ / 2 - step * i);

               topIndices[3][divisionZ - i] = points.Count;
               points.Add(p);

               point p2 = new point(p.x, p.y - scaleY, p.z);

               bottomIndices[3][divisionZ - i] = points.Count;
               points.Add(p2);
            }

            topIndices[3][0] = verticalIndices[3][0];
            bottomIndices[3][0] = verticalIndices[3][divisionY];

            topIndices[3][divisionZ] = verticalIndices[0][0];
            bottomIndices[3][divisionZ] = verticalIndices[0][divisionY];

            // Texcoord front/back
            for ( Int64 x = 0; x < divisionX + 1; x++ )
            {
               for ( Int64 y = 0; y < divisionY + 1; y++ )
               {
                  texcoord newTex = new texcoord(
                     (Double)x / divisionX,
                     1 - (Double)y / divisionY);

                  texcoords.Add(newTex);
               }
            }

            AddCubeFaces(
               divisionX,
               divisionY,
               divisionZ,
               points,
               facedatas,
               topIndices[0],
               verticalIndices[0],
               bottomIndices[0],
               verticalIndices[1],
               divisionX,
               divisionY,
               0);

            AddCubeFaces(
               divisionX,
               divisionY,
               divisionZ,
               points,
               facedatas,
               topIndices[1],
               verticalIndices[1],
               bottomIndices[1],
               verticalIndices[2],
               divisionZ,
               divisionY,
               1);

            AddCubeFaces(
               divisionX,
               divisionY,
               divisionZ,
               points,
               facedatas,
               topIndices[2],
               verticalIndices[2],
               bottomIndices[2],
               verticalIndices[3],
               divisionX,
               divisionY,
               2);

            AddCubeFaces(
               divisionX,
               divisionY,
               divisionZ,
               points,
               facedatas,
               topIndices[3],
               verticalIndices[3],
               bottomIndices[3],
               verticalIndices[0],
               divisionZ,
               divisionY,
               3);

            AddCubeFaces(
               divisionX,
               divisionY,
               divisionZ,
               points,
               facedatas,
               topIndices[2],
               topIndices[3],
               topIndices[0],
               topIndices[1],
               divisionX,
               divisionZ,
               4);

            AddCubeFaces(
               divisionX,
               divisionY,
               divisionZ,
               points,
               facedatas,
               bottomIndices[0],
               bottomIndices[3],
               bottomIndices[2],
               bottomIndices[1],
               divisionX,
               divisionZ,
               5);
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

      static void AddCubeFaces(
         Int64 divisionX,
         Int64 divisionY,
         Int64 divisionZ,
         List<point> points,
         List<facedata> facedatas,
         Int32[] top,
         Int32[] left,
         Int32[] bottom,
         Int32[] right,
         Int64 divX,
         Int64 divY,
         Int64 normalIndex)
      {
         Int32[][] faces = new Int32[divX + 1][];

         for ( Int64 i = 0; i < divX + 1; i++ )
         {
            faces[i] = new Int32[divY + 1];
         }

         // Border
         if ( normalIndex != 5 )
         {
            for ( Int64 i = 0; i < divY + 1; i++ )
            {
               faces[0][i] = left[i];
            }
         }
         else
         {
            for ( Int64 i = 0; i < divY + 1; i++ )
            {
               faces[0][i] = left[divY - i];
            }
         }

         if ( normalIndex < 4 || normalIndex == 5 )
         {
            for ( Int64 i = 0; i < divY + 1; i++ )
            {
               faces[divX][i] = right[i];
            }

            for ( Int64 i = 0; i < divX + 1; i++ )
            {
               faces[i][0] = top[i];
            }
         }
         else
         {
            for ( Int64 i = 0; i < divY + 1; i++ )
            {
               faces[divX][i] = right[divY - i];
            }

            for ( Int64 i = 0; i < divX + 1; i++ )
            {
               faces[i][0] = top[divX - i];
            }
         }

         if ( normalIndex != 5 )
         {
            for ( Int64 i = 0; i < divX + 1; i++ )
            {
               faces[i][divY] = bottom[i];
            }
         }
         else
         {
            for ( Int64 i = 0; i < divX + 1; i++ )
            {
               faces[i][divY] = bottom[divX - i];
            }
         }

         // New points
         for ( Int64 i = 1; i < divX; i++ )
         {
            point v = points[faces[i][divY]] - points[faces[i][0]];

            Double length = v.GetLength();
            v = v / length; // .Normalize();

            for ( Int64 j = 1; j < divY; j++ )
            {
               point p = points[faces[i][0]] + v * j * length / divY;

               faces[i][j] = points.Count;
               points.Add(p);
            }
         }

         // Create Face
         for ( Int64 x = 0; x < divX; x++ )
         {
            for ( Int64 y = 0; y < divY; y++ )
            {
               Int64 v = divisionX - 1 - x;

               facedata newFace = new facedata();
               newFace.numpoints = 4;
               newFace.flags = facedataenum.hastexture;
               newFace.matno = 0;
               newFace.flatnormalno = -1;
               newFace.pointdata = new pointdata[newFace.numpoints];

               // pointdata0
               pointdata p = new pointdata();
               p.pointindex = faces[x + 1][y];

               if ( normalIndex == 0 )
               {
                  p.texcoordindex = y + (x + 1) * (divisionY + 1);
               }
               else if ( normalIndex == 2 )
               {
                  p.texcoordindex = y + v * (divisionY + 1);
               }
               else if ( normalIndex == 4 || normalIndex == 5 )
               {
                  p.texcoordindex = (x + 1) * (divisionY + 1);
               }
               else if ( normalIndex == 1 || normalIndex == 3 )
               {
                  p.texcoordindex = y + divisionX * (divisionY + 1);
               }

               newFace.pointdata[0] = p;

               // pointdata1
               p = new pointdata();
               p.pointindex = faces[x + 1][y + 1];

               if ( normalIndex == 0 )
               {
                  p.texcoordindex = y + 1 + (x + 1) * (divisionY + 1);
               }
               else if ( normalIndex == 2 )
               {
                  p.texcoordindex = y + 1 + v * (divisionY + 1);
               }
               else if ( normalIndex == 4 || normalIndex == 5 )
               {
                  p.texcoordindex = (x + 1) * (divisionY + 1);
               }
               else if ( normalIndex == 1 || normalIndex == 3 )
               {
                  p.texcoordindex = y + 1 + divisionX * (divisionY + 1);
               }

               newFace.pointdata[1] = p;

               // pointdata2
               p = new pointdata();
               p.pointindex = faces[x][y + 1];

               if ( normalIndex == 0 )
               {
                  p.texcoordindex = y + 1 + x * (divisionY + 1);
               }
               else if ( normalIndex == 2 )
               {
                  p.texcoordindex = y + 1 + (v + 1) * (divisionY + 1);
               }
               else if ( normalIndex == 4 || normalIndex == 5 )
               {
                  p.texcoordindex = x * (divisionY + 1);
               }
               else if ( normalIndex == 1 || normalIndex == 3 )
               {
                  p.texcoordindex = y + 1 + divisionX * (divisionY + 1);
               }

               newFace.pointdata[2] = p;

               // pointdata3
               p = new pointdata();
               p.pointindex = faces[x][y];

               if ( normalIndex == 0 )
               {
                  p.texcoordindex = y + x * (divisionY + 1);
               }
               else if ( normalIndex == 2 )
               {
                  p.texcoordindex = y + (v + 1) * (divisionY + 1);
               }
               else if ( normalIndex == 4 || normalIndex == 5 )
               {
                  p.texcoordindex = x * (divisionY + 1);
               }
               else if ( normalIndex == 1 || normalIndex == 3 )
               {
                  p.texcoordindex = y + divisionX * (divisionY + 1);
               }

               newFace.pointdata[3] = p;

               facedatas.Add(newFace);
            }
         }
      }
   }
}
