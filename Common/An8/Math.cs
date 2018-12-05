using System;

namespace Anim8orTransl8or.An8
{
   public struct texcoord
   {
      public Double u;
      public Double v;

      public texcoord(Double u = 0, Double v = 0)
      {
         this.u = u;
         this.v = v;
      }

      public Double GetLength()
      {
         return Math.Sqrt(u * u + v * v);
      }

      public texcoord Normalize()
      {
         Double length = GetLength();

         return new texcoord()
         {
            u = u / length,
            v = v / length,
         };
      }

      public static texcoord operator +(texcoord a, texcoord b)
      {
         return new texcoord
         {
            u = a.u + b.u,
            v = a.v + b.v,
         };
      }

      public static texcoord operator -(texcoord a, texcoord b)
      {
         return new texcoord
         {
            u = a.u - b.u,
            v = a.v - b.v,
         };
      }

      public static texcoord operator *(texcoord a, Double b)
      {
         return new texcoord
         {
            u = a.u * b,
            v = a.v * b,
         };
      }

      public static texcoord operator /(texcoord a, Double b)
      {
         return new texcoord
         {
            u = a.u / b,
            v = a.v / b,
         };
      }
   }

   public struct point
   {
      public Double x;
      public Double y;
      public Double z;

      public point(Double x = 0, Double y = 0, Double z = 0)
      {
         this.x = x;
         this.y = y;
         this.z = z;
      }

      public Double GetLength()
      {
         return Math.Sqrt(x * x + y * y + z * z);
      }

      public point Normalize()
      {
         Double length = GetLength();

         return new point()
         {
            x = x / length,
            y = y / length,
            z = z / length,
         };
      }

      public Double Dot(point b)
      {
         return x * b.x + y * b.y + z * b.z;
      }

      public point Cross(point b)
      {
         return new point()
         {
            x = y * b.z - z * b.y,
            y = z * b.x - x * b.z,
            z = x * b.y - y * b.x,
         };
      }

      public static point operator +(point a, point b)
      {
         return new point
         {
            x = a.x + b.x,
            y = a.y + b.y,
            z = a.z + b.z,
         };
      }

      public static point operator -(point a, point b)
      {
         return new point
         {
            x = a.x - b.x,
            y = a.y - b.y,
            z = a.z - b.z,
         };
      }

      public static point operator *(point a, Double b)
      {
         return new point
         {
            x = a.x * b,
            y = a.y * b,
            z = a.z * b,
         };
      }

      public static point operator /(point a, Double b)
      {
         return new point
         {
            x = a.x / b,
            y = a.y / b,
            z = a.z / b,
         };
      }
   }

   public struct quaternion
   {
      public Double x;
      public Double y;
      public Double z;
      public Double w;

      public quaternion(Double x = 0, Double y = 0, Double z = 0, Double w = 0)
      {
         this.x = x;
         this.y = y;
         this.z = z;
         this.w = w;
      }

      public Double GetLength()
      {
         return Math.Sqrt(x * x + y * y + z * z + w * w);
      }

      public quaternion Normalize()
      {
         Double length = GetLength();

         return new quaternion()
         {
            x = x / length,
            y = y / length,
            z = z / length,
            w = w / length,
         };
      }

      public quaternion Rotate(quaternion q)
      {
         return new quaternion()
         {
            x = w * q.x + x * q.w + y * q.z - z * q.y,
            y = w * q.y - x * q.z + y * q.w + z * q.x,
            z = w * q.z + x * q.y - y * q.x + z * q.w,
            w = w * q.w - x * q.x - y * q.y - z * q.z,
         };
      }

      public point Rotate(point p)
      {
         return new point()
         {
            x = (w * w + x * x - y * y - z * z) * p.x +
                ((x * y - z * w) * p.y + (x * z + y * w) * p.z) * 2,
            y = (w * w - x * x + y * y - z * z) * p.y +
                ((x * y + z * w) * p.x + (y * z - x * w) * p.z) * 2,
            z = (w * w - x * x - y * y + z * z) * p.z +
                ((x * z - y * w) * p.x + (y * z + x * w) * p.y) * 2,
         };
      }

      public Double[] ToMatrix(point location)
      {
         // Row-major matrix
         return new Double[]
         {
            x * x - y * y - z * z + w * w,
            x * y + x * y - z * w - z * w,
            x * z + x * z + y * w + y * w,
            location.x,
            x * y + x * y + z * w + z * w,
            y * y - x * x - z * z + w * w,
            y * z + y * z - x * w - x * w,
            location.y,
            x * z + x * z - y * w - y * w,
            x * w + x * w + y * z + y * z,
            z * z - x * x - y * y + w * w,
            location.z,
            0,
            0,
            0,
            1,
         };
      }

      public static quaternion operator +(quaternion a, quaternion b)
      {
         return new quaternion
         {
            x = a.x + b.x,
            y = a.y + b.y,
            z = a.z + b.z,
            w = a.w + b.w,
         };
      }

      public static quaternion operator -(quaternion a, quaternion b)
      {
         return new quaternion
         {
            x = a.x - b.x,
            y = a.y - b.y,
            z = a.z - b.z,
            w = a.w - b.w,
         };
      }

      public static quaternion operator *(quaternion a, Double b)
      {
         return new quaternion
         {
            x = a.x * b,
            y = a.y * b,
            z = a.z * b,
            w = a.w * b,
         };
      }

      public static quaternion operator /(quaternion a, Double b)
      {
         return new quaternion
         {
            x = a.x / b,
            y = a.y / b,
            z = a.z / b,
            w = a.w / b,
         };
      }
   }
}
