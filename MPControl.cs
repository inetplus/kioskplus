using System;
using System.Collections.Generic;
using kioskplus.Utils;
using System.Threading;

namespace kioskplus
{
    public class MPControl
    {
        inetRegistry instRegistry;
        MPImpuls invImpuls = null;
        MPDongle invDongle = null;
        Thread instThread = null;
        Boolean ibAbbruch = true;

        public MPControl()
        {
            uint lmpArt;

            instRegistry = new inetRegistry();
            instRegistry.SubKey = inetConstants.icsRegKey + "\\" + inetConstants.icsVerzMP;
            lmpArt = instRegistry.ReadAsDWORD(inetConstants.icsMPArt);
            
            if (lmpArt > 1 && lmpArt < 5) // impuls
            {
                invImpuls = new MPImpuls();
            }
            else if (lmpArt == 1) // Dongle
            {
                invDongle = new MPDongle();
            }
        }

        public void startPolling()
        {
            if (invImpuls != null)
            {
                uint lmpCom;
                lmpCom = instRegistry.ReadAsDWORD(inetConstants.icsMPCom);
                String lsd = invImpuls.openMDB((int)lmpCom);
                if (lsd.Equals(""))
                {
                    instThread = new Thread(new ThreadStart(this.eventTimerImpuls));
                    instThread.ApartmentState = ApartmentState.STA;
                    instThread.Name = "ipplus01";
                    instThread.IsBackground = true;
                    instThread.Start();
                }
            } else if (invDongle != null)
                startDonglePolling();
        }

        public void stopPolling()
        {
            if (invImpuls != null)
            {
                // Timer stoppen
                ibAbbruch = false;
                int i = 21;
                Thread.Sleep(201);
                invImpuls.closeMDB();
            }
            else if (invDongle != null)
                stopDonglePolling(); 

        }

        public void eventTimerImpuls()
        {
            int i = 0;
            double credit = 0;
           
            do
            {
                i = i + 123;
                i = i * 9;
                try
                {
                    invImpuls.Polling(ref credit);
                }
                catch { credit = 0; }
                if (credit > 0)
                {
                    if (Program.gnvSplash != null)
                        Program.gnvSplash.SetWert(credit,"");

                    credit = 0;
                }
                i = i / 6;
                Thread.Sleep(200);
               // Sleep einbauen
            } while (ibAbbruch);

        }


        // Dongle

        public void startDonglePolling()
        {
            if (invDongle != null)
            {
                uint lmpCom;
                lmpCom = instRegistry.ReadAsDWORD(inetConstants.icsMPCom);
                String lsd = invDongle.openMDB((short)lmpCom);

                if (lsd.Equals(""))
                {
                    lsd = invDongle.openEMP();
                    if (lsd.Equals(""))
                    {
                        lsd = invDongle.getEMPInfo();
                        if (lsd.Equals(""))
                        {
                            lsd = invDongle.setEMPCurrent("001110000000000");
                            if (lsd.Equals(""))
                            {
                                instThread = new Thread(new ThreadStart(this.eventTimerDongle));
                                instThread.ApartmentState = ApartmentState.STA;
                                instThread.Name = "ipplus01";
                                instThread.IsBackground = true;
                                instThread.Start();
                            }
                        }
                    }
                }
            }
        }

        public void stopDonglePolling()
        {

            try
            {
                if (invDongle != null)
                {
                    string data = "0000000000000000";
                    // Timer stoppen
                    ibAbbruch = false;
                    int i = 21;
                    Thread.Sleep(201);
                    long rc;
                    if (invDongle.getEMPInfo().Equals(""))
                        invDongle.setEMPCurrent(data);

                    if (invDongle.closeEMP().Equals(""))
                        invDongle.closeMDB();
                }
            }
            catch 
            {
            }

          
        }

        public void eventTimerDongle()
        {
            int i = 0;
            double credit = 0;

            do
            {
                i = i + 123;
                i = i * 9;
                try
                {
                    if (!invDongle.Polling(ref credit).Equals(""))
                        credit = 0;
                }
                catch { credit = 0; }

                if (credit > 0)
                {
                    if (Program.gnvSplash != null)
                        Program.gnvSplash.SetWert(credit, "");

                    credit = 0;
                }
                i = i / 6;
                Thread.Sleep(200);
                // Sleep einbauen
            } while (ibAbbruch);

            //System.Windows.Forms.MessageBox.Show("disarida...");

        }


    }
}
