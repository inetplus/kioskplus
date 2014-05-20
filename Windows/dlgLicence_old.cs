using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kioskplus.Windows
{
    public partial class dlgLicence_old : Form
    {
        int id = 1;

        //List<Panel> myPanel = new List<Panel>();

        public dlgLicence_old()
        {
            InitializeComponent();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            kdnr.Enabled = radioButton2.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dlgLicence_Load(object sender, EventArgs e)
        {
            id = 0;
           // myPanel.Add(panel1);
           // myPanel.Add(panel2);
            panel2.Visible = false;
           

        }

        // weiter
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;

            id = id + 1;

            panel1.Visible = false;
            panel2.Visible = true;
            panel2.Show();

          //  if (id == myPanel.Count - 1)
            //    button2.Enabled = false;

        }

        // back
        private void button2_Click(object sender, EventArgs e)
        {
            if (id > 0)
                id = id - 1;

            //myPanel[id + 1].Visible = false;
            //myPanel[id].Visible = true;

            if (id == 0)
                button2.Enabled = false;

            button1.Enabled = true;

        }
    }
}
