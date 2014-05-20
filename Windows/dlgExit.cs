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
    public partial class dlgExit : dlgBase
    {
        inetDateTime invDT;

        public dlgExit()
        {
            InitializeComponent();
        }

        public dlgExit(inetDateTime adt)
        {
            InitializeComponent();
            invDT = adt;
        }

        private void dlgExit_Load(object sender, EventArgs e)
        {
            if ((Program.gnvSplash.iKundenID > 0) || (!Program.gnvSplash.ibAufladen) ||
                  (!Program.gnvSplash.ibStatusPing) ||
                    (invDT.GetRestDauerAsMin() <= 5))
            {
                rbKonto.Visible = false;
                rbBlock.Location = rbKonto.Location;
            }
            else
            {
                rbKonto.Checked = true;
                
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbKonto.Checked) // Konto Fenster öffnen
            {
                this.Visible = false;
                dlgUserAccount acount = new dlgUserAccount();
                acount.ShowDialog();
            }
            else if (rbBlock.Checked) // schliessen
            {
                Program.gnvSplash.SperrenComputer("1");
            }
            this.Close();
        }

        private void dlgExit_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                invDT = null;
            }
            catch
            { }
        }

    }
}