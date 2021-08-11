using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
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
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for TicketCountPage.xaml
    /// </summary>
    public partial class TicketCountPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(TicketCountPage).Name);
        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        Grid TicketCountGrid = null;
        Grid PassengerCountGrid = null;
        public TicketCountPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            initialTimer();
            if (Ticket.language == Languages.English || Ticket.language == Languages.Hint)
            {
                lblType.FontSize = 12;
                lblDisType.FontSize = 12;
                lblDestination.FontSize = 12;
                lblDisDestination.FontSize = 12;
                lblNoOfTickets.FontSize = 12;
                lblAmount.FontSize = 12;
            }
            else
            {
                lblType.FontSize = 10;
                lblDisType.FontSize = 10;
                lblDestination.FontSize = 10;
                lblDisDestination.FontSize = 10;
                lblNoOfTickets.FontSize = 10;
                lblAmount.FontSize = 10;
            }
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
            }
            lblDisType.Content = MultiLanguage.GetText("DispTicketType");
            lblNoOfTickets.Content = MultiLanguage.GetText("DispNoOfTickets");
            lblAmount.Content = MultiLanguage.GetText("DispAmount");
            lblDestination.Content = MultiLanguage.GetText(Ticket.endStation.name);
            lblDisDestination.Content = MultiLanguage.GetText("DispDestination");
            lblHeader.Content = MultiLanguage.GetText("selectTicketCount");
            btnBack.Content = MultiLanguage.GetText("back");
            btnFinish.Content = MultiLanguage.GetText("cancel");
            labelSJT.Content = MultiLanguage.GetText("sj");
            labelRJT.Content = MultiLanguage.GetText("rj");
            labelGT.Content = MultiLanguage.GetText("gj");
            lblOneDayPass.Content = MultiLanguage.GetText("onedaypass");
            lblWeekendPass.Content = MultiLanguage.GetText("weekenddaypass");
            EditTicketGridForAction(Ticket.journeyType);
            Message();
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
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(5, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(5, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(5, null, null, "IN");
            }
        }
        private void EditTicketGridForAction(JourneyType jt)
        {
            bool isOk = false;
            switch (jt)
            {
                case JourneyType.Unknown:
                    ListJourneyBtnsInGrid();
                    break;
                case JourneyType.Group_Ticket:
                    gridSJT.Visibility = System.Windows.Visibility.Hidden;
                    gridRJT.Visibility = System.Windows.Visibility.Hidden;
                    gridGrp.Visibility = System.Windows.Visibility.Visible;
                    gridWeekendPass.Visibility = System.Windows.Visibility.Hidden;
                    gridDayPass.Visibility = System.Windows.Visibility.Hidden;
                    lblDestination.Content = MultiLanguage.GetText(Ticket.endStation.name);
                    isOk = CreatePassengerCountGrid();
                    if (isOk)
                        ListPassengerCountInGrid();
                    break;
                case JourneyType.SJT:
                    gridSJT.Visibility = System.Windows.Visibility.Visible;
                    gridRJT.Visibility = System.Windows.Visibility.Hidden;
                    gridGrp.Visibility = System.Windows.Visibility.Hidden;
                    gridWeekendPass.Visibility = System.Windows.Visibility.Hidden;
                    gridDayPass.Visibility = System.Windows.Visibility.Hidden;
                    lblDestination.Content = MultiLanguage.GetText(Ticket.endStation.name);
                    isOk = CreateTicketCountGrid();
                    ListTicketCountInGrid();
                    break;
                case JourneyType.RJT:
                    gridSJT.Visibility = System.Windows.Visibility.Hidden;
                    gridRJT.Visibility = System.Windows.Visibility.Visible;
                    gridGrp.Visibility = System.Windows.Visibility.Hidden;
                    gridWeekendPass.Visibility = System.Windows.Visibility.Hidden;
                    gridDayPass.Visibility = System.Windows.Visibility.Hidden;
                    lblDestination.Content = MultiLanguage.GetText(Ticket.endStation.name);
                    isOk = CreateTicketCountGrid();
                    ListTicketCountInGrid();
                    break;
                case JourneyType.Day_Pass:
                    gridSJT.Visibility = System.Windows.Visibility.Hidden;
                    gridRJT.Visibility = System.Windows.Visibility.Hidden;
                    gridGrp.Visibility = System.Windows.Visibility.Hidden;
                    gridWeekendPass.Visibility = System.Windows.Visibility.Hidden;
                    gridDayPass.Visibility = System.Windows.Visibility.Visible;
                    lblDisDestination.Visibility = Visibility.Collapsed;
                    lblDestination.Visibility = Visibility.Collapsed;
                    arrorDest.Visibility = Visibility.Collapsed;
                    isOk = CreateTicketCountGrid();
                    ListTicketCountInGrid();
                    break;
                case JourneyType.Weekend_Pass:
                    gridSJT.Visibility = System.Windows.Visibility.Hidden;
                    gridRJT.Visibility = System.Windows.Visibility.Hidden;
                    gridGrp.Visibility = System.Windows.Visibility.Hidden;
                    gridDayPass.Visibility = System.Windows.Visibility.Hidden;
                    gridWeekendPass.Visibility = System.Windows.Visibility.Visible;
                    lblDisDestination.Visibility = Visibility.Collapsed;
                    lblDestination.Visibility = Visibility.Collapsed;
                    arrorDest.Visibility = Visibility.Collapsed;
                    isOk = CreateTicketCountGrid();
                    ListTicketCountInGrid();
                    break;
            }
        }
        private void ListJourneyBtnsInGrid()
        {
            //if (Parameters.TVMDynamic.GetParameter("AfcConn") == "1")
            //{
                //Grid.SetRow(btnSjt, 0);
                //Grid.SetColumn(btnSjt, 0);

                //Grid.SetRow(btnRjt, 0);
                //Grid.SetColumn(btnRjt, 2);

            //Grid.SetRow(btnDayPass, 0);
            //Grid.SetColumn(btnDayPass, 4);

            //Grid.SetRow(btnWeekPass, 2);
            //Grid.SetColumn(btnWeekPass, 0);
            gridSJT.Visibility = System.Windows.Visibility.Visible;
            gridRJT.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    //Grid.SetRow(btnDayPass, 0);
            //    //Grid.SetColumn(btnDayPass, 0);

            //    //Grid.SetRow(btnWeekPass, 0);
            //    //Grid.SetColumn(btnWeekPass, 2);

            //    //btnDayPass.Visibility = System.Windows.Visibility.Visible;
            //    //btnWeekPass.Visibility = System.Windows.Visibility.Visible;
            //}
        }
        private bool CreatePassengerCountGrid()
        {
            bool result = false;
            try
            {
                PassengerCountGrid = new Grid();
                var minGroupCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MinGroupCount"));
                var maxGroupCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MaxGroupCount"));
                var btnCount = maxGroupCount - minGroupCount + 1;
                var valueOfRow = (double)btnCount / 5;
                var rowCount = Math.Ceiling(valueOfRow);
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                ColumnDefinition gridCol = null;
                RowDefinition gridRow = null;
                for (var i = 0; i < 5; i++)
                {
                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("5*");
                    PassengerCountGrid.ColumnDefinitions.Add(gridCol);

                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                    PassengerCountGrid.ColumnDefinitions.Add(gridCol);

                }
                for (var i = 0; i < rowCount; i++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = (GridLength)gridLengthConverter.ConvertFrom("*");
                    PassengerCountGrid.RowDefinitions.Add(gridRow);
                }
                result = true;
                scrollCountGrid.Content = PassengerCountGrid;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        private void ListPassengerCountInGrid()
        {
            var a = 0;
            var b = 0;
            var style = Application.Current.FindResource("stylePassengerCountBtn") as Style;
            int minGroupCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MinGroupCount"));
            var maxGroupCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MaxGroupCount"));
            for (var i = minGroupCount; i <= maxGroupCount; i++)
            {
                var buttonTo = new Button
                {
                    Content = i,
                    Name = "btn" + i,
                    Style = style,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                buttonTo.Click += btnNumber_Click;
                Grid.SetRow(buttonTo, b);
                Grid.SetColumn(buttonTo, a);
                PassengerCountGrid.Children.Add(buttonTo);
                a = a + 2;
                if (a != 10) continue;
                b = b + 1;
                a = 0;
            }
        }
        private bool CreateTicketCountGrid()
        {
            bool result = false;
            try
            {
                TicketCountGrid = new Grid();
                var toCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MaxTicket"));
                var valueOfRow = (double)toCount / 2;
                var rowCount = Math.Ceiling(valueOfRow);
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                ColumnDefinition gridCol = null;
                RowDefinition gridRow = null;
                for (var i = 0; i < 2; i++)
                {
                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("10*");
                    TicketCountGrid.ColumnDefinitions.Add(gridCol);

                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                    TicketCountGrid.ColumnDefinitions.Add(gridCol);

                }
                for (var i = 0; i < rowCount; i++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = (GridLength)gridLengthConverter.ConvertFrom("3*");
                    TicketCountGrid.RowDefinitions.Add(gridRow);
                }

                result = true;
                scrollCountGrid.Content = TicketCountGrid;
            }
            catch (Exception ex)
            {

                result = false;
            }
            return result;
        }
        private void ListTicketCountInGrid()
        {
            var a = 0;
            var b = 0;
            var style = Application.Current.FindResource("styleTicketCountBtn") as Style;
            var ticketCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MaxTicket"));
            for (var i = 1; i <= ticketCount; i++)
            {
                var buttonTo = new Button
                {
                    Content = i,
                    Name = "btn" + i,
                    Style = style,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                buttonTo.Click += btnNumber_Click;
                Grid.SetRow(buttonTo, b);
                Grid.SetColumn(buttonTo, a);
                TicketCountGrid.Children.Add(buttonTo);
                a = a + 2;
                if (a != 4) continue;
                b = b + 1;
                a = 0;
            }
        }
        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            Button btn = (Button)sender;
            int count = Int32.Parse(btn.Content.ToString());

            StockOperations.SelStockStatus();

            Ticket.peopleCount = 1;
            Ticket.ticketCount = 1;

            if (Ticket.journeyType == JourneyType.Group_Ticket)
            {
                int minGroupCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MinGroupCount"));
                int maxGroupCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MaxGroupCount"));
                if (count >= minGroupCount && count <= maxGroupCount)
                {
                    if (count <= StockOperations.qrSlip)
                    {
                        Ticket.peopleCount = count;
                        ElectronicJournal.NumberOfTicket(count.ToString());
                        NavigationService.Navigate(new Pages.OrderPreviewPage());
                        //    PageControl.ShowPage(Pages.paymentTypePage);
                    }
                    else if (StockOperations.qrSlip == 0)
                    {
                        //MessageBoxOperations.ShowMessage("Paper Count..", "Not enough paper, You can buy " + StockOperations.qrSlip.ToString()+ " tickets.", MessageBoxButtonSet.OK);
                        MessageBoxOperations.ShowMessage(MultiLanguage.GetText("paperCount"), MultiLanguage.GetText("noQrSlip"), MessageBoxButtonSet.OK);
                    }
                    else
                    {
                        MessageBoxOperations.ShowMessage(MultiLanguage.GetText("paperCount"), MultiLanguage.GetText("notEnoughPaper") + ", " + MultiLanguage.GetText("youCanBuy") + " " + StockOperations.qrSlip.ToString() + MultiLanguage.GetText("tickets"), MessageBoxButtonSet.OK);
                    }
                }

            }
            else
            {
                int maxTicket = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_MaxTicket"));
                if (count >= 1 && count <= maxTicket)
                    if (count <= StockOperations.qrSlip)
                    {
                        Ticket.ticketCount = count;
                        ElectronicJournal.NumberOfTicket(count.ToString());
                        NavigationService.Navigate(new Pages.OrderPreviewPage());
                        //PageControl.ShowPage(Pages.paymentTypePage);
                    }
                    else if (StockOperations.qrSlip == 0)
                    {
                        //MessageBoxOperations.ShowMessage("Paper Count..", "Not enough paper, You can buy " + StockOperations.qrSlip.ToString()+ " tickets.", MessageBoxButtonSet.OK);
                        MessageBoxOperations.ShowMessage(MultiLanguage.GetText("paperCount"), MultiLanguage.GetText("noQrSlip"), MessageBoxButtonSet.OK);
                    }
                    else
                    {
                        MessageBoxOperations.ShowMessage(MultiLanguage.GetText("paperCount"), MultiLanguage.GetText("notEnoughPaper") + ", " + MultiLanguage.GetText("youCanBuy") + " " + StockOperations.qrSlip.ToString() + MultiLanguage.GetText("tickets"), MessageBoxButtonSet.OK);
                    }
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if(idleTimer != null)
                idleTimer.Dispose();
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            switch (Ticket.journeyType)
            {
                case JourneyType.Group_Ticket:
                case JourneyType.SJT:
                case JourneyType.RJT:
                    if (!Constants.IsMapPageActive)
                        NavigationService.Navigate(new Pages.StationPage());
                    else
                        NavigationService.Navigate(new Pages.StationMapPage());
                    break;
                case JourneyType.Day_Pass:
                case JourneyType.Weekend_Pass:
                        NavigationService.Navigate(new Pages.JourneyTypePage());
                    break;
                default:
                    break;
            }           
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            NavigationService.Navigate(new Pages.MainPage());
        }
    }
}
