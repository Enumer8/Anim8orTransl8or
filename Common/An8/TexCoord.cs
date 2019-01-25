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

using System;
using System.Collections.Generic;

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

      public IEnumerable<Double> GetEnumerator()
      {
         yield return u;
         yield return v;
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
}
