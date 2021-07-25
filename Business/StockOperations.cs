using Kochi_TVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Business
{
    public enum UpdateType
    {
        Increase = 1,
        Decrease = 2,
        Empty = 3
    }
    public enum StockType
    {
        Unknown = 1,
        Rpt = 2,
        Coin1 = 3,
        Coin2 = 4,
        Coin5 = 5,
        Banknote5 = 6,
        Banknote10 = 7,
        Banknote20 = 8,
        Banknote50 = 9,
        Banknote100 = 10,
        Banknote200 = 11,
        Banknote500 = 12,
        Banknote2000 = 13,
        QRSlip = 14,
        ReceiptSlip = 15,
    }
    public enum DeviceType
    {
        Dispenser = 1,
        Hopper1 = 2,
        Hopper2 = 3,
        Hopper5 = 4,
        Cassette1 = 5,
        Cassette2 = 6,
        Cassette3 = 7,
        Box = 8,
        QRPrinter = 9,
        ReceiptPrinter = 10,
    }
    public static class StockOperations
    {
        public static int rpt;
        public static int coin1;
        public static int coin2;
        public static int coin5;
        public static int escrow;
        public static int banknote20;
        public static int banknote10;
        public static int box;
        public static int qrSlip;
        public static int receiptSlip;
        public static bool SelStockStatus()
        {
            bool result = false;
            try
            {
                using (var context = new TVM_Entities())
                {
                   var data = context.sp_SelStockStatus().FirstOrDefault();

                    rpt = (int)(data.dispenser);
                    coin1 = (int)(data.hopper1);
                    coin2 = (int)(data.hopper2);
                    coin5 = (int)(data.hopper5);
                    escrow = (int)(data.cassette3);
                    banknote20 = (int)(data.cassette2);
                    banknote10 = (int)(data.cassette1);
                    box = (int)(data.box);
                    qrSlip = (int)(data.qrSlip);
                    receiptSlip = (int)(data.receiptSlip);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                ////Logger.Log.log.Write("Error: " + ex.Message);
            }
            return result;
        }

        public static bool InsStock(long trxId, int stockType, int deviceType, int updType, int quantity)
        {
            bool result = false;
            try
            {
                using (var context = new TVM_Entities())
                {
                    int val = context.sp_InsStock(trxId, stockType, deviceType, updType, quantity);
                    if(val == 2)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }


    }
}
