using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using kioskplus.Utils;
using kioskplus.Properties;

namespace kioskplus.Windows
{
    public partial class dlgPhone : Form
    {
        // Licence Key
        private String isLicenceKey = "VAXVOIP.COM-77P116P227P240P89P22P13P238P189P75P105P205P90P12P109P118P62P254P176P89P149P62P186P224P-20.INETPLUS.DE";
        
        // SIP Login Daten
        String isSipServer1 = String.Empty;
        String isSipServer2 = String.Empty;
        String isSipUser = String.Empty, isSipPwd = String.Empty;
        int iDefaultLandVorwahl = 0,iPortVon = 0, iPortBis =0;
        inetDateTime instanceDateTime = new inetDateTime();
        //
        int iCallRecord  =0;
        String isCallNumber = String.Empty;

        // Kosten
        double idcKostenEUR = 0, idcKostenWaehrung = 0, idcKostenEK = 0;
        double idcAbbuchenMinuten = 0;
        String isAbbuchenMinuten = String.Empty;

        Boolean ibTelefonieErlaubt = true;
        Boolean ibAnruf = false;
        Boolean ibAnrufNummer = false;
        // Start und berechnen
        DateTime iDTStart, iDTBerechnen;

        Boolean ibRegisterSuccess = false;
        // New
        Boolean ibEchoCancel = false;
        Boolean ibGSM610 = false;
        Boolean ibilBC = false;
        Boolean ibG711a = false;
        Boolean ibG711u = false;
        
        // new for mousemove event
        PictureBox picBox;
        
        // Standard
        private Boolean ibStartMove = false;
        private Point mausPoint = new Point();

        Boolean ibSlider = false;
        Point mausSlieder = new Point();

        // 
        Bitmap[] p = { kioskplus.sip_0, kioskplus.sip_1, kioskplus.sip_2, kioskplus.sip_3, kioskplus.sip_4, 
                       kioskplus.sip_5, kioskplus.sip_6, kioskplus.sip_7, kioskplus.sip_8, kioskplus.sip_9,
                       kioskplus.sip_del, kioskplus.sip_c, kioskplus.sip_yes, kioskplus.sip_no};

        string errMeldung = string.Empty;

        ipControl myIPControl = null;

        // 0 = normal
        // 1 = usb
        // 2 = beides
        public int kennzGestartedUSB = 0;

        public dlgPhone()
        {
            InitializeComponent();

            zahl0.Image = null;
            zahl1.Image = null;
            zahl2.Image = null;
            zahl3.Image = null;
            zahl4.Image = null;
            zahl5.Image = null;
            zahl6.Image = null;
            zahl7.Image = null;
            zahl8.Image = null;
            zahl9.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Boolean initPhone()
        {
            String myIP, fromURI;
            int listenPort;
            Boolean retValue = true;
            Boolean bTemp;

            fromURI = isSipUser + " <sip:" + isSipUser + "@" + isSipServer1 + ">";

            myIP = inetSIP.GetMyIP();

            if (iPortBis <= iPortVon)
            {
                iPortVon = 5070;
                iPortBis = 7000;
            }

            for (listenPort = iPortVon; listenPort < iPortBis; listenPort++)
            {
                bTemp = inetSIP.Initialize(false, myIP, listenPort, fromURI, isSipServer1, isSipServer2, isSipUser, isSipPwd, true, 1);
                if (!bTemp)
                {
                    if (inetSIP.GetVaxObjectError() != 11)
                    {
                        GetErrorAsString();
                        retValue = false;
                    }
                }
                else
                    return retValue;
            }

            if (listenPort >= iPortBis)
            {
                lText.Text = Program.getMyLangString("sipCommError");
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAudioDevices()
        {
            Boolean lbFound = false;
            int index = 0;

            try
            {
                for (index = 0; index < inetSIP.GetAudioInDevTotal(); index++)
                {
                    comboInput.Items.Add(inetSIP.GetAudioInDevName(index));
                    lbFound = true;
                }

                if (!lbFound)
                    comboInput.Items.Add("Default Device");

                comboInput.SelectedIndex = 0;
            }
            catch (Exception e) { Console.WriteLine("phone:"+e.Message); }

            lbFound = false;
            try
            {
                for (index = 0; index < inetSIP.GetAudioOutDevTotal(); index++)
                {
                    comboOutput.Items.Add(inetSIP.GetAudioOutDevName(index));
                    lbFound = true;
                }

                if (!lbFound)
                    comboOutput.Items.Add("Default Device");

                comboOutput.SelectedIndex = 0;
                inetSIP.SetSpkVolume(120);
                inetSIP.SetMicVolume(120);
            }
            catch { Console.WriteLine("SB-Error:"); }
          
        }

        delegate void CancelCallBack();
        public void CancelCall()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CancelCallBack(CancelCall), new object[] { });
                return;
            }
            else
            {

                if (iCallRecord == 0)
                {
                    ClickTasteC();
                    isCallNumber = "";
                    lText.Text = "";
                }
                lText.Text = Program.getMyLangString("sipCallingEnde");

                iCallRecord = 0;

                if (ibAnruf && !inetSIP.Disconnect(1))
                    GetErrorAsString();

                inetSIP.UnInitialize();
                ibAnruf = false;
                timerText.Enabled = true;
            }
        }

        private void ClickTasteC()
        {
            if (ibAnruf == false)
            {
                lTelefonNumber.Text = "";
                lText.Text = "";
                lZeit.Text = "";
                lKosten.Text = "";
            }
        }

        private void ClickTasteDel()
        {
            String lsTmp = String.Empty;

            if (!ibAnruf)
            {
                if (!lTelefonNumber.Text.Equals(""))
                    lTelefonNumber.Text = lTelefonNumber.Text.Substring(0, lTelefonNumber.Text.Length - 1);
                lZeit.Text = "";
            }
        }

        delegate Boolean GespraechStartenCallBack(Boolean usbstart);
        public Boolean GespraechStarten(Boolean usbstart)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new GespraechStartenCallBack(GespraechStarten), new object[] { usbstart });
                return true;
            }
            else
            {
                if (!ibAnruf)
                {
                    if (!GetPreis() && Program.gnvSplash.ibStatusPing)
                    {
                        return false;
                    }

                    if (!Program.gnvSplash.ibStatusPing)
                    {
                        lText.Text = Program.getMyLangString("sipNoAllow");
                        lText.ForeColor = Color.Red;
                        return false;
                    }

                    ibAnruf = true;
                    ibAnrufNummer = true;

                    iDTStart = DateTime.Now;
                    iDTStart = iDTStart.AddHours(-iDTStart.Hour);
                    iDTStart = iDTStart.AddMinutes(-iDTStart.Minute);
                    iDTStart = iDTStart.AddSeconds(-iDTStart.Second);
                    iDTStart = iDTStart.AddMilliseconds(-iDTStart.Millisecond);
                    iDTBerechnen = iDTStart;

                    lZeit.Text = iDTStart.ToString(inetConstants.isTimeFormat);
                    lText.Text = Program.getMyLangString("sipBitteWarten");
                    comboOutput.SelectedIndex = 0;
                    comboInput.SelectedIndex = 0;
                    if (usbstart)
                    {
                        for (int i = 0; i<comboInput.Items.Count; i++)
                            if (comboInput.Items[i].ToString().ToLower().Contains("usb"))
                                comboInput.SelectedIndex = i;
                        for (int i = 0; i<comboOutput.Items.Count; i++)
                            if (comboOutput.Items[i].ToString().ToLower().Contains("usb"))
                                comboOutput.SelectedIndex = i;
                    }


                    RegisterSIP();
                }
                return true;
            }
            

        }

        private void SetAbbuchen()
        {
            String lsUmsOnline;
            int liAktMinuten;

            if (Program.gnvSplash.idcStundenPreis <=0)
            {
                CancelCall();
                lText.Text = Program.getMyLangString("sipErrorNoDefinePreis");
                return;
            }

            // Internet verbindung überprüfen!!?

            if (idcAbbuchenMinuten <= 0)
            {
                CancelCall();
                lText.Text = Program.getMyLangString("sipErrorNoGuthaben");
                return;
            }

            // Aktuellen Restminuten prüfen, ob diese ausreichen zum Starten
            try{
                liAktMinuten = Int32.Parse(Program.gnvSplash.DialogNewUser.GetRestMinutenAsString());
            }catch
            {
                liAktMinuten = 0;
            }

            if (liAktMinuten <= (2 * idcAbbuchenMinuten + 3))
            {
                lText.Text = Program.getMyLangString("sipErrorGuthaben");
            }
            else
                lText.Text = String.Empty;

            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("action=vsp")
                    .Append("&1=").Append(iCallRecord.ToString())
                    .Append("&2=").Append(Program.gnvSplash.iStandortID.ToString())
                    .Append("&3=").Append(Program.gnvSplash.iKundenID.ToString())
                    .Append("&4=").Append(isCallNumber)
                    .Append("&5=").Append(idcKostenEUR.ToString())
                    .Append("&13=").Append(idcKostenWaehrung.ToString())
                    .Append("&14=").Append(idcKostenEK.ToString())
                    .Append("&6=").Append(idcAbbuchenMinuten.ToString())
                    .Append("&7=").Append(isAbbuchenMinuten)
                    .Append("&12=").Append(Program.gnvSplash.iAufstellplatzID.ToString());
                   
            liAktMinuten = liAktMinuten - (int)idcAbbuchenMinuten;
            instanceDateTime = null;
            instanceDateTime = new inetDateTime();
            lsUmsOnline = instanceDateTime.GetZeitFromMinuten(liAktMinuten);

            if (Program.gnvSplash.DialogNewUser != null)
                Program.gnvSplash.DialogNewUser.SetNewOnlineTime((int)idcAbbuchenMinuten);

            strQuery.Append("&8=").Append(lsUmsOnline)
                    .Append("&9=").Append(Program.gnvSplash.DialogNewUser.GetEndeDate())
                    .Append("&10=").Append(Program.gnvSplash.DialogNewUser.GetEndeTime())
                    .Append("&11=").Append(Program.gnvSplash.iTerminalID.ToString());

            httpcontrol local = new httpcontrol();
            local.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);
            if (local.GetParameterValue("1").Equals("-1"))
                local.SetParameterValue("1","-1");

            if (!local.GetParameterValue("1").Equals(""))
            {
                try{
                    iCallRecord = Int32.Parse(local.GetParameterValue("1"));
               }catch
               {iCallRecord = -1;}

                if (iCallRecord <0)
                {
                    CancelCall();
                    idcAbbuchenMinuten = -1;
                    lText.Text = local.GetParameterValue("9");
                }
            }

        }

        private Boolean OpenLines()
        {
            String myIP = String.Empty;
            Boolean retValue;
            int PortRTF, LineNo, ErrorCount;

            myIP = inetSIP.GetMyIP();
            PortRTF = 7000;
            LineNo = 0;
            ErrorCount = 0;
            retValue = true;

            while(LineNo <1)
            {
                if (!inetSIP.OpenLine(LineNo,false,myIP,PortRTF))
                {
                    if (inetSIP.GetVaxObjectError() == 11)
                    {
                        ErrorCount ++;
                        LineNo --;
                    }else{
                        GetErrorAsString();
                        return false;
                    }

                }
                LineNo ++;
                PortRTF = PortRTF +2; //It is importent to increament RTP Listen Port by 2

                if (ErrorCount >= 1000)
                {
                    MessageBox.Show(Program.getMyLangString("sipErrorPort"));
                    return false;
                }
            }
            return true;
        }

        private Boolean RegisterSIP()
        {
            inetSIP.SetLicenceKey(isLicenceKey);

            if (!initPhone())
                return false;

            if (!OpenLines())
                return false;

            if (false == inetSIP.RegisterToProxy(3600))
            {
                GetErrorAsString();
                return false;
            }

            ibEchoCancel = true;
            ibGSM610 = true;
            ibilBC = true;
            ibG711a = true;
            ibG711u = true;

            inetSIP.EnableKeepAlive(10);
            return true;
        }


        private Boolean GetPreis()
        {
            int liAktMinuten = 0;
            String lsNumber = String.Empty;

            lText.ForeColor = Color.White;

            try
            {
                if (Program.gnvSplash.DialogNewUser != null);
                liAktMinuten = Int32.Parse(Program.gnvSplash.DialogNewUser.GetRestMinutenAsString());
            }
            catch
            {}

            if (idcAbbuchenMinuten < 0 && kennzGestartedUSB == 0)
            {
                lText.Text = Program.getMyLangString("sipKontoGuthaben");
                lText.ForeColor = Color.Red;
                return false;
            }

            if ( (liAktMinuten <= idcAbbuchenMinuten || liAktMinuten <= 10) && kennzGestartedUSB == 0 )
            {
                lText.Text = Program.getMyLangString("sipZeitGuthaben");
                lText.ForeColor = Color.Red;
                return false;
            }

            if (lTelefonNumber.Text.Trim().Equals(""))
            {
                lText.Text = Program.getMyLangString("sipErrorTelNr");
                lText.ForeColor = Color.Red;
                return false;
            }

            lsNumber = lTelefonNumber.Text;
            int tmp = 0;
            try
            {
                tmp = Int32.Parse(lsNumber.Substring(0,2));
            }catch
            {}

            if (tmp > 0)
            {
                lsNumber = "00" + Program.gnvSplash.isLandVorwahl + lsNumber.Substring(1);
            }
            else if (!lsNumber.Substring(0, 1).Equals("0"))
            {
                lText.Text = Program.getMyLangString("sipErrorInvalidNr");
            }

            isCallNumber = lsNumber;

            StringBuilder strQuery = new StringBuilder();

            strQuery.Append("action=vps")
                    .Append("&1=").Append(Program.gnvSplash.iStandortID.ToString())
                    .Append("&2=").Append(Program.gnvSplash.iAufstellplatzID.ToString())
                    .Append("&3=").Append(lsNumber);


            httpcontrol local = new httpcontrol();
            local.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);

            try
            {
                if (local.GetParameterValue("1").Equals(""))
                {
                    local.SetParameterValue("1", "0");
                }
            }
            catch
            { }

            try
            {
                idcKostenEUR = Double.Parse(local.GetParameterValue("1"), Program.gnvDecFormat);

            }catch 
            {
                idcKostenEUR = 0;
            }


            if (idcKostenEUR <=0 )
            {
                String ls = local.GetParameterValue("3");
                lText.Text = ls;
                idcAbbuchenMinuten = -1;
                if (ls.Equals(""))
                {
                    lText.Text = Program.getMyLangString("sipErrorNoDefinePreis");
                }
                return false;
            }


            try
            {
                 if (local.GetParameterValue("2").Equals(""))
                     local.SetParameterValue("2","1");

                 idcKostenWaehrung = Double.Parse(local.GetParameterValue("2"), Program.gnvDecFormat);
            }
            catch 
            {
                idcKostenWaehrung = 1;
            }

            idcAbbuchenMinuten = ( 60 / (Program.gnvSplash.idcStundenPreis * 100)) * idcKostenWaehrung;
            idcAbbuchenMinuten = Math.Round(idcAbbuchenMinuten + 0.49,0);
            if (idcAbbuchenMinuten < 1)
                idcAbbuchenMinuten = 1;

            isAbbuchenMinuten = instanceDateTime.GetZeitFromMinuten((int)idcAbbuchenMinuten);

            if ((liAktMinuten <= idcAbbuchenMinuten || liAktMinuten <= 10) && kennzGestartedUSB == 0)
            {
                lText.Text = Program.getMyLangString("sipZeitGuthaben");
                return false;
            }

            lText.Text = local.GetParameterValue("3");

            if (idcKostenWaehrung > 0 )
            {
                lKosten.Visible = true;
                String lTmp = idcKostenWaehrung.ToString();
                if (lTmp.Length > 5)
                    lTmp = lTmp.Substring(0, 5);

                lKosten.Text = lTmp + " " + Program.gnvSplash.isWaehrungCent + "/ min("+idcAbbuchenMinuten.ToString()+ ")";
            }
            else
            {
                lText.Text = Program.getMyLangString("sipErrorNoDefineWaehrung");
                return false;
            }
            try
            {
                iDefaultLandVorwahl = Int32.Parse(local.GetParameterValue("5"));
            }
            catch 
            { iDefaultLandVorwahl = 0; }
            isSipServer1 = local.GetParameterValue("6");
            isSipServer2 = local.GetParameterValue("7");
            isSipUser = local.GetParameterValue("8");
            isSipPwd = local.GetParameterValue("9");

            /***TEST*
                isSipServer1 = "sip.siptraffic.com";
                isSipServer2 = "siptraffic.com";
                isSipUser = "inetplus1";
                isSipPwd = "sipnetplus";
             **/

            try
            {
                iPortVon = Int32.Parse(local.GetParameterValue("10"));
            }catch
            {iPortVon = 0;}

            try
            {
                iPortBis = Int32.Parse(local.GetParameterValue("11"));
            }catch
            {iPortBis = 0;}

            local = null;

            if (isSipServer1.Equals(""))
            {
                lText.Text = Program.getMyLangString("sipErrorDaten");
                return false;
            }
           
            return true;
        }

        private String GetErrorAsString()
        {
            int 		ErrorCode;
            String		sErrorText = String.Empty;
     
            ErrorCode =  inetSIP.GetVaxObjectError();

            sErrorText = Program.getMyLangString("sipError" + ErrorCode.ToString());           
            sErrorText = sErrorText + "("+ErrorCode.ToString()+")";
            return sErrorText;
        }

        private void dlgPhone_MouseDown(object sender, MouseEventArgs e)
        {
            mausPoint.X = e.X;
            mausPoint.Y = e.Y;
            ibStartMove = true;
        }

        private void dlgPhone_MouseMove(object sender, MouseEventArgs e)
        {
            if (ibStartMove)
            {
                Point currentPos = Control.MousePosition;
                currentPos.X -= mausPoint.X;
                currentPos.Y -= mausPoint.Y;
                this.Location = currentPos;
                return;
            }

            PictureBox pc = null;
			if (sender is PictureBox) // 20100130
            {
            try
            {
                pc = (PictureBox)sender;
            }
            catch 
            { pc = null; }
}

            if (pc != null)
            {
                if (picBox != null)
                    if (pc.Name.Equals(picBox.Name))
                        return;

                if (pc.Tag.Equals(""))
                    return;

                if (picBox != null)
                    picBox.Image = null;

                picBox = pc;
                int tmp = 0;
                try
                {
                    tmp = Int32.Parse(picBox.Tag.ToString());
                }
                catch
                { }

                try
                {
                    picBox.Image = p[tmp];
                }
                catch
                { }
            }
            else
            {
                if (picBox != null)
                    picBox.Image = null;
            }
        }

        private void dlgPhone_MouseUp(object sender, MouseEventArgs e)
        {
            ibStartMove = false;
        }

        private void dlgPhone_Load(object sender, EventArgs e)
        {
            ibTelefonieErlaubt = Program.gnvSplash.ibKennzVoip;

            if (ibTelefonieErlaubt)
                ibTelefonieErlaubt = Program.gnvSplash.ibStatusPing;

            if (inetSIP == null)
                ibTelefonieErlaubt = false;

            if (!ibTelefonieErlaubt)
            {
                ibTelefonieErlaubt = false;
                lText.Text = Program.getMyLangString("sipNoAllow");
                lText.ForeColor = Color.Red;
            }
            else
            {
              /*  try
                {
                    Program.gnvSplash.gnvMainInetplus.stopPhoneControl();

                    myIPControl = new ipControl();
                    myIPControl.Checked += new ipControl.checkData(myIPControl_Checked);
                    myIPControl.setStart();
                }
                catch { }*/
                UpdateAudioDevices();

               /*
                if (myIPControl != null)
                {
                    if (!myIPControl.gestarted)
                        myIPControl = null;
                    else
                        lText.Text = "";
                }*/
            }
        }

        private void dlgPhone_Click(object sender, EventArgs e)
        {

            if (!ibTelefonieErlaubt)
                return;

            PictureBox pc = null;

            try
            {
				 if (sender is PictureBox) // 20100130
                	pc = (PictureBox)sender;
            }
            catch 
            { pc = null; }

            try
            {
                if (pc != null)
                {
                    int tmp = 0;
                    tmp = Int32.Parse(pc.Tag.ToString());
                    if (tmp < 10)
                    {
                        if (ibAnrufNummer)
                        {
                            ibAnrufNummer = false;
                            lTelefonNumber.Text = "";
                            lText.Text = "";
                            lKosten.Text = "";
                        }
                        lTelefonNumber.Text += tmp.ToString();
                        if (myIPControl != null)
                        {
                            myIPControl.TextToMitte = lTelefonNumber.Text;
                            myIPControl.setTonePlay();
                            myIPControl.setDrawString();
                        }
                    }
                    switch (pc.Tag.ToString())
                    {
                        case "12": // Yes
                            {
                                GespraechStarten(false);
                                break;
                            }
                        case "13": // No
                            {
                                CancelCall();
                                break;
                            }
                        case "11": // C
                            {
                                ClickTasteC();
                                break;
                            }
                        case "10": // Del
                            {
                                ClickTasteDel();
                                break;
                            }
                    }
                }
            }
            catch
            { }
        }

        private void anfasserSound_MouseDown(object sender, MouseEventArgs e)
        {
            mausSlieder.X = Control.MousePosition.X;
            mausSlieder.Y = Control.MousePosition.Y;
            ibSlider = true;
        }

        private void anfasserMicro_MouseDown(object sender, MouseEventArgs e)
        {
            mausSlieder.X = Control.MousePosition.X;
            mausSlieder.Y = Control.MousePosition.Y;
            ibSlider = true;
        }

        private void anfasserMicro_MouseUp(object sender, MouseEventArgs e)
        {
            ibSlider = false;
        }

        private void anfasserSound_MouseUp(object sender, MouseEventArgs e)
        {
            ibSlider = false;
        }

        private void anfasserSound_MouseMove(object sender, MouseEventArgs e)
        {
            if (ibSlider)
            {
                Point currentPos = Control.MousePosition;
                currentPos.Y = anfasserSound.Location.Y;

                currentPos.X = currentPos.X - mausSlieder.X + anfasserSound.Location.X;
                mausSlieder = Control.MousePosition;

                if ((currentPos.X + anfasserSound.Width )>= (reglerSound.Location.X + reglerSound.Width))
                    currentPos.X = (reglerSound.Location.X + reglerSound.Width - anfasserSound.Width);

                if (currentPos.X <= (reglerSound.Location.X))
                    currentPos.X = reglerSound.Location.X;

                anfasserSound.Location = currentPos;

                int t = currentPos.X - reglerMicrophone.Location.X;
                t = ((t-92) * 255 / 37);
                inetSIP.SetSpkVolume(t);
            }
        }


        private void anfasserMicro_MouseMove(object sender, MouseEventArgs e)
        {            
            if (ibSlider)
            {
                Point currentPos = Control.MousePosition;
                currentPos.Y = anfasserMicro.Location.Y;

                currentPos.X = currentPos.X - mausSlieder.X + anfasserMicro.Location.X;
                mausSlieder = Control.MousePosition;

                if ((currentPos.X + anfasserMicro.Width) >= (reglerMicrophone.Location.X + reglerMicrophone.Width))
                    currentPos.X = (reglerMicrophone.Location.X + reglerMicrophone.Width - anfasserMicro.Width);

                if (currentPos.X <= (reglerMicrophone.Location.X))
                    currentPos.X = reglerMicrophone.Location.X;

                anfasserMicro.Location = currentPos;
                int t = currentPos.X - reglerMicrophone.Location.X;
                t = (t * 255 / 37);
                inetSIP.SetMicVolume(t);
            }
        }

        private void pBoxSpeaker_Click(object sender, EventArgs e)
        {
            Point p = new Point();
            p.Y = anfasserSound.Location.Y;

            if (pBoxSpeaker.Tag.ToString().Equals("1"))
            {
                pBoxSpeaker.Image = kioskplus.sip_speaker_mute;
                pBoxSpeaker.Tag = 2;
                inetSIP.MuteSpk(true);
                p.X = reglerSound.Location.X;
                anfasserSound.Location = p;
            }
            else
            {
                pBoxSpeaker.Image = kioskplus.sip_speaker_on;
                pBoxSpeaker.Tag = 1;
                inetSIP.MuteSpk(false);
                p.X = anfasserSound.Location.X + 15;
                anfasserSound.Location= p;
            }
           
        }

        private void pBoxMicrophone_Click(object sender, EventArgs e)
        {
            Point p = new Point();
            p.Y = anfasserMicro.Location.Y;

            try
            {
                if (pBoxMicrophone.Tag.ToString().Equals("1"))
                {
                    pBoxMicrophone.Image = kioskplus.sip_microphone_mute;
                    pBoxMicrophone.Tag = 2;
                    inetSIP.MuteMic(true);
                    p.X = reglerMicrophone.Location.X;
                    anfasserMicro.Location = p;
                }
                else
                {
                    pBoxMicrophone.Image = kioskplus.sip_microphone_on;
                    pBoxMicrophone.Tag = 1;
                    inetSIP.MuteMic(false);
                    p.X = anfasserMicro.Location.X + 15;
                    anfasserMicro.Location = p;
                }
            }
            catch { }
        }

       private void inetSIP_OnConnecting(object sender, AxVAXSIPUSERAGENTOCXLib._DVaxSIPUserAgentOCXEvents_OnConnectingEvent e)
        {
            ibAnruf = true;
            ibAnrufNummer = true;
        }

        private void inetSIP_OnDisconnectCall(object sender, AxVAXSIPUSERAGENTOCXLib._DVaxSIPUserAgentOCXEvents_OnDisconnectCallEvent e)
        {
            if (ibAnruf)
            {
                CancelCall();
                ibAnruf = false;
                lText.Text = Program.getMyLangString("sipCallingEnde");
                timerText.Enabled = true;
            }
        }

        private void inetSIP_OnFailToRegister(object sender, EventArgs e)
        {
            ibRegisterSuccess = false;
        }

        private void inetSIP_OnRequestFailureResponse(object sender, AxVAXSIPUSERAGENTOCXLib._DVaxSIPUserAgentOCXEvents_OnRequestFailureResponseEvent e)
        {
            if (ibAnruf)
            {
                ibAnruf = false;
                lText.Text = Program.getMyLangString("sipCallingEnde");
            }
        }

        private void inetSIP_OnSuccessToConnect(object sender, AxVAXSIPUSERAGENTOCXLib._DVaxSIPUserAgentOCXEvents_OnSuccessToConnectEvent e)
        {
          ibAnruf = true;
          ibAnrufNummer = true;
          iDTStart = iDTStart.AddSeconds(1);

          // Timer starten
          timerTelefon.Enabled = true;
          lText.Text = Program.getMyLangString("sipStartCalling");
        }

        private void inetSIP_OnSuccessToRegister(object sender, EventArgs e)
        {
            if (ibRegisterSuccess)
                return;

            String ToURI;
            int lineNo;

            ToURI = "sip:" + isCallNumber + "@" + isSipServer1;
            lineNo = 0;

            lText.Text = Program.getMyLangString("sipCallingNumber");
            if (inetSIP.Connect(lineNo, ToURI, comboInput.SelectedIndex, comboOutput.SelectedIndex))
                ibRegisterSuccess = true;
        }

        private void inetSIP_OnSuccessToUnRegister(object sender, EventArgs e)
        {
            ibRegisterSuccess = false;
        }

     private void timerTelefon_Tick(object sender, EventArgs e)
        {
            if (ibAnruf)
            {
                iDTStart = iDTStart.AddSeconds(1);
                lZeit.Text = iDTStart.ToString("HH:mm:ss");

                TimeSpan ts = iDTStart.Subtract(iDTBerechnen);
                if (ts.Seconds >= 1)
                {
                    iDTBerechnen = iDTBerechnen.AddMinutes(1);
                    SetAbbuchen();
                }
            }
            else
            {
                timerTelefon.Enabled = false;
                lText.Text = "";
            }
        }

        private void dlgPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (!ibTelefonieErlaubt)
                return;

            if (lTelefonNumber.Text.Length > 20)
                return;

            int zahl = -1;
            switch (e.KeyData)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    {
                        zahl = 0;
                        break;
                    }
                case Keys.D1:
                case Keys.NumPad1:
                    {
                        zahl = 1;
                        break;
                    }
                case Keys.D2:
                case Keys.NumPad2:
                    {
                        zahl = 2;
                        break;
                    }
                case Keys.D3:
                case Keys.NumPad3:
                    {
                        zahl = 3;
                        break;
                    }
                case Keys.D4:
                case Keys.NumPad4:
                    {
                        zahl = 4;
                        break;
                    }
                case Keys.D5:
                case Keys.NumPad5:
                    {
                        zahl = 5;
                        break;
                    }
                case Keys.D6:
                case Keys.NumPad6:
                    {
                        zahl = 6;
                        break;
                    }
                case Keys.D7:
                case Keys.NumPad7:
                    {
                        zahl = 7;
                        break;
                    }
                case Keys.D8:
                case Keys.NumPad8:
                    {
                        zahl = 8;
                        break;
                    }
                case Keys.D9:
                case Keys.NumPad9:
                    {
                        zahl = 9;
                        break;
                    }
                case Keys.Delete:
                    {
                        lTelefonNumber.Text = "";
                        break;
                    }
                case Keys.Back:
                    {
                        if (lTelefonNumber.Text.Length > 0 )
                            lTelefonNumber.Text = lTelefonNumber.Text.Substring(0, lTelefonNumber.Text.Length - 1);
                        break;
                    }
                case Keys.Enter:
                case Keys.Execute:
                    {
                        GespraechStarten(false);
                        break;
                    }
                    
            }
            
            if (zahl != -1)
                lTelefonNumber.Text += zahl.ToString();

            if (myIPControl != null)
            {
                myIPControl.TextToMitte = lTelefonNumber.Text;
                myIPControl.setDrawString();
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (linkLabel1.Tag.ToString().Equals("0"))
                {
                    panelEinstellung.Visible = true;
                    linkLabel1.Tag = "1";
                }
                else
                {
                    linkLabel1.Tag = "0";
                    panelEinstellung.Visible = false;
                    //this.Width -= (this.comboInput.Width + 10);
                }
            }
            catch 
            { }
        }

        private void timerText_Tick(object sender, EventArgs e)
        {
            lText.Text = "";
            timerText.Enabled = false;
        }


        /*** Constanten ***/
        public const int WM_USER = 0x400;
        public const int WM_HID_DEV_ADDED = WM_USER + 0x1000;
        public const int WM_HID_DEV_REMOVED = WM_USER + 0x1001;
        public const int WM_HID_KEY_DOWN = WM_USER + 0x1002;
        public const int WM_HID_KEY_UP = WM_USER + 0x1003;
        public const int WM_HID_VOLUME_DOWN = WM_USER + 0x1004;
        public const int WM_HID_VOLUME_UP = WM_USER + 0x1005;
        public const int WM_HID_PLAYBACK_MUTE = WM_USER + 0x1006;
        public const int WM_HID_RECORD_MUTE = WM_USER + 0x1007;
        public const int WM_HID_GPIO = WM_USER + 0x1008;
        public const int WM_HID_GENERIC = WM_USER + 0x1009;
        public const int WM_HID_BUFFER_TONE = WM_USER + 0x100A;
        public const int WM_HID_W1388_ISR = WM_USER + 0x100B;
        public const int WM_HID_OTHERCMD_REP = WM_USER + 0x100D;
        public const int WM_HID_VENDORCMD_REP = WM_USER + 0x100E;
        public const int WM_DEVICECHANGE = 0x0219;

        void myIPControl_Checked(int wMessage, int wParam, int lParam)
        {
            switch (wMessage)
            {
                case WM_HID_DEV_ADDED:
                    Console.WriteLine("Added");
                    break;
                case WM_HID_DEV_REMOVED:
                    Console.WriteLine("Removed");
                    break;
                case WM_HID_KEY_DOWN:
                    {
                        myIPControl.setTonePlay();
                        Console.WriteLine("Down:" + wParam.ToString() + " / " + lParam.ToString());
                        switch (wParam)
                        {
                            case 1: // 1
                            case 2: // 2
                            case 3: // 3
                                myIPControl.TextToMitte += wParam.ToString();
                                break;
                            case 6: // 4
                            case 7: // 5
                            case 8: // 6
                                myIPControl.TextToMitte += (wParam - 2).ToString();
                                break;
                            case 11: // 7
                            case 12: // 8
                            case 13: // 9
                                myIPControl.TextToMitte += (wParam - 4).ToString();
                                break;
                            case 18: // 0
                                myIPControl.TextToMitte += "0";
                                break;
                            case 15: // c delete
                                myIPControl.TextToMitte = "";
                                break;
                            case 17: // Mute

                                break;
                            case 14: // anrufen
                                GespraechStarten(true);
                                break;
                            case 20: // Anruf beenden
                                CancelCall();
                                break;
                            case 10: // einzelne löschen
                                if (myIPControl.TextToMitte.Length > 0)
                                    myIPControl.TextToMitte =  myIPControl.TextToMitte.Substring(0, myIPControl.TextToMitte.Length - 1);
                                break;
                        }
                        setText();

                        break;
                    }
                case WM_HID_KEY_UP:
                    {
                        Console.WriteLine("Up");
                        break;
                    }
                case WM_DEVICECHANGE:
                    Console.WriteLine("Device change");
                    myIPControl.setHandleChanged(wParam, lParam);
                    break;
                case WM_HID_VOLUME_DOWN:
                    Console.WriteLine("Volume down");
                    break;
                case WM_HID_VOLUME_UP:
                    Console.WriteLine("Volume up");
                    break;
                case WM_HID_PLAYBACK_MUTE:
                case WM_HID_RECORD_MUTE:
                case WM_HID_GPIO:
                case WM_HID_GENERIC:
                    // these messages not used now
                    break;
                case WM_HID_BUFFER_TONE:
                    myIPControl.setTonePlay();// TonePlay(aMelody);
                    break;
                case WM_HID_W1388_ISR:
                    // W1388ISR((unsigned char *)wParam);
                    break;
                case WM_HID_OTHERCMD_REP:
                    break;
                case WM_HID_VENDORCMD_REP:
                    break;
            }
            
            if (myIPControl != null)
                myIPControl.setDrawString();
            
        }


        delegate void setSoundBack(int volume);
        public void setSound(int volume)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new setSoundBack(setSound), new object[] { volume});
                return;
            }
            else
            {
                inetSIP.SetSpkVolume(volume);
            }
        }


        delegate void setTextCallBack();
        private void setText()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new setTextCallBack(setText), new object[] { });
                return;
            }
            else
            {
                lTelefonNumber.Text = myIPControl.TextToMitte;
            }
        }

        delegate void setTelefonTextCallBack(String nr);
        public void setTelefonText(String nr)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new setTelefonTextCallBack(setTelefonText), new object[] {nr });
                return;
            }
            else
            {
                lTelefonNumber.Text = nr;
            }
        }


        private void dlgPhone_FormClosed(object sender, FormClosedEventArgs e)
        {

            try
            {
                Program.gnvSplash.gnvMainInetplus.ShowPhone(false);
            }
            catch { }
           /* try
            {
                if (myIPControl != null)
                {
                    
                    myIPControl.TextToMitte = "";
                    myIPControl.setDrawString();
                    myIPControl.setStop();
                    myIPControl = null;
                }

                Program.gnvSplash.gnvMainInetplus.startPhoneControl();

            }
            catch { }*/
        }

    }
}