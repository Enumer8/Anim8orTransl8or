using System;
using System.Collections.Generic;
using System.Text;

namespace Anim8orTransl8or.An8
{
   public struct texcoord
   {
      public const Int32 LENGTH = 2;

      public Double u;
      public Double v;

      public texcoord(Double u = 0, Double v = 0)
      {
         this.u = u;
         this.v = v;
      }

      public override String ToString()
      {
         return $"<{u:0.000}, {v:0.000}>";
      }

      public Double GetLength()
      {
         return Math.Sqrt(u * u + v * v);
      }

      public texcoord Normalize()
      {
         Double length = GetLength();

         return new texcoord()
         {
            u = u / length,
            v = v / length,
         };
      }

      public Double this[Int32 index]
      {
         get
         {
            switch ( index )
            {
            case 0:
               return u;
            case 1:
               return v;
            default:
               throw new IndexOutOfRangeException(INDEX_OUT_OF_BOUNDS);
            }
         }
         set
         {
            switch ( index )
            {
            case 0:
               u = value;
               return;
            case 1:
               v = value;
               return;
            default:
               throw new IndexOutOfRangeException(INDEX_OUT_OF_BOUNDS);
            }
         }
      }

      public static texcoord operator +(texcoord a, texcoord b)
      {
         return new texcoord
         {
            u = a.u + b.u,
            v = a.v + b.v,
         };
      }

      public static texcoord operator -(texcoord a, texcoord b)
      {
         return new texcoord
         {
            u = a.u - b.u,
            v = a.v - b.v,
         };
      }

      public static texcoord operator *(texcoord a, Double b)
      {
         return new texcoord
         {
            u = a.u * b,
            v = a.v * b,
         };
      }

      public static texcoord operator /(texcoord a, Double b)
      {
         return new texcoord
         {
            u = a.u / b,
            v = a.v / b,
         };
      }

      const String INDEX_OUT_OF_BOUNDS =
         "Index was outside the bounds of the texcoord.";
   }

   public struct point
   {
      public const Int32 LENGTH = 3;

      public Double x;
      public Double y;
      public Double z;

      public point(Double x = 0, Double y = 0, Double z = 0)
      {
         this.x = x;
         this.y = y;
         this.z = z;
      }

      public override String ToString()
      {
         return $"<{x:0.000}, {y:0.000}, {z:0.000}>";
      }

      public Double GetLength()
      {
         return Math.Sqrt(x * x + y * y + z * z);
      }

      public point Normalize()
      {
         Double length = GetLength();

         return new point()
         {
            x = x / length,
            y = y / length,
            z = z / length,
         };
      }

      public Double Dot(point b)
      {
         return x * b.x + y * b.y + z * b.z;
      }

      public point Cross(point b)
      {
         return new point()
         {
            x = y * b.z - z * b.y,
            y = z * b.x - x * b.z,
            z = x * b.y - y * b.x,
         };
      }

      public Double this[Int32 index]
      {
         get
         {
            switch ( index )
            {
            case 0:
               return x;
            case 1:
               return y;
            case 2:
               return z;
            default:
               throw new IndexOutOfRangeException(INDEX_OUT_OF_BOUNDS);
            }
         }
         set
         {
            switch ( index )
            {
            case 0:
               x = value;
               return;
            case 1:
               y = value;
               return;
            case 2:
               z = value;
               return;
            default:
               throw new IndexOutOfRangeException(INDEX_OUT_OF_BOUNDS);
            }
         }
      }

      public static point operator +(point a, point b)
      {
         return new point
         {
            x = a.x + b.x,
            y = a.y + b.y,
            z = a.z + b.z,
         };
      }

      public static point operator -(point a, point b)
      {
         return new point
         {
            x = a.x - b.x,
            y = a.y - b.y,
            z = a.z - b.z,
         };
      }

      public static point operator *(point a, Double b)
      {
         return new point
         {
            x = a.x * b,
            y = a.y * b,
            z = a.z * b,
         };
      }

      public static point operator /(point a, Double b)
      {
         return new point
         {
            x = a.x / b,
            y = a.y / b,
            z = a.z / b,
         };
      }

      const String INDEX_OUT_OF_BOUNDS =
         "Index was outside the bounds of the point.";
   }

   public struct quaternion
   {
      public const Int32 LENGTH = 4;

      public static readonly quaternion IDENTITY = new quaternion(0, 0, 0, 1);

      public Double x;
      public Double y;
      public Double z;
      public Double w;

      public quaternion(Double x = 0, Double y = 0, Double z = 0, Double w = 0)
      {
         this.x = x;
         this.y = y;
         this.z = z;
         this.w = w;
      }

      public quaternion(point axis, Double angle)
      {
         Double sine = Math.Sin(angle / 2);
         Double cosine = Math.Cos(angle / 2);

         x = axis.x * sine;
         y = axis.y * sine;
         z = axis.z * sine;
         w = cosine;
      }

      public override String ToString()
      {
         return $"<{x:0.000}, {y:0.000}, {z:0.000}, {w:0.000}>";
      }

      public quaternion Conjugate()
      {
         return new quaternion(-x, -y, -z, w);
      }

      public Double GetLength()
      {
         return Math.Sqrt(x * x + y * y + z * z + w * w);
      }

      public quaternion Normalize()
      {
         Double length = GetLength();

         return new quaternion()
         {
            x = x / length,
            y = y / length,
            z = z / length,
            w = w / length,
         };
      }

      public quaternion Rotate(quaternion q)
      {
         return new quaternion()
         {
            x = w * q.x + x * q.w + y * q.z - z * q.y,
            y = w * q.y - x * q.z + y * q.w + z * q.x,
            z = w * q.z + x * q.y - y * q.x + z * q.w,
            w = w * q.w - x * q.x - y * q.y - z * q.z,
         };
      }

      public point Rotate(point p)
      {
         return new point()
         {
            x = (w * w + x * x - y * y - z * z) * p.x + ((x * y - z * w) *
               p.y + (x * z + y * w) * p.z) * 2,
            y = (w * w - x * x + y * y - z * z) * p.y + ((x * y + z * w) *
               p.x + (y * z - x * w) * p.z) * 2,
            z = (w * w - x * x - y * y + z * z) * p.z + ((x * z - y * w) *
               p.x + (y * z + x * w) * p.y) * 2,
         };
      }

      public Double this[Int32 index]
      {
         get
         {
            switch ( index )
            {
            case 0:
               return x;
            case 1:
               return y;
            case 2:
               return z;
            case 3:
               return w;
            default:
               throw new IndexOutOfRangeException(INDEX_OUT_OF_BOUNDS);
            }
         }
         set
         {
            switch ( index )
            {
            case 0:
               x = value;
               return;
            case 1:
               y = value;
               return;
            case 2:
               z = value;
               return;
            case 3:
               w = value;
               return;
            default:
               throw new IndexOutOfRangeException(INDEX_OUT_OF_BOUNDS);
            }
         }
      }

      public static quaternion operator +(quaternion a, quaternion b)
      {
         return new quaternion
         {
            x = a.x + b.x,
            y = a.y + b.y,
            z = a.z + b.z,
            w = a.w + b.w,
         };
      }

      public static quaternion operator -(quaternion a, quaternion b)
      {
         return new quaternion
         {
            x = a.x - b.x,
            y = a.y - b.y,
            z = a.z - b.z,
            w = a.w - b.w,
         };
      }

      public static quaternion operator *(quaternion a, Double b)
      {
         return new quaternion
         {
            x = a.x * b,
            y = a.y * b,
            z = a.z * b,
            w = a.w * b,
         };
      }

      public static quaternion operator /(quaternion a, Double b)
      {
         return new quaternion
         {
            x = a.x / b,
            y = a.y / b,
            z = a.z / b,
            w = a.w / b,
         };
      }

      const String INDEX_OUT_OF_BOUNDS =
         "Index was outside the bounds of the quaternion.";
   }

   public struct matrix
   {
      public const Int32 ROWS = 4;
      public const Int32 COLS = 4;

      public static readonly matrix IDENTITY = new matrix(
         1, 0, 0, 0,
         0, 1, 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1);

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

      // Scale is applied first, then rotation, then translation
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
            (q.z * q.z - q.x * q.x - q.y * q.y + q.w * q.w
) * scaleZ,
            p.z,
            0,
            0,
            0,
            1)
      {
      }

      public matrix(
         point p,
         quaternion q,
         Double scale = 1)
         : this(p, q, scale, scale, scale)
      {
      }

      public String ToString()
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

      public matrix Multiply(matrix m)
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

      // 4row x 4col multiply 4row x 1col
      // ROWS = 4, COLS = 4, COLS = 1
      public point Multiply(point p)
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

      Double m00;
      Double m01;
      Double m02;
      Double m03;
      Double m10;
      Double m11;
      Double m12;
      Double m13;
      Double m20;
      Double m21;
      Double m22;
      Double m23;
      Double m30;
      Double m31;
      Double m32;
      Double m33;
   }
}
