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

using Anim8orTransl8or.An8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestPoint
   {
      [TestMethod]
      public new void ToString()
      {
         point p = new point(12.345, -9.876, 3.456);
         Assert.AreEqual("<12.345, -9.876, 3.456>", p.ToString());
      }

      [TestMethod]
      public void GetEnumerator()
      {
         point p = new point(0, 1, 2);
         Double[] l = p.GetEnumerator().ToArray();
         Assert.AreEqual(3, l.Length);
         Assert.AreEqual(0, l[0]);
         Assert.AreEqual(1, l[1]);
         Assert.AreEqual(2, l[2]);
      }

      [TestMethod]
      public void Length()
      {
         point a = new point(3, 4, 5);
         Assert.AreEqual(7.0710678, a.GetLength(), TOLERANCE);

         point b = new point(-5, 12, 13);
         Assert.AreEqual(18.3847763, b.GetLength(), TOLERANCE);
      }

      [TestMethod]
      public void Normalize()
      {
         point p = new point(1.234567, 2.345678, 3.456789).Normalize();
         Assert.AreEqual(0.28341, p.x, TOLERANCE);
         Assert.AreEqual(0.5384791, p.y, TOLERANCE);
         Assert.AreEqual(0.7935483, p.z, TOLERANCE);
      }

      [TestMethod]
      public void Dot()
      {
         point a = new point(3, 4, 5);
         point b = new point(-5, 12, 13);

         Assert.AreEqual(98, a.Dot(b));
      }

      [TestMethod]
      public void Cross()
      {
         point a = new point(3, 4, 5);
         point b = new point(-5, 12, 13);
         point p = a.Cross(b);

         Assert.AreEqual(-8, p.x);
         Assert.AreEqual(-64, p.y);
         Assert.AreEqual(56, p.z);
      }

      [TestMethod]
      public void Indexer()
      {
         point p = new point(1, 2, 3);
         p[0] = 4;
         p[1] = 5;
         p[2] = 6;

         try
         {
            p[-1] = 7;

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         try
         {
            p[3] = 8;

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         Assert.AreEqual(4, p.x);
         Assert.AreEqual(5, p.y);
         Assert.AreEqual(6, p.z);
         Assert.AreEqual(4, p[0]);
         Assert.AreEqual(5, p[1]);
         Assert.AreEqual(6, p[2]);

         try
         {
            Double lowIndex = p[-1];

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         try
         {
            Double highIndex = p[3];

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
         point a = new point(3, 4, 5);
         point b = new point(-5, 12, 13);
         point p = a + b;

         Assert.AreEqual(-2, p.x);
         Assert.AreEqual(16, p.y);
         Assert.AreEqual(18, p.z);
      }

      [TestMethod]
      public void Subtract()
      {
         point a = new point(3, 4, 5);
         point b = new point(-5, 12, 13);
         point p = a - b;

         Assert.AreEqual(8, p.x);
         Assert.AreEqual(-8, p.y);
         Assert.AreEqual(-8, p.z);
      }

      [TestMethod]
      public void Multiply()
      {
         point a = new point(1, 2, 3);
         Double b = 4;
         point p = a * b;

         Assert.AreEqual(4, p.x);
         Assert.AreEqual(8, p.y);
         Assert.AreEqual(12, p.z);
      }

      [TestMethod]
      public void Divide()
      {
         point a = new point(10, 15, 20);
         Double b = 5;
         point p = a / b;

         Assert.AreEqual(2, p.x);
         Assert.AreEqual(3, p.y);
         Assert.AreEqual(4, p.z);
      }

      const Double TOLERANCE = 0.000001;
   }
}
