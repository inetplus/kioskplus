using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace kioskplus.Windows
{

   
    public partial class dlgSmsService : dlgBase
    {
        private httpcontrol insHttp;
        //
        private double idSmsText = 0, idSmsTextAusland = 0;
        private int freeSmsAnzahl;
        private int iSmsAnzahl = 0, iSmsAusland = 0;
        private int iKunSmsAnzahl = 0, iKunSmsAusland = 0;
        private char[] ch = {'§','$','&','#'};

        private String isLand = String.Empty;
        private String isVorwahl = String.Empty;
        private String isNumber = String.Empty;

        private int iAbbuchenMinuten = 0;
            
        public dlgSmsService()
        {
            InitializeComponent();
        }

        private void comboLand_SelectedIndexChanged(object sender, EventArgs e)
        {
            String lsTmp;
            lsTmp = (String)this.comboLand.SelectedItem;
            lsTmp = lsTmp.Substring(lsTmp.LastIndexOf("("));

            try
            {
                switch (lsTmp)
                {
                    case "(+49)":
                        {
                            comboVorwahl.Items.Clear();
                            comboVorwahl.Items.Add("151");
                            comboVorwahl.Items.Add("152");
                            comboVorwahl.Items.Add("159");
                            comboVorwahl.Items.Add("160");
                            comboVorwahl.Items.Add("162");
                            comboVorwahl.Items.Add("163");
                            comboVorwahl.Items.Add("170");
                            comboVorwahl.Items.Add("171");
                            comboVorwahl.Items.Add("172");
                            comboVorwahl.Items.Add("173");
                            comboVorwahl.Items.Add("174");
                            comboVorwahl.Items.Add("175");
                            comboVorwahl.Items.Add("176");
                            comboVorwahl.Items.Add("177");
                            comboVorwahl.Items.Add("178");
                            comboVorwahl.Items.Add("179");
                            break;
                        }
                    case "(+":
                        {
                            break;
                        }
                    default:
                        {
                            combonummer.Text = "";
                            comboVorwahl.Text = "";
                            comboVorwahl.Items.Clear();
                            break;
                        }
                }

                lsTmp = lsTmp.Substring(1, lsTmp.Length - 2);
                if (lsTmp.Equals(""))
                    telnr.Text = "00";
                else
                {
                    isLand = lsTmp.Substring(1);
                    telnr.Text = lsTmp;
                    SetKostenSMS();
                }
                isLand = telnr.Text.Substring(1);
            }
            catch 
            { }
            try
            {
                lsTmp = null;
            }
            catch { }
        }

        private void dlgSmsService_Load(object sender, EventArgs e)
        {
            comboLand.SelectedIndex = 0; // De

            if (!Program.gnvSplash.ibKennzSMS)
            {
                msg.Text = Program.getMyLangString("errorSmsDeaktiv");
                msg.ForeColor = Color.Red;
                btnNew.Enabled = false;
                btnSend.Enabled = false;
                return;
            }
            insHttp = new httpcontrol();

            GetSMSGuthaben();
            SetKostenSMS();
        }

        private void GetSMSGuthaben()
        {
            double ldRegKunKonto = 0, ldIkunKonto = 0;

            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("action=smss")
                    .Append("&1=").Append(Program.gnvSplash.iAufstellplatzID.ToString())
                    .Append("&2=").Append(Program.gnvSplash.iKundenID.ToString());


            insHttp.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);

            try
            {
                if (insHttp.GetParameterValue("1").Equals(""))
                    insHttp.SetParameterValue("1","0");

                if (insHttp.GetParameterValue("2").Equals(""))
                    insHttp.SetParameterValue("2", "0");

                if (insHttp.GetParameterValue("3").Equals(""))
                    insHttp.SetParameterValue("3", "0");

                if (insHttp.GetParameterValue("4").Equals(""))
                    insHttp.SetParameterValue("4", "0");

                if (insHttp.GetParameterValue("5").Equals(""))
                    insHttp.SetParameterValue("5", "0");

                if (insHttp.GetParameterValue("6").Equals(""))
                    insHttp.SetParameterValue("6", "0");

                if (insHttp.GetParameterValue("7").Equals(""))
                    insHttp.SetParameterValue("7", "0");

                try
                {
                    idSmsText = Double.Parse(insHttp.GetParameterValue("1"), Program.gnvDecFormat); // Sms Inland
                }
                catch (Exception ex)
                { idSmsText = 0; }

                try
                {
                    idSmsTextAusland = Double.Parse(insHttp.GetParameterValue("2"), Program.gnvDecFormat); // Sms Ausland
                }
                catch (Exception ex)
                {idSmsTextAusland = 0; }

               
                try
                {
                    ldRegKunKonto = Double.Parse(insHttp.GetParameterValue("5"), Program.gnvDecFormat);
                }
                catch (Exception ex)
                {
                    ldRegKunKonto = 0;
                }

                try
                {
                    ldIkunKonto = Double.Parse(insHttp.GetParameterValue("6"), Program.gnvDecFormat);
                }
                catch (Exception ex)
                {
                    ldIkunKonto = 0;
                }

                freeSmsAnzahl = Int32.Parse(insHttp.GetParameterValue("7"));
            }
            catch (Exception ex)
            {
            }


            if (ldIkunKonto > 0)
            {
                if (idSmsText > 0)
                    iKunSmsAnzahl = (int)Math.Truncate((ldIkunKonto / idSmsText));
                
                if (idSmsTextAusland > 0)
                    iKunSmsAusland = (int)Math.Truncate((ldIkunKonto / idSmsTextAusland));

            }

            if (ldRegKunKonto > 0)
            {
                if (idSmsText > 0)
                    iSmsAnzahl = (int)Math.Truncate((ldRegKunKonto / idSmsText));

                if (idSmsTextAusland > 0)
                    iSmsAusland = (int)Math.Truncate((ldRegKunKonto / idSmsTextAusland));

            }
            else
            {
                msg.Text = Program.getMyLangString("errorSmsDeaktiv");
                msg.ForeColor = Color.Red;
                btnNew.Enabled = false;
                btnSend.Enabled = false;
            }

            GetText();

        }

        private void GetText()
        {

            allgText.Text = String.Format(Program.getMyLangString("smsKonto"), iSmsAnzahl.ToString());

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                int guthaben = 0;
                msg.Text = "";
                msg.ForeColor = Color.White;
                if (!checkEnter())
                {
                    msg.ForeColor = Color.Red;
                    return;
                }
                // Überprüfe den Guthaben
                try
                {
                    if (Program.gnvSplash.DialogNewUser != null)
                        guthaben = Int32.Parse(Program.gnvSplash.DialogNewUser.GetRestMinutenAsString());
                }
                catch (Exception ex)
                {
                    guthaben = 0;
                }

                if (guthaben <= (iAbbuchenMinuten + 5))
                {
                    msg.Text = Program.getMyLangString("errorSMSZeitGuthaben");
                    msg.ForeColor = Color.Red;
                    return;
                }

                // Internet exist überprüfen

                msg.Text = Program.getMyLangString("bittewartenSMS");

                btnSend.Enabled = false;
                btnBeenden.Enabled = false;
                btnNew.Enabled = false;
                richTextSms.Enabled = false;
                comboLand.Enabled = false;
                combonummer.Enabled = false;
                comboVorwahl.Enabled = false;

                SendSMS();

                GetSMSGuthaben();
                GetText();
            }
            catch
            { }

            if (Program.gnvSplash.ibStatusPing)
            {
                btnSend.Enabled = true;
                btnNew.Enabled = true;
                richTextSms.Enabled = true;
                comboLand.Enabled = true;
                combonummer.Enabled = true;
                comboVorwahl.Enabled = true;
            }
            btnBeenden.Enabled = true;

        }


        private void SendSMS()
        {
            String lsEmpfaenger = String.Empty;
            String lsNachricht = String.Empty, lsNachricht2;
            String lsTyp = "5";
            double ldcSmsBetrag = idSmsText;
            int anzahlSMS = 1;
            Boolean lbAktualisieren = false;
            StringBuilder strQuery = new StringBuilder();
            String lsMinuten = String.Empty ;
            String lsRest = String.Empty;
            double d;
            decimal de;

            lsEmpfaenger = telnr.Text;
            lsNachricht = richTextSms.Text.Trim();
            decimal de1 = 160;
            anzahlSMS = lsNachricht.Length + 79;
            de = (anzahlSMS);
            de = de / de1;
            d = (double)Math.Round(de);
           
            anzahlSMS = Int32.Parse(d.ToString());

                
            if (!isLand.Equals("49"))
            {
                lsTyp = "7";
                ldcSmsBetrag = idSmsTextAusland;
            }

            for (int i = 0; i < anzahlSMS; i++)
            {
                if (lsNachricht.Length > 160)
                {
                    lsNachricht2 = lsNachricht.Substring(0, 160);
                    lsNachricht = lsNachricht.Substring(160);
                }
                else
                    lsNachricht2 = lsNachricht;


                if (Program.gnvSplash.iKundenID > 0 && freeSmsAnzahl > 0)
                {
                    freeSmsAnzahl--;
                }
                else
                {
                    lbAktualisieren = true;
                }

                if (Program.gnvSplash.DialogNewUser != null)
                {
                    lsMinuten = Program.gnvSplash.DialogNewUser.GetRestMinutenAsString();
                    lsRest = Program.gnvSplash.DialogNewUser.GetOnlineZeitAsString();
                }

                if (lsMinuten.Equals(""))
                    lsMinuten = "0";

                strQuery.Append("action=smsa")
                        .Append("&8=").Append(Program.gnvSplash.iTerminalID.ToString())
                        .Append("&3=").Append(DateTime.Now.ToString("yyyy-MM-dd"))
                        .Append("&4=").Append(DateTime.Now.ToString("HH:mm:ss"))
                        .Append("&2=").Append(Program.gnvSplash.iStandortID.ToString())
                        .Append("&21=").Append(Program.gnvSplash.iAufstellplatzID.ToString())
                        .Append("&5=").Append(lsNachricht2)
                        .Append("&6=").Append(lsEmpfaenger)
                        .Append("&7=").Append(Program.gnvSplash.iKundenID)
                        .Append("&9=").Append(lsTyp)
                        .Append("&10=").Append(isLand)
                        .Append("&11=").Append(isVorwahl)
                        .Append("&12=").Append(isNumber)
                        .Append("&13=").Append(lsMinuten)
                        .Append("&14=").Append(iAbbuchenMinuten.ToString())
                        .Append("&17=").Append(lsRest)
                        .Append("&22=").Append(ldcSmsBetrag.ToString())
                        .Append("&21=").Append(Program.gnvSplash.iAufstellplatzID.ToString())
                        .Append("&16=").Append(GetZeit(iAbbuchenMinuten));


                if (lbAktualisieren && Program.gnvSplash.ibStatusPing)
                {
                    if (Program.gnvSplash.DialogNewUser != null)
                    {
                        Program.gnvSplash.DialogNewUser.SetNewOnlineTime(iAbbuchenMinuten);
                        strQuery.Append("&18=").Append(Program.gnvSplash.DialogNewUser.GetEndeDate());
                        strQuery.Append("&19=").Append(Program.gnvSplash.DialogNewUser.GetEndeTime());
                        strQuery.Append("&20=").Append(lsRest);
                    }
                }

                httpcontrol local = new httpcontrol();
                local.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);

                if (!local.GetParameterValue("15").Equals("")) // fehler meldung
                {
                    msg.Text = local.GetParameterValue("15");
                }
                else
                    msg.Text = Program.getMyLangString("successSMS");

                // Wenn Fehler auftritt (Server nicht verfügbar oder Internet, soll das Guthaben ...
                if (!Program.gnvSplash.ibStatusPing)
                {
                    Program.gnvSplash.DialogNewUser.SetNewOnlineTime(-iAbbuchenMinuten);
                    
                }

                local = null;
            }

            try
            {
                lsEmpfaenger = null;
                lsNachricht = null;
                lsNachricht2 = null;
                lsTyp = null;
                strQuery = null;
                lsMinuten = null;
                lsRest = null;
            }
            catch { }
        }

        private String GetZeit(int argInteger)
        {
            DateTime ld = DateTime.Now;
            DateTime ld2 = ld;

            ld2 = ld2.AddMinutes(argInteger);
            TimeSpan ts;
            ts = ld2.Subtract(ld);

            int hh, mm;
            String lsReturn;
            hh = ts.Days * 24 + ts.Hours;
            mm = ts.Minutes;

            lsReturn = hh.ToString();
            if (hh < 10)
                lsReturn = "0" + hh.ToString();

            if (mm < 10)
                lsReturn += ":0" + mm.ToString();
            else
                lsReturn += ":"+mm.ToString();

            return lsReturn;

        }


        private void SetKostenSMS()
        {
            if (isLand.Equals(""))
                return;

            if (idSmsText <= 0 || idSmsTextAusland <=0 )
                return;

            int liMinuten = 0;
            double ld = 0;

            switch (isLand)
            {
                case "49":
                    {
                        liMinuten = (int)Math.Truncate(idSmsText * 60 / Program.gnvSplash.idcStundenPreis);
                        ld = idSmsText * 100;
                        break;
                    }
                default:
                    {
                        liMinuten = (int)Math.Truncate(idSmsTextAusland * 60 / Program.gnvSplash.idcStundenPreis);
                        ld = idSmsTextAusland * 100;
                        break;
                    }
            }

            if (liMinuten > 0)
            {
                String[] s ={liMinuten.ToString(),ld.ToString(),Program.gnvSplash.isWaehrungCent };
                smskostet.Text = String.Format(Program.getMyLangString("proSMSKostet"),s);
            }
            iAbbuchenMinuten = liMinuten;
        }


        private Boolean checkEnter()
        {
            try
            {
                String lsTmp;
                lsTmp = (String)this.comboLand.Text;
                if (lsTmp.Equals(""))
                {
                    msg.Text = Program.getMyLangString("errorChooseLand");
                    return false;
                }

                lsTmp = (String)this.comboVorwahl.Text;
                if (lsTmp.Equals(""))
                {
                    msg.Text = Program.getMyLangString("errorVorwahlEingabe");
                    return false;
                }

                if (!Program.gnvSplash.isNumber(lsTmp))
                {
                    msg.Text = Program.getMyLangString("errorVorwahl");
                    return false;
                }

                lsTmp = this.combonummer.Text;
                if (lsTmp.Equals(""))
                {
                    msg.Text = Program.getMyLangString("errorChooseTelNr");
                    return false;
                }
                if (!Program.gnvSplash.isNumber(lsTmp))
                {
                    msg.Text = Program.getMyLangString("errorCheckTelNr");
                    return false;
                }

                lsTmp = (String)this.richTextSms.Text;
                if (lsTmp.Equals(""))
                {
                    msg.Text = Program.getMyLangString("errorNoNachricht");
                    return false;
                }

                if (lsTmp.IndexOfAny(ch) > -1)
                {
                    msg.Text = Program.getMyLangString("errorInvalidNachricht");
                    return false;
                }

            }
            catch (Exception ex)
            { return false;/*MessageBox.Show("fehler: " + ex.Message);*/ }

            return true;
        }

        private void comboVorwahl_TextChanged(object sender, EventArgs e)
        {
            if (!Program.gnvSplash.ibKennzSMS)
                return;

            String lsTmp;
            lsTmp = comboVorwahl.Text;

            if (lsTmp.Length > 5)
            {
                comboVorwahl.Text = lsTmp.Substring(0, 5);
                
                return;
            }
            if (!lsTmp.Equals(""))
            {
                if (Program.gnvSplash.isNumber(lsTmp))
                {
                    telnr.Text = "+" + isLand + lsTmp;
                    isVorwahl = lsTmp;
                }
                else
                {
                    msg.ForeColor = Color.Red;
                    msg.Text = Program.getMyLangString("errorVorwahl");
                }
            }
            else
            {
                telnr.Text = "+" + isLand;
                isVorwahl = "";
                msg.Text = "";
            }
        }

        private void combonummer_TextChanged(object sender, EventArgs e)
        {
            if (!Program.gnvSplash.ibKennzSMS)
                return;
            
            

            String lsTmp;
            lsTmp = combonummer.Text;
            if (!lsTmp.Equals(""))
            {
                if (Program.gnvSplash.isNumber(lsTmp))
                {
                    telnr.Text = "+" + isLand + isVorwahl + lsTmp;
                    isNumber = lsTmp;
                    msg.Text = "";
                }
                else
                {
                    msg.Text = Program.getMyLangString("errorCheckTelNr");
                    msg.ForeColor = Color.Red;
                }
            }
            else
            {
                msg.ForeColor = Color.White;
                msg.Text = String.Empty;
                telnr.Text = "+" + isLand + isVorwahl;
                isNumber = "";
            }
            lsTmp = null;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            combonummer.Text = "";
            comboVorwahl.Text = "";
            richTextSms.Text = "";
            msg.Text = "";
        }

        private void richTextSms_TextChanged(object sender, EventArgs e)
        {
            if (!Program.gnvSplash.ibKennzSMS)
                return;

            double d;
            d = (richTextSms.Text.Length + 159) / 160;
            laenge.Text = richTextSms.Text.Length.ToString() + "/1600";
            if (richTextSms.Text.Length > 0)
                labelSmsAnzahl.Text = ((int)Math.Round(d, 1)).ToString() + " SMS";
            else
                labelSmsAnzahl.Text = "";
        }

        private void dlgSmsService_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                insHttp = null;
                ch = null;
                isLand = null;
                isVorwahl = null;
                isNumber = null;
            }
            catch { }
        }

      
    }


}