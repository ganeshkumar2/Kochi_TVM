using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Sensors;
using Kochi_TVM.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for AlarmPage.xaml
    /// </summary>
    public partial class AlarmPage : Page
    {
        public AlarmPage()
        {
            InitializeComponent();
        }

        private void btnSetAlarm_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            KMY200DoorAlarm.Instance.SetAlarm();
            MessageBoxOperations.ShowMessage("ALARM", "Alarm active", MessageBoxButtonSet.OK);
        }

        private void btnCancelAlarm_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            KMY200DoorAlarm.Instance.SetAlarmClose();
            MessageBoxOperations.ShowMessage("ALARM", "Alarm deactivate", MessageBoxButtonSet.OK);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.DeviceTestsPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
    }
}
