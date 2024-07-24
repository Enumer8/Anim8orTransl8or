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
   public class TestAn8FloatKey : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8FloatKey.Calculate(null, null, null, null, 0);
         An8FloatKey.Calculate(new sequence(), null, null, null, 0, null);
      }

      static void CompareFloatKeys(
         String angleFile,
         floatkey[] expectedX,
         floatkey[] expectedY,
         floatkey[] expectedZ)
      {
         Int32 warnings = 0;

         ANIM8OR angle = LoadAn8File(angleFile);

         Int32 totalFrames = (Int32)angle.sequence[0].frames.text;

         for ( Int32 i = 0; i < totalFrames; i++ )
         {
            CompareFloatKeys(
               expectedX[i],
               An8FloatKey.Calculate(
                  angle.sequence[0],
                  angle.figure,
                  angle.figure[0].bone.bone[0].name,
                  "X",
                  i,
                  (String s) => warnings++));

            CompareFloatKeys(
               expectedY[i],
               An8FloatKey.Calculate(
                  angle.sequence[0],
                  angle.figure,
                  angle.figure[0].bone.bone[0].name,
                  "Y",
                  i,
                  (String s) => warnings++));

            CompareFloatKeys(
               expectedZ[i],
               An8FloatKey.Calculate(
                  angle.sequence[0],
                  angle.figure,
                  angle.figure[0].bone.bone[0].name,
                  "Z",
                  i,
                  (String s) => warnings++));
         }

         Assert.IsTrue(warnings == 0);
      }

      static void CompareFloatKeys(floatkey expected, floatkey actual)
      {
         Assert.AreEqual(
            expected.frame,
            actual.frame,
            TOLERANCE);

         Assert.AreEqual(
            expected.value,
            actual.value,
            TOLERANCE);

         // TODO: What do these values mean and how are they calculated?
         //Assert.AreEqual(
         //   expected.value2,
         //   actual.value2,
         //   TOLERANCE);

         //Assert.AreEqual(
         //   expected.value3,
         //   actual.value3,
         //   TOLERANCE);

         Assert.AreEqual(
            expected.modifier,
            actual.modifier);
      }

      // The tolerance is a little lower since Anim8or v1.00 rounds more
      protected new const Double TOLERANCE = 0.01;
   }
}
