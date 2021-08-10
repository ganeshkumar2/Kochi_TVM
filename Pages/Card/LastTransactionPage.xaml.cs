using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Kochi_TVM.Pages.Card
{
    /// <summary>
    /// Interaction logic for LastTransactionPage.xaml
    /// </summary>
    public partial class LastTransactionPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(LastTransactionPage).Name);
        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        string cardNoId;
        DateTime trxDate;
        public LastTransactionPage(string cardId, DateTime trxDT)
        {
            InitializeComponent();
            cardNoId = cardId;
            trxDate = trxDT;
        }

        decimal cardBalance;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            initialTimer();
            TransactionRow txnHeader;
            TransactionRow txn;

            //test data
            cardBalance = 600;

            try
            {
                TransactionInfo.cardID = cardNoId;
                TransactionInfo.trxDT = Convert.ToDateTime(trxDate);
                var rpt = Parameters.sr.SelLastTransactions(TransactionInfo.cardID, Convert.ToDateTime(TransactionInfo.trxDT));

                txnHeader = new TransactionRow("Date", "Time", "Station", "Transaction", "Amount", "Card Balance", true);
                Grid.SetRow(txnHeader, 0);
                operationGrid.Children.Add(txnHeader);

                if (rpt.Data.Tables[0].Rows.Count > 0)
                {

                    for (var i = 0; i < rpt.Data.Tables[0].Rows.Count; i++)
                    {

                        string lTrxDate = Convert.ToDateTime(rpt.Data.Tables[0].Rows[i]["trxDt"]).ToString("dd.MM.yyyy");
                        string lTrxTime = Convert.ToDateTime(rpt.Data.Tables[0].Rows[i]["trxDt"]).ToString("HH:mm");
                        String lStation = rpt.Data.Tables[0].Rows[i]["station"].ToString();
                        string lTrxType = rpt.Data.Tables[0].Rows[i]["trxType"].ToString();
                        String lTrxAmount = Constants.CurrencySymbol + Convert.ToDecimal(rpt.Data.Tables[0].Rows[i]["trxAmount"]).ToString("0.#");

                        if (lTrxType == "Topup")
                            cardBalance -= Convert.ToDecimal(rpt.Data.Tables[0].Rows[i]["trxAmount"]);
                        else
                            cardBalance += Convert.ToDecimal(rpt.Data.Tables[0].Rows[i]["trxAmount"]);

                        string cardBalanceTxt = Constants.CurrencySymbol + cardBalance.ToString("0.#");
                        txn = new TransactionRow(lTrxDate, lTrxTime, lStation, lTrxType, lTrxAmount, cardBalanceTxt, false);
                        Grid.SetRow(txn, i + 1);
                        operationGrid.Children.Add(txn);
                    }
                }
            }
            catch (Exception ex)
            {

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
