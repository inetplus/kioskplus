using System;
using System.Collections.Generic;
using System.Threading;


namespace kioskplus.Utils
{
    public class inetDateTime
    {
        private DateTime idStartFreigabe;
        private DateTime idEndeFreigabe;

        public static object monitorEndeRechnen = new object();
        public static object endeDateTime = new object();
        public static object monitorNutzDauer = new object();
        public static object monitorRestDauer = new object();

        public inetDateTime()
        {
            idStartFreigabe = DateTime.Now;
            idEndeFreigabe = idStartFreigabe;
        }


        /// <summary>
        /// addiert die als parameter angegebene Minuten auf Ende-DateTime
        /// </summary>
        /// <param name="aiMinuten"></param>
        public void SetEndeDateTime(int aiMinuten)
        {
           Monitor.Enter(endeDateTime);
           idEndeFreigabe = idEndeFreigabe.AddMinutes(aiMinuten);
           Monitor.Exit(endeDateTime);
        }

        /// <summary>
        /// liefert das Ende-Datum als String im Format yyyy-mm-dd
        /// </summary>
        /// <returns></returns>
        public String GetEndeDate()
        {
            return idEndeFreigabe.ToString(inetConstants.isDateFormat);
        }

        /// <summary>
        /// liefert die Ende-Zeit als String im Format hh:mm:ss
        /// </summary>
        /// <returns></returns>
        public String GetEndeTime()
        {
            return idEndeFreigabe.ToString(inetConstants.isTimeFormat);
        }

        /// <summary>
        /// liefert das Start-Date als String im Format yyyy-mm-dd
        /// </summary>
        /// <returns>Start-Date yyyy-mm-dd</returns>
        public String GetStartDate()
        {
            return idStartFreigabe.ToString(inetConstants.isDateFormat);
        }

        /// <summary>
        /// liefert die Start-Zeit als String im Format hh:mm:ss
        /// </summary>
        /// <returns>Start-Zeit hh:mm:ss</returns>
        public String GetSartTime()
        {
            return idStartFreigabe.ToString(inetConstants.isTimeFormat);
        }

        /// <summary>
        /// setzt die Start-DateTime. 
        /// Es wird verwendet wenn der Rechner neu gestartet wird und der Recher vorher frei war!
        /// </summary>
        /// <param name="adt"></param>
        public void SetStartDateTime(DateTime adt)
        {
            idStartFreigabe = adt;
        }

        /// <summary>
        /// setzt die Ende-DateTime erneut
        /// </summary>
        /// <param name="adt"></param>
        public void SetEndeDateTime(DateTime adt) //SetEndeFreigabe -> SetEndeDateTime
        {
            Monitor.Enter(monitorEndeRechnen);
            idEndeFreigabe = adt;
            Monitor.Exit(monitorEndeRechnen);
        }


       /// <summary>
       /// liefert die Nutzdauer als String im Format hh:mm zurück
       /// </summary>
       /// <returns>Die Nutzdauer hh:mm</returns>
        public String GetNutzDauerAsString()
        {
            Monitor.Enter(monitorNutzDauer);
            TimeSpan ts;
            String lsDate = String.Empty;

            ts = DateTime.Now.Subtract(idStartFreigabe);

            lsDate = GetTSAsString(ts);

            Monitor.Exit(monitorNutzDauer);
            return lsDate;
        }

        /// <summary>
        /// liefert die RestDauer als Minuten (Integer)
        /// </summary>
        /// <returns>Rest-Minuten integer</returns>
        public int GetRestDauerAsMin()
        {
            Monitor.Enter(monitorRestDauer);
            DateTime ldDate = DateTime.Now;
            TimeSpan ltspan;
            
            ltspan = idEndeFreigabe.Subtract(DateTime.Now);

            Monitor.Exit(monitorRestDauer);
            return (int)ltspan.TotalMinutes;
        }


      /// <summary>
      /// rechnet die Stunden und Minuten für den Credit-Betrag
      /// </summary>
      /// <param name="adCredit">Credit</param>
      /// <param name="adStunden">Stunden-Preis</param>
      /// <param name="aihh">liefert als Referenz die Stunden vom Credit zurück</param>
      /// <param name="aimm">liefert als Referenz die Minuten vom Credit zurück</param>
        public void GetZeitFromCredit(double adCredit,double adStunden, ref int aihh, ref int aimm)
        {

            if (adStunden == 0)
                adStunden = 1;

            // adCredit = 1 Std kostet adcredit
            int liGesamtMin= 0;
            DateTime ldt = idStartFreigabe;

            adCredit = adCredit * 60;
            liGesamtMin = (int)Math.Round((adCredit / adStunden));

            ldt = ldt.AddMinutes(liGesamtMin);

            SetEndeDateTime(liGesamtMin);

            TimeSpan ts;
            ts = ldt.Subtract(idStartFreigabe);

            aihh = ts.Days * 24 + ts.Hours;
            aimm = ts.Minutes;

        }

        /// <summary>
        /// liefert die Zeit als hh:mm von dem angegebenen Minuten
        /// </summary>
        /// <param name="aiMinuten"></param>
        /// <returns>hh:mm</returns>
        public String GetZeitFromMinuten(int aiMinuten)
        {
            TimeSpan ts;
            DateTime ldt;
            String ls = String.Empty;

            ldt = idStartFreigabe;

            ldt = ldt.AddMinutes(aiMinuten);

            ts = ldt.Subtract(idStartFreigabe);

            ls = GetTSAsString(ts);

            return ls;
        }

        /// <summary>
        /// liefert die RestOnline-Zeit zurück
        /// </summary>
        /// <param name="adt"></param>
        /// <returns>Rest-Onlinezeit hh:mm</returns>
        public String GetOnlineZeitAsString()
        {
            String lsDate;
            TimeSpan ts;

            ts = idEndeFreigabe.Subtract(DateTime.Now);

            lsDate = GetTSAsString(ts);

            return lsDate;
        }

        /// <summary>
        /// liefert die gesamte Online-Zeit zurück
        /// </summary>
        /// <returns></returns>
        public String GetGesamtOnlineZeit()
        {
            String lsDate;
            TimeSpan ts;

            ts = idEndeFreigabe.Subtract(idStartFreigabe);
            lsDate = GetTSAsString(ts);

            return lsDate;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="argSpan"></param>
        /// <returns></returns>
        private String GetTSAsString(TimeSpan argSpan)
        {
            String lsDate = String.Empty;

            int tmp = argSpan.Days * 24 + argSpan.Hours;
            if (tmp < 0)
                tmp = 0;

            lsDate = tmp.ToString();

            if (lsDate.Length == 1)
                lsDate = "0" + lsDate;

            tmp = argSpan.Minutes;
            if (tmp < 0)
                tmp = 0;

            if (tmp < 10)
                lsDate += ":0" + tmp.ToString();
            else
                lsDate += ":" + tmp.ToString();

                return lsDate;
        }

        /// <summary>
        /// liefert das gesamte minuten ....
        /// </summary>
        /// <param name="adt"></param>
        /// <returns></returns>
        public int GetMinutenFromDT(DateTime adt)
        {
            TimeSpan ts;

            ts = adt.Subtract(idStartFreigabe);

            return (int)ts.TotalMinutes;
        }

        /// <summary>
        /// liefert Minuten vom OnlineZeit 
        /// </summary>
        /// <param name="asOnline">Onlinezeit im Format hh:mm</param>
        /// <returns>die gesamte Minuten as int</returns>
        public int GetMinutenFromOnlineZeit(String asOnline)
        {
            if (asOnline.Equals(""))
                return 0;

            int liMinuten = 0;
            String[] lsTmp;
            char[] c = { ':' };
            lsTmp = asOnline.Split(c);
            try
            {
                liMinuten = Int32.Parse(lsTmp[0]) * 60 + Int32.Parse(lsTmp[1]);
            }
            catch (Exception ex)
            { liMinuten = 1; }

            return liMinuten;
        }

        public String GetZeit(int argInteger)
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
                lsReturn += ":" + mm.ToString();

            return lsReturn;

        }
    }
}
