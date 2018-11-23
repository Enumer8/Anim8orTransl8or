using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Anim8orTransl8or.An8.V100
{
   public class An8Loader
   {
      public ANIM8OR Load(Stream inStream)
      {
         An8Serializer deserializer = new An8Serializer(typeof(ANIM8OR));
         return (ANIM8OR)deserializer.Deserialize(inStream);
      }

      public void Save(Stream outStream, ANIM8OR an8)
      {
         An8Serializer serializer = new An8Serializer(typeof(ANIM8OR));
         serializer.Serialize(outStream, an8);
      }
   }

   #region An8Serializer
   public class An8Serializer
   {
      Type mType;

      public An8Serializer(Type type)
      {
         mType = type;
      }

      public Object Deserialize(Stream stream)
      {
         Object o = Activator.CreateInstance(mType);
         FieldInfo[] fis = mType.GetFields();

         using ( StreamReader sr = new StreamReader(stream) )
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
                              oldArray.LongLength + 1) as Array;

                           Array.Copy(oldArray, newArray, oldArray.LongLength);
                        }
                        else
                        {
                           // Create a new array
                           newArray = Activator.CreateInstance(
                              fi.FieldType,
                              1) as Array;
                        }

                        newArray.SetValue(o2, newArray.LongLength - 1);
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
                  String unknownChunk = ParseUnknownChunk(sr);
                  ParseWhiteSpace(sr);
                  continue;
               }
            }
         }

         return o;
      }

      public void Serialize(Stream stream, Object o)
      {
         // TODO: Implement
         throw new NotImplementedException();
      }

      #region Deserialize
      delegate Boolean CharCondition(Char c);

      static Char AssertChar(StreamReader sr, CharCondition condition)
      {
         // Note: Don't read yet just in case the condition wants to 'peek'
         Char c = (Char)sr.Peek();

         if ( condition(c) )
         {
            sr.Read();
            return c;
         }
         else
         {
            throw new FormatException();
         }
      }

      /// <summary>
      /// Reads all continguous whitespace.
      /// </summary>
      static void ParseWhiteSpace(StreamReader sr)
      {
         while ( Char.IsWhiteSpace((Char)sr.Peek()) )
         {
            sr.Read();
         }
      }

      static Boolean IsSlashStarCommentStart(StreamReader sr)
      {
         return sr.Peek() == '/';
      }

      /// <summary>
      /// Reads a C-style comment (e.g. /* and */).
      /// </summary>
      static String ParseSlashStarComment(StreamReader sr)
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

      static Boolean IsIdentifierStart(StreamReader sr)
      {
         return sr.Peek() == '_' || Char.IsLetter((Char)sr.Peek());
      }

      /// <summary>
      /// Reads a C-style identifier.
      /// </summary>
      static String ParseIdentifier(StreamReader sr)
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

      static Boolean IsIntStart(StreamReader sr)
      {
         return sr.Peek() == '+' ||
                sr.Peek() == '-' ||
                Char.IsNumber((Char)sr.Peek());
      }

      /// <summary>
      /// Reads a C-style int.
      /// </summary>
      static Int64 ParseInt(StreamReader sr)
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

      static Boolean IsFloatStart(StreamReader sr)
      {
         return sr.Peek() == '+' ||
                sr.Peek() == '-' ||
                sr.Peek() == '.' ||
                Char.IsNumber((Char)sr.Peek());
      }

      /// <summary>
      /// Reads a C-style float.
      /// </summary>
      static Double ParseFloat(StreamReader sr)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(AssertChar(sr, (Char c) => IsFloatStart(sr)));

         // Note: This is not perfect, but it is assumed to be good enough
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

      static Boolean IsStringStart(StreamReader sr)
      {
         return sr.Peek() == '\"';
      }

      /// <summary>
      /// Reads a C-style string.
      /// </summary>
      static String ParseString(StreamReader sr)
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

      #region Special Case
      static Boolean IsPointStart(StreamReader sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 point, e.g. "(1.0 2.0 3.0)".
      /// </summary>
      static point ParsePoint(StreamReader sr)
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

      static Boolean IsQuaternionStart(StreamReader sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 quaternion, e.g. "(1.0 2.0 3.0 4.0)".
      /// </summary>
      static quaternion ParseQuaternion(StreamReader sr)
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

      static Boolean IsTexCoordStart(StreamReader sr)
      {
         return sr.Peek() == '(';
      }

      /// <summary>
      /// Reads an An8 texcoord, e.g. "(1.0 2.0)".
      /// </summary>
      static texcoord ParseTexCoord(StreamReader sr)
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

      static Boolean IsFaceDataStart(StreamReader sr)
      {
         return IsIntStart(sr);
      }

      /// <summary>
      /// Reads an An8 facedata, e.g. "3 4 0 -1 ( (17 2) (85 1) (4087 0) )".
      /// </summary>
      static facedata ParseFaceData(StreamReader sr)
      {
         facedata facedata = new facedata();

         facedata.numpoints = ParseInt(sr);
         ParseWhiteSpace(sr);

         facedata.flags = (facedataenum)Enum.ToObject(
            typeof(facedataenum),
            ParseInt(sr));

         ParseWhiteSpace(sr);

         facedata.matno = ParseInt(sr);
         ParseWhiteSpace(sr);

         facedata.flatnormalno = ParseInt(sr);
         ParseWhiteSpace(sr);

         List<pointdata> pointdatas = new List<pointdata>();
         AssertChar(sr, (Char c) => c == '(');
         ParseWhiteSpace(sr);

         while ( sr.Peek() != ')' )
         {
            pointdata pointdata = new pointdata();
            AssertChar(sr, (Char c) => c == '(');
            ParseWhiteSpace(sr);

            pointdata.pointindex = ParseInt(sr);
            ParseWhiteSpace(sr);

            if ( facedata.flags.HasFlag(facedataenum.hasnormals) )
            {
               pointdata.normalindex = ParseInt(sr);
               ParseWhiteSpace(sr);
            }

            if ( facedata.flags.HasFlag(facedataenum.hastexture) )
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
      #endregion

      static Boolean IsChunkStart(StreamReader sr)
      {
         return sr.Peek() == '{';
      }

      static Object ParseChunk(StreamReader sr, Type type)
      {
         Object o = Activator.CreateInstance(type);
         FieldInfo[] fis = type.GetFields();
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
            #region Special Case - parsing a point
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
            #endregion
            #region Special Case - parsing a quaternion
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
            #endregion
            #region Special Case - parsing a texcoord
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
            #endregion
            #region Special Case - parsing a facedata
            else if ( fi.FieldType == typeof(facedata) )
            {
               fi.SetValue(o, ParseFaceData(sr));
               ParseWhiteSpace(sr);
            }
            else if ( fi.FieldType == typeof(facedata[]) )
            {
               List<facedata> facedatas = new List<facedata>();

               while ( IsFaceDataStart(sr) )
               {
                  facedatas.Add(ParseFaceData(sr));
                  ParseWhiteSpace(sr);
               }

               fi.SetValue(o, facedatas.ToArray());
            }
            #endregion
         }

         // Read chunk fields
         while ( sr.Peek() != '}' )
         {
            // TODO: How often do we need to check for comments?
            if ( IsSlashStarCommentStart(sr) )
            {
               String comment = ParseSlashStarComment(sr);
               ParseWhiteSpace(sr);
            }

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
                        // TODO: Why does this crash with long instead of int?
                        newArray = Activator.CreateInstance(
                           fi.FieldType,
                           (int)oldArray.LongLength + 1) as Array;

                        Array.Copy(oldArray, newArray, oldArray.LongLength);
                     }
                     else
                     {
                        // Create a new array
                        newArray = Activator.CreateInstance(
                           fi.FieldType,
                           1) as Array;
                     }

                     newArray.SetValue(o2, newArray.LongLength - 1);
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
               String unknownChunk = ParseUnknownChunk(sr);
               ParseWhiteSpace(sr);
               continue;
            }
         }

         AssertChar(sr, (Char c) => c == '}');
         return o;
      }

      static String ParseUnknownChunk(StreamReader sr)
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
   #endregion
}
