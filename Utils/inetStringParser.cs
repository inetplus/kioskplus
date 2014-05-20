using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace kioskplus
{
   
    public class inetStringParser
    {
        Hashtable mParameter = null;

        public void SetParamter(String asText)
        {

            String[] sTmp;
            asText = asText.Trim();

            if (asText =="" || asText.Length <= 0){
                return;
            }

            if (asText.StartsWith("0=0"))
                asText = asText.Substring(3);

            char[] chDelimiter ={'\t'};
            String[] spl = { "(#)" };

            mParameter = new Hashtable();
            int liTmp;
            String lsZeichen;
            String lsNumber;

            sTmp = asText.Split(chDelimiter);
            foreach (string s in sTmp)
            {

                // überspringen leere Ergebnisse! 20100130
                if (s.Equals(string.Empty))
                {
                    continue;
                }

                liTmp = s.IndexOf("=");

                if (liTmp == -1) //20100130
                    continue;

                try
                {
                    lsZeichen = String.Empty; // 20100130
                    if (s.Length > liTmp + 1) // 20100130
                        lsZeichen = s.Substring(liTmp + 1);


                    try
                    {
                        int tmpZeichen = lsZeichen.IndexOf("(#)");
                        if (tmpZeichen > -1)
                        {
                            String[] trSplit = lsZeichen.Split(spl, StringSplitOptions.None);
                            lsZeichen = "";
                            foreach (String tmpStr in trSplit)
                            {
                                if (!tmpStr.Equals(""))
                                {
                                    if (tmpStr.StartsWith("kpc"))
                                    {
                                        try
                                        {
                                            lsZeichen += Program.getMyLangString(tmpStr);
                                        }
                                        catch
                                        { lsZeichen = String.Empty; }
                                    }
                                    else
                                        lsZeichen += tmpStr;
                                }
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            //lsZeichen = s.Substring(liTmp + 1);
                            lsZeichen = String.Empty; // 20100130
                            if (s.Length > liTmp + 1) // 20100130
                                lsZeichen = s.Substring(liTmp + 1);
                        }
                        catch
                        { 
                            lsZeichen = String.Empty;
                        }
                    }

                    mParameter.Add(s.Substring(0, liTmp), lsZeichen);
                }
                catch
                { }
            }
        }

        public void SetParamter(String asText, char[] chDeli)
        {

            String[] sTmp;
            asText = asText.Trim();

            if (asText == "" || asText.Length <= 0)
            {
                return;
            }


            mParameter = new Hashtable();
            int liTmp;

            sTmp = asText.Split(chDeli);
            foreach (string s in sTmp)
            {
                // Leere Werte überspringen 20100130
                if (s.Equals(string.Empty))
                {
                    continue;
                }

                liTmp = s.IndexOf("=");

                if (liTmp == -1) //20100130
                    continue;

                try
                {
                    if (s.Length > liTmp + 1)
                    {
                        mParameter.Add(s.Substring(0, liTmp), s.Substring(liTmp + 1));
                    }
                    else
                    {
                        mParameter.Add(s.Substring(0, liTmp), "");
                    }

                }
                catch
                { }
            }
        }
        public String GetValue(String asKey)
        {
            String lsTmp="";

            if (mParameter.ContainsKey(asKey))
            {
                lsTmp = (String)mParameter[asKey];
            }

            return lsTmp;
        }

        public void SetValue(String asKey, String asValue)
        {
            try
            {
                if (mParameter == null)
                    return;

                if (mParameter.ContainsKey(asKey))
                    mParameter.Remove(asKey);

                mParameter.Add(asKey, asValue);
            }
            catch
            { }
        }
    }
}
