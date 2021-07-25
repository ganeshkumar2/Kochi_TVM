using Kochi_TVM.Business;
using Kochi_TVM.PID;
using Kochi_TVM.Utils;
using log4net;
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

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for StationClosedPage.xaml
    /// </summary>
    public partial class StationClosedPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(StationClosedPage).Name);

        public static DateTime startDate;
        public static DateTime endDate;

        private static Timer checkDeviceTimer;
        private static TimerCallback checkDeviceTimerDelegate;
        public StationClosedPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Constants.IsMaintenanceActive = false;

                LedOperations.Close();

                checkDeviceTimerDelegate = new TimerCallback(CheckDeviceAction);
                checkDeviceTimer = new Timer(checkDeviceTimerDelegate, null, 1000, Constants.CheckDeviceTime);
            }
            catch (Exception ex)
            {
                log.Error("Error StationClosedPage -> Page_Loaded() : " + ex.ToString());
            }
        }

        private void CheckDeviceAction(object o)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    checkDateChanges();
                    if ((startDate <= DateTime.Now) && (endDate >= DateTime.Now))
                    {
                        NavigationService.Navigate(new Pages.OutOfSevicePage());
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error StationClosedPage -> CheckDeviceAction() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }
        private void checkDateChanges()
        {
            startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
            endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkDeviceTimer != null)
                    checkDeviceTimer.Dispose();
            }
            catch (Exception ex)
            {
                log.Error("Error StationClosedPage -> Page_Loaded() : " + ex.ToString());
            }
        }
    }
}
