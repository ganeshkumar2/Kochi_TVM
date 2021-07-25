using Kochi_TVM.Business;
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

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for OperationPage.xaml
    /// </summary>
    public partial class OperationPage : Page
    {
        public OperationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.menuItems.Contains(Parameters.MenuStrings.CoinRep) ||
                Parameters.menuItems.Contains(Parameters.MenuStrings.CoinEmpt))
            {
                btnAddCoin.Visibility = Visibility.Visible;
            }

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.CashBoxRem) ||
                Parameters.menuItems.Contains(Parameters.MenuStrings.CashDump) ||
                Parameters.menuItems.Contains(Parameters.MenuStrings.CashEscCheck))
            {
                btnAddBanknote.Visibility = Visibility.Visible;
            }
            if (Parameters.menuItems.Contains(Parameters.MenuStrings.QrRep) ||
                Parameters.menuItems.Contains(Parameters.MenuStrings.RptRep))
            {
                btnAddTicket.Visibility = Visibility.Visible;
            }

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnChangeSlip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChangeQRSlip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddCoin_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.CoinOperationPage());
        }

        private void btnAddTicket_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AddCardAndQRPage());
        }

        private void btnAddBanknote_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.BNROperationPage());
        }

        private void btnMadeCollection_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
    }
}
