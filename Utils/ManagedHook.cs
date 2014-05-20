using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;

namespace kioskplus
{
    public class ManagedHook
    {

        [DllImport("user32", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, int hMod, int dwThreadId);
        [DllImport("user32", EntryPoint = "UnhookWindowsHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int UnhookWindowsHookEx(int hHook);
        public delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        public const int WH_KEYBOARD_LL = 13;

    
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public static int intLLKey;


        public void start()
        {
            intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 0);
Console.WriteLine("Hook:" + intLLKey.ToString());
        }

        public void stop()
        {
            UnhookWindowsHookEx(intLLKey);
        }

        public int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            bool blnEat = false;
 
            switch (wParam)
            {
                case 256:
                case 257:
                case 260:
                case 261:
                    //Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key,
                    blnEat = ((lParam.vkCode == 9) && (lParam.flags == 32)) | ((lParam.vkCode == 27) && (lParam.flags == 32)) | ((lParam.vkCode == 27) && (lParam.flags == 0)) | ((lParam.vkCode == 91) && (lParam.flags == 1)) | ((lParam.vkCode == 92) && (lParam.flags == 1)) | ((lParam.vkCode == 73) && (lParam.flags == 0));
                    break;

            }
 
            if (blnEat == true)
            {
                return 1;
            }
            else
            {
                return CallNextHookEx(0, nCode, wParam, ref lParam);
            }
        }

        /***
         * private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public void Start()
        {
            _hookID = SetHook(_proc);

        }

        public void Stop()
        {
            UnhookWindowsHookEx(_hookID);

        }
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {

            using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if ((Keys)vkCode == System.Windows.Forms.Keys.LWin)
                {
                    return (System.IntPtr)0;
                }
                if ((Keys)vkCode == System.Windows.Forms.Keys.RWin)
                {
                    return (System.IntPtr)1;
                }
                if ((Keys)vkCode == System.Windows.Forms.Keys.Alt)
                {
                    return (System.IntPtr)1;
                }
                if ((Keys)vkCode == System.Windows.Forms.Keys.ControlKey)
                {
                    return (System.IntPtr)1;
                }
                if ((Keys)vkCode == System.Windows.Forms.Keys.Control)
                {
                    return (System.IntPtr)1;
                }
                if ((Keys)vkCode == System.Windows.Forms.Keys.Escape)
                {
                    return (System.IntPtr)1;
                }
                if ((Keys)vkCode == System.Windows.Forms.Keys.Tab)
                {
                    return (System.IntPtr)1;
                }

            }
            else
            {
                if ((wParam == (System.IntPtr)260) || (wParam == (System.IntPtr)261))
                {
                    return (System.IntPtr)1;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
         * **/
    }
}
