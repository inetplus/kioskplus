namespace kioskplus.Windows
{
    partial class dlgWebCam
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
            this.pBoxCam = new System.Windows.Forms.PictureBox();
            this.pBoxSnap = new System.Windows.Forms.PictureBox();
            this.btnSnap = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxCam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxSnap)).BeginInit();
            this.SuspendLayout();
            // 
            // pBoxCam
            // 
            this.pBoxCam.Location = new System.Drawing.Point(12, 12);
            this.pBoxCam.Name = "pBoxCam";
            this.pBoxCam.Size = new System.Drawing.Size(328, 281);
            this.pBoxCam.TabIndex = 0;
            this.pBoxCam.TabStop = false;
            // 
            // pBoxSnap
            // 
            this.pBoxSnap.Location = new System.Drawing.Point(346, 12);
            this.pBoxSnap.Name = "pBoxSnap";
            this.pBoxSnap.Size = new System.Drawing.Size(328, 281);
            this.pBoxSnap.TabIndex = 1;
            this.pBoxSnap.TabStop = false;
            // 
            // btnSnap
            // 
            this.btnSnap.Location = new System.Drawing.Point(690, 46);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(108, 45);
            this.btnSnap.TabIndex = 2;
            this.btnSnap.Text = "Snap";
            this.btnSnap.UseVisualStyleBackColor = true;
            this.btnSnap.Click += new System.EventHandler(this.btnSnap_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(690, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 45);
            this.button1.TabIndex = 3;
            this.button1.Text = "send per Email";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(690, 186);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(108, 64);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Schließen";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dlgWebCam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::kioskplus.kioskplus.wdlgcam1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(843, 326);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSnap);
            this.Controls.Add(this.pBoxSnap);
            this.Controls.Add(this.pBoxCam);
            this.Name = "dlgWebCam";
            this.Text = "dlgWebCam";
            this.Load += new System.EventHandler(this.dlgWebCam_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pBoxCam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxSnap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pBoxCam;
        private System.Windows.Forms.PictureBox pBoxSnap;
        private System.Windows.Forms.Button btnSnap;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnClose;
    }
}