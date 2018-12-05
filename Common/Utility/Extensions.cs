using System;

namespace Anim8orTransl8or.Utility
{
   static class ArrayExtensions
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
   }
}
