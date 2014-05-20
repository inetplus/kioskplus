using System;
using System.Collections.Generic;
using System.Threading;
using whCcTalkCommunication;
using kioskplus.Utils;


namespace kioskplus
{
    public struct SelCredit
    {
        public string ID;
        public double Total;
    }

    public class UsbDriversCtrl
    {
        private MPUsb invMpUsb = null;
        private BillUsb invBillUsb = null;
        private Boolean ibPolling = true;
        private whCcTalkDeviceList currentDevices;

        private Thread thMain = null;
        private ThreadStart thStart = null;
        private String isKennzeichen = String.Empty;

        public UsbDriversCtrl()
        {
            Console.WriteLine("mp01:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
            currentDevices = new whCcTalkCommunication.whCcTalkDeviceList();
            Console.WriteLine("mp02:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
        }

        public int searchDevices(Boolean abPin, whPortTypes awhPort)
        {
            Console.WriteLine("mp03:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
            if (currentDevices == null)
                return 0;

            if (awhPort == null)
                awhPort = whPortTypes.USB;

            byte[] pin = new byte[6] { 1, 2, 3, 4, 5, 6 };
            currentDevices.SearchPortType = awhPort;
            Console.WriteLine("mp031:" + DateTime.Now.ToString("HH:mm:ss ffff"));
            currentDevices.InDepthSearch = false;
            currentDevices.SearchDevices(pin);
            
            Console.WriteLine("mp032:" + DateTime.Now.ToString("HH:mm:ss ffff"));
            if (currentDevices.Count > 0)
            {
                for (int i = 0; i < currentDevices.Count; i++)
                {
                    if (currentDevices.CcTalkDevices[i].OpenComm() == whCcTalkErrors.Ok)
                    {
                        Console.WriteLine("mp033:" + DateTime.Now.ToString("HH:mm:ss ffff"));
                        currentDevices.CcTalkDevices[i].SendBreak();
                        if (currentDevices.CcTalkDevices[i].Category == whCcTalkCategory.CoinSelector)
                        {
                            invMpUsb = new MPUsb(currentDevices.CcTalkDevices[i]);
                        }
                        else if (currentDevices.CcTalkDevices[i].Category == whCcTalkCategory.BillValidator)
                        {
                            invBillUsb = new BillUsb(currentDevices.CcTalkDevices[i]);
                        }
                    }
Console.WriteLine("mp034:" + DateTime.Now.ToString("HH:mm:ss ffff"));
                    currentDevices.CcTalkDevices[i].CloseComm();
Console.WriteLine("mp035:" + DateTime.Now.ToString("HH:mm:ss ffff"));
                }
            }
            Console.WriteLine("mp04:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
            return currentDevices.Count;
        }

        public MPUsb GetMPControl()
        {
            return invMpUsb;
        }

        public BillUsb GetBilControl()
        {
            return invBillUsb;
        }


        public void StartPolling(String asKennzeichen)
        {
            Console.WriteLine("mp05:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
            searchDevices(false, whPortTypes.USB);

            isKennzeichen = asKennzeichen;
            switch (asKennzeichen)
            {
                case "1":
                    {
                        if (invMpUsb != null)
                            if (invMpUsb.StartMPPolling() != 1)
                            {
                                Program.gnvSplash.ibOwner = invMpUsb.IsInet();
                              
Console.WriteLine("USB:" + invMpUsb.IsInet().ToString());
                                invMpUsb = null;
                            }
                        if (invMpUsb != null)
                            Program.gnvSplash.ibOwner = invMpUsb.IsInet();
                        invBillUsb = null;
                        break;
                    }
                case "2":
                    {
                        if (invBillUsb != null)
                            if (invBillUsb.StartBillPolling() != 1)
                                invBillUsb = null;

                        invMpUsb = null;
                        break;
                    }
                case "3":
                    {
                        if (invBillUsb != null )
                            if (invBillUsb.StartBillPolling() != 1)
                                invBillUsb = null;

                        if (invMpUsb != null)
                            if (invMpUsb.StartMPPolling() != 1)
                            {
                                Program.gnvSplash.ibOwner = invMpUsb.IsInet();
                                invMpUsb = null;
                            }
                        break;
                    }
            }
            Console.WriteLine("mp06:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
            // wenn eine von beiden exist
            if (invBillUsb != null || invMpUsb != null)
          {
                try
                {
                    thStart = new ThreadStart(Polling);
                    thMain = new Thread(thStart);
                    thMain.Name = "ippolling";

                    thMain.SetApartmentState(ApartmentState.STA);
                    thMain.Priority = ThreadPriority.Normal;
                    // Thread.Sleep(2500);
                    thMain.Start();
                    Console.WriteLine("mp07:" + DateTime.Now.ToString("HH:mm:ss ffff")); 
                }
                catch
                { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void pollingMP()
        {
            double credit = 0;
            int i = 0;
            string id = string.Empty;

                if (invMpUsb != null)
                {
                    i = i + 2;
                    i = 2;
                    invMpUsb.Polling(ref credit, ref id);
                    i = 21;
                    if (credit > 0)
                    {
                        if (Program.gnvSplash != null)
                            Program.gnvSplash.SetWert(credit, id);
                        credit = 0;
                    }
                }
          }
           
        /// <summary>
        /// 
        /// </summary>
        private void pollingBill()
        {
            double credit = 0;
            int i = 0;
            string id = string.Empty;
            if (invBillUsb != null)
            {
                i = i + 3;
                i += 2;
                i = i * 21;
                invBillUsb.Polling(ref credit, ref id);
                if (credit > 0)
                {
                    Program.gnvSplash.SetWert(credit,id);
                    credit = 0;
                }
            }
        }

        public void Polling()
        {
            if (Program.gnvSplash.DialogShutz != null && ibPolling)
                if (isKennzeichen.Equals("1"))
                    Program.gnvSplash.DialogShutz.SwitchColor(System.Drawing.Color.Lime, System.Drawing.Color.Black);
                else if (isKennzeichen.Equals("2"))
                    Program.gnvSplash.DialogShutz.SwitchColor(System.Drawing.Color.Black, System.Drawing.Color.Lime);
                else
                    Program.gnvSplash.DialogShutz.SwitchColor(System.Drawing.Color.Lime, System.Drawing.Color.Lime);

            if (Program.gnvSplash != null)
                Program.gnvSplash.ibUSBCtrlPolled = true;

            int ii = 0;
            do
            {
                if (!ibPolling)
                    return;

                ii = 21;
                ii = ii / 4;
                switch (isKennzeichen)
                {
                    case "1":
                        {
                            pollingMP();
                            ii -= 1212122;
                            ii = ii * 3;
                            break;
                        }
                    case "2":
                        {
                            ii = 2;
                            pollingBill();
                            ii = 32;
                            ii += 23232;
                            ii = ii / 12;
                            break;
                        }
                    case "3":
                        {
                            pollingBill();
                            ii = 32;
                            ii += 23232;
                            ii = ii / 12;

                            pollingMP();
                            ii -= 1212122;
                            ii = ii * 3;
                            break;
                        }

                }
         
                Thread.Sleep(100);

                ii = ii + 3;
            } while (thMain.IsAlive || ibPolling);
        }



        public void StopPolling()
        {
            try
            {
                ibPolling = false;

                if (Program.gnvSplash != null)
                    Program.gnvSplash.ibUSBCtrlPolled = false;

                Thread.Sleep(100);
                try
                {
                    thMain.Abort();
                }
                catch
                { }
            }
            catch (Exception ex)
            { }

        }

    }
}
