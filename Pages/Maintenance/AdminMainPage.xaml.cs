using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.Sensors;
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
    /// Interaction logic for AdminMainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        Timer checkDeviceTimer;
        TimerCallback checkDeviceTimerDelegate;
        public AdminMainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Constants.MaintenanceSeq = 0;
            Constants.IsMaintenanceActive = true;          

            KMY200DoorAlarm.Instance.SetAlarmClose();            
            if (Parameters.menuItems.Contains(Parameters.MenuStrings.QrRep) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.RptRep) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.CashEscCheck) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.CashDump) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.CashBoxRem)
             )
            {
                btnCollection.Visibility = Visibility.Visible;
            }
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Log Out";
            btnBack.Visibility = Visibility.Hidden;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {           
            if (checkDeviceTimer != null)
                checkDeviceTimer.Dispose();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminLoginPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Log Out", "Do you want to Log Out?", MessageBoxButtonSet.OKCancel);

            if (messageBoxResult == Custom.MessageBoxResult.OK)
            {
                if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                {
                    CustomTL60Printer.Instance.MaintenanceSummary(Constants.BNRTest, Constants.HopperTest, Constants.CoinAddOperations, Constants.CoinDispOperations, Constants.CoinEmptyOperations,
                                                                     Constants.QR_RPT_OperationsAdd, Constants.QR_RPT_OperationsDisp, Constants.QR_RPT_OperationsEmpty,Constants.SendToBoxes, Constants.Removecashbox, Constants.AddNotes);
                }
                int status = KMY200DoorAlarm.Instance.GetStatus();
                Enums.DoorStatus doorStatus = (Enums.DoorStatus)(status);
                if(doorStatus == Enums.DoorStatus.DOOR_ALL_CLOSE)
                {
                    KMY200DoorAlarm.Instance.SetAlarm();
                    TVMUtility.killExplorer();
                    NavigationService.Navigate(new Pages.MainPage());
                }
                else
                {
                    LedOperations.Close();
                    outofservice.Visibility = Visibility.Visible;
                    txtErrorCode.Text = "Door Open";
                    checkDeviceTimerDelegate = new TimerCallback(CheckDeviceAction);
                    checkDeviceTimer = new Timer(checkDeviceTimerDelegate, null, 1000, Constants.CheckDeviceTime);
                    //MessageBoxOperations.ShowMessage("Door", "Please close the all doors.", MessageBoxButtonSet.OKCancel);
                }
            }
        }
        private void CheckDeviceAction(object o)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    int status = KMY200DoorAlarm.Instance.GetStatus();
                    Enums.DoorStatus doorStatus = (Enums.DoorStatus)(status);
                    if (doorStatus == Enums.DoorStatus.DOOR_ALL_CLOSE)
                    {
                        KMY200DoorAlarm.Instance.SetAlarm();
                        TVMUtility.killExplorer();
                        NavigationService.Navigate(new Pages.MainPage());
                    }
                    else
                    {
                        txtErrorCode.Text = "Door Open";
                        outofservice.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                }
            }), DispatcherPriority.Background);
        }
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminInfoPage());
        }

        private void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.OperationPage());
        }

        private void btnMaintance_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.MaintancePage());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminSettingPage());
        }
    }
}
