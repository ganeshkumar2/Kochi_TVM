using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MaintancePage.xaml
    /// </summary>
    public partial class MaintancePage : Page
    {
        bool svcMode = false;
        public MaintancePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.menuItems.Contains(Parameters.MenuStrings.HopTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.Keypad1Test) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.Keypad2Test) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.QRTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.RcptPrntTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.RFIDRdTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.RptTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.ScrlDispTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.SpkrTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.TouchScreenTest) ||
                           Parameters.menuItems.Contains(Parameters.MenuStrings.VibTest))
            {
                btnTestDevice.Visibility = Visibility.Visible;
            }

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSetServiceMode_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            if (svcMode)
            {
                svcMode = false;
                btnSetServiceMode.Content = "Out Of Service Mode";
            }
            else
            {
                svcMode = true;
                btnSetServiceMode.Content = "Set Service Mode";
            }
        }

        private void btnGetUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnShutDown_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            var result = MessageBoxOperations.ShowMessage("SHUTDOWN", "Are you sure you want to Shutdown the TVM?", MessageBoxButtonSet.YesNo);

            if (result == Custom.MessageBoxResult.OK)
            {
                TVMUtility.ShutDownSystem();
            }
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            var result = MessageBoxOperations.ShowMessage("RESTART", "Are you sure you want to Restart the TVM?", MessageBoxButtonSet.YesNo);

            if (result == Custom.MessageBoxResult.OK)
            {
                TVMUtility.RestartSystem();
            }
        }

        private void btnTestDevice_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.DeviceTestsPage());
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
