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
using System.Windows.Shapes;

namespace Kochi_TVM.Pages.Custom
{
    /// <summary>
    /// Interaction logic for NumberPad.xaml
    /// </summary>
    public partial class NumberPad : Window
    {
        public int number = 0;
        public NumberPad()
        {
            InitializeComponent();
        }
        private void btn1_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(1);
        }

        private void btn2_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(2);

        }

        private void btn3_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(3);
        }

        private void btn4_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(4);
        }

        private void btn5_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(5);
        }

        private void btn6_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(6);
        }

        private void btn7_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(7);
        }

        private void btn8_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(8);
        }

        private void btn9_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(9);
        }

        private void btn0_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Add(0);
        }

        private void btnClr_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Clear();
        }

        private void btnDel_Click(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Delete();
        }

        void Add(int num)
        {
            if (num >= 0 && num <= 9)
            {
                number = (number * 10) + num;
            }

            txtTypeNumber.Text = number.ToString();
        }

        void Delete()
        {
            if (number > 0)
            {
                number = (int)(number / 10);
            }

            txtTypeNumber.Text = number.ToString();
        }

        void Clear()
        {
            number = 0;
            txtTypeNumber.Text = number.ToString();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            this.DialogResult = false;
            this.Close();
        } 
    }
}
