using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AddCardAndQRPage.xaml
    /// </summary>
    public partial class AddCardAndQRPage : Page
    {
        public AddCardAndQRPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (StockOperations.SelStockStatus())
                UpdValOnScr();

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        int selected = 0;
        bool card = false;
        private void lblTypeCount_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            grdUserControl.Visibility = Visibility.Visible;
            selected = 1;
        }

        bool qr = false;
        private void lblQrCount_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            grdUserControl.Visibility = Visibility.Visible;
            selected = 2;
        }

        private void UpdValOnScr()
        {
            lblCardInfo.Content = "Dispenser Card Count : " + StockOperations.rpt.ToString();
            lblQrInfo.Content = "Qr Ticket Count : " + StockOperations.qrSlip.ToString();
            lblTypeCount.Text = "0";
            lblQrCount.Text = "0";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(lblTypeCount.Text) == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }
            else if (Convert.ToInt32(lblTypeCount.Text) > 9999)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "You can add maximum 9999 cards.", MessageBoxButtonSet.OKCancel);
                return;
            }

            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_RPT));
            if (StockOperations.InsStock(trxId, (int)StockType.Rpt, (int)DeviceType.Dispenser, (int)UpdateType.Increase, Convert.ToInt32(lblTypeCount.Text)))
                if (StockOperations.SelStockStatus())
                {
                    if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                    {
                        CustomTL60Printer.Instance.AddPrintQRRPT(Convert.ToInt32(lblTypeCount.Text), TransactionType.TT_RPT, StockOperations.rpt);
                    }
                    UpdValOnScr();
                }
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(lblTypeCount.Text) == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (StockOperations.rpt == 0)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "There is no card in the dispenser!", MessageBoxButtonSet.OKCancel);
            else if (Convert.ToInt32(lblTypeCount.Text) > StockOperations.rpt)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Less value cant be greater than total value!", MessageBoxButtonSet.OKCancel);
            else
            {
                long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_PRT));
                if (StockOperations.InsStock(trxId, (int)StockType.Rpt, (int)DeviceType.Dispenser, (int)UpdateType.Decrease, Convert.ToInt32(lblTypeCount.Text)))
                    if (StockOperations.SelStockStatus())
                    {
                        if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                        {
                            CustomTL60Printer.Instance.DispatchQRRPT(Convert.ToInt32(lblTypeCount.Text), TransactionType.TT_RPT, StockOperations.rpt);
                        }
                        UpdValOnScr();
                    }
            }
        }

        private void btnQrAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(lblQrCount.Text) == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(lblQrCount.Text) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            else if (Convert.ToInt32(lblQrCount.Text) > 9999)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "You can add maximum 9999 tickets.", MessageBoxButtonSet.OKCancel);
                return;
            }

            decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_QR);
            if (StockOperations.InsStock((Int64)trxId, (int)StockType.QRSlip, (int)DeviceType.QRPrinter, (int)UpdateType.Increase, Convert.ToInt32(lblQrCount.Text)))
                if (StockOperations.SelStockStatus())
                {
                    if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                    {
                        CustomTL60Printer.Instance.AddPrintQRRPT(Convert.ToInt32(lblQrCount.Text), TransactionType.TT_QR, StockOperations.qrSlip);
                    }
                    MessageBoxOperations.ShowMessage("ADD QR", "Added Type : QR" + "\nAdded Count : " + lblQrCount.Text.ToString(), MessageBoxButtonSet.OK);
                    UpdValOnScr();
                }
        }

        private void btnQrMinus_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(lblQrCount.Text) == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(lblQrCount.Text) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (StockOperations.qrSlip == 0)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "There is no card in the dispenser!", MessageBoxButtonSet.OKCancel);
            else if (Convert.ToInt32(lblQrCount.Text) > StockOperations.qrSlip)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Less value cant be greater than total value!", MessageBoxButtonSet.OKCancel);
            else
            {
                long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_QR));
                if (StockOperations.InsStock(trxId, (int)StockType.QRSlip, (int)DeviceType.QRPrinter, (int)UpdateType.Decrease, Convert.ToInt32(lblQrCount.Text)))
                    if (StockOperations.SelStockStatus())
                    {
                        if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                        {
                            CustomTL60Printer.Instance.DispatchQRRPT(Convert.ToInt32(lblQrCount.Text), TransactionType.TT_QR, StockOperations.qrSlip);
                        }
                        MessageBoxOperations.ShowMessage("LESS QR", "Less Type : QR" + "\nLess Count : " + lblQrCount.Text.ToString(), MessageBoxButtonSet.OK);
                        UpdValOnScr();
                    }
            }
        }

        private void btnEmpty_Click(object sender, RoutedEventArgs e)
        {
            if (StockOperations.rpt == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Stock is already empty.", MessageBoxButtonSet.OKCancel);
                return;
            }
            MessageBoxOperations.ShowMessage("ATTENTION!!", "Please waiting while dispense all card.", MessageBoxButtonSet.OKCancel);
            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_EMPTY_RPT));
            int stock = StockOperations.rpt;
            if (StockOperations.InsStock(trxId, (int)StockType.Rpt, (int)DeviceType.Dispenser, (int)UpdateType.Empty, StockOperations.rpt))
                if (StockOperations.SelStockStatus())
                {
                    if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                    {
                        CustomTL60Printer.Instance.EmptyQRRPT(stock, TransactionType.TT_RPT);
                    }
                    UpdValOnScr();
                }
        }

        private void btnQrEmpty_Click(object sender, RoutedEventArgs e)
        {
            if (StockOperations.qrSlip == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Stock is already empty.", MessageBoxButtonSet.OKCancel);
                return;
            }
            MessageBoxOperations.ShowMessage("ATTENTION!!", "Please Remove QR Slips.", MessageBoxButtonSet.OKCancel);
            long trxId = Convert.ToInt64(TransactionInfo.SelTrxId((long)TransactionType.TT_EMPTY_QR));
            int stock = StockOperations.qrSlip;
            if (StockOperations.InsStock(trxId, (int)StockType.QRSlip, (int)DeviceType.QRPrinter, (int)UpdateType.Empty, StockOperations.qrSlip))
                if (StockOperations.SelStockStatus())
                {
                    if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                    {
                        CustomTL60Printer.Instance.EmptyQRRPT(stock, TransactionType.TT_QR);
                    }
                    UpdValOnScr();
                }
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

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (selected == 1 && numberpad.txtTypeNumber.Text != "0")
            {
                lblTypeCount.Text = numberpad.txtTypeNumber.Text;
            }
            else if (selected == 1 && numberpad.txtTypeNumber.Text == "0")
            {
                lblTypeCount.Text = numberpad.txtTypeNumber.Text;
            }

            if (selected == 2 && numberpad.txtTypeNumber.Text != "0")
            {
                lblQrCount.Text = numberpad.txtTypeNumber.Text;
            }
            else if (selected == 2 && numberpad.txtTypeNumber.Text == "0")
            {
                lblQrCount.Text = numberpad.txtTypeNumber.Text;
            }

            selected = 0;
            numberpad.number = 0;
            numberpad.txtTypeNumber.Text = "0";
            grdUserControl.Visibility = Visibility.Collapsed;
        }
    }
}
