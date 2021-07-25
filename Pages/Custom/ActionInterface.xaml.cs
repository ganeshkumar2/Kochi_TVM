using System.Windows;

namespace Kochi_TVM.Pages.Custom
{
    /// <summary>
    /// Interaction logic for ActionInterface.xaml
    /// </summary>
    public partial class ActionInterface : Window
    {
        public ActionInterface()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
