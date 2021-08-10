using Kochi_TVM.Business;
using Kochi_TVM.Models;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for AdminLoginPage.xaml
    /// </summary>
    public partial class AdminLoginPage : Page
    {
        public AdminLoginPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {            
            LedOperations.Maintance();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Constants.IsMaintenanceActive = false;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            bool notAuthorized = false;
            bool log = Login(txtUserID.Text, pbUserPassword.Password, Stations.currentStation.id, ref notAuthorized);
            if (notAuthorized)
            {
                Parameters.userId = txtUserID.Text;

                Parameters.menuItems.Clear();

                using (var context = new TVM_Entities())
                {
                    int? userRecId = context.sp_SelUserId(Parameters.userId).FirstOrDefault();

                    var menu = context.sp_userAuth(userRecId, "TVM").ToList();

                    foreach (var row2 in menu)
                    {
                        Parameters.menuItems.Add(row2.explanation.ToString());
                    }
                }

                Constants.BNRTest = false;
                Constants.HopperTest = false;
                Constants.CoinAddOperations = new List<CoinOperation>();
                Constants.CoinDispOperations = new List<CoinOperation>();
                Constants.CoinEmptyOperations = new List<CoinOperation>();
                Constants.QR_RPT_OperationsAdd = new List<QR_RPT_Operation>();
                Constants.QR_RPT_OperationsDisp = new List<QR_RPT_Operation>();
                Constants.QR_RPT_OperationsEmpty = new List<QR_RPT_Operation>();
                Constants.SendToBoxes = new List<SendToBox>();
                Constants.Removecashbox = new List<SendToBox>();
                Constants.AddNotes = new List<AddNote>();

                TVMUtility.RestartExplorer();
                NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
            }
            else
            {
                if (notAuthorized)
                {
                    MessageBoxOperations.ShowMessage("Login Failed", "User is not authorized..", MessageBoxButtonSet.OK);
                }
                else
                {
                    MessageBoxOperations.ShowMessage("Login Failed", "User Id or password is wrong..", MessageBoxButtonSet.OK);
                }
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();            
            NavigationService.Navigate(new Pages.MainPage());
        }
        public bool Login(string userName, string password, int stationId, ref bool notAuthorized)
        {
            bool result = false;
            string hashedPassword = "";
            hashedPassword = USHDLL.Crypto.Ush(password);            
            try
            {
                using (var context = new TVM_Entities())
                {
                    var trxDetail = context.sp_TomAppLogon(userName, "TVM", hashedPassword, "127.0.0.1", stationId).FirstOrDefault();
                    if(trxDetail.alias != null)
                    {
                        notAuthorized = true;                                               
                        result = true;
                    }
                    else
                    {
                        notAuthorized = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
