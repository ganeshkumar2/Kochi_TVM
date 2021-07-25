using Kochi_TVM.BNR;
using Kochi_TVM.CCTalk;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.RptDispenser;
using Kochi_TVM.Sensors;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for OutOfSevicePage.xaml
    /// </summary>
    public partial class OutOfSevicePage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(OutOfSevicePage).Name);
        public OutOfSevicePage()
        {
            InitializeComponent();            
            new Thread(() => AsyncIntFunc()).Start();
        }

        private void AsyncIntFunc()
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(async () =>
                {
                    LedOperations.Close();
                    await Task.Delay(1000);
                    InitializeBNR();

                }));
            }
            catch (Exception ex)
            {
                log.Error("Error OutOfServicePage -> asyncFunc() :" + ex.ToString());
            }
        }

        private void InitializeBNR()
        {

            try
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    outOfServiceLbl.Content = "Initializing BNR";
                    BNRManager.Instance.PollingAction();
                }));

            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    outOfServiceLbl.Content = ex.Message;
                }));
            }

        }


        private void CoinHopper1()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    if (CCTalkManager.Instance.coinHopperEV4000_1.Manufacture != null)
                    {
                        CCTalkManager.Instance.coinHopperEV4000_1.EnableHopper();
                        outOfServiceLbl.Content = "Coin Hopper One :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "OK";
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = "Coin Hopper One :";
                    outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                    outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                    outOfServiceRedLbl.Content = "Not OK";
                    log.Error("Error OutOfSevicePage -> CoinHopper1() :" + ex.ToString());
                }
            }));
        }

        private void CoinHopper2()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    if (CCTalkManager.Instance.coinHopperEV4000_2.Manufacture != null)
                    {
                        CCTalkManager.Instance.coinHopperEV4000_2.EnableHopper();
                        outOfServiceLbl.Content = "Coin Hopper Two :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "OK";
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = "Coin Hopper Two :";
                    outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                    outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                    outOfServiceRedLbl.Content = "Not OK";
                    log.Error("Error OutOfSevicePage -> CoinHopper2() :" + ex.ToString());
                }
            }));
        }

        private void CoinHopper3()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    if (CCTalkManager.Instance.coinHopperEV4000_3.Manufacture != null)
                    {
                        CCTalkManager.Instance.coinHopperEV4000_3.EnableHopper();
                        outOfServiceLbl.Content = "Coin Hopper Three :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "OK";
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = "Coin Hopper Three :";
                    outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                    outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                    outOfServiceRedLbl.Content = "Not OK";
                    log.Error("Error OutOfSevicePage -> CoinHopper2() :" + ex.ToString());
                }
            }));
        }

        private void Dispencer()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    DISP_STAT stat = DISP_STAT.STACKER_FULL;
                    RPTOperations.GetStatus(ref stat);
                    byte rptstatus = 1;

                    if (stat == DISP_STAT.STACKER_UNKNOWN)
                    {
                        rptstatus = 0;
                    }
                    else if ((stat == DISP_STAT.STACKER_NOCARD) && (!RPTOperations.IsCardInRFCardOperationPosition()))
                    {
                        rptstatus = 0;
                    }

                    if (rptstatus == 1)
                    {
                        outOfServiceLbl.Content = "Dispenser Status :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "OK";
                    }
                    else if (rptstatus == 0)
                    {
                        outOfServiceLbl.Content = "Dispenser Status :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "Not OK";
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = ex.Message;
                }
            }));
            
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
        }

        private void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
        }

        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                    outOfServiceLbl.Content = "BNR Status :";
                    outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                    outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                    outOfServiceGreenLbl.Content = Constants.BNRStatus;
                    if (state == BNRState.DISABLED)
                    {
                        new Thread(() => AsyncIntHopperFunc()).Start();
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = "BNR Status :";
                    outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                    outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                    outOfServiceRedLbl.Content = "Not OK";
                    log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRStateInputEvent :" + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void AsyncIntHopperFunc()
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(async () =>
                {
                    BNRManager.Instance.GetCassetteStatus();

                    await Task.Delay(1000);                                       

                    CoinHopper1();

                    await Task.Delay(1000);

                    CoinHopper2();

                    await Task.Delay(1000);

                    CoinHopper3();
                    await Task.Delay(1000);

                    Dispencer();
                    await Task.Delay(1000);

                    UpdDevStat();
                    await Task.Delay(8000);

                    PRINTER_STATE QRStatus = QRPrinter.Instance.CheckQrPrinterStatus();//CustomKPM150HPrinter.Instance.getStatusWithUsb();
                    if (QRStatus == PRINTER_STATE.OK)
                    {
                        outOfServiceLbl.Content = "QRPrinter Status :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "OK";
                    }
                    else
                    {
                        outOfServiceLbl.Content = "QRPrinter Status :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceGreenLbl.Content = "Not OK";
                    }
                    await Task.Delay(1000);
                    PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
                    if (ReceiptPrinter == PRINTER_STATE.OK)
                    {
                        outOfServiceLbl.Content = "Receipt Status :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "OK";
                    }
                    else
                    {
                        outOfServiceLbl.Content = "Receipt Status :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceGreenLbl.Content = "Not OK";
                    }
                    await Task.Delay(1000);
                    KMY200DoorAlarm.Instance.SetAlarm();
                    outOfServiceLbl.Content = "Alarm Activated :";
                    outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                    outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                    outOfServiceGreenLbl.Content = "OK";
                    await Task.Delay(2000);
                    NavigationService.Navigate(new Pages.MainPage());
                }));
            }
            catch (Exception ex)
            {
                log.Error("Error OutOfServicePage -> asyncFunc() :" + ex.ToString());
            }
        }

        private void UpdDevStat()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(async () =>
            {
                try
                {
                    int status = KMY200DoorAlarm.Instance.GetStatus();
                    Enums.DoorStatus doorStatus = (Enums.DoorStatus)(status);
                    await Task.Delay(1000);
                    if (doorStatus == Enums.DoorStatus.DOOR_ALL_CLOSE)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);                        
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_1_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                       
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_2_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_3_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right Open";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_4_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right Door:";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_12_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_13_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_14_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_123_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_24_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_23_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_234_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_34_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_134_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_124_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceGreenLbl.Content = "CLOSE";
                        await Task.Delay(1000);
                    }
                    if (doorStatus == Enums.DoorStatus.DOOR_1234_OPEN)
                    {
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Front Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Hopper Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Door :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                        outOfServiceLbl.Content = "Back Left Right :";
                        outOfServiceGreenLbl.Visibility = System.Windows.Visibility.Collapsed;
                        outOfServiceRedLbl.Visibility = System.Windows.Visibility.Visible;
                        outOfServiceRedLbl.Content = "OPEN";
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                }
            }));
        }
    }
}
