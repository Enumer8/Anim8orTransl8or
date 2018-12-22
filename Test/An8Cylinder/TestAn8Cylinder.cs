using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestAn8Cylinder
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Cylinder.Calculate(null);
         An8Cylinder.Calculate(new cylinder());

         // Test a cylinder with a couple invalid settings
         cylinder cylinder = new cylinder();
         cylinder.name = new @string() { text = "EdgeCase" };
         cylinder.length = new @float() { text = -1 }; // invalid
         cylinder.diameter = new @float() { text = 1 };
         cylinder.topdiameter = new @float() { text = 1 };
         cylinder.longlat = new longlat();
         cylinder.longlat.longitude = 2; // invalid
         cylinder.longlat.latitude = 1;

         Int32 warnings = 0;
         An8Cylinder.Calculate(cylinder, (String s) => warnings++);
         Assert.IsTrue(warnings > 0);
      }

      [TestMethod]
      public void Lon3_Lat1()
      {
         CompareCylinder(
            @"..\..\An8Cylinder\Cylinder_Lon3_Lat1.an8",
            @"..\..\An8Cylinder\Cylinder_Lon3_Lat1_Mesh.an8");
      }

      [TestMethod]
      public void Lon4_Lat5()
      {
         CompareCylinder(
            @"..\..\An8Cylinder\Cylinder_Lon4_Lat5.an8",
            @"..\..\An8Cylinder\Cylinder_Lon4_Lat5_Mesh.an8");
      }


      [TestMethod]
      public void Lon6_Lat6()
      {
         CompareCylinder(
            @"..\..\An8Cylinder\Cylinder_Lon6_Lat6.an8",
            @"..\..\An8Cylinder\Cylinder_Lon6_Lat6_Mesh.an8");
      }

      [TestMethod]
      public void Lon11_Lat7()
      {
         CompareCylinder(
            @"..\..\An8Cylinder\Cylinder_Lon11_Lat7.an8",
            @"..\..\An8Cylinder\Cylinder_Lon11_Lat7_Mesh.an8");
      }

      [TestMethod]
      public void Lon4_Lat2()
      {
         CompareCylinder(
            @"..\..\An8Cylinder\Cone_Lon4_Lat2.an8",
            @"..\..\An8Cylinder\Cone_Lon4_Lat2_Mesh.an8");
      }

      [TestMethod]
      public void Lon5_Lat2()
      {
         CompareCylinder(
            @"..\..\An8Cylinder\Cone_Lon5_Lat2.an8",
            @"..\..\An8Cylinder\Cone_Lon5_Lat2_Mesh.an8");
      }

      static void CompareCylinder(String cylinderFile, String meshFile)
      {
         Int32 warnings = 0;

         ANIM8OR cylinder = LoadAn8File(cylinderFile);
         ANIM8OR mesh = LoadAn8File(meshFile);

         mesh expected = mesh.@object[0].mesh[0];
         mesh actual = An8Cylinder.Calculate(
            cylinder.@object[0].cylinder[0],
            (String s) => warnings++);

         Assert.IsTrue(warnings == 0);

         Assert.AreEqual(
            expected.name.text,
            actual.name.text,
            "Calculated cylinder's name doesn't match.");

         Assert.AreEqual(
            expected.material.name,
            actual.material.name,
            "Calculated cylinder's material doesn't match.");

         Assert.AreEqual(
            expected.materiallist.materialname.Length,
            actual.materiallist.materialname.Length,
            "Calculated cylinder's material list length doesn't match.");

         Assert.AreEqual(
            expected.materiallist.materialname[0].text,
            actual.materiallist.materialname[0].text,
            "Calculated cylinder's material list doesn't match.");

         Assert.AreEqual(
            expected.points.point.Length,
            actual.points.point.Length,
            "Calculated cylinder's points length doesn't match.");

         for ( Int32 i = 0; i < expected.points.point.Length; i++ )
         {
            point expectedPoint = expected.points.point[i];
            point actualPoint = actual.points.point[i];

            Assert.AreEqual(
               expectedPoint.x,
               actualPoint.x,
               TOLERANCE,
               $"Calculated cylinder's point {i}'s X doesn't match");

            Assert.AreEqual(
               expectedPoint.y,
               actualPoint.y,
               TOLERANCE,
               $"Calculated cylinder's point {i}'s Y doesn't match");

            Assert.AreEqual(
               expectedPoint.z,
               actualPoint.z,
               TOLERANCE,
               $"Calculated cylinder's point {i}'s Z doesn't match");
         }

         // Note: The normals should be null.
         Assert.AreEqual(
            expected.normals,
            actual.normals,
            "Calculated cylinder's normals length doesn't match.");

         Assert.AreEqual(
            expected.texcoords.texcoord.Length,
            actual.texcoords.texcoord.Length,
            "Calculated cylinder's tex coords list doesn't match");

         for ( Int32 i = 0; i < expected.texcoords.texcoord.Length; i++ )
         {
            texcoord expectedTexcoord = expected.texcoords.texcoord[i];
            texcoord actualTexcoord = actual.texcoords.texcoord[i];

            Assert.AreEqual(
               expectedTexcoord.u,
               actualTexcoord.u,
               TOLERANCE,
               $"Calculated cylinder's tex coord {i}'s U doesn't match");

            Assert.AreEqual(
               expectedTexcoord.v,
               actualTexcoord.v,
               TOLERANCE,
               $"Calculated cylinder's tex coord {i}'s V doesn't match");
         }

         Assert.AreEqual(
            expected.faces.facedata.Length,
            actual.faces.facedata.Length,
            "Calculated cylinder's faces list doesn't match");

         for ( Int32 i = 0; i < expected.faces.facedata.Length; i++ )
         {
            facedata expectedFacedata = expected.faces.facedata[i];
            facedata actualFacedata = actual.faces.facedata[i];

            Assert.AreEqual(
               expectedFacedata.numpoints,
               actualFacedata.numpoints,
               $"Calculated cylinder's face {i}'s num points doesn't match");

            Assert.AreEqual(
               expectedFacedata.flags,
               actualFacedata.flags,
               $"Calculated cylinder's face {i}'s flags doesn't match");

            Assert.AreEqual(
               expectedFacedata.matno,
               actualFacedata.matno,
               $"Calculated cylinder's face {i}'s mat no doesn't match");

            // Note: The flat normal no should be -1.
            Assert.AreEqual(
               expectedFacedata.flatnormalno,
               actualFacedata.flatnormalno,
               $"Calculated cylinder's face {i}'s flat normal no doesn't match");

            Assert.AreEqual(
               expectedFacedata.pointdata.Length,
               actualFacedata.pointdata.Length,
               $"Calculated cylinder's face {i}'s points list doesn't match");

            for ( Int32 j = 0; j < expectedFacedata.pointdata.Length; j++ )
            {
               pointdata expectedPointdata = expectedFacedata.pointdata[j];
               pointdata actualPointdata = actualFacedata.pointdata[j];

               Assert.AreEqual(
                  expectedPointdata.pointindex,
                  actualPointdata.pointindex,
                  $"Calculated cylinder's face {i}'s data {j}'s point index doesn't match");

               // Note: The normal index should be 0.
               Assert.AreEqual(
                  expectedPointdata.normalindex,
                  actualPointdata.normalindex,
                  $"Calculated cylinder's face {i}'s data {j}'s normal index doesn't match");

               Assert.AreEqual(
                  expectedPointdata.texcoordindex,
                  actualPointdata.texcoordindex,
                  $"Calculated cylinder's face {i}'s data {j}'s tex coord index doesn't match");
            }
         }
      }

      static ANIM8OR LoadAn8File(String file)
      {
         ANIM8OR an8;

         using ( Stream stream = File.Open(file, FileMode.Open) )
         {
            An8Serializer deserializer = new An8Serializer(typeof(ANIM8OR));
            an8 = (ANIM8OR)deserializer.Deserialize(stream);
         }

         return an8;
      }

      const Double TOLERANCE = 0.001;
   }
}
