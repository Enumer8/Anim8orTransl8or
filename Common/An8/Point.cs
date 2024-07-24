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

      public IEnumerable<Double> GetEnumerator()
      {
         yield return x;
         yield return y;
         yield return z;
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
}
