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
