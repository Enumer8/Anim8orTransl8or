using Anim8orTransl8or.An8.V100;
using System;

namespace Anim8orTransl8or.Utility
{
   static class An8TextCom
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="t">the textcom</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the converted mesh</returns>
      internal static mesh Calculate(textcom t, Action<String> callback = null)
      {
         mesh m = new mesh();
         m.name = t?.name;
         m.@base = t?.@base;
         m.pivot = t?.pivot;

         // TODO: Support textcom?
         callback?.Invoke($"The \"{t?.name?.text}\" textcom is not supported. Please convert it to a mesh in Anim8or.");

         return m;
      }
   }
}
