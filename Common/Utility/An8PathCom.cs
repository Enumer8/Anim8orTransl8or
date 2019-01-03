using Anim8orTransl8or.An8.V100;

namespace Anim8orTransl8or.Utility
{
   static class An8PathCom
   {
      internal static mesh Calculate(pathcom p)
      {
         // TODO: Support pathcom?
         mesh m = new mesh();
         m.name = p.name;
         m.@base = p.@base;
         m.pivot = p.pivot;

         return m;
      }
   }
}
