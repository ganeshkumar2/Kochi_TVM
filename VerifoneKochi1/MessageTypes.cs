using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.VerifoneKochi1
{
    public enum MessageTypes
    {
        CheckCard = 0x00,
        CheckContactless = 0x03,
        TopUp = 0x10,
        OnlineBalanceInquery = 0x11,
        OfflineBalanceInquery = 0x12,
        Sync = 0x14,
        PresentCard = 0x15,
        InstallApp = 0x0C,
        ServiceDataRead = 0x0D,
        ServiceDataUpdate = 0x0F,
        PINRequired = 0x16, //only receive
        PINTypeResult = 0x17, //only receive
        ChangePosSetting = 0x18,
        EOD = 0x1A,
        PINBtnPressed = 0x1B,
        Unknown = 0xFF
    }
    public enum ErrorCodes
    {
        WrongParameterType = 0x01,
        MessageLengthError = 0x02,
        CRCFailed = 0x03,
        CheckCardCustomProcessEntryFailed = 0x04,
        VivotechAntennaEnableFailed = 0x05,
        VivotechOpenPassThroughModeFailed = 0x06,
        VivotechPollForTokenFailed = 0x07,
        VivotechReadBlocksFailed = 0x08,
        VivotechWriteBlocksFailed = 0x09,
        VivotechAntennaDisableFailed = 0x0A,
        VivotechAccessFailed = 0x0B,
        SAMUnlockFailed = 0x0C,
        SAMGeneratePasswordFailed = 0x0D,
        SAMDecryptionFailed = 0x0E,
        SAMVerifyPersoMACFailed = 0x0F,
        SAMVerifyTransactionMACFailed = 0x10,
        SAMResetFailed = 0x11,
        VerifyRPTFailed = 0x12,
        SAMInitFailed = 0x13,
        UX400CheckFailed = 0x14,
        UX100CheckFailed = 0x15,
        StepCustomProcessCardEntryFailed = 0xA0,
        StepCustomContinueCardTransactionFailed = 0xA1,
        StepHostConnectAsyncFailed = 0xA2,
        StepHostCommunicationFailed = 0xA3,
        StepCompleteCardTransactionFailed = 0xA4,
        StepSaveOfflineTxnFailed = 0xA5,
        StepCompleteFinancialTransactionFailed = 0xA6,
        StepSettlementRequiredFailed = 0xA7,
        StepConfigUpdateRequiredFailed = 0xA8,
        StepDisplayBalanceFailed = 0xA9,
        StepRupayServiceAvailablityCheckFailed = 0xAA,
        StepPlantServiceKeyFailed = 0xAB,
        Unknown = 0xFF
    }
    public enum CommandStatus
    {
        OK = 0x00,
        NOK = 0x88,
        Unknown = 0xFF
    }
    public enum OperationResponse
    {
        PinExpected = 1,
        Contact = 2,
        Contactless = 3,
        PinEntered = 4,
        Unknown = 5,
        PinBtnPressed = 6,
        CardNotPresent = 7
    }
    public enum CardTypes
    {
        Contact = 4,
        Contactless = 8,
        RPT = 2,
        NoCard = 8,
        Unknown = 9
    }
    public class PosDataLayout
    {
        public decimal topupAmount = 100;
        public decimal newBalance = 0.0m;
        public decimal e_Balance = 0.0m;
        public decimal totalBalance = 0.0m;
        public string panToken = String.Empty;
        public string pan = String.Empty;
        public string serviceData = String.Empty;
        public string RRN = String.Empty;
        public int numPinPressed = 0;
        public OperationResponse oprResponse = OperationResponse.Unknown;
        public CardTypes cardType = CardTypes.Unknown;
        public CommandStatus commandStatus = CommandStatus.Unknown;
        public MessageTypes messageType = MessageTypes.Unknown;
        public ErrorCodes errorCodes = ErrorCodes.Unknown;
    }
    public class MessageLayout
    {
        public byte[] msgData = null;
        public MessageTypes messageType = MessageTypes.Unknown;
        public CommandStatus commandStatus = CommandStatus.Unknown;
        public bool crcControl = false;
        public bool formatControl = false;
    }
}
