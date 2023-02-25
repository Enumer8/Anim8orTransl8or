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

namespace Anim8orTransl8or
{
   static class Extensions
   {
      internal static L[] Append<L, T>(this L[] list, T item) where L : class
      {
         if ( item != null )
         {
            if ( list != null )
            {
               // Add to the array
               L[] newList = new L[list.Length + 1];
               Array.Copy(list, newList, list.Length);
               newList[list.Length] = item as L;
               return newList;
            }
            else
            {
               // Create a new array
               return new L[] { item as L };
            }
         }
         else
         {
            return list;
         }
      }

      internal static Double Limit(
         this Double d,
         Double minimum,
         Double maximum = Double.MaxValue)
      {
         return Math.Min(Math.Max(!Double.IsNaN(d) ? d : 0, minimum), maximum);
      }

      internal static Int64 Limit(
         this Int64 i,
         Int64 minimum,
         Int64 maximum = Int64.MaxValue)
      {
         return Math.Min(Math.Max(i, minimum), maximum);
      }
   }
}
