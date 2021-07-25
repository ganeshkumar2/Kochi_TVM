using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.RptDispenser
{

    public static class rptc
    {
        public const byte PAGE_LEN = 4;
        public const byte MAC_LEN = 4;
        public const byte PERSO_SEC_PAGE_CNT = 6;
        public const byte PERSO_DATA_PAGE_CNT = 3;
        public const byte PERSO_SEC_START_PAGE = 16;
        public const byte TRAN_SEC_CNT = 3;
        public const byte TRAN_SEC_PAGE_CNT = 4;
        public const byte TRAN_SECS_START_PAGE = 22;
        public const byte MIN_VALUE_COUNTER = 128;
        public const byte CONFIG_PAGE_NO = 2;
        public const byte LOCK_PAGE_NO = 36;
    }

    public enum TICKET_TYPE
    {
        Invalid = 0,
        WeekendPass = 4,
        Group = 9,
        SJT = 11,
        RJT = 12,
        DayPass = 13,
        EventPass = 14,
        FreeExit = 15,
        PaidExit = 16,
        StationAccess = 20,
        Employee = 48
    }

    public interface ITicketBuffer
    {
        byte[] ToBuffer();
        void FromBuffer(byte[] data, int index);
    }

    public class TicketWeekendPass : ITicketBuffer
    {
        public byte institutionId;
        public byte[] rfu = new byte[3];
        public byte passType;
        public byte[] rfu1 = new byte[3];
        public byte[] rfu2 = new byte[2];
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Result[jj++] = institutionId;
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Result[jj++] = passType;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(rfu2, 0, Result, jj, rfu2.Length); jj += rfu2.Length;
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            institutionId = data[jj++];
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            passType = data[jj++];
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            Array.Copy(data, jj, rfu2, 0, rfu2.Length); jj += rfu2.Length;
            price = BitConverter.ToUInt16(data, jj);
        }
    }

    public class TicketGroup : ITicketBuffer
    {
        public UInt16 stationIn;
        public UInt16 stationOut;
        public byte peopleCount;
        public byte[] rfu = new byte[3];
        public byte[] rfu1 = new byte[2];
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(BitConverter.GetBytes(stationIn), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(BitConverter.GetBytes(stationOut), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Result[jj++] = peopleCount;
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            stationIn = BitConverter.ToUInt16(data, jj); jj += sizeof(ushort);
            stationOut = BitConverter.ToUInt16(data, jj); jj += sizeof(ushort);
            peopleCount = data[jj++];
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            price = BitConverter.ToUInt16(data, jj);
        }
    }

    public class TicketSJT : ITicketBuffer
    {
        public UInt16 stationIn;
        public UInt16 stationOut;
        public byte[] rfu = new byte[4];
        public byte[] rfu1 = new byte[2];
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(BitConverter.GetBytes(stationIn), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(BitConverter.GetBytes(stationOut), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            stationIn = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            stationOut = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            price = BitConverter.ToUInt16(data, jj);
        }
    }

    public class TicketRJT : ITicketBuffer
    {
        public UInt16 stationInOut;
        public UInt16 stationOutIn;
        public byte[] rfu = new byte[4];
        public byte[] rfu1 = new byte[2];
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(BitConverter.GetBytes(stationInOut), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(BitConverter.GetBytes(stationOutIn), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            stationInOut = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            stationOutIn = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            price = BitConverter.ToUInt16(data, jj);
        }
    }

    public class TicketDayPass : ITicketBuffer
    {
        public byte institutionId;
        public byte[] rfu = new byte[3];
        public byte[] rfu1 = new byte[4];
        public byte[] rfu2 = new byte[2];
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Result[jj++] = institutionId;
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(rfu2, 0, Result, jj, rfu2.Length); jj += rfu2.Length;
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            institutionId = data[jj++];
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            Array.Copy(data, jj, rfu2, 0, rfu2.Length); jj += rfu2.Length;
            price = BitConverter.ToUInt16(data, jj);
        }
    }

    public class TicketEventPass : ITicketBuffer
    {
        public DateTime startDT;
        public DateTime endDT;
        public UInt16 stationNo;
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(BitConverter.GetBytes(RPTUtils.DateTimeToUnix(startDT)), 0, Result, jj, sizeof(UInt32)); jj += sizeof(UInt32);
            Array.Copy(BitConverter.GetBytes(RPTUtils.DateTimeToUnix(endDT)), 0, Result, jj, sizeof(UInt32)); jj += sizeof(UInt32);
            Array.Copy(BitConverter.GetBytes(stationNo), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            startDT = new DateTime(2010, 1, 1).AddSeconds(BitConverter.ToUInt32(data, jj)); jj += sizeof(UInt32);
            endDT = new DateTime(2010, 1, 1).AddSeconds(BitConverter.ToUInt32(data, jj)); jj += sizeof(UInt32);
            stationNo = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            price = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
        }
    }

    public class TicketFreeExit : ITicketBuffer
    {
        public byte[] rfu = new byte[2];
        public UInt16 stationOut;
        public byte[] rfu1 = new byte[4];
        public byte[] rfu2 = new byte[4];

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(BitConverter.GetBytes(stationOut), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(rfu2, 0, Result, jj, rfu2.Length); jj += rfu2.Length;

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            stationOut = BitConverter.ToUInt16(data, jj); jj += sizeof(ushort);
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            Array.Copy(data, jj, rfu2, 0, rfu2.Length); jj += rfu2.Length;
        }
    }

    public class TicketPaidExit : ITicketBuffer
    {
        public byte[] rfu = new byte[2];
        public UInt16 stationOut;
        public byte[] rfu1 = new byte[4];
        public byte[] rfu2 = new byte[2];
        public UInt16 price;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(BitConverter.GetBytes(stationOut), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(rfu2, 0, Result, jj, rfu2.Length); jj += rfu2.Length;
            Array.Copy(BitConverter.GetBytes(price), 0, Result, jj, sizeof(UInt16));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            stationOut = BitConverter.ToUInt16(data, jj); jj += sizeof(ushort);
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            Array.Copy(data, jj, rfu2, 0, rfu2.Length); jj += rfu2.Length;
            price = BitConverter.ToUInt16(data, jj);
        }
    }

    public class TicketEmployee : ITicketBuffer
    {
        public byte subType;
        public byte institutionId;
        public byte[] rfu = new byte[2];
        public byte[] rfu1 = new byte[4];
        public DateTime validUntil;

        public TicketEmployee()
        {

        }

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Result[jj++] = subType;
            Result[jj++] = institutionId;
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(BitConverter.GetBytes(RPTUtils.DateTimeToUnix(validUntil)), 0, Result, jj, sizeof(UInt32));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            subType = data[jj++];
            institutionId = data[jj++];
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            validUntil = new DateTime(2010, 1, 1).AddSeconds(BitConverter.ToUInt32(data, jj));
        }
    }

    public class TicketStationAccess : ITicketBuffer
    {
        public UInt16 stationNo;
        public byte[] rfu = new byte[2];
        public byte[] rfu1 = new byte[4];
        public DateTime validUntil;

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(BitConverter.GetBytes(stationNo), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(rfu, 0, Result, jj, rfu.Length); jj += rfu.Length;
            Array.Copy(rfu1, 0, Result, jj, rfu1.Length); jj += rfu1.Length;
            Array.Copy(BitConverter.GetBytes(RPTUtils.DateTimeToUnix(validUntil)), 0, Result, jj, sizeof(UInt32));

            return Result;
        }

        public void FromBuffer(byte[] data, int index)
        {
            int jj = index;

            stationNo = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            Array.Copy(data, jj, rfu, 0, rfu.Length); jj += rfu.Length;
            Array.Copy(data, jj, rfu1, 0, rfu1.Length); jj += rfu1.Length;
            validUntil = new DateTime(2010, 1, 1).AddSeconds(BitConverter.ToUInt32(data, jj));
        }
    }

    public class RPTPersoSector
    {
        private TICKET_TYPE ticketType_;

        public byte version = 0x01;
        public bool testing = false;
        public TICKET_TYPE ticketType
        {
            get { return ticketType_; }
        }
        public ushort terminalId;
        public DateTime dt = DateTime.Now;
        public object ticketData;
        public byte[] MAC = new byte[rptc.MAC_LEN];

        public RPTPersoSector(ushort terminalId, DateTime dt, object data)
        {
            if (data is TicketWeekendPass)
                ticketType_ = TICKET_TYPE.WeekendPass;
            else if (data is TicketGroup)
                ticketType_ = TICKET_TYPE.Group;
            else if (data is TicketSJT)
                ticketType_ = TICKET_TYPE.SJT;
            else if (data is TicketRJT)
                ticketType_ = TICKET_TYPE.RJT;
            else if (data is TicketDayPass)
                ticketType_ = TICKET_TYPE.DayPass;
            else if (data is TicketEventPass)
                ticketType_ = TICKET_TYPE.EventPass;
            else if (data is TicketFreeExit)
                ticketType_ = TICKET_TYPE.FreeExit;
            else if (data is TicketPaidExit)
                ticketType_ = TICKET_TYPE.PaidExit;
            else if (data is TicketStationAccess)
                ticketType_ = TICKET_TYPE.StationAccess;
            else if (data is TicketEmployee)
                ticketType_ = TICKET_TYPE.Employee;
            else
                ticketType_ = TICKET_TYPE.Invalid;

            this.ticketData = (ticketType_ != TICKET_TYPE.Invalid) ? data : null;

            this.terminalId = terminalId;
            this.dt = dt;
        }

        public RPTPersoSector(byte[] data)
        {
            int jj = 0;

            byte ticketStatus = data[jj++];

            version = Convert.ToByte(ticketStatus & 0x0F);
            testing = (ticketStatus & 0x80) == 0x80;

            ticketType_ = (TICKET_TYPE)data[jj++];
            terminalId = BitConverter.ToUInt16(data, jj); jj += sizeof(ushort);
            dt = new DateTime(2010, 1, 1).AddSeconds(BitConverter.ToUInt32(data, jj)); jj += sizeof(uint);

            if (ticketType_ == TICKET_TYPE.WeekendPass)
                ticketData = new TicketWeekendPass();
            else if (ticketType_ == TICKET_TYPE.Group)
                ticketData = new TicketGroup();
            else if (ticketType_ == TICKET_TYPE.SJT)
                ticketData = new TicketSJT();
            else if (ticketType_ == TICKET_TYPE.RJT)
                ticketData = new TicketRJT();
            else if (ticketType_ == TICKET_TYPE.DayPass)
                ticketData = new TicketDayPass();
            else if (ticketType_ == TICKET_TYPE.EventPass)
                ticketData = new TicketEventPass();
            else if (ticketType_ == TICKET_TYPE.FreeExit)
                ticketData = new TicketFreeExit();
            else if (ticketType_ == TICKET_TYPE.PaidExit)
                ticketData = new TicketPaidExit();
            else if (ticketType_ == TICKET_TYPE.StationAccess)
                ticketData = new TicketStationAccess();
            else if (ticketType_ == TICKET_TYPE.Employee)
                ticketData = new TicketEmployee();
            else
                ticketData = null;

            if (ticketData != null)
                ((ITicketBuffer)ticketData).FromBuffer(data, jj); jj += 12;

            Array.Copy(data, jj, MAC, 0, MAC.Length);
        }
    }

    public class RPTTranSector
    {
        public DateTime dt;
        public ushort stationNo;
        public ushort gateNo;
        public byte direction;
        public byte operationType;
        public ushort usageCount;
        public byte[] MAC = new byte[rptc.MAC_LEN];

        public RPTTranSector()
        {
            dt = new DateTime(2010, 1, 1);
            stationNo = 0;
            gateNo = 0;
            direction = (byte)TRAN_DIR.None;
            operationType = 0;
            usageCount = 0;
            Array.Clear(MAC, 0, MAC.Length);
        }

        public RPTTranSector(DateTime dt)
        {
            this.dt = dt;
            this.stationNo = 0xFFFF;
            this.gateNo = 0xFFFF;
            this.direction = (byte)TRAN_DIR.Unknown;
            this.operationType = 0xFF;
            this.usageCount = 0xFFFF;
            Array.Copy(new byte[rptc.MAC_LEN] { 0xFF, 0xFF, 0xFF, 0xFF }, 0, this.MAC, 0, rptc.MAC_LEN);
        }

        public RPTTranSector(byte[] data)
        {
            int jj = 0;

            dt = new DateTime(2010, 1, 1).AddSeconds(BitConverter.ToUInt32(data, jj)); jj += sizeof(UInt32);
            stationNo = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            gateNo = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            direction = data[jj++];
            operationType = data[jj++];
            usageCount = BitConverter.ToUInt16(data, jj); jj += sizeof(UInt16);
            Array.Copy(data, jj, MAC, 0, MAC.Length);
        }

        public byte[] ToBuffer()
        {
            byte[] Result = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

            int jj = 0;

            Array.Copy(BitConverter.GetBytes(RPTUtils.DateTimeToUnix(this.dt)), 0, Result, jj, sizeof(uint)); jj += sizeof(uint);
            Array.Copy(BitConverter.GetBytes(this.stationNo), 0, Result, jj, sizeof(ushort)); jj += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.gateNo), 0, Result, jj, sizeof(ushort)); jj += sizeof(ushort);
            Result[jj++] = this.direction;
            Result[jj++] = this.operationType;
            Array.Copy(BitConverter.GetBytes(this.usageCount), 0, Result, jj, sizeof(ushort)); jj += sizeof(ushort);
            Array.Copy(this.MAC, 0, Result, jj, this.MAC.Length);

            return Result;
        }
    }

    public class RPTAnalise
    {
        public bool cancelled;
        public bool persoMACValid;
        public bool[] tranMACValid = new bool[rptc.TRAN_SEC_CNT];
        public TRAN_ORDER[] tranOrder = new TRAN_ORDER[rptc.TRAN_SEC_CNT];
    }

    public enum LIB_ERR
    {
        UNKNOWN = 255,
        THERE_IS_NO_AVAILABLE_PORT = 254,
        CRT571_COULD_NOT_FOUND = 253,
        CRT571_COMM_OPEN = 252,
        SAM_UNLOCK_PART_1 = 251,
        SAM_UNLOCK_PART_2 = 250,
        SAM_UNLOCK_PART_3 = 249,
        SAM_UNLOCK_MAC_VERIFY = 248,
        SAM_ACTIVATE_OFFLINE_KEY = 247,
        SAM_SELECT = 246,
        SAM_ACTIVATE = 245,
        SAM_GENERATE_MAC = 244,
        SAM_ENCHIPHER_OFFLINE_DATA = 243,
        CRT571_READ_SECTOR_DATA = 242,
        CRT571_ACCESS_DENIED = 241,
        CRT571_SET_PASSWORD = 240,
        CRT571_WRITE_SECTOR_DATA = 239,
        CRT571_SET_VALUE_COUNTER = 238,
        INVALID_VALUE_COUNTER = 237,
        ACTIVATE_CONTACTLESS_ICCARD_FAILES_ON_READ = 236,
        SAM_DECHIPER_OFFLINE_DATA = 235,
        CRT571_GET_VALUE_COUNTER = 234,
        CRT571_DEC_VALUE_COUNTER = 233,
        INVALID_TICKET_TYPE = 232,
        CRT571_ACTIVATE_CARD = 231,
        INVALID_TICKET_STATE = 230,
        THERE_IS_NO_PCSC_READER = 229,
        GET_READERS_FAILED = 228,
        CONNECT_READER_FAILED = 227,
        CONNECT_SAM_FAILED = 226,
        GET_CARD_UID_FAILED = 225,
        CONNECT_CARD_FAILED = 224,
        ACS_READ_SECTOR_DATA = 223,
        ACS_ACCESS_DENIED = 222,
        ACS_GET_VALUE_COUNTER = 221,
        ACS_WRITE_SECTOR_DATA = 220,
        ACS_DEC_VALUE_COUNTER = 219,
        ACS_SET_PASSWORD = 218,
        ACS_START_TRANSPARENT_SESSION = 217,
        ACS_ACTIVATE_ISO14443_3A = 216,
        SAM_GET_VERSION = 215,

        NO_ERROR = 0
    }

    public enum DISP_STAT
    {
        STACKER_UNKNOWN = 0x00,
        STACKER_NOCARD = 0x30,
        STACKER_LACK = 0x31,
        STACKER_FULL = 0x32,
    }

    public enum ST0
    {
        Unknown,
        NoCardInCRT571 = 0x30,
        OneCardInGate = 0x31,
        OneCardOnRFICCardPosition = 0x32
    }

    public enum ST1
    {
        Unknown,
        NoCardInStacker = 0x30,
        FewCardInStacker = 0x31,
        EnoughCardsInStacker = 0x32
    }

    public enum ST2
    {
        Unknown,
        ErrorCardBinNotFull = 0x30,
        ErrorCardBinFull = 0x31
    }

    public enum SENSOR
    {
        Unkown,
        NoCard = 0x30,
        HaveCard = 0x31,
    }

    public enum E01
    {
        ReceptionOfUndefinedCommand = 0x00,
        CommandParameterError = 0x01,
        CommandSequenceError = 0x02,
        OutOfHardwareSupportCommand = 0x03,
        CommandDataError = 0x04,
        ICCardContactNotRelease = 0x05,
        CardJam = 0x10,
        SensorError = 0x12,
        TooLongCard = 0x13,
        TooShortCard = 0x14,
        CardMoveManually = 0x16,
        MoveCardWhenRecycling = 0x40,
        MagnentOfICCardError = 0x41,
        DisableToMoveCardToICCardPosition = 0x43,
        ManuallyMoveCard = 0x45,
        ReceiveCardCounterOverflow = 0x50,
        MotorError = 0x51,
        ShortCircuitOfICCardSupplyPower = 0x60,
        ActiviationOfCardFalse = 0x61,
        CommandOutOfICCardSupport = 0x62,
        DisablityOfICCard = 0x65,
        CommandOutOfICCurrentCardSupport = 0x66,
        ICCardTransmittionError = 0x67,
        ICCardTransmittionOvertime = 0x68,
        CPU_SAMNon_ComplianceToEMVStandard = 0x69,
        EmptyStacker = 0xA0,
        FullStacker = 0xA1,
        NotReset = 0xB0,
        None = 0xFF
    }

    public enum TRAN_ORDER
    {
        LAST = 0,
        PREVIOUS = 1,
        OLDEST = 2,
        INVALID = 255
    }

    public enum OPR_TYPE
    {
        Issued = 0,
        CheckIn = 1,
        CheckOut = 2,
        Replacement = 3,
        EntryMismatch = 4,
        ExitMismatch = 5,
        OverStay = 6,
        OverTrevel = 7,
        Refund = 8,
        Cancellation = 9,
        ByPassMode1 = 10,
        ByPassMode2 = 11,
        FreeExited = 12,
        TopupEmployee = 13
    }

    public enum DEVICE_TYPE
    {
        CRT571_DISPENSER = 0,
        ACS_READER = 1
    }

    public enum TRAN_DIR
    {
        None = 0x00,
        Entry = 1,
        Exit = 2,
        Unknown = 0xFF
    }
}
