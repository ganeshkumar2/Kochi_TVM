using Kochi_TVM.MultiLanguages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Utils
{
    public static class Conversion
    {
        public static string MoneyFormat(decimal amount)
        {
            return String.Format(MultiLanguage.GetText("rs") + "{0:0.00}", amount);
        }

    }
}
