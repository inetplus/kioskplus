namespace kioskplus.Windows
{
    partial class dlgAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgAbout));
            this.label1 = new System.Windows.Forms.Label();
            this.regfor = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Name = "label1";
            // 
            // regfor
            // 
            this.regfor.BackColor = System.Drawing.Color.Transparent;
            this.regfor.ForeColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.regfor, "regfor");
            this.regfor.Name = "regfor";
            this.regfor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dlgAbout_MouseMove);
            this.regfor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dlgAbout_MouseDown);
            this.regfor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dlgAbout_MouseUp);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label2.Name = "label2";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Gainsboro;
            this.label3.Name = "label3";
            // 
            // dlgAbout
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.BackgroundImage = global::kioskplus.kioskplus.inet_about;
            this.CancelButton = this.button1;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.regfor);
            this.Controls.Add(this.label1);
            this.MinimizeBox = false;
            this.Name = "dlgAbout";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label regfor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
    }
}