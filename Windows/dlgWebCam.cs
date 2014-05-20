using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using kioskplus.Utils.iWCam;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace kioskplus.Windows
{
    public partial class dlgWebCam : dlgBase
    {
        private Capture cCapture;
        private IntPtr mIP = IntPtr.Zero;

        public dlgWebCam()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(pBoxSnap.Image);
            b.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + Program.gnvSplash.isKundenID + ".jpg");
            b.Dispose();
            b = null;

            dlgEmailService email = new dlgEmailService();
            email.SetMyFoto(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + Program.gnvSplash.isKundenID + ".jpg");
            email.ShowDialog();

            email = null;
        }

        private void btnSnap_Click(object sender, EventArgs e)
        {
            if (mIP != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(mIP);
                mIP = IntPtr.Zero;
            }

            mIP = cCapture.Click();
            Bitmap bmp = new Bitmap(cCapture.Width, cCapture.Height, cCapture.Stride, PixelFormat.Format24bppRgb, mIP);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            bmp = BitmapManipulator.ResizeBitmap(bmp, pBoxSnap.Width, pBoxSnap.Height);
            pBoxSnap.Image = bmp;
        }

        private void dlgWebCam_Load(object sender, EventArgs e)
        {
            try
            {
                const int w = 640;
                const int h = 480;
                const int VIDEODEVICE = 0;
                const int VIDEOBITSPERPIXEL = 24;

                cCapture = new Capture(VIDEODEVICE, w, h, VIDEOBITSPERPIXEL, pBoxCam);
                
                this.btnSnap_Click(null, null);
            }
            catch
            {
                MessageBox.Show(Program.getMyLangString("errorWebCam"));
                this.Close();
            }
        }
    }
}
