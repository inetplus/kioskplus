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
    public partial class dlgDrucken : dlgBase
    {
        private int iTotalPages = -1;
        String isDruckerName = String.Empty;
        private double idcDruckKosten = 0;
        private inetDrucker invDrucker = null;

        public dlgDrucken(inetDrucker argDrucker)
        {
            InitializeComponent();

            invDrucker = argDrucker;
        }

      

        private void dlgDrucken_Load(object sender, EventArgs e)
        {

            try
            {
                lEinheit.Text = Program.gnvSplash.isWaehrung;
                idcDruckKosten = Program.gnvSplash.idcDruckKosten;
                this.txtKosten.Text = idcDruckKosten.ToString();
            }
            catch { }

            try
            {
                timerDrucker.Interval = 1000;
                timerDrucker.Enabled = true;

                this.TopMost = true;
            }
            catch
            { }
        }

        //delegate void SetCloseCallBack(){}
        public void SetClose()
        {
            timerDrucker.Enabled = false;
            if (invDrucker != null)
                invDrucker.stopPrintMonitor();

            invDrucker = null;
        }
     
        private void timerDrucker_Tick(object sender, EventArgs e)
        {
            int liTotalPages = 0, liMinuten = 0 ;
            int printJobID = 0, printPages = 0;
            double ldDruck;
           // timerDrucker.Enabled = false;
            try
            {
                invDrucker.GetAktuelleJob(ref printJobID, ref printPages);

                if (printPages != 0 && printJobID != 0 && printJobID != null && printPages != null)
                {
                    this.Visible = true;
                    txtSeiten.Text = printPages.ToString();

                    ldDruck = idcDruckKosten * printPages;
                    liMinuten = (int)Math.Round(((ldDruck * 60) / Program.gnvSplash.idcStundenPreis), 0);
                    this.txtGuthaben.Text = liMinuten.ToString();
Console.WriteLine("true true .......");
                    liTotalPages = printPages;
                }
            }
            catch
            {
                liTotalPages = 0;
            }

            if (liTotalPages != 0 && iTotalPages == liTotalPages)
            {
                this.btnYes.Enabled = true;
                this.btnNo.Enabled = true;
                this.timerDrucker.Enabled = false;
                this.timerDrucker.Stop();
                Console.WriteLine("Timer beendet . . . ");

            }
            else
            {
                iTotalPages = liTotalPages;
            }
            Console.WriteLine("Timer läuft------");
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.btnNo.Enabled = false;
            this.btnYes.Enabled = false;

            int abzug = 0;
            try
            {
                abzug = Int32.Parse(txtGuthaben.Text);
            }
            catch 
            { }
            int resMinuten = Program.gnvSplash.DialogNewUser.GetRestDauerAsMinuten();

            if (resMinuten <= (abzug + 5))
            {
                this.btnNo.Enabled = true;
                this.btnYes.Enabled = true;
                msgText.Text = Program.getMyLangString("errorDruckenGuthaben");
                return;
            }
            //dlgNewUser aktualisieren
            if (Program.gnvSplash.DialogNewUser != null)
                Program.gnvSplash.DialogNewUser.SetNewOnlineTime(abzug);

            inetDateTime local;
            local = new inetDateTime();
            String lsOn = local.GetZeitFromMinuten(abzug);
            invDrucker.fortsetzenAktJob();

            Program.gnvSplash.SetUmsatz(0, ref lsOn, "9", 0,"");
            
            System.Threading.Thread.Sleep(200);
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.btnNo.Enabled = false;
            this.btnYes.Enabled = false;
            invDrucker.deleteAktJob();
            
            System.Threading.Thread.Sleep(200);
            this.Close();
        }

        private void dlgDrucken_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                isDruckerName = null;
                invDrucker = null;
            }
            catch { }
        }


    }
}