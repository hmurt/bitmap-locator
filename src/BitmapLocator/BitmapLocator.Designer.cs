namespace ScreenScraper
{
    partial class BitmapLocator
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
            if (disposing && (components != null))
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
            this.bitmapOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.labelBitmapFile = new System.Windows.Forms.Label();
            this.textBoxBitmap = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonFind = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bitmapOpenFileDialog
            // 
            this.bitmapOpenFileDialog.Multiselect = true;
            this.bitmapOpenFileDialog.Title = "Please choose a bitmap or multiple bitmaps, up to 3";
            // 
            // labelBitmapFile
            // 
            this.labelBitmapFile.AutoSize = true;
            this.labelBitmapFile.Location = new System.Drawing.Point(13, 13);
            this.labelBitmapFile.Name = "labelBitmapFile";
            this.labelBitmapFile.Size = new System.Drawing.Size(131, 13);
            this.labelBitmapFile.TabIndex = 0;
            this.labelBitmapFile.Text = "Bitmap file(s) to search for:";
            // 
            // textBoxBitmap
            // 
            this.textBoxBitmap.Location = new System.Drawing.Point(150, 10);
            this.textBoxBitmap.Name = "textBoxBitmap";
            this.textBoxBitmap.Size = new System.Drawing.Size(257, 20);
            this.textBoxBitmap.TabIndex = 1;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(413, 8);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(59, 23);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonFind
            // 
            this.buttonFind.Location = new System.Drawing.Point(197, 51);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(75, 23);
            this.buttonFind.TabIndex = 3;
            this.buttonFind.Text = "Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // BitmapLocator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 97);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxBitmap);
            this.Controls.Add(this.labelBitmapFile);
            this.Name = "BitmapLocator";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog bitmapOpenFileDialog;
        private System.Windows.Forms.Label labelBitmapFile;
        private System.Windows.Forms.TextBox textBoxBitmap;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonFind;

    }
}

