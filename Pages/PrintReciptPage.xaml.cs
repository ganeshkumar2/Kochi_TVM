using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.RptDispenser;
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
    /// Interaction logic for PrintReciptPage.xaml
    /// </summary>
    public partial class PrintReciptPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(PrintReciptPage).Name);

        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;

        string RecAmt;
        string ChaAmt;
        public PrintReciptPage(string NumberOfTicket, string ReceivedAmt, string ChangeAmt)
        {
            InitializeComponent();
            try
            {
                LedOperations.GreenText("PAYMENT SUCCESSFUL");
                RecAmt = ReceivedAmt;
                ChaAmt = ChangeAmt;
                returnCashImageGif.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\giving_money.gif");
                lblTicketCount.Content = NumberOfTicket;
                lblChange.Content = ChangeAmt;
                if (!Constants.NoReceiptMode)
                {
                    stkPrint.Visibility = Visibility.Visible;
                    btnFinish.Visibility = Visibility.Collapsed;
                    Message();
                }
                else
                {
                    stkPrint.Visibility = Visibility.Collapsed;
                    btnFinish.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                log.Debug("Error PrintReciptPage -> PrintReciptPage() : " + ex.ToString());
            }
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(9, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(9, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(9, null, null, "IN");
            }
        }
        async void PrintReceipt()
        {
            try
            {
                //foreach (var selectedTickets in Ticket.listTickets)
                //{
                //    var qr = Utility.PrepareQRImage(selectedTickets.TicketGUID);
                //    CustomTL60Printer.Instance.PrintQRTicket(selectedTickets, qr);
                //}
                if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
                {
                    ElectronicJournal.ReceiptPrintStarted();
                    CustomTL60Printer.Instance.TicketReceipt(RecAmt, ChaAmt);
                    ElectronicJournal.ReceiptPrintOver();
                }
                await Task.Delay(1000);
                switch (Ticket.journeyType)
                {
                    case JourneyType.Group_Ticket:
                    case JourneyType.SJT:
                    case JourneyType.RJT:
                        PrintQR();
                        break;
                    case JourneyType.Day_Pass:
                    case JourneyType.Weekend_Pass:
                        PrintQR();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error PrintReciptPage -> PrintReceipt() : " + ex.ToString());
            }
            //NavigationService.Navigate(new Pages.MainPage());
        }
        async void PrintQR()
        {
            try
            {
                LedOperations.GreenText("PLEASE COLLECT TICKET");
                LastMessage();
                await Task.Delay(100);
                NavigationService.Navigate(new Pages.MainPage());
            }
            catch(Exception ex)
            {
                log.Error("Error PrintReciptPage -> PrintQR() : " + ex.ToString());
            }
        }

        async void LastMessage()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(10, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(10, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(10, null, null, "IN");
            }

            if(Constants.IsVoiceEnabled)
                await Task.Delay(5000);
        }
        private void returnCashImageGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            returnCashImageGif.Position = new TimeSpan(0, 0, 1);
            returnCashImageGif.Play();
        }

        private void btnPrintReciptSkip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                idleTimer.Dispose();
                btnFinish.IsEnabled = false;
                btnPrintReciptSkip.IsEnabled = false;
                switch (Ticket.journeyType)
                {
                    case JourneyType.Group_Ticket:
                    case JourneyType.SJT:
                    case JourneyType.RJT:
                        PrintQR();
                        break;
                    case JourneyType.Day_Pass:
                    case JourneyType.Weekend_Pass:
                        PrintQR();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error PrintReciptPage -> btnPrintReciptSkip_Click() : " + ex.ToString());
            }
        }

        private void btnPrintRecipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                idleTimer.Dispose();
                btnPrintRecipt.IsEnabled = false;
                PrintReceipt();
            }
            catch (Exception ex)
            {
                log.Error("Error PrintReciptPage -> btnPrintRecipt_Click() : " + ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Ticket.language == Languages.English)
                {
                    lblChaAmt.FontSize = 24;
                    lblSaleSucc.FontSize = 24;
                    lblCollect.FontSize = 24;
                    lblNoOfTick.FontSize = 24;
                    btnPrintRecipt.FontSize = 24;
                    btnPrintReciptSkip.FontSize = 24;
                    btnFinish.FontSize = 24;
                }
                else
                {
                    lblChaAmt.FontSize = 14;
                    lblSaleSucc.FontSize = 14;
                    lblCollect.FontSize = 14;
                    lblNoOfTick.FontSize = 14;
                    btnPrintRecipt.FontSize = 14;
                    btnPrintReciptSkip.FontSize = 14;
                    btnFinish.FontSize = 14;
                }
                lblTicketCount.FontSize = 24;
                lblChange.FontSize = 24;
                lblChaAmt.Content = MultiLanguage.GetText("ChangeAmt");
                lblSaleSucc.Content = MultiLanguage.GetText("ticketSaleSucces");
                lblCollect.Content = MultiLanguage.GetText("collectTicketChange");
                lblNoOfTick.Content = MultiLanguage.GetText("numberOfTickets");
                btnPrintRecipt.Content = MultiLanguage.GetText("printReceipt");
                btnPrintReciptSkip.Content = MultiLanguage.GetText("skipReceipt");
                btnFinish.Content = MultiLanguage.GetText("Ok");
                initialTimer();
            }
            catch (Exception ex)
            {
                log.Error("PrintReciptPage -> Page_Loaded() : " + ex.ToString());
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            returnCashImageGif.Stop();
            returnCashImageGif.Close();
            returnCashImageGif.Source = null;
            ElectronicJournal.OrderFinalised();
        }
        private void initialTimer()
        {
            try
            {
                idleTimerDelegate = new TimerCallback(NavigateAction);
                idleTimer = new Timer(idleTimerDelegate, null, 60000, 0);
            }
            catch (Exception ex)
            {
                log.Error("Error PrintReciptPage -> initialTimer() : " + ex.ToString());
            }
        }
        private void NavigateAction(object obj)
        {
            try
            {
                idleTimer.Dispose();
                this.Dispatcher.Invoke(() =>
                {
                    switch (Ticket.journeyType)
                    {
                        case JourneyType.Group_Ticket:
                        case JourneyType.SJT:
                        case JourneyType.RJT:
                            PrintQR();
                            break;
                        case JourneyType.Day_Pass:
                        case JourneyType.Weekend_Pass:
                            PrintQR();
                            break;
                        default:
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("Error PrintReciptPage -> DateTimeTimerAction() : " + ex.ToString());
            }
        }
    }
}
