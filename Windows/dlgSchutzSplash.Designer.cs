namespace kioskplus.Windows
{
    partial class dlgSchutzSplash
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
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.panel = new System.Windows.Forms.Panel();
            this.regfor = new System.Windows.Forms.Button();
            this.picLink = new System.Windows.Forms.PictureBox();
            this.txtSpiele = new System.Windows.Forms.Label();
            this.txtVoip = new System.Windows.Forms.Label();
            this.txtSms = new System.Windows.Forms.Label();
            this.txtMP = new System.Windows.Forms.Label();
            this.txtIKunden = new System.Windows.Forms.Label();
            this.txtBill = new System.Windows.Forms.Label();
            this.labelSpiele = new System.Windows.Forms.Label();
            this.labelVOIP = new System.Windows.Forms.Label();
            this.labelSMS = new System.Windows.Forms.Label();
            this.labelMP = new System.Windows.Forms.Label();
            this.labelIkunden = new System.Windows.Forms.Label();
            this.labelBill = new System.Windows.Forms.Label();
            this.msgtext = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.timerSplash = new System.Windows.Forms.Timer(this.components);
            this.timerCheck = new System.Windows.Forms.Timer(this.components);
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLink)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(1010, 520);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.Black;
            this.panel.Controls.Add(this.regfor);
            this.panel.Controls.Add(this.picLink);
            this.panel.Controls.Add(this.txtSpiele);
            this.panel.Controls.Add(this.txtVoip);
            this.panel.Controls.Add(this.txtSms);
            this.panel.Controls.Add(this.txtMP);
            this.panel.Controls.Add(this.txtIKunden);
            this.panel.Controls.Add(this.txtBill);
            this.panel.Controls.Add(this.labelSpiele);
            this.panel.Controls.Add(this.labelVOIP);
            this.panel.Controls.Add(this.labelSMS);
            this.panel.Controls.Add(this.labelMP);
            this.panel.Controls.Add(this.labelIkunden);
            this.panel.Controls.Add(this.labelBill);
            this.panel.Controls.Add(this.msgtext);
            this.panel.Controls.Add(this.progressBar);
            this.panel.Cursor = System.Windows.Forms.Cursors.No;
            this.panel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panel.Location = new System.Drawing.Point(0, 557);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(1010, 81);
            this.panel.TabIndex = 1;
            this.panel.TabStop = true;
            this.panel.Click += new System.EventHandler(this.panel_Click);
            // 
            // regfor
            // 
            this.regfor.BackColor = System.Drawing.Color.Black;
            this.regfor.ForeColor = System.Drawing.Color.White;
            this.regfor.Location = new System.Drawing.Point(811, 1);
            this.regfor.Name = "regfor";
            this.regfor.Size = new System.Drawing.Size(193, 73);
            this.regfor.TabIndex = 18;
            this.regfor.Text = "inetplus";
            this.regfor.UseVisualStyleBackColor = false;
            this.regfor.Click += new System.EventHandler(this.regfor_Click);
            // 
            // picLink
            // 
            this.picLink.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picLink.Image = global::kioskplus.kioskplus.ti;
            this.picLink.Location = new System.Drawing.Point(321, 9);
            this.picLink.Name = "picLink";
            this.picLink.Size = new System.Drawing.Size(48, 48);
            this.picLink.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picLink.TabIndex = 17;
            this.picLink.TabStop = false;
            this.picLink.Visible = false;
            this.picLink.Click += new System.EventHandler(this.picLink_Click);
            // 
            // txtSpiele
            // 
            this.txtSpiele.AutoSize = true;
            this.txtSpiele.ForeColor = System.Drawing.Color.White;
            this.txtSpiele.Location = new System.Drawing.Point(176, 39);
            this.txtSpiele.Name = "txtSpiele";
            this.txtSpiele.Size = new System.Drawing.Size(40, 13);
            this.txtSpiele.TabIndex = 16;
            this.txtSpiele.Text = "Games";
            // 
            // txtVoip
            // 
            this.txtVoip.AutoSize = true;
            this.txtVoip.ForeColor = System.Drawing.Color.White;
            this.txtVoip.Location = new System.Drawing.Point(176, 24);
            this.txtVoip.Name = "txtVoip";
            this.txtVoip.Size = new System.Drawing.Size(28, 13);
            this.txtVoip.TabIndex = 15;
            this.txtVoip.Text = "Voip";
            // 
            // txtSms
            // 
            this.txtSms.AutoSize = true;
            this.txtSms.ForeColor = System.Drawing.Color.White;
            this.txtSms.Location = new System.Drawing.Point(176, 9);
            this.txtSms.Name = "txtSms";
            this.txtSms.Size = new System.Drawing.Size(27, 13);
            this.txtSms.TabIndex = 14;
            this.txtSms.Text = "Sms";
            // 
            // txtMP
            // 
            this.txtMP.AutoSize = true;
            this.txtMP.ForeColor = System.Drawing.Color.White;
            this.txtMP.Location = new System.Drawing.Point(14, 24);
            this.txtMP.Name = "txtMP";
            this.txtMP.Size = new System.Drawing.Size(23, 13);
            this.txtMP.TabIndex = 13;
            this.txtMP.Text = "MP";
            // 
            // txtIKunden
            // 
            this.txtIKunden.AutoSize = true;
            this.txtIKunden.ForeColor = System.Drawing.Color.White;
            this.txtIKunden.Location = new System.Drawing.Point(14, 39);
            this.txtIKunden.Name = "txtIKunden";
            this.txtIKunden.Size = new System.Drawing.Size(46, 13);
            this.txtIKunden.TabIndex = 12;
            this.txtIKunden.Text = "Ikunden";
            // 
            // txtBill
            // 
            this.txtBill.AutoSize = true;
            this.txtBill.ForeColor = System.Drawing.Color.White;
            this.txtBill.Location = new System.Drawing.Point(14, 9);
            this.txtBill.Name = "txtBill";
            this.txtBill.Size = new System.Drawing.Size(20, 13);
            this.txtBill.TabIndex = 11;
            this.txtBill.Text = "Bill";
            // 
            // labelSpiele
            // 
            this.labelSpiele.BackColor = System.Drawing.Color.Red;
            this.labelSpiele.Location = new System.Drawing.Point(164, 41);
            this.labelSpiele.Name = "labelSpiele";
            this.labelSpiele.Size = new System.Drawing.Size(7, 8);
            this.labelSpiele.TabIndex = 10;
            // 
            // labelVOIP
            // 
            this.labelVOIP.BackColor = System.Drawing.Color.Red;
            this.labelVOIP.Location = new System.Drawing.Point(164, 26);
            this.labelVOIP.Name = "labelVOIP";
            this.labelVOIP.Size = new System.Drawing.Size(7, 8);
            this.labelVOIP.TabIndex = 9;
            // 
            // labelSMS
            // 
            this.labelSMS.BackColor = System.Drawing.Color.Red;
            this.labelSMS.Location = new System.Drawing.Point(164, 11);
            this.labelSMS.Name = "labelSMS";
            this.labelSMS.Size = new System.Drawing.Size(7, 8);
            this.labelSMS.TabIndex = 8;
            // 
            // labelMP
            // 
            this.labelMP.BackColor = System.Drawing.Color.Red;
            this.labelMP.Location = new System.Drawing.Point(3, 26);
            this.labelMP.Name = "labelMP";
            this.labelMP.Size = new System.Drawing.Size(7, 8);
            this.labelMP.TabIndex = 7;
            // 
            // labelIkunden
            // 
            this.labelIkunden.BackColor = System.Drawing.Color.Red;
            this.labelIkunden.Location = new System.Drawing.Point(3, 41);
            this.labelIkunden.Name = "labelIkunden";
            this.labelIkunden.Size = new System.Drawing.Size(7, 8);
            this.labelIkunden.TabIndex = 6;
            // 
            // labelBill
            // 
            this.labelBill.BackColor = System.Drawing.Color.Red;
            this.labelBill.Location = new System.Drawing.Point(3, 11);
            this.labelBill.Name = "labelBill";
            this.labelBill.Size = new System.Drawing.Size(7, 8);
            this.labelBill.TabIndex = 5;
            // 
            // msgtext
            // 
            this.msgtext.BackColor = System.Drawing.Color.Black;
            this.msgtext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msgtext.ForeColor = System.Drawing.Color.White;
            this.msgtext.Location = new System.Drawing.Point(356, 27);
            this.msgtext.Name = "msgtext";
            this.msgtext.Size = new System.Drawing.Size(489, 32);
            this.msgtext.TabIndex = 3;
            this.msgtext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(359, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(208, 7);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 1;
            this.progressBar.Value = 100;
            // 
            // timerSplash
            // 
            this.timerSplash.Tick += new System.EventHandler(this.timerSplash_Tick);
            // 
            // timerCheck
            // 
            this.timerCheck.Interval = 1000;
            this.timerCheck.Tick += new System.EventHandler(this.timerCheck_Tick);
            // 
            // timerAnimation
            // 
            this.timerAnimation.Interval = 1000;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // dlgSchutzSplash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1010, 638);
            this.ControlBox = false;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.webBrowser);
            this.Cursor = System.Windows.Forms.Cursors.No;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "dlgSchutzSplash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.dlgSchutzSplash_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dlgSchutzSplash_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dlgSchutzSplash_KeyDown);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLink)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Timer timerSplash;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label msgtext;
        private System.Windows.Forms.Label labelBill;
        private System.Windows.Forms.Label labelIkunden;
        private System.Windows.Forms.Label labelMP;
        private System.Windows.Forms.Label labelSpiele;
        private System.Windows.Forms.Label labelVOIP;
        private System.Windows.Forms.Label labelSMS;
        private System.Windows.Forms.Label txtBill;
        private System.Windows.Forms.Label txtSpiele;
        private System.Windows.Forms.Label txtVoip;
        private System.Windows.Forms.Label txtSms;
        private System.Windows.Forms.Label txtMP;
        private System.Windows.Forms.Label txtIKunden;
        private System.Windows.Forms.Timer timerCheck;
        private System.Windows.Forms.Timer timerAnimation;
        private System.Windows.Forms.PictureBox picLink;
        private System.Windows.Forms.Button regfor;
    }
}