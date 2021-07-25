using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Utils
{
    public enum PrinterResponse
    {
        ACK = 0x06,
        PROGRAMMING_DONE = 0xAA
    }

    public enum PrinterStatus
    {
        Unknown = 0,
        PaperEnd = 16,
        NoError = 17,
        CoverOpen = 24,
        WrongCommand = 25,
        NotchError = 32,
        HeadingOverTemperatureError = 33,
        PowerSupplyVoltageError = 34,
        CutterError = 35
    }

    public enum FullStatus
    {
        PaperPresent = 0,
        PaperNotPreset = 1,
        PaperNearEnd = 2,
        TicketNotPresentInOutput = 3,
        TicketPresentInOutput = 4,
        PaperVirtuallyPresent = 5,
        VirtualPaperEnd = 6,
        NotchNotFound = 7,
        NotchFound = 8
    }

    public enum UserStatus
    {
        PrintingHeadDown = 0,
        PrintingHeadUpError = 1,
        CoverClosed = 2,
        CoverOpened = 3,
        NoSpooling = 4,
        Spooling = 5,
        DragPaperMotorOff = 6,
        DragPaperMotorOn = 7,
        LFKeyReleased = 8,
        LFKeyPressed = 9,
        FFKeyReleased = 10,
        FFKeyPressed = 11
    }

    public enum RecoverebleErrorStatus
    {
        HeadTemperatureOk = 0,
        HeadTemperatureError = 1,
        NoCOMError = 2,
        RS232COMError = 3,
        PowerSupplyVoltageOk = 4,
        PowerSupplyVoltageError = 5,
        AcknowledgeCommand = 6,
        NotAcknowledgeCommandError = 7,
        FreePaperPath = 8,
        PaperJam = 9,
        NotchSearchOk = 10,
        NotchSearchError = 11
    }

    public enum UnrecoverebleErrorStatus
    {
        CutterOk = 0,
        CutterError = 1,
        CutterCoverOk = 2,
        CutterCoverOpen = 3,
        RAMOk = 4,
        RAMError = 5,
        EEPROMOk = 6,
        EEPROMError = 7
    }

    public enum QREncodeScheme
    {
        QRcode = 0,
        MicroQR = 1
    }

    public class Svelta
    {
        public byte[] ASCIIGetBytes(string val)
        {
            return ASCIIEncoding.ASCII.GetBytes(val);
        }

        public byte[] PrintText(string text)
        {
            return ASCIIGetBytes(text);
        }

        public void ParseStatus(byte[] status,
                                ref List<FullStatus> fullStatus,
                                ref List<UserStatus> userStatus,
                                ref List<RecoverebleErrorStatus> recoverebleErrorStatus,
                                ref List<UnrecoverebleErrorStatus> unrecoverebleErrorStatus)
        {
            fullStatus.Clear();
            userStatus.Clear();
            recoverebleErrorStatus.Clear();
            unrecoverebleErrorStatus.Clear();

            if (status.Length >= 1)
            {
                fullStatus.Add(((status[0] & 0x01) == 0x01) ? FullStatus.PaperNotPreset : FullStatus.PaperPresent);
                fullStatus.Add(((status[0] & 0x04) == 0x04) ? FullStatus.PaperNearEnd : FullStatus.PaperPresent);
                fullStatus.Add(((status[0] & 0x20) == 0x20) ? FullStatus.TicketPresentInOutput : FullStatus.TicketNotPresentInOutput);
                fullStatus.Add(((status[0] & 0x40) == 0x40) ? FullStatus.VirtualPaperEnd : FullStatus.PaperVirtuallyPresent);
                fullStatus.Add(((status[0] & 0x80) == 0x80) ? FullStatus.NotchFound : FullStatus.NotchNotFound);
            }

            if (status.Length >= 2)
            {
                userStatus.Add(((status[1] & 0x01) == 0x01) ? UserStatus.PrintingHeadUpError : UserStatus.PrintingHeadDown);
                userStatus.Add(((status[1] & 0x02) == 0x02) ? UserStatus.CoverOpened : UserStatus.CoverClosed);
                userStatus.Add(((status[1] & 0x04) == 0x04) ? UserStatus.Spooling : UserStatus.NoSpooling);
                userStatus.Add(((status[1] & 0x08) == 0x08) ? UserStatus.DragPaperMotorOn : UserStatus.DragPaperMotorOff);
                userStatus.Add(((status[1] & 0x20) == 0x20) ? UserStatus.LFKeyPressed : UserStatus.LFKeyReleased);
                userStatus.Add(((status[1] & 0x40) == 0x40) ? UserStatus.FFKeyPressed : UserStatus.FFKeyReleased);
            }

            if (status.Length >= 3)
            {
                recoverebleErrorStatus.Add(((status[2] & 0x01) == 0x01) ? RecoverebleErrorStatus.HeadTemperatureError : RecoverebleErrorStatus.HeadTemperatureOk);
                recoverebleErrorStatus.Add(((status[2] & 0x02) == 0x02) ? RecoverebleErrorStatus.RS232COMError : RecoverebleErrorStatus.NoCOMError);
                recoverebleErrorStatus.Add(((status[2] & 0x08) == 0x08) ? RecoverebleErrorStatus.PowerSupplyVoltageError : RecoverebleErrorStatus.PowerSupplyVoltageOk);
                recoverebleErrorStatus.Add(((status[2] & 0x20) == 0x20) ? RecoverebleErrorStatus.NotAcknowledgeCommandError : RecoverebleErrorStatus.AcknowledgeCommand);
                recoverebleErrorStatus.Add(((status[2] & 0x40) == 0x40) ? RecoverebleErrorStatus.PaperJam : RecoverebleErrorStatus.FreePaperPath);
                recoverebleErrorStatus.Add(((status[2] & 0x80) == 0x80) ? RecoverebleErrorStatus.NotchSearchError : RecoverebleErrorStatus.NotchSearchOk);
            }

            if (status.Length >= 4)
            {
                unrecoverebleErrorStatus.Add(((status[3] & 0x01) == 0x01) ? UnrecoverebleErrorStatus.CutterError : UnrecoverebleErrorStatus.CutterOk);
                unrecoverebleErrorStatus.Add(((status[3] & 0x02) == 0x02) ? UnrecoverebleErrorStatus.CutterCoverOpen : UnrecoverebleErrorStatus.CutterCoverOk);
                unrecoverebleErrorStatus.Add(((status[3] & 0x04) == 0x04) ? UnrecoverebleErrorStatus.RAMError : UnrecoverebleErrorStatus.RAMOk);
                unrecoverebleErrorStatus.Add(((status[3] & 0x08) == 0x08) ? UnrecoverebleErrorStatus.EEPROMError : UnrecoverebleErrorStatus.EEPROMOk);
            }
        }

        #region BIT-IMAGE COMMANDS

        public byte[] ClearBuffer()
        {
            return ASCIIGetBytes("<CB>");
        }

        #endregion

        #region CHARACTER COMMANDS

        public byte[] RestoreTextHorizontal()
        {
            return ASCIIGetBytes("<NR>");
        }

        public byte[] Rotate90CounterClockwise()
        {
            return ASCIIGetBytes("<RL>");
        }

        public byte[] Rotate90Clockwise()
        {
            return ASCIIGetBytes("<RR>");
        }

        public byte[] Rotate180()
        {
            return ASCIIGetBytes("<RU>");
        }

        #endregion

        #region PRINT POSITION COMMANDS

        public byte[] SetTicketDimension(ushort length, ushort height, ushort notch, ushort dimnotch)
        {
            return ASCIIGetBytes(string.Format("<LHT{0:d},{1:d},{2:d},{3:d}>", length, height, notch, dimnotch));
        }

        public byte[] GetTicketDimension()
        {
            return ASCIIGetBytes("<T>");
        }


        /// <summary>
        /// Feed the paper n step
        /// </summary>
        /// <param name="step"></param>
        /// <remarks>1 STEP = 0,125 mm (1/8 mm)</remarks>
        /// <returns></returns>
        public byte[] FeedPaper(byte step)
        {
            return ASCIIGetBytes(string.Format("<MM{0:d}>", step));
        }

        public byte[] PositionCursor(ushort row, ushort column)
        {
            return ASCIIGetBytes(string.Format("<RC{0:d},{1:d}>", row, column));
        }

        #endregion

        #region MISCELLANEOUS COMMANDS

        /// <summary>
        /// ReadDT
        /// </summary>
        /// <param name="format"></param>
        /// <remarks>0 = DD/MM/YY hh:mm:ss, 1 = DDMMYYhhmmss, 2 = YYMMDDhhmmss, 3 = YYMMDDhhmmssd</remarks> 
        /// <returns></returns>
        public byte[] ReadDT(string format)
        {
            return ASCIIGetBytes(string.Format("<DT{0:d}>", format));
        }

        public byte[] ReloadPaper()
        {
            return ASCIIGetBytes("<LOAD>");
        }

        public byte[] SetDT(DateTime dt)
        {
            return ASCIIGetBytes(string.Format("<SDT {0:d} {1:yyMMddHHmmss}>", 2, dt));
        }

        public byte[] SetLabelControl(bool value)
        {
            return ASCIIGetBytes(string.Format("<JAM{0:d}>", value));
        }

        public byte[] ChangePrinterEmulationToESC_POS()
        {
            return ASCIIGetBytes("<EPOS>");
        }

        #endregion

        #region PRINT COMMANDS

        public byte[] PrintDate()
        {
            return ASCIIGetBytes("<DATE>");
        }

        public byte[] PrintTime()
        {
            return ASCIIGetBytes("<TIME>");
        }

        public byte[] PrintCut()
        {
            return ASCIIGetBytes("<P>");
        }

        public byte[] PrintCutReverse()
        {
            return ASCIIGetBytes("<p>");
        }

        public byte[] Print()
        {
            return ASCIIGetBytes("<Q>");
        }

        public byte[] PrintReverse()
        {
            return ASCIIGetBytes("<q>");
        }

        #endregion

        #region STATUS COMMANDS

        public byte[] StatusRequest(byte type = 1)
        {
            return ASCIIGetBytes(string.Format("<S{0:d}>", type));
        }

        public byte[] FullStatusRequest(byte y = 0x0F, byte x = 0x0F)
        {
            return ASCIIGetBytes(string.Format("<SBF,{0:X}{1:X}>", y, x));
        }

        #endregion

        #region TRUE TYPE FONTS MANAGEMENT COMMANDS

        public byte[] FontSetReqular()
        {
            return ASCIIGetBytes("<F:regular>");
        }

        public byte[] FontSetBold()
        {
            return ASCIIGetBytes("<F:bold>");
        }

        public byte[] FontSetItalic()
        {
            return ASCIIGetBytes("<F:italic>");
        }

        public byte[] FontSetDimension(byte size)
        {
            return ASCIIGetBytes(string.Format("<F:size:{0:d}>", size));
        }

        public byte[] FontSetRotation(ushort aa)
        {
            return ASCIIGetBytes(string.Format("<F:rotate:{0:d}>", aa));
        }

        public byte[] FontSetASCIIEncoding()
        {
            return ASCIIGetBytes("<F:enc:ascii>");
        }

        public byte[] FontSetUTF8Encoding()
        {
            return ASCIIGetBytes("<F:enc:utf-8>");
        }

        public byte[] FontSetUTF16Encoding()
        {
            return ASCIIGetBytes("<F:enc:utf-16>");
        }

        public byte[] FontSelect(byte no)
        {
            return ASCIIGetBytes(string.Format("<F{0}>", no));
        }

        #endregion

        #region BARCODE COMMANDS

        public byte[] QREncodeScheme(QREncodeScheme scheme)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,A,{0:d}>", scheme));
        }

        public byte[] QRModuleSize(byte size)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,B,{0:d}>", size));
        }

        public byte[] QRSize(byte size)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,C,{0:d}>", size));
        }

        public byte[] QRECCLevel(byte level)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,D,{0:d}>", level));
        }

        public byte[] QRPrint(string text)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,P,{0:d},{1}>", text.Length, text));
        }

        public byte[] PDF417SetNumOfCol(byte n = 0)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,A,{0:d}>", n));
        }

        public byte[] PDF417SetNumOfRow(byte n = 1)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,B,{0:d}>", n));
        }

        public byte[] PDF417SetWidth(byte w = 1)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,C,{0:d}>", w));
        }

        public byte[] PDF417SetHeight(byte h = 3)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,D,{0:d}>", h));
        }

        public byte[] PDF417SetErrCorrectionLevel(byte m, byte x)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,E,{0:d},{1:d}>", m, x));
        }

        public byte[] PDF417Store(string text)
        {
            return ASCIIGetBytes(string.Format("<B2Dk,P,{0:d},{1}>", text.Length, text));
        }

        #endregion

        #region LOGOS MANAGEMENT COMMANDS

        public byte[] SaveImage(ushort no, ushort x, ushort y, string id, byte[] data, ushort tbd = 0x0000)
        {
            byte[] noAry = BitConverter.GetBytes(no);
            Array.Reverse(noAry);

            byte[] xAry = BitConverter.GetBytes(x);
            Array.Reverse(xAry);

            byte[] yAry = BitConverter.GetBytes(y);
            Array.Reverse(yAry);

            byte[] tbdAry = BitConverter.GetBytes(tbd);
            Array.Reverse(tbdAry);

            byte[] idAry = new byte[16];
            Array.Copy(ASCIIGetBytes(id), 0, idAry, 0, id.Length);

            List<byte> Result = new List<byte>();

            Result.AddRange(ASCIIGetBytes("<PC"));
            Result.AddRange(noAry);
            Result.AddRange(xAry);
            Result.AddRange(yAry);
            Result.AddRange(tbdAry);
            Result.AddRange(idAry);
            Result.AddRange(data);
            Result.AddRange(ASCIIGetBytes(">"));

            return Result.ToArray();
        }

        public byte[] PrintImage(byte no, ushort x, ushort y, byte sp = 0)
        {
            return ASCIIGetBytes(string.Format("<PP{0:d},{1:d},{2:d},{3:d}>", no, x, y, sp));
        }

        public byte[] PrintRotatedImage(byte no, ushort x, ushort y, byte sp = 0)
        {
            return ASCIIGetBytes(string.Format("<PR{0:d},{1:d},{2:d},{3:d}>", no, x, y, sp));
        }

        public byte[] GetPictureHeaderInfo(byte no)
        {
            return ASCIIGetBytes(string.Format("<PI{0:d}>", no));
        }

        #endregion
    }
}
