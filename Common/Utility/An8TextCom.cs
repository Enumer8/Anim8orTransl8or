using Anim8orTransl8or.An8.V100;

namespace Anim8orTransl8or.Utility
{
   static class An8TextCom
   {
      internal static mesh Calculate(textcom t)
      {
         // TODO: Support textcom?
         mesh m = new mesh();
         m.name = t.name;
         m.@base = t.@base;
         m.pivot = t.pivot;

         return m;
      }
   }
}
