using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using kioskplus.Utils;
using System.Threading;

namespace kioskplus.Windows
{
    public partial class dlgSchutzSplash : Form
    {
        private Boolean isCancel = false;
        private Boolean ibNavigate = false;
        //private String[] isKey = {"","","" };
        private List<string> isKeys;
        private int iRed = 0, iGreen = 0, iBlue = 0;
        private char[] c = { '&', '=', '%', '"', '\'' };

        private int zaehler = 0;
        private bool bMinus = false;
        private int iAgentMeldung = 0;


        System.Diagnostics.Process pApplication = null;

        public dlgSchutzSplash()
        {
            InitializeComponent();
        }

        delegate void SetTextCallBack(String abVisible);
        public void SetText(String asText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetTextCallBack(SetText), new object[] { asText });
                return;
            }
            else
                this.msgtext.Text = asText;

        }

        private void LoadForm()
        {
           isKeys = Program.gnvSplash.GetKeys();
           String lsURL = "";

           int lwidth, lheight;
           lwidth = Program.gnvSplash.gnvMainInetplus.displayResolution.iWidth;
           lheight = Program.gnvSplash.gnvMainInetplus.displayResolution.iHeight;
          
            this.Size = new Size(lwidth, lheight);
            this.Left = 0;
            this.Top = 0;
            int x, y;

            SetNavigateSeite();

            if (!Program.gnvSplash.isLinkAdresse.Equals("") &&
                File.Exists(Program.gnvSplash.isLinkAdresse))
                picLink.Visible = true;


            //Screen.PrimaryScreen.WorkingArea.Bottom;
            panel.Top = /**Screen.PrimaryScreen.Bounds.Bottom**/ lheight - 60;
            panel.Size = new Size(lwidth, 60); //Screen.PrimaryScreen.WorkingArea.Width

            progressBar.Location = new Point((int)((int)(panel.Width / 2) - (int)(progressBar.Width / 2)), 5);
            regfor.Location = new Point(panel.Width - regfor.Width, regfor.Top);
            msgtext.Location = new Point(((int)(lwidth / 2) - (int)(msgtext.Size.Width / 2)), progressBar.Top + 10);

            regfor.Text = Program.gnvSplash.isReg;

            //Screen.PrimaryScreen.WorkingArea.Width Screen.PrimaryScreen.WorkingArea.Height
            if (lwidth> 1024 && ( lheight - 80) > 768)
            {
                x = lwidth - 1024; //Screen.PrimaryScreen.WorkingArea.Width
                x /= 2;
                y = lheight - 768; //Screen.PrimaryScreen.WorkingArea.Height
                y /= 2;
                webBrowser.Size = new Size(1024, 768);
                webBrowser.Location = new Point(x, y);                
            }
            else
                webBrowser.Size = new Size(lwidth, this.Height - 80); //Screen.PrimaryScreen.WorkingArea.Width

            System.IntPtr tmp;
            tmp = (System.IntPtr)(-1);
             Win32User.SetWindowPos(this.Handle, tmp, 0, 0, 0, 0, 83);

            timerSplash.Interval = 100;
            timerSplash.Enabled = true;

            timerAnimation.Enabled = true;
            try
            {
                 // Bei Expired: SMS,Tel,Konto deaktivieren
                if (Program.gnvSplash.kennzKPExpired.Equals("1"))
                {
                    labelIkunden.BackColor = Color.Silver;
                    labelVOIP.BackColor = Color.Silver;
                    labelSMS.BackColor = Color.Silver;
                    txtIKunden.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoIkunden"));
                    txtVoip.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoVoip"));
                    txtSms.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoSMS"));
                }
                else
                {
                    // Benutzerkonten
                    if (Program.gnvSplash.ibKennzKunde)
                    {
                        labelIkunden.BackColor = Color.Lime;
                        txtIKunden.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoIkunden"));
                    }
                    else
                    {
                        labelIkunden.BackColor = Color.Red;
                        txtIKunden.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoIkunden"));
                    }

                    // SMS
                    if (Program.gnvSplash.ibKennzSMS)
                    {
                        labelSMS.BackColor = Color.Lime;
                        txtSms.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoSMS"));
                    }
                    else
                    {
                        labelSMS.BackColor = Color.Red;
                        txtSms.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoSMS"));
                    }

                    // Voip
                    if (Program.gnvSplash.ibKennzVoip)
                    {
                        labelVOIP.BackColor = Color.Lime;
                        txtVoip.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoVoip"));
                    }
                    else
                    {
                        labelVOIP.BackColor = Color.Red;
                        txtVoip.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoVoip"));
                    }
                }
                // Games
                if (Program.gnvSplash.ibKennzSpiele)
                {
                    labelSpiele.BackColor = Color.Lime;
                    txtSpiele.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoGames"));
                }
                else
                {
                    labelSpiele.BackColor = Color.Red;
                    txtSpiele.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoGames"));
                }

                // MP
                switch (Program.gnvSplash.isModulMP)
                {
                    case "0": // alle geschlossen
                        {
                            labelMP.BackColor = Color.Red;
                            txtMP.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoMP"));
                            labelBill.BackColor = Color.Red;
                            txtBill.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoBill"));
                            break;
                        }
                    case "1": // Nur MP aktiv
                        {

                            inetRegistry local = new inetRegistry();
                            local.SubKey = inetConstants.icsRegKey + "\\" + inetConstants.icsVerzMP;
                            uint ergebnis = local.ReadAsDWORD(inetConstants.icsMPArt);
                            local = null;
                            if ((ergebnis > 4) || (ergebnis == 0))
                            {
                                if (Program.gnvSplash.ibUSBCtrlPolled)
                                    labelMP.BackColor = Color.Lime;
                                else
                                   labelMP.BackColor = Color.Yellow;
                            }
                            else
                                labelMP.BackColor = Color.Lime;

                            txtMP.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoMP"));
                            labelBill.BackColor = Color.Red;
                            txtBill.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoBill"));
                            break;
                        }
                    case "2": // Nur Bill aktiv
                        {
                            if (Program.gnvSplash.ibUSBCtrlPolled)
                                labelBill.BackColor = Color.Lime;
                            else
                                labelBill.BackColor = Color.Yellow;

                            txtBill.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoBill"));
                            labelMP.BackColor = Color.Red;
                            txtMP.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoMP"));
                            break;
                        }
                    case "3": // MP &  Bill aktiv
                        {
                            if (Program.gnvSplash.ibUSBCtrlPolled)
                            {
                                labelMP.BackColor = Color.Lime;
                                labelBill.BackColor = Color.Lime;
                            }else{
                                labelMP.BackColor = Color.Yellow;
                                labelBill.BackColor = Color.Yellow;
                            }
                           
                            txtMP.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoMP"));
                            txtBill.Text = String.Format(Program.getMyLangString("infoActive"), Program.getMyLangString("infoBill"));
                            break;
                        }
                }
            }
            catch
            { }


            timerCheck.Interval = 1000;
            timerCheck.Enabled = true;
            this.progressBar.Visible = false;
        }


        delegate void SetMPTextCallBack(String asText);
        public void SetMPText(String asText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetMPTextCallBack(SetMPText), new object[] { asText });
                return;
            }
            else
            {
                this.txtMP.Text = String.Format(Program.getMyLangString("infoDeactive"), Program.getMyLangString("infoMP"));
                labelMP.BackColor = Color.Red;
            }
        }

        delegate void SwitchColorCallBack(Color arg1, Color arg2);
        public void SwitchColor(Color arg1,Color arg2)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SwitchColorCallBack(SwitchColor), new object[] { arg1,arg2 });
                return;
            }
            else
            {
                Console.WriteLine("Color1:" + arg1.ToString() + " /arg2:" + arg2.ToString()); 
                if (arg1 != Color.Black)
                    labelMP.BackColor = arg1;

                if (arg2 != Color.Black)
                    labelBill.BackColor = arg2;
            }
        }



        delegate void SetNavigateSeiteCallBack();
        public void SetNavigateSeite()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new SetNavigateSeiteCallBack(SetNavigateSeite), new object[] { });
                return;
            }
            else
            {
                String lsURL = String.Empty;
                Uri lUri;


                if (!Program.gnvSplash.isWWWLocal.Equals("") && !File.Exists(Program.gnvSplash.isWWWLocal.Replace("file:///", "")))
                {
                    httpcontrol localhttp = new httpcontrol();
                    localhttp.fileDownload(Program.gnvSplash.isWWW + "/all600.zip", Program.gnvSplash.isFileUrlIP, "all.zip");
                    localhttp = null;
                }
                else if (Program.gnvSplash.isWWWLocal.Equals(string.Empty))
                    Program.gnvSplash.isWWWLocal = Program.gnvSplash.isWWW;

                lsURL = Program.gnvSplash.isWWWLocal + Program.gnvSplash.isWWWPar;
                if (Program.gnvSplash.ibKennzKunde && Program.gnvSplash.ibStatusPing)
                    lsURL += "?log=1";

                lsURL += "?localLang=" + Program.defaultLanguage + "?prskp=" + Program.getMyLangString("preisTitle") + Program.gnvSplash.idcStundenPreis.ToString("0.00") + " " + Program.gnvSplash.isWaehrung;
Console.WriteLine(lsURL);
                try
                {
                    if (webBrowser.ReadyState != WebBrowserReadyState.Loading)
                    {
                        lUri = new Uri(lsURL, true);//true
                        webBrowser.Url = lUri;
                    }
                }
                catch
                { }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void appBeenden()
        {
            timerSplash.Enabled = false;
            timerCheck.Enabled = false;
            timerAnimation.Enabled = false;
            isCancel = true;
            Program.gnvSplash.AppClose();
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlgSchutzSplash_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isCancel && (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.UserClosing) )
            {
                e.Cancel = true;
                return;
            }
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try{
            if (ibNavigate)
                {

                    String lsTmp;
                    lsTmp = e.Url.OriginalString;

                    lsTmp = lsTmp.Substring(lsTmp.LastIndexOf("?2=") + 3);
                    String[] lsData;
                    char[] c = { ';' };
                    lsData = lsTmp.Split(c);

                    if ((lsData[0].IndexOfAny(c) > -1) || (lsData[1].IndexOfAny(c) > -1))
                    {
                        SetText(String.Format(Program.getMyLangString("errorInValid"), ""));
                        return;
                    }

                    try
                    {
                        if (lsData.Length > 2 && !lsData[2].ToLower().Equals("undefined") &&
                            !lsData[2].Equals(Program.defaultLanguage))
                        {
                            Program.switchLanguage(lsData[2]);
                        }
                    }
                    catch
                    {
                        Program.switchLanguage(Program.defaultLanguage);
                    }
                    e.Cancel = true;


                    try
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(this.ClickUserConnect));
                        t.SetApartmentState(ApartmentState.STA);
                        t.Priority = ThreadPriority.Highest;
                        t.Name = "inetclickuser";
                        t.Start(lsData);
                        t = null;
                    }
                    catch { }
                }
            }
            catch { }
        }

        // delegate void ClickUserConnectCallBack(object arg1);
         public void ClickUserConnect(object arg1)
         {
           /*  if (this.InvokeRequired)
             {
                 this.Invoke(new ClickUserConnectCallBack(ClickUserConnect), new object[] { arg1 });
                 return;
             }
             else
             {*/

                 try
                 {
                     String[] lsData;
                     lsData = (String[])arg1;
                     if (lsData.Length >= 2)
                     {
                         Program.gnvSplash.SetUserAccount(lsData[0], lsData[1]);
                     }
                 }
                 catch { }
        // }

         }


         private void changeUILanguage(String arg1)
         {
             Program.switchLanguage(arg1);
         }


        private void timerSplash_Tick(object sender, EventArgs e)
        {
            System.IntPtr tmp, liFGWindow;
            tmp = (System.IntPtr)(-1);

            if (pApplication != null)
            {
                if (this.TopMost)
                    this.TopMost = false;
            }
            else
            {
                if (Program.gnvSplash.il_hwnd3 > 0)
                {
                    Win32User.SetWindowPos((IntPtr)Program.gnvSplash.il_hwnd3, tmp, 0, 0, 0, 0, 83);
                    Win32User.SetWindowPos(this.Handle, (IntPtr)Program.gnvSplash.il_hwnd3, 0, 0, 0, 0, 83);
                }
                else
                {
                    this.TopMost = true;
                    Win32User.SetWindowPos(this.Handle, tmp, 0, 0, 0, 0, 83);
                }


                liFGWindow = (System.IntPtr)Win32User.GetForegroundWindow();
                if (liFGWindow == this.Handle ||
                    liFGWindow.ToInt32() == Program.gnvSplash.il_hwnd1 ||
                    liFGWindow.ToInt32() == Program.gnvSplash.il_hwnd2 ||
                    liFGWindow.ToInt32() == Program.gnvSplash.il_hwnd3) {
                }
                else
                {
                    tmp = (System.IntPtr)(-2);
                    Win32User.SetWindowPos(liFGWindow, tmp, 0, 0, 0, 0, 83);
                    Win32User.CloseWindow(liFGWindow.ToInt32());
                }

            }            
        }

        delegate void SetCloseCallBack();
        public void SetCloseWindow()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetCloseCallBack(SetCloseWindow), new object[] { });
                return;
            }
            else
            {
                try
                {
                    isCancel = true;
                    timerCheck.Enabled = false;
                    timerSplash.Enabled = false;
                    timerAnimation.Enabled = false;
                    Close();
                }
                catch { }
            }

        }


        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ibNavigate = true;
        }

     
        private void dlgSchutzSplash_Load(object sender, EventArgs e)
        {
            LoadForm();
        }

        private void dlgSchutzSplash_KeyDown(object sender, KeyEventArgs e)
        {
          
            String[] localTmp;
            List<string> localKey = new List<string>();
           
            char[] c = { ',' };

            localTmp = e.KeyData.ToString().Split(c);
            int i = 0;
            try
            {
                if (localTmp != null)
                {
                    for (i = 0; i < localTmp.Length; i++)
                    {
                        localTmp[i] = localTmp[i].Trim();
                        if (localTmp[i].ToLower().IndexOf("key") > -1)
                            localTmp[i] = localTmp[i].Substring(0, localTmp[i].ToLower().IndexOf("key"));

                        if (!localKey.Contains(localTmp[i]))
                            localKey.Add(localTmp[i]);

                    }
                }
            }
            catch 
            {  }
            i = 0;

            try
            {
                if (localKey != null)
                {
                    foreach (String s in localKey)
                        if (isKeys.Contains(s.ToUpper().Trim()))
                            i++;
                }
            }
            catch 
            { }
            
            if (i == 3)
            {
                timerCheck.Enabled = false;
                timerSplash.Enabled = false;
                dlgSplashAbbruch dlgAbbruch = new dlgSplashAbbruch();
                dlgAbbruch.ShowDialog();
               
                timerSplash.Enabled = true;
                timerCheck.Enabled = true;
            }
        }

        private void timerCheck_Tick(object sender, EventArgs e)
        {
            if (Program.gnvSplash.isKioskID.Equals(""))
            {
                try
                {
                    dlgEinstellung dlg = new dlgEinstellung(true);
                    this.timerSplash.Enabled = false;
                    this.timerCheck.Enabled = false;
                    this.TopMost = false;
                    dlg.Show();
                    dlg.TopMost = true;
                }
                catch 
                { }
            }

            // Neu Start
            if (Program.gnvSplash.ibStatReboot)
            {
                try
                {
                    TimeSpan ts;
                    ts = DateTime.Now.Subtract(Program.gnvSplash.idtStatReboot);
                    if (ts.TotalMinutes >= 1 && ts.TotalMinutes <= 2)
                    {
                        this.timerSplash.Enabled = false;
                        this.timerCheck.Enabled = false;
                        this.timerAnimation.Enabled = false;

                        if (Program.gnvSplash.DialogNewUser != null)
                        {
                            // Timer bennden
                            Program.gnvSplash.ibStatReboot = false;
                        }
                        // Neu Start
                        inetAPI.PCReboot();
                    }
                }
                catch { }
            }

            if (Program.gnvSplash.ibStatShutDown)
            {
                try
                {
                    TimeSpan ts;
                    ts = DateTime.Now.Subtract(Program.gnvSplash.idtStatShutDown);
                    if (ts.TotalMinutes >= 1 && ts.TotalMinutes <= 2)
                    {
                        this.timerSplash.Enabled = false;
                        this.timerCheck.Enabled = false;
                        this.timerAnimation.Enabled = false;

                        if (Program.gnvSplash.DialogNewUser != null)
                        {
                            // Timer bennden
                            Program.gnvSplash.ibStatReboot = false;
                        }
                        // Shutdown
                        Program.gnvSplash.ibStatShutDown = false;
                        inetAPI.PCShutDown();
                    }
                }
                catch { }
            }

            // Prüfen ob neustart mit Sperrvorgang
            if (Program.gnvSplash.ibNeuStartAuto & Program.gnvSplash.ibNeuStartNow)
            {
                try
                {
                    // Bitte keine Münze einwerfen....
                    // Neustart in 10 sekunden.....
                    if (Program.gnvSplash != null && Program.gnvSplash.gnvMainInetplus != null)
                        Program.gnvSplash.gnvMainInetplus.SetAgentText(Program.getMyLangString("noMoneyInsert"));
                  
                    timerCheck.Enabled = false;

                    Program.gnvSplash.SetControlModul(false);

                    timerSplash.Enabled = false;
                    timerAnimation.Enabled = false;
                    // Do While
                    System.Threading.Thread.Sleep(3000);
                    inetAPI.PCReboot();
                }
                catch { }
                // neu start
                return;
            }


            try
            {
                // Werbetext für Agent anzeigen
                iAgentMeldung++;
                if (iAgentMeldung > 600)
                {
                    iAgentMeldung = 0;
                    if (Program.gnvSplash.isWerbeText != null && !Program.gnvSplash.isWerbeText.Equals(""))
                        Program.gnvSplash.gnvMainInetplus.SetAgentText(Program.gnvSplash.isWerbeText);
                }
            }
            catch
            { }
        }

        private void panel_Click(object sender, EventArgs e)
        {

            try
            {
                int i = 0;
                i = i + 20;
                regfor_Click(sender, e);
                regfor.Focus();
                i = (int) (i / 2);
                i = 0;
                this.Focus();
            }
            catch
            { }
        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(iRed, iGreen, iBlue);//iColorArray[(new Random()).Next(0, iColorArray.Count)];

            if (!bMinus)
            {
                iRed += 5;
                iGreen += 5;
                iBlue += 5;
            }
            else
            {
                iRed -= 5;
                iGreen -= 5;
                iBlue -= 5;
            }

            if (iRed > 180)
                bMinus = true;

            if (iRed <= 0)
            {
                bMinus = false;

                iRed = 0;
                iGreen = 0;
                iBlue = 0;
            }

            if (zaehler > 20)
            {
                msgtext.Text = "";
                zaehler = 0;
            }
            else if (!msgtext.Text.Equals(""))
            {
                zaehler++;
            }
        }

        private void picLink_Click(object sender, EventArgs e)
        {
            try
            {

                Program.gnvSplash.SetShowTrayWnd(false);

                Thread th = new Thread(new ThreadStart(myTestFunc));
                th.Name = "myTestFunc";
                th.Priority = ThreadPriority.Normal;
                th.Start();

              

            }
            catch 
            {
                Program.gnvSplash.SetControlModul(true);
                Program.gnvSplash.DialogShutz.TopMost = true;
                pApplication = null;
            }

        }


        private void myTestFunc()
        {
            try
            {
                String lsFile = Program.gnvSplash.isLinkAdresse;

                if (File.Exists(lsFile))
                {
                    Program.gnvSplash.SetControlModul(false);

                   
                    pApplication = new System.Diagnostics.Process();

                    pApplication.StartInfo.FileName = lsFile;
                    pApplication.StartInfo.CreateNoWindow = true;
                    pApplication.StartInfo.UseShellExecute = false;
                    pApplication.Start();

                    Program.gnvSplash.DialogShutz.TopMost = false;
                    
                    pApplication.WaitForExit();

                    pApplication.Close();
                    Program.gnvSplash.DialogShutz.TopMost = true;
                    try
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Program.gnvSplash.SetControlModul));
                        t.SetApartmentState(ApartmentState.STA);
                        t.Priority = ThreadPriority.Highest;
                        t.Name = "inet01";
                        t.Start(true);
                    }
                    catch { }
                    Program.gnvSplash.SetShowTrayWnd(true);
                    pApplication = null;
                }
            }
            catch { }

        }


        private void regfor_Click(object sender, EventArgs e)
        {
            regfor.Text = regfor.Text;
        }


    }
}