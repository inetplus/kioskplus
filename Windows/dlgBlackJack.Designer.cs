namespace kioskplus
{
    partial class dlgBlackJack
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
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(783, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(24, 24);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "x";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dlgBlackJack
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Pink;
            this.BackgroundImage = global::kioskplus.kioskplus.inet_blackjack;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(819, 619);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dlgBlackJack";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TransparencyKey = System.Drawing.Color.Pink;
            this.Load += new System.EventHandler(this.dlgBlackJack_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dlgBlackJack_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.dlgBlackJack_Paint);
            this.Click += new System.EventHandler(this.dlgBlackJack_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dlgBlackJack_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dlgBlackJack_FormClosing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dlgBlackJack_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;


    }
}