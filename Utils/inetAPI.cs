using System;
using System.Collections.Generic;
using System.Text;

namespace kioskplus.Utils
{
    public static class inetAPI
    {
        public static void PCReboot()
        {
            bool ok;

            TokPriv1Luid tp;

            IntPtr hproc = System.Diagnostics.Process.GetCurrentProcess().Handle;
            IntPtr htok = IntPtr.Zero;

            ok = AdvApi.OpenProcessToken(hproc, /*TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY */Win32Kernel.TOKEN_ALL_ACCESS, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = Win32Kernel.SE_PRIVILEGE_ENABLED;
            ok = AdvApi.LookupPrivilegeValue(null, Win32Kernel.SE_SHUTDOWN_NAME, ref tp.Luid);

            ok = AdvApi.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

            Win32User.ExitWindowsEx(Win32User.EWX_REBOOT, 0);

         
        }

        public static void PCShutDown()
        {
            bool ok;

            TokPriv1Luid tp;

            IntPtr hproc = System.Diagnostics.Process.GetCurrentProcess().Handle;
            IntPtr htok = IntPtr.Zero;

            ok = AdvApi.OpenProcessToken(hproc, /*TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY */Win32Kernel.TOKEN_ALL_ACCESS, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = Win32Kernel.SE_PRIVILEGE_ENABLED;
            ok = AdvApi.LookupPrivilegeValue(null, Win32Kernel.SE_SHUTDOWN_NAME, ref tp.Luid);

            ok = AdvApi.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

            Win32User.ExitWindowsEx(Win32User.EWX_SHUTDOWN, 0);
           


            /**
            GetAdjustToken(Win32Kernel.SE_SHUTDOWN_NAME);

            Win32User.ExitWindowsEx(Win32User.EWX_SHUTDOWN, 0);
             * **/
        }

        private static void GetAdjustToken(string argString)
        {
            
            IntPtr tokHandle = IntPtr.Zero;
            int ret;
            TOKEN_PRIVILEGES tkp;
            tkp = new TOKEN_PRIVILEGES();
            tkp.Privileges = new LUID_AND_ATTRIBUTES[1];

            LUID ll;
            //Win32Kernel.SE_SYSTEMTIME_NAME
            /*int i =*/ AdvApi.OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle, Win32Kernel.TOKEN_ALL_ACCESS, ref tokHandle);
            ret = AdvApi.LookupPrivilegeValue(null, argString, ref tkp.Privileges[0].pLuid);
            tkp.Privileges[0].Attributes = Win32Kernel.SE_PRIVILEGE_ENABLED;

            ret = AdvApi.AdjustTokenPrivileges(tokHandle, false, ref tkp, tkp.PrivilegeCount, (IntPtr)0, 0);
            
        }

        public static void SetLocalDateTime(DateTime argDateTime)
        {

            GetAdjustToken(Win32Kernel.SE_SYSTEMTIME_NAME);

            SYSTEMTIME lpTime;
            lpTime.wYear = (short)argDateTime.Year;
            lpTime.wMonth = (short)argDateTime.Month;
            lpTime.wDayOfWeek = (short)argDateTime.DayOfWeek;
            lpTime.wDay = (short)argDateTime.Day;
            lpTime.wHour = (short)argDateTime.Hour;
            lpTime.wMinute = (short)(argDateTime.Minute);
            lpTime.wSecond = (short)argDateTime.Second;
            lpTime.wMilliseconds = (short)argDateTime.Millisecond;

            Win32Kernel.SetLocalTime(ref lpTime);

        }
    }
}
