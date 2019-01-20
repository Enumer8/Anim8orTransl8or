using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Utility
{
   static class An8Weights
   {
      /// <summary>
      /// This will produce the same weights as if the user had clicked Weights
      /// in Anim8or v1.00.
      /// </summary>
      /// <param name="n">the named object</param>
      /// <param name="ms">the meshes to weight</param>
      /// <param name="s">the skeleton that owns the named object</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the named object with weights</returns>
      internal static namedobject Calculate(
         namedobject n,
         mesh[] ms,
         bone1 s,
         Action<String> callback = null)
      {
         namedobject n2 = new namedobject();
         n2.objectname = n?.objectname;
         n2.name = n?.name;
         n2.@base = n?.@base;
         n2.pivot = n?.pivot;
         n2.material = n?.material;
         n2.scale = n?.scale;
         n2.weightedby = n?.weightedby;
         n2.weights = null;

         // Calculate the absolute transformations of the skeleton
         List<SkeletonNode> skeletonNodes = new List<SkeletonNode>();
         CalculateSkeleton(skeletonNodes, s);

         // Find the absolute transformation of the bone that owns the object
         SkeletonNode parentNode = skeletonNodes.Find((SkeletonNode sn) =>
         {
            foreach ( namedobject no in sn.Bone?.namedobject ??
               new namedobject[0] )
            {
               if ( no == n )
               {
                  return true;
               }
            }

            return false;
         });

         matrix boneMatrix = new matrix(
            parentNode?.AbsoluteOrigin ?? new point(),
            parentNode?.AbsoluteOrientation ?? quaternion.IDENTITY);

         matrix objectMatrix = boneMatrix.Multiply(new matrix(
            n2.@base?.origin?.point ?? new point(),
            n2.@base?.orientation?.quaternion ?? quaternion.IDENTITY,
            n2.scale?.text ?? 1));

         // Sort the skeleton nodes that weight the object
         for ( Int32 i = 0; i < n2.weightedby?.Length; i++ )
         {
            String boneName = n2.weightedby[i]?.text;

            for ( Int32 j = 0; j < skeletonNodes.Count; j++ )
            {
               if ( skeletonNodes[j].Bone?.name == boneName )
               {
                  SkeletonNode sn = skeletonNodes[i];
                  skeletonNodes[i] = skeletonNodes[j];
                  skeletonNodes[j] = sn;
                  break;
               }
            }
         }

         // Remove skeleton nodes that do not weight the object
         if ( skeletonNodes.Count > n2.weightedby?.Length )
         {
            skeletonNodes.RemoveRange(
               n2.weightedby.Length,
               skeletonNodes.Count - n2.weightedby.Length);
         }

         // Calculate the weights for each mesh
         foreach ( mesh m in ms ?? new mesh[0] )
         {
            matrix meshMatrix = objectMatrix.Multiply(new matrix(
               m?.@base?.origin?.point ?? new point(),
               m?.@base?.orientation?.quaternion ?? quaternion.IDENTITY));

            weights weights = new weights();
            n2.weights = n2.weights.Append(weights);

            weights.meshname = m?.name?.text;

            weightdata[] weightdata = new weightdata[
               m?.points?.point?.Length ?? 0];
            weights.weightdata = weightdata;

            for ( Int32 i = 0; i < weightdata.Length; i++ )
            {
               weightdata wd = new weightdata();
               weightdata[i] = wd;

               // Transform the point to its final location
               point point = meshMatrix.Multiply(m.points.point[i]);

               CalculateWeight(skeletonNodes, wd, point);

               // Add the default weight if none were found
               wd.bonedata = wd.bonedata ?? new bonedata[]
               {
                  new bonedata() { boneindex = 0, boneweight = 1 },
               };

               wd.numweights = wd.bonedata.Length;

               // Normalize the weights
               Double totalWeight = 0;

               for ( Int32 j = 0; j < wd.bonedata.Length; j++ )
               {
                  totalWeight += wd.bonedata[j].boneweight;
               }

               for ( Int32 j = 0; j < wd.bonedata.Length; j++ )
               {
                  wd.bonedata[j].boneweight /= totalWeight;
               }

               // Sort the weights
               Array.Sort(
                  wd.bonedata,
                  (bonedata b1, bonedata b2) =>
                  {
                     if ( b1.boneweight != b2.boneweight )
                     {
                        // Sort larger weights first
                        return b2.boneweight.CompareTo(b1.boneweight);
                     }
                     else
                     {
                        // If the weights match, sort lower indices first
                        return b1.boneindex.CompareTo(b2.boneindex);
                     }
                  });
            }
         }

         return n2;
      }

      static void CalculateWeight(
         List<SkeletonNode> skeletonNodes,
         weightdata wd,
         point point)
      {
         for ( Int32 i = 0; i < skeletonNodes.Count; i++ )
         {
            SkeletonNode skeletonNode = skeletonNodes[i];

            // Note: Anim8or v1.00 doesn't ignore the root bone's length when
            // calculating the influences (only when calculating the positions
            // of the other bones).
            Double length = skeletonNode.Bone?.length?.text ?? 0;
            point origin = skeletonNode.AbsoluteOrigin;
            quaternion orientation = skeletonNode.AbsoluteOrientation;
            influence influence = skeletonNode.Bone?.influence;

            if ( influence == null )
            {
               // Note: These defaults were reversed engineered.
               influence = new influence();
               influence.center0 = 0.25;
               influence.inradius0 = 0.15 * length;
               influence.outradius0 = 0.4 * length;
               influence.center1 = 0.75;
               influence.inradius1 = influence.inradius0;
               influence.outradius1 = influence.outradius0;
            }

            point boneVector = orientation.Rotate(new point(0, length, 0));
            point influenceStart = origin + boneVector * influence.center0;
            point influenceEnd = origin + boneVector * influence.center1;
            point influenceVector = influenceEnd - influenceStart;
            Double influenceLength = influenceVector.GetLength();
            influenceVector = influenceVector.Normalize();

            point startToPoint = point - influenceStart;
            Double t = influenceVector.Dot(startToPoint);

            Double distance, inRadius, outRadius;

            // If the point is "before" the start point
            if ( t < 0 )
            {
               distance = startToPoint.GetLength();
               inRadius = influence.inradius0;
               outRadius = influence.outradius0;
            }
            // If the point is "after" the end point
            else if ( t > influenceLength )
            {
               point endToPoint = point - influenceEnd;

               distance = endToPoint.GetLength();
               inRadius = influence.inradius1;
               outRadius = influence.outradius1;
            }
            // If the point is between the start and end
            else
            {
               Double percent = t / influenceLength;

               point middleToPoint = point -
                  (influenceStart + influenceVector * t);

               distance = middleToPoint.GetLength();

               inRadius = percent * influence.inradius1 +
                  (1 - percent) * influence.inradius0;

               outRadius = percent * influence.outradius1 +
                  (1 - percent) * influence.outradius0;
            }

            Double boneWeight;

            // If the distance is inside the inner radius
            if ( distance <= inRadius )
            {
               boneWeight = 1.0;
            }
            // If the distance is outside the outer radius
            else if ( distance >= outRadius )
            {
               boneWeight = 0.0;
            }
            // If the distance is between the inner and outer radius
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
         List<SkeletonNode> skeletonNodes,
         bone1 bone,
         SkeletonNode parentNode = null)
      {
         SkeletonNode node = new SkeletonNode();
         node.Bone = bone;

         if ( parentNode != null )
         {
            node.Length = bone?.length?.text ?? 0.0;
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
            node.AbsoluteOrientation = bone?.orientation?.quaternion ??
               quaternion.IDENTITY;
         }

         skeletonNodes.Add(node);

         foreach ( bone1 childBone in bone?.bone ?? new bone1[0] )
         {
            CalculateSkeleton(skeletonNodes, childBone, node);
         }
      }
   }
}
