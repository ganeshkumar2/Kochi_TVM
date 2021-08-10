using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kochi_TVM.Pages.Custom
{
    /// <summary>
    /// Interaction logic for TransactionRow.xaml
    /// </summary>
    public partial class TransactionRow : UserControl
    {
        public TransactionRow(string date, string time, string station, string operation, string amount, string cardBalance, bool firstRow)
        {
            Thickness thic;
            InitializeComponent();
            txnDate.Content = date;
            txnTime.Content = time;
            txnStation.Content = station;
            txnOperation.Content = operation;
            txnAmount.Content = amount;
            txnBalance.Content = cardBalance;

            if (amount.StartsWith("-"))
                txnAmount.Foreground = Brushes.Red;

            if (firstRow)
                thic = new Thickness(5, 5, 5, 5);
            else
                thic = new Thickness(5, 0, 5, 5);

            brdTxn.BorderThickness = thic;
        }
    }
}
