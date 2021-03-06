using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Collections;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using kioskplus.Windows;

namespace kioskplus
{

    /*
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    internal struct inetPrinterDefaults
    {
        [MarshalAs(UnmanagedType.SysInt)]
        private IntPtr dDataType;
        [MarshalAs(UnmanagedType.SysInt)]
        private IntPtr dDeviceMode;
        [MarshalAs(UnmanagedType.U4)]
        public int DesiredAccess;

        internal inetPrinterDefaults(bool AllAccess)
        {
            const int PRINTER_ACCESS_ADMINISTER = 0x4;
            const int PRINTER_ACCESS_USE = 0x8;
            const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            const int PRINTER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE);
            this.dDataType = IntPtr.Zero;
            this.dDeviceMode = IntPtr.Zero;
            if (AllAccess)
            {
                new PrintingPermission(PermissionState.Unrestricted).Demand();
                this.DesiredAccess = (int)PRINTER_ALL_ACCESS;
            }
            else this.DesiredAccess = (int)PRINTER_ACCESS_USE;
        }
    }

    // Enums
    public enum Page_Orientation : short
    {
        PORTRAIT = 1,
        LANDSCAPE = 2
    }

    public enum Page_Duplex : short
    {
        SIMPLEX = 1,
        VERTICAL = 2,
        HORIZONTAL = 3
    }
    public enum Printer_Collate : short
    {
        FALSE = 0,
        TRUE = 1
    }
    public enum Printer_ICMIntend : short
    {
        SATURATE = 1,
        CONTRAST = 2,
        COLORMETRIC = 3,
        USER = 0x100
    }
    public enum Printer_ICMMethod : short
    {
        NONE = 1,
        SYSTEM = 2,
        DRIVER = 3,
        DEVICE = 4,
        USER = 0x100
    }
    public enum Printer_Dither : short
    {
        None = 0,
        NONE = 1,
        COARSE = 2,
        FINE = 3,
        LINEART = 4,
        GRAYSCALE = 5,
        USER = 0x100
    }
    public enum Printer_Media : short
    {
        STANDARD = 1,
        TRANSPARENCY = 2,
        GLOSSY = 3,
        USER = 0x100
    }
    public enum Paper_Source : short
    {
        FIRST = UPPER,
        UPPER = 1,
        ONLYONE = 1,
        LOWER = 2,
        MIDDLE = 3,
        MANUAL = 4,
        ENVELOPE = 5,
        ENVMANUAL = 6,
        AUTO = 7,
        TRACTOR = 8,
        SMALLFMT = 9,
        LARGEFMT = 10,
        LARGECAPACITY = 11,
        CASSETTE = 14,
        FORMSOURCE = 15,
        LAST = FORMSOURCE,
        user = 256
    }

    public enum Print_Color : short
    {
        // color enable/disable for color printers 
        MONOCHROME = 1,
        COLOR = 2,
    }
    public enum Print_TrueType : short
    {
        BITMAP = 1,
        DOWNLOAD = 2,
        SUBDEV = 3,
        DOWNLOAD_OUTLINE = 4,
    }


    public enum PrinterControl : int
    {
        Nul = 0,
        Pause = 1,
        Resume = 2,
        Purge = 3,
        SetStatus = 4
    }

    [Flags]
    public enum Printer_Change : uint
    {
        ADD_PRINTER = 0x00000001,
        SET_PRINTER = 0x00000002,
        DELETE_PRINTER = 0x00000004,
        FAILED_CONNECTION_PRINTER = 0x00000008,
        PRINTER = 0x000000FF,
        ADD_JOB = 0x00000100,
        SET_JOB = 0x00000200,
        DELETE_JOB = 0x00000400,
        WRITE_JOB = 0x00000800,
        JOB = 0x0000FF00,
        ADD_FORM = 0x00010000,
        SET_FORM = 0x00020000,
        DELETE_FORM = 0x00040000,
        FORM = 0x00070000,
        ADD_PORT = 0x00100000,
        CONFIGURE_PORT = 0x00200000,
        DELETE_PORT = 0x00400000,
        PORT = 0x00700000,
        ADD_PRINT_PROCESSOR = 0x01000000,
        DELETE_PRINT_PROCESSOR = 0x04000000,
        PRINT_PROCESSOR = 0x07000000,
        ADD_PRINTER_DRIVER = 0x10000000,
        SET_PRINTER_DRIVER = 0x20000000,
        DELETE_PRINTER_DRIVER = 0x40000000,
        PRINTER_DRIVER = 0x70000000,
        TIMEOUT = 0x80000000,
        ALL = 0x7777FFFF
    }
    public enum Printer_Notification_Types : short
    {
        PRINTER_NOTIFY_TYPE = 0,
        JOB_NOTIFY_TYPE = 1
    }
    public enum Printer_Notify_Field_Indexes
    {
        SERVER_NAME = 0,
        PRINTER_NAME = 1,
        SHARE_NAME = 2,
        PORT_NAME = 3,
        DRIVER_NAME = 4,
        COMMENT = 5,
        LOCATION = 6,
        DEVMODE = 7,
        SEPFILE = 8,
        PRINT_PROCESSOR = 9,
        PARAMETERS = 10,
        DATATYPE = 11,
        SECURITY_DESCRIPTOR = 12,
        ATTRIBUTES = 13,
        PRIORITY = 14,
        DEFAULT_PRIORITY = 15,
        START_TIME = 16,
        UNTIL_TIME = 17,
        STATUS = 18,
        STATUS_STRING = 19,
        CJOBS = 20,
        AVERAGE_PPM = 21,
        TOTAL_PAGES = 22,
        PAGES_PRINTED = 23,
        TOTAL_BYTES = 24,
        BYTES_PRINTED = 25,
        OBJECT_GUID = 26
    }

    public enum Job_Notify_Field_Indexes
    {
        PRINTER_NAME = 0,
        MACHINE_NAME = 1,
        PORT_NAME = 2,
        USER_NAME = 3,
        NOTIFY_NAME = 4,
        DATATYPE = 5,
        PRINT_PROCESSOR = 6,
        PARAMETERS = 7,
        DRIVER_NAME = 8,
        DEVMODE = 9,
        STATUS = 10,
        STATUS_STRING = 11,
        SECURITY_DESCRIPTOR = 12,
        DOCUMENT = 13,
        PRIORITY = 14,
        POSITION = 15,
        SUBMITTED = 16,
        START_TIME = 17,
        UNTIL_TIME = 18,
        TIME = 19,
        TOTAL_PAGES = 20,
        PAGES_PRINTED = 21,
        TOTAL_BYTES = 22,
        BYTES_PRINTED = 23//,
        //ERROR = 999
    }
    [Flags]
    public enum Job_Status
    {
        BLOCKED_DEVICEQUEUE = 0x200,
        DELETED = 0x100,
        DELETING = 4,
        ERROR = 2,
        OFFLINE = 0x20,
        PAPEROUT = 0x40,
        PAUSED = 1,
        PRINTED = 0x80,
        PRINTING = 0x10,
        RESTART = 0x800,
        SPOOLING = 8,
        INTERVENTION = 0x400
    }

    [FlagsAttribute]
    enum PrinterEnumFlags
    {
        PRINTER_ENUM_DEFAULT = 0x00000001,
        PRINTER_ENUM_LOCAL = 0x00000002,
        PRINTER_ENUM_CONNECTIONS = 0x00000004,
        PRINTER_ENUM_FAVORITE = 0x00000004,
        PRINTER_ENUM_NAME = 0x00000008,
        PRINTER_ENUM_REMOTE = 0x00000010,
        PRINTER_ENUM_SHARED = 0x00000020,
        PRINTER_ENUM_NETWORK = 0x00000040,
        PRINTER_ENUM_EXPAND = 0x00004000,
        PRINTER_ENUM_CONTAINER = 0x00008000,
        PRINTER_ENUM_ICONMASK = 0x00ff0000,
        PRINTER_ENUM_ICON1 = 0x00010000,
        PRINTER_ENUM_ICON2 = 0x00020000,
        PRINTER_ENUM_ICON3 = 0x00040000,
        PRINTER_ENUM_ICON4 = 0x00080000,
        PRINTER_ENUM_ICON5 = 0x00100000,
        PRINTER_ENUM_ICON6 = 0x00200000,
        PRINTER_ENUM_ICON7 = 0x00400000,
        PRINTER_ENUM_ICON8 = 0x00800000,
        PRINTER_ENUM_HIDE = 0x01000000
    }



    // Ende Enums

    // Anfang Options
    [StructLayout(LayoutKind.Sequential)]
    sealed class inetPrinterNotifyOptions
    {
        public Int32 dwVersion;
        public Int32 dwFlags;
        public Int32 Count;
        public IntPtr lpTypes;

        internal inetPrinterNotifyOptions(bool refresh)
        {
            dwVersion = 2;
            if (refresh)
                dwFlags = 1;
            else
                dwFlags = 0;
            Count = 2;
            inetPrinterNotifyOptionsType type1 = new inetPrinterNotifyOptionsType();
            int num1 = Marshal.SizeOf(type1);
            this.lpTypes = Marshal.AllocHGlobal(num1);
            Marshal.StructureToPtr(type1, this.lpTypes, true);
        }


        ~inetPrinterNotifyOptions()
        {
            if (lpTypes != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(lpTypes);
            }
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    sealed class inetPrinterNotifyOptionsType
    {
        public short wPrinterType;
        public short wPrinterReserved0;
        public int dwPrinterReserved1;
        public int dwPrinterReserved2;
        public int PrinterFieldCount;
        public IntPtr pPrinterFields;
        public short wJobType;
        public short wJobReserved0;
        public int dwJobReserved1;
        public int dwJobReserved2;
        public int JobFieldCount;
        public IntPtr pJobFields;

        public inetPrinterNotifyOptionsType()
        {
            this.wPrinterType = 0;
            this.PrinterFieldCount = 20;
            this.pPrinterFields = Marshal.AllocCoTaskMem(42);

            //Printer_Notify_Field_Indexes.SERVER_NAME  //not supported
            Marshal.WriteInt16(this.pPrinterFields, 0, (short)Printer_Notify_Field_Indexes.PRINTER_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 2, (short)Printer_Notify_Field_Indexes.SHARE_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 4, (short)Printer_Notify_Field_Indexes.PORT_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 6, (short)Printer_Notify_Field_Indexes.DRIVER_NAME);
            Marshal.WriteInt16(this.pPrinterFields, 8, (short)Printer_Notify_Field_Indexes.COMMENT);
            Marshal.WriteInt16(this.pPrinterFields, 10, (short)Printer_Notify_Field_Indexes.LOCATION);
            Marshal.WriteInt16(this.pPrinterFields, 12, (short)Printer_Notify_Field_Indexes.DEVMODE);
            Marshal.WriteInt16(this.pPrinterFields, 14, (short)Printer_Notify_Field_Indexes.SEPFILE);
            Marshal.WriteInt16(this.pPrinterFields, 16, (short)Printer_Notify_Field_Indexes.PRINT_PROCESSOR);
            Marshal.WriteInt16(this.pPrinterFields, 18, (short)Printer_Notify_Field_Indexes.PARAMETERS);
            Marshal.WriteInt16(this.pPrinterFields, 20, (short)Printer_Notify_Field_Indexes.DATATYPE);
            Marshal.WriteInt16(this.pPrinterFields, 22, (short)Printer_Notify_Field_Indexes.SECURITY_DESCRIPTOR);
            Marshal.WriteInt16(this.pPrinterFields, 24, (short)Printer_Notify_Field_Indexes.ATTRIBUTES);
            Marshal.WriteInt16(this.pPrinterFields, 26, (short)Printer_Notify_Field_Indexes.PRIORITY);
            Marshal.WriteInt16(this.pPrinterFields, 28, (short)Printer_Notify_Field_Indexes.DEFAULT_PRIORITY);
            Marshal.WriteInt16(this.pPrinterFields, 30, (short)Printer_Notify_Field_Indexes.START_TIME);
            Marshal.WriteInt16(this.pPrinterFields, 32, (short)Printer_Notify_Field_Indexes.UNTIL_TIME);
            Marshal.WriteInt16(this.pPrinterFields, 34, (short)Printer_Notify_Field_Indexes.STATUS);
            //Printer_Notify_Field_Indexes.STATUS_STRING //not supported
            Marshal.WriteInt16(this.pPrinterFields, 36, (short)Printer_Notify_Field_Indexes.CJOBS);
            //Marshal.WriteInt16(this.pPrinterFields, 38, (short)Printer_Notify_Field_Indexes.AVERAGE_PPM);
            //Printer_Notify_Field_Indexes.TOTAL_PAGES //not supported
            //Printer_Notify_Field_Indexes.PAGES_PRINTED //not supported
            //Printer_Notify_Field_Indexes.TOTAL_BYTES //not supported
            Marshal.WriteInt16(this.pPrinterFields, 38, (short)Printer_Notify_Field_Indexes.BYTES_PRINTED);//not supported
            Marshal.WriteInt16(this.pPrinterFields, 40, (short)Printer_Notify_Field_Indexes.OBJECT_GUID);


            this.wJobType = 1;
            this.JobFieldCount = 22;
            this.pJobFields = Marshal.AllocCoTaskMem(46);
            Marshal.WriteInt16(this.pJobFields, 0, (short)Job_Notify_Field_Indexes.PRINTER_NAME);
            Marshal.WriteInt16(this.pJobFields, 2, (short)Job_Notify_Field_Indexes.MACHINE_NAME);
            Marshal.WriteInt16(this.pJobFields, 4, (short)Job_Notify_Field_Indexes.PORT_NAME);
            Marshal.WriteInt16(this.pJobFields, 6, (short)Job_Notify_Field_Indexes.USER_NAME);
            Marshal.WriteInt16(this.pJobFields, 8, (short)Job_Notify_Field_Indexes.NOTIFY_NAME);
            Marshal.WriteInt16(this.pJobFields, 10, (short)Job_Notify_Field_Indexes.DATATYPE);
            Marshal.WriteInt16(this.pJobFields, 12, (short)Job_Notify_Field_Indexes.PRINT_PROCESSOR);
            Marshal.WriteInt16(this.pJobFields, 14, (short)Job_Notify_Field_Indexes.PARAMETERS);
            Marshal.WriteInt16(this.pJobFields, 16, (short)Job_Notify_Field_Indexes.DRIVER_NAME);
            Marshal.WriteInt16(this.pJobFields, 18, (short)Job_Notify_Field_Indexes.DEVMODE);
            Marshal.WriteInt16(this.pJobFields, 20, (short)Job_Notify_Field_Indexes.STATUS);
            Marshal.WriteInt16(this.pJobFields, 22, (short)Job_Notify_Field_Indexes.STATUS_STRING);
            //Job_Notify_Field_Indexes.SECURITY_DESCRIPTOR) //Not supported
            Marshal.WriteInt16(this.pJobFields, 24, (short)Job_Notify_Field_Indexes.DOCUMENT);
            Marshal.WriteInt16(this.pJobFields, 26, (short)Job_Notify_Field_Indexes.PRIORITY);
            Marshal.WriteInt16(this.pJobFields, 28, (short)Job_Notify_Field_Indexes.POSITION);
            Marshal.WriteInt16(this.pJobFields, 30, (short)Job_Notify_Field_Indexes.SUBMITTED);
            Marshal.WriteInt16(this.pJobFields, 32, (short)Job_Notify_Field_Indexes.START_TIME);
            Marshal.WriteInt16(this.pJobFields, 34, (short)Job_Notify_Field_Indexes.UNTIL_TIME);
            Marshal.WriteInt16(this.pJobFields, 36, (short)Job_Notify_Field_Indexes.TIME);
            Marshal.WriteInt16(this.pJobFields, 38, (short)Job_Notify_Field_Indexes.TOTAL_PAGES);
            Marshal.WriteInt16(this.pJobFields, 40, (short)Job_Notify_Field_Indexes.PAGES_PRINTED);
            Marshal.WriteInt16(this.pJobFields, 42, (short)Job_Notify_Field_Indexes.TOTAL_BYTES);
            Marshal.WriteInt16(this.pJobFields, 44, (short)Job_Notify_Field_Indexes.BYTES_PRINTED);
        }
        ~inetPrinterNotifyOptionsType()
        {
            Marshal.FreeCoTaskMem(this.pJobFields);
            Marshal.FreeCoTaskMem(this.pPrinterFields);
        }
    }

    */


    // Ende Options



    public class inetDrucker
    {
        //indrknt
        [DllImport("indrknt.dll", EntryPoint = "DeleteJob",CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void DeleteJob(string AControll, int ljobid);

        [DllImport("indrknt.dll", EntryPoint = "GetJobs",CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void GetJobs(ref int id, ref int pages);

        [DllImport("indrknt.dll", EntryPoint = "PrintJob", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void PrintJob(string AControll, int ljobid);

        [DllImport("indrknt.dll", EntryPoint = "OpenPrintJob",CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void OpenPrintJob(string AControll);

        [DllImport("indrknt.dll", EntryPoint = "ClosePrintJob",CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void ClosePrintJob(string AControll);

        private Timer mTimer;
        private TimerCallback mTimerCallBack;
        private Boolean bDrin = false;
        private int jobID, jobPages;
        private int aktJobID = 0;
        private Boolean ibOpen = false;
        dlgDrucken drucker = null;

        public void startPrintMonitor()
        {
            try
            {
                OpenPrintJob("");
                mTimerCallBack = new TimerCallback(checkPrintJob);
                mTimer = new Timer(mTimerCallBack, null, 1000, 1000);
            }
            catch
            { }
          
        }

        private void checkPrintJob(object state)
        {
           // Console.WriteLine("checkPrintJob:" + ibOpen.ToString() + "/" + bDrin.ToString());
            if (bDrin || ibOpen)
                return;

            bDrin = true;

            GetAktuelleJob(ref jobID, ref jobPages);
            //Console.WriteLine("checkPrintJob-II: " +jobID.ToString() + " /" + jobPages.ToString());
            if (jobPages != 0 && jobID != 0 && jobID != null && jobPages != null)
            {
                ibOpen = true;
               //  System.Windows.Forms.MessageBox.Show("drin");
                drucker = new dlgDrucken(this);
                drucker.ShowDialog();
                drucker = null;
                ibOpen = false;
            }

            bDrin = false;
        }

        public void stopPrintMonitor()
        {
            try
            {

                try
                {
                    if (mTimer != null)
                        mTimer.Dispose();

                    mTimerCallBack = null;
                    mTimer = null;
                }
                catch (Exception ex)
                {
                
                }
                while (jobID != null && jobID != 0)
                {
                    deleteAktJob();
                    
                    GetAktuelleJob(ref jobID, ref jobPages);
                }


                try
                {
                    if (drucker != null)
                    {
                        drucker.Close();
                        drucker = null;
                    }
                }
                catch { }
            }
            catch { }

            try
            {
                ClosePrintJob("");
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobid"></param>
        /// <param name="jpages"></param>
        public void GetAktuelleJob(ref int jobid, ref int jpages)
        {
            GetJobs(ref jobID, ref jobPages);
            jobid = jobID;
            jpages = jobPages;
        }

        /// <summary>
        /// 
        /// </summary>
        public void deleteAktJob()
        {
            DeleteJob("", jobID);
            jobID = 0;
            jobPages = 0;
        }

        public void fortsetzenAktJob()
        {
            PrintJob("", jobID);
            jobID = 0;
            jobPages = 0;
        }
        /*
        // Anfang DLL
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenPrinter(string PrinterName, out IntPtr PrinterHandle, ref inetPrinterDefaults PrinterDefaults);

        [DllImport("winspool.drv", EntryPoint = "OpenPrinterW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenPrinter(string PrinterName, out IntPtr PrinterHandle, int PrinterDefaults);

        [DllImport("winspool.Drv", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "GetDefaultPrinterW", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetDefaultPrinterW(string PrinterName, out int dwNeeded);

        [DllImport("winspool.drv", EntryPoint = "SetDefaultPrinter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetDefaultprinter(string Printername);

        [DllImport("winspool.drv", EntryPoint = "GetPrinterW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetPrinter(IntPtr hPrinter, int Level, IntPtr pPrinter, int cbBuf, out int dwNeeded);

        [DllImport("winspool.drv", EntryPoint = "SetPrinterW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetPrinter(IntPtr hPrinter, int Level, IntPtr pPrinter, [In, MarshalAs(UnmanagedType.U4)] PrinterControl Command);

        [DllImport("winspool.drv", EntryPoint = "GetPrinterDriverW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetPrinterDriver(IntPtr PrinterHandle, IntPtr pEnvironment, int Level, IntPtr pPrinter, int cbBuf, out int dwNeeded);

        [DllImport("winspool.drv", EntryPoint = "EnumPrinterDriversW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumPrinterDrivers([In] string ServerName, [In] string Environmentname, [In] int Level, [Out] IntPtr pdrivers, [In] int cbBuf, out int pcbNeeded, out int pcbReturned);

        [DllImport("winspool.drv", EntryPoint = "DeviceCapabilitiesW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int DeviceCapabilitiesW(string sDevice, string Port, int fwCapability, IntPtr Output, IntPtr device);

        [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr FindFirstPrinterChangeNotification(uint hPrinter, uint fdwFlags, System.UInt32 fdwOptions, [MarshalAs(UnmanagedType.LPStruct)] inetPrinterNotifyOptions pPrinterNotifyOptions);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 FindNextPrinterChangeNotification(uint hChange, out uint pdwChange, IntPtr PrinterNotifyOptions, ref IntPtr pPrinterNotifyInfo);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 FindNextPrinterChangeNotification(uint hChange, out uint pdwChange, inetPrinterNotifyOptions PrinterNotifyOptions, ref IntPtr pPrinterNotifyInfo);

        [DllImport("winspool.drv", EntryPoint = "FindClosePrinterChangeNotification", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool FindClosePrinterChangeNotification(System.IntPtr hChange);

        [DllImport("winspool.drv", EntryPoint = "FreePrinterNotifyInfo", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern System.Int32 FreePrinterNotifyInfo(IntPtr pPrinterNotifyInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumPrinters(PrinterEnumFlags Flags, string Name, uint Level, IntPtr pPrinterEnum, uint cbBuf, ref uint pcbNeeded, ref uint pcReturned);

        private ArrayList mAllPrinter;

        private ManualResetEvent manResetEvent;
        private RegisteredWaitHandle regWaitHandle;


        private void getAllPrinter()
        {
            mAllPrinter = new ArrayList();

            foreach (String sPrinter in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                mAllPrinter.Add(sPrinter);
            }
        }

        private void printJob(String asName)
        {
            IntPtr  hPrinter;
            IntPtr hChangeNot;

            inetPrinterDefaults pDefault = new inetPrinterDefaults(true);
            inetPrinterNotifyOptions pNotify = new inetPrinterNotifyOptions(true);

            if (OpenPrinter(asName, out hPrinter, ref pDefault))
            {
                //.FindFirstPrinterChangeNotification(SafePrinterHandle, (uint)Printer_Change.ALL, 0, null);
                hChangeNot = FindFirstPrinterChangeNotification((uint)hPrinter, (uint)Printer_Change.ALL, 0, null);

                if (hChangeNot != IntPtr.Zero)
                {
                    manResetEvent = new ManualResetEvent(false);
                    manResetEvent.SafeWaitHandle = new SafeWaitHandle(hChangeNot, true);
                   // regWaitHandle = ThreadPool.RegisterWaitForSingleObject(manResetEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), null, -1, true);


                }

            }

        }

          private void PrinterNotifyWaitCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                //Should not happen 
                shithappened = string.Empty;
                return;
            }
            else
            {
                try
                {
                    if (!this.IsInvalid)
                    {
                        IntPtr pni = new IntPtr();
                        uint uuu = 0;
                        //IntPtr pnj = new IntPtr();
                        if (qStatic.FindNextPrinterChangeNotification(this, out uuu, IntPtr.Zero, ref pni) != 0)
                        {
                            if (uuu != 0)
                            {
                                shithappened = "Server Changed : ";
                                if (GetServerChange(uuu, Printer_Change.ADD_FORM))
                                    shithappened += "Form Added ";
                                if (GetServerChange(uuu, Printer_Change.ADD_JOB))
                                {
                                    shithappened += "Job Added ";
                                    
                                }
                                if (GetServerChange(uuu, Printer_Change.ADD_PORT))
                                    shithappened += "Port Added ";
                                if (GetServerChange(uuu, Printer_Change.ADD_PRINT_PROCESSOR))
                                    shithappened += "Processor Added ";
                                if (GetServerChange(uuu, Printer_Change.ADD_PRINTER))
                                    shithappened += "Printer Added ";
                                if (GetServerChange(uuu, Printer_Change.ADD_PRINTER_DRIVER))
                                    shithappened += "Driver Added ";
                                if (GetServerChange(uuu, Printer_Change.CONFIGURE_PORT))
                                    shithappened += "Port Configured ";
                                if (GetServerChange(uuu, Printer_Change.DELETE_FORM))
                                    shithappened += "Form Deleted ";
                                if (GetServerChange(uuu, Printer_Change.DELETE_JOB))
                                    shithappened += "Job Deleted ";
                                if (GetServerChange(uuu, Printer_Change.DELETE_PORT))
                                    shithappened += "Port Deleted ";
                                if (GetServerChange(uuu, Printer_Change.DELETE_PRINT_PROCESSOR))
                                    shithappened += "Processor Deleted ";
                                if (GetServerChange(uuu, Printer_Change.DELETE_PRINTER))
                                    shithappened += "Printer Deleted ";
                                if (GetServerChange(uuu, Printer_Change.DELETE_PRINTER_DRIVER))
                                    shithappened += "Driver Deleted ";
                                if (GetServerChange(uuu, Printer_Change.FAILED_CONNECTION_PRINTER))
                                    shithappened += "Printer Failed Connection ";
                                if (GetServerChange(uuu, Printer_Change.SET_FORM))
                                    shithappened += "Form Added ";
                                if (GetServerChange(uuu, Printer_Change.SET_JOB))
                                    shithappened += "Job Set ";
                                if (GetServerChange(uuu, Printer_Change.SET_PRINTER))
                                    shithappened += "Printer Set ";
                                if (GetServerChange(uuu, Printer_Change.SET_PRINTER_DRIVER))
                                    shithappened += "Driver Set ";
                                if (GetServerChange(uuu, Printer_Change.TIMEOUT))
                                    shithappened += "Temeout ";
                                if (GetServerChange(uuu, Printer_Change.WRITE_JOB))
                                    shithappened += "Job Written ";
                                qSomethingHappened(0, shithappened);
                                System.Diagnostics.Debug.WriteLine("Server Changed : " + shithappened);
                            }
                            if (pni != IntPtr.Zero)
                            {
                            restart:
                                int count = Marshal.ReadInt32(pni, (IntPtrSize * 2));
                                int jobid;
                                int nnn;
                                for (int ii = 0; ii < count; ii++)
                                {
                                    shithappened = "";
                                    nnn = 0;
                                    int j = (IntPtrSize * 5) * ii;
                                    short type = Marshal.ReadInt16(pni, (j + (IntPtrSize * 3)));
                                    short field = Marshal.ReadInt16(pni, (j + (IntPtrSize * 3) + 2));
                                    if (type == 0) // Printer Notification
                                    {
                                        switch (field)
                                        {
                                            case (short)Printer_Notify_Field_Indexes.PRINTER_NAME:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.SHARE_NAME:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.PORT_NAME:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.DRIVER_NAME:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.COMMENT:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.LOCATION:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.DEVMODE:
                                                DevMode d = new DevMode(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                //new IntPtr(pbuf));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.SEPFILE:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.PRINT_PROCESSOR:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.PARAMETERS:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.DATATYPE:
                                                shithappened = Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.ATTRIBUTES:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.PRIORITY:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.DEFAULT_PRIORITY:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.START_TIME:
                                                shithappened = "Start time";
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                nnn += qStatic.hourdiv;
                                                TimeSpan ts = new TimeSpan(nnn / 60, nnn % 60, 0);
                                                shithappened = ts.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.UNTIL_TIME:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                nnn += qStatic.hourdiv;
                                                TimeSpan tss = new TimeSpan(nnn / 60, nnn % 60, 0);
                                                shithappened = nnn.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.STATUS:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.CJOBS:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                break;
                                            case (short)Printer_Notify_Field_Indexes.AVERAGE_PPM:
                                                shithappened = "Average PPM";
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened = nnn.ToString();
                                                break;
                                            default:
                                                shithappened = "";
                                                nnn = 0;
                                                break;
                                        }
                                        Printer_Notify_Field_Indexes fi = (Printer_Notify_Field_Indexes)field;
                                        qSomethingHappened(0, fi.ToString() + " " + shithappened);
                                        System.Diagnostics.Debug.WriteLine("Printer : " + fi.ToString() + " " + shithappened);

                                    }
                                    if (type == 1)  //Job Notifiction
                                    {
                                        jobid = Marshal.ReadInt32(pni, (j + (IntPtrSize * 5)));
                                        Job_Notify_Field_Indexes fi = (Job_Notify_Field_Indexes)field;
                                        shithappened = fi.ToString() + " ";
                                        switch (field)
                                        {
                                            case (short)Job_Notify_Field_Indexes.PRINTER_NAME:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.MACHINE_NAME:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PORT_NAME:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.USER_NAME:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.NOTIFY_NAME:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.DATATYPE:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PRINT_PROCESSOR:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PARAMETERS:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.DRIVER_NAME:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.DEVMODE:
                                                DevMode d = new DevMode(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                shithappened += "Changed";
                                                break;
                                            case (short)Job_Notify_Field_Indexes.STATUS:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                Job_Status stat = (Job_Status)nnn;
                                                shithappened += stat.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.STATUS_STRING:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.DOCUMENT:
                                                shithappened += Marshal.PtrToStringUni(Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7))));
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PRIORITY:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.POSITION:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.SUBMITTED:
                                                //systemtime in pdev
                                                IntPtr ptr1 = Marshal.ReadIntPtr(pni, (j + (IntPtrSize * 7)));
                                                SYSTEMTIME st = new SYSTEMTIME();
                                                Marshal.PtrToStructure(ptr1, st);
                                                st.wHour += (short)(qStatic.hourdiv / 60);
                                                shithappened += st.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.START_TIME:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.UNTIL_TIME:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.TIME:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.TOTAL_PAGES:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.PAGES_PRINTED:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.TOTAL_BYTES:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            case (short)Job_Notify_Field_Indexes.BYTES_PRINTED:
                                                nnn = Marshal.ReadInt32(pni, (j + (IntPtrSize * 6)));
                                                shithappened += nnn.ToString();
                                                break;
                                            default:
                                                shithappened += "Error";
                                                nnn = 0;
                                                break;

                                        }
                                        qSomethingHappened(jobid, shithappened);
                                        System.Diagnostics.Debug.WriteLine("Job : " + jobid.ToString() + " " + shithappened);
                                    }
                                }
                                int fla = Marshal.ReadInt32(pni, IntPtrSize);
                                if (Convert.ToBoolean(fla & 0x00000001))
                                {
                                    // some notifications had to be discarded
                                    qStatic.FindNextPrinterChangeNotification(this, out uuu, new qPrinterNotifyOptions(true), ref   pni);
                                    if (Marshal.ReadInt32(pni, (IntPtrSize * 2)) > 0)
                                        goto restart;
                                }
                                qStatic.FreePrinterNotifyInfo(pni);
                            }
                        }
                    }

                    _RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject((ManualResetEvent)_ManualResetEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), null, -1, true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message); //needed for design time errors
                }
                return;
        }
         * */
    
    }
}
