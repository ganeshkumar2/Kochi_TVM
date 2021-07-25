using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
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
    /// Interaction logic for PIDSettingPage.xaml
    /// </summary>
    public partial class PIDSettingPage : Page
    {
        public PIDSettingPage()
        {
            InitializeComponent();
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
            txtMessage.Text = PIDMessageLog.getMessage();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtMessage.Text != "" && txtMessage.Text != null)
            {
                PIDMessageLog.setMessage(txtMessage.Text);
                MessageBoxOperations.ShowMessage("PID", "Default Message Saved.", MessageBoxButtonSet.OK);
            }
            else
            {
                MessageBoxOperations.ShowMessage("PID", "Please Enter Default Message.", MessageBoxButtonSet.OK);
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
    }
}
