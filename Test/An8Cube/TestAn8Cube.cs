// Copyright © 2023 Contingent Games.
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
   public class TestAn8Cube : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Cube.Calculate(null);
         An8Cube.Calculate(new cube(), null);

         // Test a cube with a couple invalid settings
         cube cube = new cube();
         cube.name = new @string() { text = "EdgeCase" };
         cube.scale = new scale();
         cube.scale.x = -1; // invalid
         cube.scale.y = 2;
         cube.scale.z = 3;
         cube.divisions = new divisions();
         cube.divisions.x = 2;
         cube.divisions.y = 1;
         cube.divisions.z = 0; // invalid

         Int32 warnings = 0;
         An8Cube.Calculate(cube, (String s) => warnings++);
         Assert.IsTrue(warnings > 0);

         CompareCubes(
            @"..\..\..\An8Cube\Cube_Edge_Case.an8",
            @"..\..\..\An8Cube\Cube_Edge_Case_Mesh.an8");
      }

      [TestMethod]
      public void X1_Y1_Z1()
      {
         CompareCubes(
            @"..\..\..\An8Cube\Cube_X1_Y1_Z1.an8",
            @"..\..\..\An8Cube\Cube_X1_Y1_Z1_Mesh.an8");
      }

      [TestMethod]
      public void X2_Y3_Z5()
      {
         CompareCubes(
            @"..\..\..\An8Cube\Cube_X2_Y3_Z5.an8",
            @"..\..\..\An8Cube\Cube_X2_Y3_Z5_Mesh.an8");
      }

      [TestMethod]
      public void X13_Y11_Z7()
      {
         CompareCubes(
            @"..\..\..\An8Cube\Cube_X13_Y11_Z7.an8",
            @"..\..\..\An8Cube\Cube_X13_Y11_Z7_Mesh.an8");
      }

      static void CompareCubes(String cubeFile, String meshFile)
      {
         Int32 warnings = 0;

         ANIM8OR cube = LoadAn8File(cubeFile);
         ANIM8OR mesh = LoadAn8File(meshFile);

         mesh expected = mesh.@object[0].mesh[0];
         mesh actual = An8Cube.Calculate(
            cube.@object[0].cube[0],
            (String s) => warnings++);

         Assert.IsTrue(warnings == 0);

         CompareMeshes(expected, actual);
      }
   }
}
