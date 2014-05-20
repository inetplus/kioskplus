using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kioskplus.Windows
{
    public partial class dlgBase : Form
    {
        private Boolean ibStartMove = false;
        private Point mausPoint = new Point();

        public dlgBase()
        {
            InitializeComponent();
        }

        public void dlgBase_MouseDown(object sender, MouseEventArgs e)
        {
            mausPoint.X = e.X;
            mausPoint.Y = e.Y;
            ibStartMove = true;
        }

        public void dlgBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (ibStartMove)
            {
                Point currentPos = Control.MousePosition;
                currentPos.X -= mausPoint.X;
                currentPos.Y -= mausPoint.Y;
                this.Location = currentPos;
                return;
            }
        }

        public void dlgBase_MouseUp(object sender, MouseEventArgs e)
        {
            ibStartMove = false;
        }

        private void dlgBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            mausPoint = Point.Empty;
        }
    }
}
