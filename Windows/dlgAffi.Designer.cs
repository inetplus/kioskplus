namespace kioskplus.Windows
{
    partial class dlgAffi
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
            this.pbaffi = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbaffi)).BeginInit();
            this.SuspendLayout();
            // 
            // pbaffi
            // 
            this.pbaffi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbaffi.ImageLocation = "";
            this.pbaffi.Location = new System.Drawing.Point(0, 0);
            this.pbaffi.Name = "pbaffi";
            this.pbaffi.Size = new System.Drawing.Size(110, 150);
            this.pbaffi.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbaffi.TabIndex = 2;
            this.pbaffi.TabStop = false;
            this.pbaffi.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pbaffi_LoadCompleted);
            this.pbaffi.Click += new System.EventHandler(this.pbaffi_Click);
            // 
            // dlgAffi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(129, 150);
            this.Controls.Add(this.pbaffi);
            this.Name = "dlgAffi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "dlgAffi";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.dlgAffi_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dlgAffi_FormClosing);
            this.LocationChanged += new System.EventHandler(this.dlgAffi_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pbaffi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbaffi;
    }
}