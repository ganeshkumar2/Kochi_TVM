using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Sensors;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Kochi_TVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ILog log = LogManager.GetLogger(typeof(MainWindow).Name);

        private static Timer dateTimeTimer;
        private static TimerCallback dateTimeTimerDelegate;

        private static Timer maintanceTimeTimer;
        private static TimerCallback maintanceTimerDelegate;
        public MainWindow()
        {
            InitializeComponent();           
            try
            {
                ConfigLog4net();
                log.Debug("***TVM Application Started***");
                log.Debug("TVM App Version: " + Constants.appVersion);
                TVMUtility.killExplorer();
                MultiLanguage.Init("EN");
                InitialTimer();
                Stations.FillStationList();
                Stations.FillCurrentStation();
                //Parameters.TVMDynamic.FillOrUpdateParameters();
                DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));
                lStation.Content = Stations.currentStation.name.ToUpper();

                if (ConfigurationManager.AppSettings["VoiceEnable"].ToString() == "True")
                    Constants.IsVoiceEnabled = true;
                else
                    Constants.IsVoiceEnabled = false;                
                //frameHomeMain.Navigate(new Pages.MainPage());
                frameHomeMain.Navigate(new Pages.OutOfSevicePage());
                KMY200DoorAlarm.HelpButtonInputEvent += new KMY200DoorAlarm.HelpButtonInputEventHandler(CheckHelpAction);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        private void CheckHelpAction()
        {
            bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                         string.Format("Help button pressed!"));

            log.Debug("InsNStationAlarm Help button pressed --> result : " + (result == true ? "true" : "false"));
        }
        private void ConfigLog4net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        private void InitialTimer()
        {
            try
            {
                dateTimeTimerDelegate = new TimerCallback(DateTimeTimerAction);
                dateTimeTimer = new Timer(dateTimeTimerDelegate, null, 0, 1000);
            }
            catch (Exception ex)
            {
                log.Error("Error  OptionPage -> InitialTimer() : " + ex.ToString());
            }
        }
        private void DateTimeTimerAction(object obj)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    lDate.Content = String.Format("{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-GB")));
                }));
            }
            catch (Exception ex)
            {
                log.Error("Error MainWindow -> DateTimeTimerAction() : " + ex.ToString());
            }
        }
        
        private void gridLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Constants.IsMaintenanceActive)
                return;

            if (Constants.MaintenanceSeq >= 1)
                return;

            TVMUtility.PlayClick();

            maintanceTimerDelegate = new TimerCallback(maintanceTimerAction);
            maintanceTimeTimer = new Timer(maintanceTimerDelegate, null, 2000, 0);

            Constants.MaintenanceSeq++;            
        }

        private void gridDT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Constants.IsMaintenanceActive)
                return;

            if (Constants.MaintenanceSeq == 0)
                return;

            if (Constants.MaintenanceSeq == 1)
                return;

            TVMUtility.PlayClick();
            Constants.MaintenanceSeq++;
            if (Constants.MaintenanceSeq == 3)
            {
                maintanceTimeTimer.Dispose();
                Constants.MaintenanceSeq = 0;
                Constants.IsMaintenanceActive = true;
                frameHomeMain.Navigate(new Pages.Maintenance.AdminLoginPage());
            }
        }

        private void gridBankLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Constants.IsMaintenanceActive)
                return;

            if (Constants.MaintenanceSeq == 0)
                return;

            if (Constants.MaintenanceSeq == 2)
                return;          

            TVMUtility.PlayClick();
            Constants.MaintenanceSeq++;
        }
        private void maintanceTimerAction(object obj)
        {
            try
            {
                maintanceTimeTimer.Dispose();
                Constants.MaintenanceSeq = 0;
            }
            catch (Exception ex)
            {
                log.Error("Error MainWindow -> maintanceTimerAction() : " + ex.ToString());
            }
        }
    }
}
