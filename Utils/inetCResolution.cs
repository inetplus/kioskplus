using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace kioskplus.Utils
{
    public class inetCResolution
    {
        public int iWidth=0, iHeight=0;

        public inetCResolution()
        {
            iWidth = Screen.PrimaryScreen.Bounds.Width;
            iHeight = Screen.PrimaryScreen.Bounds.Height;
        }


        public void SetResolution()
        {

            if (Screen.PrimaryScreen.Bounds.Width == iWidth &&
                Screen.PrimaryScreen.Bounds.Height == iHeight)
                return;

           
Console.WriteLine("resolution...:" + iWidth.ToString() + "/" + iHeight.ToString());

            DEVMODE dm = new DEVMODE();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (Win32User.EnumDisplaySettings(null, Win32User.ENUM_CURRENT_SETTINGS, ref dm) != 0)
            {
                dm.dmPelsWidth = iWidth;
                dm.dmPelsHeight = iHeight;

                int iRet = Win32User.ChangeDisplaySettings(ref dm, Win32User.CDS_TEST);

                if (iRet == Win32User.DISP_CHANGE_FAILED)
                {
                }
                else
                {
                    iRet = Win32User.ChangeDisplaySettings(ref dm, Win32User.CDS_TEST);
                    if (Win32User.DISP_CHANGE_FAILED == iRet)
                    {
                    }
                    else
                    {
                        iRet = Win32User.ChangeDisplaySettings(ref dm, Win32User.CDS_UPDATEREGISTRY);
                        switch (iRet)
                        {
                            case Win32User.DISP_CHANGE_SUCCESSFUL:
                                {
                                    break;
                                }
                            case Win32User.DISP_CHANGE_RESTART:
                                {
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }


            }

        }
     

    }
}
