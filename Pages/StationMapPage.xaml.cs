using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for StationMapPage.xaml
    /// </summary>
    public partial class StationMapPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(StationMapPage).Name);
        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        public StationMapPage()
        {            
            InitializeComponent();
            if (Ticket.language == Languages.English || Ticket.language == Languages.Hint)
            {
                btnStation1.FontSize = 14;
                btnStation2.FontSize = 14;
                btnStation3.FontSize = 14;
                btnStation4.FontSize = 14;
                btnStation5.FontSize = 14;
                btnStation6.FontSize = 14;
                btnStation7.FontSize = 14;
                btnStation8.FontSize = 14;
                btnStation9.FontSize = 14;
                btnStation10.FontSize = 14;
                btnStation11.FontSize = 14;
                btnStation12.FontSize = 14;
                btnStation13.FontSize = 14;
                btnStation14.FontSize = 14;
                btnStation15.FontSize = 14;
                btnStation16.FontSize = 14;
                btnStation17.FontSize = 14;
                btnStation18.FontSize = 14;
                btnStation19.FontSize = 14;
                btnStation20.FontSize = 14;
                btnStation21.FontSize = 14;
                btnStation22.FontSize = 14;
                lblDestination.FontSize = 14;
                lblNoOfTickets.FontSize = 14;
                lblAmount.FontSize = 14;
                lblType.FontSize = 14;
                lblDisType.FontSize = 14;
            }
            else
            {
                btnStation1.FontSize = 12;
                btnStation2.FontSize = 12;
                btnStation3.FontSize = 12;
                btnStation4.FontSize = 12;
                btnStation5.FontSize = 12;
                btnStation6.FontSize = 12;
                btnStation7.FontSize = 12;
                btnStation8.FontSize = 12;
                btnStation9.FontSize = 12;
                btnStation10.FontSize = 12;
                btnStation11.FontSize = 12;
                btnStation12.FontSize = 12;
                btnStation13.FontSize = 12;
                btnStation14.FontSize = 12;
                btnStation15.FontSize = 12;
                btnStation16.FontSize = 12;
                btnStation17.FontSize = 12;
                btnStation18.FontSize = 12;
                btnStation19.FontSize = 12;
                btnStation20.FontSize = 12;
                btnStation21.FontSize = 12;
                btnStation22.FontSize = 12;
                lblDestination.FontSize = 12;
                lblNoOfTickets.FontSize = 12;
                lblAmount.FontSize = 12;
                lblType.FontSize = 12;
                lblDisType.FontSize = 12;
            }
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(3, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(3, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(3, null, null, "IN");
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            switch (Ticket.journeyType)
            {
                case JourneyType.Period_Pass:
                case JourneyType.Trip_Pass:
                case JourneyType.Topup:
                    Ticket.journeyType = JourneyType.Unknown;
                    NavigationService.Navigate(new Pages.CardOperationPage());
                    break;
                case JourneyType.Day_Pass:
                case JourneyType.Weekend_Pass:
                case JourneyType.Group_Ticket:
                case JourneyType.RJT:
                case JourneyType.SJT:
                    NavigationService.Navigate(new Pages.JourneyTypePage());
                    break;
                default:
                    break;
            }            
        }

        private void btnStationList_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnStation_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            int selectedStationId = 0;
            SetDefaultStatus();
            selectedStationId = Convert.ToInt32(((Button)sender).Tag);
            if (Stations.stationList.ContainsKey(selectedStationId))
                SetStation(selectedStationId);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                initialTimer();
                Message();
                btnBack.Content = MultiLanguage.GetText("back");
                btnFinish.Content = MultiLanguage.GetText("cancel");
                lblDisType.Content = MultiLanguage.GetText("DispTicketType");
                btnStationList.Content = MultiLanguage.GetText("showStationList");
                lblDestination.Content = MultiLanguage.GetText("DispDestination");
                lblNoOfTickets.Content = MultiLanguage.GetText("DispNoOfTickets");
                lblAmount.Content = MultiLanguage.GetText("DispAmount");
                SetHeaderText();
                SetDefaultStatus();
                ListStationsInGrid();
                switch (Ticket.journeyType)
                {
                    case JourneyType.SJT:
                        lblType.Content = MultiLanguage.GetText("SJT");
                        break;
                    case JourneyType.RJT:
                        lblType.Content = MultiLanguage.GetText("RJT");
                        break;
                    case JourneyType.Group_Ticket:
                        lblType.Content = MultiLanguage.GetText("GRO");
                        break;
                    case JourneyType.Day_Pass:
                        lblType.Content = MultiLanguage.GetText("onedaypass");
                        break;
                    case JourneyType.Weekend_Pass:
                        lblType.Content = MultiLanguage.GetText("weekenddaypass");
                        break;
                    case JourneyType.Period_Pass:
                    case JourneyType.Trip_Pass:
                        grdInfo.Visibility = Visibility.Collapsed;
                        break;
                }
                btnStation1.Content = MultiLanguage.GetText(Stations.GetStation(1).description);
                btnStation2.Content = MultiLanguage.GetText(Stations.GetStation(2).description);
                btnStation3.Content = MultiLanguage.GetText(Stations.GetStation(3).description);
                btnStation4.Content = MultiLanguage.GetText(Stations.GetStation(4).description);
                btnStation5.Content = MultiLanguage.GetText(Stations.GetStation(5).description);
                btnStation6.Content = MultiLanguage.GetText(Stations.GetStation(6).description);
                btnStation7.Content = MultiLanguage.GetText(Stations.GetStation(7).description);
                btnStation8.Content = MultiLanguage.GetText(Stations.GetStation(8).description);
                btnStation9.Content = MultiLanguage.GetText(Stations.GetStation(9).description);
                btnStation10.Content = MultiLanguage.GetText(Stations.GetStation(10).description);
                btnStation11.Content = MultiLanguage.GetText(Stations.GetStation(11).description);
                btnStation12.Content = MultiLanguage.GetText(Stations.GetStation(12).description);
                btnStation13.Content = MultiLanguage.GetText(Stations.GetStation(13).description);
                btnStation14.Content = MultiLanguage.GetText(Stations.GetStation(14).description);
                btnStation15.Content = MultiLanguage.GetText(Stations.GetStation(15).description);
                btnStation16.Content = MultiLanguage.GetText(Stations.GetStation(16).description);
                btnStation17.Content = MultiLanguage.GetText(Stations.GetStation(17).description);
                btnStation18.Content = MultiLanguage.GetText(Stations.GetStation(18).description);
                btnStation19.Content = MultiLanguage.GetText(Stations.GetStation(19).description);
                btnStation20.Content = MultiLanguage.GetText(Stations.GetStation(20).description);
                btnStation21.Content = MultiLanguage.GetText(Stations.GetStation(21).description);
                btnStation22.Content = MultiLanguage.GetText(Stations.GetStation(22).description);

            }
            catch (Exception ex)
            {
                log.Error("Error StationMapPage -> Page_Loaded() : " + ex.ToString());
            }
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
        private void SetHeaderText()
        {
            switch (Ticket.mapHeaderType)
            {
                case MapHeaderType.EndStation:
                    lblHeader.Content = MultiLanguage.GetText("selectDest");
                    break;
                case MapHeaderType.Station1:
                    lblHeader.Content = "Select First Station";
                    break;
                case MapHeaderType.Station2:
                    lblHeader.Content = "Select Second Station";
                    break;
                default:
                    break;
            }
        }
        private void SetDefaultStatus()
        {
            lblWarning.Visibility = Visibility.Hidden;
            lblWarning.Content = "";
        }
        private void SetStation(int selectedStationId)
        {
            switch (Ticket.mapHeaderType)
            {
                case MapHeaderType.EndStation:
                    SetEndStation(selectedStationId);
                    break;
                case MapHeaderType.Station1:
                    SetFirstStation(selectedStationId);
                    break;
                case MapHeaderType.Station2:
                    SetSecondStation(selectedStationId);
                    break;
                default:
                    break;
            }
        }
        private void SetSecondStation(int selectedStationId)
        {
            if (selectedStationId != Ticket.startStation.id)
            {
                Ticket.endStation = Stations.GetStation(selectedStationId);
                NavigationService.Navigate(new Pages.CardOperationPage());
            }
        }

        private void SetFirstStation(int selectedStationId)
        {
            Ticket.mapHeaderType = MapHeaderType.Station2;
            Ticket.startStation = Stations.GetStation(selectedStationId);
            SetHeaderText();
        }

        private void SetEndStation(int selectedStationId)
        {
            if (selectedStationId != Stations.currentStation.id)
            {
                Ticket.endStation = Stations.GetStation(selectedStationId);
                Ticket.startStation = Stations.currentStation;
                ElectronicJournal.DestinationSelected(Ticket.endStation.name.ToString());
                Constants.IsMapPageActive = true;
                NavigationService.Navigate(new Pages.TicketCountPage());
                //PageControl.ShowPage(Pages.journeyPage);
            }
            else
            {
                lblWarning.Visibility = Visibility.Visible;
                lblWarning.Content = "This station can not be selected!";
            }
        }
        private void ListStationsInGrid()
        {
            try
            {
                Grid.SetRow(imgHere, Stations.currentStation.mapHereRow);
                Grid.SetColumn(imgHere, Stations.currentStation.mapHereColumn);
                operationGrid.Children.Add(imgHere);
            }
            catch (Exception ex)
            {
                log.Error("Error StationMapPage -> ListStationsInGrid() : " + ex.ToString());
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (idleTimer != null)
                idleTimer.Dispose();
        }
    }
}
