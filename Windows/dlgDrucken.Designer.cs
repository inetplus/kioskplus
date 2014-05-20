namespace kioskplus.Windows
{
    partial class dlgDrucken
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgDrucken));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtKosten = new System.Windows.Forms.TextBox();
            this.txtSeiten = new System.Windows.Forms.TextBox();
            this.txtGuthaben = new System.Windows.Forms.TextBox();
            this.lEinheit = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.timerDrucker = new System.Windows.Forms.Timer(this.components);
            this.msgText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.LightGray;
            this.label4.Name = "label4";
            // 
            // txtKosten
            // 
            resources.ApplyResources(this.txtKosten, "txtKosten");
            this.txtKosten.ForeColor = System.Drawing.Color.Black;
            this.txtKosten.Name = "txtKosten";
            // 
            // txtSeiten
            // 
            resources.ApplyResources(this.txtSeiten, "txtSeiten");
            this.txtSeiten.ForeColor = System.Drawing.Color.Black;
            this.txtSeiten.Name = "txtSeiten";
            // 
            // txtGuthaben
            // 
            resources.ApplyResources(this.txtGuthaben, "txtGuthaben");
            this.txtGuthaben.ForeColor = System.Drawing.Color.Black;
            this.txtGuthaben.Name = "txtGuthaben";
            // 
            // lEinheit
            // 
            resources.ApplyResources(this.lEinheit, "lEinheit");
            this.lEinheit.BackColor = System.Drawing.Color.Transparent;
            this.lEinheit.ForeColor = System.Drawing.Color.LightGray;
            this.lEinheit.Name = "lEinheit";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.LightGray;
            this.label6.Name = "label6";
            // 
            // btnYes
            // 
            this.btnYes.BackColor = System.Drawing.SystemColors.ButtonFace;
            resources.ApplyResources(this.btnYes, "btnYes");
            this.btnYes.Name = "btnYes";
            this.btnYes.UseVisualStyleBackColor = false;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnNo, "btnNo");
            this.btnNo.Name = "btnNo";
            this.btnNo.UseVisualStyleBackColor = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // timerDrucker
            // 
            this.timerDrucker.Interval = 1500;
            this.timerDrucker.Tick += new System.EventHandler(this.timerDrucker_Tick);
            // 
            // msgText
            // 
            resources.ApplyResources(this.msgText, "msgText");
            this.msgText.BackColor = System.Drawing.Color.Transparent;
            this.msgText.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.msgText.Name = "msgText";
            // 
            // dlgDrucken
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Pink;
            this.BackgroundImage = global::kioskplus.kioskplus.wdrucken;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnNo;
            this.Controls.Add(this.msgText);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lEinheit);
            this.Controls.Add(this.txtGuthaben);
            this.Controls.Add(this.txtSeiten);
            this.Controls.Add(this.txtKosten);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "dlgDrucken";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.dlgDrucken_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.dlgDrucken_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtKosten;
        private System.Windows.Forms.TextBox txtSeiten;
        private System.Windows.Forms.TextBox txtGuthaben;
        private System.Windows.Forms.Label lEinheit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Timer timerDrucker;
        private System.Windows.Forms.Label msgText;
    }
}