using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.MultiLanguages
{
    public interface LocalSettings
    {
        string GetDate();
        string GetTime();
        string GetCurrency();
        string NumberConversion(string numericValue);
    }

    public class DefaultConversion : LocalSettings
    {
        public string GetDate()
        {
            return DateTime.Now.ToString("dd MMMM yyyy");
        }

        public string GetTime()
        {
            return DateTime.Now.ToString("hh:mm tt");
        }

        public string GetCurrency()
        {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }

        public string NumberConversion(string input)
        {
            return input;
        }
    }
}
