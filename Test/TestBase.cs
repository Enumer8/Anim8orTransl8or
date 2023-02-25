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
using Anim8orTransl8or.An8.V100;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Anim8orTransl8or.Test
{
   public class TestBase
   {
      protected static void CompareMeshes(mesh expected, mesh actual)
      {
         Assert.AreEqual(
            expected?.name?.text,
            actual?.name?.text,
            "Mesh's name doesn't match.");

         Assert.AreEqual(
            expected?.material?.name,
            actual?.material?.name,
            "Mesh's material doesn't match.");

         Assert.AreEqual(
            expected?.materiallist?.materialname?.Length,
            actual?.materiallist?.materialname?.Length,
            "Mesh's material list length doesn't match.");

         for (
            Int32 i = 0;
            i < expected?.materiallist?.materialname.Length;
            i++ )
         {
            Assert.AreEqual(
               expected.materiallist.materialname[i]?.text,
               actual.materiallist.materialname[i]?.text,
               "Mesh's material list {i}'s name doesn't match.");
         }

         Assert.AreEqual(
            expected?.points?.point?.Length,
            actual?.points?.point?.Length,
            "Mesh's points length doesn't match.");

         for ( Int32 i = 0; i < expected?.points?.point?.Length; i++ )
         {
            point expectedPoint = expected.points.point[i];
            point actualPoint = actual.points.point[i];

            Assert.AreEqual(
               expectedPoint.x,
               actualPoint.x,
               TOLERANCE,
               $"Mesh's point {i}'s X doesn't match");

            Assert.AreEqual(
               expectedPoint.y,
               actualPoint.y,
               TOLERANCE,
               $"Mesh's point {i}'s Y doesn't match");

            Assert.AreEqual(
               expectedPoint.z,
               actualPoint.z,
               TOLERANCE,
               $"Mesh's point {i}'s Z doesn't match");
         }

         Assert.AreEqual(
            expected?.normals?.point?.Length,
            actual?.normals?.point?.Length,
            "Mesh's normals length doesn't match.");

         for ( Int32 i = 0; i < expected?.normals?.point?.Length; i++ )
         {
            point expectedNormal = expected.normals.point[i];
            point actualNormal = actual.normals.point[i];

            Assert.AreEqual(
               expectedNormal.x,
               actualNormal.x,
               TOLERANCE,
               $"Mesh's normal {i}'s X doesn't match");

            Assert.AreEqual(
               expectedNormal.y,
               actualNormal.y,
               TOLERANCE,
               $"Mesh's normal {i}'s Y doesn't match");

            Assert.AreEqual(
               expectedNormal.z,
               actualNormal.z,
               TOLERANCE,
               $"Mesh's point {i}'s Z doesn't match");
         }

         Assert.AreEqual(
            expected?.texcoords?.texcoord?.Length,
            actual?.texcoords?.texcoord?.Length,
            "Mesh's tex coords length doesn't match");

         for ( Int32 i = 0; i < expected?.texcoords?.texcoord?.Length; i++ )
         {
            texcoord expectedTexcoord = expected.texcoords.texcoord[i];
            texcoord actualTexcoord = actual.texcoords.texcoord[i];

            Assert.AreEqual(
               expectedTexcoord.u,
               actualTexcoord.u,
               TOLERANCE,
               $"Mesh's tex coord {i}'s U doesn't match");

            Assert.AreEqual(
               expectedTexcoord.v,
               actualTexcoord.v,
               TOLERANCE,
               $"Mesh's tex coord {i}'s V doesn't match");
         }

         Assert.AreEqual(
            expected?.faces?.facedata?.Length,
            actual?.faces?.facedata?.Length,
            "Mesh's faces length doesn't match");

         for ( Int32 i = 0; i < expected?.faces?.facedata?.Length; i++ )
         {
            facedata expectedFacedata = expected.faces.facedata[i];
            facedata actualFacedata = actual.faces.facedata[i];

            Assert.AreEqual(
               expectedFacedata?.numpoints,
               actualFacedata?.numpoints,
               $"Mesh's face {i}'s num points doesn't match");

            Assert.AreEqual(
               expectedFacedata?.flags,
               actualFacedata?.flags,
               $"Mesh's face {i}'s flags doesn't match");

            Assert.AreEqual(
               expectedFacedata?.matno,
               actualFacedata?.matno,
               $"Mesh's face {i}'s mat no doesn't match");

            Assert.AreEqual(
               expectedFacedata?.flatnormalno,
               actualFacedata?.flatnormalno,
               $"Mesh's face {i}'s flat normal no doesn't match");

            Assert.AreEqual(
               expectedFacedata?.pointdata?.Length,
               actualFacedata?.pointdata?.Length,
               $"Mesh's face {i}'s data length doesn't match");

            for ( Int32 j = 0; j < expectedFacedata?.pointdata?.Length; j++ )
            {
               pointdata expectedPointdata = expectedFacedata.pointdata[j];
               pointdata actualPointdata = actualFacedata.pointdata[j];

               Assert.AreEqual(
                  expectedPointdata?.pointindex,
                  actualPointdata?.pointindex,
                  $"Mesh's face {i}'s data {j}'s point index doesn't match");

               Assert.AreEqual(
                  expectedPointdata?.normalindex,
                  actualPointdata?.normalindex,
                  $"Mesh's face {i}'s data {j}'s normal index doesn't match");

               Assert.AreEqual(
                  expectedPointdata?.texcoordindex,
                  actualPointdata?.texcoordindex,
                  $"Mesh's face {i}'s data {j}'s tex coord index doesn't match");
            }
         }
      }

      protected static ANIM8OR LoadAn8File(String file)
      {
         ANIM8OR an8;

         using ( Stream stream = File.Open(file, FileMode.Open) )
         {
            Serializer deserializer = new Serializer(typeof(ANIM8OR));
            an8 = (ANIM8OR)deserializer.Deserialize(stream);
         }

         return an8;
      }

      protected const Double TOLERANCE = 0.001;
   }
}
