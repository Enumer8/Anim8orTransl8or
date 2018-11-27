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
         List<point> normals = new List<point>();
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

            Int64[][] VertexIndicesVertical = new Int64[4][];
            VertexIndicesVertical[0] = new Int64[divisionY + 1];
            VertexIndicesVertical[1] = new Int64[divisionY + 1];
            VertexIndicesVertical[2] = new Int64[divisionY + 1];
            VertexIndicesVertical[3] = new Int64[divisionY + 1];

            Double step = scaleY / divisionY;

            // Front Left
            point pointRef = new point();
            pointRef.x = -scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = scaleZ / 2;

            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x;
               newPoint.y = pointRef.y - step * i;
               newPoint.z = pointRef.z;
               points.Add(newPoint);
               VertexIndicesVertical[0][i] = points.Count - 1;
            }

            // Front Right
            pointRef.x = scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = scaleZ / 2;

            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x;
               newPoint.y = pointRef.y - step * i;
               newPoint.z = pointRef.z;
               points.Add(newPoint);
               VertexIndicesVertical[1][i] = points.Count - 1;
            }

            // Back Right
            pointRef.x = scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = -scaleZ / 2;

            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x;
               newPoint.y = pointRef.y - step * i;
               newPoint.z = pointRef.z;
               points.Add(newPoint);
               VertexIndicesVertical[2][i] = points.Count - 1;
            }

            // Back Left
            pointRef.x = -scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = -scaleZ / 2;

            for ( Int64 i = 0; i < divisionY + 1; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x;
               newPoint.y = pointRef.y - step * i;
               newPoint.z = pointRef.z;
               points.Add(newPoint);
               VertexIndicesVertical[3][i] = points.Count - 1;
            }

            // Top
            Int64[][] VertexIndicesTop = new Int64[4][];
            VertexIndicesTop[0] = new Int64[divisionX + 1]; // Front
            VertexIndicesTop[1] = new Int64[divisionZ + 1]; // Right
            VertexIndicesTop[2] = new Int64[divisionX + 1]; // Back
            VertexIndicesTop[3] = new Int64[divisionZ + 1]; // Left

            // Bottom
            Int64[][] VertexIndicesBottom = new Int64[4][];
            VertexIndicesBottom[0] = new Int64[divisionX + 1]; // Front
            VertexIndicesBottom[1] = new Int64[divisionZ + 1]; // Right
            VertexIndicesBottom[2] = new Int64[divisionX + 1]; // Back
            VertexIndicesBottom[3] = new Int64[divisionZ + 1]; // Left

            step = scaleX / divisionX;

            // Top/Bottom Front
            pointRef.x = -scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = scaleZ / 2;

            for ( Int64 i = 1; i < divisionX; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x + step * i;
               newPoint.y = pointRef.y;
               newPoint.z = pointRef.z;
               points.Add(newPoint);
               VertexIndicesTop[0][i] = points.Count - 1;

               point newPoint2 = new point();
               newPoint2.x = newPoint.x;
               newPoint2.y = newPoint.y - scaleY;
               newPoint2.z = newPoint.z;
               points.Add(newPoint2);
               VertexIndicesBottom[0][i] = points.Count - 1;
            }

            VertexIndicesTop[0][0] = VertexIndicesVertical[0][0];
            VertexIndicesBottom[0][0] = VertexIndicesVertical[0][divisionY];

            VertexIndicesTop[0][divisionX] = VertexIndicesVertical[1][0];
            VertexIndicesBottom[0][divisionX] = VertexIndicesVertical[1][divisionY];

            // Top/Bottom Back
            pointRef.x = -scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = -scaleZ / 2;

            for ( Int64 i = 1; i < divisionX; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x + step * i;
               newPoint.y = pointRef.y;
               newPoint.z = pointRef.z;
               points.Add(newPoint);
               VertexIndicesTop[2][divisionX - i] = points.Count - 1;

               point newPoint2 = new point();
               newPoint2.x = newPoint.x;
               newPoint2.y = newPoint.y - scaleY;
               newPoint2.z = newPoint.z;
               points.Add(newPoint2);
               VertexIndicesBottom[2][divisionX - i] = points.Count - 1;
            }

            VertexIndicesTop[2][0] = VertexIndicesVertical[2][0];
            VertexIndicesBottom[2][0] = VertexIndicesVertical[2][divisionY];

            VertexIndicesTop[2][divisionX] = VertexIndicesVertical[3][0];
            VertexIndicesBottom[2][divisionX] = VertexIndicesVertical[3][divisionY];

            step = scaleZ / divisionZ;

            // Top/Bottom Right
            pointRef.x = scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = scaleZ / 2;

            for ( Int64 i = 1; i < divisionZ; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x;
               newPoint.y = pointRef.y;
               newPoint.z = pointRef.z - step * i;
               points.Add(newPoint);
               VertexIndicesTop[1][i] = points.Count - 1;

               point newPoint2 = new point();
               newPoint2.x = newPoint.x;
               newPoint2.y = newPoint.y - scaleY;
               newPoint2.z = newPoint.z;
               points.Add(newPoint2);
               VertexIndicesBottom[1][i] = points.Count - 1;
            }

            VertexIndicesTop[1][0] = VertexIndicesVertical[1][0];
            VertexIndicesBottom[1][0] = VertexIndicesVertical[1][divisionY];

            VertexIndicesTop[1][divisionZ] = VertexIndicesVertical[2][0];
            VertexIndicesBottom[1][divisionZ] = VertexIndicesVertical[2][divisionY];

            // Top/Bottom Left
            pointRef.x = -scaleX / 2;
            pointRef.y = scaleY / 2;
            pointRef.z = scaleZ / 2;

            for ( Int64 i = 1; i < divisionZ; i++ )
            {
               point newPoint = new point();
               newPoint.x = pointRef.x;
               newPoint.y = pointRef.y;
               newPoint.z = pointRef.z - step * i;
               points.Add(newPoint);
               VertexIndicesTop[3][divisionZ - i] = points.Count - 1;

               point newPoint2 = new point();
               newPoint2.x = newPoint.x;
               newPoint2.y = newPoint.y - scaleY;
               newPoint2.z = newPoint.z;
               points.Add(newPoint2);
               VertexIndicesBottom[3][divisionZ - i] = points.Count - 1;
            }

            VertexIndicesTop[3][0] = VertexIndicesVertical[3][0];
            VertexIndicesBottom[3][0] = VertexIndicesVertical[3][divisionY];

            VertexIndicesTop[3][divisionZ] = VertexIndicesVertical[0][0];
            VertexIndicesBottom[3][divisionZ] = VertexIndicesVertical[0][divisionY];

            // Texcoord Front/back
            for ( Int64 x = 0; x < divisionX + 1; x++ )
            {
               for ( Int64 y = 0; y < divisionY + 1; y++ )
               {
                  texcoord newTex = new texcoord();
                  newTex.u = (Double)x / divisionX;
                  newTex.v = 1 - (Double)y / divisionY;
                  texcoords.Add(newTex);
               }
            }

            void AddCubeFaces(
               //Int64 divisionX,
               //Int64 divisionY,
               //Int64 divisionZ,
               //List<point> points,
               //List<facedata> facedatas,
               Int64[] i_Top,
               Int64[] i_Left,
               Int64[] i_Bottom,
               Int64[] i_Right,
               Int64 i_DivX,
               Int64 i_DivY,
               Int64 i_NormalIndex,
               Int64 i_TextureOffset)
            {
               Int64[][] faces = new Int64[i_DivX + 1][];

               for ( Int64 i = 0; i < i_DivX + 1; i++ )
               {
                  faces[i] = new Int64[i_DivY + 1];
               }

               // Border ////////////////////////////////////////////////////////////
               if ( i_NormalIndex != 5 )
               {
                  for ( Int64 i = 0; i < i_DivY + 1; i++ )
                  {
                     faces[0][i] = i_Left[i];
                  }
               }
               else
               {
                  for ( Int64 i = 0; i < i_DivY + 1; i++ )
                  {
                     faces[0][i] = i_Left[i_DivY - i];
                  }
               }

               if ( i_NormalIndex < 4 || i_NormalIndex == 5 )
               {
                  for ( Int64 i = 0; i < i_DivY + 1; i++ )
                  {
                     faces[i_DivX][i] = i_Right[i];
                  }

                  for ( Int64 i = 0; i < i_DivX + 1; i++ )
                  {
                     faces[i][0] = i_Top[i];
                  }
               }
               else
               {
                  for ( Int64 i = 0; i < i_DivY + 1; i++ )
                  {
                     faces[i_DivX][i] = i_Right[i_DivY - i];
                  }

                  for ( Int64 i = 0; i < i_DivX + 1; i++ )
                  {
                     faces[i][0] = i_Top[i_DivX - i];
                  }
               }

               if ( i_NormalIndex != 5 )
               {
                  for ( Int64 i = 0; i < i_DivX + 1; i++ )
                  {
                     faces[i][i_DivY] = i_Bottom[i];
                  }
               }
               else
               {
                  for ( Int64 i = 0; i < i_DivX + 1; i++ )
                  {
                     faces[i][i_DivY] = i_Bottom[i_DivX - i];
                  }
               }

               // New points
               for ( Int64 i = 1; i < i_DivX; i++ )
               {
                  point vec = new point();
                  vec.x = points[(Int32)faces[i][i_DivY]].x - points[(Int32)faces[i][0]].x;
                  vec.y = points[(Int32)faces[i][i_DivY]].y - points[(Int32)faces[i][0]].y;
                  vec.z = points[(Int32)faces[i][i_DivY]].z - points[(Int32)faces[i][0]].z;
                  Double length = Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
                  vec = An8Math.Normalize(vec);

                  for ( Int64 j = 1; j < i_DivY; j++ )
                  {
                     point newPoint = new point();
                     newPoint.x = points[(Int32)faces[i][0]].x + vec.x * j * length / i_DivY;
                     newPoint.y = points[(Int32)faces[i][0]].y + vec.y * j * length / i_DivY;
                     newPoint.z = points[(Int32)faces[i][0]].z + vec.z * j * length / i_DivY;
                     points.Add(newPoint);
                     faces[i][j] = points.Count - 1;
                  }
               }

               // Create Face
               for ( Int64 x = 0; x < i_DivX; x++ )
               {
                  for ( Int64 y = 0; y < i_DivY; y++ )
                  {
                     Int64 v = divisionX - 1 - x;
                     facedata newFace = new facedata();
                     newFace.numpoints = 4;
                     newFace.flags = facedataenum.hasnormals | facedataenum.hastexture;
                     newFace.matno = 0;
                     newFace.flatnormalno = 0;
                     newFace.pointdata = new pointdata[newFace.numpoints];

                     // 0
                     pointdata index = new pointdata();
                     index.pointindex = faces[x + 1][y];
                     index.normalindex = i_NormalIndex;

                     if ( i_NormalIndex == 0 )
                     {
                        index.texcoordindex = y + (x + 1) * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 2 )
                     {
                        index.texcoordindex = y + v * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 4 || i_NormalIndex == 5 )
                     {
                        index.texcoordindex = (x + 1) * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 1 || i_NormalIndex == 3 )
                     {
                        index.texcoordindex = y + divisionX * (divisionY + 1);
                     }

                     newFace.pointdata[0] = index;

                     // 1
                     index = new pointdata();
                     index.pointindex = faces[x + 1][y + 1];
                     index.normalindex = i_NormalIndex;

                     if ( i_NormalIndex == 0 )
                     {
                        index.texcoordindex = y + 1 + (x + 1) * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 2 )
                     {
                        index.texcoordindex = y + 1 + v * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 4 || i_NormalIndex == 5 )
                     {
                        index.texcoordindex = (x + 1) * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 1 || i_NormalIndex == 3 )
                     {
                        index.texcoordindex = y + 1 + divisionX * (divisionY + 1);
                     }

                     newFace.pointdata[1] = index;

                     // 2
                     index = new pointdata();
                     index.pointindex = faces[x][y + 1];
                     index.normalindex = i_NormalIndex;

                     if ( i_NormalIndex == 0 )
                     {
                        index.texcoordindex = y + 1 + x * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 2 )
                     {
                        index.texcoordindex = y + 1 + (v + 1) * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 4 || i_NormalIndex == 5 )
                     {
                        index.texcoordindex = x * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 1 || i_NormalIndex == 3 )
                     {
                        index.texcoordindex = y + 1 + divisionX * (divisionY + 1);
                     }

                     newFace.pointdata[2] = index;

                     // 3
                     index = new pointdata();
                     index.pointindex = faces[x][y];
                     index.normalindex = i_NormalIndex;

                     if ( i_NormalIndex == 0 )
                     {
                        index.texcoordindex = y + x * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 2 )
                     {
                        index.texcoordindex = y + (v + 1) * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 4 || i_NormalIndex == 5 )
                     {
                        index.texcoordindex = x * (divisionY + 1);
                     }
                     else if ( i_NormalIndex == 1 || i_NormalIndex == 3 )
                     {
                        index.texcoordindex = y + divisionX * (divisionY + 1);
                     }

                     newFace.pointdata[3] = index;

                     facedatas.Add(newFace);
                  }
               }
            }

            AddCubeFaces(
               //divisionX, divisionY, divisionZ, points, facedatas,
               VertexIndicesTop[0], VertexIndicesVertical[0], VertexIndicesBottom[0], VertexIndicesVertical[1],
               divisionX, divisionY,
               0,
               0 // Texture Offset
            );

            AddCubeFaces(
               //divisionX, divisionY, divisionZ, points, facedatas,
               VertexIndicesTop[1], VertexIndicesVertical[1], VertexIndicesBottom[1], VertexIndicesVertical[2],
               divisionZ, divisionY,
               1,
               divisionX * divisionY // Texture Offset
            );

            AddCubeFaces(
               //divisionX, divisionY, divisionZ, points, facedatas,
               VertexIndicesTop[2], VertexIndicesVertical[2], VertexIndicesBottom[2], VertexIndicesVertical[3],
               divisionX, divisionY,
               2,
               0 // Texture Offset
            );

            AddCubeFaces(
               //divisionX, divisionY, divisionZ, points, facedatas,
               VertexIndicesTop[3], VertexIndicesVertical[3], VertexIndicesBottom[3], VertexIndicesVertical[0],
               divisionZ, divisionY,
               3,
               divisionX * divisionY // Texture Offset
            );

            AddCubeFaces(
               //divisionX, divisionY, divisionZ, points, facedatas,
               VertexIndicesTop[2], VertexIndicesTop[3], VertexIndicesTop[0], VertexIndicesTop[1],
               divisionX, divisionZ,
               4,
               divisionX * divisionY + divisionZ * divisionY // Texture Offset
           );

            AddCubeFaces(
               //divisionX, divisionY, divisionZ, points, facedatas,
               VertexIndicesBottom[0], VertexIndicesBottom[3], VertexIndicesBottom[2], VertexIndicesBottom[1],
               divisionX, divisionZ,
               5,
               divisionX * divisionY + divisionZ * divisionY // Texture Offset
            );

            point newNormal = new point();
            newNormal.z = 1;
            normals.Add(newNormal);

            newNormal = new point();
            newNormal.x = 1;
            normals.Add(newNormal);

            newNormal = new point();
            newNormal.z = -1;
            normals.Add(newNormal);

            newNormal = new point();
            newNormal.x = -1;
            normals.Add(newNormal);

            newNormal = new point();
            newNormal.y = 1;
            normals.Add(newNormal);

            newNormal = new point();
            newNormal.y = -1;
            normals.Add(newNormal);
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
