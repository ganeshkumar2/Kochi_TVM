using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for BNROperationPage.xaml
    /// </summary>
    public partial class BNROperationPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(BNROperationPage).Name);
        List<BillTable> billTable = new List<BillTable>();
        List<Cassette> cassettes = new List<Cassette>();
        List<StackedNotes> stackedNotesListReceived = new List<StackedNotes>();
        int noteincasset1 = 0, noteincasset2 = 0, noteincasset3 = 0;
        int notevalincasset1 = 0, notevalincasset2 = 0, notevalincasset3 = 0;
        int Casette1Billtype = 0, Casette2Billtype = 0, Casette3Billtype = 0;
        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        public BNROperationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
            BNRManager.Instance.PollingAction();
            BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            BNRManager.BNRBillTableInputEvent += new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.Instance.GetBillTableProcess();
            BNRManager.BNRCurrencyStateInputEvent += new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
            BNRManager.Instance.GetCassetteStatus();
            BNRManager.BNRCassetteStatusInputEvent += new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
            lblBNR.Content = Constants.BNRStatus;
            UpdValOnScr();
        }

        private void BNRManager_BNRBillTableInputEvent(List<BillTable> billTables)
        {
            try
            {
                billTable = billTables;
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> BNRManager_BNRBillTableInputEvent() : " + ex.ToString());
            }
        }

        private void BNRManager_BNRCassetteStatusInputEvent(List<Cassette> cassette)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (cassette[0].billType == 0 && cassette[1].billType == 0 && cassette[2].billType == 0)
                    {
                        BNRManager.Instance.GetCassetteStatus();
                    }
                    cassettes = cassette;
                    foreach (Cassette cassetteset in cassettes)
                    {
                        int bill = billTable.Where(x => x.BillType == cassetteset.billType).Select(x => x.DigitBillType).FirstOrDefault();
                        string billval = Convert.ToString(bill);
                        if (cassetteset.cassetteId == 1)
                        {
                            noteincasset1 = cassetteset.billNumber;
                            if (cassetteset.cassetteId == 1 && cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset1 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset1 = bill;
                            }
                            cbNumberDisp1.Items.Clear();
                            for (int i = 1; i <= cassetteset.billNumber; i++)
                            {
                                cbNumberDisp1.Items.Add(i);
                            }
                            lblCassette1.Content = "₹" + notevalincasset1;
                            lblCassette1Add.Content = "₹" + notevalincasset1;
                            lblnoteincasset1.Content = "₹" + notevalincasset1;
                            Casette1Billtype = cassetteset.billType;
                            lblCassette1Count.Content = noteincasset1.ToString();
                            lblCassette1Amount.Content = "₹" + (noteincasset1 * notevalincasset1);
                            if (noteincasset1 > 0)
                            {
                                btnSendBox1.IsEnabled = true;
                                btnSendBox1.Opacity = 1;
                            }
                            else
                            {
                                btnSendBox1.IsEnabled = false;
                                btnSendBox1.Opacity = 0.2;
                            }
                        }
                        if (cassetteset.cassetteId == 2)
                        {
                            noteincasset2 = cassetteset.billNumber;
                            if (cassetteset.cassetteId == 2 && cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset2 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset2 = bill;
                            }
                            cbNumberDisp2.Items.Clear();
                            for (int i = 1; i <= cassetteset.billNumber; i++)
                            {
                                cbNumberDisp2.Items.Add(i);
                            }
                            lblCassette2.Content = "₹" + notevalincasset2;
                            lblCassette2Add.Content = "₹" + notevalincasset2;
                            lblnoteincasset2.Content = "₹" + notevalincasset2;
                            Casette2Billtype = cassetteset.billType;
                            lblCassette2Count.Content = noteincasset2.ToString();
                            lblCassette2Amount.Content = "₹" + (noteincasset2 * notevalincasset2);
                            if (noteincasset2 > 0)
                            {
                                btnSendBox2.IsEnabled = true;
                                btnSendBox2.Opacity = 1;
                            }
                            else
                            {
                                btnSendBox2.IsEnabled = false;
                                btnSendBox2.Opacity = 0.2;
                            }
                        }
                        if (cassetteset.cassetteId == 3)
                        {
                            noteincasset3 = cassetteset.billNumber;
                            if (cassetteset.cassetteId == 3 && cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset3 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset3 = bill;
                            }
                            cbNumberDisp3.Items.Clear();
                            for (int i = 1; i <= cassetteset.billNumber; i++)
                            {
                                cbNumberDisp3.Items.Add(i);
                            }
                            lblCassette3.Content = "₹" + notevalincasset3;
                            lblCassette3Add.Content = "₹" + notevalincasset3;
                            lblnoteincasset3.Content = "₹" + notevalincasset3;
                            Casette3Billtype = cassetteset.billType;
                            lblCassette3Count.Content = noteincasset3.ToString();
                            lblCassette3Amount.Content = "₹" + (noteincasset3 * notevalincasset3);
                            if (noteincasset3 > 0)
                            {
                                btnSendBox3.IsEnabled = true;
                                btnSendBox3.Opacity = 1;
                            }
                            else
                            {
                                btnSendBox3.IsEnabled = false;
                                btnSendBox3.Opacity = 0.2;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error BNROperationPage -> BNRManager_BNRCassetteStatusInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void BNRManager_BNRCurrencyStateInputEvent(List<StackedNotes> stackedNotesListBox)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    stackedNotesListReceived = new List<StackedNotes>();
                    int bill = 0;
                    int billCount = 0;
                    int total = 0;
                    foreach (var stackedNote in stackedNotesListBox)
                    {
                        if (stackedNote.BillNumber > 0)
                        {
                            bill = billTable.Where(x => x.BillType == stackedNote.BillType).Select(x => x.DigitBillType).FirstOrDefault();
                            billCount = stackedNote.BillNumber;

                            if(notevalincasset1 == bill)
                                lblAddCount1.Content = stackedNote.BillNumber.ToString();
                            if (notevalincasset2 == bill)
                                lblAddCount2.Content = stackedNote.BillNumber.ToString();
                            if (notevalincasset3 == bill)
                                lblAddCount3.Content = stackedNote.BillNumber.ToString();

                            total += bill * billCount;
                        }
                    }
                    stackedNotesListReceived = stackedNotesListBox;
                    lblBNRAmount.Content = Convert.ToString(total);
                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> BNRManager_BNRCurrencyStateInputEvent() : " + ex.ToString());
            }
        }

        bool cashboxRemoved = false;
        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                    lblBNR.Content = Constants.BNRStatus;

                    if (state == BNRState.UNLOADED)
                    {
                        UnloadValue();
                    }
                    if (state == BNRState.DISPENSED)
                    {
                        DispenceValue();
                    }
                    if (state == BNRState.DROP_CASSETTE_REMOVED)
                    {
                        cashboxRemoved = true;
                        grdCashBoxRemove.Visibility = Visibility.Visible;
                    }
                    
                    if (state == BNRState.DISABLED)
                    {
                        if (cashboxRemoved)
                        {
                            cashboxRemoved = false;
                            idleTimer.Dispose();
                            grdCashBoxRemove.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error BNROperationPage -> BNRManager_BNRStateInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (idleTimer != null)
                idleTimer.Dispose();

            BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            BNRManager.BNRBillTableInputEvent -= new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
            BNRManager.BNRCassetteStatusInputEvent -= new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.OperationPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
        bool isAccepting = false;
        private void btnAddMoney_Click(object sender, RoutedEventArgs e)
        {
            if (!isAccepting)
            {
                TVMUtility.PlayClick();
                log.Debug("Debug notevalincasset1 : " + notevalincasset1 + " notevalincasset2 : " + notevalincasset2 +" notevalincasset3 : " + notevalincasset3);
                grdMoneyAccept.Visibility = Visibility.Visible;
                isAccepting = true;
                if (notevalincasset1 == 10 && notevalincasset2 == 20 && notevalincasset3 == 50)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x0E;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 10 && notevalincasset2 == 20 && notevalincasset3 == 100)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x16;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 10 && notevalincasset2 == 50 && notevalincasset3 == 100)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x1A;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 20 && notevalincasset2 == 50 && notevalincasset3 == 100)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x1C;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 20 && notevalincasset2 == 100 && notevalincasset3 == 200)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x34;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 50 && notevalincasset2 == 100 && notevalincasset3 == 200)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x38;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
            }
        }

        int unloadedCasset = 0;
        void UnloadValue()
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                if (unloadedCasset == 1)
                {
                    unloadedCasset = 0;
                    MessageBoxOperations.ShowMessage("SENT BOX", "Sent Type : ₹ " + notevalincasset1 + "" + "\nSent Count : " + noteincasset1.ToString() +
                                                                                        "\nSent Amount : ₹ " + noteincasset1 * notevalincasset1, MessageBoxButtonSet.OK);
                }
                else if (unloadedCasset == 2)
                {
                    unloadedCasset = 0;
                    MessageBoxOperations.ShowMessage("SENT BOX", "Sent Type : ₹ " + notevalincasset2 + "" + "\nSent Count : " + noteincasset2.ToString() +
                                                                                        "\nSent Amount : ₹ " + noteincasset2 * notevalincasset2, MessageBoxButtonSet.OK);
                }
                else if (unloadedCasset == 3)
                {
                    unloadedCasset = 0;
                    MessageBoxOperations.ShowMessage("SENT BOX", "Sent Type : ₹ " + notevalincasset3 + "" + "\nSent Count : " + noteincasset3.ToString() +
                                                                                    "\nSent Amount : ₹ " + noteincasset3 * notevalincasset3, MessageBoxButtonSet.OK);
                }
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(300);
                UpdValOnScr();
            }), DispatcherPriority.Background);
        }

        private void btnSendBox1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TVMUtility.PlayClick();
                Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Send To Box", "Do you want to send all notes to cash box?", MessageBoxButtonSet.OKCancel);
                if (messageBoxResult == Custom.MessageBoxResult.OK)
                {
                    if (Constants.BNRStatus == "DISABLED")
                    {
                        if (noteincasset1 != 0)
                        {
                            unloadedCasset = 1;

                            UnloadCassette(1, noteincasset1);

                            long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset1)));
                            if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)DeviceType.Cassette1, (int)UpdateType.Decrease, noteincasset1))
                                if (MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)DeviceType.Cassette1, (int)UpdateType.Decrease, notevalincasset1 * noteincasset1))
                                {
                                    long trxId2 = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_BOX));
                                    if (StockOperations.InsStock(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)DeviceType.Box, (int)UpdateType.Increase, noteincasset1))
                                        if (MoneyOperations.InsMoney(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)DeviceType.Box, (int)UpdateType.Increase, notevalincasset1 * noteincasset1))
                                            if (MoneyOperations.SelMoneyStatus())
                                            {
                                                SendToBox sendToBox = new SendToBox { count = noteincasset1, billType = notevalincasset1 };
                                                Constants.SendToBoxes.Add(sendToBox);
                                                //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                                                //{
                                                //    CustomTL60Printer.Instance.SendBoxNotes(noteincasset1, notevalincasset1);
                                                //}
                                            }
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnSendBox1_Click() : " + ex.ToString());
            }
        }

        private async void btnStopMoney_Click(object sender, RoutedEventArgs e)
        {
            if (isAccepting)
            {
                try
                {
                    lblAddCount1.Content = "";
                    lblAddCount2.Content = "";
                    lblAddCount3.Content = "";
                    lblBNRAmount.Content = "";

                    TVMUtility.PlayClick();
                    grdMoneyAccept.Visibility = Visibility.Hidden;
                    BNRManager.Instance.StopProcess();
                    await Task.Delay(300);
                    BNRManager.Instance.GetCassetteStatus();
                    await Task.Delay(1000);
                    isAccepting = false;

                    int billincast1 = 0, billincast2 = 0, billincast3 = 0;
                    int billincont1 = 0, billincont2 = 0, billincont3 = 0;

                    foreach (var data in stackedNotesListReceived)
                    {
                        if (data.BillNumber != 0)
                        {
                            int bill = 0;
                            bill = billTable.Where(x => x.BillType == data.BillType).Select(x => x.DigitBillType).FirstOrDefault();
                            if (notevalincasset1 == bill || cassettes[0].billType == 24)
                            {
                                billincast1 = bill;
                                billincont1 = data.BillNumber;
                                long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_ADD_BANKNOTE" + bill)));
                                if (StockOperations.InsStock(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette1, (int)UpdateType.Increase, data.BillNumber))
                                    MoneyOperations.InsMoney(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette1, (int)UpdateType.Increase, bill);
                            }
                            else if (notevalincasset2 == bill || cassettes[1].billType == 24)
                            {
                                billincast2 = bill;
                                billincont2 = data.BillNumber;
                                long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_ADD_BANKNOTE" + bill)));
                                if (StockOperations.InsStock(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette2, (int)UpdateType.Increase, data.BillNumber))
                                    MoneyOperations.InsMoney(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette2, (int)UpdateType.Increase, bill);
                            }
                            else if (notevalincasset3 == bill || cassettes[2].billType == 24)
                            {
                                billincast3 = bill;
                                billincont3 = data.BillNumber;
                                long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_ADD_BANKNOTE" + bill)));
                                if (StockOperations.InsStock(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette3, (int)UpdateType.Increase, data.BillNumber))
                                    MoneyOperations.InsMoney(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette3, (int)UpdateType.Increase, bill);
                            }
                        }
                    }

                    if (billincont1 > 0 || billincont2 > 0 || billincont3 > 0)
                    {
                        string msg = "";

                        if (billincont1 > 0)
                            msg = billincast1.ToString();

                        if (billincont2 > 0)
                        {
                            if (billincont1 > 0)
                                msg += " , " + billincast2.ToString();
                            else
                                msg = billincast2.ToString();
                        }

                        if (billincont3 > 0)
                        {
                            if (billincont1 > 0 && billincont2 > 0)
                                msg += " and " + billincast3.ToString();
                            else if (billincont1 == 0 && billincont2 > 0)
                                msg += " , " + billincast3.ToString();
                            else if (billincont1 == 0 && billincont2 == 0)
                                msg = billincast3.ToString();
                        }

                        MessageBoxOperations.ShowMessage("ADD CASH", "Added Type : " + "₹ " + msg +
                                                   "\nAdded Count : " + (billincont1 + billincont2 + billincont3) + "\nAdded Amount : " + "₹ " + ((billincont1* billincast1) + (billincont2 * billincast2) + (billincont3 * billincast3)) + "\n", MessageBoxButtonSet.OK);

                        AddNote addNote = new AddNote { count1 = billincont1, notes1 = billincast1, count2 = billincont2, notes2 = billincast2, count3 = billincont3, notes3 = billincast3 };
                        Constants.AddNotes.Add(addNote);

                        //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                        //{
                        //    CustomTL60Printer.Instance.AddBanknotes(billincont1, billincast1, billincont2, billincast2, billincont3, billincast3);
                        //}
                    }

                    stackedNotesListReceived.Clear();
                }
                catch { }
            }
        }

        private void btnSendBox2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TVMUtility.PlayClick();
                Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Send To Box", "Do you want to send all notes to cash box?", MessageBoxButtonSet.OKCancel);
                if (messageBoxResult == Custom.MessageBoxResult.OK)
                {
                    if (Constants.BNRStatus == "DISABLED")
                    {
                        if (noteincasset2 != 0)
                        {
                            unloadedCasset = 2;

                            UnloadCassette(2, noteincasset2);

                            long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset2)));
                            if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)DeviceType.Cassette2, (int)UpdateType.Decrease, noteincasset2))
                                if (MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)DeviceType.Cassette2, (int)UpdateType.Decrease, notevalincasset2 * noteincasset2))
                                {
                                    long trxId2 = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_BOX));
                                    if (StockOperations.InsStock(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)DeviceType.Box, (int)UpdateType.Increase, noteincasset2))
                                        if (MoneyOperations.InsMoney(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)DeviceType.Box, (int)UpdateType.Increase, notevalincasset2 * noteincasset2))
                                            if (MoneyOperations.SelMoneyStatus())
                                            {
                                                SendToBox sendToBox = new SendToBox { count = noteincasset2, billType = notevalincasset2 };
                                                Constants.SendToBoxes.Add(sendToBox);

                                                //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                                                //{
                                                //    CustomTL60Printer.Instance.SendBoxNotes(noteincasset2, notevalincasset2);
                                                //}
                                            }
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnSendBox2_Click() : " + ex.ToString());
            }            
        }

        private void btnSendBox3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TVMUtility.PlayClick();
                Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Send To Box", "Do you want to send all notes to cash box?", MessageBoxButtonSet.OKCancel);
                if (messageBoxResult == Custom.MessageBoxResult.OK)
                {
                    if (Constants.BNRStatus == "DISABLED")
                    {
                        if (noteincasset3 != 0)
                        {
                            unloadedCasset = 3;

                            UnloadCassette(3, noteincasset3);

                            long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset3)));
                            if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)DeviceType.Cassette3, (int)UpdateType.Decrease, noteincasset3))
                                if (MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)DeviceType.Cassette3, (int)UpdateType.Decrease, notevalincasset3 * noteincasset3))
                                {
                                    long trxId2 = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_BOX));
                                    if (StockOperations.InsStock(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)DeviceType.Box, (int)UpdateType.Increase, noteincasset3))
                                        if (MoneyOperations.InsMoney(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)DeviceType.Box, (int)UpdateType.Increase, notevalincasset3 * noteincasset3))
                                            if (MoneyOperations.SelMoneyStatus())
                                            {
                                                SendToBox sendToBox = new SendToBox { count = noteincasset3, billType = notevalincasset3 };
                                                Constants.SendToBoxes.Add(sendToBox);

                                                //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                                                //{
                                                //    CustomTL60Printer.Instance.SendBoxNotes(noteincasset3, notevalincasset3);
                                                //}
                                            }
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnSendBox3_Click() : " + ex.ToString());
            }
        }

        private void btnClearBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TVMUtility.PlayClick();
                int moneyStock = 0;
                int countStock = 0;
                if (MoneyOperations.SelMoneyStatus())
                {
                    Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Clear Box", "Money Amount : " + (int)MoneyOperations.box, MessageBoxButtonSet.OKCancel);
                    if (messageBoxResult == Custom.MessageBoxResult.OK)
                    {
                        idleTimerDelegate = new TimerCallback(NavigateAction);
                        idleTimer = new Timer(idleTimerDelegate, null, 0, 1000);

                        moneyStock = (int)MoneyOperations.box;
                        countStock = StockOperations.box;
                        if (moneyStock == 0)
                        {
                            MessageBoxOperations.ShowMessage("ATTENTION!!", "Box is empty!", MessageBoxButtonSet.OKCancel);
                            return;
                        }
                        else
                        {
                            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_EMPTY_BOX));
                            if (StockOperations.InsStock(trxId, (int)StockType.Unknown, (int)DeviceType.Box, (int)UpdateType.Empty, 0))
                                MoneyOperations.InsMoney(trxId, (int)StockType.Unknown, (int)DeviceType.Box, (int)UpdateType.Empty, 0);
                            if (MoneyOperations.SelMoneyStatus())
                            {
                                grdCashBoxRemove.Visibility = Visibility.Visible;
                                UpdValOnScr();
                            }
                        }
                    }
                    SendToBox sendToBox = new SendToBox { count = countStock, billType = moneyStock };
                    Constants.Removecashbox.Add(sendToBox);
                    //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                    //{
                    //    CustomTL60Printer.Instance.RemoveCashBoxNotes(moneyStock, countStock);
                    //}
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnClearBox_Click() : " + ex.ToString());
            }
        }

        private void btnDispense_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            grdDispence.Visibility = Visibility.Visible;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            grdDispence.Visibility = Visibility.Collapsed;
        }

        private void btnDispenseAmount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TVMUtility.PlayClick();

                lblmessage.Visibility = Visibility.Hidden;

                if (noteincasset1 == 0 && noteincasset2 == 0 && noteincasset3 == 0)
                {
                    lblmessage.Visibility = Visibility.Visible;
                    lblmessage.Content = "All cassettes are empty";
                    return;
                }

                List<byte> lArrSend = new List<byte>();

                if (cbNumberDisp1.SelectedIndex >= 0)
                {
                    long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset1)));
                    if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, Convert.ToInt16(cbNumberDisp1.SelectedValue)))
                        MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, notevalincasset1 * Convert.ToInt16(cbNumberDisp1.SelectedValue));

                    lArrSend.Add(Convert.ToByte(Casette1Billtype));
                    lArrSend.Add(Convert.ToByte(cbNumberDisp1.SelectedValue));
                }
                if (cbNumberDisp2.SelectedIndex >= 0)
                {
                    long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset2)));
                    if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, Convert.ToInt16(cbNumberDisp2.SelectedValue)))
                        MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, notevalincasset2 * Convert.ToInt16(cbNumberDisp2.SelectedValue));

                    lArrSend.Add(Convert.ToByte(Casette2Billtype));
                    lArrSend.Add(Convert.ToByte(cbNumberDisp2.SelectedValue));
                }
                if (cbNumberDisp3.SelectedIndex >= 0)
                {
                    long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset3)));
                    if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, Convert.ToInt16(cbNumberDisp3.SelectedValue)))
                        MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, notevalincasset3 * Convert.ToInt16(cbNumberDisp3.SelectedValue));

                    lArrSend.Add(Convert.ToByte(Casette3Billtype));
                    lArrSend.Add(Convert.ToByte(cbNumberDisp3.SelectedValue));
                }
                if (lArrSend.Count > 0)
                {
                    byte[] snd_arr = lArrSend.ToArray();
                    DispenseSeqBill(snd_arr);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> GridDispenseSeq_MouseLeftButtonDown() : " + ex.ToString());
            }
        }

        private void NavigateAction(object obj)
        {
            try
            {
                BNRManager.Instance.PollingAction();
            }
            catch (Exception ex)
            {
                log.Error("Error TicketTypePage -> DateTimeTimerAction() : " + ex.ToString());
            }
        }
        private void UpdValOnScr()
        {
            MoneyOperations.SelMoneyStatus();
            StockOperations.SelStockStatus();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblBoxInfo.Content = String.Format("{0}", "₹" + Convert.ToInt32(MoneyOperations.box));
                lblBoxCount.Content = String.Format("{0}", StockOperations.box);
                if (StockOperations.box == 0)
                {
                    btnClearBox.IsEnabled = false;
                    btnClearBox.Opacity = 0.2;
                }
                else
                {
                    btnClearBox.IsEnabled = true;
                    btnClearBox.Opacity = 1;
                }
            }));
        }

        private void UnloadCassette(int cassetteNumber, int count)
        {
            try
            {
                BNRManager.Instance.UnloadCassette(cassetteNumber, count);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRTestPage -> UnloadCassette() : " + ex.ToString());
            }
        }

        private void DispenseSeqBill(byte[] snd_arr)
        {
            try
            {
                BNRManager.Instance.DispenseBillSequnce(snd_arr);
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> DispenseSeqBill() : " + ex.ToString());
            }
        }
        void DispenceValue()
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                grdDispence.Visibility = Visibility.Collapsed;
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(500);
            }), DispatcherPriority.Background);
        }
    }
}
