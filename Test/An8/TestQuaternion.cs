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

using Anim8orTransl8or.An8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestQuaternion
   {
      [TestMethod]
      public void AxisAngleConstructor()
      {
         quaternion q = new quaternion(new point(0.707, 0, 0.707), 1);
         Assert.AreEqual(0.3389538, q.x, TOLERANCE);
         Assert.AreEqual(0, q.y, TOLERANCE);
         Assert.AreEqual(0.3389538, q.z, TOLERANCE);
         Assert.AreEqual(0.8775825, q.w, TOLERANCE);
      }

      [TestMethod]
      public new void ToString()
      {
         quaternion q = new quaternion(12.345, -9.876, 3.456, -4.567);
         Assert.AreEqual("<12.345, -9.876, 3.456, -4.567>", q.ToString());
      }

      [TestMethod]
      public void GetEnumerator()
      {
         quaternion q = new quaternion(0, 1, 2, 3);
         Double[] l = q.GetEnumerator().ToArray();
         Assert.AreEqual(4, l.Length);
         Assert.AreEqual(0, l[0]);
         Assert.AreEqual(1, l[1]);
         Assert.AreEqual(2, l[2]);
         Assert.AreEqual(3, l[3]);
      }

      [TestMethod]
      public void Length()
      {
         quaternion a = new quaternion(3, 4, 5, 6);
         Assert.AreEqual(9.2736185, a.GetLength(), TOLERANCE);

         quaternion b = new quaternion(-5, 12, 13, 14);
         Assert.AreEqual(23.1084400, b.GetLength(), TOLERANCE);
      }

      [TestMethod]
      public void Normalize()
      {
         quaternion q = new quaternion(
            1.234567,
            2.345678,
            3.456789,
            4.56789).Normalize();

         Assert.AreEqual(0.1955903, q.x, TOLERANCE);
         Assert.AreEqual(0.3716218, q.y, TOLERANCE);
         Assert.AreEqual(0.5476533, q.z, TOLERANCE);
         Assert.AreEqual(0.7236832, q.w, TOLERANCE);
      }

      [TestMethod]
      public void Conjugate()
      {
         quaternion q = new quaternion(0, 1, 2, 3).Conjugate();
         Assert.AreEqual(0, q.x);
         Assert.AreEqual(-1, q.y);
         Assert.AreEqual(-2, q.z);
         Assert.AreEqual(3, q.w);
      }

      [TestMethod]
      public void RotateQuaternion()
      {
         quaternion a = new quaternion(0.271, -0.271, 0.653, 0.653);
         quaternion b = new quaternion(0.271, -0.653, -0.271, 0.653);
         quaternion q = a.Rotate(b);

         Assert.AreEqual(0.853776, q.x, TOLERANCE);
         Assert.AreEqual(-0.352968, q.y, TOLERANCE);
         Assert.AreEqual(0.145924, q.z, TOLERANCE);
         Assert.AreEqual(0.352968, q.w, TOLERANCE);
      }

      [TestMethod]
      public void RotatePoint()
      {
         quaternion a = new quaternion(0.271, -0.271, 0.653, 0.653);
         point b = new point(-5, 12, 13);
         point p = a.Rotate(b);

         Assert.AreEqual(-11.9964, p.x, TOLERANCE);
         Assert.AreEqual(-12.731756, p.y, TOLERANCE);
         Assert.AreEqual(5.637908, p.z, TOLERANCE);
      }

      [TestMethod]
      public void Indexer()
      {
         quaternion q = new quaternion(1, 2, 3, 4);
         q[0] = 5;
         q[1] = 6;
         q[2] = 7;
         q[3] = 8;

         try
         {
            q[-1] = 9;

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         try
         {
            q[4] = 10;

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         Assert.AreEqual(5, q.x);
         Assert.AreEqual(6, q.y);
         Assert.AreEqual(7, q.z);
         Assert.AreEqual(8, q.w);
         Assert.AreEqual(5, q[0]);
         Assert.AreEqual(6, q[1]);
         Assert.AreEqual(7, q[2]);
         Assert.AreEqual(8, q[3]);

         try
         {
            Double lowIndex = q[-1];

            // Should have been an exception
            Assert.Fail();
         }
         catch ( IndexOutOfRangeException )
         {
            // This is the expected exception
         }

         try
         {
            Double highIndex = q[4];

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
         quaternion a = new quaternion(3, 4, 5, 6);
         quaternion b = new quaternion(-5, 12, 13, -14);
         quaternion q = a + b;

         Assert.AreEqual(-2, q.x);
         Assert.AreEqual(16, q.y);
         Assert.AreEqual(18, q.z);
         Assert.AreEqual(-8, q.w);
      }

      [TestMethod]
      public void Subtract()
      {
         quaternion a = new quaternion(3, 4, 5, 6);
         quaternion b = new quaternion(-5, 12, 13, -14);
         quaternion q = a - b;

         Assert.AreEqual(8, q.x);
         Assert.AreEqual(-8, q.y);
         Assert.AreEqual(-8, q.z);
         Assert.AreEqual(20, q.w);
      }

      [TestMethod]
      public void Multiply()
      {
         quaternion a = new quaternion(1, 2, 3, 4);
         Double b = 4;
         quaternion q = a * b;

         Assert.AreEqual(4, q.x);
         Assert.AreEqual(8, q.y);
         Assert.AreEqual(12, q.z);
         Assert.AreEqual(16, q.w);
      }

      [TestMethod]
      public void Divide()
      {
         quaternion a = new quaternion(10, 15, 20, 25);
         Double b = 5;
         quaternion q = a / b;

         Assert.AreEqual(2, q.x);
         Assert.AreEqual(3, q.y);
         Assert.AreEqual(4, q.z);
         Assert.AreEqual(5, q.w);
      }

      const Double TOLERANCE = 0.000001;
   }
}
