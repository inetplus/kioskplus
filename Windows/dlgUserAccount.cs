using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using kioskplus.Utils;

namespace kioskplus.Windows
{
    public partial class dlgUserAccount : dlgBase
    {
        private String userPWD = String.Empty;
        private String isPwd = String.Empty;
       
        private char[] ch = {'!','"','§','$','%','&','/','(',')','[',']','=',
                             '?','+','*',';',':','>','<','\\',
                             '"','|','{','}','~','#', ' '};

        private char[] che = {'!','"','§','$','%','&','/','(',')','[',']','=',
                             '?','+','*',';',':',',','>','<','\\',
                             '"','|','{','}','~','#',' '};

        private char[] czahlen = {'0','1','2','3','4','5','6','7','8','9', ' ', '/','-' };


        private char[] hobbies = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };

        public dlgUserAccount()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            timerAccount.Enabled = false;
        }


        private void dlgUserAccount_Load(object sender, EventArgs e)
        {
            
            if (Program.gnvSplash.iKundenID > 0 && Program.gnvSplash.ibStatusPing)
            {
                httpcontrol localHttp = new httpcontrol();

                StringBuilder strQuery = new StringBuilder();
                strQuery.Append("action=uis/info");
                strQuery.Append("&2=").Append(Program.gnvSplash.isKundenID);

               localHttp.RunAction(strQuery.ToString(),Program.gnvSplash.iActServerID,false);

               if (!localHttp.GetParameterValue("2").Equals(""))
               {
                   kdNr.Text = localHttp.GetParameterValue("1");
                   kdNr2.Text = kdNr.Text;
                   kdNr3.Text = kdNr.Text;

                   nickname.Text = localHttp.GetParameterValue("2");
                   nickname2.Text = nickname.Text;

                   userPWD = localHttp.GetParameterValue("3");
                   // 4 = Standort - ID

                   vorname.Text = localHttp.GetParameterValue("5");
                   nachname.Text = localHttp.GetParameterValue("6");
                   strasse.Text = localHttp.GetParameterValue("7");
                   plz.Text = localHttp.GetParameterValue("8");
                   ort.Text = localHttp.GetParameterValue("9");
                   telefon.Text = localHttp.GetParameterValue("10");
                   telefax.Text = localHttp.GetParameterValue("11");
                   mobil.Text = localHttp.GetParameterValue("12");
                   emailadr.Text = localHttp.GetParameterValue("13");
                   guthaben.Text = localHttp.GetParameterValue("14");
                   hobbies = localHttp.GetParameterValue("47").ToCharArray();

                   try
                   {
                       if (hobbies != null && hobbies.Length > 9)
                           for (int i = 1; i < 11; i++)
                               ((CheckBox)panel4.Controls["cb" + i.ToString("00")]).Checked = Convert.ToBoolean(Int32.Parse(hobbies[i - 1].ToString()));
                   }
                   catch {  }
                  

                   nickname.Enabled = false;
                   nickname2.Enabled = false;
               }

               if (!Program.gnvSplash.ibStatusPing)
               {
                   btnBack.Enabled = false;
                   btnEnde.Enabled = false;
                   btnForward.Enabled = false;

               }
               localHttp = null;
           }
           else
           {
            
               groupBox1.Visible = false;

               if (Program.gnvSplash.ibKennzKunde && Program.gnvSplash.iKundenID <=0)
               {
                   if (Program.gnvSplash.DialogNewUser != null)
                       guthaben.Text = Program.gnvSplash.DialogNewUser.GetOnlineZeitAsInt().ToString();
               }
               else
               {
                   this.btnBack.Enabled = false;
                   this.btnForward.Enabled = false;
                   this.btnEnde.Enabled = false;

                   this.errorMsg.Text = Program.getMyLangString("deaktivKontoAnlegen");
                   this.errorMsg.ForeColor = Color.Red;
               }

           }

            panel1.MouseDown += new MouseEventHandler(dlgBase_MouseDown);
            panel1.MouseMove += new MouseEventHandler(dlgBase_MouseMove);
            panel1.MouseUp += new MouseEventHandler(dlgBase_MouseUp);

            panel2.MouseDown += new MouseEventHandler(dlgBase_MouseDown);
            panel2.MouseMove += new MouseEventHandler(dlgBase_MouseMove);
            panel2.MouseUp += new MouseEventHandler(dlgBase_MouseUp);

            panel3.MouseDown += new MouseEventHandler(dlgBase_MouseDown);
            panel3.MouseMove += new MouseEventHandler(dlgBase_MouseMove);
            panel3.MouseUp += new MouseEventHandler(dlgBase_MouseUp);

            panel4.MouseDown += new MouseEventHandler(dlgBase_MouseDown);
            panel4.MouseMove += new MouseEventHandler(dlgBase_MouseMove);
            panel4.MouseUp += new MouseEventHandler(dlgBase_MouseUp);

        }


        private void btnForward_Click(object sender, EventArgs e)
        {
            errorMsg.ForeColor = Color.White;
            if (panel1.Visible)
            {   // Plausibilität überprüfen
                if (!checkEnterPage1())
                {
                    errorMsg.ForeColor = Color.Red;
                    return;
                }
                this.Update();
                panel2.Visible = true;
                this.Update();
                panel1.Visible = false;
                btnBack.Enabled = true;
                nickname2.Text = nickname.Text;
            }
            else if (panel2.Visible)
            {
                if (!checkEnterPage2())
                {
                    errorMsg.ForeColor = Color.Red;
                    return;
                }
                this.Update();
                panel4.Visible = true;
                this.Update();
                panel2.Visible = false;
                this.Update();
               // btnForward.Enabled = false;
              //  btnEnde.Enabled = true;
            }
            else if (panel4.Visible)
            {
                if (!checkEnterPage4())
                {
                    errorMsg.ForeColor = Color.Red;
                    return;
                }
                this.Update();
                panel4.Visible = false;
                this.Update();
                panel3.Visible = true;
                this.Update();
                btnForward.Enabled = false;
                btnEnde.Enabled = true;
            }
             
        }

        private Boolean checkEnterPage1()
        {
            if (vorname.Text.Trim().Equals(""))
            {
                errorMsg.Text = Program.getMyLangString("errorVorname");
                vorname.Focus();
                return false;
            }

            if (checkInValidCharacter(vorname))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"),lvorname.Text);
                vorname.Focus();
                return false;
            }

            if (checkInValidCharacterZahlen(vorname))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lvorname.Text);
                vorname.Focus();
                return false;
            }

            if (nachname.Text.Trim().Equals(""))
            {
                errorMsg.Text = Program.getMyLangString("errorName");
                nachname.Focus();
                return false;
            }

            if (checkInValidCharacter(nachname))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lnachname.Text);
                nachname.Focus();
                return false;
            }
            if (checkInValidCharacterZahlen(nachname))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lnachname.Text);
                nachname.Focus();
                return false;
            }

            if (nickname.Text.Trim().Equals(""))
            {
                errorMsg.Text = Program.getMyLangString("errorNickname");
                nickname.Focus();
                return false;
            }

            if (checkInValidCharacter(nickname))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lnick.Text);
                nickname.Focus();
                return false;
            }

            if (emailadr.Text.Trim().Equals(""))
            {
                errorMsg.Text = Program.getMyLangString("errorEmailAdress");
                emailadr.Focus();
                return false;
            }
            
            if (emailadr.Text.IndexOf("@") == -1 || emailadr.Text.Substring(emailadr.Text.Length -1, 1).Equals("@"))
            {
                errorMsg.Text = Program.getMyLangString("errorEmailAdress");
                emailadr.Focus();
                return false;
            }
            if (emailadr.Text.IndexOfAny(che) > -1  )
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lemail.Text);
                emailadr.Focus();
                return false;
            }

            errorMsg.Text = "";
            return true;
        }


        private Boolean checkEnterPage2()
        {
            if (checkInValidCharacter(strasse))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lstr.Text);
                strasse.Focus();
                return false;
            }
            if (checkInValidCharacter(plz))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lplz.Text);
                plz.Focus();
                return false;
            }
            if (checkInValidCharacter(ort))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lplz.Text);
                ort.Focus();
                return false;
            }
            if (checkInValidCharacter(telefon))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), ltelefon.Text);
                telefon.Focus();
                return false;
            }
            if (telefon.Text.Length > 0 && !checkInValidCharacterZahlen(telefon))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), ltelefon.Text);
                telefon.Focus();
                return false;
            }

            if (checkInValidCharacter(telefax))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lfax.Text);
                telefax.Focus();
                return false;
            }
            if (telefax.Text.Length > 0 && !checkInValidCharacterZahlen(telefax))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lfax.Text);
                telefax.Focus();
                return false;
            }
            if (checkInValidCharacter(mobil))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lmobil.Text);
                mobil.Focus();
                return false;
            }
            if (mobil.Text.Length > 0 && !checkInValidCharacterZahlen(mobil))
            {
                errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lmobil.Text);
                mobil.Focus();
                return false;
            }
            errorMsg.Text = "";
            return true;
        }

        private Boolean checkEnterPage3()
        {
            errorMsg.Text = "";
            errorMsg.ForeColor = Color.White;
            if (altesPWD.Text.Trim().Equals(""))
            {
                errorMsg.Text = Program.getMyLangString("errorPwdEingabe");
                errorMsg.ForeColor = Color.Red;
                nickname.Focus();
                return false;
            }

            if (Program.gnvSplash.iKundenID <=0)
                if (checkInValidCharacter(altesPWD))
                {
                    errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), laltpwd.Text);
                    errorMsg.ForeColor = Color.Red;
                    altesPWD.Focus();
                    return false;
                }
            
            return true;
        }

        private Boolean checkEnterPage4()
        {
            errorMsg.Text = "";
            errorMsg.ForeColor = Color.White;

            String temp = new String(hobbies);
            if (temp.Equals("000000000000000"))
            {
                errorMsg.Text = Program.getMyLangString("errorhobby");
                errorMsg.ForeColor = Color.Red;
                return false;
            }
            return true;
        }


        private Boolean checkInValidCharacter(TextBox arg)
        {
            int tmp;
            tmp = arg.Text.IndexOfAny(ch);
            if (tmp != -1)
                return true;

            return false;
        }

        private Boolean checkInValidCharacterZahlen(TextBox arg)
        {
            int tmp;
            tmp = arg.Text.IndexOfAny(czahlen);
            if (tmp != -1)
                return true;

            return false;
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            if (panel2.Visible)
            {
                this.Update();
                panel1.Visible = true;
                this.Update();
                panel2.Visible = false;
                btnForward.Enabled = true;
                btnBack.Enabled = false;
            }
            else if (panel3.Visible)
            {
                this.Update();
                panel3.Visible = false;
                panel4.Visible = true;
                this.Update();
                btnForward.Enabled = true;
                btnEnde.Enabled = false;
            }
            else if (panel4.Visible)
            {
                this.Update();
                panel4.Visible = false;
                panel2.Visible = true;
                this.Update();
                btnForward.Enabled = true;
                btnEnde.Enabled = false;
            }
        }

        private void btnEnde_Click(object sender, EventArgs e)
        {
            errorMsg.ForeColor = Color.White;
            if (btnEnde.Text.Equals(Program.getMyLangString("btnBeenden")))
            {
                Close();
                return;
            }

            if (!checkEnterPage1())
            {
                panel1.Visible = true;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                errorMsg.ForeColor = Color.Red;
                return;
            }
            if (!checkEnterPage2())
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;
                panel4.Visible = false;
                errorMsg.ForeColor = Color.Red;
                return;
            }
           
            if (!checkEnterPage4())
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = true;
                errorMsg.ForeColor = Color.Red;
                return;
            }

            if (!checkEnterPage3())
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = true;
                panel4.Visible = false;
                errorMsg.ForeColor = Color.Red;
                return;
            }


            if (Program.gnvSplash.iKundenID > 0)
            {
                if (altesPWD.Equals(""))
                {
                    errorMsg.Text = Program.getMyLangString("errorPwdEingabe");
                    errorMsg.ForeColor = Color.Red;
                    return;
                }
                if (!altesPWD.Text.Equals(userPWD))
                {
                    errorMsg.Text = Program.getMyLangString("errorFalschPwd");
                    errorMsg.ForeColor = Color.Red;
                    return;
                }
                isPwd = altesPWD.Text;

                if (isnewPWD.Checked)
                {
                    if (npwd.Text.Trim().Equals(""))
                    {
                        errorMsg.Text = Program.getMyLangString("errorNewPwdEingabe");
                        errorMsg.ForeColor = Color.Red;
                        return;
                    }

                    if (checkInValidCharacter(npwd))
                    {
                        errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), lnpwd.Text);
                        errorMsg.ForeColor = Color.Red;
                        npwd.Focus();
                        return;
                    }

                    if (!npwd.Text.Trim().Equals(wpwd.Text.Trim()))
                    {
                        errorMsg.Text = Program.getMyLangString("errorPwdAgain");
                        errorMsg.ForeColor = Color.Red;
                        return;
                    }
                    isPwd = npwd.Text;
                } // Ende IsNewPwd
            }
            else
            {

                if (checkInValidCharacter(altesPWD))
                {
                    errorMsg.Text = String.Format(Program.getMyLangString("errorInValid"), laltpwd.Text);
                    errorMsg.ForeColor = Color.Red;
                    altesPWD.Focus();
                    return;
                }
                isPwd = altesPWD.Text;
            }
            if (isPwd.Trim().Equals(""))
            {
                errorMsg.Text = Program.getMyLangString("errorFalschPwd");
                errorMsg.ForeColor = Color.Red;
                return;
            }

            StringBuilder strQuery = new StringBuilder();
            String tmp = String.Empty;
            if (Program.gnvSplash.iKundenID>0)
                tmp = Program.gnvSplash.iKundenID.ToString();

            strQuery.Append("action=uiss")
                    .Append("&1=").Append(tmp)
                    .Append("&2=").Append(nickname.Text)
                    .Append("&3=").Append(isPwd)
                    .Append("&4=").Append(Program.gnvSplash.iStandortID.ToString())
                    .Append("&5=").Append(vorname.Text)
                    .Append("&6=").Append(nachname.Text)
                    .Append("&7=").Append(strasse.Text)
                    .Append("&8=").Append(plz.Text)
                    .Append("&9=").Append(ort.Text)
                    .Append("&10=").Append(telefon.Text)
                    .Append("&11=").Append(telefax.Text)
                    .Append("&12=").Append(mobil.Text)
                    .Append("&13=").Append(emailadr.Text)
                    .Append("&14=").Append(guthaben.Text)
                    .Append("&15=").Append(DateTime.Now.ToString("yyyy-MM-dd"))
                    .Append("&16=").Append(DateTime.Now.ToString("HH:mm:ss"))
                    .Append("&23=").Append(Program.gnvSplash.iAufstellplatzID.ToString())
                    .Append("&25=").Append(Program.gnvSplash.iTerminalID.ToString())
                    .Append("&46=").Append("")
                    .Append("&47=").Append(new String(hobbies));


            httpcontrol localHttp = new httpcontrol();
            localHttp.RunAction(strQuery.ToString(),Program.gnvSplash.iActServerID,false);

            if (Program.gnvSplash.ibStatusPing)
            {

                if (localHttp.GetParameterValue("26").Equals(""))
                {
                    errorMsg.Text = Program.getMyLangString("successSaveKD");
                    this.btnEnde.Text = Program.getMyLangString("btnBeenden");

                    this.btnBack.Visible = false;
                    this.btnCancel.Visible = false;
                    this.btnForward.Visible = false;


                    if (Program.gnvSplash.iKundenID <= 0)
                    {
                        try
                        {
                            Program.gnvSplash.iKundenID = Int32.Parse(localHttp.GetParameterValue("1"));
                            Program.gnvSplash.isKundenID = this.nickname.Text;
                            Program.gnvSplash.DialogNewUser.SetUser(this.nickname.Text);
                            kdNr.Text = Program.gnvSplash.iKundenID.ToString();
                            kdNr2.Text = kdNr.Text;
                            kdNr3.Text = kdNr.Text;

                        }
                        catch
                        { }
                    }
                }
                else
                {
                    errorMsg.Text = localHttp.GetParameterValue("26");
                }
            }
            else
            {
                errorMsg.ForeColor = Color.Red;
                errorMsg.Text = Program.getMyLangString("deaktivKontoAnlegen");
            }



            localHttp = null;
        }

        private void isnewPWD_CheckedChanged(object sender, EventArgs e)
        {
      
                this.npwd.Enabled = this.isnewPWD.Checked;
                this.wpwd.Enabled = this.isnewPWD.Checked; 
        }

        private void cb01_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[0] = Convert.ToChar(Convert.ToInt32(cb01.Checked).ToString());
        }

        private void cb02_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[1] = Convert.ToChar(Convert.ToInt32(cb02.Checked).ToString());
        }

        private void cb03_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[2] = Convert.ToChar(Convert.ToInt32(cb03.Checked).ToString());
        }

        private void cb04_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[3] = Convert.ToChar(Convert.ToInt32(cb04.Checked).ToString());
        }

        private void cb05_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[4] = Convert.ToChar(Convert.ToInt32(cb05.Checked).ToString());
        }

        private void cb06_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[5] = Convert.ToChar(Convert.ToInt32(cb06.Checked).ToString());
        }

        private void cb07_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[6] = Convert.ToChar(Convert.ToInt32(cb07.Checked).ToString());
        }

        private void cb08_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[7] = Convert.ToChar(Convert.ToInt32(cb08.Checked).ToString());
        }

        private void cb09_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[8] = Convert.ToChar(Convert.ToInt32(cb09.Checked).ToString());
        }

        private void cb10_CheckedChanged(object sender, EventArgs e)
        {
            hobbies[9] = Convert.ToChar(Convert.ToInt32(cb10.Checked).ToString());
        }

    }
}