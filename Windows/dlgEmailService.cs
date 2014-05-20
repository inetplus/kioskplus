using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kioskplus.Windows
{
    public partial class dlgEmailService : dlgBase
    {
        private String isDateiName = String.Empty;

        public dlgEmailService()
        {
            InitializeComponent();
        }

        public void SetMyFoto(String argFileName)
        {
            isDateiName = argFileName;
            datei.Text = "...." + isDateiName.Substring(isDateiName.LastIndexOf("\\"));
            btnAnhang.Enabled = false;
        }

        private void dlgEmailService_Load(object sender, EventArgs e)
        {
            absender.Text = Program.gnvSplash.isKundenID;
            openFD.Title = Program.getMyLangString("dlgFileTitle");
            openFD.Filter = Program.getMyLangString("dlgFileExtension");

            if (Program.gnvSplash.isEmailUser.Equals(""))
            {
                Program.gnvSplash.ReadEmailData();
            }
            if (Program.gnvSplash.isEmailUser.Equals(""))
            {
                // EMail versand ist deaktiv
                btnNew.Enabled = false;
                btnSenden.Enabled = false;
                btnAnhang.Enabled = false;
                msg.ForeColor = Color.Red;
                msg.Text = Program.getMyLangString("errorEmailService");
            }
           
        }

        private void btnNew_Click(object sender, EventArgs e)
        { 
            this.betreff.Text = "";
            this.empfaenger.Text = "";
            this.datei.Text = "";
            this.richText.Text = "";
            this.msg.Text = "";
            isDateiName = "";
        }

        private void openFD_FileOk(object sender, CancelEventArgs e)
        {
            isDateiName = openFD.FileName;

            datei.Text = "...." + isDateiName.Substring(isDateiName.LastIndexOf("\\"));
        }

        private void btnSenden_Click(object sender, EventArgs e)
        {
            Boolean lbError = false;
            
            msg.ForeColor = Color.White;
            msg.Text = "";

            if (this.empfaenger.Text.IndexOf("@") <= 0 ||
                (this.empfaenger.Text.Substring(this.empfaenger.Text.IndexOf("@") + 1).Length <= 0))
            {
                msg.Text = Program.getMyLangString("errorEmailAdress");
                msg.ForeColor = Color.Red;
                return;
            }

            if (this.richText.Text.Trim().Equals(""))
            {
                msg.Text = Program.getMyLangString("allgEingabeError");
                msg.ForeColor = Color.Red;
                return;
            }

            this.Cursor = Cursors.WaitCursor;
           
            EmailService localEmail = null;
            localEmail = new EmailService();
            localEmail.SMTPServerName = Program.gnvSplash.isEmailServer;
            localEmail.SMTPUserName = Program.gnvSplash.isEmailUser;
            localEmail.SMTPUserPassword = Program.gnvSplash.isEmailPwd;
            localEmail.Priority = System.Web.Mail.MailPriority.Normal;
            localEmail.BodyFormat = System.Web.Mail.MailFormat.Html;
            localEmail.From = Program.gnvSplash.isEmailAdr;

            this.btnSenden.Enabled = false;
            this.richText.Enabled = false;
            this.betreff.Enabled = false;
            this.absender.Enabled = false;
            this.btnBeenden.Enabled = false;
            this.btnAnhang.Enabled = false;
            this.btnNew.Enabled = false;
            this.empfaenger.Enabled = false;

            msg.Text = Program.getMyLangString("bittewarten");
            
            this.Update();

            // check emailadress
            int standort = Program.gnvSplash.iStandortID;
            standort = 100000 + standort;

            StringBuilder   strQuery = new StringBuilder();
            strQuery.Append(this.richText.Text.Replace("\n","<br>"))
                    .Append("<br><br><br><hr><strong><font size=\"2\">")
                    .Append("Diese Email wurde gesendet von:</font></strong>")
                    .Append("<br><font size=\"2\">Standort:").Append(standort.ToString()).Append("</font>")
                    .Append("<br><font size=\"2\">Stellplatz: A").Append(Program.gnvSplash.iAufstellplatzID.ToString()).Append("</font>")
                    .Append("<br><font size=\"2\">Terminal: ").Append(Program.gnvSplash.iTerminalID.ToString()).Append("</font>")
                    .Append("<br><font size=\"2\">Kunde:").Append(Program.gnvSplash.iKundenID.ToString()).Append("</font>")
                    .Append("<br><font size=\"2\">Name: ").Append(Program.gnvSplash.isKundenID).Append("</font>")
                    .Append(Program.gnvSplash.isEmailFooter);

            if (!isDateiName.Equals(""))
            {
                System.Web.Mail.MailAttachment lAttachment;
                lAttachment = new System.Web.Mail.MailAttachment(isDateiName);
                localEmail.Attachments.Add(lAttachment);
            }

            localEmail.FromName = Program.gnvSplash.isEmailAnzeige + "("+  this.absender.Text + ")";
            localEmail.Subject = this.betreff.Text;
            localEmail.To = this.empfaenger.Text;
            localEmail.Body = strQuery.ToString();

            try
            {
                localEmail.Send();
            }
            catch (Exception ex)
            {
                lbError = true;
                msg.ForeColor = Color.Red;
                msg.Text = Program.getMyLangString("errorEmail") + Environment.NewLine + ex.Message;
            }

            if (!lbError)
                msg.Text = Program.getMyLangString("successEmail");

            localEmail = null;

            this.Cursor = Cursors.Default;

            this.btnSenden.Enabled = true;
            this.richText.Enabled = true;
            this.betreff.Enabled = true;
            this.absender.Enabled = true;
            this.btnBeenden.Enabled = true;
            this.btnAnhang.Enabled = true;
            this.btnNew.Enabled = true;
            this.empfaenger.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            datei.Text = "";
            openFD.ShowDialog();
        }
    }
}