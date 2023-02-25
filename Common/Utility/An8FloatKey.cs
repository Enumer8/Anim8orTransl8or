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
using System;

namespace Anim8orTransl8or.Utility
{
   static class An8FloatKey
   {
      /// <summary>
      /// This will produce the same floatkey as if the user had clicked
      /// Edit->Bones->Key Selected Bones in Anim8or v1.00.
      /// Note: The floatkey is not inserted into the list.
      /// </summary>
      /// <param name="s">the sequence</param>
      /// <param name="fs">the possible figures</param>
      /// <param name="bone">the bone's name</param>
      /// <param name="axis">the axis ("X", "Y", or "Z")</param>
      /// <param name="time">the frame</param>
      /// <param name="callback">the callback for warnings</param>
      /// <returns>the calculated floatkey</returns>
      internal static floatkey Calculate(
         sequence s,
         figure[] fs,
         String bone,
         String axis,
         Double time,
         Action<String> callback = null)
      {
         floatkey floatkey = new floatkey();
         floatkey.frame = (Int64)time;
         floatkey.modifier = "S";

         floatkey start = null;
         floatkey end = null;

         const Int64 minFrame = 0;
         Int64 maxFrame = s?.frames?.text ?? 0;

         foreach ( jointangle j in s?.jointangle ?? new jointangle[0] )
         {
            if ( j?.bone == bone && j?.axis == axis )
            {
               foreach ( floatkey f in j?.track?.floatkey ?? new floatkey[0] )
               {
                  if ( f?.frame <= time &&
                     f.frame >= minFrame &&
                     f.frame < maxFrame &&
                     (start == null || start.frame < f.frame) )
                  {
                     start = f;
                  }

                  if ( f?.frame >= time &&
                     f.frame >= minFrame &&
                     f.frame < maxFrame &&
                     (end == null || end.frame > f.frame) )
                  {
                     end = f;
                  }
               }
            }
         }

         if ( start == null )
         {
            if ( end == null )
            {
               // There are no key frames for this bone/axis
               floatkey.value = 0;
            }
            else
            {
               // There is only one key frame for this bone/axis (after time)
               floatkey.value = end.value;
            }
         }
         else if ( end == null || end == start )
         {
            // There is only one key frame for this bone/axis (before time)
            floatkey.value = start.value;
         }
         else
         {
            Double diff = start.value - end.value;

            if ( diff > 360 )
            {
               // If the end is more than 360 degrees less than the start,
               // e.g. end = -350 and start = 350, then it is shorter to
               // rotate the 20 degrees by adding and wrapping around;
               floatkey.value = InterpolateLinear(
                  start.value,
                  720 - end.value,
                  (time - start.frame) / (end.frame - start.frame));

               if ( floatkey.value > 360 )
               {
                  floatkey.value -= 720;
               }
            }
            else if ( diff < -360 )
            {
               // If the end is more than 360 degrees larger than the start,
               // e.g. end = 350 and start = -350, then it is shorter to
               // rotate the 20 degrees by subtracting and wrapping around.
               floatkey.value = InterpolateLinear(
                  start.value,
                  end.value - 720,
                  (time - start.frame) / (end.frame - start.frame));

               if ( floatkey.value < -360 )
               {
                  floatkey.value += 720;
               }
            }
            else
            {
               // Interpolate normally
               floatkey.value = InterpolateLinear(
                  start.value,
                  end.value,
                  (time - start.frame) / (end.frame - start.frame));
            }

            // TODO: Figure out how to interpolate like Anim8or.
            #region
            //// Look up the bone's degrees of freedom
            //dof dof = null;

            //Boolean FindBone(bone1 b)
            //{
            //   if ( b?.name == bone )
            //   {
            //      foreach ( dof d in b?.dof ?? new dof[0] )
            //      {
            //         if ( d?.axis == axis )
            //         {
            //            dof = d;
            //            return true;
            //         }
            //      }

            //      return false;
            //   }
            //   else
            //   {
            //      foreach ( bone1 b2 in b?.bone ?? new bone1[0] )
            //      {
            //         if ( FindBone(b2) )
            //         {
            //            return true;
            //         }
            //      }

            //      return false;
            //   }
            //}

            //foreach ( figure f in fs ?? new figure[0] )
            //{
            //   if ( f?.name == s?.figure?.text )
            //   {
            //      FindBone(f?.bone);
            //      break;
            //   }
            //}

            //switch ( start.modifier )
            //{
            //// "C" stands for Corner. It uses linear interpolates.
            //case "C":
            //{
            //   Double difference = start.value - end.value;

            //   if ( difference > 360 )
            //   {
            //      // If the end is more than 360 degrees less than the start,
            //      // e.g. end = -350 and start = 350, then it is shorter to
            //      // rotate the 20 degrees by adding and wrapping around;
            //      floatkey.value = InterpolateLinear(
            //         start.value,
            //         720 - end.value,
            //         (time - start.frame) / (end.frame - start.frame));

            //      if ( floatkey.value > 360 )
            //      {
            //         floatkey.value -= 720;
            //      }
            //   }
            //   else if ( difference < -360 )
            //   {
            //      // If the end is more than 360 degrees larger than the start,
            //      // e.g. end = 350 and start = -350, then it is shorter to
            //      // rotate the 20 degrees by subtracting and wrapping around.
            //      floatkey.value = InterpolateLinear(
            //         start.value,
            //         720 - difference,
            //         (time - start.frame) / (end.frame - start.frame));

            //      if ( floatkey.value < -360 )
            //      {
            //         floatkey.value += 720;
            //      }
            //   }
            //   else
            //   {
            //      // Interpolate normally
            //      floatkey.value = InterpolateLinear(
            //         start.value,
            //         end.value,
            //         (time - start.frame) / (end.frame - start.frame));
            //   }

            //   floatkey.modifier = "C";
            //   break;
            //}
            //// "T" stands for Threshold/Step. TODO: What kind of interpolation?
            //case "T":
            //{
            //   break;
            //}
            //// "S" stands for Smooth. It uses hermite interpolation.
            //case "S":
            //default:
            //{
            //   floatkey.value = InterpolateHermite(
            //      start.value,
            //      start.value,
            //      end.value,
            //      end.value,
            //      (time - start.frame) / (end.frame - start.frame));

            //   floatkey.modifier = "S";
            //   break;
            //}
            //}
            #endregion
         }

         return floatkey;
      }

      internal static Double InterpolateHermite(
         Double p0,
         Double p1,
         Double p2,
         Double p3,
         Double x)
      {
         Double a = p3 * 0.5 - p2 * 1.5 + p1 * 1.5 - p0 * 0.5;
         Double b = -p3 * 0.5 + p2 * 2 - p1 * 2.5 + p0;
         Double c = p2 * 0.5 - p0 * 0.5;
         Double d = p1;

         return a * x * x * x + b * x * x + c * x + d;
      }

      internal static Double InterpolateLinear(Double p0, Double p1, Double x)
      {
         Double a = p1 - p0;
         Double b = p0;

         return a * x + b;
      }

      internal static Double Difference(Double a, Double b)
      {
         Double diff = Math.Abs(a - b);

         if ( diff <= 360 )
         {
            return diff;
         }
         else
         {
            return 720 - diff;
         }
      }
   }
}
