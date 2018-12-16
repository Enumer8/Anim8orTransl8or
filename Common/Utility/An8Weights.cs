using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Weights
   {
      internal static namedobject Calculate(
         namedobject namedobject,
         @object[] objects,
         bone1 bone,
         bone1 skeleton)
      {
         List<SkeletonNode> skeletonNodes = new List<SkeletonNode>();
         CalculateSkeleton(skeleton, skeletonNodes);

         // Find the "base" of the bone that owns the object
         point boneOrigin;
         quaternion boneOrientation;

         SkeletonNode parentNode = skeletonNodes.Find(n => n.Bone == bone);

         if ( parentNode != null )
         {
            boneOrigin = parentNode.AbsoluteOrigin;
            boneOrientation = parentNode.AbsoluteOrientation;
         }
         else
         {
            boneOrigin = new point();
            boneOrientation = quaternion.IDENTITY;
         }

         // Sort the bones that weight the object
         for ( Int32 i = 0; i < namedobject.weightedby?.Length; i++ )
         {
            String boneName = namedobject.weightedby[i].text;

            for ( Int32 j = 0; j < skeletonNodes.Count; j++ )
            {
               if ( skeletonNodes[j].Bone.name == boneName )
               {
                  SkeletonNode temp = skeletonNodes[i];
                  skeletonNodes[i] = skeletonNodes[j];
                  skeletonNodes[j] = temp;
                  break;
               }
            }
         }

         // Remove bones that do not weight the object
         if ( skeletonNodes.Count > namedobject.weightedby?.Length )
         {
            skeletonNodes.RemoveRange(
               namedobject.weightedby.Length,
               skeletonNodes.Count - namedobject.weightedby.Length);
         }

         @object @object = null;

         // Find the named object
         foreach ( @object o in objects ?? new @object[0] )
         {
            if ( o.name == namedobject.objectname )
            {
               @object = o;
               break;
            }
         }

         // Calculate the weights for each mesh
         foreach ( mesh mesh in @object?.mesh ?? new mesh[0] )
         {
            Boolean missing = true;

            foreach ( weights weights in namedobject.weights ??
               new weights[0] )
            {
               if ( weights.meshname == mesh.name?.text )
               {
                  missing = false;
                  break;
               }
            }

            if ( missing )
            {
               weights weights = new weights();
               namedobject.weights = namedobject.weights.Append(weights);

               weights.meshname = mesh.name?.text;

               weightdata[] weightdata = new weightdata[
                  mesh.points?.point?.Length ?? 0];
               weights.weightdata = weightdata;

               Double objectScale = namedobject.scale?.text ?? 1;
               point objectOrigin = boneOrigin + boneOrientation.Rotate(
                  namedobject.@base?.origin?.point ?? new point());
               quaternion objectOrientation = boneOrientation.Rotate(
                  namedobject.@base?.orientation?.quaternion ??
                  quaternion.IDENTITY).Normalize();

               point meshOrigin = mesh.@base?.origin?.point ?? new point();
               quaternion meshOrientation =
                  mesh.@base?.orientation?.quaternion.Normalize() ??
                  quaternion.IDENTITY;

               for ( Int32 i = 0; i < weightdata.Length; i++ )
               {
                  weightdata wd = new weightdata();
                  weightdata[i] = wd;

                  point meshPoint = meshOrientation.Rotate(mesh.points.point[i]) + meshOrigin;
                  point objectPoint = objectOrientation.Rotate(meshPoint * objectScale) + objectOrigin;
                  point bonePoint = boneOrientation.Rotate(objectPoint) + boneOrigin;

                  CalculateWeight(wd, bonePoint, skeletonNodes);

                  wd.numweights = wd.bonedata?.Length ?? 0;

                  // Normalize the weights
                  Double totalWeight = 0;
                  for ( Int32 j = 0; j < wd.numweights; j++ )
                  {
                     totalWeight += wd.bonedata[j].boneweight;
                  }
                  totalWeight = 1 / totalWeight;
                  for ( Int32 j = 0; j < wd.numweights; j++ )
                  {
                     wd.bonedata[j].boneweight *= totalWeight;
                  }
               }
            }
         }

         return namedobject;
      }

      static void CalculateWeight(
         weightdata wd,
         point point,
         List<SkeletonNode> skeletonNodes)
      {
         for ( Int32 i = 0; i < skeletonNodes.Count; i++ )
         {
            SkeletonNode skeletonNode = skeletonNodes[i];
            Double boneWeight;

            if ( skeletonNode.Bone?.influence == null )
            {
               continue;
            }

            point boneVector = skeletonNode.AbsoluteOrientation.Rotate(new point(0, skeletonNode.Length, 0));
            point start = skeletonNode.AbsoluteOrigin + boneVector * skeletonNode.Bone.influence.center0;
            point end = skeletonNode.AbsoluteOrigin + boneVector * skeletonNode.Bone.influence.center1;
            point startToPoint = point - start;
            point b = end - start;
            Double d = b.GetLength();
            b = b.Normalize();
            Double t = b.Dot(startToPoint);

            Double distance, inRadius, outRadius;

            if ( t < 0 )
            {
               distance = startToPoint.GetLength();
               inRadius = skeletonNode.Bone.influence.inradius0;
               outRadius = skeletonNode.Bone.influence.outradius0;
            }
            else if ( t > d )
            {
               point endToPoint = point - end;

               distance = endToPoint.GetLength();
               inRadius = skeletonNode.Bone.influence.inradius1;
               outRadius = skeletonNode.Bone.influence.outradius1;
            }
            else
            {
               point middleToPoint = point - (start + b * t);

               distance = middleToPoint.GetLength();
               Double percent = t / d;
               inRadius = percent * (skeletonNode.Bone.influence.inradius1 - skeletonNode.Bone.influence.inradius0) + skeletonNode.Bone.influence.inradius0;
               outRadius = percent * (skeletonNode.Bone.influence.outradius1 - skeletonNode.Bone.influence.outradius0) + skeletonNode.Bone.influence.outradius0;
            }

            if ( distance >= outRadius )
            {
               boneWeight = 0.0;
            }
            else if ( distance <= inRadius )
            {
               boneWeight = 1.0;
            }
            else
            {
               boneWeight = (outRadius - distance) / (outRadius - inRadius);
               boneWeight = Math.Min(Math.Max(boneWeight, 0), 1);
            }

            if ( boneWeight > 0 )
            {
               bonedata bd = new bonedata();
               wd.bonedata = wd.bonedata.Append(bd);

               bd.boneindex = i;
               bd.boneweight = boneWeight;
            }
         }
      }

      class SkeletonNode
      {
         public bone1 Bone;
         public Double Length;
         public point AbsoluteOrigin;
         public quaternion AbsoluteOrientation;
      }

      static void CalculateSkeleton(
         bone1 bone,
         List<SkeletonNode> skeletonNodes,
         SkeletonNode parentNode = null)
      {
         SkeletonNode node = new SkeletonNode();
         node.Bone = bone;

         if ( parentNode != null )
         {
            node.Length = bone.length?.text ?? 0.0;
            node.AbsoluteOrigin = parentNode.AbsoluteOrigin +
               parentNode.AbsoluteOrientation.Rotate(
                  new point(0, parentNode.Length, 0));
            node.AbsoluteOrientation = parentNode.AbsoluteOrientation.Rotate(
               bone.orientation?.quaternion ?? quaternion.IDENTITY)
               .Normalize();
         }
         else
         {
            // Note: Anim8or v1.00 ignores the root bone's length
            node.AbsoluteOrientation = bone.orientation?.quaternion ??
               quaternion.IDENTITY;
         }

         skeletonNodes.Add(node);

         foreach ( bone1 childBone in bone.bone ?? new bone1[0] )
         {
            CalculateSkeleton(childBone, skeletonNodes, node);
         }
      }
   }
}
