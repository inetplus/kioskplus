using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;
using kioskplus.Properties;
using System.Reflection;
using kioskplus.Windows;
using System.IO;
using kioskplus.Utils;
using System.Diagnostics;

namespace kioskplus
{
    public partial class MainInetplus : Form
    {
        Thread th;
        inetAgentCtl invAgent = null;
        String isAgentVerzeichnis = String.Empty;
        String[] isFileNames;
        public Boolean ibClosing = false;
        Boolean ibHookAtiv = true;
        List<Keys> disableKey = new List<Keys>();
        public inetCResolution displayResolution = new inetCResolution();

        private ipControl myIPControl = null;

        private dlgPhone myPhone = null;
        private int phoneVolume = 120;
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

        bool ibAnrufGestarted = false;

        inetDrucker invDrucker = null;

        public MainInetplus()
        {
            InitializeComponent();

            disableKey.Add(Keys.Delete);
            disableKey.Add(Keys.Space);
            disableKey.Add(Keys.Escape);
            disableKey.Add(Keys.BrowserBack);
            disableKey.Add(Keys.BrowserFavorites);
            disableKey.Add(Keys.BrowserForward);
            disableKey.Add(Keys.BrowserHome);
            disableKey.Add(Keys.BrowserRefresh);
            disableKey.Add(Keys.BrowserSearch);
            disableKey.Add(Keys.BrowserStop);
            disableKey.Add(Keys.Down);
            disableKey.Add(Keys.End);
            disableKey.Add(Keys.Exsel);
            disableKey.Add(Keys.Help);
            disableKey.Add(Keys.Home);
            disableKey.Add(Keys.LaunchApplication1);
            disableKey.Add(Keys.LaunchApplication2);
            disableKey.Add(Keys.LaunchMail);
            disableKey.Add(Keys.MediaNextTrack);
            disableKey.Add(Keys.MediaPlayPause);
            disableKey.Add(Keys.MediaPreviousTrack);
            disableKey.Add(Keys.MediaStop);
            disableKey.Add(Keys.Next);
            disableKey.Add(Keys.PageDown);
            disableKey.Add(Keys.PageUp);
            disableKey.Add(Keys.Pause);
            disableKey.Add(Keys.Play);
            disableKey.Add(Keys.Print);
            disableKey.Add(Keys.PrintScreen);
            disableKey.Add(Keys.Prior);
            disableKey.Add(Keys.ProcessKey);
            disableKey.Add(Keys.RWin);
            disableKey.Add(Keys.Select);
            disableKey.Add(Keys.SelectMedia);
            disableKey.Add(Keys.Zoom);
            disableKey.Add(Keys.LWin);
            disableKey.Add(Keys.F4);

            try
            {
                inetRegistry localReg = new inetRegistry();
                if (!localReg.ReadAsString(inetConstants.icsAgentDeactive).Equals("Y"))
                {
                    isAgentVerzeichnis = inetConstants.isWindowsDirectory;//Environment.GetEnvironmentVariable(inetConstants.SYSTEMROOT);
                    if (isAgentVerzeichnis.Equals(""))
                        invAgent = null;
                    else
                    {
                        try
                        {
                            invAgent = new inetAgentCtl();
                            isAgentVerzeichnis += @"\MSAgent\chars";

                            isFileNames = Directory.GetFiles(isAgentVerzeichnis);
                            invAgent.setMainSpeaker("main", isFileNames[(new Random().Next(1, isFileNames.Length)) - 1], true);
                        }
                        catch { }
                    }
                }
            }
            catch {  
                // Messagebox kann angezeigt werden
            }
        }


        delegate void ShowPhoneCallback(bool arg);
        public void ShowPhone(bool arg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowPhoneCallback(ShowPhone), new object[] {arg});
                return;
            }
            else
            {
                try
                {
                    if (arg)
                    {
                        myPhone = new dlgPhone();
                        myPhone.ShowDialog(Program.gnvSplash.DialogNewUser);
                    }
                    else
                        myPhone = null;
                }
                catch { }
            }

        }



        delegate void SetTextCallback(String asText);
        public void SetText(String asText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetTextCallback(SetText), new object[] { asText });
                return;
            }
            else
            {
               this.msgText.Text = asText;

               try
               {
                   if (Program.gnvSplash.DialogShutz != null)
                       Program.gnvSplash.DialogShutz.SetText(asText);
               }
               catch { }

               try
               {
                   SetAgentText(asText.ToString());
                   //this.Update();
               }
               catch { }
                
               
            }
                
        }

        delegate void SetVisibleCallBack(Boolean abVisible);
        public void SetVisible(Boolean abVisible)
        {
        
            if (this.InvokeRequired)
            {
                this.Invoke(new SetVisibleCallBack(SetVisible), new object[] { abVisible });
                return;
            }
            else
            {
                this.Visible = abVisible;
            }

          //  SetHideAgent();
        }

        delegate void SetCloseCallBack();
        public void SetClose()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetCloseCallBack(SetClose), new object[] { });
                return;
            }
            else
            {
                try
                {
                    if (invAgent != null)
                    {
                        invAgent.SetStop();
                        invAgent = null;
                    }
                    ibClosing = true;
                    Application.ExitThread();
                    this.Close();
                }
                catch
                {
                  //  MessageBox.Show(ex.Message);
                }
              
            }
        }


        public void SetAgentText(String asText)
        {
            try
            {
                if (invAgent != null)
                    invAgent.SetText(asText + " . . . . . . . . . . ");
            }
            catch { }

        }

        public void SetHideAgent()
        {
            try
            {
                if (invAgent != null)
                {
                    invAgent.SetHideAgent();
                }
            }
            catch { }
        }

        private void MainInetplus_Load(object sender, EventArgs e)
        {

            SetBildRandom();
            SetAgentText(label1.Text);
            this.SetText(Program.getMyLangString("bittewarten"));
            inetRegistry localReg = new inetRegistry();
            localReg.BaseRegistryKey = Registry.CurrentUser;

            localReg.SubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            localReg.WriteNoCrypt("DisableTaskMgr", "0");// Ctrl + Alt + Del Sperren
            localReg.WriteNoCrypt("DisableRegistryTools", 1/*long.Parse("1")*/); //Registry sperren


            localReg.SubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            localReg.WriteNoCrypt("NoDesktop", "1");// Desktop Icons wegblenden
            localReg.WriteNoCrypt("NoAddPrinter", long.Parse("1"));// Hinzufügen von Druckern sperren
            localReg.WriteNoCrypt("NoDeletePrinter", long.Parse("1")); // Delete Printer sperren
            localReg.WriteNoCrypt("NoPrinterTabs", long.Parse("1"));
            localReg.WriteNoCrypt("NoControlPanel", long.Parse("1")); // 20100130

           /**** SBSB */
            // i-client autostart aktivieren
            localReg.SubKey = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";
            localReg.WriteNoCrypt("shell", "icshell.exe");
            /****************/

            // windows vista, User Account Protection deaktivieren
            localReg.BaseRegistryKey = Registry.LocalMachine;
            localReg.SubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            localReg.WriteNoCrypt("EnableLUA", 0);

            try
            {
                Program.gnvSplash.gnvMainInetplus = this;
                th = new Thread(new ThreadStart(Program.gnvSplash.AppStart));
                th.Name = "inetstart";
                th.Priority = ThreadPriority.Normal;
                th.SetApartmentState(ApartmentState.STA);

                th.Start();
            }
            catch
            {
            }


            try
            {
                inetHook.InstallHook(true);
            }
            catch { }

            try
            {
                startPhoneControl();
            }
            catch { }
        }

        public void stopPhoneControl()
        {
            try
            {
                if (myIPControl != null)
                    myIPControl.setStop();
            }
            catch { }
        }

        public void startPhoneControl()
        {
            try
            {
                myIPControl = new ipControl();
                myIPControl.Checked += new ipControl.checkData(myIPControl_Checked);
                myIPControl.setStart();
                if (!myIPControl.gestarted)
                    myIPControl = null;
                else
                {
                    // Timer Starten
                }
            }
            catch { }
        }

        void myIPControl_Checked(int wMessage, int wParam, int lParam)
        {
            switch (wMessage)
            {
                case WM_HID_DEV_ADDED:
                    Console.WriteLine("Added");
                    break;
                case WM_HID_DEV_REMOVED:
                    Console.WriteLine("Removed");
                    if (myIPControl != null)
                        myIPControl = null;
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

                                if (Program.gnvSplash.DialogNewUser == null)
                                    return;


                                if (myPhone != null)
                                {
                                    myPhone.GespraechStarten(true);
                                }
                                ibAnrufGestarted = true;
                                break;
                            case 20: // Anruf beenden
                                if (!ibAnrufGestarted)
                                    myIPControl.TextToMitte = "";

                                if (myPhone != null)
                                    myPhone.CancelCall();

                                ibAnrufGestarted = false;
                                break;
                            case 10: // einzelne löschen
                                if (myIPControl.TextToMitte.Length > 0)
                                    myIPControl.TextToMitte = myIPControl.TextToMitte.Substring(0, myIPControl.TextToMitte.Length - 1);
                                break;
                            case 21: // Sound ++
                                if (myPhone != null)
                                {
                                    phoneVolume +=5;
                                    if (phoneVolume > 255)
                                        phoneVolume = 255;
                                    myPhone.setSound(phoneVolume);
                                }
                                break;
                            case 22: // Sound --
                                if (myPhone != null)
                                {
                                    phoneVolume -=5;
                                    if (phoneVolume < 0)
                                        phoneVolume = 0;
                                    myPhone.setSound(phoneVolume);
                                }
                                break;
                        }
                        
                        if (myPhone != null && myIPControl != null)
                            myPhone.setTelefonText(myIPControl.TextToMitte);

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



        private void SetBildRandom()
        {
            Random rd = new Random();
            int index;
            index = rd.Next(1, 6);
            Bitmap[] p = { kioskplus._1, kioskplus._2, kioskplus._3, kioskplus._4, kioskplus._5, kioskplus._6 };
            pictureBox1.Image = p[index];
        }

        delegate void OpenSchutzCallBack();
        public void OpenSchutzSplash()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new OpenSchutzCallBack(OpenSchutzSplash), new object[] { });
                return;
            }
            else
            {
                try
                {
                    SetHook(true);
                    Program.gnvSplash.BlockMaus(true);
                }
                catch { }

                try
                {
                  Program.gnvSplash.KillAllOtherProcess();
                }
                catch
                { }

                try
                {
                    displayResolution.SetResolution();

                }
                catch { }

                try
                {
                    if (Program.gnvSplash.DialogNewUser != null)
                    {
                        Program.gnvSplash.DialogNewUser.SetClose();
                    }

                    if (Program.gnvSplash.DialogShutz != null)
                        Program.gnvSplash.DialogShutz.Show();
                    else
                    {
                        Program.gnvSplash.DialogShutz = new dlgSchutzSplash();
                        Program.gnvSplash.DialogShutz.Show();
                    }

                    try
                    {
                        Program.gnvSplash.BlockMaus(false);
                    }
                    catch { }

                    Thread t = new Thread(new ThreadStart(backDelThread));
                    t.Start();

                    try
                    {
                        if (invDrucker != null)
                        {
                            invDrucker.stopPrintMonitor();

                            invDrucker = null;
                        }
                    }
                    catch
                    { }


                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:" + e.Message); // MessageBox.Show("OpenSchutzSplash" + e.Message);
                }

              
            }
        }


        private void backDelThread()
        {
            try
            {
                if (File.Exists(Program.gnvSplash.isSystemDrive + "netuses.bat"))
                    System.Diagnostics.Process.Start(Program.gnvSplash.isSystemDrive + "netuses.bat");

                if (File.Exists(inetConstants.isSystemDirectory + "\\InetCach.exe"))
                {
                    System.Diagnostics.Process pApplication = new System.Diagnostics.Process();

                    pApplication.StartInfo.FileName = inetConstants.isSystemDirectory + "\\InetCach.exe";
                    pApplication.StartInfo.Arguments = "inet+";
                    pApplication.StartInfo.CreateNoWindow = true;
                    pApplication.StartInfo.UseShellExecute = false;
                    pApplication.Start();
                }
            }
            catch (Exception ex) {
                //MessageBox.Show (ex.Message);
            }

        }


        delegate void OpenNewUserCallBack();
        public void OpenNewUser()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new OpenNewUserCallBack(OpenNewUser), new object[] { });
                return;
            }
            else
            {
                try
                {
                    try
                    {
                        Thread th = new Thread(new ThreadStart(callFromOpenNewUser));
                        th.Name = "ipcalluser";
                        th.Priority = ThreadPriority.Normal;
                        th.Start();
                        th = null;
                    }
                    catch { }  

                    if (Program.gnvSplash.DialogShutz != null)
                    {
                        Program.gnvSplash.DialogShutz.SetCloseWindow();
                        Program.gnvSplash.DialogShutz = null;
                    }

                    if (Program.gnvSplash.DialogNewUser != null)
                       Program.gnvSplash.DialogNewUser.Show();

                   Program.gnvSplash.SetShowTrayWnd(true);

                   try
                   {
                       SetHook(false);
                       Program.gnvSplash.BlockTaste(false);
                       Program.gnvSplash.BlockMaus(false);
                   }
                   catch { }




                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);//MessageBox.Show("openNewUser" + ex.Message);
                }

//                try
//                {
//Console.WriteLine("GetAllProcess");
//                    Program.gnvSplash.GetAllProcess();
//                }
//                catch
//                { }

                //try
                //{
                //    if (File.Exists(Program.gnvSplash.isSystemDrive + "netusef.bat"))
                //        System.Diagnostics.Process.Start(Program.gnvSplash.isSystemDrive + "netusef.bat");

                //    if (Program.gnvSplash.ibOlingooOpen)
                //    {
                //        string ls;
                //        ls = Program.gnvSplash.isLanguage;
                //        if (ls.Length >2)
                //            ls = ls.Substring(0,2);

                //        System.Diagnostics.Process.Start("http://www.olingoo.net?lang="+ls);

                //    }
                //    Program.gnvSplash.ibNeuStartNow = true;
                //}
                //catch { }
            }
        }
        private void callFromOpenNewUser()
        {

            try
            {
                Console.WriteLine("GetAllProcess");
                Program.gnvSplash.GetAllProcess();
            }
            catch
            { }

            try
            {
                if (Program.gnvSplash.idcDruckKosten > 0 && !Program.gnvSplash.kennzKPExpired.Equals("1"))
                {
                    invDrucker = new inetDrucker();
                    invDrucker.startPrintMonitor();
                }
            }
            catch { }


            try
            {
                if (File.Exists(Program.gnvSplash.isSystemDrive + "netusef.bat"))
                    System.Diagnostics.Process.Start(Program.gnvSplash.isSystemDrive + "netusef.bat");

                if (Program.gnvSplash.ibOlingooOpen)
                {
                    string ls;
                    ls = Program.gnvSplash.isLanguage;
                    if (ls.Length > 2)
                        ls = ls.Substring(0, 2);

                    System.Diagnostics.Process.Start("http://www.olingoo.net?lang=" + ls);
                }

                Program.gnvSplash.ibNeuStartNow = true;
            }
            catch { }


        }
        private void MainInetplus_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!ibClosing)
            if (!ibClosing && (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.UserClosing))
            {
                e.Cancel = true;
            }
            else
            {
                myIPControl.setStop();
            }
        }

        internal void SetNewAgent()
        {
            invAgent.SetNewAgent("main", isFileNames[(new Random().Next(1, isFileNames.Length)) - 1],true);
        }

        internal void SetAgentHide(Boolean abSwitch)
        {
            if (abSwitch)
            {
                invAgent = new inetAgentCtl();
                invAgent.setMainSpeaker("main", isFileNames[(new Random().Next(1, isFileNames.Length)) - 1], true);
            }
            else
            {
                invAgent.SetStop();
                invAgent = null;
            }
            
        }

        private void inetHook_KeyUp(object sender, WindowsHookLib.KeyEventArgs e)
        {
            if (disableKey.Contains(e.KeyCode) || (e.Alt && e.KeyCode == Keys.F4) || (e.Alt && e.KeyCode == Keys.Escape))
                e.Handled = ibHookAtiv;
            else
                e.Handled = false;
             
        }

        private void inetHook_KeyDown(object sender, WindowsHookLib.KeyEventArgs e)
        {
            if (disableKey.Contains(e.KeyCode) || (e.Alt && e.KeyCode == Keys.F4) || 
                (e.Alt && e.KeyCode == Keys.Escape) || (e.Alt && e.KeyCode == Keys.Tab))
                e.Handled = ibHookAtiv;
               
        }

        private void inetHook_StateChanged(object sender, WindowsHookLib.StateChangedEventArgs e)
        {
            
        }

        public void SetHook(Boolean abSwitch)
        {
            ibHookAtiv = abSwitch;
        }

    }
}