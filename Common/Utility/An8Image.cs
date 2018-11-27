using Anim8orTransl8or.An8.V100;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Image
   {
      internal static mesh Convert(image i)
      {
         // Convert the image to a mesh
         mesh m = new mesh();
         m.name = i.name;
         m.@base = i.@base;
         m.pivot = i.pivot;

         List<point> points = new List<point>();
         List<point> normals = new List<point>();
         List<texcoord> texcoords = new List<texcoord>();
         List<facedata> facedatas = new List<facedata>();

         // TODO: Support image?

         if ( points.Count > 0 )
         {
            m.points = new points();
            m.points.point = points.ToArray();
         }

         if ( normals.Count > 0 )
         {
            m.normals = new normals();
            m.normals.point = normals.ToArray();
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
