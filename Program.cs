using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.IO;
using System.ServiceProcess;
using Microsoft.Win32;
using kioskplus.Utils;

namespace kioskplus
{
    static class Program
    {
        public static inetplusApp gnvSplash;
        public static ResourceManager gnvResources = null;
        public static IFormatProvider gnvDecFormat = new NumberFormatInfo();
        public static Mutex mutex = new Mutex(true, inetConstants.icsMutex);
        public static String defaultLanguage = String.Empty;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {

            Console.WriteLine(inetBase64.encodeAsString("Y"));
            Console.WriteLine(inetBase64.encodeAsString("ru-RU"));

            try
            {
               // MessageBox.Show("1");

                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    //MessageBox.Show("2");
                    autoStartAktiv();
                    //MessageBox.Show("3");
                    gnvSplash = new inetplusApp();

                    inetRegistry Reg = new inetRegistry();


                    // if Win7 - check Permissions and reboot if require to change it 
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                        Environment.OSVersion.Version.Major >= 6 &&
                        Environment.OSVersion.Version.Minor >= 0)
                    {

                        inetRegistry lReg = new inetRegistry();
                        String lsVal = "0";
                        lReg.BaseRegistryKey = Registry.LocalMachine;
                        lReg.SubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                        lsVal = lReg.ReadAsStringNoEncrypt("firstinet");
                        lReg.DeleteKey("firstinet");
                        //MessageBox.Show("loe hat nich klappt/" + System.Environment.OSVersion.Version+"/"+lsVal);
                        if (lsVal.Equals("1"))
                        {
                            MessageBox.Show("Ihr Terminal wird jetzt neu gestartet, damit der gesamte Funktionsumfang aktiviert werden kann !" +
                                            Environment.NewLine + "Bitte betätigen Sie 'OK' zum Fortfahren" + Environment.NewLine +
                                            Environment.NewLine + "Terminal will be restarted to activate all kiosplus features." + Environment.NewLine +
                                            "Please select 'OK' button to continue");
                            inetAPI.PCReboot();
                            return;
                        }
                       
                    }


                    defaultLanguage = Reg.ReadAsString(inetConstants.icsLanguage);

                    if (defaultLanguage == null)
                        defaultLanguage = "";

                    switchLanguage(defaultLanguage);
                   // MessageBox.Show("4");
                    Reg = null;
                    
                    try
                    {
                        gnvSplash.BlockMaus(true);
                        gnvSplash.BlockTaste(true);
                    }
                    catch
                    { }
                    
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(true);

                  //  Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
                    //MessageBox.Show("5");
                    Application.Run(new MainInetplus());
                    //MessageBox.Show("6");

                    // 20100130
                    try
                    {
                        gnvSplash.BlockTaste(true);
                    }
                    catch
                    { }

                    System.Diagnostics.ProcessThreadCollection t = System.Diagnostics.Process.GetCurrentProcess().Threads;
                    foreach (Thread t1 in t)
                        try
                        {
                            t1.Abort();
                        }
                        catch { Console.WriteLine("noch"); }

                    mutex.ReleaseMutex();
                    mutex = null;
                }
            }
            catch
            {
                //MessageBox.Show("7-error");
                try
                {
                    mutex.ReleaseMutex();
                    mutex = null;
                }
                catch
                { }
            }
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="argLanguage"></param>
        public static void switchLanguage(String argLanguage)
        {

            try
            {
                gnvResources = new ResourceManager("kioskplus.kioskplus" + argLanguage, Assembly.GetExecutingAssembly());
                
                Thread.CurrentThread.CurrentCulture = new CultureInfo(argLanguage,true);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(argLanguage,true);
                
                Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator = ",";
                Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyGroupSeparator = ".";
                Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ",";
                Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberGroupSeparator = ".";

                gnvDecFormat = Thread.CurrentThread.CurrentUICulture.NumberFormat;
            }
            catch 
            {  }

            gnvSplash.isLanguage = argLanguage;

            try
            {
                if (gnvSplash != null && gnvSplash.gnvMainInetplus != null)
                    gnvSplash.gnvMainInetplus.SetNewAgent();
            }
            catch
            { }

        }

        /// <summary>
        /// get the value from 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string getMyLangString(string key)
        {
            try
            {
                key = gnvResources.GetString(key);
                if (key == null)
                    key = "<" + string.Empty + ">"; 
            }
            catch 
            {
                key = "<"+string.Empty + ">";
            }

            return key;
        }


        static void autoStartAktiv()
        {
            string[] ls_keys= {"txt","hlp","ini","inf","fon","exe","com","bat","wri","reg","pif","mp3","m3u","hta","cmd","clp"};
            string ls_verz1= @"Software\Microsoft\Windows\CurrentVersion\Run";
            string ls_verz2= @"Software\Microsoft\Windows\CurrentVersion\RunServices";
            string ls_ieNOT4= @"CLSID\{871C5380-42A0-1069-A2EA-08002B30309D}\shell\openhomepage\command";
            string ls_ie4	= @"CLSID\{FBF23B42-E3F0-101B-8488-00AA003E56F8}\shell\open\command";

            inetRegistry localRegistry = new inetRegistry();

            try
            {
                localRegistry.SubKey = ls_verz1;
                localRegistry.WriteNoCrypt("inetplus", inetConstants.isSystemDirectory + "\\i-client.exe");
                localRegistry.SubKey = ls_verz2;
                localRegistry.WriteNoCrypt("inetplus", inetConstants.isSystemDirectory + "\\i-client.exe");

                localRegistry.BaseRegistryKey = Microsoft.Win32.Registry.CurrentUser;
                localRegistry.SubKey = ls_verz1;
                localRegistry.WriteNoCrypt("inetplus", inetConstants.isSystemDirectory + "\\i-client.exe");
                localRegistry.SubKey = ls_verz2;
                localRegistry.WriteNoCrypt("inetplus", inetConstants.isSystemDirectory + "\\i-client.exe");
            }
            catch(Exception ex)
            { Console.WriteLine("ERROR"+ex.Message); }

            //
            String ls = @"Software\Microsoft\Windows\CurrentVersion\RunOnce";
            localRegistry.SubKey = ls;

            localRegistry.WriteNoCrypt("*inetplus", inetConstants.isSystemDirectory + "\\i-client.exe");

            // bwcl
            String fileBWCL = inetConstants.isSystemDirectory + "\\bwcl.exe";
            String newValue = String.Empty;

            try
            {
                if (File.Exists(fileBWCL))
                {
                    localRegistry.BaseRegistryKey = Microsoft.Win32.Registry.ClassesRoot;
                    foreach (string tmp in ls_keys)
                    {
                        ls = tmp + "file\\shell\\open\\command";
                        localRegistry.SubKey = ls;
                        if (localRegistry.ReadAsStringNoEncrypt("orignet").Equals(""))
                        {
                            newValue = localRegistry.ReadAsStringNoEncrypt("");
                            localRegistry.WriteNoCrypt("orignet", newValue);
                            newValue = "\"" + fileBWCL + "\" " + newValue;
                            localRegistry.WriteNoCrypt("", newValue);
                        }
                    }

                    // for IE
                    localRegistry.SubKey = ls_ie4;
                    if (localRegistry.ReadAsStringNoEncrypt("orignet").Equals(""))
                    {
                        newValue = localRegistry.ReadAsStringNoEncrypt("");
                        localRegistry.WriteNoCrypt("orignet", newValue);
                        newValue = "\"" + fileBWCL + "\" " + newValue;
                        localRegistry.WriteNoCrypt("", newValue);
                    }
                    localRegistry.SubKey = ls_ieNOT4;
                    if (localRegistry.ReadAsStringNoEncrypt("orignet").Equals(""))
                    {
                        newValue = localRegistry.ReadAsStringNoEncrypt("");
                        localRegistry.WriteNoCrypt("orignet", newValue);
                        newValue = "\"" + fileBWCL + "\" " + newValue;
                        localRegistry.WriteNoCrypt("", newValue);
                    }
                }
            }
            catch(Exception ex)
            { Console.WriteLine("ERROR"+ex.Message); }

            try
            {
                // F8
                localRegistry.BaseRegistryKey = Microsoft.Win32.Registry.LocalMachine;
                localRegistry.SubKey = @"SYSTEM\CurrentControlSet\Control\SafeBoot\";
                localRegistry.RenameSubKey("Minimal", "_Minimal");
                localRegistry.RenameSubKey("Network", "_Network");
            }
            catch(Exception ex)
            { Console.WriteLine("ERROR"+ex.Message); }
            localRegistry = null;

        }
    }
}