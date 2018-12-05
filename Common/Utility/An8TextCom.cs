using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8TextCom
   {
      internal static mesh Convert(textcom t)
      {
         // Convert the textcom to a mesh
         mesh m = new mesh();
         m.name = t.name;
         m.@base = t.@base;
         m.pivot = t.pivot;

         List<point> points = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // TODO: Support textcom?

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
