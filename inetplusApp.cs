using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Management;
using System.Windows.Forms;
using kioskplus.Windows;

using System.Runtime.InteropServices;
using kioskplus.Utils;
using System.Threading;
using System.Collections.Generic;
using NistServer;

namespace kioskplus
{
    public class inetplusApp
    {
        Boolean lbInternet = false;

        // aktuelle Sprache
        public String isLanguage	= ""; // Standard Deutsch
        public String isWaehrung	= "EUR";
        public String isWaehrungCent = "Cent";
        public String isLandVorwahl = "49";

        // Auch benutzt für: Falls anhand eines Coupons ein Aufladen nicht möglich ist. 
        public Boolean ibAufladen	= true, ibKennzBonplus= false;
        public Boolean ibKennzKunde = false, ibKennzSMS=false, ibKennzVoip=false,ibKennzUMS=false, ibKennzSpiele=false;
        public Boolean ibKennzShutdownBtn = false;

        // Freischaltungsvariablen alles aus der DB!
        public String isKioskID = String.Empty;
        public String isPWD = String.Empty, isReg = "inetplus";
        public String isPcGuid = String.Empty, isKunde = String.Empty, isGesperrt = "0";
        public String isPcGuidLocal = String.Empty, isPCName = String.Empty;
        public String isWerbeText = String.Empty;

        public String isModulMP = "0";
        public String isSkinName = "standard";

        public int iiAnzahl;
        public int iTerminalID, iStandortID, iAufstellplatzID, iFreeSMS;

        // Web-Url
        public String isWWW = String.Empty;
        public String isWWWLocal = String.Empty;
        public String isWWWPar = String.Empty;

        public int iKundenID;
        public String isKundenID = "Surfer", isAnzNameFull="Surfer";

        // Gesperrt laut mysql, oder weil DEMO -> Anzeige der Website hierüber!
        Boolean ibGesperrt=false;

        // Reboot/Shutdown-Felder
        public DateTime idtStatReboot, idtStatRebootPlus1Min;
        public DateTime idtStatShutDown, idtStatShutDownPlus1Min;
        public Boolean ibStatReboot=false;
        public Boolean ibStatShutDown=false;
        public Boolean ibNeuStartAuto = false;
        
        public Double idcDruckKosten;
        public Double idcStundenPreis;

        String[] isCloseFiles;
        Boolean ibUpdateInProgress = false;

        public Boolean ibFreigabe = false;
        //
        // Ping erst beim 4 Timer-Aufruf
        int iiPing = 2;
        

        public int il_hwnd1=0;// Taskbar unten
        public int il_hwnd2=0;// Progman Desktop "Program Manager"
        public int il_hwnd3=0;// Merlin

        List<string> isTaste = new List<string>();

        public inetRegionCtrl iNetRegionControl = null;

        public String isClientVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        // OK
        public String isVerzNoNet = "";
        public MainInetplus gnvMainInetplus = null;
        public Boolean ibStatusPing = false;

        // private int intCreateMutex;
        ManagedHook managedKeyboard = null;
        inetPing invPing = null;
        httpcontrol inetHttpControl = new httpcontrol();

        public List<String> mServerList = null;
        
        public int iPort = 84;

        inetRegistry instRegistry = new inetRegistry();
        public dlgSchutzSplash DialogShutz = null;
        public dlgNewUser DialogNewUser = null;

        MPControl invMPControl = null;
        UsbDriversCtrl invUsbCtrl = null;
        inetDateTime invDateTime = null;
      

        // Email
        public String isEmailServer = String.Empty, isEmailUser = String.Empty;
        public String isEmailPwd = String.Empty,isEmailAdr = String.Empty;
        public String isEmailAnzeige = String.Empty, isEmailFooter = String.Empty;

        // Process
        inetProcess allActiveProcess = new inetProcess();
        Boolean ibStartCheckVerbindung = false;
        public Boolean ibOwner = false;

        // WebProxy-Einstellung
        public String isProxyUser = String.Empty, isProxyPwd = String.Empty, isProxyAdress = String.Empty, isProxyPort=String.Empty;


        public String isLinkAdresse = String.Empty;
        int mIndex = -1;
        public bool ibOlingooOpen = true;

        // My Currencies
        private Hashtable myCurrencies = new Hashtable();

        public bool ibNeuStartNow = false;
        private Thread tBack = null;
        public bool ibUSBCtrlPolled = false;

        //20100130
        public String isSystemDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
        public String isFileUrl = "file:///" + inetConstants.isSystemDirectory + "\\$isp$\\";
        public String isFileUrlIP = inetConstants.isSystemDirectory + "\\$isp$\\";
        public String isFileUrlIPT = inetConstants.isSystemDirectory + "\\$itmcp$\\";

        public String kennzKPExpired = "0";

        public int iActServerID = 0;

       /// private List<InetServletService> ipService = null;
        public String isKontent = "/kioskService/";

        public HttpStatusCode[] ipHttpStatusCode;
        public int countRequest = 0;

        public int ibAffili = 0;

        public String isGamesLink = String.Empty;

        // NistServer von Backend (IP-Adresse)
        public String isNistServer = "";

        public inetplusApp()
        {
           
           
            mServerList = new List<String>();
            mServerList.Add("inetplus.de");
            mServerList.Add("inetplus.de");

            ipHttpStatusCode = new HttpStatusCode[mServerList.Count];
            for (int ii = 0; ii < ipHttpStatusCode.Length; ii++)
                ipHttpStatusCode[ii] = HttpStatusCode.OK;

            // 
            if (!isSystemDrive.EndsWith("\\"))
                isSystemDrive = isSystemDrive + "\\";

            isVerzNoNet = instRegistry.ReadAsString(inetConstants.icsVerzKey);
            if (isVerzNoNet.Equals(""))
                isVerzNoNet = inetConstants.isSystemDirectory;

            if (isVerzNoNet.LastIndexOf("\\") <= 0)
                isVerzNoNet = isVerzNoNet + "\\";

            RecordKey iRKey = null;
            try
            {
                inetFileCtrl lfc = new inetFileCtrl();
                
                iRKey = lfc.readKeyFile(isVerzNoNet + "\\" + inetConstants.isFileNameRK);
                kennzKPExpired = iRKey.kennzKPExpired;
                if (kennzKPExpired == null)
                    kennzKPExpired = "0";

                lfc = null;
                iRKey = null;
            }
            catch
            {
                iRKey = null;
            }

        }


        public void AppStart()
        {

            String lsVol = "";
            int liReturn=0;

            gnvMainInetplus.SetText(Program.getMyLangString("bittewarten"));
            
            // Tastatur sperren
             BlockTaste(true);
            
            // Maus sperren
            BlockMaus(true);

            Win32User.SystemParametersInfo(Win32User.SPI_SCREENSAVERRUNNING, 1, true, 0);

            SetShowTrayWnd(false);

            
            gnvMainInetplus.SetText(Program.getMyLangString("objLaden"));
            invPing = new inetPing("<changeme>", 200);
            gnvMainInetplus.SetText(Program.getMyLangString("sucheInternet"));

           ibStatusPing = false;
           // Changed 22.01 ibStatusPing = true;
           setPing();

            gnvMainInetplus.SetText(Program.getMyLangString("zeitZone"));
            SetZeitZone(true); // Set date time
            
            PruefTaskBar();
            SetIgnore(false);
            
            gnvMainInetplus.SetText(Program.getMyLangString("readGUID"));
            isPcGuidLocal = GetMacAdress();

            //20100130
            String lsLocalVolume = isSystemDrive.Substring(0,isSystemDrive.IndexOf(":"));
            if (lsLocalVolume.Length <= 0)
                lsLocalVolume = "C";

            lsVol = GetSerialNumber(lsLocalVolume);
            isPcGuidLocal = lsVol +isPcGuidLocal;
            if (isPcGuidLocal.Length > 30)
                isPcGuidLocal = isPcGuidLocal.Substring(0, 30);

            gnvMainInetplus.SetText(Program.getMyLangString("fillinstance"));
            isKioskID = instRegistry.ReadAsString(inetConstants.icsKioskID);
            
            if (isKioskID.Equals("")) 
                inetHttpControl.SetNullHashMap();

            gnvMainInetplus.SetText(Program.getMyLangString("checkID"));
            if (checkKioskID(isKioskID).Equals("-1"))
            {
                gnvMainInetplus.SetText(Program.getMyLangString("closeApp"));
                return;
            }

            liReturn = dbLesen();

            if (lsVol.Equals(""))
                if (liReturn == -1)
                    ibGesperrt = true;

            gnvMainInetplus.SetText(Program.getMyLangString("readKeyAndPrise"));
            SetKeys();
            if (isKioskID.Equals(""))
                ibGesperrt = false;

            gnvMainInetplus.SetText(Program.getMyLangString("bittewarten"));
            isCloseFiles=instRegistry.ReadValueNames(inetConstants.icsCloseFiles);
            
            if (!ibGesperrt)
            {
                if (!isWWW.Equals("") && !isKioskID.Equals(""))
                {
                    String lsURL;
                    gnvMainInetplus.SetText(Program.getMyLangString("checkSperrSeite"));//
                    lsURL = GetPCNumber();
                    if (!lsURL.Equals(""))
                    {
                        isWWWPar += "?pn=" + lsURL;
                    }

                    String lsTmp = String.Empty;
                    if (!lsURL.Equals(""))
                    {
                        lsURL += ".jpg";
                        lsTmp = isWWW + "/bilder/" + lsURL;
                    }
                    if (inetHttpControl.fileDownload(isWWW + "/free.zip", isFileUrlIP, "free.zip"))
                        isWWWLocal = isFileUrl + "index.html";

                    if (!lsURL.Equals(""))
                        inetHttpControl.BildDownload(lsTmp, isFileUrlIP + "bilder\\" + lsURL);
                }
                else // welcome.html beim Installieren
                    isWWWLocal = isFileUrl +"welcome.html"; 

            }
           gnvMainInetplus.SetText(Program.getMyLangString("successEnde"));

           try
           {
               if (File.Exists(inetConstants.isSystemDirectory + "\\iskin.xml"))
                   (new inetXMLReader()).readXMLFile(inetConstants.isSystemDirectory + "\\iskin.xml", isSkinName, ref iNetRegionControl);
           }
           catch { }

           
            try
            {
                Thread t = new Thread(new ParameterizedThreadStart(this.SetControlModul));
                t.SetApartmentState(ApartmentState.STA);
                t.Priority = ThreadPriority.Highest;
                t.Name = "inet01";
                t.Start(true);
            }
            catch { }

            try
            {
                gnvMainInetplus.SetVisible(false);
                if (!ibGesperrt && isAppOpen())
                {
                    DialogNewUser = new dlgNewUser();
                    DialogNewUser.SetFreigabe(isKundenID,  invDateTime.GetOnlineZeitAsString(), invDateTime);
                    gnvMainInetplus.OpenNewUser();
                }
                else
                {
                    gnvMainInetplus.OpenSchutzSplash();
                }
            }
            catch { }

            try
            {
                if (kennzKPExpired.Equals("1"))
                {
                    Thread tt = new Thread(new ThreadStart(this.isKPExpired));
                    tt.SetApartmentState(ApartmentState.STA);
                    tt.Priority = ThreadPriority.Highest;
                    tt.Name = "inetkpexpired";
                    tt.Start();
                    tt = null;
                }
            }
            catch { }



        }

        private void isKPExpired()
        {
            int iAnz = 0;
            httpcontrol local = new httpcontrol();
            while (iAnz < 5 && gnvMainInetplus.ibClosing == false)
            {
                iAnz++;
                try
                {
                    local.RunAction("action=getrkexp&his=3&2=" + isKioskID, Program.gnvSplash.iActServerID, false);
                    if (local.ibRunAction)
                    {
                        iAnz = 6;
                        isNistServer = local.GetParameterValue("9");
                        if (local.GetParameterValue("5").Equals("1") ||
                            (!local.GetParameterValue("8").Equals("")
                                && !local.GetParameterValue("8").Equals(isPcGuidLocal)))
                        {
                            isGesperrt = "1";
                            isWWWLocal = String.Empty;
                            isWWW = local.GetParameterValue("7");

                            if (DialogShutz != null)
                                DialogShutz.SetNavigateSeite();

                            SetControlModul(false);
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(5 * 1000);
                    }
                }
                catch { }
            }
            local = null;
        }





        private int doPing()
        {
            if (mIndex != -1)
            {
                if (invPing.PingHost(mServerList[mIndex]) != -1)
                    return mIndex;
            }

            ibStatusPing = false;
            for (int index = 0; index < mServerList.Count; index++)
            {
                int ergebnis = invPing.PingHost(mServerList[index]);
                if (ergebnis != -1)
                {
                    ibStatusPing = true;
                    mIndex = index;
                    return index;
                }
            }
            return -1;
        }
        
        public int setPing()
        {
            if (kennzKPExpired.Equals("1"))
            {
                return -1;
            }

            if (ibStartCheckVerbindung)
                return -1;
            try
            {
                int rc = doPing();
                if (rc != -1)
                    return rc;
            }
            catch { }
            if (!ibStatusPing && !ibStartCheckVerbindung)
            {
                ibStartCheckVerbindung = true;
                // starte thread im background um zu überprüfen ob Internet-Verbindung da ist?
                // wenn ja, soll noch ein Funk. aufgerufen werden.... aus der Sperrseite logon zu zeigen
                StartThreadToCheckVerbindung();
            }

            return -1;

        }

        private void StartThreadToCheckVerbindung()
        {
                try
                {
                    if (tBack == null)
                    {
                        if (DialogShutz != null)
                        {
                            try
                            {
                                DialogShutz.SetNavigateSeite();
                            }
                            catch { }
                        }

                        tBack = new Thread(new ThreadStart(checkVerbindung));
                        tBack.SetApartmentState(ApartmentState.STA);
                        tBack.Name = "checkVerbindung";
                        tBack.Priority = ThreadPriority.Normal;
                        tBack.Start();


                    }
                }
                catch (Exception ex)
                { Console.WriteLine("Err:" + ex.Message); }
        }

        private void checkVerbindung()
        {
            ibStartCheckVerbindung = true;
            
            do
            {
                try
                {
                   
                    if (doPing() != -1)
                    {
                        lbInternet = true;
                        ibStatusPing = true;
                    }
                }
                catch (Exception ex)
                { Console.WriteLine("Exception:" + ex.Message); }

                System.Threading.Thread.Sleep(5000);
            } while (!lbInternet);

            
            lbInternet = false;

            if (DialogShutz != null)
            {
                try
                {
                    DialogShutz.SetNavigateSeite();
                }
                catch { }
            }

            ibStartCheckVerbindung = false;
            try
            {
                tBack = null;
            }
            catch { }

        }


       


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean isAppOpen()
        {
            StringBuilder   strQuery = new StringBuilder();
            String lsTmp = String.Empty;

            strQuery.Append("action=getregkeyini")
                    .Append("&1=").Append(iTerminalID);

            inetHttpControl.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);

            if (inetHttpControl.GetParameterValue("1").Equals(""))
                return false;

            
            if (invDateTime == null)
                invDateTime = new inetDateTime();

            isKundenID = inetHttpControl.GetParameterValue("2");
            iKundenID = Int32.Parse(inetHttpControl.GetParameterValue("9"));
            
            lsTmp = inetHttpControl.GetParameterValue("3") + " " + inetHttpControl.GetParameterValue("5");
            
            invDateTime.SetStartDateTime(DateTime.Parse(lsTmp));
            
            lsTmp = inetHttpControl.GetParameterValue("4") + " " + inetHttpControl.GetParameterValue("6");

            invDateTime.SetEndeDateTime(DateTime.Parse(lsTmp));
            
            ibAufladen = false;
            if (inetHttpControl.GetParameterValue("11").Equals("1"))
                ibAufladen = true;

            if (invDateTime.GetRestDauerAsMin() <= 0) // delete row from db
            {
                String lsRef = invDateTime.GetGesamtOnlineZeit();
                SetUmsatz(0, ref lsRef, "3", 0,"");
                return false;
            }


            // switch Language
            if (!inetHttpControl.GetParameterValue("12").Equals(Program.defaultLanguage))
            {
              Program.switchLanguage(inetHttpControl.GetParameterValue("12"));
            }

            ibKennzBonplus = false;
            if (inetHttpControl.GetParameterValue("13").Equals("1"))
            {
                ibKennzBonplus = true;
            }

            ibFreigabe = true;

            return true;
        }

     
        public void RestartTerminal()
        {
            inetAPI.PCReboot();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abSwitch"></param>
        public void SetControlModul(object state)
        {
            try
            {
                
                Boolean abSwitch;

                abSwitch = (Boolean)(state);
                
                if (isKioskID.Equals(""))
                    return;

                if (!abSwitch)
                {
                    try
                    {
                        if (invMPControl != null)
                        {
                            invMPControl.stopPolling();
                            invMPControl = null;
                        }


                        if (invUsbCtrl != null)
                        {
                            invUsbCtrl.StopPolling();
                            invUsbCtrl = null;

                            if (Program.gnvSplash.DialogShutz != null)
                                Program.gnvSplash.DialogShutz.SwitchColor(System.Drawing.Color.Red, System.Drawing.Color.Red);
                            
                        }

                    }
                    catch { }
                    return;
                }


                String lsTmp;
                lsTmp = instRegistry.SubKey;
                uint ergebnis = 0;
                
                switch (isModulMP)
                {
                    case "1": // nur Münzprüfer
                        {
                            instRegistry.SubKey = inetConstants.icsRegKey + "\\" + inetConstants.icsVerzMP;
                            ergebnis = instRegistry.ReadAsDWORD(inetConstants.icsMPArt);

                            if (ergebnis > 4)
                            {
                                if (invUsbCtrl == null)
                                {
                                    invUsbCtrl = new UsbDriversCtrl();
                                }
                           
                                invUsbCtrl.StartPolling("1");

                            }
                            else if (ergebnis > 0 && ergebnis <= 4)
                            {
                                ibOwner = true;
                                invMPControl = new MPControl();
                                invMPControl.startPolling();
                            }
                           

                                

                            break;
                        }
                    case "2": // nur BillValidator
                        {
                            if (invUsbCtrl == null)
                            {
                                invUsbCtrl = new UsbDriversCtrl();
                            }

                            invUsbCtrl.StartPolling("2");
                            break;
                        }
                    case "3": // MP && Bill Validator
                        {

                            if (invUsbCtrl == null)
                            {
                                invUsbCtrl = new UsbDriversCtrl();
                            }

                            invUsbCtrl.StartPolling("3");

                            break;
                        }
                }

                if (!ibOwner)
                {
                    if (DialogShutz != null)
                        DialogShutz.SetMPText("");
                }
            }
            catch { }
        }


        public void ReadEmailData()
        {
            inetHttpControl.RunAction("action=df", Program.gnvSplash.iActServerID, false);
            if (inetHttpControl.GetParameterValue("1").Equals(""))
                return;

            isEmailServer = inetHttpControl.GetParameterValue("1");
            isEmailUser = inetHttpControl.GetParameterValue("2");
            isEmailPwd =  inetHttpControl.GetParameterValue("3");
            isEmailAdr = inetHttpControl.GetParameterValue("4");
            isEmailAnzeige = inetHttpControl.GetParameterValue("5");
            isEmailFooter =  inetHttpControl.GetParameterValue("10") +
                             inetHttpControl.GetParameterValue("11") +
                             inetHttpControl.GetParameterValue("12") +
                             inetHttpControl.GetParameterValue("13") +
                             inetHttpControl.GetParameterValue("14") +
                             inetHttpControl.GetParameterValue("15");
        }
       

        private String GetPCNumber()
        {
            String lsTmp = String.Empty;
            Boolean lbTmp = false;
            uint liTmp;

            if (kennzKPExpired.Equals("1"))
                return "";

            if (!isPCName.Equals(""))
            {
                lsTmp = isPCName;

                try
                {
                    // 20100130 fehlende Trims
                    if (isPCName.Length > 2)
                    {
                        lsTmp = isPCName.Substring(isPCName.Length - 2).Trim();
                        lbTmp = UInt32.TryParse(lsTmp, out liTmp);
                    }
                }
                catch { }

                try
                {
                    if (!lbTmp && isPCName.Length > 1)
                    {
                        lsTmp = isPCName.Substring(isPCName.Length - 1).Trim();
                        lbTmp = UInt32.TryParse(lsTmp, out liTmp);
                    }
                }
                catch { }

                if (lbTmp)
                {
                    if (lsTmp.Length == 1)
                        lsTmp = "0" + lsTmp;

                    lbTmp = UInt32.TryParse(lsTmp, out liTmp);
                    if (lbTmp)
                        if (liTmp > 0)
                            return lsTmp;
                }
            }

            return "";
        }

        // noch nicht ganz fertig
        private void SetLockFiles(Boolean abSwitch)
        {
            if (ibUpdateInProgress)
                return;

            if (abSwitch)
            {
                return;
            }
        }

        public List<string> GetKeys()
        {
            return isTaste;
        }

        private void SetKeys()
        {
            String lsKombi;
            char[] c={'+'};

            lsKombi = instRegistry.ReadAsString(inetConstants.icsKombi);
            if (lsKombi.Equals("") || lsKombi.Split(c).Length <3)
                lsKombi = "CONTROL+ALT+N";

            isTaste.AddRange(lsKombi.Split(c));

            if (isTaste.Count == 3)
            {
                for (int i = 0; i < isTaste.Count; i++)
                {
                    if (isTaste[i].ToLower().Equals("strg"))
                    {
                        isTaste[i] = "CONTROL";
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int dbLesen()
        {
            int liReturn = 0;

            if (!isPcGuid.Equals(isPcGuidLocal) && !isKioskID.Equals(""))
            {
                if (isPcGuid.Length > 7 && isPcGuidLocal.Length > 7 )
                if (!isPcGuid.Substring(0, 8).Equals(isPcGuidLocal.Substring(0, 8)))
                {
                    liReturn = -1;
                    isWWW = inetConstants.icsBlockSeite;
                    
                }
            }

            if (!isGesperrt.Equals("0"))
            {
                liReturn = -1;
                isWWW = inetConstants.icsBlockSeite;
            }

            if (liReturn != 0 || isKioskID.Equals(String.Empty))
            {
                isModulMP = "0";
                //close all Moduls
            }

            if (isKioskID.Equals(String.Empty))
                isWWW = inetConstants.icsSperrSeite;

            
            if (liReturn == -1)
                ibGesperrt = true;

            return liReturn;
        }


        /// <summary>
        /// 
        /// </summary>
        private void fillInstance()
        {
            String lsTmp = "";

            isGesperrt = inetHttpControl.GetParameterValue("5");// Gesperrt?
            isPCName = inetHttpControl.GetParameterValue("15"); // PCName
            isWWW = inetHttpControl.GetParameterValue("7");
            isWWWPar = inetHttpControl.GetParameterValue("52");
            isWerbeText = inetHttpControl.GetParameterValue("60");

            iiAnzahl = 0;
            if (!inetHttpControl.GetParameterValue("10").Equals(""))
                iiAnzahl = Int32.Parse(inetHttpControl.GetParameterValue("10"));
            
            isKunde = inetHttpControl.GetParameterValue("22") + "," + 
                      inetHttpControl.GetParameterValue("23") + "," +
                      inetHttpControl.GetParameterValue("24");

            lsTmp = inetHttpControl.GetParameterValue("61"); // Waehrung
            if (!lsTmp.Equals(""))
                isWaehrung = lsTmp;

            lsTmp = inetHttpControl.GetParameterValue("62"); // Waehrung Cent
            if (!lsTmp.Equals(""))
                isWaehrungCent = lsTmp;

            ibKennzKunde = true;
            lsTmp = inetHttpControl.GetParameterValue("27"); // Kundenkonto
            if (lsTmp.Equals("") || lsTmp.Equals("0"))
                ibKennzKunde = false;

            lsTmp = inetHttpControl.GetParameterValue("63"); // Nr land
            if (!lsTmp.Equals(""))
                isLandVorwahl = lsTmp;

            ibKennzSMS = false;
            lsTmp = inetHttpControl.GetParameterValue("58");
            if (lsTmp.Equals("1"))
                ibKennzSMS = true;

            ibKennzVoip = false;
            lsTmp = inetHttpControl.GetParameterValue("57");
            if (lsTmp.Equals("1"))
                ibKennzVoip = true;

            ibKennzSpiele = false;
            lsTmp = inetHttpControl.GetParameterValue("59");
            if (lsTmp.Equals("1"))
                ibKennzSpiele = true;

            ibKennzShutdownBtn = false;
            lsTmp = inetHttpControl.GetParameterValue("56");
            if (lsTmp.Equals("1"))
                ibKennzShutdownBtn = true;

            ibKennzUMS = false;
            lsTmp = inetHttpControl.GetParameterValue("28");
            if (lsTmp.Equals("1"))
                ibKennzUMS = true;

            isReg = isKunde;
            isPcGuid = inetHttpControl.GetParameterValue("8");
            isModulMP = inetHttpControl.GetParameterValue("26");
            lsTmp = inetHttpControl.GetParameterValue("55");
          
                switch (lsTmp)
                {
                    case "1":
                        isSkinName = "modern";
                        break;
                    case "2":
                        isSkinName = "logo";
                        break;
                    case "3":
                        isSkinName = "quadrat";
                        break;
                    case "4":
                        isSkinName = "rund";
                        break;
                    case "5":
                        isSkinName = "rundb";
                        break;
                    default:
                        isSkinName = "standard";
                        break;
                }

                iTerminalID = 0;
                if (!inetHttpControl.GetParameterValue("1").Equals(""))
                    iTerminalID = Int32.Parse(inetHttpControl.GetParameterValue("1"));

                iStandortID = 0;
                if (!inetHttpControl.GetParameterValue("3").Equals(""))
                    iStandortID = Int32.Parse(inetHttpControl.GetParameterValue("3"));

                iAufstellplatzID = 0;
                if (!inetHttpControl.GetParameterValue("45").Equals(""))
                    iAufstellplatzID = Int32.Parse(inetHttpControl.GetParameterValue("45"));

                iFreeSMS = 0;
                if (!inetHttpControl.GetParameterValue("46").Equals(""))
                    iFreeSMS = Int32.Parse(inetHttpControl.GetParameterValue("46"));

                gnvMainInetplus.SetText(Program.getMyLangString("readBootProperty"));

                lsTmp = inetHttpControl.GetParameterValue("53"); // Kennz Reboot immer
                if (lsTmp.Equals("1"))
                    ibNeuStartAuto = true;

                lsTmp = inetHttpControl.GetParameterValue("48"); // Reboot
                if (lsTmp.Equals("1"))
                {
                    ibStatReboot = true;
                    lsTmp = inetHttpControl.GetParameterValue("49");
                    idtStatReboot = DateTime.Parse(lsTmp);
                    idtStatRebootPlus1Min = idtStatReboot.AddMinutes(1);
                }

                lsTmp = inetHttpControl.GetParameterValue("50"); // Shutdown
                if (lsTmp.Equals("1"))
                {
                    ibStatShutDown = true;
                    lsTmp = inetHttpControl.GetParameterValue("51");
                    idtStatShutDown = DateTime.Parse(lsTmp);
                    idtStatShutDownPlus1Min = idtStatShutDown.AddMinutes(1);
                }

                gnvMainInetplus.SetText(Program.getMyLangString("readKC"));

                lsTmp = inetHttpControl.GetParameterValue("39"); // Pwd Tasten
                if (!lsTmp.Equals(""))
                    instRegistry.Write(inetConstants.icsKombi,lsTmp);

                lsTmp = inetHttpControl.GetParameterValue("38"); // Pwd Client
                if (lsTmp.Equals(""))
                    lsTmp = inetConstants.icsStandardPassword;
                instRegistry.Write(inetConstants.icsAbbruch, lsTmp); // verschlüsselt

                lsTmp = inetHttpControl.GetParameterValue("37"); // Pwd Einst. PWD
                if (lsTmp.Equals(""))
                    lsTmp = inetConstants.icsStandardPassword;
               instRegistry.Write(inetConstants.icsEinPwd, lsTmp); // verschlüsselt

                lsTmp = inetHttpControl.GetParameterValue("36"); // Kuncenter Preise
                if (!lsTmp.Equals("") && !lsTmp.Equals("0"))
                {
                    gnvMainInetplus.SetText(Program.getMyLangString("readKCPreis"));
                    instRegistry.Write(inetConstants.icsImpPrs, lsTmp); //
                }

                gnvMainInetplus.SetText(Program.getMyLangString("readKCPrint"));
                lsTmp = inetHttpControl.GetParameterValue("40"); // pro Seite
                if (lsTmp.Equals(""))
                    lsTmp = "0";

                try
                {
                    idcDruckKosten = Double.Parse(lsTmp, Program.gnvDecFormat);
                }
                catch (Exception ex)
                { idcDruckKosten = 0; }

                idcStundenPreis = 1;
                try
                {
                    if (!instRegistry.ReadAsString(inetConstants.icsImpPrs).Equals(""))
                        idcStundenPreis = Double.Parse(instRegistry.ReadAsString(inetConstants.icsImpPrs), Program.gnvDecFormat);
                }
                catch (Exception ex)
                { idcStundenPreis = 1; }
              
                // Jugendschutz
                gnvMainInetplus.SetText(Program.getMyLangString("readProtectOfMinor"));
                lsTmp = inetHttpControl.GetParameterValue("47");  
                // Jugendschutz aktivieren/deaktivieren
                if (lsTmp.Equals("1"))
                {
                    SetJugendSchutz(true);
                }
                else
                {
                    SetJugendSchutz(false);
                }

                isLinkAdresse = inetHttpControl.GetParameterValue("64");

                ibOlingooOpen = true;
                lsTmp = inetHttpControl.GetParameterValue("66");
                if (lsTmp != null && lsTmp.Equals("0"))
                    ibOlingooOpen = false;

                try
                {
                    Console.WriteLine("kennz::1:" + kennzKPExpired);
                    kennzKPExpired = inetHttpControl.GetParameterValue("67");
                   //TESTNG kennzKPExpired ="1";
                    Console.WriteLine("kennz::2:" + kennzKPExpired + " ---" + ibStatusPing);
                    if (kennzKPExpired.Equals("1")) // 20100130
                        ibStatusPing = false;

                }
                catch { kennzKPExpired = "0"; }


                try
                {
                    // Games Link
                    isGamesLink = inetHttpControl.GetParameterValue("68");

                    ibAffili = Int32.Parse(inetHttpControl.GetParameterValue("70"));
                }
                catch
                { ibAffili = 1; }


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="abSwitch"></param>
        private void SetJugendSchutz(Boolean abSwitch)
        {
            String lsFileNameO, lsFileNameB;

            lsFileNameO = inetConstants.isWindowsDirectory + "\\inetw.xml";//Environment.GetEnvironmentVariable(inetConstants.SYSTEMROOT) + "\\" + "inetw.xml";
            lsFileNameB = inetConstants.isWindowsDirectory + "\\$inetw.xml";//Environment.GetEnvironmentVariable(inetConstants.SYSTEMROOT) + "\\" + "$inetw.xml";
            String tmp = instRegistry.SubKey;

            if (abSwitch)
            {
                try
                {
                    // Wörter
                    if (!File.Exists(lsFileNameO))
                    {
                        if (File.Exists(lsFileNameB))
                        {
                            File.Copy(lsFileNameB, lsFileNameO);
                            File.Delete(lsFileNameB);
                        }
                    }
                }
                catch { }
                // erlaubt
                
                instRegistry.SubKey = instRegistry.SubKey + "\\"+ inetConstants.icsVerzIE;
                instRegistry.WriteNoCrypt(inetConstants.icsErlaubt,"0");

                // Porno
                instRegistry.WriteNoCrypt(inetConstants.icsSex,"1");
                // andere
                instRegistry.WriteNoCrypt(inetConstants.icsAndere, "1");
                // Rassismus
                instRegistry.WriteNoCrypt(inetConstants.icsRas, "1");
                 // Flag immer setzen
                instRegistry.WriteNoCrypt(inetConstants.icsFlag, "1");

            
            } // Else
            else
            {
                // Wörter
                if (File.Exists(lsFileNameO))
                {
                    File.Copy(lsFileNameO,lsFileNameB);
                    File.Delete(lsFileNameO);
                }
                instRegistry.SubKey = instRegistry.SubKey + "\\" + inetConstants.icsVerzIE;

                 // erlaubt
                instRegistry.WriteNoCrypt(inetConstants.icsErlaubt, "0");

                // Porno
                instRegistry.WriteNoCrypt(inetConstants.icsSex, "0");
                // andere
                instRegistry.WriteNoCrypt(inetConstants.icsAndere, "0");
                // Rassismus
                instRegistry.WriteNoCrypt(inetConstants.icsRas, "0");
                 // Flag immer setzen
                instRegistry.WriteNoCrypt(inetConstants.icsFlag, "1");

            }


            instRegistry.SubKey = tmp;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asKioskID"></param>
        /// <returns></returns>
        private String checkKioskID(String asKioskID)
        {
            if (asKioskID.Equals("") || asKioskID.Equals("null"))
                return Program.getMyLangString("errorInvalidID");

            String lsAction = "";
            lsAction = "action=getregkey&2="+asKioskID;

            inetHttpControl.RunAction(lsAction, Program.gnvSplash.iActServerID, false);

            if (inetHttpControl.GetParameterValue("1").Equals(""))
                return Program.getMyLangString("errorInvalidID");

            fillInstance();

            String lsKennz = "", lsHdgPwd="", lsFile="";
            lsKennz = inetHttpControl.GetParameterValue("20");
            if (lsKennz.Equals(""))
            {
                lsKennz = "0";
                inetHttpControl.SetParameterValue("20", "0");
            }

            if (!lsKennz.Equals("0"))
            {
                if (lsHdgPwd.Equals("") || lsHdgPwd.Equals("null"))
                {
                    switch(lsKennz)
                    {
                        case "3":
                            lsKennz = "0";
                            break;
                        default:
                            lsKennz = "3";
                            break;
                    }
                }
                else
                {
                    switch (lsKennz)
                    {
                        case "1":
                            lsKennz = "2";
                            break;
                        case "2":
                            lsKennz = "3";
                            break;
                        case "3":
                            lsKennz = "4";
                            break;
                        case "4":
                            lsKennz = "0";
                            inetHttpControl.SetParameterValue("17",isClientVersion); // Version kennzeichen
                            break;
                    }
                }
                inetHttpControl.SetParameterValue("20", "0"); // Update Kennzeichen
                gnvMainInetplus.SetText(String.Format(Program.getMyLangString("onlineUpd"),lsKennz));

                // lsFile = GetHDGuard();

            }

            if (!isGesperrt.Equals("0")){
                gnvMainInetplus.SetText(Program.getMyLangString("blockedID"));
                return Program.getMyLangString("blockedID");
             }

            switch(lsKennz){
                case "2":
                    if (File.Exists(lsFile)){
                        lsFile += " OFF " + '"' +lsHdgPwd + '"';
                        System.Diagnostics.Process.Start(lsFile);
                    }
                    SetIgnore(true);
                    return "-1";
                    break;
                case "3": // update abgeschlossen HDGuard einschalten
                    //SetLockFiles(false);

                    break;

                case "4":

                    break;
            }

         //   Choose Case ls_kennz

//    Case "3" // Update abgeschlossen HDGuard einschalten
//            of_SetLockFiles(False)
//            ib_update_in_progress = True //
//            Run(ls_winsysdir+"\upd.exe " + is_client_version)
//            this.of_ignore(true)
//            If IsValid(w_inetplus) Then
//                w_inetplus.of_ende()
//            End If
//            Return "-1"			
//    Case "4"
//        If FileExists(ls_file) Then
//            Run(ls_file + ' ON "' + ls_hdg_pwd + '"')
//        End If
//        Return "-1"		
//End Choose


            return "";
        }

        /// <summary>
        /// überprüfe, ob Eingabe nur aus Zahlen besteht
        /// </summary>
        /// <param name="asString"></param>
        /// <returns></returns>
        public Boolean isNumber(String asString)
        {
            foreach (char c in asString)
            {
                if (!char.IsNumber(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abSwitch"></param>
        private void SetIgnore(Boolean abSwitch)
        {
            if (abSwitch)
                instRegistry.Write(inetConstants.icsIgnore, "Y");
            else
                instRegistry.DeleteKey(inetConstants.icsIgnore);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abSwitch"></param>
        public void BlockMaus(Boolean abSwitch)
        {
            //Win32User.BlockInput(abSwitch);
            kpTaskBar.kpTBar.Block = abSwitch;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PruefTaskBar()
        {
            int fWindow, liHide;


            fWindow = Win32User.FindWindow("Shell_TrayWnd", "");
           
            if (fWindow >0)
                il_hwnd1 = fWindow;

            fWindow = Win32User.FindWindow("Progman", null);
            if (fWindow > 0)
                il_hwnd2 = fWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abVisible"></param>
        public void SetShowTrayWnd(Boolean abVisible)
        {
            /****
            int fWindow, liHide;

            liHide = Win32User.SW_HIDE;
            if (abVisible)
                liHide = Win32User.SW_SHOW;

            fWindow = Win32User.FindWindow("Shell_TrayWnd", "");
            Win32User.ShowWindow((System.IntPtr)fWindow, liHide);

            il_hwnd1 = fWindow;

            fWindow = Win32User.FindWindow("Progman", null);
            Win32User.ShowWindow((System.IntPtr)fWindow, liHide);
            il_hwnd2 = fWindow;
            ***/
            if (il_hwnd1 == 0)
                PruefTaskBar();

            kpTaskBar.kpTBar.Visible = abVisible;
        }


        /// <summary>
        /// 
        /// </summary>
        public void AppClose()
        {
            try
            {
                SetOnlineStatus("1");

                lbInternet = true;
                if (invUsbCtrl != null)
                    invUsbCtrl.StopPolling();

                if (invMPControl != null)
                    invMPControl.stopPolling();

                invUsbCtrl = null;
                invMPControl = null;

                DialogShutz.SetCloseWindow();
                gnvMainInetplus.SetClose();

                SetShowTrayWnd(true);
            }
            catch 
            { }
          
            //HideCAD(false);
            BlockMaus(false);
            SetShowTrayWnd(true);

            BlockTaste(false);

            try {
                if (tBack != null)
                {
                    tBack.Abort();
                    tBack = null;

                }
            }
            catch { }


            try
            {
                Application.ExitThread();
            }
            catch
            { }

        }



        public void BlockTaste(bool abSwitch)
        {
            try
            {
                // 20100130
                if (gnvMainInetplus == null)
                    return;


                if (abSwitch)
                {
                    gnvMainInetplus.SetHook(true);
                }
                else
                {
                    gnvMainInetplus.SetHook(false);

                }
            }
            catch { }
           
        }

        ////void inetHookControl_KeyUp(object sender, WindowsHookLib.KeyEventArgs e)
        ////{
        ////    if (e.KeyCode == Keys.LWin)
        ////        e.Handled = true;
        ////    if (e.KeyCode == Keys.RWin)
        ////        e.Handled = true;
        ////    if (e.KeyCode == Keys.Escape)
        ////        e.Handled = true;
        ////}

        ////void inetHookControl_KeyDown(object sender, WindowsHookLib.KeyEventArgs e)
        ////{
        ////    if (e.KeyCode == Keys.LWin)
        ////        e.Handled = true;
        ////    if (e.KeyCode == Keys.RWin)
        ////        e.Handled = true;
        ////    if (e.KeyCode == Keys.Escape)
        ////        e.Handled = true;
        ////}

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adCredit"></param>
        public void SetWert(double adCredit, string id)
        {

            if (!ibFreigabe)
            {
                SetFreigabe(adCredit, "", false, id);
            }
            else
            {
                String lsOnline = String.Empty;
                SetUmsatz(adCredit, ref lsOnline, "1", 0, id);// SetZeit(adCredit);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetZeitZone(Boolean argServerRead)
        {
            String lsAction = "";
            TimeZone lTimeZone;
            lTimeZone = TimeZone.CurrentTimeZone;
           
            TimeSpan lp;
            lp = lTimeZone.GetUtcOffset(System.DateTime.Now);
           
            int lOffset = lp.Hours + lp.Minutes;
            lOffset *= 100;

            lsAction = "action=dt&1=" + lOffset.ToString();

            inetHttpControl.RunAction(lsAction, Program.gnvSplash.iActServerID, false);
            if (!Program.gnvSplash.ibStatusPing)
            {
                try
                {
                    String liServerName = String.Empty;
                    Console.WriteLine("Saati düzelt");
                    inetRegistry liRegistry = new inetRegistry();
                    liRegistry.SubKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion";
                    liServerName = liRegistry.ReadAsStringNoEncrypt("NistServer");
                    Console.WriteLine("Saati düzelt:" + liServerName);

                    // 20100130
                    if (!isNistServer.Equals(""))
                    {
                        liServerName = isNistServer;
                    }

                    IPAddress hostIpAddress = null;
                    NistServer.NistServer nistClock = null;

                    if (liServerName == null || liServerName.Equals(String.Empty))
                    {
                        nistClock = new NistServer.NistServer();
                    }
                    else
                    {
                        hostIpAddress = IPAddress.Parse(liServerName);
                        nistClock = new NistServer.NistServer(hostIpAddress);
                    }
                       
                    nistClock.SynchronizeLocalClock();
                    liRegistry = null;
                    //nistClock.PrintDiagnosticMessages = true;
                }
                catch { }
                return;
            }


            DateTime ldDate = DateTime.Now;

            String lsTmp;
            String[] lsT = { "" }, lsT1 ={ "" };
            char[] c = {';'}; //

            if (argServerRead)
            {

                lsTmp = inetHttpControl.GetParameterValue("4");
                if (!lsTmp.Equals("") && !lsTmp.StartsWith("-1:"))
                {
                    lsT = lsTmp.Split(c);
                    mServerList.Clear();
                    for (int i = 0; i < lsT.Length; i++)
                    {
                        if (!lsT[i].Equals(""))
                            mServerList.Add(lsT[i]);
                    }

                    // 20100226
                    /// ///loadServers();


                }
            }

            c[0] = '.';
            lsTmp = inetHttpControl.GetParameterValue("2"); // Date
            if (!lsTmp.Equals(""))
            {
                lsT = lsTmp.Split(c);
            }

            c[0] = ':';
            lsTmp = inetHttpControl.GetParameterValue("3");
            if (!lsTmp.Equals(""))
            {         
                lsT1 = lsTmp.Split(c);
            }

            if (lsT.Length == 3 && lsT1.Length == 3)
                ldDate = new DateTime(Int32.Parse(lsT[2]),Int32.Parse(lsT[1]),Int32.Parse(lsT[0]),Int32.Parse(lsT1[0]),Int32.Parse(lsT1[1]),Int32.Parse(lsT1[2]));
            else if (lsT.Length == 3 && lsT1.Length != 3)
                ldDate = new DateTime(Int32.Parse(lsT[2]), Int32.Parse(lsT[1]), Int32.Parse(lsT[0]),ldDate.Hour,ldDate.Minute,ldDate.Second);
            else if (lsT.Length != 3 && lsT1.Length == 3)
                ldDate = new DateTime(ldDate.Year, ldDate.Month, ldDate.Day, Int32.Parse(lsT1[0]), Int32.Parse(lsT1[1]), Int32.Parse(lsT1[2]));

            
           /// System.Windows.Forms.MessageBox.Show(argServerRead.ToString() + "/"+ ldDate.ToString(inetConstants.isDateFormat) + " / " + ldDate.ToString(inetConstants.isTimeFormat));
            SetLocalDateTime(ldDate);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argDateTime"></param>
        public void SetLocalDateTime(DateTime argDateTime)
        {

            inetAPI.SetLocalDateTime(argDateTime);
            //IntPtr  tokHandle = IntPtr.Zero;
            //int ret;
            //TOKEN_PRIVILEGES    tkp;
            //tkp = new TOKEN_PRIVILEGES();
            //tkp.Privileges = new LUID_AND_ATTRIBUTES[1];

            //LUID    ll;

            //int i= AdvApi.OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle, Win32Kernel.TOKEN_ALL_ACCESS, ref tokHandle);
            //ret = AdvApi.LookupPrivilegeValue(null, Win32Kernel.SE_SYSTEMTIME_NAME, ref tkp.Privileges[0].pLuid);
            //tkp.Privileges[0].Attributes = Win32Kernel.SE_PRIVILEGE_ENABLED;

            //ret = AdvApi.AdjustTokenPrivileges(tokHandle, false, ref tkp, tkp.PrivilegeCount, (IntPtr)0, 0);

            //SYSTEMTIME lpTime;
            //lpTime.wYear = (short)argDateTime.Year;
            //lpTime.wMonth = (short)argDateTime.Month;
            //lpTime.wDayOfWeek = (short)argDateTime.DayOfWeek;
            //lpTime.wDay = (short)argDateTime.Day;
            //lpTime.wHour = (short)argDateTime.Hour;
            //lpTime.wMinute = (short)(argDateTime.Minute);
            //lpTime.wSecond = (short)argDateTime.Second;
            //lpTime.wMilliseconds = (short)argDateTime.Millisecond;

            //Win32Kernel.SetLocalTime(ref lpTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String GetMacAdress()
        {
            String lsMac = String.Empty;

            ManagementClass manClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = manClass.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (lsMac == String.Empty)  // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) lsMac = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            lsMac = lsMac.Replace(":", "");

            return lsMac;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="astrDriveLetter"></param>
        /// <returns></returns>
        private String GetSerialNumber(String astrDriveLetter)
        {
            if (astrDriveLetter == "" || astrDriveLetter == null) astrDriveLetter = "C";
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + astrDriveLetter + ":\"");
            
            disk.Get();
           return  disk["VolumeSerialNumber"].ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="asOnlineStatus"></param>
        public void SetOnlineStatus(String asOnlineStatus)
        {
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("action=getregkey/set");
            strQuery.Append("&2=").Append(isKioskID); // KdID
            strQuery.Append("&1=").Append(iTerminalID.ToString()); // term ID
            strQuery.Append("&4=").Append(asOnlineStatus); // KennzOnline

            inetHttpControl.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="asName"></param>
        /// <param name="asPwd"></param>
        public void SetUserAccount(String asName, String asPwd)
        {
            String lsInfoText = String.Empty;

            try
            {

                StringBuilder strQuery = new StringBuilder();
                
                double ldBetrag = -1;
                int liGuthaben = 0;
                Boolean lbCoupon = false;


                gnvMainInetplus.SetText(Program.getMyLangString("checkData"));

                strQuery.Append("action=user")
                        .Append("&2=").Append(asName)
                        .Append("&3=").Append(asPwd)
                        .Append("&4=").Append(iStandortID)
                        .Append("&5=")
                        .Append("&6=").Append(iAufstellplatzID)
                        .Append("&8=").Append(iTerminalID.ToString());


                inetHttpControl.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);
                Console.WriteLine("11:::>" + inetHttpControl.GetParameterValue("24"));

                if (!inetHttpControl.GetParameterValue("24").Equals(""))
                {
                    gnvMainInetplus.SetText(inetHttpControl.GetParameterValue("24"));
                    return;
                }

                if (inetHttpControl.GetParameterValue("14").Equals(""))
                    liGuthaben = 0;
                else
                    liGuthaben = Int32.Parse(inetHttpControl.GetParameterValue("14"));

                if (liGuthaben <= 0)
                {
                    gnvMainInetplus.SetText(inetHttpControl.GetParameterValue("24"));
                    return;
                }
                gnvMainInetplus.SetText(Program.getMyLangString("freigabeDurch"));

                iKundenID = Int32.Parse(inetHttpControl.GetParameterValue("1"));
                isKundenID = inetHttpControl.GetParameterValue("2");
                isAnzNameFull = inetHttpControl.GetParameterValue("5") + " " + inetHttpControl.GetParameterValue("6");
                lsInfoText = inetHttpControl.GetParameterValue("21");

                ibAufladen = false;
                if (inetHttpControl.GetParameterValue("26").Equals("1"))
                    ibAufladen = true;

                if (!inetHttpControl.GetParameterValue("27").Equals(""))
                    ldBetrag = Double.Parse(inetHttpControl.GetParameterValue("27"),Program.gnvDecFormat);

                if (ldBetrag != -1)
                    lbCoupon = true;
                else
                    ldBetrag = 0;

                ibKennzBonplus = false;
                if (inetHttpControl.GetParameterValue("28").Equals("1"))
                    ibKennzBonplus = true;


                if (invDateTime == null)
                    invDateTime = new inetDateTime();

                // Freigabe durchführen...
                invDateTime.SetEndeDateTime(liGuthaben);
                SetFreigabe(ldBetrag, invDateTime.GetOnlineZeitAsString(), lbCoupon,"");
            }
            catch { }

            try
            {
                if (!lsInfoText.Equals(""))
                    Program.gnvSplash.gnvMainInetplus.SetAgentText(lsInfoText);
            }
            catch { }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adCredit"></param>
        /// <param name="asOnlineZeit"></param>
        /// <param name="abCoupon"></param>
        public void SetFreigabe(double adCredit,String asOnlineZeit,Boolean abCoupon, string id)
        {
            gnvMainInetplus.SetText(Program.getMyLangString("freigabePruefs"));
            int llCouponID;

            ibFreigabe = true;
            if (adCredit != -1) // PC wurde neu gestartet
            {
                gnvMainInetplus.SetText(Program.getMyLangString("checkFreigabe"));
                if (abCoupon)
                {
                    llCouponID = iKundenID;
                    iKundenID = 0;
                    // Umsatz schreiben "b"
                    this.SetUmsatz(adCredit, ref asOnlineZeit, "b", llCouponID, "");
                }
                else
                {
                    // umsatz schreiben "0"
                    this.SetUmsatz(adCredit, ref asOnlineZeit, "0", 0, id);
                }
            }

            // Freigabe soll erfolgen
            gnvMainInetplus.SetText(Program.getMyLangString("freigabeTerminal"));
            DialogNewUser = new dlgNewUser();
            DialogNewUser.SetFreigabe(isKundenID, asOnlineZeit, invDateTime);
            gnvMainInetplus.OpenNewUser();
        }


        public void GetAllProcess()
        {
            allActiveProcess.GetAllAktuelleProcess();
        }

        public void KillAllOtherProcess()
        {
            try
            {
                allActiveProcess.KillOtherProcess();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        public int SetUmsatz(double adCredit, ref String asOnline, String asKennzVerlaengert, int aiNrSMS, string id)
        {
            Boolean lbDel = false;
            int liHH = 0, liMM = 0;
            StringBuilder strQuery = new StringBuilder();
            String lsOnlineIni = String.Empty ;
            String lsRestZeit = String.Empty;

            strQuery.Append("action=us");
            DateTime ldStart = DateTime.Now;

             if (id.Equals(string.Empty))
                id = isWaehrung;
            else
            {
                if (id.ToLower().Equals("ty") || id.ToLower().Equals("tr"))
                    id = "TRY";
                else if (id.ToLower().Equals("eu"))
                    id = "EUR";
                else if (id.ToLower().Equals("ch"))
                    id = "CHF";
                else if (id.ToLower().Equals("dk"))
                    id = "DKK";
                else if (id.ToLower().Equals("cz"))
                    id = "CZK";
                else if (id.ToLower().Equals("lt"))
                    id = "LTL";
                else if (id.ToLower().Equals("no"))
                    id = "NOK";
                else if (id.ToLower().Equals("gb"))
                    id = "GBP";
                else if (id.ToLower().Equals("us"))
                    id = "USD";
                
            }


            // Bei Coupons ist der Betrag nur zur weitergabe gedacht, die Zeit ist ebenfalls bestimmt worden beim Coupon
            // Daher keine Umrechnung "Zeit ermitteln anstatt Geld"
            if (adCredit > 0 && !asKennzVerlaengert.Equals("b") && !asKennzVerlaengert.Equals("c"))
            {
                 if (!id.ToLower().Equals(isWaehrung.ToLower())) {
                    double d = 1;
                    if (myCurrencies.ContainsKey(id)) {
                        try
                        {
                            d = Double.Parse(myCurrencies[id].ToString(),Program.gnvDecFormat);
                        }catch{ 
                            d = 1; 
                        }
                        adCredit = adCredit * d;
                    }
                    else
                    {
                        ipExchangeService.ipExcService cc = new ipExchangeService.ipExcService();
                        cc.AdjustToLocalTime = false;

                        IList<ipExchangeService.CurrencyData> list = new List<ipExchangeService.CurrencyData>(1);
                        ipExchangeService.CurrencyData cd = new ipExchangeService.CurrencyData(id, isWaehrung);
                        list.Add(cd);

                        try
                        {
                            cc.GetCurrencyData(ref list);
                        }
                        catch
                        { 
                            d = 1; 
                            list = null; 
                        }

                        if (list != null && list.Count > 0)
                            d = list[0].Rate;

                        myCurrencies.Add(id, d);
                        adCredit = adCredit * d;
                    }
                }


                if (invDateTime == null)
                    invDateTime = new inetDateTime();

                invDateTime.GetZeitFromCredit(adCredit, idcStundenPreis, ref liHH, ref liMM);
                if (liHH <= 0 && liMM <= 0)
                    return 0;

                asOnline = invDateTime.GetZeitFromMinuten((liHH * 60 + liMM));
            }

            switch (asKennzVerlaengert)
            {
                case "0": // Freigabe
                case "b": // Freigabe
                    {
                        if (invDateTime == null)
                            invDateTime = new inetDateTime();

                        lsOnlineIni = asOnline;
                        liMM = invDateTime.GetMinutenFromOnlineZeit(asOnline);
                        
                        strQuery.Append("&13=").Append(isKundenID);
                        strQuery.Append("&14=").Append(isAnzNameFull);
                        strQuery.Append("&15=").Append(ldStart.ToString("yyyy-MM-dd"));
                        strQuery.Append("&16=").Append(ldStart.ToString("HH:mm:ss"));
                        if (this.ibAufladen)
                            strQuery.Append("&20=1");
                        else
                            strQuery.Append("&20=0");

                        strQuery.Append("&21=").Append(Program.gnvSplash.isLanguage);
                        break;
                    }
                case "1": // verlängerung
                    {
                        // Info Meldung anzeigen
                        if (liHH > 0 || liMM > 0)
                        {
                            liHH = invDateTime.GetMinutenFromOnlineZeit(invDateTime.GetGesamtOnlineZeit());//liHH = liHH * 60 + liMM ;
                            lsOnlineIni = invDateTime.GetZeitFromMinuten(liHH);
                        }

                        break;
                    }
                case "2": // automatisch oder manuell gesperrt
                case "3":
                    {
                        lsRestZeit = invDateTime.GetOnlineZeitAsString();
                        invDateTime = null;
                        ibFreigabe = false;
                        lbDel = true;
                        break;
                    }
                case "99": // Aufladen keine Umsatz schreiben
                case "c":
                    {
                        int tmp = 0, tmp2=0;
                        String lsTmp=String.Empty;
                        inetDateTime local = new inetDateTime();
                        if (DialogNewUser != null)
                        {
                            tmp = DialogNewUser.GetOnlineZeitAsInt();
                            tmp2 = tmp + local.GetMinutenFromOnlineZeit(asOnline);

                            lsOnlineIni = local.GetZeit(tmp2);
                        }

                        if (DialogNewUser != null)
                        {
                            int liGuthaben = local.GetMinutenFromOnlineZeit(asOnline);
                            DialogNewUser.SetNewOnlineTime(-liGuthaben);
                        }

                        if (asKennzVerlaengert.Equals("99"))// zum Aufladen ikunden als Guthaben! Addieren
                            asOnline = local.GetZeit(tmp);

                        strQuery.Append("&18=").Append(isKundenID);
                        strQuery.Append("&19=").Append(isAnzNameFull);

                        break;
                    }
                case "9": // Druckkosten
                    {
                        break;
                    }
                case "m": // Spiele
                case "p":
                    {
                       // aiNrSMS > 0 zubuchen
                       // <0 abbuchen!!!
                       invDateTime.SetEndeDateTime(aiNrSMS);
                       aiNrSMS = 0;

                        break;
                    }
            }

            if (asOnline.Equals("") || asOnline.Equals("00:00"))
                asOnline = "00:01";


            strQuery.Append("&1=").Append(iTerminalID)
                    .Append("&8=").Append(lsRestZeit)
                    .Append("&5=").Append(asOnline)
                    .Append("&7=").Append(iKundenID)
                    .Append("&6=").Append(asKennzVerlaengert)
                    .Append("&2=").Append(ldStart.ToString("yyyy-MM-dd"))
                    .Append("&3=").Append(ldStart.ToString("HH:mm:ss"));

            if (ibKennzBonplus)
                strQuery.Append("&22=1");
            else
                strQuery.Append("&22=0");

            if (lbDel == false)
            {
                strQuery.Append("&4=").Append(adCredit);
                strQuery.Append("&9=").Append(aiNrSMS);
                strQuery.Append("&10=").Append(invDateTime.GetEndeDate());
                strQuery.Append("&11=").Append(invDateTime.GetEndeTime());
                strQuery.Append("&12=").Append(lsOnlineIni);
            }
            else
            {
                strQuery.Append("&4=0&9=0");
            }

            // Gesperrt automatisch oder manuell
            if (asKennzVerlaengert.Equals("2") || asKennzVerlaengert.Equals("3"))
            {
                iKundenID = 0;
                isKundenID = "Surfer";
                isAnzNameFull = "Surfer";
            }

            // ID- Waehrung
            strQuery.Append("&23=").Append(id);

            inetHttpControl.RunAction(strQuery.ToString(), Program.gnvSplash.iActServerID, false);

            asOnline = lsOnlineIni;

            return 1;
        }

        public void SperrenComputer(String asKennzeichen)
        {
            // BlockInput
            String lsNutzDauer;

            if (asKennzeichen.Equals("1")) // manuell blocking
            {
                lsNutzDauer = invDateTime.GetNutzDauerAsString();
                SetUmsatz(0, ref lsNutzDauer, "2", 0,"");
            }
            else if (asKennzeichen.Equals("2")) // automatic blocking
            {
                lsNutzDauer = invDateTime.GetGesamtOnlineZeit();
                SetUmsatz(0, ref lsNutzDauer, "3", 0,"");
            }

            ibFreigabe = false;


            Program.switchLanguage(Program.defaultLanguage);

            gnvMainInetplus.OpenSchutzSplash();

            //SetOnlineStatus("2");

           
        }
    }
}

