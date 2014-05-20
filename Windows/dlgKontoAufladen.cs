using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using kioskplus.Utils;

namespace kioskplus.Windows
{
    public partial class dlgKontoAufladen : dlgBase
    {
        private char[] ch = {'!','"','§','$','%','&','/','(',')','[',']','=',
                             '?','+','*','-','_',';',':',',','.','>','<','\\',
                             '"','|','{','}','~','#',' '};

        public dlgKontoAufladen()
        {
            InitializeComponent();
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            int liMinuten = 0;
            String lsUser = String.Empty, lsPwd = String.Empty, lsKennz = String.Empty;
            double ldCredit = 0;
            int liGuthaben = 0, llCouponId = 0;
            lMsg.ForeColor = Color.White;
            lMsg.Text = Program.getMyLangString("aufladenMsg");
            if (Program.gnvSplash.DialogNewUser != null)
            {
                try
                {
                    liMinuten = Int32.Parse(Program.gnvSplash.DialogNewUser.GetRestMinutenAsString());
                }
                catch // (Exception ex)
                {
                    liMinuten = 0;
                }

                if (liMinuten < 3)
                {
                    lMsg.Text = String.Format(Program.getMyLangString("errorAufladenNoMinute"), 3);
                    lMsg.ForeColor = Color.Red;
                    return;
                }
            }
            else
                return;

            lsUser = txtID.Text.Trim();
            lsPwd = txtPWD.Text.Trim();

            if (lsUser.IndexOfAny(ch) > -1 || lsPwd.IndexOfAny(ch) > -1)
            {
                lMsg.Text = Program.getMyLangString("errorAufladenInvalidCharachter");
                lMsg.ForeColor = Color.Red;
                return;
            }

            lMsg.Text = Program.getMyLangString("aufladenMsg");
            if (lsPwd.Equals("") || lsUser.Equals(""))
            {
                lMsg.Text = Program.getMyLangString("errorAufladenEingabe");
                lMsg.ForeColor = Color.Red;
                return;
            }

            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("action=user")
                    .Append("&2=").Append(lsUser)
                    .Append("&3=").Append(lsPwd)
                    .Append("&4=").Append(Program.gnvSplash.iStandortID.ToString())
                    .Append("&5=").Append("auf")
                    .Append("&6=").Append(Program.gnvSplash.iAufstellplatzID.ToString())
                    .Append("&7=").Append(Program.gnvSplash.iKundenID.ToString())
                    .Append("&8=").Append(Program.gnvSplash.iTerminalID.ToString());

            httpcontrol local = new httpcontrol();
            local.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);

            if (!local.GetParameterValue("24").Equals(""))
            {
                lMsg.Text = local.GetParameterValue("24");
                lMsg.ForeColor = Color.Red;
                txtPWD.Text = "";
                return;
            }

            try
            {
                ldCredit = Double.Parse(local.GetParameterValue("27"),Program.gnvDecFormat);
            }
            catch // (Exception ex)
            { ldCredit = -1; }

            try
            {
                liGuthaben = Int32.Parse(local.GetParameterValue("14")) ;
            }
            catch //(Exception ex)
            { liGuthaben = 0; }

            // -1 heisst es handelt sich um ein Konto welches aufgeladen wurde, nicht um eine Coupon-Verlängerung
            if (ldCredit == -1 && Program.gnvSplash.iKundenID <= 0)
            {
                lsKennz = "99";
                ldCredit = 0;
                Program.gnvSplash.iKundenID = Int32.Parse(local.GetParameterValue("1"));
                Program.gnvSplash.isKundenID = local.GetParameterValue("2");
                Program.gnvSplash.isAnzNameFull = local.GetParameterValue("5") + " " + local.GetParameterValue("6");

                if (Program.gnvSplash.DialogNewUser != null)
                {
                    Program.gnvSplash.DialogNewUser.SetUser(Program.gnvSplash.isKundenID);
                }
            }
            else
            {
                lsKennz = "c";
                try
                {
                    llCouponId = Int32.Parse(local.GetParameterValue("1"));
                }
                catch // (Exception ex)
                {
                }

            }
            String lsTmp = String.Empty;
            if (liGuthaben > 0)
            {
                inetDateTime ldt = new inetDateTime();
                lsTmp = ldt.GetZeitFromMinuten(liGuthaben);
            }

         
            Program.gnvSplash.SetUmsatz(ldCredit, ref lsTmp, lsKennz, llCouponId,"");

            try
            {
                lsUser = null;
                lsTmp = null;
                lsPwd = null;
                lsKennz = null;
                strQuery = null;
                local = null;
            }
            catch { }

            Close();
        }

        private void dlgKontoAufladen_Load(object sender, EventArgs e)
        {
            if (Program.gnvSplash.iKundenID != 0)
            {
                lid.Visible = false;
                lcouponid.Visible = true;
            }
            
            Boolean lbTmp = Program.gnvSplash.ibAufladen;
            if (lbTmp)
                lbTmp = Program.gnvSplash.ibStatusPing;

            
            if (!lbTmp)
            {
                this.btnOK.Enabled = false;
                this.lMsg.Text = Program.getMyLangString("deaktivKontoAufladen");
                this.lMsg.ForeColor = Color.Red;
            }


            txtID.Focus();
        }

        private void btnEnde_Click(object sender, EventArgs e)
        {

        }
      
    }
}