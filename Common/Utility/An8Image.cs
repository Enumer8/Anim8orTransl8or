using Anim8orTransl8or.An8.V100;

namespace Anim8orTransl8or.Utility
{
   static class An8Image
   {
      internal static mesh Calculate(image i)
      {
         // TODO: Support image?
         mesh m = new mesh();
         m.name = i.name;
         m.@base = i.@base;
         m.pivot = i.pivot;

         return m;
      }
   }
}
