using Anim8orTransl8or.An8.V100;
using System;

namespace Anim8orTransl8or.Utility
{
   static class An8Image
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="i">the image</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the converted mesh</returns>
      internal static mesh Calculate(image i, Action<String> callback = null)
      {
         mesh m = new mesh();
         m.name = i?.name;
         m.@base = i?.@base;
         m.pivot = i?.pivot;

         // TODO: Support image?
         callback?.Invoke($"The \"{i?.name?.text}\" image is not supported. Please convert it to a mesh in Anim8or.");

         return m;
      }
   }
}
