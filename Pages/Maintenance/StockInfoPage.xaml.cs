using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for StockInfoPage.xaml
    /// </summary>
    public partial class StockInfoPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(StockInfoPage).Name);
        List<BillTable> billTable = new List<BillTable>();
        List<Cassette> cassettes = new List<Cassette>();
        int noteincasset1 = 0, noteincasset2 = 0, noteincasset3 = 0;
        int notevalincasset1 = 0, notevalincasset2 = 0, notevalincasset3 = 0;

        public StockInfoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BNRManager.Instance.PollingAction();
            BNRManager.BNRBillTableInputEvent += new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.Instance.GetBillTableProcess();           
            BNRManager.BNRCassetteStatusInputEvent += new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
            BNRManager.Instance.GetCassetteStatus();

            if (StockOperations.SelStockStatus())
                MoneyOperations.SelMoneyStatus();
                    

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
        }
        private void BNRManager_BNRBillTableInputEvent(List<BillTable> billTables)
        {
            try
            {
                billTable = billTables;
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> BNRManager_BNRBillTableInputEvent() : " + ex.ToString());
            }
        }

        private void BNRManager_BNRCassetteStatusInputEvent(List<Cassette> cassette)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (cassette[0].billType == 0 && cassette[1].billType == 0 && cassette[2].billType == 0)
                    {
                        BNRManager.Instance.GetCassetteStatus();
                    }
                    cassettes = cassette;
                    foreach (Cassette cassetteset in cassettes)
                    {
                        int bill = billTable.Where(x => x.BillType == cassetteset.billType).Select(x => x.DigitBillType).FirstOrDefault();
                        string billval = Convert.ToString(bill);
                        if (cassetteset.cassetteId == 1)
                        {
                            noteincasset1 = cassetteset.billNumber;
                            if (cassetteset.cassetteId == 1 && cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset1 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset1 = bill;
                            }
                        }
                        if (cassetteset.cassetteId == 2)
                        {
                            noteincasset2 = cassetteset.billNumber;
                            if (cassetteset.cassetteId == 2 && cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset2 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset2 = bill;
                            }
                        }
                        if (cassetteset.cassetteId == 3)
                        {
                            noteincasset3 = cassetteset.billNumber;
                            if (cassetteset.cassetteId == 3 && cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset3 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset3 = bill;
                            }
                        }
                    }
                    UpdValOnScr();
                }
                catch (Exception ex)
                {
                    log.Error("Error BNROperationPage -> BNRManager_BNRCassetteStatusInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }
        private void UpdValOnScr()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                #region dispenser
                DispenserValue.Content = Convert.ToString(StockOperations.rpt);
                #endregion

                #region hopper
                hoppersTotalValue.Content = String.Format("{0}", Conversion.MoneyFormat((StockOperations.coin1 * Constants.HopperAddress1Coin) + (StockOperations.coin2 * Constants.HopperAddress2Coin) + (StockOperations.coin5 * Constants.HopperAddress3Coin)));
                hopper5Value.Content = hopper2Value.Content = String.Format(StockOperations.coin5.ToString() + "x" + Constants.HopperAddress3Coin + " = {0}", Conversion.MoneyFormat(StockOperations.coin5 * Constants.HopperAddress3Coin));
                hopper2Value.Content = String.Format(StockOperations.coin2.ToString() + "x" + Constants.HopperAddress2Coin + " = {0}", Conversion.MoneyFormat(StockOperations.coin2 * Constants.HopperAddress2Coin));
                hopper1Value.Content = String.Format(StockOperations.coin1.ToString() + "x" + Constants.HopperAddress1Coin + " = {0}", Conversion.MoneyFormat(StockOperations.coin1 * Constants.HopperAddress1Coin));
                #endregion

                #region bna
                bnaTotalValue.Content = String.Format("{0}", Conversion.MoneyFormat((notevalincasset1 * noteincasset1) + (notevalincasset2 * noteincasset2)+ (notevalincasset3 * noteincasset3) + MoneyOperations.box));
                bnaCassette1Value.Content = String.Format(noteincasset1.ToString() + "x" + notevalincasset1 + " = {0}", Conversion.MoneyFormat(notevalincasset1 * noteincasset1));
                bnaCassette2Value.Content = String.Format(noteincasset2.ToString() + "x" + notevalincasset2 + " = {0}", Conversion.MoneyFormat(notevalincasset2 * noteincasset2));
                bnaCassette3Value.Content = String.Format(noteincasset3.ToString() + "x" + notevalincasset3 + " = {0}", Conversion.MoneyFormat(notevalincasset3 * noteincasset3));
                bnaBoxValue.Content = String.Format("{0}", Conversion.MoneyFormat(MoneyOperations.box));
                #endregion

                #region receipts
                QRPaperValue.Content = Convert.ToString(StockOperations.qrSlip);
                //receiptValue.Content = Convert.ToString(StockOperations.receiptSlip);
                #endregion

                #region grandtotal
                GrandTotalAmount.Content = Convert.ToString(Conversion.MoneyFormat((notevalincasset1 * noteincasset1) + (notevalincasset2 * noteincasset2) + (notevalincasset3 * noteincasset3) +
                                                            MoneyOperations.box + (StockOperations.coin1 * Constants.HopperAddress1Coin) + (StockOperations.coin2 * Constants.HopperAddress2Coin) +
                                                            (StockOperations.coin5 * Constants.HopperAddress3Coin)));

                HopperGrandAmount.Content = Convert.ToString(Conversion.MoneyFormat((StockOperations.coin1 * Constants.HopperAddress1Coin) + (StockOperations.coin2 * Constants.HopperAddress2Coin) +
                                                            (StockOperations.coin5 * Constants.HopperAddress3Coin)));

                BanknoteGrandAmount.Content = Convert.ToString(Conversion.MoneyFormat((notevalincasset1 * noteincasset1) + (notevalincasset2 * noteincasset2) + (notevalincasset3 * noteincasset3) +
                                                            MoneyOperations.box));
                #endregion
            }), DispatcherPriority.Background);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            BNRManager.BNRBillTableInputEvent -= new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.BNRCassetteStatusInputEvent -= new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminInfoPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            CustomTL60Printer.Instance.StockStatusReport(StockOperations.coin1, StockOperations.coin2, StockOperations.coin5, StockOperations.qrSlip,
               StockOperations.rpt, StockOperations.receiptSlip, noteincasset1, noteincasset2, noteincasset3, notevalincasset1, notevalincasset2, notevalincasset3,
               Convert.ToInt32(MoneyOperations.box));
        }
    }
}
