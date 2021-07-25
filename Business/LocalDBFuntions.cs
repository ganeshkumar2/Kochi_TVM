using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Business
{
    public static class LocalDBFuntions
    {
        public static decimal EPurseDiscountCalculate(decimal price, string ticketType, ApplicationVersion applicationVersion, ref decimal ticketMultiplier, ref decimal emvMultiplier)
        {
            try
            {
                using (var context = new Models.TVM_Entities())
                {
                    var localRp = context.sp_EmvDiscount(1, ticketType, price, (int)applicationVersion).FirstOrDefault();

                    if (localRp.amount != null)
                    {
                        var amount = Convert.ToDecimal(localRp.amount);

                        emvMultiplier = Convert.ToDecimal(localRp.EmvMultiplier);

                        ticketMultiplier = Convert.ToDecimal(localRp.Multiplier);

                        return amount;
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
