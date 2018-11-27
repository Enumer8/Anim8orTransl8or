using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Anim8orTransl8or.Cli
{
   class Program
   {
      static void Main(String[] args)
      {
         String inFile = args[0];
         String outFile = args[1];

         ANIM8OR an8;

         using ( Stream stream = File.Open(inFile, FileMode.Open) )
         {
            An8Serializer deserializer = new An8Serializer(typeof(ANIM8OR));
            an8 = (ANIM8OR)deserializer.Deserialize(stream);
         }

         COLLADA dae = FormatConverter.Convert(an8);

         using ( Stream stream = File.Create(outFile) )
         {
            XmlSerializer serializer = new XmlSerializer(typeof(COLLADA));
            serializer.Serialize(stream, dae);
         }
      }
   }
}
