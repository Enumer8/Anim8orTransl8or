using Anim8orTransl8or.An8.V100;
using System;

namespace Anim8orTransl8or.Utility
{
   static class An8PathCom
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="p">the pathcom</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the converted mesh</returns>
      internal static mesh Calculate(pathcom p, Action<String> callback = null)
      {
         mesh m = new mesh();
         m.name = p?.name;
         m.@base = p?.@base;
         m.pivot = p?.pivot;

         // TODO: Support pathcom?
         callback?.Invoke($"The \"{p?.name?.text}\" pathcom is not supported. Please convert it to a mesh in Anim8or.");

         return m;
      }
   }
}
