using dk.CctalkLib.Devices;
using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Logs;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for CoinSettingPage.xaml
    /// </summary>
    public partial class CoinSettingPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(CoinSettingPage).Name);
        string level = "";
        public CoinSettingPage()
        {
            InitializeComponent();
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            StockOperations.SelStockStatus();
            cbCoinType1.Items.Add("1");
            cbCoinType1.Items.Add("2");
            cbCoinType1.Items.Add("5");
            cbCoinType1.Items.Add("10");
            cbCoinType1.SelectedValue = Constants.HopperAddress1Coin.ToString();

            cbCoinType2.Items.Add("1");
            cbCoinType2.Items.Add("2");
            cbCoinType2.Items.Add("5");
            cbCoinType2.Items.Add("10");
            cbCoinType2.SelectedValue = Constants.HopperAddress2Coin.ToString();

            cbCoinType3.Items.Add("1");
            cbCoinType3.Items.Add("2");
            cbCoinType3.Items.Add("5");
            cbCoinType3.Items.Add("10");
            cbCoinType3.SelectedValue = Constants.HopperAddress3Coin.ToString();
        }

        private void btnAddCoin1_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            if (cbCoinType1.SelectedIndex != -1)
            {
                if (cbCoinType1.SelectedValue.ToString() == Constants.HopperAddress2Coin.ToString() || cbCoinType1.SelectedValue.ToString() == Constants.HopperAddress3Coin.ToString())
                {
                    MessageBoxOperations.ShowMessage("COIN", "Please try with different coin type.", MessageBoxButtonSet.OK);
                    return;
                }

                if (CCTalkManager.Instance.coinHopperEV4000_1.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_1.GetHighLowStatus();

                    if (level  == "Low Level" && StockOperations.coin1 == 0)
                    {
                        if (Constants.CoinAvailable.Contains(cbCoinType1.SelectedValue.ToString()))
                        {
                            CoinValues.setCoin1(cbCoinType1.SelectedValue.ToString());
                            Constants.HopperAddress1Coin = Convert.ToInt16(cbCoinType1.SelectedValue.ToString());
                            MessageBoxOperations.ShowMessage("COIN", "Coin hopper 1 value saved.", MessageBoxButtonSet.OK);
                        }
                        else
                        {
                            MessageBoxOperations.ShowMessage("COIN", "Entered coin value is wrong.", MessageBoxButtonSet.OK);
                        }
                    }
                    else
                    {
                        MessageBoxOperations.ShowMessage("COIN", "Please empty the hopper 1.", MessageBoxButtonSet.OK);
                    }
                }
            }
            else
            {
                MessageBoxOperations.ShowMessage("COIN", "Please enter the coin value.", MessageBoxButtonSet.OK);
            }
        }
        private void btnAddCoin2_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            if (cbCoinType2.SelectedIndex != -1)
            {
                if (cbCoinType2.SelectedValue.ToString() == Constants.HopperAddress1Coin.ToString() || cbCoinType2.SelectedValue.ToString() == Constants.HopperAddress3Coin.ToString())
                {
                    MessageBoxOperations.ShowMessage("COIN", "Please try with different coin type.", MessageBoxButtonSet.OK);
                    return;
                }

                if (CCTalkManager.Instance.coinHopperEV4000_2.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_2.GetHighLowStatus();

                    if (level == "Low Level" && StockOperations.coin2 == 0)
                    {
                        if (Constants.CoinAvailable.Contains(cbCoinType2.SelectedValue.ToString()))
                        {
                            CoinValues.setCoin2(cbCoinType2.SelectedValue.ToString());
                            Constants.HopperAddress2Coin = Convert.ToInt16(cbCoinType2.SelectedValue.ToString());
                            MessageBoxOperations.ShowMessage("COIN", "Coin hopper 2 value saved.", MessageBoxButtonSet.OK);
                        }
                        else
                        {
                            MessageBoxOperations.ShowMessage("COIN", "Entered coin value is wrong.", MessageBoxButtonSet.OK);
                        }
                    }
                    else
                    {
                        MessageBoxOperations.ShowMessage("COIN", "Please empty the hopper 2.", MessageBoxButtonSet.OK);
                    }
                }
            }
            else
            {
                MessageBoxOperations.ShowMessage("COIN", "Please enter the coin value.", MessageBoxButtonSet.OK);
            }
        }

        private void btnAddCoin3_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            if (cbCoinType3.SelectedIndex != -1)
            {
                if (cbCoinType3.SelectedValue.ToString() == Constants.HopperAddress1Coin.ToString() || cbCoinType3.SelectedValue.ToString() == Constants.HopperAddress2Coin.ToString())
                {
                    MessageBoxOperations.ShowMessage("COIN", "Please try with different coin type.", MessageBoxButtonSet.OK);
                    return;
                }

                if (CCTalkManager.Instance.coinHopperEV4000_3.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_3.GetHighLowStatus();

                    if (level == "Low Level" && StockOperations.coin5 == 0)
                    {
                        if (Constants.CoinAvailable.Contains(cbCoinType3.SelectedValue.ToString()))
                        {
                            CoinValues.setCoin3(cbCoinType3.SelectedValue.ToString());
                            Constants.HopperAddress3Coin = Convert.ToInt16(cbCoinType3.SelectedValue.ToString());
                            MessageBoxOperations.ShowMessage("COIN", "Coin hopper 3 value saved.", MessageBoxButtonSet.OK);
                        }
                        else
                        {
                            MessageBoxOperations.ShowMessage("COIN", "Entered coin value is wrong.", MessageBoxButtonSet.OK);
                        }
                    }
                    else
                    {
                        MessageBoxOperations.ShowMessage("COIN", "Please empty the hopper 3.", MessageBoxButtonSet.OK);
                    }
                }
            }
            else
            {
                MessageBoxOperations.ShowMessage("COIN", "Please enter the coin value.", MessageBoxButtonSet.OK);
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminSettingPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
        private void HooperManager_HopperStateInputEvent(byte[] res, CoinHopperCommands sentCommand)
        {
            if (sentCommand == CoinHopperCommands.REQUEST_PAYOUT_HIGH_LOW_STATUS)
            {
                var Data = res;
                var comd = sentCommand;
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    try
                    {
                        var Dataresult = Utils.TVMUtility.Hex2Binary(Data[4].ToString());
                        var highorlow = Dataresult.Substring(Dataresult.Length - 2);
                        var high = highorlow[0];
                        var low = highorlow[1];
                        if (Convert.ToInt16(high.ToString()) == 1)
                        {
                            level = "High Level";
                        }
                        else if (Convert.ToInt16(low.ToString()) == 1)
                        {
                            level = "Low Level";
                        }
                        else if (Convert.ToInt16(low.ToString()) == 0)
                        {
                            level = "Normal Level";
                        }
                    }
                    catch (Exception ex1)
                    {
                        log.Error("Error PayByCashOrCoinPage -> HooperManager_HopperStateInputEvent() : " + ex1.ToString());
                    }
                }));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedInputEvent += new dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedEventHandler(HooperManager_HopperStateInputEvent);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedInputEvent -= new dk.CctalkLib.Devices.CoinHopperEV4000.ResponseReceivedEventHandler(HooperManager_HopperStateInputEvent);
        }
    }
}
