using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace kioskplus.Utils
{
    public class inetProcess
    {
        //Process[] aktuelleProcess;
        List<Process> aktuelleProcess = new List<Process>();

        public void GetAllAktuelleProcess()
        {
            aktuelleProcess.Clear();
            aktuelleProcess.AddRange(Process.GetProcesses());
        }

        public void KillOtherProcess()
        {
            Process[] process = Process.GetProcesses();
            Boolean lbGefunden = false;

            foreach (Process p in process)
            {
                foreach (Process pin in aktuelleProcess)
                {
                    if (p.Id == pin.Id)
                        lbGefunden = true;
                }
                try
                {
                    if (!lbGefunden && aktuelleProcess.Count != 0)
                    {
                        //Console.WriteLine("Process:" + p.Id.ToString() + "/" + p.MainWindowTitle + "/" + p.MainModule.FileName);
                        int i = 0;
                        if (!p.CloseMainWindow())
                        {
                            try
                            {
                                p.Close();
                                i = i + 21;
                                p.Kill();
                                i = i * 21;
                                p.Close();
                            }
                            catch /*(Exception ex)*/
                            {
                            }
                        }
                        else
                        {
                            try
                            {
                                if (!p.WaitForExit(1000)) // 1 Sekunde warten?
                                {
                                    p.Kill();
                                    i = i / 12;
                                    p.Kill();
                                    p.Close();
                                    i = i + 23;
                                    p.Kill();
                                    p.Close();
                                    i = i / 2;

                                    p.Kill();
                                }
                            }
                            catch /*(Exception ex)*/
                            { }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(p.MainWindowTitle);
                    Console.WriteLine("Return: " + ex.Message); }

                lbGefunden = false;
            }

        }

    }
}
