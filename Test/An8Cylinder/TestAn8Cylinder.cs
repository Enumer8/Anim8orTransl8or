// Copyright © 2024 Contingent Games.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestAn8Cylinder : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Cylinder.Calculate(null);
         An8Cylinder.Calculate(new cylinder(), null);

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

         CompareCylinders(
            @"..\..\..\An8Cylinder\Cylinder_Edge_Case.an8",
            @"..\..\..\An8Cylinder\Cylinder_Edge_Case_Mesh.an8");
      }

      [TestMethod]
      public void Lon3_Lat1()
      {
         CompareCylinders(
            @"..\..\..\An8Cylinder\Cylinder_Lon3_Lat1.an8",
            @"..\..\..\An8Cylinder\Cylinder_Lon3_Lat1_Mesh.an8");
      }

      [TestMethod]
      public void Lon4_Lat5()
      {
         CompareCylinders(
            @"..\..\..\An8Cylinder\Cylinder_Lon4_Lat5.an8",
            @"..\..\..\An8Cylinder\Cylinder_Lon4_Lat5_Mesh.an8");
      }

      [TestMethod]
      public void Lon6_Lat6()
      {
         CompareCylinders(
            @"..\..\..\An8Cylinder\Cylinder_Lon6_Lat6.an8",
            @"..\..\..\An8Cylinder\Cylinder_Lon6_Lat6_Mesh.an8");
      }

      [TestMethod]
      public void Lon11_Lat7()
      {
         CompareCylinders(
            @"..\..\..\An8Cylinder\Cylinder_Lon11_Lat7.an8",
            @"..\..\..\An8Cylinder\Cylinder_Lon11_Lat7_Mesh.an8");
      }

      [TestMethod]
      public void Lon4_Lat2()
      {
         CompareCylinders(
            @"..\..\..\An8Cylinder\Cone_Lon4_Lat2.an8",
            @"..\..\..\An8Cylinder\Cone_Lon4_Lat2_Mesh.an8");
      }

      [TestMethod]
      public void Lon5_Lat2()
      {
         CompareCylinders(
            @"..\..\..\An8Cylinder\Cone_Lon5_Lat2.an8",
            @"..\..\..\An8Cylinder\Cone_Lon5_Lat2_Mesh.an8");
      }

      static void CompareCylinders(String cylinderFile, String meshFile)
      {
         Int32 warnings = 0;

         ANIM8OR cylinder = LoadAn8File(cylinderFile);
         ANIM8OR mesh = LoadAn8File(meshFile);

         mesh expected = mesh.@object[0].mesh[0];
         mesh actual = An8Cylinder.Calculate(
            cylinder.@object[0].cylinder[0],
            (String s) => warnings++);

         Assert.IsTrue(warnings == 0);

         CompareMeshes(expected, actual);
      }
   }
}
