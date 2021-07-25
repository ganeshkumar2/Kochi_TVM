using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for NoChangeOrReceiptPage.xaml
    /// </summary>
    public partial class NoChangeOrReceiptPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(NoChangeOrReceiptPage).Name);

        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        bool noReceiptprinter = false;
        bool nochageMode = false;
        public NoChangeOrReceiptPage(bool nochage,bool noreceipt)
        {
            InitializeComponent();
            lblNoChange.Content = MultiLanguage.GetText("NoChange");
            lblAvailable.Content = MultiLanguage.GetText("AvailableCoin");
            lblNoReceipt.Content = MultiLanguage.GetText("NoReceipt");
            btnFinish.Content = MultiLanguage.GetText("cancel");
            btnOk.Content = MultiLanguage.GetText("Ok");
            noReceiptprinter = noreceipt;
            nochageMode = nochage;
            if (nochage)
            {
                grdNoChangeMode.Visibility = Visibility.Visible;
            }
            else if (noreceipt)
            {
                grdNoReceiptPrinterMode.Visibility = Visibility.Visible;
            }
            initialTimer();
        }
        private void initialTimer()
        {
            try
            {
                idleTimerDelegate = new TimerCallback(NavigateAction);
                idleTimer = new Timer(idleTimerDelegate, null, 0, 1000);
            }
            catch (Exception ex)
            {
                log.Error("Error TicketTypePage -> initialTimer() : " + ex.ToString());
            }
        }
        private void NavigateAction(object obj)
        {
            try
            {
                var idleTime = IdleTimeDetector.GetIdleTimeInfo();

                if (idleTime.IdleTime.TotalMinutes >= Constants.SystemIdleTimeout)
                {
                    idleTimer.Dispose();
                    this.Dispatcher.Invoke(() =>
                    {
                        NavigationService.Navigate(new Pages.MainPage());
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Error TicketTypePage -> DateTimeTimerAction() : " + ex.ToString());
            }
        }
        int i = 0;
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            grdNoChangeMode.Visibility = Visibility.Hidden;
            if (nochageMode && noReceiptprinter)
            {
                grdNoReceiptPrinterMode.Visibility = Visibility.Visible;
                i++;
                if(i==2)
                {
                    i = 0;
                    ElectronicJournal.OrderStarted();
                    NavigationService.Navigate(new Pages.JourneyTypePage());
                }
            }
            else
            {
                ElectronicJournal.OrderStarted();
                NavigationService.Navigate(new Pages.JourneyTypePage());
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (idleTimer != null)
                idleTimer.Dispose();
        }
    }
}
