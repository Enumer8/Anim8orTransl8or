using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestAn8Weights : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Weights.Calculate(null, null, null, null);
         An8Weights.Calculate(new namedobject(), null, null, null);
      }

      [TestMethod]
      public void Simple()
      {
         CompareWeights(@"..\..\An8Weights\Weights_Simple.an8");
      }

      [TestMethod]
      public void Complex()
      {
         CompareWeights(@"..\..\An8Weights\Weights_Complex.an8");
      }

      static void CompareWeights(String weightFile)
      {
         ANIM8OR weight = LoadAn8File(weightFile);

         foreach ( figure figure in weight.figure )
         {
            CompareWeights(figure.bone, figure.bone, weight.@object);
         }
      }

      static void CompareWeights(bone1 bone, bone1 root, @object[] objects)
      {
         // Compare all named objects
         foreach ( namedobject namedobject in bone.namedobject ??
            new namedobject[0])
         {
            namedobject expected = namedobject;

            List<mesh> meshes = new List<mesh>();

            foreach ( @object @object in objects )
            {
               if ( @object.name == namedobject.objectname )
               {
                  meshes.AddRange(@object.mesh);
                  break;
               }
            }

            Int32 warnings = 0;

            namedobject actual = An8Weights.Calculate(
               expected,
               meshes.ToArray(),
               root,
               (String s) => warnings++);

            Assert.IsTrue(warnings == 0);

            CompareWeights(expected, actual);
         }

         foreach ( bone1 childBone in bone.bone ?? new bone1[0] )
         {
            CompareWeights(childBone, root, objects);
         }
      }

      static void CompareWeights(namedobject expected, namedobject actual)
      {
         Assert.AreEqual(
            expected?.weights?.Length,
            actual?.weights?.Length,
            $"Named object's weights length doesn't match");

         for ( Int32 i = 0; i < expected?.weights?.Length; i++ )
         {
            weights expectedWeights = expected.weights[i];
            weights actualWeights = actual.weights[i];

            Assert.AreEqual(
               expectedWeights?.weightdata?.Length,
               actualWeights?.weightdata?.Length,
               $"Named object's weight {i}'s data length doesn't match");

            for ( Int32 j = 0; j < expectedWeights?.weightdata?.Length; j++ )
            {
               weightdata expectedWeightdata = expectedWeights.weightdata[j];
               weightdata actualWeightdata = actualWeights.weightdata[j];

               Assert.AreEqual(
                  expectedWeightdata?.numweights,
                  actualWeightdata?.numweights,
                  $"Named object's weight {i}'s num weights doesn't match");

               Assert.AreEqual(
                  expectedWeightdata?.bonedata?.Length,
                  actualWeightdata?.bonedata?.Length,
                  $"Named object's weight {i}'s data {j}'s bone length doesn't match");

               for ( Int32 k = 0; k < expectedWeightdata?.bonedata?.Length; k++ )
               {
                  bonedata expectedBonedata = expectedWeightdata.bonedata[k];
                  bonedata actualBonedata = actualWeightdata.bonedata[k];

                  Assert.AreEqual(
                     expectedBonedata?.boneindex,
                     actualBonedata?.boneindex,
                     $"Named object's weight {i}'s data {j}'s bone {k}'s index doesn't match");

                  Assert.AreEqual(
                     expectedBonedata?.boneweight ?? 0,
                     actualBonedata?.boneweight ?? 0,
                     TOLERANCE,
                     $"Named object's weight {i}'s data {j}'s bone {k}'s weight doesn't match");
               }
            }
         }
      }
   }
}
