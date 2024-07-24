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

using System;
using System.IO;

namespace Anim8orTransl8or.An8
{
   public class StreamReaderEx : IDisposable
   {
      public StreamReaderEx(Stream s)
      {
         _Sr = new StreamReader(s);
         Line = 1;
         Column = 1;
      }

      public Boolean EndOfStream
      {
         get
         {
            return _Sr.EndOfStream;
         }
      }

      public Stream BaseStream
      {
         get
         {
            return _Sr.BaseStream;
         }
      }

      public Int32 Line
      {
         get;
         private set;
      }

      public Int32 Column
      {
         get;
         private set;
      }

      public void Dispose()
      {
         _Sr.Dispose();
      }

      public Int32 Peek()
      {
         return _Sr.Peek();
      }

      public Int32 Read()
      {
         Int32 read = _Sr.Read();

         switch ( read )
         {
         case '\r':
            Column = 1;
            break;
         case '\n':
            Line++;
            Column = 1;
            break;
         default:
            Column++;
            break;
         }

         return read;
      }

      StreamReader _Sr;
   }
}
