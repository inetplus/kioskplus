using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using kioskplus.Utils;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace kioskplus
{
    public class httpcontrol
    {
        public Boolean      ibRunAction = false;
        inetStringParser    mParameter;
        inetFileCtrl        fileControl = new inetFileCtrl();
        String              isHistory = "1";
        int                 mIndexNr = 0;
        Boolean             ibGetRegKey = false;
        Boolean             ibOfflineAlreadyExecuted = false;
        String              isPostParameter = String.Empty;

        public httpcontrol()
        {}

        public bool RunAction(String asParameter, int indexSrv, bool abSaveParam)
        {
            ibRunAction = false;
            String lsE = DateTime.Now.ToString(inetConstants.isTimeFormat);

            // 20100227
            mIndexNr = indexSrv;
            
            String lsTmp = "";

            // UMS!?
            if (!abSaveParam)
            {
                if (Program.gnvSplash.ibKennzUMS)
                    asParameter += "&UMS=1";
                else
                    asParameter += "&UMS=0";

                SetNoNetPostFunction(asParameter);

                //mParameter = null; // jedesmal neu initialisieren
                lsTmp = GetNoNetAllParameter();
                if (!lsTmp.Equals(""))
                    if (lsTmp.StartsWith("&"))
                        asParameter = asParameter + lsTmp;
                    else
                        asParameter = asParameter + "&" + lsTmp;

                asParameter = asParameter + "&his=" + isHistory + "&lang=" + Program.defaultLanguage;

                if (asParameter.StartsWith("action=getregkey&"))
                    ibGetRegKey = true;
            }

            if (asParameter.IndexOf("&exec=")>0)
                asParameter = asParameter.Substring(0,asParameter.IndexOf("&exec="));
            asParameter = asParameter +  "&exec=" + lsE;

            try
            {
             
                Program.gnvSplash.ibStatusPing = true;
                
                System.Collections.Generic.Dictionary<string, string> param = new System.Collections.Generic.Dictionary<string,string>();
 
                String[] first = asParameter.Split('&');
                first = first[0].Split('=');

                param.Add("sec",lsE);
                param.Add("text", asParameter);
                isPostParameter = asParameter;
                if (Program.gnvSplash.ipHttpStatusCode[indexSrv] == HttpStatusCode.OK)
                {
                    try
                    {
                        String ls = "http://" + Program.gnvSplash.mServerList[indexSrv] + ":" + Program.gnvSplash.iPort.ToString() + Program.gnvSplash.isKontent;
                       //String ls = "http://localhost:8080/kpservice/";// +first[1];
                        ls = ls + first[1] + "?sec=" + inetBase64.encodeAsString(lsE) + "&text=" + inetBase64.encodeAsString(asParameter);

                        Console.WriteLine("myls:::" + ls);


                        String results = kpRest.getURI(ls, indexSrv);
                        if (Program.gnvSplash.ipHttpStatusCode[indexSrv] == HttpStatusCode.OK)
                        {
                            doExecuteResult(results);
                        }
                        else
                            executeSecond(asParameter);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("eer;;;;" + ex.Message);
                    }
                }
                else
                {
                    if (Program.gnvSplash.countRequest > 10) {
                        Program.gnvSplash.countRequest = 0;
                        Program.gnvSplash.ipHttpStatusCode[indexSrv] = HttpStatusCode.OK;
                    }
                    else
                    {
                        return executeSecond(asParameter);
//                        return false;
                    }
                }
                Console.WriteLine("count..." + Program.gnvSplash.countRequest);

                 ibOfflineAlreadyExecuted = false;
                 isPostParameter = String.Empty;
                 return true;
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error:" + ex.Message);
            }

            return false;
        }

        private bool executeSecond(String asParameter)
        {

            Program.gnvSplash.countRequest++;
            if (mIndexNr < Program.gnvSplash.mServerList.Count-1)
            {
                // from a new thread!!!
                if (RunAction(asParameter, (mIndexNr + 1), true))
                {
                    isPostParameter = String.Empty;
                    return true;
                }
            }
            // already executed?
            if (!ibOfflineAlreadyExecuted)
                doExecuteOffline("");

            ibOfflineAlreadyExecuted = true;
            return false;
        }

        // 20100226
        ////void lservice_doCallCompleted(object sender, doCallCompletedEventArgs e)
        ////{
        ////    // exist any Error
        ////    if (e != null && e.Error != null && e.Error.Message != null && !e.Error.Message.Equals(""))
        ////    {
        ////        Program.gnvSplash.ibStatusPing = false;
        ////        if (!ibOfflineAlreadyExecuted)
        ////            doExecuteOffline(e.Error.Message);

        ////        ibOfflineAlreadyExecuted = true;

        ////        // all server is unreachable
        ////        if (mIndexNr >= Program.gnvSplash.mServerList.Count)
        ////        {
        ////            RunAction(isPostParameter, (mIndexNr + 1), true);
        ////        }
        ////    }
        ////    else if (e != null && e.Result != null && e.Result.@return != null) // Server is reachable
        ////    {
        ////        Program.gnvSplash.ibStatusPing = true;
        ////        doExecuteResult(e.Result.@return);
        ////    }

        ////}

        void doExecuteResult(String returnValue)
        {
            String lsErgebnis = String.Empty;

            lsErgebnis = inetBase64.decodeString(returnValue); // returnValue; //
            mParameter = null;

            Console.WriteLine("RC:" +lsErgebnis);

            if (lsErgebnis.StartsWith("0=0"))
                lsErgebnis = lsErgebnis.Substring(3);

            if (ibGetRegKey && !lsErgebnis.Equals("") && !lsErgebnis.StartsWith("-1:"))
            {
                String lsTmp;
                lsTmp = "action=getregkey/set&" + lsErgebnis;
                lsTmp = lsTmp.Replace("\t", "&");
                SetNoNetPostFunction(lsTmp);
            }

            if (!lsErgebnis.Equals("") && !lsErgebnis.StartsWith("-1:"))
            {
                // result ok ping is not checked... delete the files always
                fileControl.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameUms);
                fileControl.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameUmsKonto);

               // inetStringParser localParameter = new inetStringParser();
                mParameter = new inetStringParser();
                mParameter.SetParamter(lsErgebnis);

                //isPostParameter
                if (isPostParameter.StartsWith("action=getregkey") && !isPostParameter.StartsWith("action=getregkeyini")) // &&ibStatusPing // means it works
                {
                    WriteRegKeyFile(lsErgebnis);
                }
                return;
            }
            else if (returnValue.Equals(""))
            {
                if (isPostParameter.StartsWith("action=us"))
                {
                    if (isPostParameter.IndexOf("6=2") > 0 || isPostParameter.IndexOf("6=3") > 0)
                    {
                        fileControl.deleteFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameIni);
                    }
                }
                return;
            }


        }


        void doExecuteOffline(String asExceptionString)
        {
            // Server is not reachable
           // if (asExceptionString.IndexOf("WSE101") > 0)
            //{
               String lsTmp = String.Empty;
                if (isPostParameter.StartsWith("action=getregkeyini"))
                    lsTmp = GetNoNetAsParam(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameIni, "IO");
                else if (isPostParameter.StartsWith("action=getregkey"))
                    lsTmp = GetNoNetAsParam(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameRK, "RK");
                else if (isPostParameter.StartsWith("action=ipaffili/get"))
                    lsTmp = GetNoNetAsParam(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameAffili, "IA");
          
           
        }
        
        private String GetNewMD5String(String arg1)
        {
            StringBuilder sBuilder = new StringBuilder();

            MD5 md5 = MD5.Create("MD5");
            byte[] input = Encoding.UTF8.GetBytes(arg1);
            byte[] hash = md5.ComputeHash(input);

            for (int i = 0; i < hash.Length; i++)
                    sBuilder.Append(hash[i].ToString("x2"));

            Console.WriteLine("1>>" + arg1 + "<<>>" + sBuilder.ToString());
            return sBuilder.ToString();
           
        }

        private byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                
               // lock (locker)
                //{
                    formatter.Serialize(fs, objectToSerialize);
                //}
                return fs.ToArray();
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occured during serialization. Message: " + se.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        private void WriteRegKeyFile(String argTmp)
        {
            inetStringParser localParser = new inetStringParser();
            localParser.SetParamter(argTmp);

            if (localParser.GetValue("1").Equals(""))
                return;

            RecordKey rKey = new RecordKey();
            try
            {
                rKey.termID = Int32.Parse(localParser.GetValue("1"));
            }
            catch
            { }

            rKey.kdID = localParser.GetValue("2");
            try
            {
                rKey.standordID = Int32.Parse(localParser.GetValue("3"));
            }
            catch
            { }

            try
            {
                rKey.kennzGesperrt = localParser.GetValue("5");
                rKey.webSite = localParser.GetValue("7");
                rKey.pcGUID = localParser.GetValue("8");
                rKey.pcName = localParser.GetValue("15");
                rKey.faName = localParser.GetValue("22");
                rKey.faStr1 = localParser.GetValue("23");
                rKey.faStr2 = localParser.GetValue("24");
                rKey.kennzMP = localParser.GetValue("26");
                rKey.kennzIKunKonto = localParser.GetValue("27");
                rKey.kennzUmsatz = localParser.GetValue("28");
            }
            catch { }

            try
            {
               if (!localParser.GetValue("36").Equals(String.Empty)) // 
				  rKey.preisStunde = Double.Parse(localParser.GetValue("36"), Program.gnvDecFormat);
            }
            catch
            { }
            rKey.pwdEinPwd = localParser.GetValue("37");
            rKey.pwdClientPwd = localParser.GetValue("38");
            rKey.pwdTasten = localParser.GetValue("39");
            try
            {
				if (!localParser.GetValue("40").Equals(String.Empty)) // 
                	rKey.preisDrucken = Double.Parse(localParser.GetValue("40"), Program.gnvDecFormat);
            }
            catch
            { rKey.preisDrucken = 0; }
            try
            {
                rKey.aufstellplatzID = Int32.Parse(localParser.GetValue("45"));
            }
            catch
            { }
            try
            {
                rKey.freeSmsPkd = Int32.Parse(localParser.GetValue("46"));
            }
            catch
            { }

            rKey.kennzJugend = localParser.GetValue("47");
            rKey.kennzreboot = localParser.GetValue("48");
            try
            {
                rKey.rebootTime = DateTime.Parse(localParser.GetValue("49"));
            }
            catch
            { }
            rKey.kennzShutdown = localParser.GetValue("50");
            try
            {
                rKey.shutdownTime = DateTime.Parse(localParser.GetValue("51"));
            }
            catch
            { }

            rKey.werbeText = localParser.GetValue("52");
            rKey.kennzrebootAlways = localParser.GetValue("53");
            rKey.verzNoNet = localParser.GetValue("54");

            try
            {
                rKey.idSkin = Int32.Parse(localParser.GetValue("55"));
            }
            catch
            { }

            rKey.kennzShutdownBtn = localParser.GetValue("56");
            rKey.kennzVOIP = localParser.GetValue("57");
            rKey.kennzSMS = localParser.GetValue("58");
            rKey.kennzGames = localParser.GetValue("59");
            rKey.aufStWaehrung = localParser.GetValue("61");
            rKey.aufStWaehrungCent = localParser.GetValue("62");

            try
            {
                rKey.aufStLand = Int32.Parse(localParser.GetValue("63"));

                // 20100130
                rKey.i_link = localParser.GetValue("64");
                rKey.kennzKPExpired = localParser.GetValue("67");
            }
            catch
            { }

            try
            {
                rKey.ibAffili = Int32.Parse(localParser.GetValue("70"));
            }
            catch
            { }

            fileControl.writeKeyFile(Program.gnvSplash.isVerzNoNet + "\\" + inetConstants.isFileNameRK, rKey);
        }

        public void SetNullHashMap()
        {
            mParameter = null;
        }

        private String GetNoNetAllParameter()
        {
            String lsReturn;
            String lsTmp="";
            String lsFileNameTmp;
            String lsFileName=Program.gnvSplash.isVerzNoNet;
           

            lsFileNameTmp = lsFileName + "\\"+inetConstants.isFileNameIni;
            lsReturn = GetNoNetAsParam(lsFileNameTmp, "I");

            lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameUms;
            lsTmp = GetNoNetAsParam(lsFileNameTmp, "U");
            isHistory = "1";
            if (!lsTmp.Equals(""))
            {
                isHistory = "0";
                if (!lsReturn.Equals(""))
                    lsReturn += "&" + lsTmp;
                else
                    lsReturn = lsTmp;
            }

            lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameUmsKonto;
            lsTmp = GetNoNetAsParam(lsFileNameTmp, "K");
            if (!lsTmp.Equals(""))
            {
                isHistory = "0";
                if (!lsReturn.Equals(""))
                    lsReturn += "&" + lsTmp;
                else
                    lsReturn = lsTmp;
            }
            return lsReturn;
        }


        private void SetNoNetPostFunction(String asParameter)
        {
            char[] c ={'&' };

            try
            {
                String lsFileName = Program.gnvSplash.isVerzNoNet;
                String lsFileNameTmp;
                String ls;
                inetStringParser localParser = new inetStringParser();
                localParser.SetParamter(asParameter, c);

                if (localParser.GetValue("action").Equals("setums") && 
                    !localParser.GetValue("6").Equals("2") && 
                    !localParser.GetValue("6").Equals("3"))
                {
                    lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameIni;

                    RecordIni record;
                    bool bTmp = false;
                    record = fileControl.readIniFile(lsFileNameTmp);
                    if (record == null)
                    {
                        bTmp = true;
                        record = new RecordIni();
                    }
                    if (record.termID == 0)
                        if (!Int32.TryParse(localParser.GetValue("1"), out record.termID))
                            record.termID = 0;
                    if (!localParser.GetValue("2").Equals("") && bTmp)
                    {
                        ls = localParser.GetValue("2");
                        ls += " " + localParser.GetValue("3");
                        record.start = DateTime.Parse(ls);
                    }
                    if (!localParser.GetValue("10").Equals(""))
                    {
                        ls = localParser.GetValue("10");
                        ls += " " + localParser.GetValue("11");
                        record.ende = DateTime.Parse(ls);
                    }

                    record.onlineZeit = localParser.GetValue("12");

                    record.userName = Program.gnvSplash.isKundenID;
                    record.anzName = record.userName;

                    if (!localParser.GetValue("7").Equals(""))
                        if (!Int32.TryParse(localParser.GetValue("7"), out record.kundeID))
                            record.kundeID = 0;

                    fileControl.writeIniFile(lsFileNameTmp, record);

                    if (!Program.gnvSplash.ibStatusPing || isHistory.Equals("0"))
                    {
                            lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameUms;
                            RecordUmsatz recordx;
                            recordx = fileControl.readUmsatzFile(lsFileNameTmp);
                            if (recordx == null)
                                recordx = new RecordUmsatz();

                            recordx.betrag += Double.Parse(localParser.GetValue("4"),Program.gnvDecFormat);
                            recordx.kennzVerlaengert = "d";
                            recordx.onlineZeit = "00:00";
                            recordx.termID = record.termID;
                            recordx.umsDatum = DateTime.Now;
                            if (!localParser.GetValue("8").Equals(""))
                                recordx.kontoEndstand = localParser.GetValue("8");
 
                            fileControl.writeUmsatzFile(lsFileNameTmp, recordx);
                    }

                    if (localParser.GetValue("6").Equals("2") || localParser.GetValue("6").Equals("3"))
                    {
                         lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameIni;
                         fileControl.deleteFile(lsFileNameTmp);

                        // fileControl.writeIniFile(lsFileNameTmp, record);
                    }

                } // Ende action=setums

               

                String lsReturn = "";
                lsFileName = Program.gnvSplash.isVerzNoNet;
                 if (localParser.GetValue("action").Equals("setums"))
                {
                    if ((localParser.GetValue("6").Equals("2") || localParser.GetValue("6").Equals("3")) && !Program.gnvSplash.ibStatusPing)
                    {
                        if (!localParser.GetValue("7").Equals(""))
                        {
                            lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameUms;
                            RecordUmsatz record;
                            record = fileControl.readUmsatzFile(lsFileNameTmp);
                            if (record == null)
                                record = new RecordUmsatz();

                            try
                            {
                                record.termID = Int32.Parse(localParser.GetValue("1"));
                            }
                            catch { }

                            ls = localParser.GetValue("2");
                            ls += " " + localParser.GetValue("3");
                            record.umsDatum = DateTime.Parse(ls);

                            if (!localParser.GetValue("4").Equals(""))
                                record.betrag += Double.Parse(localParser.GetValue("4"),Program.gnvDecFormat);

                            record.onlineZeit = localParser.GetValue("5");
                            record.kennzVerlaengert = "d";
                            record.kundeID = Int32.Parse(localParser.GetValue("7"));

                            if (!localParser.GetValue("9").Equals(""))
                                record.nrSMS = Int32.Parse(localParser.GetValue("9"));

                            if (!localParser.GetValue("8").Equals(""))
                                record.kontoEndstand = localParser.GetValue("8");
                            
                            if (!localParser.GetValue("7").Equals("0")) // Kunde umsatz
                            {
                                lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameUmsKonto;
                                // ums kennz verlaengert?
                                fileControl.writeUmsatzFile(lsFileNameTmp, record);
                            }

                            if (localParser.GetValue("7").Equals("0")) // Kunde umsatz
                            {
                                lsFileNameTmp = lsFileName + "\\" + inetConstants.isFileNameUms;
                                // ums kennz verlaengert?
                                fileControl.writeUmsatzFile(lsFileNameTmp, record);
                            }

                           
                        }
                        // Ini File löschen
                        fileControl.deleteFile(lsFileName + "\\" + inetConstants.isFileNameIni);

                    } // Ende schließen
                }

                 if (localParser.GetValue("action").Equals("ipaffili/get"))
                 {
                     List<RecordAffili> myList = new List<RecordAffili>();
                     String[] myTemp = localParser.GetValue("result").Split('#');

                     foreach (String temp in myTemp)
                     {
                         String[] nextaf = temp.Split(';');
                         RecordAffili raf = new RecordAffili();
                         raf.affiCode = nextaf[0];
                         raf.affiArt = nextaf[1];
                         raf.affiImage = nextaf[2];
                         raf.affiLink = nextaf[3];
                         raf.affiCountry = nextaf[4];

                         myList.Add(raf);
                     }

                      fileControl.writeAffiFile(lsFileName + "\\" + inetConstants.isFileNameAffili,myList);

                 }

                if (!lsReturn.Equals(""))
                {
                    mParameter = new inetStringParser();
                    mParameter.SetParamter(lsReturn);
                }

            }
            catch 
            {
            }
         

        }

        private String GetNoNetAsParam(String asFile, String asKennz)
        {
            String lsTmp = "";
           
            if (asKennz.Equals("I") || asKennz.Equals("")) // Ini File
            {
                lsTmp = GetIniFileAsParameter(asFile,asKennz);
            }
            else if (asKennz.Equals("K")) // Kundenumsatz
            {
                RecordUmsatz record = fileControl.readUmsatzFile(asFile);
                if (record != null)
                {
                    lsTmp = "K1=" + record.termID.ToString();
                    lsTmp += "&K2=" + record.umsDatum.ToString(inetConstants.isDateFormat); // Date
                    lsTmp += "&K3=" + record.umsDatum.ToString(inetConstants.isTimeFormat); // Zeit
                    lsTmp += "&K4=" + record.betrag.ToString();
                    lsTmp += "&K5=" + record.onlineZeit;
                    lsTmp += "&K7=" + record.kundeID.ToString();
                    lsTmp += "&K8=" + record.kontoEndstand;
                    lsTmp += "&K9=" + record.nrSMS.ToString();
                }
            }
            else if (asKennz.Equals("U")) // Umsatz
            {
                RecordUmsatz record = fileControl.readUmsatzFile(asFile);
                if (record != null)
                {
                    lsTmp = "U1=" + record.termID.ToString();
                    lsTmp += "&U2=" + record.umsDatum.ToString(inetConstants.isDateFormat); // Date
                    lsTmp += "&U3=" + record.umsDatum.ToString(inetConstants.isTimeFormat); // Zeit
                    lsTmp += "&U4=" + record.betrag.ToString();
                    lsTmp += "&U5=" + record.onlineZeit;
                    lsTmp += "&U6=" + record.kennzVerlaengert;
                    lsTmp += "&U7=" + record.kundeID.ToString();
                    lsTmp += "&U8=" + record.kontoEndstand;
                    lsTmp += "&U9=" + record.nrSMS.ToString();
                }
            }
            else if (asKennz.Equals("IO"))
            {
                RecordIni record = fileControl.readIniFile(asFile);
                if (record != null)
                {
                    lsTmp = "1=" + record.termID.ToString();
                    lsTmp += "\t2=" + record.userName;
                    lsTmp += "\t3=" + record.start.ToString(inetConstants.isDateFormat);
                    lsTmp += "\t5=" + record.start.ToString(inetConstants.isTimeFormat);
                    lsTmp += "\t4=" + record.ende.ToString(inetConstants.isDateFormat);
                    lsTmp += "\t6=" + record.ende.ToString(inetConstants.isTimeFormat);
                    lsTmp += "\t7=" + record.onlineZeit;
                    lsTmp += "\t9=" + record.kundeID.ToString();
                    lsTmp += "\t10=" + record.anzName;
                    lsTmp += "\t11=" + record.kennzAufladbar;
                    lsTmp += "\t12=" + record.kp_lang;
                }
            }
            else if (asKennz.Equals("RK")) // regkey
            {
                RecordKey record = fileControl.readKeyFile(asFile);
                if (record != null)
                {
                    lsTmp = "1=" + record.termID.ToString();
                    lsTmp += "\t2=" + record.kdID;
                    lsTmp += "\t3=" + record.standordID.ToString();
                    // Kennz Online lsTmp = "&4=" + record.;
                    lsTmp += "\t5=" + record.kennzGesperrt;
                    // letzte Bootconnect date lsTmp = "&6=" + record..ToString();
                    lsTmp += "\t7=" + record.webSite;
                    lsTmp += "\t8=" + record.pcGUID;
                    // Anzahl PWD lsTmp = "&10=" + record.
                    lsTmp += "\t15=" + record.pcName;
                    // Version lsTmp = "&17=" + record
                    lsTmp += "\t22=" + record.faName;
                    lsTmp += "\t23=" + record.faStr1;
                    lsTmp += "\t24=" + record.faStr2;
                    lsTmp += "\t26=" + record.kennzMP;
                    lsTmp += "\t27=" + record.kennzIKunKonto;
                    lsTmp += "\t28=" + record.kennzUmsatz;
                    lsTmp += "\t36=" + record.preisStunde.ToString();
                    lsTmp += "\t37=" + record.pwdEinPwd;
                    lsTmp += "\t38=" + record.pwdClientPwd;
                    lsTmp += "\t39=" + record.pwdTasten;
                    lsTmp += "\t40=" + record.preisDrucken.ToString();
                    lsTmp += "\t45=" + record.aufstellplatzID.ToString();
                    lsTmp += "\t46=" + record.freeSmsPkd.ToString();
                    lsTmp += "\t47=" + record.kennzJugend;
                    lsTmp += "\t48=" + record.kennzreboot;
                    lsTmp += "\t49=" + record.rebootTime.ToString(inetConstants.isTimeFormat);
                    lsTmp += "\t50=" + record.kennzShutdown;
                    lsTmp += "\t51=" + record.shutdownTime.ToString(inetConstants.isTimeFormat);
                    lsTmp += "\t52=" + record.werbeText;
                    lsTmp += "\t53=" + record.kennzrebootAlways;
                    lsTmp += "\t54=" + record.verzNoNet;
                    lsTmp += "\t55=" + record.idSkin.ToString();
                    lsTmp += "\t56=" + record.kennzShutdownBtn;
                    lsTmp += "\t57=" + record.kennzVOIP;
                    lsTmp += "\t58=" + record.kennzSMS;
                    lsTmp += "\t59=" + record.kennzGames;
                    lsTmp += "\t61=" + record.aufStWaehrung;
                    lsTmp += "\t62=" + record.aufStWaehrungCent;
                    lsTmp += "\t63=" + record.aufStLand.ToString();
                    lsTmp += "\t64=" + record.i_link;
                    lsTmp += "\t67=" + record.kennzKPExpired;//20100130
                    lsTmp += "\t70=" + record.ibAffili; //20111109
                }
            }
            else if (asKennz.Equals("IA"))
            {
                lsTmp = "";
                List<RecordAffili> record = fileControl.readAffiliFile(asFile);
                if (record != null)
                {

                    foreach (RecordAffili ra in record)
                    {
                        if (lsTmp.Equals(""))
                        {
                            lsTmp = "result=" + ra.affiCode + ";" + ra.affiArt + ";" + ra.affiImage + ";" + ra.affiLink + ";" + ra.affiCountry + "#";
                        }else
                            lsTmp += ra.affiCode + ";" + ra.affiArt + ";" + ra.affiImage + ";" + ra.affiLink + ";" + ra.affiCountry + "#";

                    }
                }
            }

            return lsTmp;
        }


        private String GetIniFileAsParameter(String asFile,String asKennz)
        {
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append("");

            RecordIni record = fileControl.readIniFile(asFile);
            if (record != null)
            {
                strQuery.Append(asKennz).Append("1=").Append(record.termID.ToString())
                        .Append("&").Append(asKennz).Append("2=").Append(record.userName)
                        .Append("&").Append(asKennz).Append("3=").Append(record.start.ToString(inetConstants.isDateFormat))
                        .Append("&").Append(asKennz).Append("5=").Append(record.start.ToString(inetConstants.isTimeFormat))
                        .Append("&").Append(asKennz).Append("4=").Append(record.ende.ToString(inetConstants.isDateFormat))
                        .Append("&").Append(asKennz).Append("6=").Append(record.ende.ToString(inetConstants.isTimeFormat))
                        .Append("&").Append(asKennz).Append("7=").Append(record.onlineZeit)
                        .Append("&").Append(asKennz).Append("9=").Append(record.kundeID.ToString())
                        .Append("&").Append(asKennz).Append("10=").Append(record.anzName)
                        .Append("&").Append(asKennz).Append("11=").Append(record.kennzAufladbar)
                        .Append("&").Append(asKennz).Append("12=").Append(record.kp_lang);
            }
           
            record = null;
            return strQuery.ToString();
        }



        public String GetParameterValue(String asKey)
        {
            if (mParameter != null)
                return mParameter.GetValue(asKey);
            else
                return "";
        }

        public void SetParameterValue(String asKey, String asValue)
        {
            mParameter.SetValue(asKey, asValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asFile"></param> - File to download from Webserver!
        /// <param name="asLocalDirectory"></param> - Destination folder where to copy !
        /// <param name="asLocalFileName"></param> - New Filename to save as (in destination folder)!
        /// <returns></returns>
        public Boolean fileDownload(String asFile, String asLocalDirectory, String asLocalFileName)
        {
            bool lbOk = true;
            bool lbStatus = false;

         
                try
                {
                    String lsFileName = asLocalDirectory + asLocalFileName;
Console.WriteLine("lsFileName:" + lsFileName);
                    if (File.Exists(lsFileName))
                    {
                        // 20100106
                        FileInfo fInfox = new FileInfo(lsFileName);
                        if (fInfox.Length >= 10000 &&
                            File.GetLastAccessTime(lsFileName).ToString(inetConstants.isDateFormat).Equals(DateTime.Now.ToString(inetConstants.isDateFormat)))
                        {
                            //asLocalKey = asLocalKey.Substring(0, asLocalKey.Length - 7);
                            Console.WriteLine("FileLength or Date");
                            if (!File.Exists(asLocalDirectory + "index.html"))
                            {
                                Console.WriteLine("index.html doesn't exist, now create");
                                lsFileName = asLocalDirectory + "all.zip";
                                //asFile = asLocalKey.Substring(0, asLocalKey.LastIndexOf("\\") + 1);
                                ExtractZipFile(lsFileName, asLocalDirectory);// asFile);
                                fInfox = null;
                            }
                            return true;
                        }
                        fInfox = null;
                    }

                    // im Verzeichnis liegende ZIP-File nicht von heute oder nicht vorhanden!
                    try
                    {
                        //lsTmp = asLocalKey.Substring(0, asLocalKey.LastIndexOf("\\") + 1);
                        if (!Directory.Exists(asLocalDirectory)) // lsTmp
                            Directory.CreateDirectory(asLocalDirectory);

                        if (File.Exists(lsFileName + ".in")) // asLocalKey
                            File.Delete(lsFileName + ".in");

                        
                        int zaehler = 0;
                        if (Program.gnvSplash.ibStatusPing)
                        {
						if (File.Exists(lsFileName))
                            File.Move(lsFileName, lsFileName + ".in");

                            while (zaehler < 3)
                            {
                                try
                                {
                                    Console.WriteLine("downloading....1:> " + asFile + " / " + lsFileName);
                                    WebClient client = new WebClient();
                                    client.DownloadFile(asFile, lsFileName);

                                    FileInfo fInfo = new FileInfo(lsFileName);
                                    Console.WriteLine("downloading....11:" + zaehler);
                                    if (fInfo.Length <= 10000)
                                        zaehler++;
                                    else
                                    { // 
                                        fInfo = null;
                                        client = null;
                                        Console.WriteLine("downloading....beendet");
                                        break;
                                    }
                                    fInfo = null;
                                    client = null;

                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch
                                {
                                    zaehler++; //20100130
                                    lbStatus = true;
                                }
                            }
                        }
                        else
                            lbStatus = true;

                        try
                        {
                            // 20100130
                            if (lbStatus)
                            {   // $itmpcp-folder -> $isp-folder kopieren
                                if (!Directory.Exists(Program.gnvSplash.isFileUrlIPT)) // lsTmp
                                    Directory.CreateDirectory(Program.gnvSplash.isFileUrlIPT);
                                if (File.Exists(Program.gnvSplash.isFileUrlIPT +"all.zip")) // 20100130
                                    File.Copy(Program.gnvSplash.isFileUrlIPT + "all.zip", lsFileName);
 
                            }
                        }
                        catch
                        {
                            // 20100106
                            lbOk = false;
                        }

                    }
                    catch
                    {
                        lbOk = false;
                    }

                    if (lbOk)
                    {
                        // asFile = asLocalKey.Substring(0, asLocalKey.LastIndexOf("\\") + 1);
                        ExtractZipFile(lsFileName, asLocalDirectory);
                        Console.WriteLine("Extract zip file:");
                    }
                    else // 20100106
                    {
                        Console.WriteLine("else giriyooo");
                    }
                }
                catch
                { }
         
            return lbOk;

        }


        public Boolean BildDownload(String asFile, String asLocalKey)
        {
            bool lbOk = true;
            
            // 20100130 - keine Netzverbindungen versuchen!!!
            if (Program.gnvSplash.ibStatusPing == false)
                return true;

            try
            {
                if (File.Exists(asLocalKey))
                {
                    return true;
                }
                try
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(asFile, asLocalKey);
                    client = null;
                }
                catch
                {
                    lbOk = false;
                }
            }
            catch { }

            return lbOk;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asFile"></param>
        /// <param name="asLocalKey"></param>
        public void ExtractZipFile(String asFile, String asLocalKey)
        {
            try
            {
                ICSharpCode.SharpZipLib.Zip.FastZip tmpZip;
                tmpZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                tmpZip.ExtractZip(asFile, asLocalKey, "");
                tmpZip = null;
            }
            catch 
            { }
        }
    
    }

    /// <summary>
    /// 
    /// </summary>
    public static class inetBase64
    {
        public static byte[] encodeString(String aString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            
            byte[] bytes = encoding.GetBytes(aString);

            return bytes;
        }

        public static String encodeAsString(String aString)
        {
            Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            //UTF8Encoding encoding = new UTF8Encoding();
            
            byte[] bytes = encoding.GetBytes(aString);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aString"></param>
        /// <returns></returns>
        public static String decodeString(String aString)
        {
            Encoding encoder = System.Text.Encoding.GetEncoding("ISO-8859-1"); 
            Decoder decoder = encoder.GetDecoder();
            byte[] todecodebyte = Convert.FromBase64String(aString);
            int charCount = decoder.GetCharCount(todecodebyte, 0, todecodebyte.Length);

            char[] decodechar = new char[charCount];
            decoder.GetChars(todecodebyte, 0, todecodebyte.Length, decodechar, 0);

            aString = new String(decodechar);
            
            return aString;
        }
    }
}
