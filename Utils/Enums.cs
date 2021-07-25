using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Utils
{
    public class Enums
    {
        public enum Languages
        {
            English = 0,
            Hint = 1,
            Malayalam = 2
        }
        public enum TicketType
        {
            RPT = 0,
            QR = 1,
            SmartCard
        }
        public enum PassType
        {
            Unknown = 0,
            Period = 1,
            Trip = 2
        }
        public enum JourneyType
        {
            ShowBalance = 0,
            Topup = 1,
            Weekend_Pass = 2,
            Period_Pass = 3,
            Trip_Pass = 4,
            Group_Ticket = 9,
            SJT = 11,
            RJT = 12,
            Day_Pass = 13,
            EventTicket = 14,
            Unknown = 15
        }
        public class EmvPassTicketTypes
        {
            public int Id = 0;
            public string Explanation = "";
            public int TripCount = 0;
            public DateTime Validity = DateTime.Now;
        }
        public enum MapHeaderType
        {
            Unknown = 0,
            EndStation = 1,
            Station1 = 2,
            Station2 = 3
        }
        public enum BtnLang
        {
            // fonksiyon önceki hale göre yapılmış, son hal kararlaştırılsa fonksiyon güncellenecek
            //Önceki hal
            //CurEnBtn1HintBtn2Ml = 0,
            //CurHintBtn1EnBtn2Ml = 1,
            //CurMlBtn1EnBtn2Hint = 2

            //Son hal
            CurEnBtn1MlBtn2Hint = 0,
            CurHintBtn1MlBtn2En = 1,
            CurMlBtn1EnBtn2Hint = 2
        }
        public enum CardTypes
        {
            Contact = 4,
            Contactless = 8,
            RPT = 2,
            NoCard = 8,
            Unknown = 9
        }
        public enum MessageTypes
        {
            CheckCard = 0x00,
            CheckContactless = 0x03,
            ServiceDataRead = 0x0D,
            TopUp = 0x10,
            OnlineBalanceInquery = 0x11,
            OfflineBalanceInquery = 0x12,
            Sync = 0x14,
            PresentCard = 0x15,
            Unknown = 0xFF
        }
        public enum MessageStates
        {
            Prepare = 0,
            Sent = 1,
            WaitingResponse = 2,
            Received = 3,
            Parse = 4,
            Success = 4,
            Fail = 6,
            Timeout = 7,
            Unknown = 0xFF
        }
        public enum TransactionType
        {
            TT_UNKNOWN = 0,
            TT_QR = 1,
            TT_RPT = 2,
            TT_EMV = 3,
            TT_REFUND = 4,
            TT_REPLACEMENT = 5,
            TT_CANCEL = 6,
            TT_ADJUSTMENT = 7,
            TT_PENALTY = 8,
            TT_EMVISSUANCE = 9,
            TT_TRIPTOPUP = 10, //Staff Card
            TT_EMV_REPLACEMENT = 11,// emv pass issuance
            TT_EMV_PASS_ISSUANCE = 12,
            TT_PASS_RENEWAL = 13,
            TT_EMV_PASS_REFUND = 14,
            TT_ADD_RPT = 15,
            TT_REMOVE_PRT = 16,
            TT_EMPTY_RPT = 17,
            TT_ADD_BANKNOTE20 = 18,
            TT_REMOVE_BANKNOTE20 = 19,
            TT_ADD_BANKNOTE10 = 20,
            TT_REMOVE_BANKNOTE10 = 21,
            TT_ADD_BOX = 22,
            TT_EMPTY_BOX = 23,
            TT_ADD_COIN1 = 24,
            TT_REMOVE_COIN1 = 25,
            TT_EMPTY_COIN1 = 26,
            TT_ADD_COIN2 = 27,
            TT_REMOVE_COIN2 = 28,
            TT_EMPTY_COIN2 = 29,
            TT_ADD_COIN5 = 30,
            TT_REMOVE_COIN5 = 31,
            TT_EMPTY_COIN5 = 32,
            TT_PAYOUT_BANKNOTE20 = 33,
            TT_PAYOUT_BANKNOTE10 = 34,
            TT_PAYOUT_COIN5 = 35,
            TT_PAYOUT_COIN2 = 36,
            TT_PAYOUT_COIN1 = 37,
            TT_PAY_BANKNOTE20 = 38,
            TT_PAY_BANKNOTE10 = 39,
            TT_DISPENSE_BANKNOTE20 = 40,
            TT_DISPENSE_BANKNOTE10 = 41,
            TT_STACK_BANKNOTE20 = 42,
            TT_STACK_BANKNOTE10 = 43,
            TT_PAY_BANKNOTE5 = 44,
            TT_PAY_BANKNOTE50 = 45,
            TT_PAY_BANKNOTE100 = 46,
            TT_PAY_BANKNOTE200 = 47,
            TT_PAY_BANKNOTE500 = 48,
            TT_PAY_BANKNOTE2000 = 49,
            TT_DISPENSE_ESCROW = 50,
            TT_STACK_ESCROW = 51,
            TT_FROM_ESCROW_TO_BOX = 52,
            TT_FROM_CASSETTE2_TO_BOX = 53,
            TT_FROM_CASSETTE3_TO_BOX = 54,
            TT_ADD_QR,
            TT_REMOVE_QR,
            TT_EMPTY_QR,
            TT_ADD_BANKNOTE5,
            TT_ADD_BANKNOTE50,
            TT_ADD_BANKNOTE100,
            TT_ADD_BANKNOTE200,
            TT_ADD_BANKNOTE500,
            TT_ADD_BANKNOTE2000,
            TT_REMOVE_BANKNOTE5,
            TT_REMOVE_BANKNOTE50,
            TT_REMOVE_BANKNOTE100,
            TT_REMOVE_BANKNOTE200,
            TT_REMOVE_BANKNOTE500,
            TT_REMOVE_BANKNOTE2000,
            TT_PAYOUT_BANKNOTE5,
            TT_PAYOUT_BANKNOTE50,
            TT_PAYOUT_BANKNOTE100,
            TT_PAYOUT_BANKNOTE200,
            TT_PAYOUT_BANKNOTE500,
            TT_PAYOUT_BANKNOTE2000
        }
        public enum EmvTypeId
        {
            EmvTopupTypeId = 1,
            EmvPassTypeId = 2,
            EmvRenewalTypeId = 3
        }
        public enum Availability
        {
            ISSUE_AVAILABLE = 1,
            REFUND_RENEW_AVAILABLE = 2,
            NO_AVAILABLE = 3
        }
        public enum PassCategory
        {
            EmvPeriod = 1,
            EmvTrip = 2,
            EmvDiscount = 3,
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
        public enum CommandStatus
        {
            OK = 0x00,
            NOK = 0x88,
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
        public enum BNRState
        {
            UNKNOWN,
            POWER_UP = 0X10,
            POWER_UP_BILL_IN_VALIDATOR = 0X11,
            POWER_UP_BILL_IN_CHASSIS = 0X12,
            INITIALIZE = 0X13,
            DISABLED = 0X19,
            ACCEPTING = 0X15,
            STACKING = 0X17,
            RETURNING = 0X18,
            IDLING = 0X14,
            ESCROW = 0X80,
            PACKED_STACKED = 0X81,
            RETURNED = 0X82,
            HOLDING = 0X1A,
            REJECTING = 0X1C,
            DISPENSING = 0X1D,
            DISPENSED = 0X25,
            UNLOADING = 0X1E,
            UNLOADED = 0X26,
            BUSY = 0X1B,
            SETTING_TYPE_CASSETE = 0X21,
            INVALID_BILL_NUMBER = 0X28,
            SET_CASSETTE_TYPE = 0X29,
            INVALID_COMMAND = 0X30,
            DROP_CASSETTE_FULL = 0X41,
            DROP_CASSETTE_REMOVED = 0X42,
            JAM_IN_ACCEPTOR = 0X43,
            JAM_IN_STACKER = 0X44,
            CHEATED = 0X45,
            EXTENDED_CASSETTE_STATUS = 0X70
        }
        public enum CassetteStatus
        {
            NORMAL,
            ESCROW,
            NOT_ASSIGNED
        }
        public struct BillTable
        {
            public int BillType;
            public int DigitBillType;
            public string CountryCode;
        }        
        public struct Cassette
        {
            public int cassetteId;
            public int billType;
            public int billNumber;
            public bool presence;
            public bool cassetteFull;
            public CassetteStatus cassetteStatus;
        }

        public struct StackedNotes
        {
            public int BillType;
            public int BillNumber;
        }
        public enum BNRBillType
        {
            T5 = 0,
            T10 = 1,
            T20 = 2,
            T50 = 3,
            T100 = 4,
            T200 = 5,
            T500 = 6,
            T2000 = 7
        }

        public enum PRINTER_STATE
        {
            OK = 0X0,
            OTHER = 0X1,
            NO_ERROR = 0X2,
            LOW_PAPER = 0X3,
            NO_PAPER = 0X4,
            LOW_TONER = 0X5,
            NO_TONER = 0X6,
            DOOR_OPEN = 0X7,
            JAMMED = 0X8,
            SERVICE_REQUESTED = 0X9,
            OUTPUT_BIN_FULL = 0XA,
            PAPER_PROBLEM = 0XB,
            CANNOT_PRINT_PAGE = 0XC,
            USER_INTERVENTION_REQUIRED = 0XD,
            OUT_OF_MEMORY = 0XE,
            SERVER_UNKNOWN = 0XF,
            ERROR,

            DEFAULT_VALUE = 0xFFFE,// costome error
            OFFLINE = 0xFFFF// costome error   
        }
        public enum Dispenser_ErrorCodes
        {
            //device err : 10, device Code : 04, err Code : 1 - 99 => ex : 100312
            Success = 0,
            ParameterError = 100401,
            GetStatusError = 100402,
            ResetError = 100403,
            CommOpenError = 100404,
            EnableError = 100405,
            DisableError = 100406,
            DispenseError = 100407,
            DeviceBussyError = 100408,
            DeviceEmptyError = 100409,
            JammedError = 100410,
            UnknownError = 100499
        }
        public enum PaymentType
        {
            card = 1,
            cash = 2
        }
        public enum ApplicationVersion
        {
            Rail = 1,
            Water,
            Amritsar,
            Indore
        }
        public enum ElectronicJournalRecordType
        {
            OrderStarted,
            ItemSelected,
            Destinationselected,
            NumberOfTickets,
            AmountPayable,
            MediaSelected,
            MediaPaid,
            QRPrintStarted,
            QRPrintError,
            QRPrintOver,
            ReceiptPrintStarted,
            ReceiptPrintOver,
            OrderFinalised,
            TopupStarted,
            TopupAmount,
            TopupCompleted,
            BalanceCheckStarted,
            BalanceCheckCompleted,
            BalanceSyncStarted,
            BalanceSyncCompleted,
            NoteInserted,
            CoinInserted,
            NoteReturned,
            CoinReturned,
            BalanceNoteReturned,
            BalanceCoinReturned,
            OrderCancelled
        }

        public enum DoorStatus
        {
            DOOR_ALL_CLOSE = 0,
            DOOR_1_OPEN = 2,
            DOOR_2_OPEN = 4,
            DOOR_12_OPEN = 6,
            DOOR_3_OPEN = 16,
            DOOR_13_OPEN = 18,
            DOOR_23_OPEN = 20,
            DOOR_123_OPEN = 22,
            DOOR_4_OPEN = 8,
            DOOR_14_OPEN = 10,
            DOOR_24_OPEN = 12,
            DOOR_124_OPEN = 14,
            DOOR_34_OPEN = 24,
            DOOR_134_OPEN = 26,
            DOOR_234_OPEN = 28,
            DOOR_1234_OPEN = 30
        }

        public enum SamCardType
        {
            KochiTest,
            KochiProd, 
            IndoreTest,
            IndoreProd,
            AmritsarTest,
            AmritsarProd
        }

    }
}
