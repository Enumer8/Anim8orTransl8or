// Copyright © 2023 Contingent Games.
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Anim8orTransl8or.An8
{
   public struct matrix
   {
      public const Int32 ROWS = 4;
      public const Int32 COLS = 4;

      public static readonly matrix IDENTITY = new matrix(
         1, 0, 0, 0,
         0, 1, 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1);

      public Double m00;
      public Double m01;
      public Double m02;
      public Double m03;
      public Double m10;
      public Double m11;
      public Double m12;
      public Double m13;
      public Double m20;
      public Double m21;
      public Double m22;
      public Double m23;
      public Double m30;
      public Double m31;
      public Double m32;
      public Double m33;

      public matrix(
         Double entry00 = 0,
         Double entry01 = 0,
         Double entry02 = 0,
         Double entry03 = 0,
         Double entry10 = 0,
         Double entry11 = 0,
         Double entry12 = 0,
         Double entry13 = 0,
         Double entry20 = 0,
         Double entry21 = 0,
         Double entry22 = 0,
         Double entry23 = 0,
         Double entry30 = 0,
         Double entry31 = 0,
         Double entry32 = 0,
         Double entry33 = 0)
      {
         m00 = entry00;
         m01 = entry01;
         m02 = entry02;
         m03 = entry03;
         m10 = entry10;
         m11 = entry11;
         m12 = entry12;
         m13 = entry13;
         m20 = entry20;
         m21 = entry21;
         m22 = entry22;
         m23 = entry23;
         m30 = entry30;
         m31 = entry31;
         m32 = entry32;
         m33 = entry33;
      }

      // Scale is applied first, then rotation q, then translation p
      public matrix(
         point p,
         quaternion q,
         Double scaleX,
         Double scaleY,
         Double scaleZ)
         : this(
            (q.x * q.x - q.y * q.y - q.z * q.z + q.w * q.w) * scaleX,
            (q.x * q.y + q.x * q.y - q.z * q.w - q.z * q.w) * scaleX,
            (q.x * q.z + q.x * q.z + q.y * q.w + q.y * q.w) * scaleX,
            p.x,
            (q.x * q.y + q.x * q.y + q.z * q.w + q.z * q.w) * scaleY,
            (q.y * q.y - q.x * q.x - q.z * q.z + q.w * q.w) * scaleY,
            (q.y * q.z + q.y * q.z - q.x * q.w - q.x * q.w) * scaleY,
            p.y,
            (q.x * q.z + q.x * q.z - q.y * q.w - q.y * q.w) * scaleZ,
            (q.x * q.w + q.x * q.w + q.y * q.z + q.y * q.z) * scaleZ,
            (q.z * q.z - q.x * q.x - q.y * q.y + q.w * q.w) * scaleZ,
            p.z,
            0,
            0,
            0,
            1)
      {
      }

      // Scale is applied first, then rotation q, then translation p
      public matrix(
         point p,
         quaternion q,
         Double scale = 1)
         : this(p, q, scale, scale, scale)
      {
      }

      public override String ToString()
      {
         StringBuilder sb0 = new StringBuilder("--", 192);
         StringBuilder sb1 = new StringBuilder("| ", 32);
         StringBuilder sb2 = new StringBuilder("| ", 32);
         StringBuilder sb3 = new StringBuilder("| ", 32);
         StringBuilder sb4 = new StringBuilder("| ", 32);
         StringBuilder sb5 = new StringBuilder("--", 32);

         for ( Int32 col = 0; col < COLS; col++ )
         {
            String row0 = this[0, col].ToString("0.000");
            String row1 = this[1, col].ToString("0.000");
            String row2 = this[2, col].ToString("0.000");
            String row3 = this[3, col].ToString("0.000");

            Int32 maxLength = Math.Max(Math.Max(Math.Max(
               row0.Length, row1.Length), row2.Length), row3.Length);

            sb0.Append("".PadLeft(maxLength));
            sb1.Append(row0.PadLeft(maxLength));
            sb2.Append(row1.PadLeft(maxLength));
            sb3.Append(row2.PadLeft(maxLength));
            sb4.Append(row3.PadLeft(maxLength));
            sb5.Append("".PadLeft(maxLength));

            if ( col < COLS - 1 )
            {
               sb0.Append(' ');
               sb1.Append(' ');
               sb2.Append(' ');
               sb3.Append(' ');
               sb4.Append(' ');
               sb5.Append(' ');
            }
         }

         sb0.Append("--");
         sb1.Append(" |");
         sb2.Append(" |");
         sb3.Append(" |");
         sb4.Append(" |");
         sb5.Append("--");

         sb0.AppendLine();
         sb0.AppendLine(sb1.ToString());
         sb0.AppendLine(sb2.ToString());
         sb0.AppendLine(sb3.ToString());
         sb0.AppendLine(sb4.ToString());
         sb0.Append(sb5.ToString());
         return sb0.ToString();
      }

      public IEnumerable<Double> GetEnumerator()
      {
         yield return m00;
         yield return m01;
         yield return m02;
         yield return m03;
         yield return m10;
         yield return m11;
         yield return m12;
         yield return m13;
         yield return m20;
         yield return m21;
         yield return m22;
         yield return m23;
         yield return m30;
         yield return m31;
         yield return m32;
         yield return m33;
      }

      public matrix Transform(matrix m)
      {
         matrix matrix = new matrix();

         for ( Int32 row = 0; row < ROWS; row++ )
         {
            for ( Int32 col = 0; col < COLS; col++ )
            {
               Double e = 0;

               for ( Int32 i = 0; i < COLS; i++ )
               {
                  e += this[row, i] * m[i, col];
               }

               matrix[row, col] = e;
            }
         }

         return matrix;
      }

      public point Transform(point p)
      {
         const Int32 COL2 = 1;

         point point = new point();

         for ( Int32 row = 0; row < ROWS; row++ )
         {
            for ( Int32 col = 0; col < COL2; col++ )
            {
               Double e = 0;

               for ( Int32 i = 0; i < COLS; i++ )
               {
                  e += this[row, i] * (i < COLS - 1 ? p[i/*, col*/] : 1);
               }

               if ( row < COLS - 1 )
               {
                  point[row/*, col*/] = e;
               }
            }
         }

         return point;
      }

      public matrix Inverse()
      {
         matrix matrix = new matrix();

         for ( Int32 row = 0; row < ROWS; row++ )
         {
            for ( Int32 col = 0; col < COLS; col++ )
            {
               matrix[row, col] = PartialInverse(row, col);
            }
         }

         Double determinant = 0;

         for ( Int32 i = 0; i < 4; i++ )
         {
            determinant += this[i, 0] * matrix[0, i];
         }

         if ( determinant != 0 )
         {
            determinant = 1 / determinant;

            for ( Int32 row = 0; row < ROWS; row++ )
            {
               for ( Int32 col = 0; col < COLS; col++ )
               {
                  matrix[row, col] *= determinant;
               }
            }

            return matrix;
         }
         else
         {
            return IDENTITY;
         }
      }

      public Double this[Int32 row, Int32 col]
      {
         get
         {
            switch ( row )
            {
            case 0:
               switch ( col )
               {
               case 0:
                  return m00;
               case 1:
                  return m01;
               case 2:
                  return m02;
               case 3:
                  return m03;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            case 1:
               switch ( col )
               {
               case 0:
                  return m10;
               case 1:
                  return m11;
               case 2:
                  return m12;
               case 3:
                  return m13;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            case 2:
               switch ( col )
               {
               case 0:
                  return m20;
               case 1:
                  return m21;
               case 2:
                  return m22;
               case 3:
                  return m23;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            case 3:
               switch ( col )
               {
               case 0:
                  return m30;
               case 1:
                  return m31;
               case 2:
                  return m32;
               case 3:
                  return m33;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            default:
               throw new IndexOutOfRangeException(ROW_OUT_OF_BOUNDS);
            }
         }
         set
         {
            switch ( row )
            {
            case 0:
               switch ( col )
               {
               case 0:
                  m00 = value;
                  return;
               case 1:
                  m01 = value;
                  return;
               case 2:
                  m02 = value;
                  return;
               case 3:
                  m03 = value;
                  return;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            case 1:
               switch ( col )
               {
               case 0:
                  m10 = value;
                  return;
               case 1:
                  m11 = value;
                  return;
               case 2:
                  m12 = value;
                  return;
               case 3:
                  m13 = value;
                  return;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            case 2:
               switch ( col )
               {
               case 0:
                  m20 = value;
                  return;
               case 1:
                  m21 = value;
                  return;
               case 2:
                  m22 = value;
                  return;
               case 3:
                  m23 = value;
                  return;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            case 3:
               switch ( col )
               {
               case 0:
                  m30 = value;
                  return;
               case 1:
                  m31 = value;
                  return;
               case 2:
                  m32 = value;
                  return;
               case 3:
                  m33 = value;
                  return;
               default:
                  throw new IndexOutOfRangeException(COL_OUT_OF_BOUNDS);
               }
            default:
               throw new IndexOutOfRangeException(ROW_OUT_OF_BOUNDS);
            }
         }
      }

      Double PartialInverse(Int32 row, Int32 col)
      {
         Int32 temp = 2 + (col - row);

         row += 4 + temp;
         col += 4 - temp;

         Double result =
           this[(row + 1) % 4, (col - 1) % 4] *
           this[(row + 0) % 4, (col + 0) % 4] *
           this[(row - 1) % 4, (col + 1) % 4] +
           this[(row + 1) % 4, (col + 1) % 4] *
           this[(row + 0) % 4, (col - 1) % 4] *
           this[(row - 1) % 4, (col + 0) % 4] +
           this[(row - 1) % 4, (col - 1) % 4] *
           this[(row + 1) % 4, (col + 0) % 4] *
           this[(row + 0) % 4, (col + 1) % 4] -
           this[(row - 1) % 4, (col - 1) % 4] *
           this[(row + 0) % 4, (col + 0) % 4] *
           this[(row + 1) % 4, (col + 1) % 4] -
           this[(row - 1) % 4, (col + 1) % 4] *
           this[(row + 0) % 4, (col - 1) % 4] *
           this[(row + 1) % 4, (col + 0) % 4] -
           this[(row + 1) % 4, (col - 1) % 4] *
           this[(row - 1) % 4, (col + 0) % 4] *
           this[(row + 0) % 4, (col + 1) % 4];

         return temp % 2 != 0 ? result : -result;
      }

      const String ROW_OUT_OF_BOUNDS =
         "Row was outside the bounds of the matrix.";

      const String COL_OUT_OF_BOUNDS =
         "Column was outside the bounds of the matrix.";
   }
}
