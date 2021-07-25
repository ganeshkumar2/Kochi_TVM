using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kochi_TVM.Pages.Custom
{
    class UserControlNumberPadOperations
    {
        static Window numberContainer;
        static NumberPadUserControl numberPad;

        public static int GetNumber()
        {
            numberContainer = new Window();
            numberPad = new NumberPadUserControl();
            numberContainer.Height = 500;
            numberContainer.Width = 300;
            numberContainer.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            numberContainer.ResizeMode = ResizeMode.NoResize;
            numberContainer.WindowStyle = WindowStyle.None;
            numberContainer.Content = numberPad;
            numberContainer.ShowDialog();

            return numberPad.number;
        }

    }
}
