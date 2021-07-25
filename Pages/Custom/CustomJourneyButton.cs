using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kochi_TVM.Pages.Custom
{
    public class CustomJourneyButton : Button
    {
        public static readonly DependencyProperty prop =
            DependencyProperty.RegisterAttached("Desc", typeof(string), typeof(CustomJourneyButton), new PropertyMetadata(""));

        public string Desc
        {
            get { return (string)GetValue(prop); }
            set { SetValue(prop, value); }
        }
    }
}
