namespace kioskplus.Windows
{
    partial class dlgBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgBase));
            this.SuspendLayout();
            // 
            // dlgBase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dlgBase";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TransparencyKey = System.Drawing.Color.Pink;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dlgBase_MouseUp);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.dlgBase_FormClosed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dlgBase_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dlgBase_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}