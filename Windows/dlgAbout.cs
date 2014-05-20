using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using kioskplus.Utils;

namespace kioskplus.Windows
{
    public partial class dlgAbout : dlgBase
    {
        public dlgAbout()
        {
            InitializeComponent();

            try
            {
                regfor.Text = Program.gnvSplash.isReg;

                UnicodeEncoding u = new UnicodeEncoding();
                byte[] b = u.GetBytes(Program.gnvSplash.isReg);
                char[] out2 = new char[b.Length];
                u.GetChars(b, 0, b.Length, out2, 0);
                regfor.Text = new String(out2);

                u = null;
                b = null;
                out2 = null;
            }
            catch { }

        }

        private void dlgAbout_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void dlgAbout_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void dlgAbout_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private void dlgAbout_Load(object sender, EventArgs e)
        {
        }
    }
}