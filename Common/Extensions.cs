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
