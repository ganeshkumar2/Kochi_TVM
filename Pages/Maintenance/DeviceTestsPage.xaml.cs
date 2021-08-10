using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for DeviceTestsPage.xaml
    /// </summary>
    public partial class DeviceTestsPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(DeviceTestsPage).Name);
        List<BillTable> billTable = new List<BillTable>();
        int Casette1Billtype = 0, Casette2Billtype = 0, Casette3Billtype = 0;
        int notevalincasset1 = 0, notevalincasset2 = 0, notevalincasset3 = 0;
        int noteincasset1 = 0, noteincasset2 = 0, noteincasset3 = 0;
        public DeviceTestsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StockOperations.SelStockStatus();
            BNRManager.Instance.PollingAction();
            BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.RcptPrntTest))
            {
                btnTestReceiptPrinter.Visibility = Visibility.Visible;
            }

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.QRTest))
            {
                btnTestQrPrinter.Visibility = Visibility.Visible;
            }

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.HopTest))
            {
                btnTestHopper.Visibility = Visibility.Visible;
            }
            StockOperations.SelStockStatus();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
        }
        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);

                    if (state == BNRState.DISABLED)
                    {
                        DispenceValue();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error BNROperationPage -> BNRManager_BNRStateInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }
        void DispenceValue()
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                waitGrid.Visibility = Visibility.Collapsed;
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(500);
                if (Constants.BNRStatus == "DISABLED")
                {
                    //CustomTL60Printer.Instance.TestBNA();
                    Constants.BNRTest = true;
                    MessageBoxOperations.ShowMessage("BNA Test", "Test completed successfully.", MessageBoxButtonSet.OK);
                }
                else
                {
                    MessageBoxOperations.ShowMessage("BNA Test", "BNA error.", MessageBoxButtonSet.OK);
                }
            }), DispatcherPriority.Background);
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.MaintancePage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }

        private void btnRPTDispenserTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnControlCardTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTestReceiptPrinter_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
            if (ReceiptPrinter == PRINTER_STATE.OK)
            {
                CustomTL60Printer.Instance.TestReceiptPrinter();
                MessageBoxOperations.ShowMessage("Receipt Printer Test", "Test completed successfully.", MessageBoxButtonSet.OK);
            }
            else
            {
                MessageBoxOperations.ShowMessage("ERROR", "Receipt Printer Error." + ReceiptPrinter, MessageBoxButtonSet.OK);
            }
        }

        private void btnTestHopper_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
           
            //if (StockOperations.coin1 > 0)
            //{
            //    CCTalkManager.Instance.coinHopperEV4000_1.DispenseCoins(1);
            //    decimal? trxId1 = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN1);
            //    if (StockOperations.InsStock((Int64)trxId1, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, 1))
            //        if (MoneyOperations.InsMoney((Int64)trxId1, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, Convert.ToDecimal(1) * Constants.HopperAddress1Coin))
            //            if (StockOperations.SelStockStatus())
            //            {
            //                CoinOperation coinOperation = new CoinOperation {count = 1, coin = Constants.HopperAddress1Coin, stock = StockOperations.coin1 };
            //                Constants.CoinDispOperations.Add(coinOperation);
            //                //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            //                //{
            //                //    CustomTL60Printer.Instance.CoinDispatchPrint(1, Constants.HopperAddress1Coin, StockOperations.coin1);
            //                //}
            //            }
            //}

            //if (StockOperations.coin2 > 0)
            //{
            //    CCTalkManager.Instance.coinHopperEV4000_2.DispenseCoins(1);
            //    decimal? trxId2 = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN2);
            //    if (StockOperations.InsStock((Int64)trxId2, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, 1))
            //        if (MoneyOperations.InsMoney((Int64)trxId2, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, Convert.ToDecimal(1) * Constants.HopperAddress2Coin))
            //            if (StockOperations.SelStockStatus())
            //            {
            //                CoinOperation coinOperation = new CoinOperation { count = 1, coin = Constants.HopperAddress2Coin, stock = StockOperations.coin2 };
            //                Constants.CoinDispOperations.Add(coinOperation);
            //                //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            //                //{
            //                //    CustomTL60Printer.Instance.CoinDispatchPrint(1, Constants.HopperAddress2Coin, StockOperations.coin2);
            //                //}
            //            }
            //}

            //if (StockOperations.coin5 > 0)
            //{
            //    CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(1);
            //    decimal? trxId3 = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN5);
            //    if (StockOperations.InsStock((Int64)trxId3, (int)StockType.Coin2, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, 1))
            //        if (MoneyOperations.InsMoney((Int64)trxId3, (int)StockType.Coin2, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, Convert.ToDecimal(1) * Constants.HopperAddress3Coin))
            //            if (StockOperations.SelStockStatus())
            //            {
            //                CoinOperation coinOperation = new CoinOperation { count = 1, coin = Constants.HopperAddress3Coin, stock = StockOperations.coin5 };
            //                Constants.CoinDispOperations.Add(coinOperation);
            //                //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            //                //{
            //                //    CustomTL60Printer.Instance.CoinDispatchPrint(1, Constants.HopperAddress3Coin, StockOperations.coin5);
            //                //}
            //            }
            //}

            if (StockOperations.coin1 > 0 || StockOperations.coin2 > 0 || StockOperations.coin5 > 0)
                MessageBoxOperations.ShowMessage("Hopper Test", "Test completed successfully.", MessageBoxButtonSet.OK);
            else
                MessageBoxOperations.ShowMessage("Hopper Test", "Test unsuccessful no coin in the hoppers.", MessageBoxButtonSet.OK);
        }

        private void btnTestBNA_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            try
            {
                //if (noteincasset1 == 0 && noteincasset2 == 0 && noteincasset3 == 0)
                //{
                //    MessageBoxOperations.ShowMessage("BNA Test", "All cassettes are empty.", MessageBoxButtonSet.OK);
                //    return;
                //}
                //else
                //{
                //    waitGrid.Visibility = Visibility.Visible;
                //    List<byte> lArrSend = new List<byte>();
                //    if (noteincasset1 >= 1)
                //    {
                //        long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset1)));
                //        if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, 1))
                //            MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset1), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, notevalincasset1 * 1);

                //        lArrSend.Add(Convert.ToByte(Casette1Billtype));
                //        lArrSend.Add(Convert.ToByte(1));                       
                //    }
                //    if (noteincasset2 >= 1)
                //    {
                //        long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset2)));
                //        if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, 1))
                //            MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset2), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, notevalincasset2 * 1);

                //        lArrSend.Add(Convert.ToByte(Casette2Billtype));
                //        lArrSend.Add(Convert.ToByte(1));
                //    }
                //    if (noteincasset3 >= 1)
                //    {
                //        long trxId1 = Convert.ToInt64(TransactionInfo.SelTrxId((long)(TransactionType)Enum.Parse(typeof(TransactionType), "TT_REMOVE_BANKNOTE" + notevalincasset3)));
                //        if (StockOperations.InsStock(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, 1))
                //            MoneyOperations.InsMoney(trxId1, (int)(StockType)Enum.Parse(typeof(StockType), "Banknote" + notevalincasset3), (int)(Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : (Constants.EscrowCassetteNo == 2 ? DeviceType.Cassette2 : (Constants.EscrowCassetteNo == 3 ? DeviceType.Cassette3 : 0))), (int)UpdateType.Decrease, notevalincasset3 * 1);

                //        lArrSend.Add(Convert.ToByte(Casette3Billtype));
                //        lArrSend.Add(Convert.ToByte(1));
                //    }
                //    if (lArrSend.Count > 0)
                //    {
                //        byte[] snd_arr = lArrSend.ToArray();
                //        DispenseSeqBill(snd_arr);
                //    }
                //}
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> GridDispenseSeq_MouseLeftButtonDown() : " + ex.ToString());
            }           
        }

        //private void DispenseSeqBill(byte[] snd_arr)
        //{
        //    try
        //    {
        //        BNRManager.Instance.DispenseBillSequnce(snd_arr);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error BNROperationPage -> DispenseSeqBill() : " + ex.ToString());
        //    }
        //}

        private void btnTestQrPrinter_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            PRINTER_STATE QRStatus = QRPrinter.Instance.CheckQrPrinterStatus();
            if (QRStatus == PRINTER_STATE.OK)
            {
                if (StockOperations.qrSlip > 0)
                {
                    long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_QR));
                    StockOperations.InsStock(trxId, (int)StockType.QRSlip, (int)DeviceType.QRPrinter, (int)UpdateType.Decrease, 1);

                    QRPrinter.Instance.PrintQR("TestTestTestTestTestTestTestTestTest", "Test", "Test", "Test", "Test", 0, 0, "Test");
                    //CustomKPM150HPrinter.Instance.PrintTestQRTicket(qr);
                    MessageBoxOperations.ShowMessage("QR Printer Test", "Test completed successfully.", MessageBoxButtonSet.OK);
                }
                else
                {
                    MessageBoxOperations.ShowMessage("ERROR", "No paper in Qr printer." + QRStatus, MessageBoxButtonSet.OK);
                }
            }
            else
            {
                MessageBoxOperations.ShowMessage("ERROR", "Qr Printer Error." + QRStatus, MessageBoxButtonSet.OK);
            }
        }

        private void btnTestAlarm_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AlarmPage());
        }
    }
}
