using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;
using kioskplus.Utils;

namespace kioskplus.Windows
{
    public partial class dlgLicence : Form
    {

        bool customer = false;

        private dlgEinstellung _dlgEinst;

        private Hashtable countries = new Hashtable();

        public dlgEinstellung winEinstellung
        {
            get { return _dlgEinst; }
            set { _dlgEinst = value;  }
        }


        public dlgLicence()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Program.gnvSplash.BlockTaste(true);
            this.Close();
            
        }

    

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btncontinue_Click(object sender, EventArgs e)
        {
            btnback.Enabled = true;

            if (panel1.Visible)
            {
                
                customer = false;
                if (tbkdnr.Visible)
                {
                   

                   if ((tbkdnr.Text.Trim().Equals("") || IsAlpha(tbkdnr.Text)))
                    {
                        MessageBox.Show(Program.getMyLangString("errorKdNr"));
                        return;
                    }
                   if ((tbkdpwd.Text.Trim().Equals("")))
                   {
                       MessageBox.Show(Program.getMyLangString("errorPwdEingabe"));
                       return;
                   }
                    customer = true;

                    panel1.Visible = false;
                    panel2.Visible = true;
                   // panel2.Location = panel1.Location;

                    //rtbLicense.Text = Program.getMyLangString("bittewarten");
                   // btncontinue.Enabled = false;

                    
                    // call web service to generate a new licence
                    //getNewLicence();
                    return;
                }

                panel1.Visible = false;
                panel2.Visible = true;

            }
            else if (panel2.Visible)
            {
       
                if (customer)
                {
                    panel2.Visible = false;
                    panel4.Visible = true;
                    return;
                }

                panel2.Visible = false;
                panel3.Visible = true;
            }
            else if (panel3.Visible)
            {
                String rc = checkDaten();
                if (!rc.Equals(""))
                {
                    MessageBox.Show(Program.getMyLangString(rc));
                    return;
                }

                panel3.Visible = false;
                panel4.Visible = true;
            }
            else if (panel4.Visible)
            {
                rtbLicense.Text = Program.getMyLangString("bittewarten");
                // call web service to generate a new licence
                btncontinue.Enabled = false;
                btnback.Enabled = false;
                getNewLicence();
            }



        }

        private string checkDaten() {
            bool error;

            error = tname.Text.Equals("") || tvorname.Text.Equals("") || tstr.Text.Equals("") ||
                    tort.Text.Equals("") || tplz.Text.Equals("");

            if (!error)
                if (cbcountry.SelectedItem != null)
                    error = cbcountry.SelectedItem.Equals("");

            if (error)
                return "allgEingabeError";

            error = isEmail(temail.Text) && isEmail(temail2.Text) && (temail2.Text.Equals(temail.Text));

            if (!error)
                return "errorEmailAdress";

            if (tpwd.Text.Equals("") || tpwd2.Text.Equals(""))
                return "errorPwdEingabe";
            if (!tpwd.Text.Equals(tpwd2.Text))
                return "errorPwdAgain";
            
            return "";
        }

        private bool isEmail(string inputEmail)
        {
            if (inputEmail.Equals(""))
                return false;
            
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            if (panel4.Visible)
            {
                btncontinue.Enabled = true;
                rtbLicense.Text = String.Empty;

                if (customer)
                {
                    panel4.Visible = false;
                    panel2.Visible = true;
                }
                else
                {
                    panel4.Visible = false;
                    panel3.Visible = true;
                }
            }
            else if (panel3.Visible)
            {
                panel3.Visible = false;
                panel2.Visible = true;

                //btnback.Enabled = false;
            }
            else if (panel2.Visible)
            {
                panel2.Visible = false;
                panel1.Visible = true;

                btnback.Enabled = false;
            }

        }

        private void rbyes_CheckedChanged(object sender, EventArgs e)
        {
            tbkdnr.Visible = rbyes.Checked;
            tbkdpwd.Visible = rbyes.Checked;
            labelkdnr.Visible = rbyes.Checked;
        }

        private void dlgLicence_Load(object sender, EventArgs e)
        {
            this.Size = new Size(576, 272);
            panel2.Size = panel1.Size;
            panel3.Size = panel1.Size;
            panel4.Size = panel1.Size;

            panel2.Location = panel1.Location;
            panel3.Location = panel1.Location;
            panel4.Location = panel1.Location;


           Thread th = new Thread(new ThreadStart(getcountrylist));
            th.Start();

            Program.gnvSplash.BlockTaste(false);

        }

        private void getcountrylist()
        {
            /**
            * 
            * **/
            httpcontrol ihc = new httpcontrol();
            ihc.RunAction("1=country/get", Program.gnvSplash.iActServerID, false);
            String[] allCountries = ihc.GetParameterValue("result").Split('#');
            foreach (String temp in allCountries)
            {
                try
                {
                    if (temp.Equals(""))
                        continue;

                    String[] ss = temp.Split(';');
                    countries.Add(ss[1] + " (" + ss[0] + ")", ss[0]);
                    
                }
                catch (Exception eer)
                {
                    Console.WriteLine("Ert>>" + eer.Message);
                }
            }
            Console.WriteLine("getcountry::" + ihc.GetParameterValue("result"));
            setItemcountry();
          
        }

        delegate void setItemcountryCallBack();
        private void setItemcountry()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new setItemcountryCallBack(setItemcountry));// Invoke(new SetTextCallBack(), new object[] { });
                return;
            }

            foreach (DictionaryEntry t in countries)
                cbcountry.Items.Add(t.Key);

            bw.Visible = false;
            cbcountry.Sorted = true;

            // btncontinue.Enabled = true;


        }

        private bool IsAlpha(string input)
        {
            return Regex.IsMatch(input, "^[a-zA-Z]", RegexOptions.IgnoreCase);
        }

        private bool IsAlphaNumeric(string input)
        {

            return Regex.IsMatch(input, "^[a-zA-Z0-9]+$");
        }

        private bool IsAlphaNumericWithUnderscore(string input)
        {
            return Regex.IsMatch(input, "^[a-zA-Z0-9_]+$");
        }


        private void getNewLicence()
        {

            try
            {
                StringBuilder str = new StringBuilder();
                str.Append("1=ls");

                if (rbyes.Checked) // Customer
                    str.Append("&2=2").Append("&3=").Append(tbkdnr.Text)
                        .Append("&4=").Append(tbkdpwd.Text)
                        .Append("&13=").Append(Convert.ToInt32(rb_lizenz00.Checked));
                else
                {
                    str.Append("&2=1")
                        .Append("&3=").Append(tvorname.Text)
                        .Append("&4=").Append(tname.Text)
                        .Append("&5=").Append(tstr.Text)
                        .Append("&6=").Append(tplz.Text)
                        .Append("&7=").Append(tort.Text)
                        .Append("&8=").Append(tpwd.Text)
                        .Append("&9=").Append(tfirma.Text)
                        .Append("&10=").Append(temail.Text)
                        .Append("&11=").Append(tpwd.Text)
                        .Append("&12=").Append(countries[cbcountry.SelectedItem])
                        .Append("&13=").Append(Convert.ToInt32(rb_lizenz00.Checked));
                }

                httpcontrol ihc = new httpcontrol();
                if (ihc.RunAction(str.ToString(), Program.gnvSplash.iActServerID, false))
                {

                    if (!ihc.GetParameterValue("24").Equals(""))
                    {
                        DialogResult dr;
                        dr = MessageBox.Show(ihc.GetParameterValue("24"), "kioskplus", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            rb_lizenz02.Checked = true;

                            getNewLicence();
                            return;
                        }
                        rtbLicense.Text = Program.getMyLangString("cancelLicenceAction");
                        return;
                    }
                    else if (!ihc.GetParameterValue("25").Equals(""))
                    {
                        rtbLicense.Text = ihc.GetParameterValue("25");//Program.getMyLangString("cancelLicenceAction");
                        btnback.Enabled = true;
                        return;
                    }
                    rtbLicense.Text = String.Format(Program.getMyLangString("getfreelicence"),
                                                    Environment.NewLine,Environment.NewLine + Environment.NewLine, 
                                                    Environment.NewLine +  ihc.GetParameterValue("licence"));

                    try
                    {
                        _dlgEinst.setLicenceInformation(ihc.GetParameterValue("licence"),
                            tstr.Text + Environment.NewLine + tplz.Text + " " + tort.Text);
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine("klkl:::" + ee.Message);
                    }
                }
                ihc = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:::" + ex.Message);
            }


        }

        private void cbagb_CheckStateChanged(object sender, EventArgs e)
        {
            btncontinue.Enabled = cbagb.Checked;
        }

      
        


    }
}
