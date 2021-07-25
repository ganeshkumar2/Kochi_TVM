using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Pages.Custom
{
    public static class NumberPadOperations
    {
        public static int GetNumber()
        {
            NumberPad numberPad = new NumberPad();
            numberPad.ShowDialog();
            return (numberPad.DialogResult == true ? numberPad.number : 0);
        }
    }
}
