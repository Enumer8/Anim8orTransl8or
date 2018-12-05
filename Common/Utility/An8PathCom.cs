using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8PathCom
   {
      internal static mesh Convert(pathcom p)
      {
         // Convert the pathcom to a mesh
         mesh m = new mesh();
         m.name = p.name;
         m.@base = p.@base;
         m.pivot = p.pivot;

         List<point> points = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // TODO: Support pathcom?

         if ( points.Count > 0 )
         {
            m.points = new points();
            m.points.point = points.ToArray();
         }

         if ( texcoords.Count > 0 )
         {
            m.texcoords = new texcoords();
            m.texcoords.texcoord = texcoords.ToArray();
         }

         if ( facedatas.Count > 0 )
         {
            m.faces = new faces();
            m.faces.facedata = facedatas.ToArray();
         }

         return m;
      }
   }
}
