using CCNET;
using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for BNRSettingPage.xaml
    /// </summary>
    public partial class BNRSettingPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(BNRSettingPage).Name);
        List<BillTable> billTable = new List<BillTable>();
        List<Cassette> cassettes = new List<Cassette>();
        int noteincasset1 = 0, noteincasset2 = 0, noteincasset3 = 0;
        int notevalincasset1 = 0, notevalincasset2 = 0, notevalincasset3 = 0;
        public BNRSettingPage()
        {
            InitializeComponent();
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
            cbCassette.Items.Add("Cassette 1");
            cbCassette.Items.Add("Cassette 2");
            cbCassette.Items.Add("Cassette 3");
            BNRManager.Instance.PollingAction();
            BNRManager.Instance.GetBillTableProcess();
            BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            BNRManager.BNRBillTableInputEvent += new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.BNRCassetteStatusInputEvent += new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
            BNRManager.Instance.GetCassetteStatus();
            this.Unloaded += Page_Unloaded;
            cbEscrowBillType.SelectedValue = Constants.EscrowAmount;
            lblmessage.Content = "BNR Status : " + Constants.BNRStatus;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            BNRManager.BNRBillTableInputEvent -= new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.BNRCassetteStatusInputEvent -= new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
        }

        private void BNRManager_BNRBillTableInputEvent(List<BillTable> billTables)
        {
            billTable = billTables;
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { cbBillType.Items.Clear(); cbEscrowBillType.Items.Clear(); }));
            foreach (BillTable b in billTables)
            {
                if (b.CountryCode != null && b.DigitBillType != 0)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { cbBillType.Items.Add(b.DigitBillType); cbEscrowBillType.Items.Add(b.DigitBillType); }));
                }
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
                            if (cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset1 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset1 = bill;
                            }
                            lblCassete1Type.Content = cassetteset.cassetteStatus.ToString();
                            lblCassete1Bill.Content = notevalincasset1.ToString();
                            lblCassete1Count.Content = noteincasset1.ToString();
                        }
                        if (cassetteset.cassetteId == 2)
                        {
                            noteincasset2 = cassetteset.billNumber;
                            if (cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset2 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset2 = bill;
                            }
                            lblCassete2Type.Content = cassetteset.cassetteStatus.ToString();
                            lblCassete2Bill.Content = notevalincasset2.ToString();
                            lblCassete2Count.Content = noteincasset2.ToString();
                        }
                        if (cassetteset.cassetteId == 3)
                        {
                            noteincasset3 = cassetteset.billNumber;
                            if (cassetteset.cassetteStatus == CassetteStatus.ESCROW)
                            {
                                notevalincasset3 = Constants.EscrowAmount;
                            }
                            else
                            {
                                notevalincasset3 = bill;
                            }
                            lblCassete3Type.Content = cassetteset.cassetteStatus.ToString();
                            lblCassete3Bill.Content = notevalincasset3.ToString();
                            lblCassete3Count.Content = noteincasset3.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error BNRSettingPage -> BNRManager_BNRCassetteStatusInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
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

        private void cbCassette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rbNotAssign.IsChecked = false;
            rbEscrow.IsChecked = false;
            rbCassetAssign.IsChecked = false;
            cbBillType.SelectedIndex = -1;
            cbEscrowBillType.SelectedIndex = -1;
            if (cbCassette.SelectedIndex == 0)
            {
                if (cassettes[0].cassetteStatus == CassetteStatus.NORMAL)
                {
                    rbCassetAssign.IsChecked = true;
                    cbBillType.SelectedValue = notevalincasset1;
                }
                else
                {
                    rbEscrow.IsChecked = true;
                    cbEscrowBillType.SelectedValue = Constants.EscrowAmount;
                }
            }
            if (cbCassette.SelectedIndex == 1)
            {
                if (cassettes[1].cassetteStatus == CassetteStatus.NORMAL)
                {
                    rbCassetAssign.IsChecked = true;
                    cbBillType.SelectedValue = notevalincasset2;
                }
                else
                {
                    rbEscrow.IsChecked = true;
                    cbEscrowBillType.SelectedValue = Constants.EscrowAmount;
                }
            }
            if ((cbCassette.SelectedIndex == 2))
            {
                if (cassettes[2].cassetteStatus == CassetteStatus.NORMAL)
                {
                    rbCassetAssign.IsChecked = true;
                    cbBillType.SelectedValue = notevalincasset3;
                }
                else
                {
                    rbEscrow.IsChecked = true;
                    cbEscrowBillType.SelectedValue = Constants.EscrowAmount;
                }
            }
        }

        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                lblmessage.Content = "BNR Status : " + Constants.BNRStatus;
                if (state == BNRState.DISABLED)
                {
                    Thread.Sleep(200);
                    BNRManager.Instance.GetCassetteStatus();
                }
            }), DispatcherPriority.Background);
        }

        private void GridSubmitSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();

            if (cassettes[0].billNumber > 0)
            {
                MessageBoxOperations.ShowMessage("BNR", "Please unload the Cassette 1.", MessageBoxButtonSet.OK);
                return;
            }

            if (cassettes[1].billNumber > 0)
            {
                MessageBoxOperations.ShowMessage("BNR", "Please unload the Cassette 2.", MessageBoxButtonSet.OK);
                return;
            }

            if (cassettes[2].billNumber > 0)
            {
                MessageBoxOperations.ShowMessage("BNR", "Please unload the Cassette 3.", MessageBoxButtonSet.OK);
                return;
            }

            if ((cbCassette.SelectedIndex == -1))
            {
                MessageBoxOperations.ShowMessage("BNR", "Select Cassette No.", MessageBoxButtonSet.OK);
                return;
            }

            if (rbCassetAssign.IsChecked == true)
            {
                if ((cbCassette.SelectedIndex == -1) && (cbBillType.SelectedIndex == -1))
                {
                    MessageBoxOperations.ShowMessage("BNR", "Cassette No and Bill Type are empty.", MessageBoxButtonSet.OK);
                    return;
                }

                if ((cbBillType.SelectedIndex == -1))
                {
                    MessageBoxOperations.ShowMessage("BNR", "Select Bill Type.", MessageBoxButtonSet.OK);
                    return;
                }
            }
            else if (rbEscrow.IsChecked == true)
            {
                if ((cbEscrowBillType.SelectedIndex == -1))
                {
                    MessageBoxOperations.ShowMessage("BNR", "Select Escrow Bill Type.", MessageBoxButtonSet.OK);
                    return;
                }
            }
            else if (rbNotAssign.IsChecked == true)
            {
                MessageBoxOperations.ShowMessage("BNR", "Not assign option selected.", MessageBoxButtonSet.OK);
            }
            else
            {
                MessageBoxOperations.ShowMessage("BNR", "Select Not Assign/Escrow/Casset Assigned option.", MessageBoxButtonSet.OK);
                return;
            }

            if (Constants.BNRStatus == "DISABLED")
            {
                int cassetteNo = cbCassette.SelectedIndex + 1;
                int billType = cbBillType.SelectedIndex;
                Constants.EscrowAmount = Convert.ToInt16(cbEscrowBillType.SelectedValue);
                SetCassetteType(cassetteNo, billType);
            }
        }

        private void SetCassetteType(int cassetteNumber, int billType)
        {
            try
            {
                byte[] snd_arr = new byte[2];
                snd_arr[0] = (byte)(cassetteNumber);
                if (rbNotAssign.IsChecked == true)
                {
                    snd_arr[1] = 0x1F;
                }
                else if (rbEscrow.IsChecked == true)
                {
                    snd_arr[1] = 0x18;
                }
                else if (rbCassetAssign.IsChecked == true)
                {
                    snd_arr[1] = (byte)(billType);
                }
                new Thread(() => safeICCNET.Instance.RunCommand(CCNETCommand.SET_RECYCLING_CASSETTE_TYPE, snd_arr)).Start();
            }
            catch (Exception ex)
            {
                log.Error("Error BNRSettingPage -> SetCassetteType() : " + ex.ToString());
            }
        }
        private void GridGetStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BNRManager.Instance.GetCassetteStatus();
        }
    }
}
