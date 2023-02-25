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

using Anim8orTransl8or.An8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestTexCoord
   {
      [TestMethod]
      public new void ToString()
      {
         texcoord t = new texcoord(12.345, -9.876);
         Assert.AreEqual("<12.345, -9.876>", t.ToString());
      }

      [TestMethod]
      public void GetEnumerator()
      {
         texcoord t = new texcoord(0, 1);
         Double[] l = t.GetEnumerator().ToArray();
         Assert.AreEqual(2, l.Length);
         Assert.AreEqual(0, l[0]);
         Assert.AreEqual(1, l[1]);
      }

      [TestMethod]
      public void Length()
      {
         texcoord a = new texcoord(3, 4);
         Assert.AreEqual(5, a.GetLength(), TOLERANCE);

         texcoord b = new texcoord(-5, 12);
         Assert.AreEqual(13, b.GetLength(), TOLERANCE);
      }

      [TestMethod]
      public void Normalize()
      {
         texcoord t = new texcoord(1.234567, 2.345678).Normalize();
         Assert.AreEqual(0.4657463, t.u, TOLERANCE);
         Assert.AreEqual(0.8849183, t.v, TOLERANCE);
      }

      [TestMethod]
      public void Indexer()
      {
         texcoord t = new texcoord(1, 2);
         t[0] = 3;
         t[1] = 4;

         try
         {
            t[-1] = 5;

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         try
         {
            t[2] = 6;

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         Assert.AreEqual(3, t.u);
         Assert.AreEqual(4, t.v);
         Assert.AreEqual(3, t[0]);
         Assert.AreEqual(4, t[1]);

         try
         {
            Double lowIndex = t[-1];

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         try
         {
            Double highIndex = t[2];

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }
      }

      [TestMethod]
      public void Add()
      {
         texcoord a = new texcoord(3, 4);
         texcoord b = new texcoord(-5, 12);
         texcoord t = a + b;

         Assert.AreEqual(-2, t.u);
         Assert.AreEqual(16, t.v);
      }

      [TestMethod]
      public void Subtract()
      {
         texcoord a = new texcoord(3, 4);
         texcoord b = new texcoord(-5, 12);
         texcoord t = a - b;

         Assert.AreEqual(8, t.u);
         Assert.AreEqual(-8, t.v);
      }

      [TestMethod]
      public void Multiply()
      {
         texcoord a = new texcoord(1, 2);
         Double b = 3;
         texcoord t = a * b;

         Assert.AreEqual(3, t.u);
         Assert.AreEqual(6, t.v);
      }

      [TestMethod]
      public void Divide()
      {
         texcoord a = new texcoord(10, 15);
         Double b = 5;
         texcoord t = a / b;

         Assert.AreEqual(2, t.u);
         Assert.AreEqual(3, t.v);
      }

      const Double TOLERANCE = 0.000001;
   }
}
