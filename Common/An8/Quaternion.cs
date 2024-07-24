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

using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.An8
{
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

      public quaternion(point originalPoint, point rotatedPoint)
      {
         Double cosTheta = originalPoint.Normalize().Dot(
            rotatedPoint.Normalize());

         Double angle = Math.Acos(cosTheta);
         point axis = originalPoint.Cross(rotatedPoint).Normalize();
         this = new quaternion(axis, angle);
      }

      public override String ToString()
      {
         return $"<{x:0.000}, {y:0.000}, {z:0.000}, {w:0.000}>";
      }

      public IEnumerable<Double> GetEnumerator()
      {
         yield return x;
         yield return y;
         yield return z;
         yield return w;
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

      public quaternion Conjugate()
      {
         return new quaternion(-x, -y, -z, w);
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
}
