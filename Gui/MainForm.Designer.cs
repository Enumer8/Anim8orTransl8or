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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.mInputFileLabel = new System.Windows.Forms.Label();
         this.mInputFile = new System.Windows.Forms.TextBox();
         this.mOutputFileLabel = new System.Windows.Forms.Label();
         this.mOutputFile = new System.Windows.Forms.TextBox();
         this.mConvertButton = new System.Windows.Forms.Button();
         this.mPickInputFileButton = new System.Windows.Forms.Button();
         this.mPickOutputFileButton = new System.Windows.Forms.Button();
         this.mInputFileDialog = new System.Windows.Forms.OpenFileDialog();
         this.mOutputFileDialog = new System.Windows.Forms.SaveFileDialog();
         this.SuspendLayout();
         // 
         // mInputFileLabel
         // 
         this.mInputFileLabel.AutoSize = true;
         this.mInputFileLabel.Location = new System.Drawing.Point(13, 13);
         this.mInputFileLabel.Name = "mInputFileLabel";
         this.mInputFileLabel.Size = new System.Drawing.Size(132, 13);
         this.mInputFileLabel.TabIndex = 0;
         this.mInputFileLabel.Text = "Input ANIM8OR (.an8) file:";
         // 
         // mInputFile
         // 
         this.mInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.mInputFile.Location = new System.Drawing.Point(12, 29);
         this.mInputFile.Name = "mInputFile";
         this.mInputFile.Size = new System.Drawing.Size(317, 20);
         this.mInputFile.TabIndex = 1;
         // 
         // mOutputFileLabel
         // 
         this.mOutputFileLabel.AutoSize = true;
         this.mOutputFileLabel.Location = new System.Drawing.Point(13, 56);
         this.mOutputFileLabel.Name = "mOutputFileLabel";
         this.mOutputFileLabel.Size = new System.Drawing.Size(140, 13);
         this.mOutputFileLabel.TabIndex = 0;
         this.mOutputFileLabel.Text = "Output COLLADA (.dae) file:";
         // 
         // mOutputFile
         // 
         this.mOutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.mOutputFile.Location = new System.Drawing.Point(12, 72);
         this.mOutputFile.Name = "mOutputFile";
         this.mOutputFile.Size = new System.Drawing.Size(317, 20);
         this.mOutputFile.TabIndex = 3;
         // 
         // mConvertButton
         // 
         this.mConvertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.mConvertButton.Location = new System.Drawing.Point(297, 326);
         this.mConvertButton.Name = "mConvertButton";
         this.mConvertButton.Size = new System.Drawing.Size(75, 23);
         this.mConvertButton.TabIndex = 5;
         this.mConvertButton.Text = "Convert";
         this.mConvertButton.UseVisualStyleBackColor = true;
         this.mConvertButton.Click += new System.EventHandler(this.ConvertButton_Click);
         // 
         // mPickInputFileButton
         // 
         this.mPickInputFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.mPickInputFileButton.Location = new System.Drawing.Point(335, 27);
         this.mPickInputFileButton.Name = "mPickInputFileButton";
         this.mPickInputFileButton.Size = new System.Drawing.Size(37, 23);
         this.mPickInputFileButton.TabIndex = 2;
         this.mPickInputFileButton.Text = "...";
         this.mPickInputFileButton.UseVisualStyleBackColor = true;
         this.mPickInputFileButton.Click += new System.EventHandler(this.PickInputFileButton_Click);
         // 
         // mPickOutputFileButton
         // 
         this.mPickOutputFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.mPickOutputFileButton.Location = new System.Drawing.Point(335, 70);
         this.mPickOutputFileButton.Name = "mPickOutputFileButton";
         this.mPickOutputFileButton.Size = new System.Drawing.Size(37, 23);
         this.mPickOutputFileButton.TabIndex = 4;
         this.mPickOutputFileButton.Text = "...";
         this.mPickOutputFileButton.UseVisualStyleBackColor = true;
         this.mPickOutputFileButton.Click += new System.EventHandler(this.PickOutputFileButton_Click);
         // 
         // mInputFileDialog
         // 
         this.mInputFileDialog.Filter = "ANIM8OR (*.an8)|*.an8";
         // 
         // mOutputFileDialog
         // 
         this.mOutputFileDialog.Filter = "COLLADA (*.dae)|*.dae";
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(384, 361);
         this.Controls.Add(this.mPickOutputFileButton);
         this.Controls.Add(this.mPickInputFileButton);
         this.Controls.Add(this.mConvertButton);
         this.Controls.Add(this.mOutputFile);
         this.Controls.Add(this.mOutputFileLabel);
         this.Controls.Add(this.mInputFile);
         this.Controls.Add(this.mInputFileLabel);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(400, 200);
         this.Name = "MainForm";
         this.Text = "MainForm";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label mInputFileLabel;
      private System.Windows.Forms.TextBox mInputFile;
      private System.Windows.Forms.Label mOutputFileLabel;
      private System.Windows.Forms.TextBox mOutputFile;
      private System.Windows.Forms.Button mConvertButton;
      private System.Windows.Forms.Button mPickInputFileButton;
      private System.Windows.Forms.Button mPickOutputFileButton;
      private System.Windows.Forms.OpenFileDialog mInputFileDialog;
      private System.Windows.Forms.SaveFileDialog mOutputFileDialog;
   }
}

