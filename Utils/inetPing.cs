using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net.Sockets;

namespace kioskplus
{
    public class inetPing
    {
        /// <summary>
        /// local variables
        /// </summary>
        String host_addr = null;
        int timeout = 100;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="h: host"></param>
        /// <param name="t: timeout"></param>
        public inetPing( String h, int t)
        {
            host_addr = h;
            timeout = t;
        }
 
        /// <summary>
        /// Ping thread main loop.
        /// </summary>
        public int PingHost()
        {
           // PingReply reply = null;
            int returnValue = -1;


            try
            {
                if (!Ping(host_addr))
                    return -1;

            }
            catch
            { return -1; }


            try
            {
                TcpClient tp = new TcpClient();
               
                tp.Connect(host_addr, Program.gnvSplash.iPort);
                if (tp.Connected)
                {
                    Console.WriteLine("ok");
                    returnValue = 1;
                }
                else
                    Console.WriteLine("not ... ok");

                tp.Close();
        }
        catch (Exception ee)
        { 
            Console.WriteLine("w:"+ee.Message);
            returnValue = -1;
            return -1;
        }
            return returnValue;
        }


        public int PingHost(String s)
        {
            host_addr = s;
            return PingHost();
        }


        private bool Ping(String s)
        {
            Ping netMon = new Ping();

            byte[] buffer = new byte[32]; //der Buffer
            PingOptions pingOptions = new PingOptions();
            
            PingReply reply = netMon.Send(host_addr, timeout, buffer, pingOptions); //Senden der Anfrage
            
            //Statusüberprüfung
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else if (reply.Status == IPStatus.TimedOut)
            {
                return true;
            }
            return false;
        }
    }
}
