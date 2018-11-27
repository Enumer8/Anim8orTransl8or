using Anim8orTransl8or.An8.V100;

namespace Anim8orTransl8or.Utility
{
   static class An8Subdivision
   {
      internal static mesh Convert(subdivision s)
      {
         // Convert the subdivision to a mesh
         mesh m = new mesh();
         m.name = s.name;
         m.@base = s.@base;
         m.pivot = s.pivot;
         m.smoothangle = s.smoothangle;
         m.material = s.material;
         m.materiallist = s.materiallist;
         m.points = s.points;
         m.normals = s.normals;
         m.edges = s.edges;
         m.texcoords = s.texcoords;
         m.faces = s.faces;

         // TODO: Do we need to actually subdivide using 'working' or
         // 'divisions'? How do we do it?

         return m;
      }
   }
}
