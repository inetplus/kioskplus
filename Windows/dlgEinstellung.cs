using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using kioskplus.Utils;
using System.Threading;

namespace kioskplus.Windows
{
    public partial class dlgEinstellung : Form
    {
        inetRegistry instRegistry = new inetRegistry();
        String isSubKey = "";

        int iiAnzahlPWD = 0;
        
        // Münzprüfer
        CheckBox[] chkMP = new CheckBox[16];
        Label[] chkMPT = new Label[16];

        // Bill Validator
        CheckBox[] chkBill = new CheckBox[16];
        Label[] chkBillT = new Label[16];

        UsbDriversCtrl instDriver = new UsbDriversCtrl();
        
        Boolean ibLoad = true;
        Boolean ibOpenFromTimer = false;
        Boolean ibRegistered = false;
        Boolean ibChangeMP = false, ibChangeBill = false;

        public dlgEinstellung(Boolean argSwitch)
        {
            InitializeComponent();

            ibOpenFromTimer = argSwitch;

            chkMP[0] = checkBox0;
            chkMP[1] = checkBox1;
            chkMP[2] = checkBox2;
            chkMP[3] = checkBox3;
            chkMP[4] = checkBox4;
            chkMP[5] = checkBox5;
            chkMP[6] = checkBox6;
            chkMP[7] = checkBox7;
            chkMP[8] = checkBox8;
            chkMP[9] = checkBox9;
            chkMP[10] = checkBox10;
            chkMP[11] = checkBox11;
            chkMP[12] = checkBox12;
            chkMP[13] = checkBox13;
            chkMP[14] = checkBox14;
            chkMP[15] = checkBox15;

            chkMPT[0] = checkBoxt0;
            chkMPT[1] = checkBoxt1;
            chkMPT[2] = checkBoxt2;
            chkMPT[3] = checkBoxt3;
            chkMPT[4] = checkBoxt4;
            chkMPT[5] = checkBoxt5;
            chkMPT[6] = checkBoxt6;
            chkMPT[7] = checkBoxt7;
            chkMPT[8] = checkBoxt8;
            chkMPT[9] = checkBoxt9;
            chkMPT[10] = checkBoxt10;
            chkMPT[11] = checkBoxt11;
            chkMPT[12] = checkBoxt12;
            chkMPT[13] = checkBoxt13;
            chkMPT[14] = checkBoxt14;
            chkMPT[15] = checkBoxt15;

            chkBill[0] = cschein0;
            chkBill[1] = cschein1;
            chkBill[2] = cschein2;
            chkBill[3] = cschein3;
            chkBill[4] = cschein4;
            chkBill[5] = cschein5;
            chkBill[6] = cschein6;
            chkBill[7] = cschein7;
            chkBill[8] = cschein8;
            chkBill[9] = cschein9;
            chkBill[10] = cschein10;
            chkBill[11] = cschein11;
            chkBill[12] = cschein12;
            chkBill[13] = cschein13;
            chkBill[14] = cschein14;
            chkBill[15] = cschein15;

            chkBillT[0] = cscheint0;
            chkBillT[1] = cscheint1;
            chkBillT[2] = cscheint2;
            chkBillT[3] = cscheint3;
            chkBillT[4] = cscheint4;
            chkBillT[5] = cscheint5;
            chkBillT[6] = cscheint6;
            chkBillT[7] = cscheint7;
            chkBillT[8] = cscheint8;
            chkBillT[9] = cscheint9;
            chkBillT[10] = cscheint10;
            chkBillT[11] = cscheint11;
            chkBillT[12] = cscheint12;
            chkBillT[13] = cscheint13;
            chkBillT[14] = cscheint14;
            chkBillT[15] = cscheint15;

            tabPage1.Enabled = false;
            tabPage2.Enabled = false;
        }

        private void dlgEinstellung_Load(object sender, EventArgs e)
        {

            panel1.Location = new Point(0, 0);
            panel1.Size = this.Size;

            labelBitteWarten.Text = Program.getMyLangString("bittewarten");

            if (!Program.gnvSplash.isKioskID.Equals(""))
                ibRegistered = true;

            Thread t = new Thread(new ThreadStart(loadForm));
            t.Name = "test";
            t.Priority = ThreadPriority.Normal;
            t.Start();           
        }

        delegate void LoadFormCallBack();
        private void loadForm()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new LoadFormCallBack(loadForm), new object[] { });
                return;
            }
            else
            {
                Program.gnvSplash.SetControlModul(false);
                
                // Alle Verfügbaren Ports ermitteln
                String[] ports = System.IO.Ports.SerialPort.GetPortNames();
                comport.Items.Add(Program.getMyLangString("portAus"));
                foreach (String s in ports)
                    comport.Items.Add(s);

                isSubKey = instRegistry.SubKey;
                tabControl1.SelectedIndex = 3;
                // Waehrung
                this.waehrung.Text = Program.gnvSplash.isWaehrung;
                // Page Freischaltung
                this.kioskid.Text = Program.gnvSplash.isKioskID;
                if (!Program.gnvSplash.isKioskID.Equals(""))
                {
                    this.btnRegister.Enabled = false;
                    this.kioskid.BackColor = Color.LightGray;
                    this.kpasswd.BackColor = Color.LightGray;
                    this.kioskid.ReadOnly = true;
                    this.kpasswd.ReadOnly = true;
                    this.btnOK.Enabled = true;
                }
                else
                    this.btnReset.Enabled = false;

                
                //GUID stimmt nicht überein
                if (!Program.gnvSplash.isPcGuid.Equals(Program.gnvSplash.isPcGuidLocal))
                {
                    //this.btnReset.Enabled = false;
                    //this.btnRegister.Enabled = false;
                }

                this.guid.Text = Program.gnvSplash.isPcGuidLocal;
                
                // Anzahl der Passworteingabe
                if (Program.gnvSplash.iiAnzahl > 5)
                {
                    this.kpasswd.Enabled = false;
                    this.kioskid.Enabled = false;
                    this.kioskid.ReadOnly = true;
                    this.kpasswd.ReadOnly = true;
                    this.kpasswd.BackColor = Color.LightGray;
                    this.kioskid.BackColor = Color.LightGray;
                    this.btnRegister.Enabled = false;
                    this.btnReset.Enabled = false;
                }

                this.regfor.Text = Program.gnvSplash.isKunde;
                iiAnzahlPWD = Program.gnvSplash.iiAnzahl;

                // Ende der ersten Seite
                // Wenn das Programm gesperrt ist, soll keine MP Seite editiert werden
                switch (Program.gnvSplash.isModulMP)
                {
                    case "1": // MP
                        tabPage1.Enabled = false;
                        tabControl3.Enabled = false;
                        tabPage2.Enabled = true;
                        break;
                    case "2": // Bill
                        tabPage2.Enabled = false;
                        tabPage1.Enabled = true;
                        break;
                    case "3": // MP & Bill
                        tabPage2.Enabled = true;
                        tabPage1.Enabled = true;
                        break;
                }

                // Anfang der zweiten Seite
                String lsTmp;
                lsTmp = instRegistry.ReadAsString(inetConstants.icsImpPrs);
                if (lsTmp.Equals(""))
                    lsTmp = "0";
                
                try
                {
                    this.preis.Value = Decimal.Parse(lsTmp,Program.gnvDecFormat);
                    // Ende der zweiten Seite
                }
                catch { this.preis.Value = 1; }

                // Anfang der dritten Seite
                instRegistry.SubKey += "\\" + inetConstants.icsVerzMP;
                lsTmp = instRegistry.ReadAsDWORD(inetConstants.icsMPCom).ToString();
                if (lsTmp.Equals(""))
                    lsTmp = "0";
                try
                {
                    if (lsTmp.Equals("0"))
                        lsTmp = Program.getMyLangString("portAus");
                    else
                        lsTmp = "COM" + lsTmp;

                    int i = this.comport.Items.IndexOf(lsTmp);
                    if (i < 0)
                        this.comport.SelectedIndex = 0;
                    else
                        this.comport.SelectedIndex = i;
                }
                catch //(Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
                
                lsTmp = instRegistry.ReadAsDWORD(inetConstants.icsMPArt).ToString();
                if (lsTmp.Equals(""))
                    lsTmp = "0";

                try
                {
                    this.mpart.SelectedIndex = Int32.Parse(lsTmp);
                }
                catch { }

                uint tmp;
                for (int i = 0; i < 16; i++)
                {
                    tmp = instRegistry.ReadAsDWORD(inetConstants.icsMPMf + i.ToString());
                    chkMP[i].Checked = tmp.Equals(1);
                }
                // Ende der dritten Seite
                
                // Anfang der vierten Seite
                instRegistry.SubKey = isSubKey + "\\" + inetConstants.icsVerzBill;
                lsTmp = instRegistry.ReadAsDWORD(inetConstants.icsBillArt).ToString();
                if (lsTmp.Equals(""))
                    tabControl3.Enabled = false;

                if (lsTmp.Equals(""))
                    lsTmp = "0";

                this.geldschein.SelectedIndex = Int32.Parse(lsTmp);

                // Ende der vierten Seite
                ibLoad = false;

                panel1.Visible = false;
                
                // Anfang F8
                // Anzeigen ob Registry gelöscht wurde
                // M=Minimal,N=Network noch da oder Kombi. F8 deaktiv, wenn beide nicht da!

                try
                {
                    inetRegistry localReg = new inetRegistry();
                    localReg.SubKey = "SYSTEM\\CurrentControlSet\\Control\\SafeBoot\\Minimal";
                    String lsAnz = String.Empty;
                    if (localReg.GetSubKeyCount() <= 0)
                        lsAnz = "M";

                    localReg.SubKey = "SYSTEM\\CurrentControlSet\\Control\\SafeBoot\\Network";
                    if (localReg.GetSubKeyCount()<= 0)
                        lsAnz += "N";

                    if (lsAnz.Equals(""))
                        lsAnz = String.Format(Program.getMyLangString("infoActive"), "F8");
                    else
                        lsAnz = String.Format(Program.getMyLangString("infoDeactive"), lsAnz + " F8");

                    labelf8.Text = lsAnz;
                }
                catch //(Exception exx)
                {// MessageBox.Show(exx.Message); 
                }
                // Ende F8

                kioskid.Focus();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (this.preis.Value <= 0)
            {
                this.msgLabel.Text = Program.getMyLangString("errorPreis");
                this.tabControl1.SelectedIndex = 2;
                return;
            }

            if (!Program.gnvSplash.isModulMP.Equals("2"))
            {

                if (this.comport.SelectedIndex == 0)
                {
                    this.msgLabel.Text = Program.getMyLangString("errorMPComPort");
                    this.tabControl1.SelectedIndex = 1;
                    return;
                }
                else
                {
                    if (this.comport.SelectedIndex != 1)
                        if (this.mpart.SelectedIndex == 0)
                        {
                            this.msgLabel.Text = Program.getMyLangString("errorMP");
                            this.tabControl1.SelectedIndex = 1;
                            return;
                        }
                }


            }

            this.Cursor = Cursors.WaitCursor;
            instRegistry.SubKey = isSubKey;
            instRegistry.Write(inetConstants.icsImpPrs, this.preis.Value);
            instRegistry.Write(inetConstants.icsKioskID, this.kioskid.Text);
            
            instRegistry.BaseRegistryKey = Registry.LocalMachine;
            instRegistry.SubKey = inetConstants.icsRegKey + "\\" +inetConstants.icsVerzMP;
           // Program.gnvSplash.isModulMP = "1";

            try
            {

                switch (Program.gnvSplash.isModulMP)
                {
                    // Keine MP oder Bill Validator
                    case "1": // Nur Münzprüfer
                        {
                            SetMPWerte();
                            break;
                        }
                    case "2": // nur Bill Validator
                        {
                            SetBillWerte();
                            break;
                        }
                    case "3": // Münzprüfer und Bill Validator
                        {
                            SetMPWerte();
                            SetBillWerte();
                            break;
                        }
                }

            }
            catch
            { }

            this.Cursor = Cursors.Default;

         /*   if (ibOpenFromTimer)
            {
                MessageBox.Show(String.Format(Program.getMyLangString("msgPCNeuStart"),Environment.NewLine));
                //Program.gnvSplash.RestartTerminal();
            }
            else
            {*/
                MessageBox.Show(Program.getMyLangString("msgAppNeuStart"));
            //}
            //Program.gnvSplash.DialogShutz.appBeenden();

                try
                {
                    Program.gnvSplash.AppClose();
                }
                catch
                { }
            //this.Close();
        }


        private void SetBillWerte()
        {
            instRegistry.SubKey = isSubKey + "\\" + inetConstants.icsVerzBill;
            instRegistry.Write(inetConstants.icsBillArt, this.geldschein.SelectedIndex);
            if (this.geldschein.SelectedIndex != 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (chkBill[i].Checked)
                        instRegistry.Write(inetConstants.icsBillMf + i.ToString(), 1);
                    else
                        instRegistry.Write(inetConstants.icsBillMf + i.ToString(), 0);
                }
            }
        }

        private void SetMPWerte()
        {
            instRegistry.SubKey = isSubKey + "\\"+inetConstants.icsVerzMP;
            instRegistry.Write(inetConstants.icsMPArt, this.mpart.SelectedIndex);
            
            if (this.mpart.SelectedIndex > 4)
                instRegistry.Write(inetConstants.icsMPCom, 0); // USB
            else
            {
                String lsText = this.comport.SelectedItem.ToString();
                int p;
                lsText = lsText.Substring(3);
                try
                {
                    p = Int32.Parse(lsText);
                }
                catch { p = 0; }
                instRegistry.Write(inetConstants.icsMPCom, p);

            }
            for (int i = 0; i < 16; i++)
            {
                if (chkMP[i].Checked)
                    instRegistry.Write(inetConstants.icsMPMf + i.ToString(), 1);
                else
                    instRegistry.Write(inetConstants.icsMPMf + i.ToString(), 0);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            
            String lsPWD, lsKioskID;
            String lsParameter = String.Empty;
            String lsKennzKPExpired = Program.gnvSplash.kennzKPExpired;
            
            Program.gnvSplash.kennzKPExpired = "0";

            lsPWD = this.kpasswd.Text;
            lsKioskID = this.kioskid.Text;

            if (lsKioskID.Equals("") || lsPWD.Equals(""))
            {
                this.msgLabel.Text = Program.getMyLangString("errorKioskIDorPwd");
                Program.gnvSplash.kennzKPExpired = lsKennzKPExpired;
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            lsParameter = "action=is&2=" + lsKioskID;
            lsParameter += "&9=" + lsPWD;
            lsParameter += "&15=" + Environment.MachineName;
            lsParameter += "&11=" + DateTime.Now.ToString("yyyy-MM-dd");
            lsParameter += "&17=" + Program.gnvSplash.isClientVersion;
            lsParameter += "&8=" + Program.gnvSplash.isPcGuidLocal;

            httpcontrol localHttp = new httpcontrol();

            if (!Program.gnvSplash.ibStatusPing)
            {
                this.msgLabel.Text = Program.getMyLangString("allgInternetError");
                
                Program.gnvSplash.kennzKPExpired = lsKennzKPExpired;
                this.Cursor = Cursors.Default;
                return;
            }

            localHttp.RunAction(lsParameter, Program.gnvSplash.iActServerID, false);
            
			// if Ping=OK, but service is not available switch to another server
            if (!Program.gnvSplash.ibStatusPing)
            {
                this.msgLabel.Text = Program.getMyLangString("allgInternetError");

                Program.gnvSplash.kennzKPExpired = lsKennzKPExpired;
                //Program.gnvSplash.setPing();
                this.Cursor = Cursors.Default;
                return;
            }

			if (!localHttp.GetParameterValue("16").Equals(""))
            {
                this.msgLabel.Text = localHttp.GetParameterValue("16");
                Program.gnvSplash.kennzKPExpired = lsKennzKPExpired;
                this.Cursor = Cursors.Default;
                return;
            }

            switch (localHttp.GetParameterValue("26"))
            {
                case "1": // Nur MP Aktiv
                    {
                        tabPage2.Enabled = true;
                        tabControl2.Enabled = false;
                        break;
                    }
                case "2": // nur  Bill Validator aktiv
                    {
                        tabPage2.Enabled = false;
                        tabPage1.Enabled = true;
                        break;
                    }
                case "3": // MP && Bill Validator aktiv
                    {
                        tabPage2.Enabled = true;
                        tabPage1.Enabled = true;
                        tabControl2.Enabled = false;
                        tabControl3.Enabled = false;
                        break;
                    }
                default: // Alle AUS
                    {
                        tabPage2.Enabled = false;
                        tabPage2.Enabled = false;
                        break;
                    }
            }

            Program.gnvSplash.isModulMP = localHttp.GetParameterValue("26");
            Program.gnvSplash.isWaehrung = localHttp.GetParameterValue("27");

            this.waehrung.Text = localHttp.GetParameterValue("27");
            this.btnRegister.Enabled = false;
            this.btnCancel.Enabled = false;
            this.kioskid.ReadOnly = true;
            this.kpasswd.ReadOnly = true;
            this.kpasswd.BackColor = Color.LightGray;
            this.kioskid.BackColor = Color.LightGray;
            this.btnOK.Enabled = true;
            this.msgLabel.Text = Program.getMyLangString("successCheck");

            ibRegistered = true;

            try
            {
                inetFileCtrl inf = new inetFileCtrl();
                inf.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameRK);
                inf.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameIni);
                inf.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameUms);
                inf.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameUmsKonto);

                inf = null;
            }
            catch { }


            this.Cursor = Cursors.Default;
        }

        private void mpart_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            if (!ibRegistered)
                return;

       
            bool lbRightMPFound = false;
            bool lbgefunden = false, lbOwner = false ;
            this.msgLabel.Text = Program.getMyLangString("bittewarten");
             int count1 = -1, count2 = -1;

            Update();
            this.Cursor = Cursors.WaitCursor;

            for (int i = 0; i < 16; i++)
            {
                chkMP[i].Enabled = false;
            }
            if (this.mpart.SelectedIndex == 0)
                tabControl2.Enabled = false;

            if (this.mpart.SelectedIndex > 0 && this.mpart.SelectedIndex < 5)
            {
                if (!ibLoad)
                {
                    this.comport.Enabled = true;
                    this.comport.SelectedIndex = 0;
                }
                lbgefunden = true;
                lbRightMPFound = true;
                lbOwner = true;
                tabControl2.Enabled = false;
                for (int i = 0; i < 16; i++)
                {
                    chkMP[i].Checked = false;
                    chkMPT[i].Text = "0.00";
                    Font f = new Font(Font, FontStyle.Regular);
                    chkMPT[i].Font = f;
                    chkMPT[i].ForeColor = Color.Black;
                    switch (i)
                    {
                        case 1:
                            {
                                break;
                            }
                        case 2:
                            {
                                chkMPT[i].Text = "0.50";
                                chkMP[i].Checked = true;
                                break;
                            }
                        case 3:
                            {
                                chkMPT[i].Text = "1.00";
                                chkMP[i].Checked = true;
                                break;
                            }
                        case 4:
                            {
                                chkMPT[i].Text = "2.00";
                                chkMP[i].Checked = true;
                                break;
                            }
                    }
                    if (i > 1 && i < 5)
                    {
                        f = new Font(Font, FontStyle.Bold);
                        chkMPT[i].Font = f;
                        chkMPT[i].ForeColor = Color.Blue;
                    }
                }
            } // Ende if > 0 &&  < 5
            else if (this.mpart.SelectedIndex==1) // mit DOCK?
            {
                tabControl2.Enabled = true;
                lbgefunden = false;
            }
            else // if (this.mpart.SelectedIndex > 4 && this.mpart.SelectedIndex < 8)
            {
                this.comport.Enabled = false;
                this.comport.SelectedIndex = 1;

             
                tabControl2.Enabled = true;
                if (instDriver.searchDevices(false, whCcTalkCommunication.whPortTypes.USB) > 0)
                {
                    MPUsb localMP = instDriver.GetMPControl();
                    if (localMP != null)
                    {
                        lbgefunden = true;

                        whCcTalkCommunication.whSelCoinStatus[] lwhStatus = new whCcTalkCommunication.whSelCoinStatus[16];
                        whCcTalkCommunication.whCoinValue[] lwhValue = new whCcTalkCommunication.whCoinValue[16];
                        if (localMP.isReadCoinSelector())
                        {
                           if (localMP.IsInet())
                            {
                                lbOwner = true;

                                localMP.GetValuesAndStatus(ref lwhValue, ref lwhStatus);
                                for (int i = 0; i < lwhValue.Length; i++)
                                {
                                    chkMP[i].Checked = false;
                                    chkMPT[i].Text = "0.00";
                                    Font f = new Font(Font, FontStyle.Regular);
                                    chkMPT[i].Font = f;
                                    chkMPT[i].ForeColor = Color.Black;

                                  
                                        lbRightMPFound = true;
                                        
                                        chkMP[i].Checked = lwhStatus[i].Inhibit;
                                        chkMPT[i].Text = String.Format("{0:f}", lwhValue[i].Value);
                                        chkMPT[i].Text += Environment.NewLine + lwhValue[i].ID;
                                        chkMP[i].Enabled = false;
                                        if (lwhValue[i].Value > 0 && !lwhValue[i].ID.Equals("TK"))
                                        {
                                            if (count1 == -1 && i<= 7)
                                                count1 = i;
                                            if (i > 7 && count2 == -1)
                                                count2 = i;

                                            f = new Font(Font, FontStyle.Bold);
                                            chkMPT[i].Font = f;
                                            chkMPT[i].ForeColor = Color.Blue;
                                            chkMP[i].Enabled = true;
                                            chkMP[i].Checked = true;
                                        }
                                    
                                }
                            }
                        }
                        localMP.Close();
                        localMP = null;
                    }
                }
            }
          

            if (lbgefunden)
            {
                if (!lbOwner)
                {
                    this.msgLabel.Text = Program.getMyLangString("mpFoundFalse");
                }
                else
                {
                    if (!ibChangeMP)
                    {
                        String[] s;
                        instRegistry.SubKey = inetConstants.icsRegKey + "\\" + inetConstants.icsVerzMP;
                        s = instRegistry.ReadValueNames(inetConstants.icsRegKey + "\\" + inetConstants.icsVerzMP);
                        if (s != null && s.Length > 0)
                        {
                            uint tmp;
                            for (int i = 0; i < 16; i++)
                            {
                                tmp = instRegistry.ReadAsDWORD(inetConstants.icsMPMf + i.ToString());
                                chkMP[i].Checked = tmp.Equals(1);
                            }
                        }
                    }


                    if (count1 != -1) // erste seite aktivieren
                        tabControl2.SelectedIndex = 0;
                    else if(count2 != -1)
                        tabControl2.SelectedIndex = 1;

                    
                    if (lbRightMPFound)
                        this.msgLabel.Text = Program.getMyLangString("mpSuccessFound");
                    else if (!lbRightMPFound)
                        this.msgLabel.Text = Program.getMyLangString("mpErrorFoundII");
                }
              
            }else
                this.msgLabel.Text = Program.getMyLangString("mpErrorFound");

            ibChangeMP = true;
            this.Cursor = Cursors.Default;
        }

        private void geldschein_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!ibRegistered)
                return;

           /* if (ibLoad)
                return;
            */
            if (this.geldschein.SelectedIndex == 0)
            {
                tabControl3.Enabled = false;
                return;
            }
            tabControl3.Enabled = true;
            Boolean lbGefunden = false;

            this.msgLabel.Text = Program.getMyLangString("bittewarten");

            Update();

            this.Cursor = Cursors.WaitCursor;

            if (instDriver.searchDevices(false, whCcTalkCommunication.whPortTypes.USB) > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    chkBill[i].Enabled = false;
                }

                BillUsb localBill = instDriver.GetBilControl();
                if (localBill != null)
                {
                    lbGefunden = true;
                    whCcTalkCommunication.whBillValue[] localValue = new whCcTalkCommunication.whBillValue[16];
                    whCcTalkCommunication.whValBillStatus[] localStatus = new whCcTalkCommunication.whValBillStatus[16];

                    if (localBill.isReadBillSelector())
                    {
                        localBill.GetBillValuesAndStatus(ref localValue, ref localStatus);
                        for (int i = 0; i < localValue.Length; i++)
                        {
                            
                            chkBill[i].Checked = localStatus[i].Inhibit;
                            chkBillT[i].Text = String.Format("{0:f}", localValue[i].Value);
                            chkBillT[i].Text += Environment.NewLine + localValue[i].ID;
                            chkBill[i].Enabled = false;

                            if (localValue[i].Value > 0 && !localValue[i].ID.Equals("TK"))
                            {
                                Font f = new Font(Font, FontStyle.Bold);
                                chkBillT[i].Font = f;
                                chkBillT[i].ForeColor = Color.Blue;
                                chkBill[i].Enabled = true;
                                chkBill[i].Checked = true;
                            }
                        }
                    }
                    localBill.Close();
                    localBill = null;
                }
            }

            if (lbGefunden)
            {
                if (!ibChangeBill)
                {
                    String[] s;
                    instRegistry.SubKey = inetConstants.icsRegKey + "\\" + inetConstants.icsVerzBill;
                    s = instRegistry.ReadValueNames(inetConstants.icsRegKey + "\\" + inetConstants.icsVerzBill);
                    if (s != null && s.Length > 0)
                    {
                        uint tmp;
                        for (int i = 0; i < 16; i++)
                        {
                            tmp = instRegistry.ReadAsDWORD(inetConstants.icsBillMf + i.ToString());
                            chkBill[i].Checked = tmp.Equals(1);
                        }
                    }
                }
                this.msgLabel.Text = Program.getMyLangString("billSuccessFound");
            }
            else
                this.msgLabel.Text = Program.getMyLangString("billErrorFound");

            this.Cursor = Cursors.Default;
        }

        private void dlgEinstellung_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.gnvSplash.AppClose();
            
          //  Program.gnvSplash.DialogShutz.appBeenden();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.kioskid.BackColor = Color.White;
            this.kpasswd.BackColor = Color.White;
            this.kioskid.Enabled = true;
            this.kpasswd.Enabled = true;

            this.kioskid.ReadOnly = false;
            this.kpasswd.ReadOnly = false;

            this.btnRegister.Enabled = true;
            this.btnReset.Enabled = false;
            this.btnOK.Enabled = false;
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dlgLicence dlg = new dlgLicence();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.TopMost = true;
            dlg.winEinstellung = this;
            
           
            dlg.ShowDialog();
            
        }

        public void setLicenceInformation(String id, String argregfor)
        {
            kioskid.Text = id;
            regfor.Text = argregfor;
        }

    
    }
}