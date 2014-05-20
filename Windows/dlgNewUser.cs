using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using kioskplus.Utils;

namespace kioskplus.Windows
{
    public partial class dlgNewUser : Form
    {
        private Boolean ibClose = false;
        private Boolean ibStartMove = false;
        private Point mausPoint = new Point();
        private inetRegionCtrl iRegCtrl = null;
        private inetRegion iRegion = null;
        //
        private String isAnzName = String.Empty;
        private String isOnline = String.Empty;

        private inetDateTime invDT;

        private ImageList imageList = null;

        private const int WM_TIMECHANGE = 0x001E;
        public Boolean ibTimeChanging = false;

        dlgDrucken drucker = null;
        dlgBlackJack blackJack = null;
        Boolean ibVorne = false;
        Boolean ibWindowOpened = false;
        int mRestTime = 0;
        inetRegistry mRegistry = new inetRegistry();

        dlgAffi dAffi = null;


        public dlgNewUser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlgNewUser_MouseMove(object sender, MouseEventArgs e)
        {
            if (ibStartMove)
            {
                Point currentPos = Control.MousePosition;
                currentPos.X -= mausPoint.X;
                currentPos.Y -= mausPoint.Y;
                this.Location = currentPos;

                return;
            }
            inetRegion tmpRegion = null;
            Point point = new Point(e.X, e.Y);
            // um zu überprüfen ob point in region?
            if (iRegCtrl != null)
            {
                tmpRegion = iRegCtrl.GetRegion(point);
                if (tmpRegion != null)
                {
                    if (iRegion != null)
                    {
                        if (tmpRegion.GetName().Equals(iRegion.GetName()))
                            return;
                    }

                    iRegion = tmpRegion;
                    this.Cursor = Cursors.Hand;

                    if (imageList.Images.ContainsKey(iRegion.GetName().ToLower()))
                        this.BackgroundImage = imageList.Images[iRegion.GetName().ToLower()];
                }
                else
                {
                    if (iRegion != null)
                        if (imageList.Images.ContainsKey(iRegion.GetMausOut().ToLower()))
                            this.BackgroundImage = imageList.Images[iRegion.GetMausOut().ToLower()];

                    iRegion = tmpRegion;
                    this.Cursor = Cursors.Default;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlgNewUser_MouseDown(object sender, MouseEventArgs e)
        {
            mausPoint.X = e.X;
            mausPoint.Y = e.Y;
            ibStartMove = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlgNewUser_MouseUp(object sender, MouseEventArgs e)
        {
            ibStartMove = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlgNewUser_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (!ibClose && ( e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.TaskManagerClosing))
            {
                e.Cancel = true;
            }
            else
            {
                timerNUser.Enabled = false;
                notifyIconUser.Visible = false;

                if (blackJack != null)
                {
                    blackJack.Close();
                    blackJack = null;
                }

                try
                {
                    if (drucker != null)
                        drucker.Close();

                    drucker = null;
                }
                catch { }

                try
                {
                    iRegCtrl = null;
                    iRegion = null;
                    imageList = null;
                }
                catch { }

                try
                {
                    if (dAffi != null)
                    {
                        dAffi.SetClose();
                       // dAffi = null;
                    }
                }
                catch //(Exception ex) 
                { 
                //    Console.WriteLine("dlgAffiiii closing... versuch error::: " + ex.Message); 
                }
            }
        }

        private void dlgNewUser_Load(object sender, EventArgs e)
        {
            try {

                if (mRegistry != null)
                    mRegistry.SubKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion";

                SetNewAgent();
                
                if (Program.gnvSplash != null)
                    Program.gnvSplash.BlockMaus(false);
            }
            catch { }

            Image[] lsFiles = {kioskplus.benutzer };
            switch (Program.gnvSplash.isSkinName)
            {
                case "standard":
                    {
                        Image[] lsTmp = {kioskplus.aufladen,kioskplus.benutzer,kioskplus.exit,kioskplus.mail, kioskplus.main,kioskplus.sms, kioskplus.telefon };
                        lsFiles = lsTmp;
                        break;
                    }
                case "logo":
                    {
                        this.BackgroundImage = kioskplus.lmain;
                        Image[] lsTmp = { kioskplus.laufladen, kioskplus.lbenutzer, kioskplus.lexit, kioskplus.lmail, kioskplus.lmain, kioskplus.lsms, kioskplus.ltelefon };
                        lsFiles = lsTmp;
                        break;
                    }
                case "quadrat":
                    {
                        this.BackgroundImage = kioskplus.dmain;
                        Image[] lsTmp = { kioskplus.daufladen, kioskplus.dbenutzer, kioskplus.dexit, kioskplus.dmail, kioskplus.dmain, kioskplus.dsms, kioskplus.dtelefon };
                        lsFiles = lsTmp;
                        break;
                    }
                case "rund":
                    {
                        this.BackgroundImage = kioskplus.rmain;
                        Image[] lsTmp = { kioskplus.raufladen, kioskplus.rbenutzer, kioskplus.rexit, kioskplus.rmail, kioskplus.rmain, kioskplus.rsms, kioskplus.rtelefon };
                        lsFiles = lsTmp;
                        break;
                    }
                case "rundb":
                    {
                        this.BackgroundImage = kioskplus.rbmain;
                        Image[] lsTmp = { kioskplus.rbaufladen, kioskplus.rbbenutzer, kioskplus.rbexit, kioskplus.rbmail, kioskplus.rbmain, kioskplus.rbsms, kioskplus.rbtelefon };
                        lsFiles = lsTmp;
                        break;
                    }
                case "modern":
                    {
                        this.BackgroundImage = kioskplus.mmain;
                        Image[] lsTmp = { kioskplus.maufladen, kioskplus.mbenutzer, kioskplus.mexit, kioskplus.mmail, kioskplus.mmain, kioskplus.msms, kioskplus.mtelefon };
                        lsFiles = lsTmp;
                        break;
                    }
            }

            this.Size = this.BackgroundImage.Size;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - (this.Width + 50), 50);

            try
            {
                iRegCtrl = Program.gnvSplash.iNetRegionControl;
                if (iRegCtrl != null)
                {
                    imageList = new ImageList();
                    imageList.ImageSize = this.BackgroundImage.Size;

                    String[] lsFileNames = { "aufladen", "konto", "exit", "mail", "main", "sms", "telefon" };

                    if (lsFiles != null)
                        for (int i = 0; i < lsFiles.Length; i++)
                            imageList.Images.Add(lsFileNames[i], lsFiles[i]);

                    Rectangle r = iRegCtrl.GetTimeTextPos();
                    txtTime.Location = r.Location;
                    txtTime.Size = r.Size;

                    // Position setzen
                    r = iRegCtrl.GetUserTextPos();
                    txtUser.Location = r.Location;
                    txtUser.Size = r.Size;

                    txtTime.ForeColor = Color.FromArgb(iRegCtrl.GetUserColor());
                    txtUser.ForeColor = Color.FromArgb(iRegCtrl.GetUserColor());
                    timerNUser.Enabled = true;
                }
                else
                    MessageBox.Show(Program.getMyLangString("incompleteInstall"));
            }
            catch
            { }

            startNotifyUser();
            try
            {
                if (Program.gnvSplash.ibAffili == 1)
                    startAffili();
            }
            catch
            { }

        }

        private void startAffili()
        {
            try
            {
                Thread tr = new Thread(new ThreadStart(openAffiliWindow));
                tr.Priority = ThreadPriority.Normal;
                tr.Start();
            }
            catch
            { }
        }

        private void openAffiliWindow()
        {
            try
            {
                Thread.Sleep(10000); // 10 sekunden waretn
                dAffi = new dlgAffi();
                dAffi.ShowDialog();
            }
            catch
            { }
        }


        public inetDateTime GetDateTimeObject()
        {
            return invDT;
        }
        
        private void startNotifyUser()
        {

            while (Program.gnvSplash.il_hwnd1 <= 0)
            {
                Program.gnvSplash.PruefTaskBar();
                Thread.Sleep(1500);
            }
            
            // Gefunden raus
            ContextMenuStrip ctxMenu;
            ToolStripMenuItem[] mItem = new ToolStripMenuItem[8];
            ToolStripSeparator[] tsSeperator = new ToolStripSeparator[4];
            try
            {
                tsSeperator[0] = new ToolStripSeparator();
                tsSeperator[1] = new ToolStripSeparator();
                tsSeperator[2] = new ToolStripSeparator();
                tsSeperator[3] = new ToolStripSeparator();

                mItem[0] = new ToolStripMenuItem();
                mItem[0].Click += new EventHandler(onClickIcon);
                mItem[0].Name = "support";
                mItem[0].Text = Program.getMyLangString("menu01");

                mItem[1] = new ToolStripMenuItem();
                mItem[1].Click += new EventHandler(onClickIcon);
                mItem[1].Name = "info";
                mItem[1].Text = Program.getMyLangString("menu02");

                mItem[2] = new ToolStripMenuItem();
                mItem[2].Click += new EventHandler(onClickIcon);
                mItem[2].Name = "beenden";
                mItem[2].Text = Program.getMyLangString("menu03");

                mItem[3] = new ToolStripMenuItem();
                mItem[3].Click += new EventHandler(onClickIcon);
                mItem[3].Name = "austausch";
                mItem[3].Text = Program.getMyLangString("menu04");

                mItem[4] = new ToolStripMenuItem();
                mItem[4].Click += new EventHandler(onClickIcon);
                mItem[4].Name = "deaktiv";
                mItem[4].Text = Program.getMyLangString("menu05");

                mItem[5] = new ToolStripMenuItem();
                mItem[5].Click += new EventHandler(onClickIcon);
                mItem[5].Name = "foto";
                mItem[5].Text = Program.getMyLangString("menu07");

                mItem[6] = new ToolStripMenuItem();
                mItem[6].Click += new EventHandler(onClickIcon);
                mItem[6].Name = "blackjack";
                mItem[6].Text = Program.getMyLangString("menu06");

                mItem[7] = new ToolStripMenuItem();
                mItem[7].Click += new EventHandler(onClickIcon);
                mItem[7].Name = "games";
                mItem[7].Text = Program.getMyLangString("menu08");


                ctxMenu = new ContextMenuStrip();
                ctxMenu.Items.Add(mItem[7]);
                ctxMenu.Items.Add(mItem[6]);
                ctxMenu.Items.Add(tsSeperator[3]);
                ctxMenu.Items.Add(mItem[5]);
                ctxMenu.Items.Add(tsSeperator[0]);
                ctxMenu.Items.Add(mItem[4]);
                ctxMenu.Items.Add(mItem[3]);
                ctxMenu.Items.Add(mItem[2]);
                ctxMenu.Items.Add(tsSeperator[1]);
                ctxMenu.Items.Add(mItem[1]);
                ctxMenu.Items.Add(tsSeperator[2]);
                ctxMenu.Items.Add(mItem[0]);
                
                notifyIconUser.ContextMenuStrip = ctxMenu;
                notifyIconUser.Visible = true;

            }
            catch 
            { }
        }


        private void onClickIcon(object sender, EventArgs e)
        {
            ToolStripMenuItem ts;
            ts = (ToolStripMenuItem) sender;
            if (ts == null)
                return;

            try
            {
                switch (ts.Name.ToLower())
                {
                    case "info":
                        {
                            if (!ibWindowOpened)
                            {
                                dlgAbout about = new dlgAbout();
                                about.ShowDialog();
                                about = null;
                            }
                            break;
                        }
                    case "beenden":
                        {
                            if (!ibWindowOpened)
                            {
                                dlgExit exit = new dlgExit(invDT);
                                exit.ShowDialog();
                                exit = null;
                            }
                            break;
                        }
                    case "support":
                        {
                            connectToSupport();
                            break;
                        }
                    case "austausch":
                        {
                            SetNewAgent();
                            break;
                        }
                    case "deaktiv":
                    case "aktiv":
                        {
                            if (ts.Name.ToLower().Equals("deaktiv"))
                            {
                                ts.Text = Program.getMyLangString("menu052");
                                ts.Name = "aktiv";
                                if (Program.gnvSplash.gnvMainInetplus != null)
                                    Program.gnvSplash.gnvMainInetplus.SetAgentHide(false);
                            }
                            else
                            {
                                ts.Text = Program.getMyLangString("menu05");
                                ts.Name = "deaktiv";
                                if (Program.gnvSplash.gnvMainInetplus != null)
                                    Program.gnvSplash.gnvMainInetplus.SetAgentHide(true);
                            }
                            break;
                        }
                    case "blackjack":
                        {
                            if (blackJack == null)
                                blackJack = new dlgBlackJack();

                            blackJack.Show();
                            break;
                        }
                    case "foto":
                        {
                            dlgWebCam dv = new dlgWebCam();
                            dv.Show();
                            dv = null;
                            break;
                        }
                    case "games":
                        {
                            // Process starten
                            if (!Program.gnvSplash.isGamesLink.Equals(String.Empty))
                                System.Diagnostics.Process.Start(Program.gnvSplash.isGamesLink);


                            break;
                        }
                }
            }
            catch { }
        }


        public void CloseBlackJack()
        {
            try
            {
                if (blackJack != null)
                    blackJack.Close();

                blackJack = null;
            }
            catch { }
            
        }

        private void SetNewAgent()
        {
            if (Program.gnvSplash.gnvMainInetplus != null) 
                Program.gnvSplash.gnvMainInetplus.SetNewAgent();
        }

        private void connectToSupport()
        {
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("action=dataselect&0=")
                .Append(Program.gnvSplash.iTerminalID.ToString())
                .Append("&2=1");
            httpcontrol local = new httpcontrol();
            local.RunAction(strQuery.ToString(),Program.gnvSplash.iActServerID,false);
            if (local.GetParameterValue("1").Equals(""))
            {
                local = null;
                try
                {
                    Program.gnvSplash.gnvMainInetplus.SetText(Program.getMyLangString("supportText"));
                }
                catch { }
                return;
            }
            try
            {
                // Meldung...
                if (System.IO.File.Exists("epgclient.exe"))
                {
                    System.Diagnostics.Process pApplication = new System.Diagnostics.Process();

                    pApplication.StartInfo.FileName = inetConstants.isSystemDirectory + "\\epgclient.exe";
                    pApplication.StartInfo.Arguments = "PIN=" + local.GetParameterValue("1");
                    pApplication.StartInfo.CreateNoWindow = true;
                    pApplication.StartInfo.UseShellExecute = false;
                    pApplication.Start();
                }
                   
            }
            catch { }
            local = null;

        }


        public void RefreshInetDateTime(inetDateTime adt)
        {
            invDT = adt;
        }

        public void SetFreigabe(String asAnzName, String asOnline, inetDateTime adt)
        {
            isAnzName = asAnzName;
            isOnline = asOnline;
            invDT = adt;
            this.txtTime.Text = asOnline;
            this.txtUser.Text = asAnzName;
        }

        private void TimerUser()
        {
            Boolean lbOK=true;

            SetText(invDT.GetOnlineZeitAsString());
            int restTime = invDT.GetMinutenFromOnlineZeit(invDT.GetOnlineZeitAsString());

            if (restTime <= 5)
            {
                if ((mRestTime != restTime))
                {
Console.WriteLine("mRest<>rest");
                    mRestTime = restTime;
                    txtTime.ForeColor = Color.Red;
                    if (!ibVorne) { this.TopMost = true; ibVorne = true; }
                    try
                    {
                        Program.gnvSplash.gnvMainInetplus.SetText(String.Format(Program.getMyLangString("lastMinutenNoch"), restTime) + Program.getMyLangString("last" + restTime.ToString() + "Minuten"));
                    }
                    catch { }
                }
            }
            else
            {
                mRestTime = 0;
                txtTime.ForeColor = Color.FromArgb(iRegCtrl.GetUserColor()); //Color.White;
                if (ibVorne) { this.TopMost = false; ibVorne = false; }
            }
            if (invDT.GetMinutenFromOnlineZeit(invDT.GetOnlineZeitAsString()) <= 0)
            {
               lbOK = false;
               // Umsatz .. schließen .....inetdatetime auf null setzen
            }
            if (!lbOK)
            {
                timerNUser.Enabled = false;
                Program.gnvSplash.SperrenComputer("2");
            }
            else
                checkFensterPosition();
        }

        private void checkFensterPosition()
        {
           
            if (this.Location.X <= 10)
                this.Location = new Point(10, this.Location.Y);

            if ((this.Location.X + this.Size.Width) > Screen.PrimaryScreen.Bounds.Right)
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Size.Width, this.Location.Y);

            if (this.Location.Y < Screen.PrimaryScreen.Bounds.Top)
                this.Location = new Point(this.Location.X,Screen.PrimaryScreen.Bounds.Top);

            if ( (this.Location.Y + this.Size.Height) > Screen.PrimaryScreen.WorkingArea.Bottom)
                this.Location = new Point(this.Location.X, Screen.PrimaryScreen.WorkingArea.Bottom - this.Size.Width);

           /*** if (dAffi != null)
                dAffi.SetChangePos();***/
        }


        delegate void SetTextCallBack(String asText);
        private void SetText(String asText)
        {
            if (this.txtTime.InvokeRequired)
            {
                this.Invoke(new SetTextCallBack(SetText), new object[] { asText });
                return;
            }
            else
            {
                this.txtTime.Text = asText;
                try
                {
                    if (mRegistry != null)
                        mRegistry.WriteNoCrypt(inetConstants.icsKPTime, asText);
                }
                catch { }

            }
        }

        delegate String GetTextCallBack();
        private String GetText()
        {
            if (this.txtTime.InvokeRequired)
            {
                this.Invoke(new GetTextCallBack(GetText), new object[] { });
                return "";
            }
            else
            {
                return this.txtTime.Text;
            }
        }

        delegate void SetCloseCallBack();
        public void SetClose()
        {
            if (this.txtTime.InvokeRequired)
            {
                this.Invoke(new SetCloseCallBack(SetClose), new object[] {});
                return;
            }
            else
            {
                try
                {

                    ibClose = true;
                    
                    mRegistry = null;

                 
                    this.Close();
                }
                catch { }
            }
        }

        delegate void SetUserNameCallBack(String asText);
        public void SetUser(String asText)
        {
            if (this.txtTime.InvokeRequired)
            {
                this.Invoke(new SetUserNameCallBack(SetUser), new object[] { asText });
                return;
            }
            else
            {

                this.txtUser.Text = asText;
            }
        }

        private void dlgNewUser_Click(object sender, EventArgs e)
        {
            // Plausi
            if (iRegion == null)
                return;
            try
            {
                //Console.WriteLine(iRegion.GetText() + " <<<>>> " + Program.getMyLangString(iRegion.GetText()));
                Program.gnvSplash.gnvMainInetplus.SetAgentText(String.Format(Program.getMyLangString(iRegion.GetText()), this.txtUser.Text));
            }
            catch { }

                ibWindowOpened = true;
            try
            {

                switch (iRegion.GetName().ToLower())
                {
                    case "mail":
                        {

                            dlgEmailService emailService = new dlgEmailService();
                            emailService.ShowDialog();
                            emailService = null;
                            break;
                        }
                    case "sms":
                        {
                            dlgSmsService smsService = new dlgSmsService();
                            smsService.ShowDialog();
                            smsService = null;
                            break;
                        }
                    case "konto":
                        {
                            dlgUserAccount userAccount = new dlgUserAccount();
                            userAccount.ShowDialog();
                            userAccount = null;
                            break;
                        }
                    case "aufladen":
                        {
                            dlgKontoAufladen kAufladen = new dlgKontoAufladen();
                            kAufladen.ShowDialog();
                            kAufladen = null;
                            break;
                        }
                    case "exit":
                        {
                            dlgExit Appexit = new dlgExit(invDT);
                            Appexit.ShowDialog();
                            Appexit = null;
                            break;
                        }
                    case "telefon":
                        {
                            Program.gnvSplash.gnvMainInetplus.ShowPhone(true);
                            break;
                        }
                }
            }
            catch
            { }
            ibWindowOpened = false ;

           // Program.gnvSplash.gnvMainInetplus.SetHideAgent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetRestMinutenAsString()
        {
            return invDT.GetRestDauerAsMin().ToString();
        }

        public int GetRestDauerAsMinuten()
        {
            return invDT.GetMinutenFromOnlineZeit(GetOnlineZeitAsString());
        }

        public String GetOnlineZeitAsString()
        {

            return invDT.GetOnlineZeitAsString();
        }

        public int GetOnlineZeitAsInt()
        {
            return invDT.GetMinutenFromOnlineZeit(invDT.GetGesamtOnlineZeit());
        }


        public String GetEndeOnlineZeitAsString()
        {
            return invDT.GetGesamtOnlineZeit();
        }


        public void SetNewOnlineTime(int aiMinuten)
        {
            invDT.SetEndeDateTime(-aiMinuten);
        }

        public String GetEndeDate()
        {
            return invDT.GetEndeDate();
        }

        public String GetEndeTime()
        {
            return invDT.GetEndeTime();
        }

        private void timerNUser_Tick(object sender, EventArgs e)
        {
            TimerUser();
        }

        protected override void WndProc(ref Message m)
        {
            if (m != null)
            {

                switch (m.Msg)
                {
                    case WM_TIMECHANGE:
                        {
                            if (!ibTimeChanging)
                            {
                                try
                                {
                                    ibTimeChanging = true;
                                    Program.gnvSplash.SetZeitZone(false);
                                }
                                catch { }
                            }
                            else
                                ibTimeChanging = false;
                        
                            break;
                        }

                }

                //   Console.WriteLine(m.Msg.ToString() + " /" + m.WParam.ToString());

            }
            base.WndProc(ref m);
        }
    }
}