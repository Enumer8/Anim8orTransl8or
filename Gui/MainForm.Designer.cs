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

namespace Anim8orTransl8or.Gui
{
   partial class MainForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if ( disposing && (components != null) )
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.mInputFileLabel = new System.Windows.Forms.Label();
         this.mInputFile = new System.Windows.Forms.TextBox();
         this.mOutputFileLabel = new System.Windows.Forms.Label();
         this.mOutputFolder = new System.Windows.Forms.TextBox();
         this.mConvertButton = new System.Windows.Forms.Button();
         this.mPickInputFileButton = new System.Windows.Forms.Button();
         this.mPickOutputFolderButton = new System.Windows.Forms.Button();
         this.mInputFileDialog = new System.Windows.Forms.OpenFileDialog();
         this.mOutputFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
         this.mOutputLabel = new System.Windows.Forms.Label();
         this.mOutput = new System.Windows.Forms.ListBox();
         this.mCopyToClipboardButton = new System.Windows.Forms.Button();
         this.mProgress = new System.Windows.Forms.ProgressBar();
         this.mProgressTimer = new System.Windows.Forms.Timer(this.components);
         this.SuspendLayout();
         // 
         // mInputFileLabel
         // 
         this.mInputFileLabel.AutoSize = true;
         this.mInputFileLabel.Location = new System.Drawing.Point(12, 9);
         this.mInputFileLabel.Name = "mInputFileLabel";
         this.mInputFileLabel.Size = new System.Drawing.Size(132, 13);
         this.mInputFileLabel.TabIndex = 0;
         this.mInputFileLabel.Text = "Input ANIM8OR (.an8) file:";
         // 
         // mInputFile
         // 
         this.mInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.mInputFile.Location = new System.Drawing.Point(12, 25);
         this.mInputFile.Name = "mInputFile";
         this.mInputFile.Size = new System.Drawing.Size(317, 20);
         this.mInputFile.TabIndex = 1;
         // 
         // mOutputFileLabel
         // 
         this.mOutputFileLabel.AutoSize = true;
         this.mOutputFileLabel.Location = new System.Drawing.Point(12, 48);
         this.mOutputFileLabel.Name = "mOutputFileLabel";
         this.mOutputFileLabel.Size = new System.Drawing.Size(153, 13);
         this.mOutputFileLabel.TabIndex = 0;
         this.mOutputFileLabel.Text = "Output COLLADA (.dae) folder:";
         // 
         // mOutputFolder
         // 
         this.mOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.mOutputFolder.Location = new System.Drawing.Point(12, 64);
         this.mOutputFolder.Name = "mOutputFolder";
         this.mOutputFolder.Size = new System.Drawing.Size(317, 20);
         this.mOutputFolder.TabIndex = 3;
         // 
         // mConvertButton
         // 
         this.mConvertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.mConvertButton.Location = new System.Drawing.Point(297, 326);
         this.mConvertButton.Name = "mConvertButton";
         this.mConvertButton.Size = new System.Drawing.Size(75, 23);
         this.mConvertButton.TabIndex = 7;
         this.mConvertButton.Text = "Convert";
         this.mConvertButton.UseVisualStyleBackColor = true;
         this.mConvertButton.Click += new System.EventHandler(this.ConvertButton_Click);
         // 
         // mPickInputFileButton
         // 
         this.mPickInputFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.mPickInputFileButton.Location = new System.Drawing.Point(335, 24);
         this.mPickInputFileButton.Name = "mPickInputFileButton";
         this.mPickInputFileButton.Size = new System.Drawing.Size(37, 22);
         this.mPickInputFileButton.TabIndex = 2;
         this.mPickInputFileButton.Text = "...";
         this.mPickInputFileButton.UseVisualStyleBackColor = true;
         this.mPickInputFileButton.Click += new System.EventHandler(this.PickInputFileButton_Click);
         // 
         // mPickOutputFolderButton
         // 
         this.mPickOutputFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.mPickOutputFolderButton.Location = new System.Drawing.Point(335, 63);
         this.mPickOutputFolderButton.Name = "mPickOutputFolderButton";
         this.mPickOutputFolderButton.Size = new System.Drawing.Size(37, 22);
         this.mPickOutputFolderButton.TabIndex = 4;
         this.mPickOutputFolderButton.Text = "...";
         this.mPickOutputFolderButton.UseVisualStyleBackColor = true;
         this.mPickOutputFolderButton.Click += new System.EventHandler(this.PickOutputFolderButton_Click);
         // 
         // mInputFileDialog
         // 
         this.mInputFileDialog.Filter = "ANIM8OR (*.an8)|*.an8";
         // 
         // mOutputLabel
         // 
         this.mOutputLabel.AutoSize = true;
         this.mOutputLabel.Location = new System.Drawing.Point(12, 131);
         this.mOutputLabel.Name = "mOutputLabel";
         this.mOutputLabel.Size = new System.Drawing.Size(42, 13);
         this.mOutputLabel.TabIndex = 0;
         this.mOutputLabel.Text = "Output:";
         // 
         // mOutput
         // 
         this.mOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.mOutput.FormattingEnabled = true;
         this.mOutput.Location = new System.Drawing.Point(12, 147);
         this.mOutput.Name = "mOutput";
         this.mOutput.ScrollAlwaysVisible = true;
         this.mOutput.Size = new System.Drawing.Size(360, 173);
         this.mOutput.TabIndex = 5;
         // 
         // mCopyToClipboardButton
         // 
         this.mCopyToClipboardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.mCopyToClipboardButton.Location = new System.Drawing.Point(12, 326);
         this.mCopyToClipboardButton.Name = "mCopyToClipboardButton";
         this.mCopyToClipboardButton.Size = new System.Drawing.Size(125, 23);
         this.mCopyToClipboardButton.TabIndex = 6;
         this.mCopyToClipboardButton.Text = "Copy To Clipboard";
         this.mCopyToClipboardButton.UseVisualStyleBackColor = true;
         this.mCopyToClipboardButton.Click += new System.EventHandler(this.CopyToClipboardButton_Click);
         // 
         // mProgress
         // 
         this.mProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.mProgress.Location = new System.Drawing.Point(12, 105);
         this.mProgress.Name = "mProgress";
         this.mProgress.Size = new System.Drawing.Size(360, 23);
         this.mProgress.TabIndex = 8;
         this.mProgress.Visible = false;
         // 
         // mProgressTimer
         // 
         this.mProgressTimer.Tick += new System.EventHandler(this.ProgressTimer_Tick);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(384, 361);
         this.Controls.Add(this.mProgress);
         this.Controls.Add(this.mCopyToClipboardButton);
         this.Controls.Add(this.mOutputLabel);
         this.Controls.Add(this.mOutput);
         this.Controls.Add(this.mPickOutputFolderButton);
         this.Controls.Add(this.mPickInputFileButton);
         this.Controls.Add(this.mConvertButton);
         this.Controls.Add(this.mOutputFolder);
         this.Controls.Add(this.mOutputFileLabel);
         this.Controls.Add(this.mInputFile);
         this.Controls.Add(this.mInputFileLabel);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(400, 300);
         this.Name = "MainForm";
         this.Text = "MainForm";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label mInputFileLabel;
      private System.Windows.Forms.TextBox mInputFile;
      private System.Windows.Forms.Label mOutputFileLabel;
      private System.Windows.Forms.TextBox mOutputFolder;
      private System.Windows.Forms.Button mConvertButton;
      private System.Windows.Forms.Button mPickInputFileButton;
      private System.Windows.Forms.Button mPickOutputFolderButton;
      private System.Windows.Forms.OpenFileDialog mInputFileDialog;
      private System.Windows.Forms.FolderBrowserDialog mOutputFolderDialog;
      private System.Windows.Forms.Label mOutputLabel;
      private System.Windows.Forms.ListBox mOutput;
      private System.Windows.Forms.Button mCopyToClipboardButton;
      private System.Windows.Forms.ProgressBar mProgress;
      private System.Windows.Forms.Timer mProgressTimer;
   }
}

