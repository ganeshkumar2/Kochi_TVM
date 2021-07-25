using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.RptDispenser;
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
using System.Windows.Threading;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for DeviceInfoPage.xaml
    /// </summary>
    public partial class DeviceInfoPage : Page
    {
        private static Timer checkDeviceTimer;
        private static TimerCallback checkDeviceTimerDelegate;
        public DeviceInfoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            checkDeviceTimerDelegate = new TimerCallback(CheckDeviceAction);
            checkDeviceTimer = new Timer(checkDeviceTimerDelegate, null, 1000, Constants.CheckDeviceTime);

            UpdDevStat();

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";

        }

        private void CheckDeviceAction(object o)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    UpdDevStat();
                }
                catch (Exception ex)
                {
                }
            }), DispatcherPriority.Background);
        }

        private void UpdDevStat()
        {
            BNRManager.Instance.PollingAction();
            //add real value
            //receipt printer
            var printerReceipt = string.Empty;

            try
            {
                printerReceipt = CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK ? "OK" : "ERROR";
            }
            catch (Exception ex)
            {
                printerReceipt = "NOT READED..";
                //log.Write("Receipt Printer: " + ex.ToString());
            }

            DeviceInfoControl receiptPrinter = new DeviceInfoControl("Receipt Printer", printerReceipt);
            Grid.SetRow(receiptPrinter, 0);
            Grid.SetColumn(receiptPrinter, 0);
            operationGrid.Children.Add(receiptPrinter);

            //QR printer
            try
            {
                string status = "";
                status = QRPrinter.Instance.CheckQrPrinterStatus() == Enums.PRINTER_STATE.OK ? "OK" : "ERROR";
                DeviceInfoControl qrPrinter = new DeviceInfoControl("QR Printer", status);
                Grid.SetRow(qrPrinter, 2);
                Grid.SetColumn(qrPrinter, 0);
                operationGrid.Children.Add(qrPrinter);
            }
            catch (Exception ex)
            {
            }

            //BNA
            try
            {
                DeviceInfoControl BNA = new DeviceInfoControl("Banknote Acceptor", Constants.BNRStatus == "DISABLED" ? "OK" : "ERROR");
                Grid.SetRow(BNA, 0);
                Grid.SetColumn(BNA, 2);
                operationGrid.Children.Add(BNA);
            }
            catch (Exception ex)
            {
            }

            //Dispenser
            try
            {
                DISP_STAT stat = DISP_STAT.STACKER_FULL;
                RPTOperations.GetStatus(ref stat);
                byte rptstatus = 1;
                if (stat == DISP_STAT.STACKER_UNKNOWN)
                {
                    rptstatus = 0;
                }
                else if ((stat == DISP_STAT.STACKER_NOCARD) && (!RPTOperations.IsCardInRFCardOperationPosition()))
                {
                    rptstatus = 0;
                }
                Parameters.TVMStatic.AddOrUpdateParameter("rptDispenserStatus", rptstatus.ToString());
                DeviceInfoControl Dispenser = new DeviceInfoControl("RPT Dispenser", rptstatus == 1 ? "OK" : "ERROR");
                Grid.SetRow(Dispenser, 4);
                Grid.SetColumn(Dispenser, 0);
                operationGrid.Children.Add(Dispenser);
            }
            catch (Exception ex)
            {
            }

            ////EMV POS
            //try
            //{
            //    DeviceInfoControl emvPOS = new DeviceInfoControl("EMV", "ERROR");
            //    Grid.SetRow(emvPOS, 8);
            //    Grid.SetColumn(emvPOS, 0);
            //    operationGrid.Children.Add(emvPOS);
            //}
            //catch (Exception ex)
            //{
            //}

            ////Kochi-1 Card POS
            //try
            //{
            //    DeviceInfoControl cardPOS = new DeviceInfoControl("Card POS", "ERROR");
            //    Grid.SetRow(cardPOS, 0);
            //    Grid.SetColumn(cardPOS, 2);
            //    operationGrid.Children.Add(cardPOS);
            //}
            //catch (Exception ex)
            //{
            //}

            //Hopper 1
            try
            {
                DeviceInfoControl hopper1 = new DeviceInfoControl("Hopper 1 Rs.", CCTalkManager.Instance.coinHopperEV4000_1.Manufacture != null ? "OK" : "ERROR");
                Grid.SetRow(hopper1, 2);
                Grid.SetColumn(hopper1, 2);
                operationGrid.Children.Add(hopper1);
            }
            catch (Exception ex)
            {
            }

            //Hopper 2
            try
            {
                DeviceInfoControl hopper2 = new DeviceInfoControl("Hopper 2 Rs.", CCTalkManager.Instance.coinHopperEV4000_2.Manufacture != null ? "OK" : "ERROR");
                Grid.SetRow(hopper2, 4);
                Grid.SetColumn(hopper2, 2);
                operationGrid.Children.Add(hopper2);
            }
            catch (Exception ex)
            {
            }

            //Hopper 5
            try
            {
                DeviceInfoControl hopper5 = new DeviceInfoControl("Hopper 5 Rs.", CCTalkManager.Instance.coinHopperEV4000_3.Manufacture != null ? "OK" : "ERROR");
                Grid.SetRow(hopper5, 6);
                Grid.SetColumn(hopper5, 2);
                operationGrid.Children.Add(hopper5);
            }
            catch (Exception ex)
            {
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            checkDeviceTimer.Dispose();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            {
                CustomTL60Printer.Instance.TVMDeviceInfoReceipt();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminInfoPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
    }
}
