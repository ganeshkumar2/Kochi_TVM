using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.Utils;
using Kochi_TVM.VerifoneKochi1;
using log4net;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for CardOperationPage.xaml
    /// </summary>
    public partial class CardOperationPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(CardOperationPage).Name);
        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        Grid PassCountGrid = null;
        public CardOperationPage()
        {
            InitializeComponent();
            scrollCountGrid.ScrollToVerticalOffset(0);
            PreparePageContent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void gridShowBalance_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.BalanceCheckStarted();
            var sr = new EmvService();

            ServiceDataMultiple serviceDataMultiple = sr.GetServiceData(Parameters.TVMConst.UxPosPort);

            if (serviceDataMultiple.RespResult.Result == "Success")
            {
                ServiceData serviceData = new ServiceData();

                #region Find Required Service Data

                if (Constants.ApplicationVersion == ApplicationVersion.Rail)
                {
                    serviceData = serviceDataMultiple.ServiceDatas[0];
                }

                else if (Constants.ApplicationVersion == ApplicationVersion.Water)
                {
                    serviceData = serviceDataMultiple.ServiceDatas[1];
                }

                NavigationService.Navigate(new Pages.Card.LastTransactionPage(serviceData.PanToken, Convert.ToDateTime(serviceData.LastTapInDt)));
                #endregion
            }
        }        

        private void griLoadBalance_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.TopupStarted();
            Ticket.journeyType = JourneyType.Topup;
            PreparePageContent();
        }
        private void PreparePageContent()
        {
            SetContentVisibilityForDefault();
            SetHeaderText(Ticket.journeyType);
            SetContentVisibilityForAction(Ticket.journeyType);
            EditTicketGridForAction(Ticket.journeyType);
        }
        private void SetContentVisibilityForDefault()
        {
            scrollCountGrid.Content = null;
            numberGrid.Visibility = System.Windows.Visibility.Hidden;
            btnSelectedTrip.Visibility = System.Windows.Visibility.Hidden;
            btnSelectedPeriod.Visibility = System.Windows.Visibility.Hidden;
            btnSelectedLoad.Visibility = System.Windows.Visibility.Hidden;
            scrollCountGrid.Visibility = System.Windows.Visibility.Hidden;
            operationGrid.Visibility = System.Windows.Visibility.Hidden;
        }
        private void SetHeaderText(JourneyType jt)
        {
            switch (jt)
            {
                case JourneyType.Unknown:
                    lblHeader.Content = "Select Card Operation";
                    break;
                case JourneyType.Trip_Pass:
                    lblHeader.Content = "Select Pass Count";
                    break;
                case JourneyType.Period_Pass:
                    lblHeader.Content = "Select Pass Period";
                    break;
                case JourneyType.Topup:
                    lblHeader.Content = "Select Topup Amount";
                    break;
                default:
                    break;
            }
        }
        private void SetContentVisibilityForAction(JourneyType jt)
        {
            switch (jt)
            {
                case JourneyType.Trip_Pass:
                    numberGrid.Visibility = System.Windows.Visibility.Visible;
                    btnSelectedTrip.Visibility = System.Windows.Visibility.Visible;
                    scrollCountGrid.Visibility = System.Windows.Visibility.Visible;
                    operationGrid.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case JourneyType.Period_Pass:
                    numberGrid.Visibility = System.Windows.Visibility.Visible;
                    btnSelectedPeriod.Visibility = System.Windows.Visibility.Visible;
                    scrollCountGrid.Visibility = System.Windows.Visibility.Visible;
                    operationGrid.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case JourneyType.Topup:
                    numberGrid.Visibility = System.Windows.Visibility.Visible;
                    btnSelectedLoad.Visibility = System.Windows.Visibility.Visible;
                    scrollCountGrid.Visibility = System.Windows.Visibility.Visible;
                    operationGrid.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case JourneyType.Unknown:
                    operationGrid.Visibility = System.Windows.Visibility.Visible;
                    numberGrid.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
        private void EditTicketGridForAction(JourneyType jt)
        {
            switch (jt)
            {
                case JourneyType.Trip_Pass:
                    Ticket.PassTypeList((int)PassType.Trip);
                    CreatePassCountGrid();
                    ListPassCountInGrid("Pass");
                    break;
                case JourneyType.Period_Pass:
                    Ticket.PassTypeList((int)PassType.Period);
                    CreatePassCountGrid();
                    ListPassCountInGrid("Days");
                    break;
                case JourneyType.Topup:
                    CreateTopupCountGrid();
                    ListTopupCountInGrid();
                    break;
                    //case JourneyType.SJT:
                    //case JourneyType.RJT:
                    //case JourneyType.Day_Pass:
                    //case JourneyType.Weekend_Pass:
                    //    isOk = CreateTicketCountGrid();
                    //    ListTicketCountInGrid();
                    //    break;
            }
        }
        private void ListPassCountInGrid(String passText)
        {
            var a = 0;
            var b = 0;
            var style = Application.Current.FindResource("styleTicketCountBtn") as Style;
            for (var i = 0; i < Ticket.listPassTypes.Count; i++)
            {
                var button = new Button();
                button.Content = passText+" "+Ticket.listPassTypes[i].TripCount.ToString();
                button.Name = "Btn_" + i;
                button.Tag = Ticket.listPassTypes[i].Id;
                button.Style = style;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;

                button.Click += button_Click;
                Grid.SetRow(button, b);
                Grid.SetColumn(button, a);
                PassCountGrid.Children.Add(button);
                a = a + 2;
                if (a != 4) continue;
                b = b + 1;
                a = 0;
            }
        }
        private void ListTopupCountInGrid()
        {
            var a = 0;
            var b = 0;
            var style = Application.Current.FindResource("styleTicketCountBtn") as Style;
            for (var i = 1; i <= Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_TopupLimit")); i++)
            {              
                var button = new Button();
                button.Content = Constants.CurrencySymbol+(i * 100).ToString();
                button.Name = "Btn_" + i;
                button.Tag = (i * 100).ToString();
                button.Style = style;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.Click += button_Click;
                Grid.SetRow(button, b);
                Grid.SetColumn(button, a);
                PassCountGrid.Children.Add(button);
                a = a + 2;
                if (a != 4) continue;
                b = b + 1;
                a = 0;
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            switch (Ticket.journeyType)
            {
                case JourneyType.Period_Pass:
                case JourneyType.Trip_Pass:
                    Ticket.passSubTypeId = (int)(((Button)(sender)).Tag);
                    Ticket.passCount = Ticket.listPassTypes.Where(x => x.Id == Ticket.passSubTypeId).Select(x => x.TripCount).FirstOrDefault();
                    Ticket.ticketExpiryDts = Ticket.listPassTypes.Where(x => x.Id == Ticket.passSubTypeId).Select(x => x.Validity).FirstOrDefault();
                    break;
                case JourneyType.Topup:
                    TransactionInfo.amount = Convert.ToDecimal(((Button)(sender)).Tag);
                    break;
                default:
                    break;
            }
            NavigationService.Navigate(new Pages.OrderPreviewPage());
        }
        private void CreatePassCountGrid()
        {
            try
            {
                PassCountGrid = new Grid();
                var toCount = Ticket.listPassTypes.Count;
                var valueOfRow = (double)toCount / 2;
                var rowCount = Math.Ceiling(valueOfRow);
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                ColumnDefinition gridCol = null;
                RowDefinition gridRow = null;
                for (var i = 0; i < 2; i++)
                {
                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("10*");
                    PassCountGrid.ColumnDefinitions.Add(gridCol);

                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                    PassCountGrid.ColumnDefinitions.Add(gridCol);

                }
                for (var i = 0; i < rowCount; i++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = (GridLength)gridLengthConverter.ConvertFrom("3*");
                    PassCountGrid.RowDefinitions.Add(gridRow);
                }
                PassCountGrid.VerticalAlignment = VerticalAlignment.Center;

                scrollCountGrid.Content = PassCountGrid;
            }
            catch (Exception ex)
            {
            }

        }
        private void CreateTopupCountGrid()
        {
            try
            {
                PassCountGrid = new Grid();
                var toCount = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("sys_TopupLimit"));
                var valueOfRow = (double)toCount / 2;
                var rowCount = Math.Ceiling(valueOfRow);
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                ColumnDefinition gridCol = null;
                RowDefinition gridRow = null;
                for (var i = 0; i < 2; i++)
                {
                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("10*");
                    PassCountGrid.ColumnDefinitions.Add(gridCol);

                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                    PassCountGrid.ColumnDefinitions.Add(gridCol);

                }
                for (var i = 0; i < rowCount; i++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = (GridLength)gridLengthConverter.ConvertFrom("3*");
                    PassCountGrid.RowDefinitions.Add(gridRow);
                }
                PassCountGrid.VerticalAlignment = VerticalAlignment.Center;

                scrollCountGrid.Content = PassCountGrid;
            }
            catch (Exception ex)
            {
            }

        }
        private void griTripPass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Ticket.journeyType = JourneyType.Trip_Pass;
            Ticket.passCategory = (int)PassCategory.EmvTrip;
            GoToNextPage();
        }

        private void griPeriodPass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Ticket.journeyType = JourneyType.Period_Pass;
            Ticket.passCategory = (int)PassCategory.EmvPeriod;
            GoToNextPage();
        }
        private void GoToNextPage()
        {
            Ticket.mapHeaderType = MapHeaderType.Station1;
            Ticket.ManageDTS();
            Ticket.ManageTicketType();

            if ((Ticket.journeyType == JourneyType.Period_Pass) || (Ticket.journeyType == JourneyType.Trip_Pass))
            {
                NavigationService.Navigate(new Pages.StationPage());
            }
            else if (Ticket.journeyType == JourneyType.Topup)
            {
                NavigationService.Navigate(new Pages.Card.CardStatusPage());
            }
            else
            {
                NavigationService.Navigate(new Pages.MainPage());
            }

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (idleTimer != null)
                idleTimer.Dispose();
        }
    }
}
