using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Kochi_TVM.Pages.Custom
{
    public enum MessageBoxButtonSet
    {
        OK = 1,
        Cancel = 2,
        OKCancel = 3,
        Yes = 4,
        No = 5,
        YesNo = 6
    }

    public enum MessageBoxResult
    {
        Non = 0,
        OK = 1,
        Cancel = 2
    }

    public static class MessageBoxOperations
    {
        public static bool isBusy = false;
        public static MessageBoxResult ShowMessage(string caption, string message, MessageBoxButtonSet buttonSet)
        {
            if (!isBusy)
            {
                isBusy = true;
                ActionInterface messageBox = new ActionInterface();

                #region message box options
                messageBox.lblHeader.Content = caption;
                messageBox.lblInfo.Content = message;
                switch (buttonSet)
                {
                    case MessageBoxButtonSet.OK:
                        {
                            messageBox.btnCancel.Visibility = Visibility.Hidden;
                            //messageBox.btnCancel.Content = "Cancel";
                            messageBox.btnOK.Visibility = Visibility.Visible;
                            messageBox.btnOK.Content = "OK";
                            messageBox.imgIndicator.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\information.png"));
                        }
                        break;
                    case MessageBoxButtonSet.Cancel:
                        {
                            messageBox.btnCancel.Visibility = Visibility.Visible;
                            messageBox.btnCancel.Content = "Cancel";
                            messageBox.btnOK.Visibility = Visibility.Hidden;
                            //messageBox.btnOK.Content = "OK";
                            messageBox.imgIndicator.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\crossmark.png"));
                        }
                        break;
                    case MessageBoxButtonSet.OKCancel:
                        {
                            messageBox.btnCancel.Visibility = Visibility.Visible;
                            messageBox.btnCancel.Content = "Cancel";
                            messageBox.btnOK.Visibility = Visibility.Visible;
                            messageBox.btnOK.Content = "OK";
                            messageBox.imgIndicator.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\exclamationmark.png"));
                        }
                        break;
                    case MessageBoxButtonSet.Yes:
                        {
                            messageBox.btnCancel.Visibility = Visibility.Hidden;
                            //messageBox.btnCancel.Content = "Cancel";
                            messageBox.btnOK.Visibility = Visibility.Visible;
                            messageBox.btnOK.Content = "Yes";
                            messageBox.imgIndicator.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\exclamationmark.png"));
                        }
                        break;
                    case MessageBoxButtonSet.No:
                        {
                            messageBox.btnCancel.Visibility = Visibility.Visible;
                            messageBox.btnCancel.Content = "No";
                            messageBox.btnOK.Visibility = Visibility.Hidden;
                            messageBox.imgIndicator.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\crossmark.png"));
                            //messageBox.btnOK.Content = "OK";
                        }
                        break;
                    case MessageBoxButtonSet.YesNo:
                        {
                            messageBox.btnCancel.Visibility = Visibility.Visible;
                            messageBox.btnCancel.Content = "No";
                            messageBox.btnOK.Visibility = Visibility.Visible;
                            messageBox.btnOK.Content = "Yes";
                            messageBox.imgIndicator.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\information.png"));
                        }
                        break;
                }
                #endregion

                messageBox.ShowDialog();

                isBusy = false;
                return (MessageBoxResult)(messageBox.DialogResult == true ? 1 : 2);
            }
            else
                return MessageBoxResult.Non;
        }
    }
}
