using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using kioskplus.Utils;
using System.Threading;

namespace kioskplus.Windows
{
    public partial class dlgAffi : dlgBase
    {
        System.Windows.Forms.Timer timer = null;
        bool ibClose = false;
        int aktIndex = -1;
        int WidthOfMain = 0, HeightofMain = 0, LocationMainX = 0, locationMainy = 0;

        List<RecordAffili> allAffili = null;
        bool ibBackward = false;

        public void SetibClose(bool arg)
        {
            ibClose = arg;
        }


        public dlgAffi()
        {
            InitializeComponent();
        }

        private void checkDialogNewUserPos()
        {
            if (Program.gnvSplash.DialogNewUser != null)
            {
                WidthOfMain = Program.gnvSplash.DialogNewUser.Width;
                HeightofMain = Program.gnvSplash.DialogNewUser.Height;
                LocationMainX = Program.gnvSplash.DialogNewUser.Location.X;
                locationMainy = Program.gnvSplash.DialogNewUser.Location.Y;
            }
        }

        private void dlgAffi_Load(object sender, EventArgs e)
        {

            try
            {
                this.Visible = false;
                checkDialogNewUserPos();
                int iTimer = 5;
                httpcontrol hc = new httpcontrol();

                hc.RunAction("action=ipaffili/get&2=" + Program.gnvSplash.isLandVorwahl, 0, false);
                String result = hc.GetParameterValue("result");
                String timerInMin = result.Substring(0, 3);
                result = result.Substring(3);
                String[] myTempList = result.Split('#');
                foreach (String temp in myTempList)
                {
                    if (temp.Equals(""))
                        continue;

                    String[] temp2 = temp.Split(';');
                    RecordAffili ra = new RecordAffili();
                    ra.affiCode = temp2[0];
                    ra.affiArt = temp2[1];
                    ra.affiImage = temp2[2];
                    ra.affiLink = temp2[3];
                    ra.affiCountry = temp2[4];

                    if (allAffili == null)
                        allAffili = new List<RecordAffili>();

                    allAffili.Add(ra);
                    Console.WriteLine("AffiClass:::> " + ra.affiCode);
                }

                try
                {
                    iTimer = Int32.Parse(timerInMin);
                }
                catch { iTimer = 5; }
                Console.WriteLine("iTimer:" + iTimer);
                timer = new System.Windows.Forms.Timer();
                timer.Interval = iTimer * 60 * 1000; // 1 Minuten
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();

            }
            catch (Exception ex)
            { Console.WriteLine("load..." + ex.Message); }

            if (allAffili != null && allAffili.Count > 0)
                changeImage();

        }

        void timer_Tick(object sender, EventArgs e)
        {
            // timer aufgerufen....
            Console.WriteLine("timer aufgerufen");


            try
            {
                if (allAffili == null)
                {
                    timer.Stop();
                    
                    return;
                }
                changeImage();

            }
            catch (Exception ex)
            { Console.WriteLine("timer-affi:" + ex.Message); }

        }

        private void changeImage()
        {

            if (!ibBackward) //  aktIndex < (allAffili.Count-1) )
            {
                aktIndex++;
                if (aktIndex > (allAffili.Count - 1))
                {
                    ibBackward = true;
                    aktIndex--;
                }
            }
            else
            {
                aktIndex--;

                if (aktIndex < 0)
                {
                    aktIndex = 0;
                    ibBackward = false;
                }

            }
            RecordAffili ra = allAffili[aktIndex];
            if (ra.affiArt.Equals("0")) //link
            {
                
                Console.WriteLine(ra.affiImage + " // " + pbaffi.Size.Width);
                pbaffi.ImageLocation = ra.affiImage;

                this.TopMost = true;
            }
            else if (ra.affiArt.Equals("1")) // webservice
            {

            }

           
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }


        delegate void SetCloseCallBack();
        public void SetClose()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetCloseCallBack(SetClose), new object[] { });
                return;
            }
            else
            {
                try
                {

                    ibClose = true;

                    this.Close();
                }
                catch { }
            }
        }

        delegate void SetChangePosCallBack();
        public void SetChangePos()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetChangePosCallBack(SetChangePos), new object[] { });
                return;
            }
            else
            {
                try
                {

                    //checkDialogNewUserPos();
                //    checkFensterPosition();
                  //  int lw = (Screen.PrimaryScreen.WorkingArea.Right - pbaffi.Size.Width - 20);
                  //  if (lw > LocationMainX)
                    //    lw = Program.gnvSplash.DialogNewUser.Location.X;

                   /// this.Location = new Point(lw, Screen.PrimaryScreen.WorkingArea.Bottom);// locationMainy + HeightofMain + 2);

                    
                }
                catch { }
            }
        }

        private void dlgAffi_FormClosing(object sender, FormClosingEventArgs e)
        {


            Console.WriteLine("FormClosing...:-1:: " + ibClose);
            if (ibClose == false) // && (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                e.Cancel = true;
            }
            else
            {
                Console.WriteLine("FormClosing...: Timer schliessen");
                timer.Stop();
                timer.Enabled = false;
               // timer.Interval = 0;
            
            }

        }

        private void pbaffi_Click(object sender, EventArgs e)
        {
            try
            {
                if (allAffili != null && allAffili.Count > 0)
                {
                    System.Diagnostics.Process.Start(allAffili[aktIndex].affiLink);
                    changeImage();
                }
            }
            catch (Exception ex)
            { Console.WriteLine("Clikc:"+ex.Message); }
        }

        private void pbaffi_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {

               // checkDialogNewUserPos(); 
                Console.WriteLine("Ende Loadcomplete:: " + pbaffi.Size.Width);
                // inetAnimation.AnimateWindow(pbaffi.Handle, 500, inetAnimation.AW_SLIDE | inetAnimation.AW_VER_POSITIVE);
                this.Size = pbaffi.Size;


                int lw = (Screen.PrimaryScreen.WorkingArea.Right - pbaffi.Size.Width - 20);
              //  if (lw > LocationMainX)
                //    lw = Program.gnvSplash.DialogNewUser.Location.X;

                this.Location = new Point(lw, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);//locationMainy + HeightofMain + 2);
                
                
            }
            catch
            { }
        }

        private void checkFensterPosition()
        {

            Console.WriteLine("checkfensterposition");
            /*if (this.Location.X <= 10)
                this.Location = new Point(LocationMainX+WidthOfMain, locationMainy);

            if ((this.Location.X + this.Size.Width) > Screen.PrimaryScreen.Bounds.Right)
                this.Location = new Point(LocationMainX - this.Size.Width-2, locationMainy);

            if (this.Location.Y < Screen.PrimaryScreen.Bounds.Top)
                this.Location = new Point(this.Location.X, locationMainy + HeightofMain + 2);

            if ((this.Location.Y + this.Size.Height) > Screen.PrimaryScreen.WorkingArea.Bottom)
                this.Location = new Point(LocationMainX - this.Size.Width, this.Location.Y - this.Size.Height);
             * */
        }

      

        private void dlgAffi_LocationChanged(object sender, EventArgs e)
        {
            
           // checkFensterPosition();
        }

  


       
    }

   
}
