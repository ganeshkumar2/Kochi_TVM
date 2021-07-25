using Kochi_TVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Business
{
    public static class MoneyOperations
    {
        public static decimal coin1;
        public static decimal coin2;
        public static decimal coin5;
        public static decimal banknote20;
        public static decimal banknote10;
        public static decimal escrow;
        public static decimal box;
        public static bool SelMoneyStatus()
        {
            bool result = false;
            try
            {
                using (var context = new TVM_Entities())
                {
                    var data = context.sp_SelMoneyStatus().FirstOrDefault();
                    coin1 = (decimal)(data.hopper1);
                    coin2 = (decimal)(data.hopper2);
                    coin5 = (decimal)(data.hopper5);
                    escrow = (decimal)(data.cassette1);
                    banknote20 = (decimal)(data.cassette2);
                    banknote10 = (decimal)(data.cassette3);
                    box = (decimal)(data.box);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write("Error: " + ex.Message);
            }
            return result;
        }
        public static bool InsMoney(long trxId, int stockType, int deviceType, int updType, decimal amount)
        {
            bool result = false;
            try
            {
                using (var context = new TVM_Entities())
                {
                    int data = context.sp_InsMoney(trxId, stockType, deviceType, updType, amount);
                    if(data==2)
                    result = true;
                }
                
                //Logger.Log.log.Write(Logger.LogTypes.Info.ToString() + ": result = " + result);
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write(Logger.LogTypes.Error.ToString() + " : Error : " + ex.ToString());
            }
            return result;
        }

    }
}
