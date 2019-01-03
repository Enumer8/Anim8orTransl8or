﻿using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestAn8Sphere : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Sphere.Calculate(null);
         An8Sphere.Calculate(new sphere());

         // Test a sphere with a couple invalid settings
         sphere sphere = new sphere();
         sphere.name = new @string() { text = "EdgeCase" };
         sphere.diameter = new @float() { text = -1 }; // invalid
         sphere.longlat = new longlat();
         sphere.longlat.longitude = 2;
         sphere.longlat.latitude = -2; // invalid

         Int32 warnings = 0;
         An8Sphere.Calculate(sphere, (String s) => warnings++);
         Assert.IsTrue(warnings > 0);

         CompareSpheres(
            @"..\..\An8Sphere\Sphere_Edge_Case.an8",
            @"..\..\An8Sphere\Sphere_Edge_Case_Mesh.an8");

         CompareSpheres(
            @"..\..\An8Sphere\Sphere_Edge_Case2.an8",
            @"..\..\An8Sphere\Sphere_Edge_Case2_Mesh.an8");

         CompareSpheres(
            @"..\..\An8Sphere\Sphere_Edge_Case3.an8",
            @"..\..\An8Sphere\Sphere_Edge_Case3_Mesh.an8");

         CompareSpheres(
            @"..\..\An8Sphere\Geo_Edge_Case.an8",
            @"..\..\An8Sphere\Geo_Edge_Case_Mesh.an8");
      }

      [TestMethod]
      public void Lon3_Lat2()
      {
         CompareSpheres(
            @"..\..\An8Sphere\Sphere_Lon3_Lat2.an8",
            @"..\..\An8Sphere\Sphere_Lon3_Lat2_Mesh.an8");
      }

      [TestMethod]
      public void Lon4_Lat6()
      {
         CompareSpheres(
            @"..\..\An8Sphere\Sphere_Lon4_Lat6.an8",
            @"..\..\An8Sphere\Sphere_Lon4_Lat6_Mesh.an8");
      }

      [TestMethod]
      public void Lon11_Lat7()
      {
         CompareSpheres(
            @"..\..\An8Sphere\Sphere_Lon11_Lat7.an8",
            @"..\..\An8Sphere\Sphere_Lon11_Lat7_Mesh.an8");
      }

      [TestMethod]
      public void Geo1()
      {
         CompareSpheres(
            @"..\..\An8Sphere\Geo_1.an8",
            @"..\..\An8Sphere\Geo_1_Mesh.an8");
      }

      [TestMethod]
      public void Geo2()
      {
         CompareSpheres(
            @"..\..\An8Sphere\Geo_2.an8",
            @"..\..\An8Sphere\Geo_2_Mesh.an8");
      }

      [TestMethod]
      public void Geo6()
      {
         CompareSpheres(
            @"..\..\An8Sphere\Geo_6.an8",
            @"..\..\An8Sphere\Geo_6_Mesh.an8");
      }

      static void CompareSpheres(String sphereFile, String meshFile)
      {
         Int32 warnings = 0;

         ANIM8OR sphere = LoadAn8File(sphereFile);
         ANIM8OR mesh = LoadAn8File(meshFile);

         mesh expected = mesh.@object[0].mesh[0];
         mesh actual = An8Sphere.Calculate(
            sphere.@object[0].sphere[0],
            (String s) => warnings++);

         Assert.IsTrue(warnings == 0);

         CompareMeshes(expected, actual);
      }
   }
}
