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
            lblTypeCoin1.Text = Convert.ToString(Constants.HopperAddress1Coin);
            lblTypeCoin2.Text = Convert.ToString(Constants.HopperAddress2Coin);
            lblTypeCoin3.Text = Convert.ToString(Constants.HopperAddress3Coin);
        }

        bool coin1 = false;
        private void btnAddCoin1_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            if (!coin1)
                return;

            if (lblTypeCoin1.Text != "0")
            {
                if (CCTalkManager.Instance.coinHopperEV4000_1.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_1.GetHighLowStatus();

                    if (level  == "Low Level")
                    {
                        if (Constants.CoinAvailable.Contains(lblTypeCoin1.Text))
                        {
                            CoinValues.setCoin1(lblTypeCoin1.Text);
                            Constants.HopperAddress1Coin = Convert.ToInt16(lblTypeCoin1.Text);
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
                lblTypeCoin1.Focus();
            }
            coin1 = false;
        }
        bool coin2 = false;
        private void btnAddCoin2_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            if (!coin2)
                return;

            if (lblTypeCoin2.Text != "0")
            {
                if (CCTalkManager.Instance.coinHopperEV4000_2.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_2.GetHighLowStatus();

                    if (level == "Low Level")
                    {
                        if (Constants.CoinAvailable.Contains(lblTypeCoin1.Text))
                        {
                            CoinValues.setCoin2(lblTypeCoin2.Text);
                            Constants.HopperAddress2Coin = Convert.ToInt16(lblTypeCoin2.Text);
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
                lblTypeCoin2.Focus();
            }
            coin2 = false;
        }

        bool coin3 = false;
        private void btnAddCoin3_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            if (!coin3)
                return;

            if (lblTypeCoin3.Text != "0")
            {
                if (CCTalkManager.Instance.coinHopperEV4000_3.Category != null)
                {
                    CCTalkManager.Instance.coinHopperEV4000_3.GetHighLowStatus();

                    if (level == "Low Level")
                    {
                        if (Constants.CoinAvailable.Contains(lblTypeCoin1.Text))
                        {
                            CoinValues.setCoin3(lblTypeCoin3.Text);
                            Constants.HopperAddress3Coin = Convert.ToInt16(lblTypeCoin3.Text);
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
                lblTypeCoin3.Focus();
            }
            coin3 = false;
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

        private void lblTypeCoin1_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        int selected = 0;
        private void lblTypeCoin1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            grdUserControl.Visibility = Visibility.Visible;
            selected = 1;
        }

        private void lblTypeCoin2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            grdUserControl.Visibility = Visibility.Visible;
            selected = 2;
        }

        private void lblTypeCoin3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            grdUserControl.Visibility = Visibility.Visible;
            selected = 3;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (selected == 1 && numberpad.txtTypeNumber.Text != "0")
            {
                lblTypeCoin1.Text = numberpad.txtTypeNumber.Text;
                coin1 = true;
            }
            else if (selected == 1 && numberpad.txtTypeNumber.Text == "0")
            {
                lblTypeCoin1.Text = Convert.ToString(Constants.HopperAddress1Coin);
            }

            if (selected == 2 && numberpad.txtTypeNumber.Text != "0")
            {
                lblTypeCoin2.Text = numberpad.txtTypeNumber.Text;
                coin2 = true;
            }
            else if (selected == 2 && numberpad.txtTypeNumber.Text == "0")
            {
                lblTypeCoin2.Text = Convert.ToString(Constants.HopperAddress2Coin);
            }

            if (selected == 3 && numberpad.txtTypeNumber.Text != "0")
            {
                lblTypeCoin3.Text = numberpad.txtTypeNumber.Text;
                coin3 = true;
            }
            else if (selected == 3 && numberpad.txtTypeNumber.Text == "0")
            {
                lblTypeCoin3.Text = Convert.ToString(Constants.HopperAddress3Coin);
            }

            selected = 0;
            numberpad.number = 0;
            numberpad.txtTypeNumber.Text = "0";
            grdUserControl.Visibility = Visibility.Collapsed;
        }
    }
}
