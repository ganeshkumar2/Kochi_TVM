using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
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

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for TVMInfoPage.xaml
    /// </summary>
    public partial class TVMInfoPage : Page
    {
        int? version = 0;
        public TVMInfoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            versionLoad();
            UpdDevStat();           
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            btnFinish.Content = "Cancel";
        }
        async void versionLoad()
        {            
            using (var context = new Models.TVM_Entities())
            {
                version = context.FareTables.Where(x => x.isActive == 1 && x.isDeleted == false && DateTime.Now >= x.validityDateBegin && DateTime.Now <= x.validityDateEnd).OrderByDescending(x => x.version).Select(x => x.version).FirstOrDefault();
            }
            await Task.Delay(200);
        }
        private void UpdDevStat()
        {
            //add real value           
                
            //TVMId
            DeviceInfoControl TVMId = new DeviceInfoControl("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"));
            Grid.SetRow(TVMId, 0);
            Grid.SetColumn(TVMId, 0);
            operationGrid.Children.Add(TVMId);

            //EquipmentId
            DeviceInfoControl EquipmentId = new DeviceInfoControl("Equipment ID", Parameters.TVMDynamic.GetParameter("descCode"));
            Grid.SetRow(EquipmentId, 2);
            Grid.SetColumn(EquipmentId, 0);
            operationGrid.Children.Add(EquipmentId);           

            //AppVersion
            DeviceInfoControl AppVersion = new DeviceInfoControl("App Version", Parameters.TVMStatic.GetParameter("appVersion"));
            Grid.SetRow(AppVersion, 4);
            Grid.SetColumn(AppVersion, 0);
            operationGrid.Children.Add(AppVersion);

            //LastSyncDts
            DeviceInfoControl LastSyncDts = new DeviceInfoControl("Last Sync Date", Parameters.lastSync.ToString("dd/MM/yyyy HH:mm"));
            Grid.SetRow(LastSyncDts, 6);
            Grid.SetColumn(LastSyncDts, 0);
            operationGrid.Children.Add(LastSyncDts);

            ////ParamVersion
            DeviceInfoControl ParamVersion = new DeviceInfoControl("Parameter Version", Parameters.TVMDynamic.GetParameter("sys_CCVersion"));
            Grid.SetRow(ParamVersion, 8);
            Grid.SetColumn(ParamVersion, 0);
            operationGrid.Children.Add(ParamVersion);

            //FareVersion
            DeviceInfoControl FareVersion = new DeviceInfoControl("Fare Version", Convert.ToString(version));
            Grid.SetRow(FareVersion, 0);
            Grid.SetColumn(FareVersion, 2);
            operationGrid.Children.Add(FareVersion);

            //CentralComputer
            DeviceInfoControl CentralComputer = new DeviceInfoControl("Central Computer", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connect" : "Disconnect");
            Grid.SetRow(CentralComputer, 2);
            Grid.SetColumn(CentralComputer, 2);
            operationGrid.Children.Add(CentralComputer);

            //OCC
            DeviceInfoControl OCC = new DeviceInfoControl("OCC", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connect" : "Disconnect");
            Grid.SetRow(OCC, 4);
            Grid.SetColumn(OCC, 2);
            operationGrid.Children.Add(OCC);

            //StationComputer
            DeviceInfoControl StationComputer = new DeviceInfoControl("Station Computer", Parameters.TVMDynamic.GetParameter("SCConn") == "1" ? "Connect" : "Disconnect");
            Grid.SetRow(StationComputer, 6);
            Grid.SetColumn(StationComputer, 2);
            operationGrid.Children.Add(StationComputer);
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            {
                CustomTL60Printer.Instance.TVMInfoReceipt(Convert.ToString(version));
            }
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
    }
}
