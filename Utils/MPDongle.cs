using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace kioskplus
{
    class MPDongle
    {
        
         private struct EMPinfo
        {
            public int country; //         As Long               ' country code
            public int decimals;//        As Long               ' decimals
            public int tubes;//           As Long               ' number of slots
            public int cashbox;//         As Long               ' number of the cashbox

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
            public char[] device;//  = new char[12]; // As Byte               ' device name

           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
           public char[] model;// = new char[12];   //    As Byte               ' model

           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
           public char[] version;// = new char[1];//      As Byte               ' software version

           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
           public char[] features;// = new char[3]; //     As Byte               ' feature flags

           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
           public double[] coin_value;// = new double[15];//  As Single             ' coin values

           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
           public int[] tube;// = new long[15];//        As Long               ' coin routing

        };

        // coin routing and locking
        private struct EMPcurrent
        {
            public int cashbox; //         As Long               ' cashbox
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] nlock;// = new long[15];//       As Long               ' unlock coin
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] incbox;// = new long[15]; //      As Long               ' route coin to cashbox
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] tube;// = new long[15];//        As Long               ' coin slot  -byte
        };


        // last poll status
        private struct EMPstatus
        {
            public int status; //          As Long               ' status, see bitmasks for poll response
            public int coin; //            As Long               ' coin number
            public int slot;//            As Long               ' coin slot
            public int reject; //          As Long               ' rejection code
            public int extended;//        As Long               ' extended information
            public int mdberr;//          As Long               ' MDB error code, see above
        };

        private const short MDB_OK = 0;                       // O.K.
        private const short MDB_BADCOMNO = -1;                // invalid COM port
        private const short MDB_NOINIT = -2;                  // not initialised
        private const short MDB_OPENERROR = -3;               // can//t open COM
        private const short MDB_INITERROR = -4;               // can//t initialise COM
        private const short MDB_BADCSUM = -5;                 // checksum error
        private const short MDB_ANSTO = -6;                   // response timeout
        private const short MDB_CHARTO = -7;                  // character timeout
        private const short MDB_NOACK = -8;                   // no Acknowledge
        private const short MDB_NAKRCVD = -9;                 // NAK received
        private const short MDB_NOTRDY = -10;                 // µController not ready
        private const short MDB_TOOLONG = -11;                // block too long
        private const short MDB_SNDERROR = -12;               // COM send error
        private const short MDB_RCVERROR = -13;               // COM receive error
        private const short MDB_CTRLTOT = -14;                // µController timeout
        private const short MDB_WRONGADR = -15;               // invalid MDB address
        private const short MDB_WRONGMC = -16;                // wrong µController
        private const short MDB_BADID = -20;                  // can//t identify device
        private const short MDB_DLLERR = -21;                 // error loading DLL
        private const short MDB_PROCERR = -22;                // error getting procedure address

        // Error-Codes coin validator
        private const short EMP_OK = 0;
        private const short EMP_NOINIT = -101;                // coin validator not open
        private const short EMP_OPENED = -102;                // coin validator already opened
        private const short EMP_ANSERROR = -103;              // coin validator didn//t respond
        private const short EMP_NULLPTR = -104;               // call with invalid pointer
        private const short EMP_BADLEN = -105;                // coin validator wrong block length in response

        // bitmasks for poll response
        private const int EMPP_COIN = 1;// &H1                  // coin data transmitted
        private const int EMPP_RESET = 2; // &H2                 // was reset
        private const int EMPP_RETURN =4; // &H4                // return lever pushed
        private const int EMPP_REJECT = 8; // &H8                // coin rejected
        private const int EMPP_NOINIT = 16;// &H10               // not initialised
        private const int EMPP_NOANSWER = 32; // &H20             // no response
        private const int EMPP_UNKNOWN = 64; //&H40              // unknown status
        private const int EMPP_TIMEOUT = 128; //&H80              // No-Response time exceeded


        private EMPinfo EMP_info;
        private EMPcurrent EMP_current;
        private EMPstatus EMP_status;

        //public static extern int CommandLineToArgv(string lpCmdLine, short pNumArgs);

        [DllImport("WinMDB32.dll")]
        private static extern int OpenMDB(short com_nr);
        [DllImport("WinMDB32.dll")]
        private static extern int CloseMDB();
        [DllImport("WinMDB32.dll")]
        private static extern int OpenEMP();
        [DllImport("WinMDB32.dll")]
        private static extern int CloseEMP();
        [DllImport("WinMDB32.dll")]
        private static extern int GetEMPInfo(ref EMPinfo EMP_info);
            [DllImport("WinMDB32.dll")]
        private static extern int GetEMPCurrent(ref EMPcurrent EMP_current);
            [DllImport("WinMDB32.dll")]
        private static extern int SetEMPCurrent(ref EMPcurrent EMP_current); 
            [DllImport("WinMDB32.dll")]
        private static extern int PollEMP();
            [DllImport("WinMDB32.dll")]
        private static extern int GetLastEMPPoll(ref EMPstatus EMP_status);
            [DllImport("WinMDB32.dll")]
        private static extern String GetMDBErrorMessage(int rc );
            [DllImport("WinMDB32.dll")]
        private static extern int ResetMDBDevices();
            [DllImport("WinMDB32.dll")]
        private static extern int ResetEMP();

        
        private int iComNr;


        public MPDongle()
        {
        }


        public String openMDB(short comnr)
        {
            int rc;
            iComNr = comnr;
            try
            {
                rc = OpenMDB(comnr);
            }
            catch { return "-1"; }

            return "";
        }

        public String openEMP()
        {
            try{
                int rc;
                rc = OpenEMP();
                if (rc != EMP_OK)
                {
                    return GetMDBErrorMessage(rc);
                }
            }
            catch { return "-1"; }

            return "";
        }

        public String closeEMP()
        {
             try
            {
                int rc;
                rc = CloseEMP();
                if (rc != EMP_OK)
                {
                    return GetMDBErrorMessage(rc);
                }
            }
             catch { return "-1"; }
            
            return "";
        }

        public String closeMDB()
        {
            try
            {
                int rc;
                rc = CloseMDB();
                if (rc != EMP_OK)
                {
                    return GetMDBErrorMessage(rc);
                }
            }
            catch { return "-1"; }

            return "";
        }

        public String getEMPInfo()
        {
            string rctext;
            int rc;

            try
            {
                rc = GetEMPInfo(ref EMP_info);
                if (rc != EMP_OK)
                {
                    return GetMDBErrorMessage(rc);
                }

            }
            catch { return "-1"; }

            try
            {
                rc = GetEMPCurrent(ref EMP_current);
                if (rc != EMP_OK)
                {
                    return GetMDBErrorMessage(rc);
                }

            }
            catch { return "-1"; }

            return "";
        }
        public String setEMPCurrent(string data)
        {
            int rc;
            string rctext;
            int index;
          

            try
            {
                EMP_info.coin_value[0] = 0;
                EMP_info.coin_value[1] = 0.2;
                EMP_info.coin_value[2] = 0.5;
                EMP_info.coin_value[3] = 1;
                EMP_info.coin_value[4] = 2;
                EMP_info.coin_value[5] = 0;
                EMP_info.coin_value[6] = 0;
                EMP_info.coin_value[7] = 0;
                EMP_info.coin_value[8] = 0;
                EMP_info.coin_value[9] = 0;
                EMP_info.coin_value[10] = 0;
                EMP_info.coin_value[11] = 0;
                EMP_info.coin_value[12] = 0;
                EMP_info.coin_value[13] = 0;
                EMP_info.coin_value[14] = 0;
                EMP_info.coin_value[15] = 0;


                for (index = 0; index < 15; index++)
                {
                    EMP_current.nlock[index] = Int32.Parse(data.Substring(index, 1));
                }

                for (index = 0; index < 15; index++)
                {
                    EMP_current.incbox[index] = 0;
                    EMP_current.tube[index] = 0;
                }


                rc = SetEMPCurrent(ref EMP_current);

                if (rc != EMP_OK)
                {
                    return GetMDBErrorMessage(rc);
                }

            }
            catch { return "-1"; }


            return "";
        }
        public String ResetEMPFunc()
        {
            try
            {
                int rc;

                rc = ResetEMP();

                if (rc != EMP_OK)
                    return GetMDBErrorMessage(rc);
            } catch { return "-1"; }

            return "";
        }
        public String ResetMDBDevicesFunc()
        {
            try
            {
                int rc;
                rc = ResetMDBDevices();
                if (rc != MDB_OK)
                    return GetMDBErrorMessage(rc);
            }
            catch { return "-1"; }

            return "";
        }

        /*
         ' return : ""       - Erfolgreich
    '        : "REJECT" - Münze wurde abgewiesen
    '        : "TIMEOUT"-Timeout
    '        : "NO"      - No
         
         */
        public String Polling(ref double credit)
        {
            try
            {
                credit = 0;
                int rc;
               
                    rc = PollEMP();
                       
                 if (rc != 0)
                 {
                     GetLastEMPPoll(ref EMP_status);
                      if ((rc != 0) && (EMP_status.status == EMPP_COIN)){
                         // ' Münze wurde akzeptiert
                       // credit = EMP_status.coin;
                          if ( EMP_status.coin > -1)
                              credit = EMP_info.coin_value[EMP_status.coin];
                          
                        return "";
                      }

                     //' Münze wurde nicht akzeptiert
                     if ((rc != 0) && (EMP_status.status == EMPP_REJECT))
                     {
                          credit = EMP_status.coin;
                        return "REJECT";
                     }

                     // ' Timeout.Soll ResetMDBDevices durchgeführt werden.
                     if ((rc != 0) && (EMP_status.status == EMPP_TIMEOUT))
                     {
                         credit = EMP_status.coin;
                         return "TIMEOUT";
                     }
                 }


            }
            catch { return "-1"; }

            return "NO";
        }

    }
}
