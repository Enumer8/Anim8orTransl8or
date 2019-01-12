using Anim8orTransl8or.An8.V100;
using System;

namespace Anim8orTransl8or.Utility
{
   static class An8Subdivision
   {
      /// <summary>
      /// This will produce the same points, texcoords, and vertices as if the
      /// user had clicked Build->Convert to Mesh in Anim8or v1.00.
      /// </summary>
      /// <param name="s">the subdivision</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the converted mesh</returns>
      internal static mesh Calculate(
         subdivision s,
         Action<String> callback = null)
      {
         mesh m = new mesh();
         m.name = s?.name;
         m.@base = s?.@base;
         m.pivot = s?.pivot;
         m.smoothangle = s?.smoothangle;
         m.material = s?.material;
         m.materiallist = s?.materiallist;
         m.points = s?.points;
         m.normals = s?.normals;
         m.edges = s?.edges;
         m.texcoords = s?.texcoords;
         m.faces = s?.faces;

         // TODO: Do we need to actually subdivide using 'working' or
         // 'divisions'? How do we do it?
         callback?.Invoke($"The \"{s?.name?.text}\" subdivision is not supported. Please convert it to a mesh in Anim8or.");

         return m;
      }
   }
}
