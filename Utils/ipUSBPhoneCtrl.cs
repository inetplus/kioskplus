using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using kioskplus.Windows;

namespace kioskplus.Utils
{
    public class ipControl
    {
        private int width = 132;
        public int LCDWidth
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        private int height = 64;
        public int LCDHeight
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }


        private string isTextLeftOben = "";
        public string TextToLeftOben
        {
            get {
                return isTextLeftOben;
            }
            set {
                isTextLeftOben = value;
            }
        }
        private string isTextRightOben = "";
        public string TextToRightOben
        {
            get
            {
                return isTextRightOben;
            }
            set
            {
                isTextRightOben = value;
            }
        }

        private string isTextMitte = "";
        public string TextToMitte
        {
            get
            {
                return isTextMitte;
            }
            set
            {
                isTextMitte = value;
            }
        }

        private string isTextLeftUnten = "?";
        public string TextToUntenLeft
        {
            get
            {
                return isTextLeftUnten;
            }
            set
            {
                isTextLeftUnten = value;
            }
        }

        private string isTextRightUnten = "<<";
        public string TextToUntenRight
        {
            get
            {
                return isTextRightUnten;
            }
            set
            {
                isTextRightUnten = value;
            }
        }


        public delegate void checkData(int wMessage, int wParam, int lParam);
        private checkData onCheckData = null;

        public event checkData Checked
        {
            add
            {
                onCheckData += value;
            }
            remove
            {
                onCheckData -= value;
            }
        }

        public const int WM_USER = 0x400;
        public const int WM_HID_DEV_ADDED = WM_USER + 0x1000;
        public const int WM_HID_DEV_REMOVED = WM_USER + 0x1001;
        public const int WM_HID_KEY_DOWN = WM_USER + 0x1002;
        public const int WM_HID_KEY_UP = WM_USER + 0x1003;
        public const int WM_HID_VOLUME_DOWN = WM_USER + 0x1004;
        public const int WM_HID_VOLUME_UP = WM_USER + 0x1005;
        public const int WM_HID_PLAYBACK_MUTE = WM_USER + 0x1006;
        public const int WM_HID_RECORD_MUTE = WM_USER + 0x1007;
        public const int WM_HID_GPIO = WM_USER + 0x1008;
        public const int WM_HID_GENERIC = WM_USER + 0x1009;
        public const int WM_HID_BUFFER_TONE = WM_USER + 0x100A;
        public const int WM_HID_W1388_ISR = WM_USER + 0x100B;
        public const int WM_HID_OTHERCMD_REP = WM_USER + 0x100D;
        public const int WM_HID_VENDORCMD_REP = WM_USER + 0x100E;

        public const int WM_DEVICECHANGE = 0x0219;

        byte[] aMelody = new byte[8] {TONE_FRE(826), TONE_TIME(112),
                                    TONE_FRE(1049), TONE_TIME(112),
                                    TONE_FRE(1322), TONE_TIME(112),
                                    0, TONE_TIME(112)};



        private static byte TONE_FRE(int arg)
        {
            return ((byte)(12000000 / 16 / 32 / arg));
        }

        private static byte TONE_TIME(int arg)
        {
            return ((byte)(arg / 2));
        }

        //        public delegate void HIDCALLBACKPROC(int wMessage, int wParam, int lParam);
       
        [DllImport("WB1308HID.dll")]
        private static extern int StartDeviceDetectionProc(HIDCALLBACKPROC pHIDCallbackProc,// PFHIDCALLBACKPROC pHIDCallbackProc,
                                                            int DeviceAddedMsg, int DeviceRemovedMsg,
                                                            int KeyDownMsg, int KeyUpMsg,
                                                            int VolumeKeyDownMsg, int VolumeKeyUpMsg,
                                                            int PlaybackMuteMsg, int RecordMuteMsg,
                                                            int GPIOMsg,
                                                            int ToneBufferMsg,
                                                            int W1388ISRMsg,
                                                            int OtherCmdRep,
                                                            int VendorCmdRep,
                                                            int wVID, int wPID); // word


        [DllImport("WB1308HID.dll")]
        private static extern void HandleUsbDeviceChange(int wParam, int lParam);

        [DllImport("WB1308HID.dll")]
        private static extern int CloseDevice();

        /// <summary>
        /// set handfree mode on/off
        /// </summary>
        /// <param name="on"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void SetHandfree(int on);

        /// <summary>
        /// set lcd'sdisplay size, default is 132*64
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void LCDSetSize(int cx, int cy);

        /// <summary>
        /// black light on/off
        /// </summary>
        /// <param name="on"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void LCDBklightOn(int on);

        /// <summary>
        /// invert lcd display vertical
        /// </summary>
        /// <param name="on"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void LCDInvert(int on);

        /// <summary>
        /// set lcd contrast 1~8
        /// </summary>
        /// <param name="level"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void LCDContrast(int level);

        /// <summary>
        /// get LCD's HDC, you can draw on it
        /// </summary>
        /// <returns></returns>
        [DllImport("WB1308HID.dll")]
        private static extern IntPtr LCDGetDC();

        /// <summary>
        /// update LCD's HDC's area x-y-cx-cy to LCD
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void LCDUpdateArea(int x, int y, int cx, int cy);

        [DllImport("WB1308HID.dll")]
        private static extern void SLCDCommand(byte cmd);

        [DllImport("WB1308HID.dll")]
        private static extern void SLCDData(byte addr, byte len, byte[] val);

        /// <summary>
        /// set PWM tone volume, 1-32
        /// </summary>
        /// <param name="vol"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void ToneSetVolume(int vol);

        /// <summary>
        /// stop PWM tone generate
        /// </summary>
        [DllImport("WB1308HID.dll")]
        private static extern void ToneStop();

        /// <summary>
        /// play tone, data is an array always contains 8 bytes
        /// </summary>
        /// <param name="data"></param>
        [DllImport("WB1308HID.dll")]
        private static extern void TonePlay(byte[] data);

        [DllImport("WB1308HID.dll")]
        private static extern void RegRead(byte addr);

        [DllImport("WB1308HID.dll")]
        private static extern void RegWrite(byte addr, byte andMask, byte value);

        [DllImport("WB1308HID.dll")]
        private static extern void ExtRegRead(byte addr);

        [DllImport("WB1308HID.dll")]
        private static extern void ExtRegWrite(int addr, int andMask, char value);

        [DllImport("WB1308HID.dll")]
        private static extern void W1388RegRead(char[] addr);

        [DllImport("WB1308HID.dll")]
        private static extern void W1388RegWrite(int count, char[] values);

        [DllImport("WB1308HID.dll")]
        private static extern void W1388RegMaskWrite(int count, char[] values);

        [DllImport("WB1308HID.dll")]
        private static extern void W1388Reset();

        [DllImport("WB1308HID.dll")]
        private static extern void W1388FSKStop();

        [DllImport("WB1308HID.dll")]
        private static extern void W1388FSKConfig(byte FSKC, char FSKLCR);

        [DllImport("WB1308HID.dll")]
        private static extern void W1388FSKSend(int len, char[] values);

        [DllImport("WB1308HID.dll")]
        private static extern void W1388FSKSendRaw(int len, char[] values);

        [DllImport("WB1308HID.dll")]
        private static extern void OtherControl(int len, char[] values);

        [DllImport("WB1308HID.dll")]
        private static extern void VendorControl(int len, char[] values);

        [DllImport("user32", EntryPoint = "PostMessage")]
        private static extern int PostMessageA(int hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32")]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// CreateCompatibleDC
        /// </summary>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        /// <summary>
        /// DeleteDC
        /// </summary>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        /// <summary>
        /// SelectObject
        /// </summary>
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        /// <summary>
        /// DeleteObject
        /// </summary>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// CreateCompatibleBitmap
        /// </summary>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hObject, int width, int height);

        /// <summary>
        /// BitBlt
        /// </summary>
      
        [DllImport("gdi32")]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitCount, IntPtr lpBits);

        [DllImport("gdi32")]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitCount, short[] lpBits);

        public const int SRCCOPYx = 13369376;
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);


        [DllImport("User32.dll")]
        static extern void DrawTextA(IntPtr hdc,
            [MarshalAs(UnmanagedType.LPStr)] string lpString,
            int nCount,
            ref RECT rect,
            int uFormat
            );

        private const int DT_CENTER = 1;
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [Flags]
        public enum SoundFlags : int
        {
            SND_SYNC = 0x0000,  // play synchronously (default) 
            SND_ASYNC = 0x0001,  // play asynchronously 
            SND_NODEFAULT = 0x0002,  // silence (!default) if sound not found 
            SND_MEMORY = 0x0004,  // pszSound points to a memory file
            SND_LOOP = 0x0008,  // loop the sound until next sndPlaySound 
            SND_NOSTOP = 0x0010,  // don't stop any currently playing sound 
            SND_NOWAIT = 0x00002000, // don't wait if the driver is busy 
            SND_ALIAS = 0x00010000, // name is a registry alias 
            SND_ALIAS_ID = 0x00110000, // alias is a predefined ID
            SND_FILENAME = 0x00020000, // name is file name 
            SND_RESOURCE = 0x00040004  // name is resource name or atom 
        }

        [DllImport("winmm.dll")]
        private static extern int PlaySound(String name, IntPtr hMod, SoundFlags flags);


        public bool gestarted = false;

        public delegate void HIDCALLBACKPROC(int wMessage, int wParam, int lParam);//ref Message m);
        private  void HIDCALLBACKPROC2(int wMessage, int wParam, int lParam)
        {
            if (onCheckData != null)
                onCheckData(wMessage, wParam, lParam);
        }


        HIDCALLBACKPROC myInstanz;
        /// <summary>
        /// 
        /// </summary>
        public void setStart()
        {
            Boolean lbReturn = false;
            myInstanz = new HIDCALLBACKPROC(HIDCALLBACKPROC2);
            int ret = StartDeviceDetectionProc(myInstanz,
                                                WM_HID_DEV_ADDED, WM_HID_DEV_REMOVED,
                                                WM_HID_KEY_DOWN, WM_HID_KEY_UP,
                                                WM_HID_VOLUME_DOWN, WM_HID_VOLUME_UP,
                                                WM_HID_PLAYBACK_MUTE, WM_HID_RECORD_MUTE,
                                                WM_HID_GPIO,
                                                WM_HID_BUFFER_TONE,
                                                WM_HID_W1388_ISR,
                                                WM_HID_OTHERCMD_REP,
                                                WM_HID_VENDORCMD_REP,
                                                0, 0);

            if (ret == 1)
                gestarted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void setStop()
        {
            try
            {
                CloseDevice();
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void setFreeHand(int arg)
        {
            SetHandfree(arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void setLCDVInvert(int arg)
        {
            LCDInvert(arg);
            LCDUpdateArea(0, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void setLCDContrast(int arg)
        {
            LCDContrast(arg);
        }

        /// <summary>
        /// 
        /// </summary>
        public void setTonePlay()
        {
            /*ToneSetVolume(0x01);
            TonePlay(aMelody);
            TonePlay(aMelody);
             * */
            PlaySound(inetConstants.isSystemDirectory + "\\ipSounds\\congestion.wav", IntPtr.Zero, SoundFlags.SND_FILENAME | SoundFlags.SND_ASYNC);
        }

        /// <summary>
        /// 
        /// </summary>
        public void setToneStop()
        {
            ToneStop();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wpar"></param>
        /// <param name="lpar"></param>
        public void setHandleChanged(int wpar,int lpar)
        {
            HandleUsbDeviceChange(wpar,lpar);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void setLCDBackLight(int arg)
        {
            LCDBklightOn(arg);
        }


        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }


        uint MAKERGB(int r,int g,int b)
        {
            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);

        static int SRCCOPY = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);

        void ShowBmp(System.Drawing.Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8) throw new System.ArgumentException("1 or 8", "bpp");

            int w = b.Width, h = b.Height;

            IntPtr hbm = b.GetHbitmap();

            BITMAPINFO bmi = new BITMAPINFO();
            bmi.biSize = 40;
            bmi.biWidth = w;
            bmi.biHeight = h;
            bmi.biPlanes = 1;
            bmi.biBitCount = (short)bpp;
            bmi.biCompression = BI_RGB;
            bmi.biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8);
            bmi.biXPelsPerMeter = 2; //1000000  
            bmi.biYPelsPerMeter = 1; // 1000000  
            // Now for the colour table.

            uint ncols = (uint)1 << bpp;
            bmi.biClrUsed = ncols;
            bmi.biClrImportant = ncols;

            bmi.cols = new uint[256];

            if (bpp == 1) { bmi.cols[0] = MAKERGB(0, 0, 0); bmi.cols[1] = MAKERGB(255, 255, 255); }

            else { for (int i = 0; i < ncols; i++) bmi.cols[i] = MAKERGB(i, i, i); }

            IntPtr bits0;
            IntPtr hbm0 = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);

            IntPtr sdc = GetDC(IntPtr.Zero);
            IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc, hbm);
            IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0, hbm0);
            BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, SRCCOPY);

            IntPtr ik = LCDGetDC();
            BitBlt(ik, 0, 0, width, height, hdc0, 0, 0, SRCCOPY);
            LCDUpdateArea(0, 0, 0, 0);

            System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);

            DeleteDC(hdc);
            DeleteDC(hdc0);
            ReleaseDC(IntPtr.Zero, sdc);
            DeleteObject(hbm);
            DeleteObject(hbm0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        
        public void setDrawString()
        {

            Bitmap im = new Bitmap(LCDWidth, LCDHeight, PixelFormat.Format24bppRgb);
            Graphics grap = Graphics.FromImage(im);
            grap.Clear(Color.White);
            string tmp = "";
            Font f = new Font("Calibri", 10.0f,FontStyle.Bold);
            // Left Oben
            tmp = TextToLeftOben;
            if (tmp.Equals(string.Empty))
                tmp = DateTime.Now.ToString("dd.MM.yyyy");
            
            grap.DrawString(tmp, f, Brushes.Black, new PointF(0.0f, 0.0f));
           
            float myTmp;
            // Right oben
            tmp = TextToRightOben;
            if (tmp.Equals(string.Empty))
                tmp = DateTime.Now.ToString("HH:mm");
            SizeF sf = grap.MeasureString(tmp, f);
            
            myTmp = (float)LCDWidth - sf.Width - 3;
            grap.DrawString(tmp, f, Brushes.Black, new PointF( myTmp, 0.0f));
            Font fm = new Font("Calibri", 12.0f);
            tmp = TextToMitte;
            if (tmp.Equals(string.Empty))
                tmp = "inetplus";
            sf = grap.MeasureString(tmp,fm);
            myTmp = (float)LCDWidth/2 - (sf.Width/2);
            if (myTmp < 0)
                myTmp = 0;

            grap.DrawString(tmp, fm, Brushes.Black, new PointF( myTmp, sf.Height + 3));


            tmp = TextToUntenLeft;
            sf = grap.MeasureString(tmp, f);
            grap.DrawString(tmp, f, Brushes.Black, new PointF(4.0f, LCDHeight - sf.Height - 1));


            tmp = TextToUntenRight;
            sf = grap.MeasureString(tmp, f);
            myTmp = (float)LCDWidth - (sf.Width);
            grap.DrawString(tmp, f, Brushes.Black, new PointF(myTmp,LCDHeight - sf.Height - 1));

            im.RotateFlip(RotateFlipType.Rotate180FlipNone);

            ShowBmp(im, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void setToneVolume(int arg)
        {
            setToneVolume(arg);
        }

        public ipControl()
        { }

        ~ipControl()
        { }

       public void Calling()
        {

            try
            {
                Console.WriteLine("Anrufen");
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }
        }
        
    }
}
