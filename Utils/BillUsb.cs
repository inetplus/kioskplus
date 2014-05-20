using System;
using System.Collections.Generic;
using System.Threading;
using whCcTalkCommunication;

namespace kioskplus.Utils
{
    public class BillUsb
    {
        private whValidatorComm currentValidator;
        private whBillValue[] SelCurrentValue = new whBillValue[16];
        private whValBillStatus[] SelCurrentStatus = new whValBillStatus[16];
        private Boolean ibReset = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argComm"></param>
        public BillUsb(whCcTalkComm argComm)
        {
            currentValidator = new whValidatorComm();
            currentValidator.Port = argComm.Port;
            currentValidator.Address = argComm.Address;
            currentValidator.ChecksumType = argComm.ChecksumType;
            currentValidator.EncryptionMode = argComm.EncryptionMode;
            currentValidator.InitPINCode(argComm.GetPINCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int OpenClose()
        {
            if (currentValidator.IsOpen)
            {
                currentValidator.CloseComm();

                if (currentValidator.LastError == whCcTalkErrors.Ok)
                    return 2;
            }
            else
            {
                currentValidator.OpenComm();
                currentValidator.ResetDevice(1000);

                if (currentValidator.LastError == whCcTalkErrors.Ok)
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadValidator()
        {
            currentValidator.GetBillStates(ref SelCurrentStatus);
            currentValidator.GetBillValues(ref SelCurrentValue);

            if (currentValidator.MasterInhibit)
                ibReset = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetValidator()
        {
            currentValidator.MasterInhibit = !currentValidator.MasterInhibit;
        }


        private int WriteValidator()
        {
            ReadValidator();

            inetRegistry local = new inetRegistry();
            //local.SubKey +=  inetConstants.icsVerzMP;
            String[] s = local.ReadValueNames(local.SubKey + "\\" + inetConstants.icsVerzBill);
            int index;
            local.SubKey = local.SubKey + "\\" + inetConstants.icsVerzBill;
            if (s != null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].StartsWith(inetConstants.icsBillMf))
                    {
                        index = Int32.Parse(s[i].Substring(6));
                        if (local.ReadAsDWORD(s[i]) == 1)
                            SelCurrentStatus[index].Inhibit = true;
                        else
                            SelCurrentStatus[index].Inhibit = false;

                    }
                }
            

                try
                {
                    currentValidator.SetBillInhibit(SelCurrentStatus);
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



        public int StartBillPolling()
        {

            inetRegistry local = new inetRegistry();
            String[] s = local.ReadValueNames(local.SubKey + "\\" + inetConstants.icsVerzBill);
            int index;
            local.SubKey = local.SubKey + "\\" + inetConstants.icsVerzBill;
            if (local.ReadAsString(inetConstants.icsBillArt).Equals("0"))
                return 0;

            if (OpenClose() == 1)
            {
                ReadValidator();
                if (ibReset)
                {
                    ResetValidator();
                    ibReset = false;
                }

                if (WriteValidator() == 1)
                {
                    //timerCallBack = new TimerCallback(Polling);
                    //AutoResetEvent autoEvent = new AutoResetEvent(false);

                    //timerBill = new Timer(timerCallBack, autoEvent, 1, 100);
                    //th = new Thread(new ThreadStart(this.Polling));
                    //th.Start();
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

            whValPollResponse[] PollResps = new whValPollResponse[whValidatorComm.MaxPollEvents];

            currentValidator.PollValidator(ref PollResps, out EvtCnt);

            if (EvtCnt > 0)
            {
                for (i = 0; i < EvtCnt; i++)
                {
                    if (PollResps[i].Status == whValPollEvent.Bill)
                    {
                        cidx = PollResps[i].BillIndex;

                        switch (PollResps[i].BillPosition)
                        {
                            case whValBillPosition.Stacked:
                                {
                                    if (!SelCurrentStatus[cidx].Inhibit)
                                        currentValidator.RouteBill(whValBillRoute.Return);
                                    break;
                                }
                            case whValBillPosition.Escrow:
                                {
                                    currentValidator.RouteBill(whValBillRoute.Stack);
                                    credit = SelCurrentValue[cidx].Value;
                                    id = SelCurrentValue[cidx].ID;
                                    break;
                                }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean isReadBillSelector()
        {
            if (OpenClose() == 1)
            {
                ReadValidator();
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
        /// <param name="argValue"></param>
        /// <param name="argStatus"></param>
        public void GetBillValuesAndStatus(ref whBillValue[] argValue, ref whValBillStatus[] argStatus)
        {
            argStatus = SelCurrentStatus;
            argValue = SelCurrentValue;
        }

    }

        /*
        private void readUsbBill()
        {
            if (currentValidator == null)
                return;
            try
            {
                currentValidator.GetBillStates(ref SelCurrentStatus);
                currentValidator.GetBillValues(ref SelCurrentValue);
            }
            catch (Exception ex)
            {
            }


           

        }

        private void SetMasterInhibit()
        {
            if (currentValidator.MasterInhibit)
                currentValidator.MasterInhibit = !currentValidator.MasterInhibit;
        }

        public Boolean isReadUsbBillValidator()
        {
            if (currentValidator == null)
                return false;

            openClose();
            readUsbBill();
            SetMasterInhibit();
            return true;
        }

        public void GetBillValuesAndStatus(ref whBillValue[] argValue, ref whValBillStatus[] argStatus)
        {
            argStatus = SelCurrentStatus;
            argValue = SelCurrentValue;
        }

        public int CloseBill()
        {
            if (currentValidator.CloseComm() == whCcTalkErrors.Ok)
                return 1;

            return 0;
        }

        public void writeBillStates()
        {
            inetRegistry local = new inetRegistry();
            //local.SubKey +=  inetConstants.icsVerzMP;
            String[] s = local.ReadValueNames(local.SubKey + "\\" + inetConstants.icsVerzBill);
            int index;
            local.SubKey = local.SubKey + "\\" + inetConstants.icsVerzBill;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].StartsWith(inetConstants.icsBillMf))
                {
                    index = Int32.Parse(s[i].Substring(6));
                    if (local.ReadAsDWORD(s[i]) == 1)
                         SelCurrentStatus[index].Inhibit = true;
                    else
                         SelCurrentStatus[index].Inhibit = false;
                  
                }
            }

            try
            {
                currentValidator.SetBillInhibit(SelCurrentStatus);
            }
            catch (Exception ex)
            {
            }

            local = null;
        }


        public void Polling(ref double credit)
        {
            int i, j, cidx, EvtCnt;

            whValPollResponse[] PollResps = new whValPollResponse[whValidatorComm.MaxPollEvents];

            currentValidator.PollValidator(ref PollResps, out EvtCnt);

            if (EvtCnt > 0)
            {
                for (i = 0; i < EvtCnt; i++)
                {
                    if (PollResps[i].Status == whValPollEvent.Bill)
                    {
                        cidx = PollResps[i].BillIndex;
                        
                        switch (PollResps[i].BillPosition)
                        {
                            case whValBillPosition.Stacked:
                            {
                                if (!SelCurrentStatus[cidx].Inhibit)
                                    currentValidator.RouteBill(whValBillRoute.Return);
                                break;
                            }
                            case whValBillPosition.Escrow:
                            {
                                currentValidator.RouteBill(whValBillRoute.Stack);
                                credit = SelCurrentValue[cidx].Value;
                                break;
                            }
                        }
                    }
                }
            }
        }
         * **/
  
}
