using System.IO;
using System.Xml.Serialization;

namespace Anim8orTransl8or.Dae.V141
{
   public class DaeLoader
   {
      public COLLADA Load(Stream inStream)
      {
         XmlSerializer deserializer = new XmlSerializer(typeof(COLLADA));
         return (COLLADA)deserializer.Deserialize(inStream);
      }

      public void Save(Stream outStream, COLLADA dae)
      {
         XmlSerializer serializer = new XmlSerializer(typeof(COLLADA));
         serializer.Serialize(outStream, dae);
      }
   }
}
