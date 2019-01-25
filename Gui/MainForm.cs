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
         if ( mOutputFolderDialog.ShowDialog() == DialogResult.OK )
         {
            mOutputFile.Text = mOutputFolderDialog.SelectedPath;
         }
      }

      void ConvertButton_Click(Object sender, EventArgs e)
      {
         String inFile = mInputFile.Text;
         String outFolder = mOutputFile.Text;

         ANIM8OR an8;

         using ( Stream stream = File.Open(inFile, FileMode.Open) )
         {
            Serializer deserializer = new Serializer(typeof(ANIM8OR));
            an8 = (ANIM8OR)deserializer.Deserialize(stream);
         }

         // One An8 file can result in multiple files
         Directory.CreateDirectory(outFolder);
         String cwd = Path.GetDirectoryName(inFile);

         foreach ( Converter.Result result in Converter.Convert(an8, cwd) )
         {
            String outFile = Path.Combine(outFolder, result.FileName);

            if ( result.Dae != null )
            {
               using ( Stream stream = File.Create(outFile) )
               {
                  XmlSerializer xml = new XmlSerializer(typeof(COLLADA));
                  xml.Serialize(stream, result.Dae);
               }
            }
            else if ( result.Png != null )
            {
               result.Png.Save(outFile);
            }
         }
      }
   }
}
