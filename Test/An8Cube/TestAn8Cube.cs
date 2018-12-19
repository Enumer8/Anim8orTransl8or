using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Anim8orTransl8or.Test.An8Cube
{
   [TestClass]
   public class TestAn8Cube
   {
      [TestMethod]
      public void Null()
      {
         mesh mesh = Utility.An8Cube.Calculate(null);
         Assert.AreEqual(null, mesh.name);
         Assert.AreEqual(null, mesh.@base);
         Assert.AreEqual(null, mesh.pivot);
         Assert.AreEqual(null, mesh.material);
         Assert.AreEqual(null, mesh.smoothangle);
         Assert.AreEqual(null, mesh.materiallist);
         Assert.AreEqual(null, mesh.points);
         Assert.AreEqual(null, mesh.normals);
         Assert.AreEqual(null, mesh.edges);
         Assert.AreEqual(null, mesh.texcoords);
         Assert.AreEqual(null, mesh.faces);
      }

      [TestMethod]
      public void Degenerate()
      {
         cube cube = new cube();
         cube.name = new @string();
         cube.name.text = "Degenerate";
         cube.scale = new scale();
         cube.scale.x = 1;
         cube.scale.y = 2;
         cube.scale.z = 3;
         cube.divisions = new divisions();
         cube.divisions.x = 2;
         cube.divisions.y = 1;
         cube.divisions.z = 0; // degenerate

         Int32 warnings = 0;

         mesh mesh = Utility.An8Cube.Calculate(cube, (String s) => warnings++);
         Assert.IsTrue(warnings > 0);
         Assert.AreEqual(cube.name, mesh.name);
         Assert.AreEqual(cube.@base, mesh.@base);
         Assert.AreEqual(cube.pivot, mesh.pivot);
         Assert.AreEqual(cube.material, mesh.material);
         Assert.AreEqual(null, mesh.smoothangle);
         Assert.AreEqual(null, mesh.materiallist);
         Assert.AreEqual(null, mesh.points);
         Assert.AreEqual(null, mesh.normals);
         Assert.AreEqual(null, mesh.edges);
         Assert.AreEqual(null, mesh.texcoords);
         Assert.AreEqual(null, mesh.faces);
      }

      [TestMethod]
      public void X1_Y1_Z1()
      {
         CompareCube(
            @"..\..\An8Cube\Cube_X1_Y1_Z1.an8",
            @"..\..\An8Cube\Cube_X1_Y1_Z1_Mesh.an8");
      }

      [TestMethod]
      public void X2_Y3_Z5()
      {
         CompareCube(
            @"..\..\An8Cube\Cube_X2_Y3_Z5.an8",
            @"..\..\An8Cube\Cube_X2_Y3_Z5_Mesh.an8");
      }

      [TestMethod]
      public void X13_Y11_Z7()
      {
         CompareCube(
            @"..\..\An8Cube\Cube_X13_Y11_Z7.an8",
            @"..\..\An8Cube\Cube_X13_Y11_Z7_Mesh.an8");
      }

      static void CompareCube(String cubeFile, String meshFile)
      {
         Int32 warnings = 0;

         void Callback(String warning)
         {
            warnings++;
         }

         ANIM8OR cube = LoadAn8File(cubeFile);
         ANIM8OR mesh = LoadAn8File(meshFile);

         mesh expected = mesh.@object[0].mesh[0];
         mesh actual = Utility.An8Cube.Calculate(
            cube.@object[0].cube[0],
            Callback);

         Assert.IsTrue(warnings == 0);

         Assert.AreEqual(
            expected.name.text,
            actual.name.text,
            "Calculated cube's name doesn't match.");

         Assert.AreEqual(
            expected.material.name,
            actual.material.name,
            "Calculated cube's material doesn't match.");

         Assert.AreEqual(
            expected.materiallist.materialname.Length,
            actual.materiallist.materialname.Length,
            "Calculated cube's material list length doesn't match.");

         Assert.AreEqual(
            expected.materiallist.materialname[0].text,
            actual.materiallist.materialname[0].text,
            "Calculated cube's material list doesn't match.");

         Assert.AreEqual(
            expected.points.point.Length,
            actual.points.point.Length,
            "Calculated cube's points length doesn't match.");

         for ( Int32 i = 0; i < expected.points.point.Length; i++ )
         {
            point expectedPoint = expected.points.point[i];
            point actualPoint = actual.points.point[i];

            Assert.AreEqual(
               expectedPoint.x,
               actualPoint.x,
               TOLERANCE,
               $"Calculated cube's point {i}'s X doesn't match");

            Assert.AreEqual(
               expectedPoint.y,
               actualPoint.y,
               TOLERANCE,
               $"Calculated cube's point {i}'s Y doesn't match");

            Assert.AreEqual(
               expectedPoint.z,
               actualPoint.z,
               TOLERANCE,
               $"Calculated cube's point {i}'s Z doesn't match");
         }

         // Note: The normals should be null.
         Assert.AreEqual(
            expected.normals,
            actual.normals,
            "Calculated cube's normals length doesn't match.");

         Assert.AreEqual(
            expected.texcoords.texcoord.Length,
            actual.texcoords.texcoord.Length,
            "Calculated cube's tex coords list doesn't match");

         for ( Int32 i = 0; i < expected.texcoords.texcoord.Length; i++ )
         {
            texcoord expectedTexcoord = expected.texcoords.texcoord[i];
            texcoord actualTexcoord = actual.texcoords.texcoord[i];

            Assert.AreEqual(
               expectedTexcoord.u,
               actualTexcoord.u,
               TOLERANCE,
               $"Calculated cube's tex coord {i}'s U doesn't match");

            Assert.AreEqual(
               expectedTexcoord.v,
               actualTexcoord.v,
               TOLERANCE,
               $"Calculated cube's tex coord {i}'s V doesn't match");
         }

         Assert.AreEqual(
            expected.faces.facedata.Length,
            actual.faces.facedata.Length,
            "Calculated cube's faces list doesn't match");

         for ( Int32 i = 0; i < expected.faces.facedata.Length; i++ )
         {
            facedata expectedFacedata = expected.faces.facedata[i];
            facedata actualFacedata = actual.faces.facedata[i];

            Assert.AreEqual(
               expectedFacedata.numpoints,
               actualFacedata.numpoints,
               $"Calculated cube's face {i}'s num points doesn't match");

            Assert.AreEqual(
               expectedFacedata.flags,
               actualFacedata.flags,
               $"Calculated cube's face {i}'s flags doesn't match");

            Assert.AreEqual(
               expectedFacedata.matno,
               actualFacedata.matno,
               $"Calculated cube's face {i}'s mat no doesn't match");

            // Note: The flat normal no should be -1.
            Assert.AreEqual(
               expectedFacedata.flatnormalno,
               actualFacedata.flatnormalno,
               $"Calculated cube's face {i}'s flat normal no doesn't match");

            Assert.AreEqual(
               expectedFacedata.pointdata.Length,
               actualFacedata.pointdata.Length,
               $"Calculated cube's face {i}'s points list doesn't match");

            for ( Int32 j = 0; j < expectedFacedata.pointdata.Length; j++ )
            {
               pointdata expectedPointdata = expectedFacedata.pointdata[j];
               pointdata actualPointdata = actualFacedata.pointdata[j];

               Assert.AreEqual(
                  expectedPointdata.pointindex,
                  actualPointdata.pointindex,
                  $"Calculated cube's face {i}'s data {j}'s point index doesn't match");

               // Note: The normal index should be 0.
               Assert.AreEqual(
                  expectedPointdata.normalindex,
                  actualPointdata.normalindex,
                  $"Calculated cube's face {i}'s data {j}'s normal index doesn't match");

               Assert.AreEqual(
                  expectedPointdata.texcoordindex,
                  actualPointdata.texcoordindex,
                  $"Calculated cube's face {i}'s data {j}'s tex coord index doesn't match");
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
