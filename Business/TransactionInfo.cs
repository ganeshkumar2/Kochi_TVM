using Kochi_TVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Business
{
    public static class TransactionInfo
    {
        //language
        public static Languages lang = Languages.English;

        //card info
        public static string cardID;
        public static CardTypes cardType;
        public static decimal amount;
        public static decimal currentBalance;
        public static decimal newBalance;
        public static decimal e_Balance;
        public static decimal totalBalance;
        public static bool pinRequired;
        public static string panToken;
        public static string serviceData;
        public static string RRN;
        public static long recId;
        public static DateTime trxDT;

        //serial message info
        public static MessageTypes messageType;
        public static MessageStates messageState;
        public static OperationResponse oprResponse;
        public static CommandStatus commandStatus;
        public static ErrorCodes errorCodes;
        public static int numPinPressed;




        public static bool pinWrong;
        public static int pinEtryCount;

        public static void Clear()
        {
            cardID = String.Empty;
            panToken = String.Empty;
            serviceData = String.Empty;
            RRN = String.Empty;
            cardType = CardTypes.Unknown;
            currentBalance = 0;
            newBalance = 0;
            messageType = MessageTypes.Unknown;
            messageState = MessageStates.Unknown;
            recId = 0;
            trxDT = DateTime.Now;
            oprResponse = OperationResponse.Unknown;
            commandStatus = CommandStatus.Unknown;
            errorCodes = ErrorCodes.Unknown;
            numPinPressed = 0;


            pinWrong = false;
            pinEtryCount = 0;
        }

        public static decimal? SelTrxId(long trxTypeId)
        {

            decimal? trxId = 0;
            try
            {
                using (var context = new TVM_Entities())
                {
                    trxId = context.sp_SelTrxId(trxTypeId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }

            return trxId;
        }
    }
}
