using CCNET;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.BNR
{
    public class BNRManager
    {
        private static ILog log = LogManager.GetLogger(typeof(BNRManager).Name);
        Answer answer;
        private bool enablePolling = false;
        public Dictionary<int, int> stackedNotes;

        private static System.Threading.Timer checkTimer;
        private static TimerCallback checkTimerDelegate;

        public delegate void BNRStateEventHandler(Enums.BNRState state);
        public static event BNRStateEventHandler BNRStateInputEvent;

        public delegate void BNRCurrencyStateEventHandler(List<StackedNotes> stackedNotes);
        public static event BNRCurrencyStateEventHandler BNRCurrencyStateInputEvent;

        public delegate void BNRBillTableEventHandler(List<BillTable> billTable);
        public static event BNRBillTableEventHandler BNRBillTableInputEvent;

        public delegate void BNRCassetteStatusEventHandler(List<Cassette> cassettes);
        public static event BNRCassetteStatusEventHandler BNRCassetteStatusInputEvent;

        public delegate void BNRExtenedCassetteStatusEventHandler(List<string> extenedcassettes);
        public static event BNRExtenedCassetteStatusEventHandler BNRExtenedCassetteStatusInputEvent;

        private static BNRManager _instance = null;
        public static BNRManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BNRManager();
                return _instance;
            }
        }

        public void Start()
        {

        }

        public BNRManager()
        {
            InitialEvents();
            initialTimers();
        }

        private void initialTimers()
        {
            checkTimerDelegate = new TimerCallback(CheckPollingAction);
            checkTimer = new System.Threading.Timer(checkTimerDelegate, null, 1000, 500);
        }

        private void InitialEvents()
        {
            try
            {
                if (safeICCNET.Instance.SearchAndConnect())
                {
                    Constants.IsBNRAvalable = true;
                    safeICCNET.Instance.RunCommand(CCNETCommand.RESET);
                    Answer ans = new Answer();
                    ans = safeICCNET.Instance.RunCommand(CCNETCommand.Poll);
                    safeICCNET.Instance.OnAnswerReceived += new Iccnet.AnswerReceivedEvent(Answer_Received_Action);
                    InitialStackedNotesDic();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> InitialEvents() - " + ex.ToString());
            }
        }
        private void CheckPollingAction(object o)
        {
            try
            {
                if (enablePolling)
                    safeICCNET.Instance.RunCommand(CCNETCommand.Poll);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> CheckPollingAction() - " + ex.ToString());
            }
        }

        private void Answer_Received_Action(object sender, CCNETEventArgs e)
        {
            try
            {
                if (e.ReceivedAnswer.Error == null)
                {
                    answer = e.ReceivedAnswer;
                    switch (e.SentCommand)
                    {
                        case CCNETCommand.Poll:
                            ParseResponse(answer.Data);
                            break;
                        case CCNETCommand.GET_BILL_TABLE:
                            FillBillTableConsole(ExtractBillTable(answer.Data));
                            break;
                        case CCNETCommand.RECYCLING_CASSETTE_STATUS:
                            if (answer.SendedData[3] == 0x3B && answer.Message.ToLower() != "illegal comand" && answer.ReceivedData != null)
                                FillCassetteStatusTable(ExtractCassetteStatus(answer.Data));
                            break;
                        case CCNETCommand.EXTENDED_CASSETTE_STATUS:
                            if (answer.Message.ToLower() != "illegal comand" && answer.ReceivedData != null)
                                ExtenedCassetteStatus(answer.Message);
                            break;
                        case CCNETCommand.SET_RECYCLING_CASSETTE_TYPE:
                            if (answer.Data[0] == 0x00)
                            {
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            }
                            break;
                        case CCNETCommand.ENABLE_BILL_TYPES:
                            if (answer.Data[0] == 0x00)
                            {
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            }
                            break;
                        case CCNETCommand.UNLOAD:
                            if (answer.Data[0] == 0x00)
                            {
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            }
                            else
                            {

                            }
                            break;
                        case CCNETCommand.DISPENSE:
                            if (answer.Data[0] == 0x00)
                            {
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> Answer_Received_Action() - " + ex.ToString());
            }

        }

        private Enums.BNRState ParseResponse(byte[] data)
        {
            Enums.BNRState state = Enums.BNRState.UNKNOWN;
            try
            {
                if (data != null)
                {
                    state = (Enums.BNRState)data[0];
                    log.Debug("BNR State is -> " + state.ToString());
                    switch (state)
                    {
                        case BNRState.POWER_UP:
                            if (!enablePolling)
                            {
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.RESET)).Start();
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            }
                            enablePolling = true;
                            break;
                        case BNRState.INITIALIZE:
                            enablePolling = true;
                            break;
                        case BNRState.DISABLED:
                            enablePolling = false;
                            break;
                        case BNRState.SETTING_TYPE_CASSETE:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.SET_CASSETTE_TYPE:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.ACCEPTING:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.STACKING:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.PACKED_STACKED:
                            log.Debug("BNRState.PACKED_STACKED : " + (int)data[1]);
                            stackedNotes[(int)data[1]] += 1;
                            FillStackedNotes(stackedNotes);
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.EXTENDED_CASSETTE_STATUS:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.IDLING:
                            enablePolling = true;
                            break;
                        case BNRState.UNLOADING:
                            //if (!enablePolling)
                            enablePolling = true;
                            //new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.UNLOADED:
                            int unloadedNotesNumber = (int)data[1];
                            //if (!enablePolling)
                            enablePolling = true;
                            //new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.DISPENSING:
                            //if (!enablePolling)
                            enablePolling = true;
                            //new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.DISPENSED:
                            //if (!enablePolling)
                            enablePolling = true;
                            //new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.INVALID_BILL_NUMBER:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                        case BNRState.UNKNOWN:
                            if (!enablePolling)
                                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.Poll)).Start();
                            break;
                    }
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                    if (BNRStateInputEvent != null)
                        BNRStateInputEvent(state);
                }
            }
            catch (Exception ex)
            {
                Constants.BNRStatus = "";
                log.Error("Error BNRManager -> ParseResponse() - " + ex.ToString());
            }
            return state;
        }

        private void FillStackedNotes(Dictionary<int, int> stackedNotesDic)
        {
            try
            {
                List<StackedNotes> stackedNotesListBox = new List<StackedNotes>();
                foreach (int billType in stackedNotesDic.Keys)
                {
                    StackedNotes stackedNotes = new StackedNotes
                    {
                        BillNumber = stackedNotesDic[billType],
                        BillType = billType
                    };
                    stackedNotesListBox.Add(stackedNotes);
                }
                if (BNRCurrencyStateInputEvent != null)
                    BNRCurrencyStateInputEvent(stackedNotesListBox);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> FillStackedNotes() - " + ex.ToString());
            }
        }

        public void InitialStackedNotesDic()
        {
            try
            {
                stackedNotes = new Dictionary<int, int>();

                for (int i = 0; i <= 7; i++)
                {
                    stackedNotes.Add(i, 0);
                }
                FillStackedNotes(stackedNotes);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> InitialStackedNotesDic() - " + ex.ToString());
            }
        }

        public void FillBillTableConsole(BillTable[] billTables)
        {
            try
            {
                List<BillTable> billTableListBox = new List<BillTable>();
                foreach (BillTable b in billTables)
                {
                    if (b.CountryCode != null)
                    {
                        billTableListBox.Add(b);
                    }
                }
                if (BNRBillTableInputEvent != null)
                    BNRBillTableInputEvent(billTableListBox);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> FillBillTableConsole() - " + ex.ToString());
            }
        }

        public void ExtenedCassetteStatus(string stingAnswer)
        {
            try
            {
                log.Debug("Debug BNRManager -> ExtenedCassetteStatus() - stingAnswer " + stingAnswer);
                char spilt = '\n';
                List<string> stingList = stingAnswer.Split(spilt).ToList();
                log.Debug("Debug BNRManager -> ExtenedCassetteStatus() - stingList " + stingList.Count);
                if (BNRExtenedCassetteStatusInputEvent != null)
                    BNRExtenedCassetteStatusInputEvent(stingList);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> ExtenedCassetteStatus() - " + ex.ToString());
            }
        }

        public void FillCassetteStatusTable(Cassette[] cassettes)
        {
            try
            {
                List<Cassette> cassetteStatusListBox = new List<Cassette>();
                foreach (Cassette cassette in cassettes)
                {
                    log.Debug("Debug BNRManager -> CassetteStatus() - billType : " + cassette.billType + " billNumber : " + cassette.billNumber);
                    cassetteStatusListBox.Add(cassette);
                }
                if (BNRCassetteStatusInputEvent != null)
                    BNRCassetteStatusInputEvent(cassetteStatusListBox);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> FillCassetteStatusTable() - " + ex.ToString());
            }
        }

        private BillTable[] ExtractBillTable(byte[] Data)
        {
            BillTable[] billtable = new BillTable[24];
            //if (Data != null && Data.Length == 125)
            //{
            //    int p = 0;
            //    for (int i = 3; i < 122; i++)
            //    {
            //        billtable[p].CountryCode = null;
            //        billtable[p].CountryCode += Encoding.ASCII.GetString(Data, i + 1, 3);
            //        switch (Data[i + 4])
            //        {
            //            case 0x00:
            //                billtable[p].DigitBillType = Data[i];
            //                billtable[p].BillType = p;
            //                break;
            //            case 0x01:
            //                billtable[p].DigitBillType = Data[i] * 10;
            //                billtable[p].BillType = p;
            //                break;
            //            case 0x02:
            //                billtable[p].DigitBillType = Data[i] * 100;
            //                billtable[p].BillType = p;
            //                break;
            //            case 0x03:
            //                billtable[p].DigitBillType = Data[i] * 1000;
            //                billtable[p].BillType = p;
            //                break;
            //            case 0x04:
            //                billtable[p].DigitBillType = Data[i] * 10000;
            //                billtable[p].BillType = p;
            //                break;
            //            case 0x05:
            //                billtable[p].DigitBillType = Data[i] * 100000;
            //                billtable[p].BillType = p;
            //                break;
            //            case 0x06:
            //                billtable[p].DigitBillType = Data[i] * 1000000;
            //                billtable[p].BillType = p;
            //                break;
            //            default:
            //                break;
            //        }

            //        i = i + 4;
            //        p++;
            //    }
            //}
            //else
            //{
            //    if (Data != null)
            //    {
            //        for (int i = 0; i < 24; i++)
            //        {
            //            billtable[i].CountryCode = null;
            //            billtable[i].DigitBillType = 0;
            //            billtable[i].BillType = 0;
            //        }
            //    }
            //}

            if (Data != null && Data.Length == 120)
            {
                int p = 0;
                for (int i = 0; i < 120; i++)
                {
                    billtable[p].CountryCode = null;
                    billtable[p].CountryCode += Encoding.ASCII.GetString(Data, i + 1, 3);
                    switch (Data[i + 4])
                    {
                        case 0x00:
                            billtable[p].DigitBillType = Data[i];
                            billtable[p].BillType = p;
                            break;
                        case 0x01:
                            billtable[p].DigitBillType = Data[i] * 10;
                            billtable[p].BillType = p;
                            break;
                        case 0x02:
                            billtable[p].DigitBillType = Data[i] * 100;
                            billtable[p].BillType = p;
                            break;
                        case 0x03:
                            billtable[p].DigitBillType = Data[i] * 1000;
                            billtable[p].BillType = p;
                            break;
                        case 0x04:
                            billtable[p].DigitBillType = Data[i] * 10000;
                            billtable[p].BillType = p;
                            break;
                        case 0x05:
                            billtable[p].DigitBillType = Data[i] * 100000;
                            billtable[p].BillType = p;
                            break;
                        case 0x06:
                            billtable[p].DigitBillType = Data[i] * 1000000;
                            billtable[p].BillType = p;
                            break;
                        default:
                            break;
                    }

                    i = i + 4;
                    p++;
                }
            }
            else
            {
                if (Data != null)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        billtable[i].CountryCode = null;
                        billtable[i].DigitBillType = 0;
                        billtable[i].BillType = 0;
                    }
                }
            }
            return billtable;
        }

        private Cassette[] ExtractCassetteStatus(byte[] Data)
        {
            Cassette[] cassettes = new Cassette[3];
            try
            {
                if (Data != null && Data.Length > 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        cassettes[i].cassetteId = i + 1;
                        cassettes[i].billType = (int)(Data[2 * i] & 0x1F);
                        cassettes[i].billNumber = (int)Data[(2 * i) + 1];
                        cassettes[i].presence = ((Data[2 * i] & 0x80) == 0x80) ? true : false;
                        cassettes[i].cassetteFull = ((Data[2 * i] & 0x40) == 0x40) ? true : false;
                        if ((Data[2 * i] & 0x1F) == 0x1F)
                        {
                            cassettes[i].cassetteStatus = CassetteStatus.NOT_ASSIGNED;
                        }
                        else if ((Data[2 * i] & 0x1F) == 0x18)
                        {
                            cassettes[i].cassetteStatus = CassetteStatus.ESCROW;
                            Constants.EscrowCassetteNo = i + 1;
                        }
                        else
                        {
                            cassettes[i].cassetteStatus = CassetteStatus.NORMAL;
                            if (i == 0)
                            {
                                Constants.Cassette1Note = cassettes[0].billType;
                                Constants.Cassette1NoteCont = cassettes[0].billNumber;
                            }
                            if (i == 1)
                            {
                                Constants.Cassette1Note = cassettes[1].billType;
                                Constants.Cassette2NoteCont = cassettes[1].billNumber;
                            }
                            if (i == 2)
                            {
                                Constants.Cassette1Note = cassettes[2].billType;
                                Constants.Cassette3NoteCont = cassettes[2].billNumber;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> ExtractCassetteStatus() - " + ex.ToString());
            }
            Constants.CassettesInfo = cassettes;
            return cassettes;
        }

        public void AcceptProcess(byte[] arr_enable)
        {
            try
            {
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.ENABLE_BILL_TYPES, arr_enable, true)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> AcceptProcess() : " + ex.ToString());
            }
        }

        public void StopProcess()
        {
            try
            {
                byte[] arr_enable = new byte[6];
                arr_enable[0] = 0x00;
                arr_enable[1] = 0x00;
                arr_enable[2] = 0x00;
                arr_enable[3] = 0x00;
                arr_enable[4] = 0x00;
                arr_enable[5] = 0x00;
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.ENABLE_BILL_TYPES, arr_enable, true)).Start();
                stackedNotes = new Dictionary<int, int>();
                for (int i = 0; i <= 7; i++)
                {
                    stackedNotes.Add(i, 0);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> StopProcess() : " + ex.ToString());
            }
        }

        public void ReturnProcess()
        {
            try
            {
                byte[] arr_enable = new byte[6];
                arr_enable[0] = 0x00;
                arr_enable[1] = 0x00;
                arr_enable[2] = 0x00;
                arr_enable[3] = 0x00;
                arr_enable[4] = 0x00;
                arr_enable[5] = 0x00;
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.RETURN, arr_enable, true)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> AcceptProcess() : " + ex.ToString());
            }
        }

        public void GetBillTableProcess()
        {
            try
            {
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.GET_BILL_TABLE)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> GetBillTableProcess() - " + ex.ToString());
            }
        }

        public void GetExtendedCassetteStatus()
        {
            try
            {
                byte[] snd_arr = new byte[1];
                snd_arr[0] = (byte)(Constants.EscrowCassetteNo);
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.EXTENDED_CASSETTE_STATUS, snd_arr)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> GetExtendedCassetteStatus() - " + ex.ToString());
            }
        }

        public void GetCassetteStatus()
        {
            try
            {
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.RECYCLING_CASSETTE_STATUS)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> GetCassetteStatus() - " + ex.ToString());
            }
        }

        public void DispenseBill(int billType, int count)
        {
            try
            {
                byte[] snd_arr = new byte[2];
                snd_arr[0] = (byte)(billType);
                snd_arr[1] = (byte)(count);
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.DISPENSE, snd_arr)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> DispenseBill() - " + ex.ToString());
            }
        }

        public void DispenseBillSequnce(byte[] snd_arr)
        {
            try
            {
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.DISPENSE, snd_arr)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> DispenseBillSequnce() - " + ex.ToString());
            }
        }

        public void UnloadCassetteSequnce(byte[] snd_arr)
        {
            try
            {
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.UNLOAD, snd_arr)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManage -> UnloadCassette() : " + ex.ToString());
            }
        }

        public void UnloadCassette(int cassetteNumber, int count)
        {
            try
            {
                log.Debug("Debug BNRManage -> UnloadCassette() : cassetteNumber : " + cassetteNumber + " count : " + count);
                byte[] snd_arr = new byte[2];
                snd_arr[0] = (byte)(cassetteNumber);
                snd_arr[1] = (byte)(count);
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.UNLOAD, snd_arr)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManage -> UnloadCassette() : " + ex.ToString());
            }
        }

        public void SetCassetteType()
        {
            try
            {

                byte[] snd_arr = new byte[6];

                snd_arr[0] = (byte)(1);
                snd_arr[1] = (byte)(2);

                snd_arr[2] = (byte)(2);
                snd_arr[3] = (byte)(3);

                snd_arr[4] = (byte)(3);
                snd_arr[5] = 0x18;

                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.SET_RECYCLING_CASSETTE_TYPE, snd_arr)).Start();

            }
            catch (Exception ex)
            {
                log.Error("Error BNRManage -> SetCassetteType() : " + ex.ToString());
            }
        }

        public void PollingAction()
        {
            try
            {
                safeICCNET.Instance.RunCommand(CCNETCommand.Poll);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRManager -> CheckPollingAction() - " + ex.ToString());
            }
        }
    }
}
