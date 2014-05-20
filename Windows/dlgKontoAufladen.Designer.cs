namespace kioskplus.Windows
{
    partial class dlgKontoAufladen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgKontoAufladen));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnEnde = new System.Windows.Forms.Button();
            this.lid = new System.Windows.Forms.Label();
            this.lpwd = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtPWD = new System.Windows.Forms.TextBox();
            this.lMsg = new System.Windows.Forms.Label();
            this.lcouponid = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnOK.BackgroundImage = null;
            this.btnOK.Font = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnEnde
            // 
            this.btnEnde.AccessibleDescription = null;
            this.btnEnde.AccessibleName = null;
            resources.ApplyResources(this.btnEnde, "btnEnde");
            this.btnEnde.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnEnde.BackgroundImage = null;
            this.btnEnde.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnEnde.Font = null;
            this.btnEnde.Name = "btnEnde";
            this.btnEnde.UseVisualStyleBackColor = false;
            this.btnEnde.Click += new System.EventHandler(this.btnEnde_Click);
            // 
            // lid
            // 
            this.lid.AccessibleDescription = null;
            this.lid.AccessibleName = null;
            resources.ApplyResources(this.lid, "lid");
            this.lid.BackColor = System.Drawing.Color.Transparent;
            this.lid.Font = null;
            this.lid.ForeColor = System.Drawing.Color.Gainsboro;
            this.lid.Name = "lid";
            // 
            // lpwd
            // 
            this.lpwd.AccessibleDescription = null;
            this.lpwd.AccessibleName = null;
            resources.ApplyResources(this.lpwd, "lpwd");
            this.lpwd.BackColor = System.Drawing.Color.Transparent;
            this.lpwd.Font = null;
            this.lpwd.ForeColor = System.Drawing.Color.Gainsboro;
            this.lpwd.Name = "lpwd";
            // 
            // txtID
            // 
            this.txtID.AccessibleDescription = null;
            this.txtID.AccessibleName = null;
            resources.ApplyResources(this.txtID, "txtID");
            this.txtID.BackColor = System.Drawing.Color.White;
            this.txtID.BackgroundImage = null;
            this.txtID.Font = null;
            this.txtID.Name = "txtID";
            // 
            // txtPWD
            // 
            this.txtPWD.AccessibleDescription = null;
            this.txtPWD.AccessibleName = null;
            resources.ApplyResources(this.txtPWD, "txtPWD");
            this.txtPWD.BackColor = System.Drawing.Color.White;
            this.txtPWD.BackgroundImage = null;
            this.txtPWD.Font = null;
            this.txtPWD.Name = "txtPWD";
            this.txtPWD.UseSystemPasswordChar = true;
            // 
            // lMsg
            // 
            this.lMsg.AccessibleDescription = null;
            this.lMsg.AccessibleName = null;
            resources.ApplyResources(this.lMsg, "lMsg");
            this.lMsg.BackColor = System.Drawing.Color.Transparent;
            this.lMsg.Font = null;
            this.lMsg.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lMsg.Name = "lMsg";
            // 
            // lcouponid
            // 
            this.lcouponid.AccessibleDescription = null;
            this.lcouponid.AccessibleName = null;
            resources.ApplyResources(this.lcouponid, "lcouponid");
            this.lcouponid.BackColor = System.Drawing.Color.Transparent;
            this.lcouponid.Font = null;
            this.lcouponid.ForeColor = System.Drawing.Color.Gainsboro;
            this.lcouponid.Name = "lcouponid";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.Yellow;
            this.label8.Name = "label8";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Name = "label1";
            // 
            // dlgKontoAufladen
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.BackgroundImage = global::kioskplus.kioskplus.waufladen;
            this.CancelButton = this.btnEnde;
            this.ControlBox = false;
            this.Controls.Add(this.lcouponid);
            this.Controls.Add(this.lMsg);
            this.Controls.Add(this.txtPWD);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.lpwd);
            this.Controls.Add(this.lid);
            this.Controls.Add(this.btnEnde);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.Name = "dlgKontoAufladen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TransparencyKey = System.Drawing.Color.Pink;
            this.Load += new System.EventHandler(this.dlgKontoAufladen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnEnde;
        private System.Windows.Forms.Label lid;
        private System.Windows.Forms.Label lpwd;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtPWD;
        private System.Windows.Forms.Label lMsg;
        private System.Windows.Forms.Label lcouponid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
    }
}