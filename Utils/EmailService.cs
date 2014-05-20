using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mail;

namespace kioskplus
{
    class EmailService : MailMessage
    {
        private string fromName;
		private string smtpServerName;
		private string smtpUserName;
		private string smtpUserPassword;
		private int smtpServerPort;
		private bool smtpSSL;

        public EmailService()
		{
			fromName = string.Empty;
			smtpServerName = string.Empty;
			smtpUserName = string.Empty;
			smtpUserPassword = string.Empty;
			smtpServerPort = 25;
			smtpSSL = false;
		}

        /// <summary>
        /// The display name that will appear in the recipient mail client
        /// </summary>
        public string FromName
        {
            set
            {
                fromName = value;
            }
            get
            {
                return fromName;
            }
        }

        /// <summary>
        /// SMTP server (name or IP address)
        /// </summary>
        public string SMTPServerName
        {
            set
            {
                smtpServerName = value;
            }
            get
            {
                return smtpServerName;
            }
        }

        /// <summary>
        /// Username needed for a SMTP server that requires authentication
        /// </summary>
        public string SMTPUserName
        {
            set
            {
                smtpUserName = value;
            }
            get
            {
                return smtpUserName;
            }
        }

        /// <summary>
        /// Password needed for a SMTP server that requires authentication
        /// </summary>
        public string SMTPUserPassword
        {
            set
            {
                smtpUserPassword = value;
            }
            get
            {
                return smtpUserPassword;
            }
        }

        /// <summary>
        /// SMTP server port (default 25)
        /// </summary>
        public int SMTPServerPort
        {
            set
            {
                smtpServerPort = value;
            }
            get
            {
                return smtpServerPort;
            }
        }

        /// <summary>
        /// If SMTP server requires SSL
        /// </summary>
        public bool SMTPSSL
        {
            set
            {
                smtpSSL = value;
            }
            get
            {
                return smtpSSL;
            }
        }

        public void Send()
        {
            if (smtpServerName.Length == 0)
            {
                throw new Exception("SMTP Server not specified");
            }

            if (fromName.Length > 0)
            {
                this.Headers.Add("From", string.Format("{0} <{1}>", FromName, From));
            }

            // set SMTP server name
            this.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"] = smtpServerName;
            // set SMTP server port
            this.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = smtpServerPort;
            this.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;

            if (smtpUserName.Length > 0 && smtpUserPassword.Length > 0)
            {
                this.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = 1;

                // set SMTP username
                this.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = smtpUserName;
                // set SMTP user password
                this.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = smtpUserPassword;
            }

            // ssl if needed
            if (smtpSSL)
            {
                this.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");
            }

            SmtpMail.SmtpServer = smtpServerName;
            SmtpMail.Send(this);
        }





    }
}
