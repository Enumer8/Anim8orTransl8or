using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Anim8orTransl8or.Gui
{
   public partial class MainForm : Form
   {
      public MainForm()
      {
         InitializeComponent();

         AssemblyName assemblyName = GetType().Assembly.GetName();
         Text = assemblyName.Name + " v" + assemblyName.Version.ToString(3);

#if DEBUG
         Text = Text + "-Debug";
#endif
      }

      void PickInputFileButton_Click(Object sender, EventArgs e)
      {
         if ( mInputFileDialog.ShowDialog() == DialogResult.OK )
         {
            mInputFile.Text = mInputFileDialog.FileName;
         }
      }

      void PickOutputFileButton_Click(Object sender, EventArgs e)
      {
         if ( mOutputFileDialog.ShowDialog() == DialogResult.OK )
         {
            mOutputFile.Text = mOutputFileDialog.FileName;
         }
      }

      void ConvertButton_Click(Object sender, EventArgs e)
      {
         String inFile = mInputFile.Text;
         String outFile = mOutputFile.Text;

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
