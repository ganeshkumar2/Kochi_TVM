using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
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
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for DeviceTestsPage.xaml
    /// </summary>
    public partial class DeviceTestsPage : Page
    {
        public DeviceTestsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StockOperations.SelStockStatus();

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
           
            if (StockOperations.coin1 > 0)
            {
                CCTalkManager.Instance.coinHopperEV4000_1.DispenseCoins(1);
                decimal? trxId1 = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN1);
                if (StockOperations.InsStock((Int64)trxId1, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, 1))
                    if (MoneyOperations.InsMoney((Int64)trxId1, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, Convert.ToDecimal(1) * Constants.HopperAddress1Coin))
                        if (StockOperations.SelStockStatus())
                        {
                            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                            {
                                CustomTL60Printer.Instance.CoinDispatchPrint(1, Constants.HopperAddress1Coin, StockOperations.coin1);
                            }
                        }
            }

            if (StockOperations.coin2 > 0)
            {
                CCTalkManager.Instance.coinHopperEV4000_2.DispenseCoins(1);
                decimal? trxId2 = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN2);
                if (StockOperations.InsStock((Int64)trxId2, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, 1))
                    if (MoneyOperations.InsMoney((Int64)trxId2, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, Convert.ToDecimal(1) * Constants.HopperAddress2Coin))
                        if (StockOperations.SelStockStatus())
                        {
                            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                            {
                                CustomTL60Printer.Instance.CoinDispatchPrint(1, Constants.HopperAddress2Coin, StockOperations.coin2);
                            }
                        }
            }

            if (StockOperations.coin5 > 0)
            {
                CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(1);
                decimal? trxId3 = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN5);
                if (StockOperations.InsStock((Int64)trxId3, (int)StockType.Coin2, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, 1))
                    if (MoneyOperations.InsMoney((Int64)trxId3, (int)StockType.Coin2, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, Convert.ToDecimal(1) * Constants.HopperAddress3Coin))
                        if (StockOperations.SelStockStatus())
                        {
                            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                            {
                                CustomTL60Printer.Instance.CoinDispatchPrint(1, Constants.HopperAddress3Coin, StockOperations.coin5);
                            }
                        }
            }
            if (StockOperations.coin1 > 0 || StockOperations.coin2 > 0 || StockOperations.coin5 > 0)
                MessageBoxOperations.ShowMessage("Hopper Test", "Test completed successfully.", MessageBoxButtonSet.OK);
            else
                MessageBoxOperations.ShowMessage("Hopper Test", "Test unsuccessful no coin in the hoppers.", MessageBoxButtonSet.OK);
        }

        private void btnTestBNA_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            BNRManager.Instance.PollingAction();
            if (Constants.BNRStatus == "DISABLED")
            {
                CustomTL60Printer.Instance.TestBNA();
                MessageBoxOperations.ShowMessage("BNA Test", "Test completed successfully.", MessageBoxButtonSet.OK);
            }
            else
            {
                MessageBoxOperations.ShowMessage("BNA Test", "BNA error.", MessageBoxButtonSet.OK);
            }
        }

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
