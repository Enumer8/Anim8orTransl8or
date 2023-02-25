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
using Anim8orTransl8or.Dae.V141;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
         if ( File.Exists(mInputFile.Text) )
         {
            mInputFileDialog.InitialDirectory = Path.GetDirectoryName(
               Path.GetFullPath(mInputFile.Text));

            mInputFileDialog.FileName = Path.GetFileName(mInputFile.Text);
         }

         if ( mInputFileDialog.ShowDialog() == DialogResult.OK )
         {
            mInputFile.Text = mInputFileDialog.FileName;
         }
      }

      void PickOutputFolderButton_Click(Object sender, EventArgs e)
      {
         if ( Directory.Exists(mOutputFolder.Text) )
         {
            mOutputFolderDialog.SelectedPath = Path.GetDirectoryName(
               mOutputFolder.Text.TrimEnd('\\') + '\\');
         }

         if ( mOutputFolderDialog.ShowDialog() == DialogResult.OK )
         {
            mOutputFolder.Text =
               mOutputFolderDialog.SelectedPath.TrimEnd('\\') + '\\';
         }
      }

      void CopyToClipboardButton_Click(Object sender, EventArgs e)
      {
         StringBuilder sb = new StringBuilder();

         foreach ( Object line in mOutput.Items )
         {
            if ( line is String s )
            {
               sb.AppendLine(s);
            }
         }

         Clipboard.SetText(sb.ToString());
      }

      void ConvertButton_Click(Object sender, EventArgs e)
      {
         EnableForm(false);

         mOutput.Items.Clear();

         String inFile = mInputFile.Text;
         String outFolder = mOutputFolder.Text;

         Task.Factory.StartNew(() =>
         {
            try
            {
               Convert(inFile, outFolder);
            }
            catch ( Exception ex )
            {
               mOutput.Invoke(new Action(() =>
                  mOutput.Items.Add(ex.Message)));
            }

            mOutput.Invoke(new Action(() => EnableForm(true)));
         });
      }

      void ProgressTimer_Tick(Object sender, EventArgs e)
      {
         if ( mProgress.Value < 100 )
         {
            mProgress.Value++;
         }
         else
         {
            mProgress.Value = 0;
         }
      }

      void EnableForm(Boolean enabled)
      {
         mInputFile.Enabled = enabled;
         mPickInputFileButton.Enabled = enabled;
         mOutputFolder.Enabled = enabled;
         mPickOutputFolderButton.Enabled = enabled;
         mOutput.Enabled = enabled;
         mCopyToClipboardButton.Enabled = enabled;
         mConvertButton.Enabled = enabled;

         if ( enabled )
         {
            mProgressTimer.Enabled = false;
            mProgress.Value = 0;
            mProgress.Visible = false;
         }
         else
         {
            mProgressTimer.Enabled = true;
            mProgress.Value = 0;
            mProgress.Visible = true;
         }
      }

      void AddOutput(String output)
      {
         if ( mOutput.InvokeRequired )
         {
            mOutput.Invoke(new Action(() => AddOutput(output)));
         }
         else
         {
            mOutput.Items.Add(output);
         }
      }

      void Convert(String inFile, String outFolder)
      {
         ANIM8OR an8;

         using ( Stream stream = File.Open(inFile, FileMode.Open) )
         {
            Serializer deserializer = new Serializer(
               typeof(ANIM8OR),
               AddOutput);

            an8 = (ANIM8OR)deserializer.Deserialize(stream);
         }

         // One an8 file can result in multiple files
         Directory.CreateDirectory(outFolder);
         String cwd = Path.GetDirectoryName(inFile);

         foreach ( ConverterResult result in
            Converter.Convert(an8, AddOutput, cwd) )
         {
            String outFile = Path.Combine(outFolder, result.FileName);

            if ( result.Dae != null )
            {
               using ( Stream stream = File.Create(outFile) )
               {
                  XmlSerializer xml = new XmlSerializer(typeof(COLLADA));
                  xml.Serialize(stream, result.Dae);
               }

               AddOutput($"Created {result.FileName}");
            }
            else if ( result.Png != null )
            {
               result.Png.SaveAsPng(outFile);

               AddOutput($"Created {result.FileName}");
            }
         }
      }
   }
}
