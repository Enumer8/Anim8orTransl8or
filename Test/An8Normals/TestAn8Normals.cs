using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestAn8Normals : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Normals.Calculate(null);
         An8Normals.Calculate(new mesh(), null);
      }

      [TestMethod]
      public void Not_Smooth()
      {
         CompareNormals(@"..\..\An8Normals\Normals_Fully_Smooth.an8");
      }

      [TestMethod]
      public void Partially_Smooth()
      {
         CompareNormals(@"..\..\An8Normals\Normals_Partially_Smooth.an8");
      }

      [TestMethod]
      public void Fully_Smooth()
      {
         CompareNormals(@"..\..\An8Normals\Normals_Fully_Smooth.an8");
      }

      // TODO: Will the normal calculation ever be good enough to enable this?
      //[TestMethod]
      public void Example()
      {
         CompareNormals(@"..\..\..\Examples\Cat.an8");
      }

      static void CompareNormals(String normalFile)
      {
         Int32 warnings = 0;

         ANIM8OR normal = LoadAn8File(normalFile);

         mesh expected = normal.@object[0].mesh[0];

         mesh actual = An8Normals.Calculate(
            expected,
            (String s) => warnings++);

         CompareNormals(expected, actual);
      }

      static void CompareNormals(mesh expected, mesh actual)
      {
         Assert.AreEqual(
            expected?.faces?.facedata.Length,
            actual?.faces?.facedata.Length,
            $"Mesh's faces length doesn't match");

         Int32 expectedLength = expected?.normals?.point?.Length ?? 0;
         Int32 actualLength = actual?.normals?.point?.Length ?? 0;

         for ( Int32 i = 0; i < expected?.faces?.facedata?.Length; i++ )
         {
            facedata expectedFacedata = expected.faces.facedata[i];
            facedata actualFacedata = actual.faces.facedata[i];

            Assert.AreEqual(
               expectedFacedata?.numpoints,
               actualFacedata?.numpoints,
               $"Mesh's face {i}'s num points doesn't match");

            Assert.AreEqual(
               expectedFacedata?.flags,
               actualFacedata?.flags,
               $"Mesh's face {i}'s flags doesn't match");

            Assert.AreEqual(
               expectedFacedata?.matno,
               actualFacedata?.matno,
               $"Mesh's face {i}'s mat no doesn't match");

            point expectedFaceNormal = expected.normals.point[
               expectedFacedata.flatnormalno];

            point actualFaceNormal = actual.normals.point[
               actualFacedata.flatnormalno];

            Assert.AreEqual(
               expectedFaceNormal.x,
               actualFaceNormal.x,
               TOLERANCE,
               $"Mesh's face {i}'s normal X doesn't match");

            Assert.AreEqual(
               expectedFaceNormal.y,
               actualFaceNormal.y,
               TOLERANCE,
               $"Mesh's face {i}'s normal Y doesn't match");

            Assert.AreEqual(
               expectedFaceNormal.z,
               actualFaceNormal.z,
               TOLERANCE,
               $"Mesh's face {i}'s normal Z doesn't match");


            Assert.AreEqual(
               expectedFacedata?.pointdata?.Length,
               actualFacedata?.pointdata?.Length,
               $"Mesh's face {i}'s point length doesn't match");

            for ( Int32 j = 0; j < expectedFacedata?.pointdata?.Length; j++ )
            {
               pointdata expectedPointdata = expectedFacedata.pointdata[j];
               pointdata actualPointdata = actualFacedata.pointdata[j];

               Int64 expectedIndex = expectedPointdata?.normalindex ?? -1;
               Int64 actualIndex = actualPointdata?.normalindex ?? -1;

               Assert.IsTrue(
                  expectedIndex >= 0 && expectedIndex < expectedLength,
                  "Expected normal index is out of bounds");

               Assert.IsTrue(
                  actualIndex >= 0 && actualIndex < actualLength,
                  "Actual normal index is out of bounds");

               point expectedNormal = expected.normals.point[expectedIndex];
               point actualNormal = actual.normals.point[actualIndex];

               Assert.AreEqual(
                  expectedNormal.x,
                  actualNormal.x,
                  TOLERANCE,
                  $"Mesh's face {i}'s data {j}'s normal X doesn't match");

               Assert.AreEqual(
                  expectedNormal.y,
                  actualNormal.y,
                  TOLERANCE,
                  $"Mesh's face {i}'s data {j}'s normal Y doesn't match");

               Assert.AreEqual(
                  expectedNormal.z,
                  actualNormal.z,
                  TOLERANCE,
                  $"Mesh's face {i}'s data {j}'s normal Z doesn't match");
            }
         }
      }
   }
}
