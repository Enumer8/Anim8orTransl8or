using Anim8orTransl8or.An8.V100;
using System;

namespace Anim8orTransl8or.Utility
{
   static class An8Math
   {
      internal static Double Length(texcoord t)
      {
         return Math.Sqrt(t.u * t.u + t.v * t.v);
      }

      internal static texcoord Normalize(texcoord t)
      {
         Double length = Length(t);

         return new texcoord()
         {
            u = t.u / length,
            v = t.v / length,
         };
      }

      internal static Double Length(point p)
      {
         return Math.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
      }

      internal static point Normalize(point p)
      {
         Double length = Length(p);

         return new point()
         {
            x = p.x / length,
            y = p.y / length,
            z = p.z / length,
         };
      }

      internal static Double Length(quaternion q)
      {
         return Math.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
      }

      internal static quaternion Normalize(quaternion q)
      {
         Double length = Length(q);

         return new quaternion()
         {
            x = q.x / length,
            y = q.y / length,
            z = q.z / length,
            w = q.w / length,
         };
      }

      internal static point Rotate(quaternion q, point p)
      {
         return new point()
         {
            x = (q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z) * p.x + ((q.x *
               q.y - q.w * q.z) * p.y + (q.x * q.z + q.w * q.y) * p.z) * 2,
            y = (q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z) * p.y + ((q.x *
               q.y + q.w * q.z) * p.x + (q.y * q.z - q.w * q.x) * p.z) * 2,
            z = (q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z) * p.z + ((q.x *
               q.z - q.w * q.y) * p.x + (q.y * q.z + q.w * q.x) * p.y) * 2,
         };
      }

      internal static Double[] Transform(@base @base)
      {
         // Row-major matrix
         Double[] matrix = new Double[16];
         matrix[0] = 1;
         matrix[5] = 1;
         matrix[10] = 1;
         matrix[15] = 1;

         if ( @base?.origin?.point != null )
         {
            matrix[3] = @base.origin.point.x;
            matrix[7] = @base.origin.point.y;
            matrix[11] = @base.origin.point.z;
         }

         if ( @base?.orientation?.quaternion != null )
         {
            Double x = @base.orientation.quaternion.x;
            Double y = @base.orientation.quaternion.y;
            Double z = @base.orientation.quaternion.z;
            Double w = @base.orientation.quaternion.w;

            matrix[0] = x * x - y * y - z * z + w * w;
            matrix[4] = x * y + x * y + z * w + z * w;
            matrix[8] = x * z + x * z - y * w - y * w;
            matrix[1] = x * y + x * y - z * w - z * w;
            matrix[5] = y * y - x * x - z * z + w * w;
            matrix[9] = x * w + x * w + y * z + y * z;
            matrix[2] = x * z + x * z + y * w + y * w;
            matrix[6] = y * z + y * z - x * w - x * w;
            matrix[10] = z * z - x * x - y * y + w * w;
         }

         return matrix;
      }
   }
}
