using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace kioskplus.Utils
{
    public class inetFileCtrl
    {
        FileStream fsReader;

        public RecordIni readIniFile(String asFile)
        {
            RecordIni record = null;
           
            try{

            if (File.Exists(asFile)){
                record = new RecordIni();
                BinaryFormatter binFormatter = new BinaryFormatter();

                fsReader = File.OpenRead(asFile);
                record = (RecordIni)binFormatter.Deserialize(fsReader);
                fsReader.Close();
            }
            }
            catch (Exception ex)
            { fsReader.Close(); 
                Console.WriteLine("FileReadIni:" + ex.Message);
                record = null;
            }
            return record;
        }


        public RecordKey readKeyFile(String asFile)
        {
            RecordKey record = null;
           
            try{
                if (File.Exists(asFile))
                {
                    record = new RecordKey();
                    BinaryFormatter binFormatter = new BinaryFormatter();

                    fsReader = File.OpenRead(asFile);
                    record = (RecordKey)binFormatter.Deserialize(fsReader);
                    fsReader.Close();
                }
            }
            catch (Exception ex)
            {
                fsReader.Close();
                Console.WriteLine("FileReadKey:" + ex.Message);
                record = null;
            }
            return record;
        }

        public RecordUmsatz readUmsatzFile(String asFile)
        {
            RecordUmsatz record = null;
            

            if (File.Exists(asFile))
            {
                record = new RecordUmsatz();
                BinaryFormatter binFormatter = new BinaryFormatter();

                try
                {
                    fsReader = File.OpenRead(asFile);
                    record = (RecordUmsatz)binFormatter.Deserialize(fsReader);

                    fsReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("FileReadUmsatz:" + e.Message);
                    fsReader.Close();
                    record = null;
                }
            }
            return record;
        }


        public void writeUmsatzFile(String asFile, RecordUmsatz argRecord)
        {
            deleteFile(asFile);

            BinaryFormatter binFormatter = new BinaryFormatter();
            try
            {
                fsReader = File.OpenWrite(asFile);
                binFormatter.Serialize(fsReader, argRecord);
                fsReader.Close();
            }
            catch (Exception ex)
            { Console.WriteLine("FileWriteUmsatz:" + ex.Message); }
        }

        public void writeKeyFile(String asFile, RecordKey argRecord)
        {
            deleteFile(asFile);

            BinaryFormatter binFormatter = new BinaryFormatter();
            try{

                fsReader = File.OpenWrite(asFile);
                binFormatter.Serialize(fsReader, argRecord);
                fsReader.Close();
            }
            catch (Exception ex)
            { fsReader.Close(); 
                Console.WriteLine("FileWriteKey:" + ex.Message); }
        }

        public void writeIniFile(String asFile, RecordIni argRecord)
        {
            deleteFile(asFile);

            BinaryFormatter binFormatter = new BinaryFormatter();
            try
            {
                fsReader = File.OpenWrite(asFile);
                binFormatter.Serialize(fsReader, argRecord);
                fsReader.Close();
            }
            catch (Exception ex)
            { fsReader.Close(); Console.WriteLine("FileWriteIni:" + ex.Message); }
        }

        public void deleteFile(String asFile)
        {
            try
            {
                File.Delete(asFile);
            }
            catch(Exception e)
            {
                Console.WriteLine("Filedelete:" + e.Message);
            //    System.Windows.Forms.MessageBox.Show("FileDelete:" + e.Message); 
            }
        }



        public List<RecordAffili> readAffiliFile(String asFile)
        {
            //RecordAffili record = null;
            List<RecordAffili> mList = null; // new List<RecordAffili>();

            if (File.Exists(asFile))
            {
               // record = new RecordAffili();
                

                BinaryFormatter binFormatter = new BinaryFormatter();

                try
                {
                    fsReader = File.OpenRead(asFile);
                    mList = (List<RecordAffili>)binFormatter.Deserialize(fsReader);

                    fsReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("FileReadAffili:" + e.Message);
                    fsReader.Close();
                    mList = null;
                }
            }
            return mList;
        }

        public void writeAffiFile(String asFile, List<RecordAffili> argRecord)
        {
            deleteFile(asFile);

            BinaryFormatter binFormatter = new BinaryFormatter();
            try
            {
                fsReader = File.OpenWrite(asFile);
                binFormatter.Serialize(fsReader, argRecord);
                fsReader.Close();
            }
            catch (Exception ex)
            { fsReader.Close(); Console.WriteLine("FileWriteAffili:" + ex.Message); }
        }


    }

    [Serializable]
    public class RecordUmsatz
    {
        public int termID = 0; // terminal ID
        public int kundeID = 0; // kunde ID
        public DateTime umsDatum = DateTime.Now; // start date time
        public double betrag = 0; // betrag
        public int nrSMS = 0; // SMS
        public String onlineZeit = String.Empty; // online zeit
        public String kennzVerlaengert = String.Empty;
        public String kontoEndstand = String.Empty; // Konto Endstand
    }

    [Serializable]
    public class RecordIni
    {
        public int termID = 0; // terminal ID
        public int kundeID = 0; // kunde ID
        public DateTime start = DateTime.Now; // start date time
        public DateTime ende = DateTime.Now; // ende date time
        public String onlineZeit = String.Empty; // Online Zeit
        public String userName = String.Empty; // User name
        public String anzName = String.Empty; // Anzeigename
        public String kennzAufladbar = "1"; // Kennz aufladbar
        public String kp_lang = ""; // Language ID
    }

    [Serializable]
    public class RecordKey
    {
        public int termID = 0; // terminal ID
        public int standordID = 0;//Standord
        public String kdID = String.Empty; // KioskID
        public String kennzGesperrt = String.Empty; // Kennzeichen gesperrt
        public String webSite = String.Empty;// WebSite
        public String pcGUID = String.Empty; // PC Guid
        public String faName = String.Empty;//Firma name
        public String faStr1 = String.Empty;//Strasse
        public String faStr2 = String.Empty;//Strasse
        public String kennzMP = String.Empty;//Kennzeichen MP
        public String pcName = String.Empty;//Pc Name
        public String pwdTasten = String.Empty;//
        public String pwdEinPwd = String.Empty;
        public String pwdClientPwd = String.Empty;
        public double preisStunde = 1;//Preis
        public String kennzIKunKonto = String.Empty; // IKundenKonto
        public String kennzUmsatz = String.Empty; // Kennzeichen Umsatz
        public double preisDrucken = 0;// Drucken preis
        public int aufstellplatzID = 0;//Aufstellplatz
        public int freeSmsPkd = 0;//Free SMS pro Kunde
        public String kennzJugend = String.Empty;//Kennzeichen jugendschutz
        public String kennzreboot = String.Empty;//Kennezeichen Reboot
        public DateTime rebootTime;//Reboot time
        public String kennzShutdown;//Kennezeichen Shutdown
        public DateTime shutdownTime;// Shutdown time
        public String werbeText = String.Empty;//Werbetext
        public String kennzrebootAlways = String.Empty;//Reboot immer?
        public String verzNoNet = String.Empty;//
        public int idSkin = 1;
        public String kennzShutdownBtn = String.Empty;// Shutdown Button visible?
        public String kennzVOIP = String.Empty;
        public String kennzSMS = String.Empty;
        public String kennzGames = String.Empty;
        public String aufStWaehrung = String.Empty;
        public String aufStWaehrungCent = String.Empty;
        public int aufStLand = 0;
        public String i_link = String.Empty;
        public String kennzKPExpired = "0";
        public int ibAffili = 0;
    }

    [Serializable]
    public class RecordAffili
    {
        public String affiCode = "";
        public String affiArt = "";
        public String affiImage = "";
        public String affiLink = "";
        public String affiCountry = "";
    }

}
