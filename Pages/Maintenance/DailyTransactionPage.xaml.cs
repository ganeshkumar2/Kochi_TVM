using Kochi_TVM.Business;
using Kochi_TVM.Models;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for DailyTransactionPage.xaml
    /// </summary>
    public partial class DailyTransactionPage : Page
    {
        int QRSJTCashCount = 0;
        int QRSJTCashAmount = 0;
        int QRSJTNonCashCount = 0;
        int QRSJTNonCashAmount = 0;
        int QRRJTCashCount = 0;
        int QRRJTCashAmount = 0;
        int QRRJTNonCashCount = 0;
        int QRRJTNonCashAmount = 0;
        int QRGroupCashCount = 0;
        int QRGroupCashAmount = 0;
        int QRGroupNonCashCount = 0;
        int QRGroupNonCashAmount = 0;
        int RPTSJTCashCount = 0;
        int RPTSJTCashAmount = 0;
        int RPTSJTNonCashCount = 0;
        int RPTSJTNonCashAmount = 0;
        int RPTRJTCashCount = 0;
        int RPTRJTCashAmount = 0;
        int RPTRJTNonCashCount = 0;
        int RPTRJTNonCashAmount = 0;
        int RPTGroupCashCount = 0;
        int RPTGroupCashAmount = 0;
        int RPTGroupNonCashCount = 0;
        int RPTGroupNonCashAmount = 0;
        int RPTDayPassCashCount = 0;
        int RPTDayPassCashAmount = 0;
        int RPTDayPassNonCashCount = 0;
        int RPTDayPassNonCashAmount = 0;
        int RPTWeekendPassCashCount = 0;
        int RPTWeekendPassCashAmount = 0;
        int RPTWeekendPassNonCashCount = 0;
        int RPTWeekendPassNonCashAmount = 0;
        public DailyTransactionPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
                lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
                btnFinish.Content = "Cancel";

                using (var context = new TVM_Entities())
                {
                    DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                    DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                    var trxData = context.sp_SelShiftPaymentReport(Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), Stations.currentStation.id, 0, 0, startDate, endDate).ToList();
                    foreach (var data in trxData)
                    {
                        if (Convert.ToString(data.Transaction) == "QR SJT-CASH")
                        {
                            QRSJTCashCount = Convert.ToInt32(data.Count);
                            QRSJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "QR SJT-NonCASH")
                        {
                            QRSJTNonCashCount = Convert.ToInt32(data.Count);
                            QRSJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        if (Convert.ToString(data.Transaction) == "QR RJT-CASH")
                        {
                            QRRJTCashCount = Convert.ToInt32(data.Count);
                            QRRJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "QR RJT-NonCASH")
                        {
                            QRRJTNonCashCount = Convert.ToInt32(data.Count);
                            QRRJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        if (Convert.ToString(data.Transaction) == "Group Ticket-CASH")
                        {
                            QRGroupCashCount = Convert.ToInt32(data.Count);
                            QRGroupCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT SJT-CASH")
                        {
                            RPTSJTCashCount = Convert.ToInt32(data.Count);
                            RPTSJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT SJT-NonCASH")
                        {
                            RPTSJTNonCashCount = Convert.ToInt32(data.Count);
                            RPTSJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT Group-CASH")
                        {
                            RPTGroupCashCount = Convert.ToInt32(data.Count);
                            RPTGroupCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT Group-NonCASH")
                        {
                            RPTGroupNonCashCount = Convert.ToInt32(data.Count);
                            RPTGroupNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT RJT-CASH")
                        {
                            RPTRJTCashCount = Convert.ToInt32(data.Count);
                            RPTRJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT RJT-NonCASH")
                        {
                            RPTRJTNonCashCount = Convert.ToInt32(data.Count);
                            RPTRJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT Day Pass-CASH")
                        {
                            RPTDayPassCashCount = Convert.ToInt32(data.Count);
                            RPTDayPassCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT Day Pass-NonCASH")
                        {
                            RPTDayPassNonCashCount = Convert.ToInt32(data.Count);
                            RPTDayPassNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT Weekend Pass-CASH")
                        {
                            RPTWeekendPassCashCount = Convert.ToInt32(data.Count);
                            RPTWeekendPassCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT Weekend Pass-NonCASH")
                        {
                            RPTWeekendPassNonCashCount = Convert.ToInt32(data.Count);
                            RPTWeekendPassNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }            

            lblQRSJTCashCount.Content = QRSJTCashCount;
            lblQRSJTCashAmount.Content = Conversion.MoneyFormat(QRSJTCashAmount);

            //lblQRSJTNonCashCount.Content = QRSJTNonCashCount;
            //lblQRSJTNonCashAmount.Content = Conversion.MoneyFormat(QRSJTNonCashAmount);

            lblQRSJTTotalCount.Content = QRSJTCashCount + QRSJTNonCashCount;
            lblQRSJTTotalAmount.Content = Conversion.MoneyFormat(QRSJTCashAmount + QRSJTNonCashAmount);

            lblQRRJTCashCount.Content = QRRJTCashCount;
            lblQRRJTCashAmount.Content = Conversion.MoneyFormat(QRRJTCashAmount);

            //lblQRRJTNonCashCount.Content = QRRJTNonCashCount;
            //lblQRRJTNonCashAmount.Content = Conversion.MoneyFormat(QRRJTNonCashAmount);

            lblQRRJTTotalCount.Content = QRRJTCashCount + QRRJTNonCashCount;
            lblQRRJTTotalAmount.Content = Conversion.MoneyFormat(QRRJTCashAmount + QRRJTNonCashAmount);         

            lblQRGroupCashCount.Content = QRGroupCashCount;
            lblQRGroupCashAmount.Content = Conversion.MoneyFormat(QRGroupCashAmount);

            lblQRGroupTotalCount.Content = QRGroupCashCount + QRGroupNonCashCount;
            lblQRGroupTotalAmount.Content = Conversion.MoneyFormat(QRGroupCashAmount + QRGroupNonCashAmount);

            lblRPTQRSJTCashCount.Content = RPTSJTCashCount;
            lblRPTQRSJTCashAmount.Content = Conversion.MoneyFormat(RPTSJTCashAmount);

            lblRPTQRSJTTotalCount.Content = RPTSJTCashCount;
            lblRPTQRSJTTotalAmount.Content = Conversion.MoneyFormat(RPTSJTCashAmount);

            lblRPTQRGroupCashCount.Content = RPTGroupCashCount;
            lblRPTQRGroupCashAmount.Content = Conversion.MoneyFormat(RPTGroupCashAmount);

            lblRPTQRGroupTotalCount.Content = RPTGroupCashCount;
            lblRPTQRGroupTotalAmount.Content = Conversion.MoneyFormat(RPTGroupCashAmount);

            lblRPTDayPassCashCount.Content = RPTDayPassCashCount;
            lblRPTDayPassCashAmount.Content = Conversion.MoneyFormat(RPTDayPassCashAmount);

            lblRPTDayPassTotalCount.Content = RPTDayPassCashCount;
            lblRPTDayPassTotalAmount.Content = Conversion.MoneyFormat(RPTDayPassCashAmount);

            lblRPTWeekendCashCount.Content = RPTWeekendPassCashCount;
            lblRPTWeekendCashAmount.Content = Conversion.MoneyFormat(RPTWeekendPassCashAmount);

            lblRPTWeekendTotalCount.Content = RPTWeekendPassCashCount;
            lblRPTWeekendTotalAmount.Content = Conversion.MoneyFormat(RPTWeekendPassCashAmount);

            lblSummaryCount.Content = QRSJTCashCount + QRSJTNonCashCount + QRRJTCashCount + QRRJTNonCashCount + QRGroupCashCount + QRGroupNonCashCount+ RPTSJTCashCount + RPTGroupCashCount + RPTDayPassCashCount+ RPTWeekendPassCashCount;
            lblSummaryAmount.Content = Conversion.MoneyFormat(QRSJTCashAmount + QRSJTNonCashAmount+ QRRJTCashAmount + QRRJTNonCashAmount+ QRGroupCashAmount + QRGroupNonCashAmount+ RPTSJTCashAmount + RPTGroupCashAmount + RPTDayPassCashAmount+ RPTWeekendPassCashAmount);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            {
                CustomTL60Printer.Instance.StandAloneReceipt();
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
