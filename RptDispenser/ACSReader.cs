using PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.RptDispenser
{
    public class ACSReader
    {
        private const byte ACK = 0x0A;

        private SCardContext context = null;
        private SCardReader reader = null;
        private SCardReader sam = null;
        private string[] readers = null;
        private int indexReader = -1;
        private int indexSAM = -1;

        private IntPtr SCARD_CTL_CODE(int code)
        {
            int ioctl = (0x31 << 16 | (code) << 2);

            return new IntPtr(ioctl);
        }

        public string Version = "";

        public bool Init(ref LIB_ERR error)
        {
            bool Result = true;

            try
            {
                context = new SCardContext();

                context.Establish(SCardScope.System);

                string[] groups = context.GetReaderGroups();

                Result = (groups != null) && (groups.Length > 0);

                if (Result)
                {
                    readers = context.GetReaders(groups);

                    Result = (readers != null) && (readers.Length > 0);
                }

                if (!Result)
                    error = LIB_ERR.THERE_IS_NO_PCSC_READER;

                if (Result)
                {
                    reader = new SCardReader(context);

                    indexReader = Array.FindIndex(readers, element => element.Contains("PICC"));

                    Result = (indexReader > -1);

                    if (!Result)
                        error = LIB_ERR.CONNECT_READER_FAILED;
                }

                if (Result)
                {
                    sam = new SCardReader(context);

                    indexSAM = Array.FindIndex(readers, element => element.Contains("SAM"));

                    Result = (indexSAM > -1);

                    if (Result)
                    {
                        SCardError ret = sam.Connect(readers[indexSAM], SCardShareMode.Shared, SCardProtocol.T0 | SCardProtocol.T1);

                        Result = ((ret == SCardError.Success) && sam.IsConnected);
                    }

                    if (!Result)
                        error = LIB_ERR.CONNECT_SAM_FAILED;
                }
            }
            catch (Exception E)
            {
                Result = false;

                error = LIB_ERR.GET_READERS_FAILED;
            }

            return Result;
        }

        public void Done()
        {
            try
            {
                if (sam != null)
                {
                    if (sam.IsConnected)
                        sam.Disconnect(SCardReaderDisposition.Unpower);

                    sam.Dispose();
                }

                if (reader != null)
                {
                    if (reader.IsConnected)
                        reader.Disconnect(SCardReaderDisposition.Unpower);

                    reader.Dispose();
                }
            }
            catch (Exception E)
            {
                // !!!
            }
        }

        public bool ReadVersionInformation()
        {
            bool Result = false;

            Version = "";

            byte[] capdu = new byte[] { 0xE0, 0x00, 0x00, 0x18, 0x00 };
            byte[] rapdu = new byte[128];

            try
            {
                SCardError ret = reader.Connect(readers[indexReader], SCardShareMode.Direct, SCardProtocol.Unset);

                Result = (ret == SCardError.Success);

                if (Result)
                {
                    ret = reader.Control(SCARD_CTL_CODE(3500), capdu, ref rapdu);

                    Result = (ret == SCardError.Success) &&
                             (rapdu.Length >= 6) && (rapdu[0] == 0xE1) && (rapdu[1] == 0x00) && (rapdu[2] == 0x00) && (rapdu[3] == 0x00);

                    if (Result)
                        Version = System.Text.ASCIIEncoding.ASCII.GetString(rapdu, 5, rapdu[4]);

                    reader.Disconnect(SCardReaderDisposition.Leave);
                }
            }
            catch (Exception E)
            {
                Result = false;
            }

            return Result;
        }

        public bool GetSerialNumber(ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;
            uidLen = 0;

            byte[] capdu = new byte[5] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
            byte[] rapdu = new byte[32];

            try
            {
                SCardError ret = reader.Transmit(capdu, ref rapdu);

                Result = (ret == SCardError.Success) &&
                         ((rapdu.Length == 4 + 2) || (rapdu.Length == 7 + 2)) && (rapdu[rapdu.Length - 2] == 0x90) && (rapdu[rapdu.Length - 1] == 0x00);

                if (Result)
                {
                    uidLen = Convert.ToByte(rapdu.Length - 2);

                    uid = new byte[uidLen];

                    Array.Copy(rapdu, 0, uid, 0, uidLen);
                }
                else
                    error = LIB_ERR.GET_CARD_UID_FAILED;
            }
            catch (Exception E)
            {
                error = LIB_ERR.GET_CARD_UID_FAILED;

                Result = false;
            }

            return Result;
        }

        public bool CardConnect(ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            bool Result = true;

            try
            {
                SCardError ret = reader.Connect(readers[indexReader], SCardShareMode.Shared, SCardProtocol.T1);

                Result = (ret == SCardError.Success) && reader.IsConnected;
            }
            catch
            {
                Result = false;
            }

            if (Result)
            {
                Result = GetSerialNumber(ref uid, ref uidLen, ref error) &&
                         StartTransparentSession(ref error) &&
                         ActivateISO14443_3A(ref error);
            }
            else
                error = LIB_ERR.CONNECT_CARD_FAILED;

            return Result;
        }

        public bool CardDisconnect()
        {
            bool Result = true;

            try
            {
                Result = (reader.Disconnect(SCardReaderDisposition.Unpower) == SCardError.Success);
            }
            catch
            {
                // !!!
            }

            return Result;
        }

        public bool IsCardOnRFCardOperationPosition()
        {
            bool Result = true;

            SCardError ret = reader.Connect(readers[indexReader], SCardShareMode.Shared, SCardProtocol.T1);

            Result = (ret == SCardError.Success) && reader.IsConnected;

            byte[] uid = new byte[7];
            byte uidLen = 0;

            LIB_ERR error = LIB_ERR.NO_ERROR;

            if (Result)
            {
                Result = GetSerialNumber(ref uid, ref uidLen, ref error);

                Result = Result && (error == LIB_ERR.NO_ERROR);
            }
            else
                error = LIB_ERR.CONNECT_CARD_FAILED;

            return Result;
        }

        public bool StartTransparentSession(ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[7] { 0xFF, 0xC2, 0x00, 0x00, 0x02, 0x81, 0x00 };
            byte[] rapdu = new byte[32];

            try
            {
                SCardError ret = reader.Transmit(capdu, ref rapdu);

                Result = (ret == SCardError.Success) &&
                         (rapdu.Length == 7) && (rapdu[rapdu.Length - 2] == 0x90) && (rapdu[rapdu.Length - 1] == 0x00);
            }
            catch (Exception E)
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_START_TRANSPARENT_SESSION;

            return Result;
        }

        public bool TurnTheAntennaOn()
        {
            bool Result = false;

            byte[] capdu = new byte[7] { 0xFF, 0xC2, 0x00, 0x00, 0x02, 0x84, 0x00 };
            byte[] rapdu = new byte[32];

            try
            {
                SCardError ret = reader.Transmit(capdu, ref rapdu);

                Result = (ret == SCardError.Success) &&
                         (rapdu.Length == 7) && (rapdu[rapdu.Length - 2] == 0x90) && (rapdu[rapdu.Length - 1] == 0x00);
            }
            catch (Exception E)
            {
                Result = false;
            }

            return Result;
        }

        public bool ActivateISO14443_3A(ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[9] { 0xFF, 0xC2, 0x00, 0x02, 0x04, 0x8F, 0x02, 0x00, 0x03 };
            byte[] rapdu = new byte[64];

            try
            {
                SCardError ret = reader.Transmit(capdu, ref rapdu);

                Result = (ret == SCardError.Success) &&
                         (rapdu.Length > 7) && (rapdu[rapdu.Length - 2] == 0x90) && (rapdu[rapdu.Length - 1] == 0x00);
            }
            catch (Exception E)
            {
                Result = false;
            }

            if (Result)
            {
                capdu = new byte[15] { 0xFF, 0xC2, 0x00, 0x01, 0x0A, 0x90, 0x02, 0x10, 0x00, 0xFF, 0x6E, 0x03, 0x07, 0x01, 0x0A };
                Array.Resize<byte>(ref rapdu, 128);

                try
                {
                    SCardError ret = reader.Transmit(capdu, ref rapdu);

                    Result = (ret == SCardError.Success) &&
                             (rapdu.Length == 7) && (rapdu[rapdu.Length - 2] == 0x90) && (rapdu[rapdu.Length - 1] == 0x00);
                }
                catch (Exception E)
                {
                    Result = false;
                }
            }

            if (!Result)
                error = LIB_ERR.ACS_ACTIVATE_ISO14443_3A;

            return Result;
        }

        public bool EndTransparentSession()
        {
            bool Result = true;

            byte[] capdu = new byte[7] { 0xFF, 0xC2, 0x00, 0x00, 0x02, 0x82, 0x00 };
            byte[] rapdu = new byte[32];

            try
            {
                SCardError ret = reader.Transmit(capdu, ref rapdu);

                Result = (ret == SCardError.Success) &&
                         (rapdu.Length == 7) && (rapdu[rapdu.Length - 2] == 0x90) && (rapdu[rapdu.Length - 1] == 0x00);
            }
            catch (Exception E)
            {
                Result = false;
            }

            return Result;
        }

        public bool SendAPDUSAM(byte[] capdu, ref byte[] rapdu)
        {
            bool Result = false;

            rapdu = new byte[128];

            try
            {
                Result = (sam.Transmit(capdu, ref rapdu) == SCardError.Success);
            }
            catch
            {
                // !!! 
            }

            return Result;
        }

        public bool ReadSectorData(byte pageNo, ref byte[] data, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x0B, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x02, 0x31, pageNo };
            byte[] rapdu = new byte[32];

            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success) &&
                         (rapdu.Length >= 14 + rptc.PAGE_LEN + 2) &&
                         (rapdu[13] == ACK) &&
                         (rapdu[rapdu.Length - 2] == 0x90) &&
                         (rapdu[rapdu.Length - 1] == 0x00);

                if (Result)
                    Array.Copy(rapdu, 14, data, 0, rptc.PAGE_LEN);
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_READ_SECTOR_DATA;

            return Result;
        }

        public bool WriteSectorData(byte pageNo, byte[] data, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x0F, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x06, 0xA2, pageNo, 0x00, 0x00, 0x00, 0x00 };
            Array.Copy(data, 0, capdu, capdu.Length - data.Length, data.Length);

            byte[] rapdu = new byte[32];

            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success) &&
                         (rapdu.Length >= 14 + 2) &&
                         (rapdu[14] == ACK) &&
                         (rapdu[rapdu.Length - 2] == 0x90) &&
                         (rapdu[rapdu.Length - 1] == 0x00);
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_WRITE_SECTOR_DATA;

            return Result;
        }

        public bool Access(byte[] password, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x0E, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x05, 0xB2, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(password, 0, capdu, capdu.Length - password.Length, password.Length);

            byte[] rapdu = new byte[32];
            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success);

                Result = Result &&
                         (rapdu.Length >= 17) && (rapdu[12] == 0x97) && (rapdu[13] == 0x01) && (rapdu[14] == ACK);
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_ACCESS_DENIED;

            return Result;
        }

        public bool GetValueCounter(ref ushort counter, ref LIB_ERR error)
        {
            bool Result = false;

            counter = 0x00;
            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x0C, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x03, 0xD0, 0x00, 0x00 };
            byte[] rapdu = new byte[32];

            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success);

                Result = Result &
                         (rapdu.Length >= 20) && (rapdu[12] == 0x97) && (rapdu[13] == 0x04);

                if (Result)
                    counter = BitConverter.ToUInt16(rapdu, 14);
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_GET_VALUE_COUNTER;

            return Result;
        }

        public bool DecValueCounter(ref ushort counter, ref LIB_ERR error)
        {
            bool Result = false;

            counter = 0x00;
            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x0C, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x03, 0xD0, 0x01, 0x00 };
            byte[] rapdu = new byte[32];

            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success);

                Result = Result &
                         (rapdu.Length >= 20) && (rapdu[12] == 0x97) && (rapdu[13] == 0x04);

                if (Result)
                    counter = BitConverter.ToUInt16(rapdu, 14);
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_DEC_VALUE_COUNTER;

            return Result;
        }

        public bool SetValueCounter(ushort counter, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] value = new byte[2];
            RPTUtils.LEU16(counter, ref value, 0);

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x13, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x0A, 0xA1, 0x22, value[0], (byte)(value[0] ^ 0xFF), value[1], 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] rapdu = new byte[32];

            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success);

                Result = Result &&
                        (rapdu.Length >= 17) && (rapdu[12] == 0x97) && (rapdu[13] == 0x01) && (rapdu[14] == ACK);
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.CRT571_SET_VALUE_COUNTER;

            return Result;
        }

        public bool SetPassword(byte[] password, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[] { 0xFF, 0xC2, 0x00, 0x01, 0x0E, 0x5F, 0x46, 0x04, 0x40, 0x42, 0x0F, 0x00, 0x95, 0x05, 0xB1, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(password, 0, capdu, capdu.Length - password.Length, password.Length);

            byte[] rapdu = new byte[32];

            try
            {
                Result = (reader.Transmit(capdu, ref rapdu) == SCardError.Success);

                Result = Result &&
                         (rapdu.Length >= 22) && (rapdu[12] == 0x97) && (rapdu[13] == 0x06);

                if (Result)
                {
                    byte[] rpassword = new byte[password.Length];
                    Array.Copy(rapdu, 14, rpassword, 0, password.Length);

                    Result = password.SequenceEqual(rpassword);
                }
            }
            catch
            {
                Result = false;
            }

            if (!Result)
                error = LIB_ERR.ACS_SET_PASSWORD;

            return Result;
        }
    }
}
