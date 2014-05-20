using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using kioskplus.Utils;

namespace kioskplus.Windows
{
    public partial class dlgSplashAbbruch : Form
    {
        String isPWD = String.Empty;
        String isPWDEinstellung = String.Empty;
        int iiFalsch = 0;

        public dlgSplashAbbruch()
        {
            InitializeComponent();
        }

        private void dlgSplashAbbruch_Load(object sender, EventArgs e)
        {
            inetRegistry local = new inetRegistry();

            isPWD= local.ReadAsString(inetConstants.icsAbbruch);
            isPWDEinstellung = local.ReadAsString(inetConstants.icsEinPwd);
            local = null;
            if (Program.gnvSplash.ibKennzShutdownBtn)
                btnHerunterfahren.Enabled = true;

            if (isPWD.Equals(""))
                isPWD = inetConstants.icsStandardPassword;
            
            if (isPWDEinstellung.Equals(""))
                isPWDEinstellung = inetConstants.icsStandardPassword;

            timerAbbruch.Enabled = true;

            lversion.Text = "Version: " + Program.gnvSplash.isClientVersion;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            if (iiFalsch > 3)
            {
                this.Close();
                return;
            }

            if (txtPwd.Text.Equals(isPWD))
            {
                SetButtonState();

                // Alt+Ctrl+Del sperren immer
                inetRegistry localReg = new inetRegistry();
                //  localReg.WriteNoCrypt(inetConstants.icsDauerIgnore, "Y");
                try
                {
                    localReg.WriteNoCrypt(inetConstants.icsIgnore, "Y");
                }
                catch (Exception eX) { MessageBox.Show("Error---icyIgnore: " + eX.Message); }

                if (checkBoxAdmin.Checked)
                {

                    try
                    {
                        localReg.BaseRegistryKey = Registry.CurrentUser;

                        localReg.SubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                        localReg.DeleteKey("DisableTaskMgr");// Ctrl + Alt + Del Sperren
                        localReg.DeleteKey("DisableRegistryTools"); //Registry sperren

                        localReg.SubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
                        localReg.DeleteKey("NoDesktop");
                        localReg.DeleteKey("NoAddPrinter");
                        localReg.DeleteKey("NoDeletePrinter");
                        localReg.DeleteKey("NoPrinterTabs");
                        localReg.DeleteKey("NoControlPanel"); // 20100130
                    }
                    catch (Exception ee)
                    { MessageBox.Show("Err: " + ee.Message); }
                }
                Program.gnvSplash.AppClose();
            }
            else
                iiFalsch++;

        }

        private void SetButtonState()
        {
            btnHerunterfahren.Enabled = false;
            btnEinstellung.Enabled = false;
            btnOK.Enabled = false;
            checkBoxAdmin.Enabled = false;
        }

        private void btnHerunterfahren_Click(object sender, EventArgs e)
        {
            SetButtonState();
            inetAPI.PCShutDown();
        }

        private void btnEinstellung_Click(object sender, EventArgs e)
        {
            SetButtonState();
            txtPwd.Enabled = false;
            btnEinstellung.Enabled = false;
            btnOK.Enabled = false;
            this.AcceptButton = btnOk2;
            progressBar1.Value = 0;

            txtEinPwd.Focus();
            this.Height += 100;
        }

        private void btnOk2_Click(object sender, EventArgs e)
        {
            if (isPWDEinstellung.Equals(txtEinPwd.Text))
            {
                this.Close();
                dlgEinstellung dlg = new dlgEinstellung(false);
                dlg.ShowDialog();
            }
        }

        private void timerAbbruch_Tick(object sender, EventArgs e)
        {


            if (progressBar1.Value == 20)
            {
                timerAbbruch.Enabled = false;
                Close();
                return;
            }
            progressBar1.Value += 1;
            labelZeit.Text = progressBar1.Value.ToString();

        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            
            if (isPWD.Equals(txtPwd.Text))
            {
                this.btnOK.Enabled = true;
                this.btnEinstellung.Enabled = true;
            }
            else
            {
                this.btnOK.Enabled = false;
                this.btnEinstellung.Enabled = false;
            }
        }

        private void txtEinPwd_TextChanged(object sender, EventArgs e)
        {
            this.btnOk2.Enabled = isPWDEinstellung.Equals(txtEinPwd.Text);
        }

        private void dlgSplashAbbruch_Shown(object sender, EventArgs e)
        {
            txtPwd.Focus();
        }
    }
}