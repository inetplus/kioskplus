namespace kioskplus.Windows
{
    partial class dlgNewUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgNewUser));
            this.txtUser = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.Label();
            this.timerNUser = new System.Windows.Forms.Timer(this.components);
            this.notifyIconUser = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // txtUser
            // 
            this.txtUser.BackColor = System.Drawing.Color.Transparent;
            this.txtUser.Location = new System.Drawing.Point(27, 138);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(83, 13);
            this.txtUser.TabIndex = 0;
            this.txtUser.Text = "Surfer";
            this.txtUser.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtTime
            // 
            this.txtTime.BackColor = System.Drawing.Color.Transparent;
            this.txtTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTime.Location = new System.Drawing.Point(42, 60);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(50, 15);
            this.txtTime.TabIndex = 1;
            this.txtTime.Text = "00:00";
            this.txtTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // timerNUser
            // 
            this.timerNUser.Interval = 1000;
            this.timerNUser.Tick += new System.EventHandler(this.timerNUser_Tick);
            // 
            // notifyIconUser
            // 
            this.notifyIconUser.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconUser.Icon")));
            // 
            // dlgNewUser
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::kioskplus.kioskplus.main;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(140, 159);
            this.ControlBox = false;
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.txtUser);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dlgNewUser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dlgNewUser_MouseUp);
            this.Click += new System.EventHandler(this.dlgNewUser_Click);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dlgNewUser_FormClosing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dlgNewUser_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dlgNewUser_MouseDown);
            this.Load += new System.EventHandler(this.dlgNewUser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label txtUser;
        private System.Windows.Forms.Label txtTime;
        private System.Windows.Forms.Timer timerNUser;
        private System.Windows.Forms.NotifyIcon notifyIconUser;
    }
}