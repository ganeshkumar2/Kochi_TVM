using dk.CctalkLib.Devices;
using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.RptDispenser;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for PayByCashPage.xaml
    /// </summary>
    public partial class PayByCashPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(PayByCashPage).Name);
        DispatcherTimer dt = new DispatcherTimer();
        private static Timer checkTranTimer;
        private static TimerCallback checkTranTimerDelegate;

        decimal TotalAmountToCollect;

        decimal receivedNote = 0;

        List<BillTable> billTable = new List<BillTable>();
        List<Cassette> cassettes = new List<Cassette>();
        List<StackedNotes> stackedNotesRecived = new List<StackedNotes>();
        List<StackedNotes> stackedNotesListReceived = new List<StackedNotes>();
        public static int TranCancelTimer = 60;
        public PayByCashPage()
        {
            InitializeComponent();
            StockOperations.SelStockStatus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.OrderPreviewPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            this.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (Constants.BNRStatus != "IDLING")
                    {
                        await Task.Delay(8000);
                    }
                    log.Info("PayByCashOrCoinPage - dispatcherTimer_Tick");
                    DisposePage();
                    await Task.Delay(500);
                    isReturn = true;
                    isCancel = true;
                    await Task.Delay(500);
                    byte[] snd_arr = await getDispence();
                    if (snd_arr != null && snd_arr.Length > 0)
                    {
                        waitGrid.Visibility = Visibility.Hidden;
                        mainGrid.Visibility = Visibility.Hidden;
                        returnAmountTxt.Content = "₹" + Convert.ToString(receivedNote);
                        ElectronicJournal.NoteReturned(Convert.ToInt32(receivedNote));
                        cashGrid.Visibility = Visibility.Visible;
                        DispenseSeqBill(snd_arr);
                    }
                    else
                    {
                        ElectronicJournal.OrderCancelled();
                        NavigationService.Navigate(new Pages.MainPage());
                    }
                    checkTranTimer.Dispose();
                }
                catch (Exception ex1)
                {
                    log.Error("Error PayByCashPage -> dispatcherTimer_Tick() : " + ex1.ToString());
                }
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {            
            myGif.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\getting_money.gif");
            returnCashImageGif.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\giving_money.gif");
            returnCashImageGif1.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\giving_money.gif");
            loadingImageGif.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Spinner.gif");

            if (Ticket.language == Languages.English)
            {
                lblTotalAmountKey.FontSize = 24;
                lblPaidAmountKey.FontSize = 24;
                lblChangeAmountKey.FontSize = 24;
                lblNotes.FontSize = 24;
                btnBack.FontSize = 24;
                btnFinish.FontSize = 24;
                lblSaleUnSucc.FontSize = 24;
                lblPleaseCollect.FontSize = 24;
                lblMaxAccept.FontSize = 20;
            }
            else
            {
                lblTotalAmountKey.FontSize = 18;
                lblPaidAmountKey.FontSize = 18;
                lblChangeAmountKey.FontSize = 18;
                lblNotes.FontSize = 18;
                btnBack.FontSize = 18;
                btnFinish.FontSize = 18;
                lblSaleUnSucc.FontSize = 18;
                lblPleaseCollect.FontSize = 18;
                lblMaxAccept.FontSize = 16;
            }

            lblTotalAmountValue.FontSize = 24;
            lblPaidAmountValue.FontSize = 24;
            lblChangeAmountValue.FontSize = 24;
            lblMaxNoteAccept.FontSize = 22;

            lblMaxNoteAccept.Content = 20;           

            lblMaxAccept.Content = MultiLanguage.GetText("MaxNoteAccept");
            lblTotalAmountKey.Content = MultiLanguage.GetText("totalPrice");
            lblPaidAmountKey.Content = MultiLanguage.GetText("insertedAmount");
            lblChangeAmountKey.Content = MultiLanguage.GetText("deficitAmount");
            lblNotes.Content = MultiLanguage.GetText("acceptableNotes");
            btnBack.Content = MultiLanguage.GetText("back");
            btnFinish.Content = MultiLanguage.GetText("cancel");
            lblSaleUnSucc.Content = MultiLanguage.GetText("ticketSaleUnsucces");
            lblPleaseCollect.Content = MultiLanguage.GetText("collectCash");

            lblTotalAmountValue.Content = String.Format("{0}", Conversion.MoneyFormat(Ticket.totalPrice));
            lblPaidAmountValue.Content = String.Format("{0}", Conversion.MoneyFormat(0));
            lblChangeAmountValue.Content = String.Format("{0}", Conversion.MoneyFormat(0));
            lblRemain.Content = Parameters.TVMDynamic.GetParameter("sys_CashPayTime");
            TranCancelTimer = Convert.ToInt16(Parameters.TVMDynamic.GetParameter("sys_CashPayTime"));
            //lblMoney5.Visibility = Visibility.Collapsed;
            grdMoney10.Visibility = Visibility.Collapsed;
            grdMoney20.Visibility = Visibility.Collapsed;
            grdMoney50.Visibility = Visibility.Collapsed;
            grdMoney100.Visibility = Visibility.Collapsed;
            grdMoney200.Visibility = Visibility.Collapsed;
            grdMoney500.Visibility = Visibility.Collapsed;
            grdMoney2000.Visibility = Visibility.Collapsed;

            //StockOperations.SelStockStatus();
            //

            switch (Ticket.journeyType)
            {
                case JourneyType.Day_Pass:
                case JourneyType.Weekend_Pass:
                    lblType.Content = Ticket.journeyTypeText.ToString();
                    lblDestination.Visibility = Visibility.Collapsed;
                    arrorDest.Visibility = Visibility.Collapsed;
                    lblNoOfTickets.Content = Ticket.ticketCount;
                    lblAmount.Content = Conversion.MoneyFormat(Ticket.totalPrice);
                    break;
                case JourneyType.Group_Ticket:
                    lblType.Content = Ticket.journeyTypeText.ToString();
                    lblDestination.Content = MultiLanguage.GetText(Ticket.endStation.name);
                    lblNoOfTickets.Content = Ticket.ticketCount;
                    lblAmount.Content = Conversion.MoneyFormat(Ticket.totalPrice);
                    break;
                case JourneyType.RJT:
                case JourneyType.SJT:
                    lblType.Content = Ticket.journeyTypeText.ToString();
                    lblDestination.Content = MultiLanguage.GetText(Ticket.endStation.name);
                    lblNoOfTickets.Content = Ticket.ticketCount;
                    lblAmount.Content = Conversion.MoneyFormat(Ticket.totalPrice);
                    break;
                default:
                    break;
            }

            if ((Constants.Cassette1NoteCont <= Constants.NoChangeAvailable && Constants.Cassette2NoteCont <= Constants.NoChangeAvailable && Constants.Cassette3NoteCont <= Constants.NoChangeAvailable) || (StockOperations.coin1 <= Constants.NoChangeAvailable && StockOperations.coin2 <= Constants.NoChangeAvailable && StockOperations.coin1 <= Constants.NoChangeAvailable))
            {
                if (Ticket.totalPrice > 5 && Ticket.totalPrice <= 10)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Visible;
                    grdMoney50.Visibility = Visibility.Visible;
                    grdMoney100.Visibility = Visibility.Visible;
                    grdMoney200.Visibility = Visibility.Visible;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice > 10 && Ticket.totalPrice <= 20)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Visible;
                    grdMoney100.Visibility = Visibility.Visible;
                    grdMoney200.Visibility = Visibility.Visible;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice > 20 && Ticket.totalPrice <= 50)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Visible;
                    grdMoney200.Visibility = Visibility.Visible;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice > 50 && Ticket.totalPrice <= 100)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Visible;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice > 100 && Ticket.totalPrice <= 200)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    lblMoney200.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Collapsed;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice > 200 && Ticket.totalPrice <= 1500)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    lblMoney200.Visibility = Visibility.Visible;
                    lblMoney500.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Collapsed;
                    grdMoney500.Visibility = Visibility.Collapsed;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice > 1500)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    lblMoney200.Visibility = Visibility.Visible;
                    lblMoney500.Visibility = Visibility.Visible;
                    lblMoney2000.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Collapsed;
                    grdMoney500.Visibility = Visibility.Collapsed;
                    grdMoney2000.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (Ticket.totalPrice < 50)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Visible;
                    grdMoney200.Visibility = Visibility.Visible;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                if (Ticket.totalPrice >= 50 && Ticket.totalPrice <= 100)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Visible;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                else if (Ticket.totalPrice > 100 && Ticket.totalPrice <= 200)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    lblMoney200.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Collapsed;
                    grdMoney500.Visibility = Visibility.Visible;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                else if (Ticket.totalPrice > 200 && Ticket.totalPrice <= 1500)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    lblMoney200.Visibility = Visibility.Visible;
                    lblMoney500.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Collapsed;
                    grdMoney500.Visibility = Visibility.Collapsed;
                    grdMoney2000.Visibility = Visibility.Visible;
                }
                else if (Ticket.totalPrice > 1500)
                {
                    lblMoney10.Visibility = Visibility.Visible;
                    lblMoney20.Visibility = Visibility.Visible;
                    lblMoney50.Visibility = Visibility.Visible;
                    lblMoney100.Visibility = Visibility.Visible;
                    lblMoney200.Visibility = Visibility.Visible;
                    lblMoney500.Visibility = Visibility.Visible;
                    lblMoney2000.Visibility = Visibility.Visible;
                    grdMoney10.Visibility = Visibility.Collapsed;
                    grdMoney20.Visibility = Visibility.Collapsed;
                    grdMoney50.Visibility = Visibility.Collapsed;
                    grdMoney100.Visibility = Visibility.Collapsed;
                    grdMoney200.Visibility = Visibility.Collapsed;
                    grdMoney500.Visibility = Visibility.Collapsed;
                    grdMoney2000.Visibility = Visibility.Collapsed;
                }
            }            

            try
            {
                checkTranTimerDelegate = new TimerCallback(dispatcherTimer_Tick);
                checkTranTimer = new Timer(checkTranTimerDelegate, null, TranCancelTimer * 1000, 0);
            }
            catch (Exception ex2)
            {

            }
            PRINTER_STATE QRStatus = QRPrinter.Instance.CheckQrPrinterStatus();//CustomKPM150HPrinter.Instance.getStatusWithUsb();
            if (QRStatus != PRINTER_STATE.OK)
            {
                MessageBoxOperations.ShowMessage("QR Printer", "QR Printer Error.", MessageBoxButtonSet.OK);
                return;
            }
            Message();
            IniEvent();
        }

        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(7, null, Convert.ToString(Convert.ToInt16(Ticket.totalPrice)), "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(7, null, Convert.ToString(Convert.ToInt16(Ticket.totalPrice)), "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(7, null, Convert.ToString(Convert.ToInt16(Ticket.totalPrice)), "IN");
            }
        }
        private async void IniEvent()
        {
            try
            {
                LedOperations.GreenText("CASH PAYMENT IN PROGRESS");
                TotalAmountToCollect = Ticket.totalPrice;
                BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
                BNRManager.BNRBillTableInputEvent += new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
                BNRManager.BNRCurrencyStateInputEvent += new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                BNRManager.BNRCassetteStatusInputEvent += new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
                BNRManager.BNRExtenedCassetteStatusInputEvent += new BNRManager.BNRExtenedCassetteStatusEventHandler(BNRManager_BNRExtenedCassetteStatusEvent);
                BNRManager.Instance.GetBillTableProcess();
                await Task.Delay(500);
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(200);
                BNRManager.Instance.AcceptProcess(Constants.EnableBillNotes);
                await Task.Delay(300);
                if (Constants.BNRStatus != "IDLING")
                    BNRManager.Instance.AcceptProcess(Constants.EnableBillNotes);
            }
            catch (Exception ex3)
            {

            }
            stackedNotesRecived = new List<StackedNotes>();
           
            Dispatcher.Invoke((Action)(() =>
            {
                Countdown(TranCancelTimer, TimeSpan.FromSeconds(1), cur => lblRemain.Content = cur.ToString());
            }
            ));
        }
        private void BNRManager_BNRCurrencyStateInputEvent(List<StackedNotes> stackedNotesListBox)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                try
                {
                    stackedNotesListReceived = new List<StackedNotes>();
                    int bill = 0;
                    int billCount = 0;
                    int total = 0;
                    stackedNotesRecived.Clear();
                    foreach (var stackedNote in stackedNotesListBox)
                    {
                        if (stackedNote.BillNumber > 0)
                        {
                            bill = billTable.Where(x => x.BillType == stackedNote.BillType).Select(x => x.DigitBillType).FirstOrDefault();
                            billCount = stackedNote.BillNumber;
                            log.Debug("bill : " + bill);
                            log.Debug("billCount" + billCount);
                            total += bill * billCount;
                            StackedNotes stackedNotesRec = new StackedNotes { BillNumber = stackedNote.BillNumber, BillType = stackedNote.BillType };
                            stackedNotesRecived.Add(stackedNotesRec);
                            log.Debug("Stacked Note -> " + "Bill Type : " + stackedNote.BillType);
                        }
                    }
                    stackedNotesListReceived = stackedNotesListBox;
                    log.Debug("bill = " + bill + " && billCount = " + billCount);
                    if (bill > 0 && billCount > 0)
                    {
                        log.Debug("bill > 0 && billCount > 0");
                        receivedNote = total;
                        CashRecevied(Convert.ToString(total));
                    }
                }
                catch (Exception ex1)
                {
                    log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRCurrencyStateInputEvent() : " + ex1.ToString());
                }
            }));
        }

        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                    log.Debug("BNR Status : " + Constants.BNRStatus);
                    txtBNRStatus.Text = Constants.BNRStatus;
                    if (state == BNRState.DISABLED && !disableMyself)
                    {
                        log.Debug("State is Disabled and We should start Accepting again");
                        Thread.Sleep(200);
                        BNRManager.Instance.AcceptProcess(Constants.EnableBillNotes);
                    }

                    if (state == BNRState.UNLOADED)
                    {
                        UnloadValue();
                    }

                    if (state == BNRState.DISPENSING)
                    {
                        DispenceingValue();
                    }

                    if (state == BNRState.DISPENSED)
                    {
                        i = 0;
                        DispenceValue();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRStateInputEvent : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void BNRManager_BNRBillTableInputEvent(List<BillTable> billTables)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    billTable = billTables;
                }
                catch (Exception ex)
                {
                    log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRBillTableInputEvent : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void BNRManager_BNRCassetteStatusInputEvent(List<Cassette> cassette)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    cassettes = cassette;
                    log.Debug("Debug BNRManager_BNRCassetteStatusInputEvent -> Countdown() : " + cassettes.Count + cassettes[0].billNumber + cassettes[1].billNumber + cassettes[2].billNumber);
                }
                catch (Exception ex)
                {
                    log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRCassetteStatusInputEvent : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        int unloadbillNumber = 0;
        List<int> noteunload = new List<int>();

        private void BNRManager_BNRExtenedCassetteStatusEvent(List<string> extenedCassetteStatus)
        {
            try
            {
                log.Debug("Debug PayByCashOrCoinPage -> BNRManager_BNRExtenedCassetteStatusEvent : extened Cassette Status : " + extenedCassetteStatus.Count());
                unloadbillNumber = 0;
                if (extenedCassetteStatus.Count > 0)
                {
                    noteunload = new List<int>();
                    List<string> extenedCassette = extenedCassetteStatus.ToList();
                    extenedCassette.RemoveAt(0);
                    int billtype = billTable.Where(x => x.DigitBillType == Constants.EscrowAmount).Select(x => x.BillType).FirstOrDefault();
                    if (extenedCassette.Contains("Bill type ->" + billtype))
                    {
                        if (extenedCassette.Count > 1)
                        {
                            int billNumber = 0;

                            for (int i = 0; i < extenedCassette.Count; i++)
                            {
                                for (int j = 0; j < i; j++)
                                {
                                    if (extenedCassette[i] != extenedCassette[j])
                                    {
                                        billNumber++;
                                        string str = extenedCassette[i];
                                        string substr = str.Substring(str.Length - 1);
                                        int bill = billTable.Where(x => x.BillType == Convert.ToInt16(substr)).Select(x => x.DigitBillType).FirstOrDefault();
                                        noteunload.Add(bill);
                                        break;
                                    }
                                }
                            }

                            log.Debug("Debug PayByCashOrCoinPage -> BNRManager_BNRExtenedCassetteStatusEvent : bill Number : " + billNumber);

                            if (billNumber != 0)
                                unloadbillNumber = billNumber;
                            //BNRManager.Instance.UnloadCassette(Constants.EscrowCassetteNo, billNumber);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < extenedCassette.Count; i++)
                        {
                            string str = extenedCassette[i];
                            string substr = str.Substring(str.Length - 1);
                            int bill = billTable.Where(x => x.BillType == Convert.ToInt16(substr)).Select(x => x.DigitBillType).FirstOrDefault();
                            noteunload.Add(bill);
                        }

                        //BNRManager.Instance.UnloadCassette(Constants.EscrowCassetteNo, extenedCassette.Count);
                        unloadbillNumber = extenedCassette.Count;
                        log.Debug("Debug PayByCashOrCoinPage -> BNRManager_BNRExtenedCassetteStatusEvent : UnloadCassette extenedCassette : " + extenedCassette.Count);
                    }
                }

                if (unloadbillNumber > 0)
                {
                    foreach (var billVal in noteunload)
                    {
                        long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + billVal)));
                        if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + billVal), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, 1))
                            if (MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + billVal), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, billVal * 1))
                            {
                                long trxId2 = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_BOX));
                                if (StockOperations.InsStock(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + billVal), (int)DeviceType.Box, (int)UpdateType.Increase, 1))
                                    MoneyOperations.InsMoney(trxId2, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + billVal), (int)DeviceType.Box, (int)UpdateType.Increase, billVal * 1);

                            }
                    }

                    BNRManager.Instance.UnloadCassette(Constants.EscrowCassetteNo, unloadbillNumber);                  
                }
                else
                {
                    UnloadValue();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRExtenedCassetteStatusEvent : " + ex.ToString());
            }
        }

        EventHandler handler;

        void Countdown(int count, TimeSpan interval, Action<int> ts)
        {
            try
            {
                dt.Stop();
                dt.Tick -= handler;
                dt = new DispatcherTimer();
                dt.Interval = interval;
                dt.Tick += (_, a) =>
                {
                    if (count-- == 0)
                        dt.Stop();
                    else
                        ts(count);


                    if (Constants.BNRStatus == "IDLING")
                    {
                        btnFinish.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnFinish.Visibility = Visibility.Hidden;
                    }

                    if (count <= 2)
                    {
                        if (Constants.BNRStatus != "IDLING")
                        {
                            if (Constants.BNRStatus != "REJECTING")
                            {
                                checkTranTimer.Dispose();
                            }
                        }
                    }

                    if(count == 0)
                    {
                        if (Constants.BNRStatus == "IDLING" || Constants.BNRStatus == "REJECTING")
                        {
                            DisposePage();
                        }
                    }
                };
                ts(count);
                dt.Start();
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashOrCoinPage -> Countdown() : " + ex.ToString());
            }
        }
        private void dispatcherTimer_Tick(object o)
        {
            try
            {
                this.Dispatcher.Invoke(async () =>
                {
                    try
                    {                         
                        await Task.Delay(3000);
                        log.Info("PayByCashOrCoinPage - dispatcherTimer_Tick");
                        DisposePage();
                        await Task.Delay(300);
                        isReturn = true;
                        isCancel = true;
                        if (!dispenseMyself)
                        {
                            await Task.Delay(300);

                            byte[] snd_arr = await getDispence();
                            if (snd_arr != null && snd_arr.Length > 0)
                            {
                                waitGrid.Visibility = Visibility.Hidden;
                                mainGrid.Visibility = Visibility.Hidden;
                                returnAmountTxt.Content = "₹" + Convert.ToString(receivedNote);
                                ElectronicJournal.NoteReturned(Convert.ToInt32(receivedNote));
                                cashGrid.Visibility = Visibility.Visible;
                                DispenseSeqBill(snd_arr);
                            }
                            else
                            {
                                ElectronicJournal.OrderCancelled();
                                NavigationService.Navigate(new Pages.MainPage());
                            }
                        }
                        else
                        {
                            ElectronicJournal.OrderCancelled();
                            NavigationService.Navigate(new Pages.MainPage());
                        }
                        checkTranTimer.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        log.Error("Error PayByCashPage -> dispatcherTimer_Tick() : " + ex1.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashPage -> dispatcherTimer_Tick() : " + ex.ToString());
            }
        }
        private bool disableMyself = false;
        private void DisposePage()
        {
            try
            {
                disableMyself = true;
                BNRManager.Instance.StopProcess();

                foreach (var data in stackedNotesListReceived)
                {
                    if (data.BillNumber != 0)
                    {
                        int bill = 0;
                        bill = billTable.Where(x => x.BillType == data.BillType).Select(x => x.DigitBillType).FirstOrDefault();
                        if (cassettes[0].billType == data.BillType || cassettes[0].billType == 24)
                        {
                            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_ADD_BANKNOTE" + bill)));
                            if (StockOperations.InsStock(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette1, (int)UpdateType.Increase, data.BillNumber))
                                MoneyOperations.InsMoney(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette1, (int)UpdateType.Increase, bill);
                        }
                        else if (cassettes[1].billType == data.BillType || cassettes[1].billType == 24)
                        {
                            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_ADD_BANKNOTE" + bill)));
                            if (StockOperations.InsStock(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette2, (int)UpdateType.Increase, data.BillNumber))
                                MoneyOperations.InsMoney(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette2, (int)UpdateType.Increase, bill);
                        }
                        else if (cassettes[2].billType == data.BillType || cassettes[2].billType == 24)
                        {
                            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_ADD_BANKNOTE" + bill)));
                            if (StockOperations.InsStock(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette3, (int)UpdateType.Increase, data.BillNumber))
                                MoneyOperations.InsMoney(trxId, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + bill), (int)DeviceType.Cassette3, (int)UpdateType.Increase, bill);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashPage -> DisposePage() : " + ex.ToString());
            }
        }
        private bool dispenseMyself = false;
        private void DispenseSeqBill(byte[] snd_arr)
        {
            try
            {
                log.Info("PayByCashOrCoinPage - DispenseSeqBill");
                dispenseMyself = true;
                BNRManager.Instance.DispenseBillSequnce(snd_arr);
            }
            catch (Exception ex)
            {
                log.Debug("Error PayByCashOrCoinPage -> DispenseBill() : " + ex.ToString());
            }
        }

        private Int32 maxNoteAccept = 20;
        private Int32 balance = 0;
        private Int32 noteCount = 0;
        bool isReturn = false;
        List<string> noteRec = new List<string>();
        private async void CashRecevied(string receivedCash)
        {
            try
            {
                if (isCancel)
                    return;
                else
                {
                    if (receivedCash != "")
                    {
                        noteRec.Add(receivedCash);
                        if (noteRec.Count == 1)
                        {
                            ElectronicJournal.NoteInserted(Convert.ToInt16(noteRec[0]));
                        }
                        else
                        {
                            for (int i = noteRec.Count - 1; i >= 1; i--)
                            {
                                int amount = Convert.ToInt16(noteRec[i]) - Convert.ToInt16(noteRec[i - 1]);
                                ElectronicJournal.NoteInserted(amount);
                            }
                        }
                        lblPaidAmountValue.Content = "₹" + receivedCash + ".00";
                        customerMsgGrid.Visibility = Visibility.Hidden;
                        resetTimmer();
                        decimal disbalance = TotalAmountToCollect - (Convert.ToDecimal(receivedCash));
                        lblChangeAmountValue.Content = "₹" + (disbalance > 0 ? disbalance : disbalance * (-1));
                        btnBack.Visibility = Visibility.Hidden;
                        await Task.Delay(250);
                        if ((Convert.ToDecimal(receivedCash) - TotalAmountToCollect) > 0)
                        {
                            lblCusomerMsg.Content = MultiLanguage.GetText("PaymentSccess");
                            customerMsgGrid.Background = System.Windows.Media.Brushes.Green;
                            customerMsgGrid.Visibility = Visibility.Visible;

                            lblChangeAmountKey.Content = MultiLanguage.GetText("balanceAmount");
                            noteRec = new List<string>();
                            DisposePage();
                            checkTranTimer.Dispose();
                            BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                            await Task.Delay(5000);
                            mainGrid.Visibility = Visibility.Hidden;
                            waitGrid.Visibility = Visibility.Visible;                            
                            log.Debug("Debug PayByCashOrCoinPage ->  receivedCashTxtBox.Text : " + receivedCash);
                            await Task.Delay(200);
                            BNRManager.Instance.GetCassetteStatus();
                            await Task.Delay(500);
                            balance = Convert.ToInt32((Convert.ToDecimal(receivedCash)) - TotalAmountToCollect);
                            Int32 cashincast = await checkAvalabeBalance(balance);
                            log.Debug("PayByCashOrCoinPage -> cashincast : " + cashincast);
                            if (cashincast == 0)
                            {
                                Constants.Change = balance;
                                isReturn = false;
                                try
                                {
                                    switch (Ticket.journeyType)
                                    {
                                        case JourneyType.Group_Ticket:
                                        case JourneyType.SJT:
                                        case JourneyType.RJT:
                                            bool result = false;
                                            result = OCCOperations.InsertQRTransaction();
                                            if (result)
                                            {
                                                log.Debug("LogTypes.Info : InsertQRTransaction return true");
                                                if (StockOperations.qrSlip > Ticket.listTickets.Count)
                                                {
                                                    bool print_result = false;
                                                    Ticket.sellTicketCount = 0;
                                                    foreach (var t in Ticket.listTickets)
                                                    {
                                                        if (QRPrinter.Instance.CheckQrPrinterStatus() == Enums.PRINTER_STATE.OK)
                                                        {
                                                            ElectronicJournal.QRPrintStarted();
                                                            bool flag = QRPrinter.Instance.PrintQR(t.TicketGUID, Ticket.ticketActivateDts.ToString(), t.explanation, t.From, t.To, t.peopleCount, t.price, String.Format("{0}.{1}.{2}.{3}", Ticket.dayCount, t.FromId, Parameters.TVMDynamic.GetParameter("unitId"), t.alias));
                                                            await Task.Delay(4000);
                                                            if (true)
                                                            {
                                                                print_result = true;
                                                                long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_QR));
                                                                int stock = StockOperations.qrSlip;
                                                                StockOperations.InsStock(trxId, (int)StockType.QRSlip, (int)DeviceType.QRPrinter, (int)UpdateType.Decrease, 1);
                                                                ElectronicJournal.QRPrintOver();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ElectronicJournal.QRPrintError();
                                                            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                                                            {
                                                                CustomTL60Printer.Instance.UnableToPrintQR(Ticket.ticketActivateDts.ToString(), t.explanation, t.From, t.To, t.alias.ToString());
                                                            }
                                                            print_result = true;
                                                        }
                                                    }

                                                    if (print_result)
                                                    {
                                                        await Task.Delay(200);
                                                        BNRManager.Instance.GetExtendedCassetteStatus();
                                                        await Task.Delay(1000);
                                                    }
                                                    else
                                                    {
                                                        BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                        mainGrid.Visibility = Visibility.Visible;
                                                        waitGrid.Visibility = Visibility.Hidden;
                                                        lblPaidAmountValue.Content = "";
                                                        lblChangeAmountValue.Content = "";
                                                        isCancel = true;
                                                        returncash(false, false);
                                                    }
                                                }
                                                else
                                                {
                                                    BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                    mainGrid.Visibility = Visibility.Visible;
                                                    waitGrid.Visibility = Visibility.Hidden;
                                                    lblPaidAmountValue.Content = "";
                                                    lblChangeAmountValue.Content = "";
                                                    isCancel = true;
                                                    returncash(false, false);
                                                }
                                            }
                                            else
                                            {
                                                if (RPT())
                                                {
                                                    await Task.Delay(200);
                                                    BNRManager.Instance.GetExtendedCassetteStatus();
                                                    await Task.Delay(1000);
                                                }
                                                else
                                                {
                                                    BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                    mainGrid.Visibility = Visibility.Visible;
                                                    waitGrid.Visibility = Visibility.Hidden;
                                                    lblPaidAmountValue.Content = "";
                                                    lblChangeAmountValue.Content = "";
                                                    isCancel = true;
                                                    returncash(false, false);
                                                }
                                            }
                                            break;
                                        case JourneyType.Day_Pass:
                                        case JourneyType.Weekend_Pass:
                                            if (RPT())
                                            {
                                                await Task.Delay(200);
                                                BNRManager.Instance.GetExtendedCassetteStatus();
                                                await Task.Delay(1000);
                                            }
                                            else
                                            {
                                                BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                mainGrid.Visibility = Visibility.Visible;
                                                waitGrid.Visibility = Visibility.Hidden;
                                                lblPaidAmountValue.Content = "";
                                                lblChangeAmountValue.Content = "";
                                                isCancel = true;
                                                returncash(false, false);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    
                                }
                                catch(Exception ex)
                                {
                                    log.Error("PayByCashOrCoinPage -> CashRecevied : " + ex.ToString());
                                }                                                                
                                return;
                            }
                            else
                            {
                                BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                mainGrid.Visibility = Visibility.Visible;
                                waitGrid.Visibility = Visibility.Hidden;
                                lblPaidAmountValue.Content = "";
                                lblChangeAmountValue.Content = "";
                                returncash(true, true);
                            }
                        }
                        else if ((Convert.ToDecimal(receivedCash) - TotalAmountToCollect) == 0)
                        {
                            lblCusomerMsg.Content = MultiLanguage.GetText("PaymentSccess");
                            customerMsgGrid.Background = System.Windows.Media.Brushes.Green;
                            customerMsgGrid.Visibility = Visibility.Visible;

                            noteRec = new List<string>();
                            DisposePage();
                            checkTranTimer.Dispose();
                            BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                            await Task.Delay(5000);
                            mainGrid.Visibility = Visibility.Hidden;
                            waitGrid.Visibility = Visibility.Visible;
                            balance = 0;
                            try
                            {
                                switch (Ticket.journeyType)
                                {
                                    case JourneyType.Group_Ticket:
                                    case JourneyType.SJT:
                                    case JourneyType.RJT:
                                        bool result = false;
                                        result = OCCOperations.InsertQRTransaction();
                                        if (result)
                                        {
                                            log.Debug("LogTypes.Info : InsertQRTransaction return true");
                                            if (StockOperations.qrSlip > Ticket.listTickets.Count)
                                            {
                                                bool print_result = false;
                                                Ticket.sellTicketCount = 0;
                                                foreach (var t in Ticket.listTickets)
                                                {
                                                    if (QRPrinter.Instance.CheckQrPrinterStatus() == Enums.PRINTER_STATE.OK)
                                                    {
                                                        bool flag = QRPrinter.Instance.PrintQR(t.TicketGUID, Ticket.ticketActivateDts.ToString(), t.explanation, t.From, t.To, t.peopleCount, t.price, String.Format("{0}.{1}.{2}.{3}", Ticket.dayCount, t.FromId, Parameters.TVMDynamic.GetParameter("unitId"), t.alias));
                                                        await Task.Delay(4000);
                                                        if (true)
                                                        {
                                                            print_result = true;
                                                            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_QR));
                                                            int stock = StockOperations.qrSlip;
                                                            StockOperations.InsStock(trxId, (int)StockType.QRSlip, (int)DeviceType.QRPrinter, (int)UpdateType.Decrease, Ticket.ticketCount);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ElectronicJournal.QRPrintError();
                                                        if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                                                        {
                                                            CustomTL60Printer.Instance.UnableToPrintQR(Ticket.ticketActivateDts.ToString(), t.explanation, t.From, t.To, t.alias.ToString());
                                                        }
                                                        print_result = true;
                                                    }
                                                }

                                                if (print_result)
                                                {
                                                    await Task.Delay(200);
                                                    BNRManager.Instance.GetExtendedCassetteStatus();
                                                    await Task.Delay(1000);
                                                }
                                                else
                                                {
                                                    BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                    mainGrid.Visibility = Visibility.Visible;
                                                    waitGrid.Visibility = Visibility.Hidden;
                                                    lblPaidAmountValue.Content = "";
                                                    lblChangeAmountValue.Content = "";
                                                    isCancel = true;
                                                    returncash(false, false);
                                                }
                                            }
                                            else
                                            {
                                                BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                mainGrid.Visibility = Visibility.Visible;
                                                waitGrid.Visibility = Visibility.Hidden;
                                                lblPaidAmountValue.Content = "";
                                                lblChangeAmountValue.Content = "";
                                                isCancel = true;
                                                returncash(false, false);
                                            }
                                        }
                                        else
                                        {
                                            if (RPT())
                                            {
                                                await Task.Delay(200);
                                                BNRManager.Instance.GetExtendedCassetteStatus();
                                                await Task.Delay(1000);
                                            }
                                            else
                                            {
                                                BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                                mainGrid.Visibility = Visibility.Visible;
                                                waitGrid.Visibility = Visibility.Hidden;
                                                lblPaidAmountValue.Content = "";
                                                lblChangeAmountValue.Content = "";
                                                isCancel = true;
                                                returncash(false, false);
                                            }
                                        }
                                        break;
                                    case JourneyType.Day_Pass:
                                    case JourneyType.Weekend_Pass:
                                        if (RPT())
                                        {
                                            await Task.Delay(200);
                                            BNRManager.Instance.GetExtendedCassetteStatus();
                                            await Task.Delay(1000);
                                        }
                                        else
                                        {
                                            BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                            mainGrid.Visibility = Visibility.Visible;
                                            waitGrid.Visibility = Visibility.Hidden;
                                            lblPaidAmountValue.Content = "";
                                            lblChangeAmountValue.Content = "";
                                            isCancel = true;
                                            returncash(false, false);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("PayByCashOrCoinPage -> CashRecevied : " + ex.ToString());
                            }
                            return;
                        }

                        if (noteCount == maxNoteAccept)
                        {
                            try
                            {
                                noteRec = new List<string>();
                                checkTranTimer.Dispose();
                                BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                                DisposePage();
                                lblPaidAmountValue.Content = "";
                                lblChangeAmountValue.Content = "";
                                mainGrid.Visibility = Visibility.Visible;
                                waitGrid.Visibility = Visibility.Hidden;
                                returncash(false, true);
                            }
                            catch (Exception ex1)
                            {
                                log.Error("Error PayByCashOrCoinPage -> ReceivedCashTxtBox_TextChanged() - CCTalk Manager : " + ex1.ToString());
                            }
                            return;
                        }

                        noteCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //?????????????????????????
                    DisposePage();
                    await Task.Delay(250);
                    byte[] snd_arr = await getDispence();
                    if (snd_arr != null && snd_arr.Length > 0)
                        DispenseSeqBill(snd_arr);
                    await Task.Delay(250);
                    stackedNotesRecived.Clear();
                    NavigationService.Navigate(new Pages.MainPage());
                }
                catch (Exception ex2)
                {
                    log.Error("Error PayByCashPage -> ReceivedCashTxtBox_TextChanged() - 2 : " + ex2.ToString());
                }
                log.Error("Error PayByCashPage -> ReceivedCashTxtBox_TextChanged() : " + ex.ToString());
            }
        }
        private bool RPT()
        {
            bool result = false;
            try
            {
                result = RPTOperations.GiveRPTTicket();
                if (result)
                {
                    log.Debug("LogTypes.Info : GiveRPTTicket return true");
                }
                else
                {
                    result = false;
                    log.Debug("LogTypes.Warning : GiveRPTTicket return false");
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error " + ex.ToString());
            }
            return result;
        }
        void UnloadValue()
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(300);

                if (balance > 0)
                {
                    log.Debug("PayByCashPage -> calculateBalance called : " + balance);
                    calculateBalance(balance);
                }
                else
                {
                    await Task.Delay(300);
                    lblChangeAmountValue.Content = "";

                    stackedNotesRecived.Clear();

                    NavigationService.Navigate(new Pages.PrintReciptPage(Convert.ToString(Ticket.ticketCount), Convert.ToString(receivedNote), Convert.ToString(balance)));
                }

            }), DispatcherPriority.Background);
        }
        private async void calculateBalance(Int32 balance)
        {
            try
            {
                int loopbalance = balance;

                int totalCount = 0;

                bool dispenceCall = false;

                if (loopbalance >= 10)
                {
                    var cashInDescOrder = cassettes.OrderByDescending(s => s.billType);

                    List<byte> lArrSend = new List<byte>();

                    foreach (var cashinhand in cashInDescOrder)
                    {
                        if (cashinhand.billType == 24)
                        {
                            if (cashinhand.billNumber != 0)
                            {
                                int bill = Constants.EscrowAmount;//billTable.Where(x => x.BillType == cashinhand.billType).Select(x => x.DigitBillType).FirstOrDefault();
                                int billNumber = cashinhand.billNumber;
                                int count = 0;
                                if (cashinhand.billNumber == 1)
                                {
                                    int billType = billTable.Where(x => x.DigitBillType == Constants.EscrowAmount).Select(x => x.BillType).FirstOrDefault();
                                    int billEscrow = stackedNotesRecived.Where(x => x.BillType == billType).Select(x => x.BillType).FirstOrDefault();
                                    if (billEscrow != 0)
                                    {
                                        count = loopbalance / bill;
                                        if (billNumber <= count)
                                        {
                                            totalCount += billNumber;
                                            loopbalance = loopbalance - (bill * billNumber);
                                            ElectronicJournal.BalanceNoteReturned(bill * billNumber);
                                            lArrSend.Add(Convert.ToByte(cashinhand.billType));
                                            lArrSend.Add(Convert.ToByte(billNumber));
                                        }
                                        else if (billNumber > count)
                                        {
                                            totalCount += count;
                                            loopbalance = loopbalance - (bill * count);
                                            ElectronicJournal.BalanceNoteReturned(bill * count);
                                            lArrSend.Add(Convert.ToByte(cashinhand.billType));
                                            lArrSend.Add(Convert.ToByte(count));
                                        }
                                    }
                                }
                                else
                                {
                                    count = loopbalance / bill;
                                    if (billNumber <= count)
                                    {
                                        totalCount += billNumber;
                                        loopbalance = loopbalance - (bill * billNumber);
                                        ElectronicJournal.BalanceNoteReturned(bill * billNumber);
                                        lArrSend.Add(Convert.ToByte(cashinhand.billType));
                                        lArrSend.Add(Convert.ToByte(billNumber));
                                    }
                                    else if (billNumber > count)
                                    {
                                        totalCount += count;
                                        loopbalance = loopbalance - (bill * count);
                                        ElectronicJournal.BalanceNoteReturned(bill * count);
                                        lArrSend.Add(Convert.ToByte(cashinhand.billType));
                                        lArrSend.Add(Convert.ToByte(count));
                                    }
                                }
                                if (loopbalance == 0)
                                    break;
                            }
                        }
                        else
                        {
                            if (cashinhand.billNumber != 0)
                            {
                                int bill = billTable.Where(x => x.BillType == cashinhand.billType).Select(x => x.DigitBillType).FirstOrDefault();
                                int billNumber = cashinhand.billNumber;
                                int count = 0;
                                count = loopbalance / bill;
                                if (billNumber <= count)
                                {
                                    totalCount += billNumber;
                                    loopbalance = loopbalance - (bill * billNumber);
                                    ElectronicJournal.BalanceNoteReturned(bill * billNumber);
                                    lArrSend.Add(Convert.ToByte(cashinhand.billType));
                                    lArrSend.Add(Convert.ToByte(billNumber));
                                }
                                else if (billNumber > count)
                                {
                                    totalCount += count;
                                    loopbalance = loopbalance - (bill * count);
                                    ElectronicJournal.BalanceNoteReturned(bill * count);
                                    lArrSend.Add(Convert.ToByte(cashinhand.billType));
                                    lArrSend.Add(Convert.ToByte(count));
                                }

                                if (loopbalance == 0)
                                    break;
                            }
                        }
                    }
                    
                    if (totalCount <= 15)
                    {
                        byte[] snd_arr = lArrSend.ToArray();
                        if (snd_arr != null && snd_arr.Length > 0)
                        {
                            dispenceCall = true;
                            DispenseSeqBill(snd_arr);
                        }
                    }
                    else
                    {
                        loopbalance = balance;
                    }
                }

                if (loopbalance != 0)
                {
                    int bal = await checkHooperAvalabeBalance(loopbalance);
                    if (bal == 0)
                    {
                        await dispenceHooperBalance(loopbalance);
                        if (!dispenceCall)
                        {
                            if (Constants.BNRStatus == "DISABLED")
                            {
                                isReturn = false;
                                DispenceValue();
                            }
                        }
                    }
                }

                log.Debug("PayByCashOrCoinPage -> balance() : " + balance);
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashOrCoinPage -> calculateBalance() : " + ex.ToString());
            }

        }
        int i = 0;
        void DispenceingValue()
        {            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (i == 0)
                {
                    checkTranTimer.Dispose();

                    if (!isReturn)
                    {
                        waitGrid.Visibility = Visibility.Hidden;
                        mainGrid.Visibility = Visibility.Hidden;
                        returnAmountTxt.Content = "₹" + Convert.ToString(balance);
                        cashGrid.Visibility = Visibility.Visible;
                    }
                    else if (isReturn && !isCancel)
                    {
                        waitGrid.Visibility = Visibility.Hidden;
                        mainGrid.Visibility = Visibility.Hidden;
                        returnAmountTxt.Content = "₹" + Convert.ToString(receivedNote);
                        cashGrid.Visibility = Visibility.Visible;
                    }
                    else if (isReturn && isCancel)
                    {
                        waitGrid.Visibility = Visibility.Hidden;
                        mainGrid.Visibility = Visibility.Hidden;
                        returnAmountTxt.Content = "₹" + Convert.ToString(receivedNote);
                        cashGrid.Visibility = Visibility.Visible;
                    }
                }
                i++;
            }), DispatcherPriority.Background);
        }

        bool isCancel = false;
        void DispenceValue()
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                stackedNotesRecived.Clear();

                if (!isReturn)
                {
                    if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
                    {
                        TVMUtility.PlayVoice(8, null, Convert.ToString(Convert.ToInt16(balance)), "EN");
                    }
                    if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
                    {
                        TVMUtility.PlayVoice(8, null, Convert.ToString(Convert.ToInt16(balance)), "ML");
                    }
                    if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
                    {
                        TVMUtility.PlayVoice(8, null, Convert.ToString(Convert.ToInt16(balance)), "IN");
                    }
                    LedOperations.GreenText("PLEASE COLLECT BALANCE");
                    await Task.Delay(1000);
                    
                    waitGrid.Visibility = Visibility.Hidden;
                    mainGrid.Visibility = Visibility.Hidden;
                    returnAmountTxt.Content = "₹" + Convert.ToString(balance);
                    cashGrid.Visibility = Visibility.Visible;

                    await Task.Delay(2000);
                    NavigationService.Navigate(new Pages.PrintReciptPage(Convert.ToString(Ticket.ticketCount), Convert.ToString(receivedNote), Convert.ToString(balance)));
                }
                else if (isReturn && !isCancel)
                {
                    waitGrid.Visibility = Visibility.Hidden;
                    mainGrid.Visibility = Visibility.Hidden;
                    returnAmountTxt.Content = "₹" + Convert.ToString(receivedNote);
                    cashGrid.Visibility = Visibility.Visible;
                    acceptAgainAsync();
                }
                else if (isReturn && isCancel)
                {
                    LedOperations.GreenText("PAYMENT UNSUCCESSFUL");
                    waitGrid.Visibility = Visibility.Hidden;
                    mainGrid.Visibility = Visibility.Hidden;
                    cashGrid.Visibility = Visibility.Hidden;
                    TranUnSuccGrid.Visibility = Visibility.Visible;
                    ElectronicJournal.OrderCancelled();
                    await Task.Delay(5000);
                    NavigationService.Navigate(new Pages.MainPage());
                }
            }), DispatcherPriority.Background);
        }
        private void returncash(bool lowbalance, bool note)
        {
            this.Dispatcher.Invoke(async () =>
            {
                isReturn = true;
                await Task.Delay(5000);
                byte[] snd_arr = await getDispence();
                if (snd_arr != null && snd_arr.Length > 0)
                {
                    DispenseSeqBill(snd_arr);
                }
                else
                {
                    if (lowbalance)
                        acceptAgainAsync();
                }

                if (note)
                {
                    if (lowbalance)
                    {
                        lblCusomerMsg.Content = MultiLanguage.GetText("nobalance");
                        customerMsgGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        noteCount = 1;
                        lblCusomerMsg.Content = MultiLanguage.GetText("maxnotes");
                        customerMsgGrid.Visibility = Visibility.Visible;
                    }
                    stackedNotesRecived.Clear();
                }
            });
        }

        void acceptAgainAsync()
        {
            isReturn = false;
            cashGrid.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Visible;
            //resetTimmer();
            lblPaidAmountValue.Content = "";
            lblChangeAmountValue.Content = "";
            NavigationService.Navigate(new Pages.MainPage());
            //if (Constants.BNRStatus == "DISABLED")
            //{
            //    BNRManager.Instance.AcceptProcess(Constants.EnableBillNotes);
            //    BNRManager.BNRCurrencyStateInputEvent += new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
            //}
        }

        private void resetTimmer()
        {
            TranCancelTimer = 15;
            checkTranTimer.Dispose();
            checkTranTimerDelegate = new TimerCallback(dispatcherTimer_Tick);
            checkTranTimer = new Timer(checkTranTimerDelegate, null, TranCancelTimer * 1000,0);
            Dispatcher.Invoke((Action)(() =>
            {
                Countdown(TranCancelTimer, TimeSpan.FromSeconds(1), cur => lblRemain.Content = cur.ToString());
                btnFinish.Visibility = Visibility.Hidden;
            }
            ));
        }

        private async Task<byte[]> getDispence()
        {
            byte[] snd_arr = null;

            try
            {
                dispenseMyself = true;
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(1000);
                List<byte> lArrSend = new List<byte>();
                foreach (var recivednote in stackedNotesRecived)
                {
                    log.Debug("Debug stackedNotesRecived -> () BillType : " + recivednote.BillType + "billNumber : " + recivednote.BillNumber);
                    int dispencetype = 0;
                    foreach (var casset in cassettes)
                    {
                        if (casset.billType == recivednote.BillType && casset.billNumber != 0)
                        {
                            dispencetype = casset.billType;
                            break;
                        }
                        else if (casset.billType == 24 && casset.billNumber != 0)
                        {
                            dispencetype = casset.billType;
                            break;
                        }
                    }
                    lArrSend.Add(Convert.ToByte(dispencetype));
                    lArrSend.Add(Convert.ToByte(recivednote.BillNumber));
                }
                snd_arr = lArrSend.ToArray();
            }
            catch (Exception ex)
            {
                log.Debug("Error PayByCashPage -> getDispence() : " + ex.ToString());
            }

            return snd_arr;
        }

        private async Task<int> checkAvalabeBalance(int balance)
        {
            try
            {
                int loopbalance = balance;

                int totalCount = 0;

                if (loopbalance >= 10)
                {
                    var cashInDescOrder = cassettes.OrderByDescending(s => s.billType);
                    log.Debug("Debug PayByCashPage -> cassettes.Count : " + cassettes.Count());
                    foreach (var cashinhand in cashInDescOrder)
                    {
                        log.Debug("Debug PayByCashPage -> cashinhand.billType : " + cashinhand.billType + " cashinhand.billNumber : " + cashinhand.billNumber);
                        if (cashinhand.billType == 24)
                        {
                            if (cashinhand.billNumber != 0)
                            {
                                int bill = Constants.EscrowAmount;//billTable.Where(x => x.BillType == cashinhand.billType).Select(x => x.DigitBillType).FirstOrDefault();
                                int billNumber = cashinhand.billNumber;
                                int count = 0;
                                if (cashinhand.billNumber == 1)
                                {
                                    int billType = billTable.Where(x => x.DigitBillType == Constants.EscrowAmount).Select(x => x.BillType).FirstOrDefault();
                                    int billEscrow = stackedNotesRecived.Where(x => x.BillType == billType).Select(x => x.BillType).FirstOrDefault();
                                    if (billEscrow != 0)
                                    {
                                        count = loopbalance / bill;
                                        if (billNumber <= count)
                                        {
                                            totalCount += billNumber;
                                            loopbalance = loopbalance - (bill * billNumber);
                                        }
                                        else if (billNumber > count)
                                        {
                                            totalCount += count;
                                            loopbalance = loopbalance - (bill * count);
                                        }
                                    }
                                }
                                else
                                {
                                    count = loopbalance / bill;
                                    if (billNumber <= count)
                                    {
                                        totalCount += billNumber;
                                        loopbalance = loopbalance - (bill * billNumber);
                                    }
                                    else if (billNumber > count)
                                    {
                                        totalCount += count;
                                        loopbalance = loopbalance - (bill * count);
                                    }
                                }                               
                            }
                        }
                        else
                        {
                            if (cashinhand.billNumber != 0)
                            {
                                int count = 0;
                                int bill = billTable.Where(x => x.BillType == cashinhand.billType).Select(x => x.DigitBillType).FirstOrDefault();
                                int billNumber = cashinhand.billNumber;
                                count = loopbalance / bill;
                                if (billNumber <= count)
                                {
                                    totalCount += billNumber;
                                    loopbalance = loopbalance - (bill * billNumber);
                                }
                                else if (billNumber > count)
                                {
                                    totalCount += count;
                                    loopbalance = loopbalance - (bill * count);
                                }
                            }
                        }

                        if (loopbalance == 0)
                            break;
                    }

                    if (totalCount > Convert.ToInt32(15))
                    {
                        loopbalance = balance;
                    }
                }
                if (loopbalance != 0)
                {
                    int bal = await checkHooperAvalabeBalance(loopbalance);
                    if (bal == 0)
                        loopbalance = 0;
                }
                log.Debug("PayByCashPage -> loopbalance() : " + loopbalance);
                return loopbalance;
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashPage -> checkAvalabeBalance() : " + ex.ToString());
                return 0;
            }
        }
        string level = "";
        int maxCoinDispence = 15;

        private async Task<int> checkHooperAvalabeBalance(int balance)
        {
            int loopbalance = balance;
            try
            {
                dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedInputEvent += new dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedEventHandler(HooperManager_HopperStateInputEvent);

                int totalCoinCount = 0;

                if (CCTalkManager.Instance.coinHopperEV4000_3.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_3.GetHighLowStatus();
                    int coinnumber = loopbalance / Constants.HopperAddress3Coin;
                    if (StockOperations.coin5 >= coinnumber)
                    {
                        Constants.Hopper3Level = level.Substring(0, 1);
                        //int coinnumber = loopbalance / 2;
                        if (coinnumber > 0)
                        {
                            totalCoinCount += coinnumber;
                            loopbalance = loopbalance - (Constants.HopperAddress3Coin * coinnumber);
                        }
                    }
                }

                await Task.Delay(200);
                if (CCTalkManager.Instance.coinHopperEV4000_2.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_2.GetHighLowStatus();
                    int coinnumber = loopbalance / Constants.HopperAddress2Coin;
                    if (StockOperations.coin2 >= coinnumber)
                    {
                        Constants.Hopper2Level = level.Substring(0, 1);
                        if (coinnumber > 0)
                        {
                            totalCoinCount += coinnumber;
                            loopbalance = loopbalance - (Constants.HopperAddress2Coin * coinnumber);
                        }
                    }
                }
                await Task.Delay(200);

                if (CCTalkManager.Instance.coinHopperEV4000_1.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_1.GetHighLowStatus();
                    int coinnumber = loopbalance / Constants.HopperAddress1Coin;
                    if (StockOperations.coin1 >= coinnumber)
                    {
                        Constants.Hopper1Level = level.Substring(0, 1);
                        if (coinnumber > 0)
                        {
                            totalCoinCount += coinnumber;
                            loopbalance = loopbalance - (Constants.HopperAddress1Coin * coinnumber);
                        }
                    }
                }

                if (totalCoinCount > maxCoinDispence)
                {
                    loopbalance = balance;
                }

                log.Error("Error PayByCashPage -> coin number() : " + loopbalance);
                return loopbalance;
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashPage -> checkAvalabeBalance() : " + ex.ToString());
                return loopbalance;
            }
        }

        private async Task<int> dispenceHooperBalance(int balance)
        {
            int loopbalance = balance;
            try
            {
                if (CCTalkManager.Instance.coinHopperEV4000_3.Category != null)
                {
                    int coinnumber = loopbalance / Constants.HopperAddress3Coin;
                    if (StockOperations.coin5 >= coinnumber)
                    {
                        if (coinnumber > 0)
                        {
                            loopbalance = loopbalance - (Constants.HopperAddress3Coin * coinnumber);
                            DispenceCoin3(coinnumber);
                            await Task.Delay(200);
                        }
                    }
                }

                if (CCTalkManager.Instance.coinHopperEV4000_2.Category != null)
                {
                    int coinnumber = loopbalance / Constants.HopperAddress2Coin;
                    if (StockOperations.coin2 >= coinnumber)
                    {
                        //int coinnumber = loopbalance / 5;
                        if (coinnumber > 0)
                        {
                            loopbalance = loopbalance - (Constants.HopperAddress2Coin * coinnumber);
                            DispenceCoin2(coinnumber);
                            await Task.Delay(200);
                        }
                    }
                }


                if (CCTalkManager.Instance.coinHopperEV4000_1.Category != null)
                {
                    int coinnumber = loopbalance / Constants.HopperAddress1Coin;
                    if (StockOperations.coin1 >= coinnumber)
                    {
                        //int coinnumber = loopbalance / 10;
                        if (coinnumber > 0)
                        {
                            loopbalance = loopbalance - (Constants.HopperAddress1Coin * coinnumber);
                            DispenceCoin1(coinnumber);
                            await Task.Delay(200);
                        }
                    }
                }

                log.Error("PayByCashPage -> Dispence Coin balance() : " + loopbalance);
                return loopbalance;
            }
            catch (Exception ex)
            {
                log.Error("Error PayByCashPage -> checkAvalabeBalance() : " + ex.ToString());
                return loopbalance;
            }
        }

        private void HooperManager_HopperStateInputEvent(byte[] res, CoinHopperCommands sentCommand)
        {
            if (sentCommand == CoinHopperCommands.REQUEST_PAYOUT_HIGH_LOW_STATUS)
            {
                var Data = res;
                var comd = sentCommand;
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    try
                    {
                        var Dataresult = Utils.TVMUtility.Hex2Binary(Data[4].ToString());
                        var highorlow = Dataresult.Substring(Dataresult.Length - 2);
                        var high = highorlow[0];
                        var low = highorlow[1];
                        if (Convert.ToInt16(high.ToString()) == 1)
                        {
                            level = "High Level";
                        }
                        else if (Convert.ToInt16(low.ToString()) == 1)
                        {
                            level = "Low Level";
                        }
                        else if (Convert.ToInt16(low.ToString()) == 0)
                        {
                            level = "Normal Level";
                        }
                    }
                    catch (Exception ex1)
                    {
                        log.Error("Error PayByCashPage -> HooperManager_HopperStateInputEvent() : " + ex1.ToString());
                    }
                }));
            }
        }

        private void DispenceCoin1(int coinCount)
        {
            int value = StockOperations.coin1 - Convert.ToInt16(coinCount);
            if (value > 0)
            {
                try
                {
                    decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_PAYOUT_COIN1);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, coinCount))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, Convert.ToDecimal(coinCount * Constants.HopperAddress1Coin)))
                        log.Debug(String.Format("{0} x coin1 decreased from hopper1", coinCount));

                    ElectronicJournal.BalanceCoinReturned(coinCount * Constants.HopperAddress1Coin);
                }
                catch (Exception ex1)
                {
                    log.Error("Error PayByCashPage -> DispenceCoin1() : " + ex1.ToString());
                }
                CCTalkManager.Instance.coinHopperEV4000_1.DispenseCoins(coinCount);
            }
        }

        private void DispenceCoin2(int coinCount)
        {
            int value = StockOperations.coin2 - Convert.ToInt16(coinCount);
            if (value > 0)
            {
                try
                {
                    decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_PAYOUT_COIN2);
                    if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, coinCount))
                        if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, Convert.ToDecimal(coinCount * Constants.HopperAddress2Coin)))
                            log.Debug(String.Format("{0} x coin2 decreased from hopper2", coinCount));

                    ElectronicJournal.BalanceCoinReturned(coinCount * Constants.HopperAddress2Coin);
                }
                catch (Exception ex1)
                {
                    log.Error("Error PayByCashPage -> DispenceCoin2() : " + ex1.ToString());
                }
                CCTalkManager.Instance.coinHopperEV4000_2.DispenseCoins(coinCount);
            }
        }

        private void DispenceCoin3(int coinCount)
        {
            int value = StockOperations.coin5 - Convert.ToInt16(coinCount);
            if (value > 0)
            {
                try
                {
                    decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_PAYOUT_COIN5);
                    if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, coinCount))
                        if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, Convert.ToDecimal(coinCount * Constants.HopperAddress3Coin)))
                            log.Debug(String.Format("{0} x coin3 decreased from hopper3", coinCount));

                    ElectronicJournal.BalanceCoinReturned(coinCount * Constants.HopperAddress3Coin);
                }
                catch (Exception ex1)
                {
                    log.Error("Error PayByCashPage -> DispenceCoin3() : " + ex1.ToString());
                }
                CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(coinCount);
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                checkTranTimer.Dispose();
                dt.Stop();

                BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
                BNRManager.BNRBillTableInputEvent -= new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
                BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
                BNRManager.BNRCassetteStatusInputEvent -= new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
                BNRManager.BNRExtenedCassetteStatusInputEvent -= new BNRManager.BNRExtenedCassetteStatusEventHandler(BNRManager_BNRExtenedCassetteStatusEvent);
                dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedInputEvent -= new dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedEventHandler(HooperManager_HopperStateInputEvent);
  
                myGif.Stop();
                myGif.Close();
                myGif.Source = null;

                returnCashImageGif.Stop();
                returnCashImageGif.Close();
                returnCashImageGif.Source = null;

                returnCashImageGif1.Stop();
                returnCashImageGif1.Close();
                returnCashImageGif1.Source = null;

                loadingImageGif.Stop();
                loadingImageGif.Close();
                loadingImageGif.Source = null;

                DisposePage();                
            }
            catch (Exception ex2)
            {
                log.Error("Error PayByCashPage -> Page_Unloaded() - BNR : " + ex2.ToString());
            }
            
        }
        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0, 0, 1);
            myGif.Play();
        }

        private void returnCashImageGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            returnCashImageGif.Position = new TimeSpan(0, 0, 1);
            returnCashImageGif.Play();
        }
        private void returnCashImageGif1_MediaEnded(object sender, RoutedEventArgs e)
        {
            returnCashImageGif1.Position = new TimeSpan(0, 0, 1);
            returnCashImageGif1.Play();
        }
        private void loadingImageGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            loadingImageGif.Position = new TimeSpan(0, 0, 1);
            loadingImageGif.Play();
        }
    }
}
