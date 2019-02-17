// Copyright © 2018 Contingent Games.
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

      static void CompareNormals(String normalFile)
      {
         Int32 warnings = 0;

         ANIM8OR normal = LoadAn8File(normalFile);

         mesh expected = normal.@object[0].mesh[0];

         mesh actual = An8Normals.Calculate(
            expected,
            (String s) => warnings++);

         Assert.IsTrue(warnings == 0);

         CompareMeshes(expected, actual);
      }
   }
}
