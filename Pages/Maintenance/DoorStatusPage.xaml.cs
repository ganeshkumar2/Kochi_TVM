using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
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
    /// Interaction logic for DoorStatusPage.xaml
    /// </summary>
    public partial class DoorStatusPage : Page
    {
        private static Timer checkDeviceTimer;
        private static TimerCallback checkDeviceTimerDelegate;
        public DoorStatusPage()
        {
            InitializeComponent();

            checkDeviceTimerDelegate = new TimerCallback(CheckDeviceAction);
            checkDeviceTimer = new Timer(checkDeviceTimerDelegate, null, 1000, Constants.CheckDeviceTime);

            UpdDevStat();

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";

            DeviceInfoControl hopper51 = new DeviceInfoControl("Front Door", "Close");
            Grid.SetRow(hopper51, 0);
            Grid.SetColumn(hopper51, 0);
            operationGrid.Children.Add(hopper51);

            DeviceInfoControl BNA1 = new DeviceInfoControl("Hopper Door", "Close");
            Grid.SetRow(BNA1, 0);
            Grid.SetColumn(BNA1, 2);
            operationGrid.Children.Add(BNA1);

            DeviceInfoControl hopper21 = new DeviceInfoControl("Back Left Door", "Close");
            Grid.SetRow(hopper21, 2);
            Grid.SetColumn(hopper21, 2);
            operationGrid.Children.Add(hopper21);

            DeviceInfoControl Dispenser1 = new DeviceInfoControl("Back Right Door", "Close");
            Grid.SetRow(Dispenser1, 2);
            Grid.SetColumn(Dispenser1, 0);
            operationGrid.Children.Add(Dispenser1);           

            KMY200DoorAlarm.VibrationSensorInputEvent += new KMY200DoorAlarm.VibrationSensorInputEventHandler(CheckVibrationAction);
            KMY200DoorAlarm.HelpButtonInputEvent += new KMY200DoorAlarm.HelpButtonInputEventHandler(CheckHelpAction);
        }
        private void CheckVibrationAction()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    DeviceInfoControl Dispenser = new DeviceInfoControl("Vibration", "Detected");
                    Grid.SetRow(Dispenser, 4);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);
                }
                catch (Exception ex)
                {
                }
            }), DispatcherPriority.Background);
        }
        private void CheckHelpAction()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    DeviceInfoControl Dispenser = new DeviceInfoControl("Help Button", "Pressed");
                    Grid.SetRow(Dispenser, 4);
                    Grid.SetColumn(Dispenser, 2);
                    operationGrid.Children.Add(Dispenser);
                }
                catch (Exception ex)
                {
                }
            }), DispatcherPriority.Background);
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
           
            try
            {
                int status = KMY200DoorAlarm.Instance.GetStatus();
                Enums.DoorStatus doorStatus = (Enums.DoorStatus)(status);               

                if (doorStatus == Enums.DoorStatus.DOOR_ALL_CLOSE)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door","Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door","Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);
                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_1_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);
                    
                }
                if (doorStatus == Enums.DoorStatus.DOOR_2_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);
                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_3_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);
                }
                if (doorStatus == Enums.DoorStatus.DOOR_4_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                    
                }
                if (doorStatus == Enums.DoorStatus.DOOR_12_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_13_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_14_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_123_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_24_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                    
                }
                if (doorStatus == Enums.DoorStatus.DOOR_23_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Close");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_234_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                 
                }
                if (doorStatus == Enums.DoorStatus.DOOR_34_OPEN)
                {

                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Close");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);
                }
                if (doorStatus == Enums.DoorStatus.DOOR_134_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Close");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                    
                }
                if (doorStatus == Enums.DoorStatus.DOOR_124_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 0);
                    Grid.SetColumn(BNA, 2);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Close");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                   
                }
                if (doorStatus == Enums.DoorStatus.DOOR_1234_OPEN)
                {
                    DeviceInfoControl hopper5 = new DeviceInfoControl("Front Door", "Open");
                    Grid.SetRow(hopper5, 0);
                    Grid.SetColumn(hopper5, 0);
                    operationGrid.Children.Add(hopper5);

                    DeviceInfoControl BNA = new DeviceInfoControl("Hopper Door", "Open");
                    Grid.SetRow(BNA, 2);
                    Grid.SetColumn(BNA, 0);
                    operationGrid.Children.Add(BNA);

                    DeviceInfoControl hopper2 = new DeviceInfoControl("Back Left Door", "Open");
                    Grid.SetRow(hopper2, 2);
                    Grid.SetColumn(hopper2, 2);
                    operationGrid.Children.Add(hopper2);

                    DeviceInfoControl Dispenser = new DeviceInfoControl("Back Right Door", "Open");
                    Grid.SetRow(Dispenser, 2);
                    Grid.SetColumn(Dispenser, 0);
                    operationGrid.Children.Add(Dispenser);                 
                }
            }
            catch (Exception ex)
            {
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

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            KMY200DoorAlarm.VibrationSensorInputEvent -= new KMY200DoorAlarm.VibrationSensorInputEventHandler(CheckVibrationAction);
            KMY200DoorAlarm.HelpButtonInputEvent -= new KMY200DoorAlarm.HelpButtonInputEventHandler(CheckHelpAction);
            checkDeviceTimer.Dispose();
        }        
    }
}
