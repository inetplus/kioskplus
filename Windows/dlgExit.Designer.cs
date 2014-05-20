namespace kioskplus.Windows
{
    partial class dlgExit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgExit));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbBlock = new System.Windows.Forms.RadioButton();
            this.rbCancel = new System.Windows.Forms.RadioButton();
            this.rbKonto = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnOK.BackgroundImage = null;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnOK.ForeColor = System.Drawing.Color.Black;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // rbBlock
            // 
            this.rbBlock.AccessibleDescription = null;
            this.rbBlock.AccessibleName = null;
            resources.ApplyResources(this.rbBlock, "rbBlock");
            this.rbBlock.BackColor = System.Drawing.Color.Transparent;
            this.rbBlock.BackgroundImage = null;
            this.rbBlock.Checked = true;
            this.rbBlock.Font = null;
            this.rbBlock.Name = "rbBlock";
            this.rbBlock.TabStop = true;
            this.rbBlock.UseVisualStyleBackColor = false;
            // 
            // rbCancel
            // 
            this.rbCancel.AccessibleDescription = null;
            this.rbCancel.AccessibleName = null;
            resources.ApplyResources(this.rbCancel, "rbCancel");
            this.rbCancel.BackColor = System.Drawing.Color.Transparent;
            this.rbCancel.BackgroundImage = null;
            this.rbCancel.Font = null;
            this.rbCancel.Name = "rbCancel";
            this.rbCancel.UseVisualStyleBackColor = false;
            // 
            // rbKonto
            // 
            this.rbKonto.AccessibleDescription = null;
            this.rbKonto.AccessibleName = null;
            resources.ApplyResources(this.rbKonto, "rbKonto");
            this.rbKonto.BackColor = System.Drawing.Color.Transparent;
            this.rbKonto.BackgroundImage = null;
            this.rbKonto.Font = null;
            this.rbKonto.Name = "rbKonto";
            this.rbKonto.UseVisualStyleBackColor = false;
            // 
            // dlgExit
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.Pink;
            this.BackgroundImage = global::kioskplus.kioskplus.wsperren;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.rbKonto);
            this.Controls.Add(this.rbCancel);
            this.Controls.Add(this.rbBlock);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = null;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Icon = null;
            this.Name = "dlgExit";
            this.Load += new System.EventHandler(this.dlgExit_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.dlgExit_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbBlock;
        private System.Windows.Forms.RadioButton rbCancel;
        private System.Windows.Forms.RadioButton rbKonto;
    }
}