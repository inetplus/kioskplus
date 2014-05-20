using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace kioskplus.Utils
{
    class kpRest
    {
        public static String getURI(String uri, int index)
        {
            String returnValue = "";

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Stream respStream = webResponse.GetResponseStream();
                StreamReader sReader = new StreamReader(respStream);

                returnValue = (sReader.ReadToEnd());

                sReader.Close();
                respStream.Close();

                webResponse.Close();


                try
                {
                    UTF8Encoding enc = new UTF8Encoding();
                    ReqResult re = Deserialize(enc.GetBytes(returnValue));

                    if (re != null)
                        returnValue = re.result;

                }
                catch (Exception ex)
                {
                }



            }
            catch (WebException webex)
            {
                // error handling
                if (webex.Response != null)
                {
                    HttpWebResponse hwResponse = (HttpWebResponse)webex.Response;
                    Program.gnvSplash.ipHttpStatusCode[index] = hwResponse.StatusCode;
                }
                else
                {
                    Program.gnvSplash.ipHttpStatusCode[index] = HttpStatusCode.NotFound;
                }

                returnValue = "";
            }
            catch (Exception ex)
            {
                Program.gnvSplash.ipHttpStatusCode[index] = HttpStatusCode.NotFound;
                returnValue = "";
            }
            
            return returnValue;
        }


        public static String postURI(String uri, Dictionary<String, String> postDataDictionary)
        {
            String returnValue = "";

            try
            {

                string postData = "";

                foreach (KeyValuePair<string, string> kvp in postDataDictionary)
                {
                    postData += string.Format("{0}={1}&", HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value));
                }

                postData = postData.Remove(postData.Length - 1); // remove the trailing ampersand

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Method = "POST";

                byte[] postArray = Encoding.UTF8.GetBytes(postData);

                req.ContentType = "text/xml"; // "application/x-www-form-urlencoded";
                req.KeepAlive = false;
                req.Timeout = 5000;
                req.ContentLength = postArray.Length;


                Stream dataStream = req.GetRequestStream();
                dataStream.Write(postArray, 0, postArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream,Encoding.UTF8);


                returnValue = (reader.ReadToEnd());
                
                Console.WriteLine(returnValue);

                reader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                returnValue = "";
            }

            return returnValue;
        }


        private static String Serialize(ReqResult emp)
        {
            try
            {
                String XmlizedString = null;
                XmlSerializer xs = new XmlSerializer(typeof(ReqResult));
                //create an instance of the MemoryStream class since we intend to keep the XML string 
                //in memory instead of saving it to a file.
                MemoryStream memoryStream = new MemoryStream();
                //XmlTextWriter - fast, non-cached, forward-only way of generating streams or files 
                //containing XML data
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //Serialize emp in the xmlTextWriter
                xs.Serialize(xmlTextWriter, emp);
                //Get the BaseStream of the xmlTextWriter in the Memory Stream
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                //Convert to array
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return "";
        }

        private static ReqResult Deserialize(byte[] xmlByteData)
        {
            try
            {
                XmlSerializer ds = new XmlSerializer(typeof(ReqResult));
                MemoryStream memoryStream = new MemoryStream(xmlByteData);
                ReqResult emp = new ReqResult();
                emp = (ReqResult)ds.Deserialize(memoryStream);
                return emp;
            }
            catch (Exception ex)
            {
                //errHandler.ErrorMessage = dal.GetException();
                // errHandler.ErrorMessage = ex.Message.ToString();
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }


    }
}
