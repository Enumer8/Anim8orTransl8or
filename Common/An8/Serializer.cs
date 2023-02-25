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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Anim8orTransl8or.An8
{
   public class Serializer
   {
      Type mType;
      Action<String> mCallback;

      public Serializer(Type t, Action<String> callback = null)
      {
         mType = t;
         mCallback = callback;
      }

      public Object Deserialize(Stream s)
      {
         Object o = Activator.CreateInstance(mType);
         FieldInfo[] fis = mType.GetFields();

         using ( StreamReaderEx sr = new StreamReaderEx(s) )
         {
            while ( !sr.EndOfStream )
            {
               String chunkName = ParseIdentifier(sr);
               ParseWhiteSpace(sr);

               Boolean known = false;

               foreach ( FieldInfo fi in fis )
               {
                  if ( fi.Name == chunkName )
                  {
                     if ( fi.FieldType.IsArray )
                     {
                        Type type2 = fi.FieldType.GetElementType();
                        Object o2 = ParseChunk(sr, type2);
                        ParseWhiteSpace(sr);

                        Array newArray;

                        if ( fi.GetValue(o) is Array oldArray )
                        {
                           // Add to the array
                           newArray = Activator.CreateInstance(
                              fi.FieldType,
                              oldArray.Length + 1) as Array;

                           Array.Copy(oldArray, newArray, oldArray.Length);
                        }
                        else
                        {
                           // Create a new array
                           newArray = Activator.CreateInstance(
                              fi.FieldType,
                              1) as Array;
                        }

                        newArray.SetValue(o2, newArray.Length - 1);
                        fi.SetValue(o, newArray);
                     }
                     else
                     {
                        Type type2 = fi.FieldType;
                        Object o2 = ParseChunk(sr, type2);
                        ParseWhiteSpace(sr);

                        fi.SetValue(o, o2);
                     }

                     known = true;
                     break;
                  }
               }

               if ( !known )
               {
                  mCallback?.Invoke($"The \"{chunkName}\" chunk is not recognized.");

                  String unknownChunk = ParseUnknownChunk(sr);
                  ParseWhiteSpace(sr);
                  continue;
               }
            }
         }

         return o;
      }

      public void Serialize(Stream s, Object o)
      {
         // TODO: Implement
         throw new NotImplementedException();
      }

      #region Deserialize
      delegate Boolean CharCondition(Char c);

      static Char AssertChar(StreamReaderEx sr, CharCondition condition)
      {
         // Note: Don't read yet just in case the condition wants to 'peek'.
         Char c = (Char)sr.Peek();

         if ( condition(c) )
         {
            sr.Read();
            return c;
         }
         else
         {
            throw new InvalidOperationException(
               $"({sr.Line}, {sr.Column}): Encountered unexpected character: '{c}'");
         }
      }

      static Boolean IsWhiteSpace(StreamReaderEx sr)
      {
         return Char.IsWhiteSpace((Char)sr.Peek());
      }

      /// <summary>
      /// Reads all continguous whitespace.
      /// Note: For simplicity, slash star comments are skipped as well.
      /// </summary>
      static void ParseWhiteSpace(StreamReaderEx sr)
      {
         while ( IsSlashStarCommentStart(sr) || IsWhiteSpace(sr) )
         {
            if ( IsSlashStarCommentStart(sr) )
            {
               ParseSlashStarComment(sr);
            }
            else
            {
               sr.Read();
            }
         }
      }

      static Boolean IsSlashStarCommentStart(StreamReaderEx sr)
      {
         return sr.Peek() == '/';
      }

      /// <summary>
      /// Reads a C-style comment (e.g. /* and */).
      /// </summary>
      static String ParseSlashStarComment(StreamReaderEx sr)
      {
         StringBuilder sb = new StringBuilder();
         AssertChar(sr, (Char c) => IsSlashStarCommentStart(sr));
         AssertChar(sr, (Char c) => c == '*');

         Boolean endOfComment = false;
         Boolean foundStar = false;

         while ( !endOfComment )
         {
            Char readChar = (Char)sr.Read();

            if ( foundStar )
            {
               if ( readChar == '/' )
               {
                  endOfComment = true;
               }
               else
               {
                  // False alarm. Not the end afterall.
                  sb.Append('*');

                  if ( readChar != '*' )
                  {
                     sb.Append(readChar);
                  }
               }
            }
            else if ( readChar != '*' )
            {
               sb.Append(readChar);
            }

            foundStar = readChar == '*';
         }

         return sb.ToString();
      }

      static Boolean IsIdentifierStart(StreamReaderEx sr)
      {
         return sr.Peek() == '_' || Char.IsLetter((Char)sr.Peek());
      }

      /// <summary>
      /// Reads a C-style identifier.
      /// </summary>
      static String ParseIdentifier(StreamReaderEx sr)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(AssertChar(sr, (Char c) => IsIdentifierStart(sr)));

         while ( sr.Peek() == '_' ||
                Char.IsLetter((Char)sr.Peek()) ||
                Char.IsNumber((Char)sr.Peek()) )
         {
            Char readChar = (Char)sr.Read();
            sb.Append(readChar);
         }

         return sb.ToString();
      }

      static Boolean IsIntStart(StreamReaderEx sr)
      {
         return sr.Peek() == '+' ||
                sr.Peek() == '-' ||
                Char.IsNumber((Char)sr.Peek());
      }

      /// <summary>
      /// Reads a C-style int.
      /// </summary>
      static Int64 ParseInt(StreamReaderEx sr)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(AssertChar(sr, (Char c) => IsIntStart(sr)));

         while ( Char.IsNumber((Char)sr.Peek()) )
         {
            Char readChar = (Char)sr.Read();
            sb.Append(readChar);
         }

         return Int64.Parse(sb.ToString());
      }

      static Boolean IsFloatStart(StreamReaderEx sr)
      {
         return sr.Peek() == '+' ||
                sr.Peek() == '-' ||
                sr.Peek() == '.' ||
                Char.IsNumber((Char)sr.Peek());
      }

      /// <summary>
      /// Reads a C-style float.
      /// </summary>
      static Double ParseFloat(StreamReaderEx sr)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(AssertChar(sr, (Char c) => IsFloatStart(sr)));

         // Note: This is not perfect, but it is assumed to be good enough.
         while ( sr.Peek() == '.' ||
                 sr.Peek() == 'e' ||
                 sr.Peek() == 'E' ||
                 sr.Peek() == '+' ||
                 sr.Peek() == '-' ||
                 Char.IsNumber((Char)sr.Peek()) )
         {
            Char readChar = (Char)sr.Read();
            sb.Append(readChar);
         }

         return Double.Parse(sb.ToString());
      }

      static Boolean IsStringStart(StreamReaderEx sr)
      {
         return sr.Peek() == '\"';
      }

      /// <summary>
      /// Reads a C-style string.
      /// </summary>
      static String ParseString(StreamReaderEx sr)
      {
         StringBuilder sb = new StringBuilder();
         AssertChar(sr, (Char c) => IsStringStart(sr));

         Boolean endOfString = false;
         Boolean escaped = false;

         while ( !endOfString )
         {
            Char readChar = (Char)sr.Read();

            if ( escaped )
            {
               switch ( readChar )
               {
               case 'a': // audible bell
                  sb.Append('\a');
                  break;
               case 'b': // backspace
                  sb.Append('\b');
                  break;
               case 'f': // form feed
                  sb.Append('\f');
                  break;
               case 'n': // line feed
                  sb.Append('\n');
                  break;
               case 'r': // carriage return
                  sb.Append('\r');
                  break;
               case 't': // horizontal tab
                  sb.Append('\t');
                  break;
               case 'v': // vertical tab
                  sb.Append('\v');
                  break;
               default:
                  sb.Append(readChar);
                  break;
               }

               escaped = false;
            }
            else if ( readChar != '\\' )
            {
               if ( readChar == '\"' )
               {
                  endOfString = true;
               }
               else
               {
                  sb.Append(readChar);
               }
            }
            else
            {
               escaped = true;
            }
         }

         return sb.ToString();
      }

      static Boolean IsTexCoordStart(StreamReaderEx sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 texcoord, e.g. "(1.0 2.0)".
      /// </summary>
      static texcoord ParseTexCoord(StreamReaderEx sr)
      {
         texcoord texcoord = new texcoord();
         AssertChar(sr, (Char c) => IsTexCoordStart(sr));
         ParseWhiteSpace(sr);

         texcoord.u = ParseFloat(sr);
         ParseWhiteSpace(sr);

         texcoord.v = ParseFloat(sr);
         ParseWhiteSpace(sr);

         AssertChar(sr, (Char c) => c == ')');
         return texcoord;
      }

      static Boolean IsPointStart(StreamReaderEx sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 point, e.g. "(1.0 2.0 3.0)".
      /// </summary>
      static point ParsePoint(StreamReaderEx sr)
      {
         point point = new point();
         AssertChar(sr, (Char c) => IsPointStart(sr));
         ParseWhiteSpace(sr);

         point.x = ParseFloat(sr);
         ParseWhiteSpace(sr);

         point.y = ParseFloat(sr);
         ParseWhiteSpace(sr);

         point.z = ParseFloat(sr);
         ParseWhiteSpace(sr);

         AssertChar(sr, (Char c) => c == ')');
         return point;
      }

      static Boolean IsQuaternionStart(StreamReaderEx sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 quaternion, e.g. "(1.0 2.0 3.0 4.0)".
      /// </summary>
      static quaternion ParseQuaternion(StreamReaderEx sr)
      {
         quaternion quaternion = new quaternion();
         AssertChar(sr, (Char c) => IsQuaternionStart(sr));
         ParseWhiteSpace(sr);

         quaternion.x = ParseFloat(sr);
         ParseWhiteSpace(sr);

         quaternion.y = ParseFloat(sr);
         ParseWhiteSpace(sr);

         quaternion.z = ParseFloat(sr);
         ParseWhiteSpace(sr);

         quaternion.w = ParseFloat(sr);
         ParseWhiteSpace(sr);

         AssertChar(sr, (Char c) => c == ')');
         return quaternion;
      }

      #region Special Case
      static Boolean IsEdgeStart(StreamReaderEx sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 edge, e.g. "(3 7 -1)".
      /// </summary>
      static V100.edge ParseEdge(StreamReaderEx sr)
      {
         V100.edge edge = new V100.edge();
         AssertChar(sr, (Char c) => IsPointStart(sr));
         ParseWhiteSpace(sr);

         edge.startpointindex = ParseInt(sr);
         ParseWhiteSpace(sr);

         edge.endpointindex = ParseInt(sr);
         ParseWhiteSpace(sr);

         if ( sr.Peek() != ')' )
         {
            edge.sharpness = ParseInt(sr);
            ParseWhiteSpace(sr);
         }

         AssertChar(sr, (Char c) => c == ')');
         return edge;
      }

      static Boolean IsFaceDataStart(StreamReaderEx sr)
      {
         return IsIntStart(sr);
      }

      /// <summary>
      /// Reads an An8 facedata, e.g. "3 4 0 -1 ( (17 2) (85 1) (4087 0) )".
      /// </summary>
      static V100.facedata ParseFaceData(StreamReaderEx sr)
      {
         V100.facedata facedata = new V100.facedata();
         facedata.numpoints = ParseInt(sr);
         ParseWhiteSpace(sr);

         facedata.flags = (V100.facedataenum)Enum.ToObject(
            typeof(V100.facedataenum),
            ParseInt(sr));

         ParseWhiteSpace(sr);

         facedata.matno = ParseInt(sr);
         ParseWhiteSpace(sr);

         facedata.flatnormalno = ParseInt(sr);
         ParseWhiteSpace(sr);

         List<V100.pointdata> pointdatas = new List<V100.pointdata>();
         AssertChar(sr, (Char c) => c == '(');
         ParseWhiteSpace(sr);

         while ( sr.Peek() != ')' )
         {
            V100.pointdata pointdata = new V100.pointdata();
            AssertChar(sr, (Char c) => c == '(');
            ParseWhiteSpace(sr);

            pointdata.pointindex = ParseInt(sr);
            ParseWhiteSpace(sr);

            if ( facedata.flags.HasFlag(V100.facedataenum.hasnormals) )
            {
               pointdata.normalindex = ParseInt(sr);
               ParseWhiteSpace(sr);
            }

            if ( facedata.flags.HasFlag(V100.facedataenum.hastexture) )
            {
               pointdata.texcoordindex = ParseInt(sr);
               ParseWhiteSpace(sr);
            }

            AssertChar(sr, (Char c) => c == ')');
            pointdatas.Add(pointdata);

            ParseWhiteSpace(sr);
         }

         AssertChar(sr, (Char c) => c == ')');
         facedata.pointdata = pointdatas.ToArray();

         return facedata;
      }

      static Boolean IsWeightDataStart(StreamReaderEx sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 weightdata, e.g. "(2 (0 0.928) (1 0.072))".
      /// </summary>
      static V100.weightdata ParseWeightData(StreamReaderEx sr)
      {
         V100.weightdata weightdata = new V100.weightdata();
         AssertChar(sr, (Char c) => IsWeightDataStart(sr));
         ParseWhiteSpace(sr);

         weightdata.numweights = ParseInt(sr);
         ParseWhiteSpace(sr);

         List<V100.bonedata> bonedatas = new List<V100.bonedata>();

         while ( sr.Peek() != ')' )
         {
            V100.bonedata bonedata = new V100.bonedata();
            AssertChar(sr, (Char c) => c == '(');
            ParseWhiteSpace(sr);

            bonedata.boneindex = ParseInt(sr);
            ParseWhiteSpace(sr);

            bonedata.boneweight = ParseFloat(sr);
            ParseWhiteSpace(sr);

            AssertChar(sr, (Char c) => c == ')');
            bonedatas.Add(bonedata);

            ParseWhiteSpace(sr);
         }

         AssertChar(sr, (Char c) => c == ')');
         weightdata.bonedata = bonedatas.ToArray();

         return weightdata;
      }
      #endregion

      static Boolean IsChunkStart(StreamReaderEx sr)
      {
         return sr.Peek() == '{';
      }

      /*static*/ Object ParseChunk(StreamReaderEx sr, Type t)
      {
         Object o = Activator.CreateInstance(t);
         FieldInfo[] fis = t.GetFields();
         AssertChar(sr, (Char c) => IsChunkStart(sr));
         ParseWhiteSpace(sr);

         // Read non-chunk fields
         foreach ( FieldInfo fi in fis )
         {
            if ( fi.FieldType == typeof(Int64) )
            {
               fi.SetValue(o, ParseInt(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(Int64[]) )
            {
               List<Int64> ints = new List<Int64>();

               while ( IsIntStart(sr) )
               {
                  ints.Add(ParseInt(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, ints.ToArray());
            }
            else if ( fi.FieldType == typeof(Double) )
            {
               fi.SetValue(o, ParseFloat(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(Double[]) )
            {
               List<Double> floats = new List<Double>();

               while ( IsFloatStart(sr) )
               {
                  floats.Add(ParseFloat(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, floats.ToArray());
            }
            else if ( fi.FieldType == typeof(String) )
            {
               // TODO: Do we need to parse L-prefixed strings differently?
               if ( sr.Peek() == 'L' )
               {
                  sr.Read();
               }

               fi.SetValue(o, ParseString(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(String[]) )
            {
               List<String> strings = new List<String>();

               while ( IsStringStart(sr) )
               {
                  strings.Add(ParseString(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, strings.ToArray());
            }
            else if ( fi.FieldType.IsEnum )
            {
               if ( fi.FieldType.IsDefined(
                       typeof(FlagsAttribute),
                       inherit: false) )
               {
                  // Parse as an int
                  fi.SetValue(o, Enum.ToObject(fi.FieldType, ParseInt(sr)));
                  ParseWhiteSpace(sr);
               }
               else
               {
                  // Parse as an enum
                  fi.SetValue(o, Enum.Parse(
                     fi.FieldType,
                     ParseIdentifier(sr)));

                  ParseWhiteSpace(sr);
               }
            }
            else if ( fi.FieldType == typeof(texcoord) )
            {
               fi.SetValue(o, ParseTexCoord(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(texcoord[]) )
            {
               List<texcoord> texcoords = new List<texcoord>();

               while ( IsTexCoordStart(sr) )
               {
                  texcoords.Add(ParseTexCoord(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, texcoords.ToArray());
            }
            else if ( fi.FieldType == typeof(point) )
            {
               fi.SetValue(o, ParsePoint(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(point[]) )
            {
               List<point> points = new List<point>();

               while ( IsPointStart(sr) )
               {
                  points.Add(ParsePoint(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, points.ToArray());
            }
            else if ( fi.FieldType == typeof(quaternion) )
            {
               fi.SetValue(o, ParseQuaternion(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(quaternion[]) )
            {
               List<quaternion> quaternions = new List<quaternion>();

               while ( IsQuaternionStart(sr) )
               {
                  quaternions.Add(ParseQuaternion(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, quaternions.ToArray());
            }
            #region Special Case - parsing edges
            else if ( fi.FieldType == typeof(V100.edge) )
            {
               fi.SetValue(o, ParseEdge(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(V100.edge[]) )
            {
               List<V100.edge> edges = new List<V100.edge>();

               while ( IsEdgeStart(sr) )
               {
                  edges.Add(ParseEdge(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, edges.ToArray());
            }
            #endregion
            #region Special Case - parsing facedata
            else if ( fi.FieldType == typeof(V100.facedata) )
            {
               fi.SetValue(o, ParseFaceData(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(V100.facedata[]) )
            {
               List<V100.facedata> facedatas = new List<V100.facedata>();

               while ( IsFaceDataStart(sr) )
               {
                  facedatas.Add(ParseFaceData(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, facedatas.ToArray());
            }
            #endregion
            #region Special Case - parsing weightdata
            else if ( fi.FieldType == typeof(V100.weightdata) )
            {
               fi.SetValue(o, ParseWeightData(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(V100.weightdata[]) )
            {
               List<V100.weightdata> weightdatas = new List<V100.weightdata>();

               while ( IsWeightDataStart(sr) )
               {
                  weightdatas.Add(ParseWeightData(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, weightdatas.ToArray());
            }
            #endregion
         }

         // Read chunk fields
         while ( sr.Peek() != '}' )
         {
            String chunkName = ParseIdentifier(sr);
            ParseWhiteSpace(sr);

            Boolean known = false;

            foreach ( FieldInfo fi in fis )
            {
               if ( fi.Name == chunkName )
               {
                  if ( fi.FieldType.IsArray )
                  {
                     Type type2 = fi.FieldType.GetElementType();
                     Object o2 = ParseChunk(sr, type2);
                     ParseWhiteSpace(sr);

                     Array newArray;

                     if ( fi.GetValue(o) is Array oldArray )
                     {
                        // Add to the array
                        newArray = Activator.CreateInstance(
                           fi.FieldType,
                           oldArray.Length + 1) as Array;

                        Array.Copy(oldArray, newArray, oldArray.Length);
                     }
                     else
                     {
                        // Create a new array
                        newArray = Activator.CreateInstance(
                           fi.FieldType,
                           1) as Array;
                     }

                     newArray.SetValue(o2, newArray.Length - 1);
                     fi.SetValue(o, newArray);
                  }
                  else
                  {
                     Type type2 = fi.FieldType;
                     Object o2 = ParseChunk(sr, type2);
                     ParseWhiteSpace(sr);

                     fi.SetValue(o, o2);
                  }

                  known = true;
                  break;
               }
            }

            if ( !known )
            {
               mCallback?.Invoke($"The \"{chunkName}\" chunk is not recognized.");

               String unknownChunk = ParseUnknownChunk(sr);
               ParseWhiteSpace(sr);
               continue;
            }
         }

         AssertChar(sr, (Char c) => c == '}');
         return o;
      }

      static String ParseUnknownChunk(StreamReaderEx sr)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(AssertChar(sr, (Char c) => IsChunkStart(sr)));

         while ( sr.Peek() != '}' )
         {
            if ( IsStringStart(sr) )
            {
               // Don't get confused by strings
               Boolean endOfString = false;
               Boolean escaped = false;

               while ( !endOfString )
               {
                  Char readChar = (Char)sr.Read();
                  sb.Append(readChar);

                  if ( escaped )
                  {
                     escaped = false;
                  }
                  else if ( readChar != '\\' )
                  {
                     if ( readChar == '\"' )
                     {
                        endOfString = true;
                     }
                  }
                  else
                  {
                     escaped = true;
                  }
               }
            }
            else if ( IsChunkStart(sr) )
            {
               String unknownChunk = ParseUnknownChunk(sr); // Recurse!
               sb.Append(unknownChunk);
            }
            else
            {
               sb.Append((Char)sr.Read());
            }
         }

         sb.Append(AssertChar(sr, (Char c) => c == '}'));
         return sb.ToString();
      }
      #endregion
   }
}
