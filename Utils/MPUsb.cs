using System;
using System.Collections.Generic;
using System.Threading;
using whCcTalkCommunication;


namespace kioskplus.Utils
{


    public class MPUsb
    {
        
        private whSelectorComm currentMP = null;
        // Selector Coins
        private whCoinValue[] SelCoinValues = new whCoinValue[16];
        private whSelCoinStatus[] SelCoinStates = new whSelCoinStatus[16];

        private Boolean ibReset = false;
        private Boolean ibOwnMP = false;

        public MPUsb(whCcTalkComm argSelector)
        {
            currentMP = new whSelectorComm();
            currentMP.Port = argSelector.Port;
            currentMP.Address = argSelector.Address;
            currentMP.ChecksumType = argSelector.ChecksumType;
            currentMP.EncryptionMode = argSelector.EncryptionMode;
            currentMP.InitPINCode(argSelector.GetPINCode());
        }


        private int OpenClose()
        {
            if (currentMP.IsOpen)
            {
                currentMP.CloseComm();

                if (currentMP.LastError == whCcTalkErrors.Ok)
                    return 2;
            }
            else
            {
                currentMP.OpenComm();
                currentMP.ResetDevice(100);

                ibOwnMP = true;

                if (currentMP.LastError == whCcTalkErrors.Ok)
                    return 1;
            }

            return 0;
        }

        private void ReadSelector()
        {
            currentMP.GetCoinStates(ref SelCoinStates);
            currentMP.GetCoinValues(ref SelCoinValues);

            if (currentMP.MasterInhibit)
                ibReset = true;
        }

        private void ResetSelector()
        {
            currentMP.MasterInhibit = !currentMP.MasterInhibit;
        }

        private int WriteSelector()
        {
            ReadSelector();

            int liReturn = 0;
            inetRegistry local = new inetRegistry();
            String[] s = local.ReadValueNames(local.SubKey + "\\" + inetConstants.icsVerzMP);
            int index;
            local.SubKey = local.SubKey +"\\" + inetConstants.icsVerzMP;
            if (s != null)
            {
                try
                {

                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i].StartsWith(inetConstants.icsMPMf))
                        {
                            index = Int32.Parse(s[i].Substring(2));
                            if (local.ReadAsDWORD(s[i]) == 1)
                                SelCoinStates[index].Inhibit = true;
                            else
                                SelCoinStates[index].Inhibit = false;
                        }
                    }
                }
                catch
                { }
                try
                {
                    if (currentMP.SetCoinInhibit(SelCoinStates) == whCcTalkErrors.Ok)
                        liReturn = 1;

                }
                catch
                {
                    return 0;
                }

                return 1;
            }
            local = null;

            return 0;
        }

        public int StartMPPolling()
        {
            if (OpenClose() == 1)
            {
                ReadSelector();
                if (ibReset)
                {
                    ResetSelector();
                    ibReset = false;
                }

                if (WriteSelector() == 1)
                {
                    return 1;
                }
            }
            return 0;
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credit"></param>
        public void Polling(ref double credit, ref string id)
        {

              int i, j, cidx, EvtCnt;

              credit = 0;
              if (currentMP.LastError != whCcTalkErrors.Ok)
                  return;


                try
                {
                    whSelPollResponse[] PollResps = new whSelPollResponse[whSelectorComm.MaxPollEvents];

                    currentMP.PollSelector(ref PollResps, out EvtCnt);
                    // Process poll response(s)
                    if (EvtCnt > 0)
                    {
                        for (i = 0; i < EvtCnt; i++)
                        {
                            // Show last poll
                            if (PollResps[i].Status == whSelPollEvent.Coin) {
                                cidx = PollResps[i].CoinIndex;
                                credit = SelCoinValues[cidx].Value;
                                id = SelCoinValues[cidx].ID;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                   // System.Windows.Forms.MessageBox.Show("Fehler...polling:" + ex.Message);
                }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean isReadCoinSelector()
        {
            if (OpenClose() == 1)
            {
                ReadSelector();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean Close()
        {
            if (OpenClose() == 2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="awhValue"></param>
        /// <param name="awhStatus"></param>
        public void GetValuesAndStatus(ref whCoinValue[] awhValue, ref whSelCoinStatus[] awhStatus)
        {
            awhValue = SelCoinValues;
            awhStatus = SelCoinStates;
        }

        public Boolean IsInet()
        {
            return ibOwnMP;
        }
      
    }
}
