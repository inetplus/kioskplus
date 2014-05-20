using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using inetmpk;

namespace kioskplus
{
    class MPImpuls
    {
        ucmpk MPControl = new ucmpk();
        
        private int iComNr;
      
        public String openMDB(int comnr)
        {
            iComNr = comnr;
            try
            {
                MPControl.of_OpenMDB(comnr, "inet+");
            }
            catch
            { }
            return "";
        }

        public String openEMP()
        {
            return "";
        }

        public String closeEMP()
        {
            return "";
        }
        public String closeMDB()
        {
            try
            {
                int lRC = 1;
                MPControl.of_CloseEMP("inet+");
            }
            catch
            { }
            return "";// "Error";
        }

        public String getEMPInfo()
        {
            return "";
        }
        public String setEMPCurrent()
        {
            return "";
        }
        public String ResetEMP()
        {
            return "";
        }
        public String ResetMDBDevices()
        {
            return "";
        }

        public String Polling(ref double credit)
        {
            if (MPControl != null)
            {
                double cr = 0;
                int impuls = 0;

                try
                {
                    MPControl.of_Polling(ref cr, "inet+");
                }
                catch
                { }
                impuls = (int)cr;
                switch (impuls)
                {
                    case 2: // 0.5 Euro
                        credit = 0.5;
                        break;
                    case 3: // 1 Euro 
                        credit = 1;
                        break;
                    case 4: // 2 Euro
                        credit = 2;
                        break;
                }
            }

            return "";
        }

    }
}
