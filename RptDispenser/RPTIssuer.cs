using CRT571Dll;
using SAMOprLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Kochi_TVM.RptDispenser
{
    public class RPTIssuer
    {
        public RPTIssuer(byte[] samUnlockKey)
        {
            _samUnlockKey = samUnlockKey;
        }

        #region PRIVATE MEMBERS

        private const byte ACK = 0x0A;
        private const byte NACK0 = 0x00;
        private const byte NACK1 = 0x01;

        private SAM_OPR_ERR samErr = SAM_OPR_ERR.NO_ERROR;

        private SAMOpr sam = new SAMOpr();
        private CRT571 crt571 = new CRT571();
        private ACSReader acs = new ACSReader();

        private DEVICE_TYPE deviceType = DEVICE_TYPE.CRT571_DISPENSER;

        private readonly byte[] _samUnlockKey = null;

        // prod
        //private byte[] samUnlockKey = new byte[16] { 0x0E, 0x34, 0xCE, 0x38, 0x4A, 0x75, 0x6D, 0x61, 0x0B, 0xF4, 0x08, 0xA2, 0x25, 0x83, 0xDF, 0x10 };

        // test
        //private byte[] samUnlockKey = new byte[16] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };

        private byte samUnlockKeyNr = 0x00;
        private byte samUnlockKeyVer = 0x00;

        private byte persoMACKeyNr = 0x10;
        private byte persoMACKeyVer = 0x01;

        private byte tranMACKeyNr = 0x11;
        private byte tranMACKeyVer = 0x01;

        private byte passwordKeyNr = 0x20;
        private byte passwordKeyVer = 0x01;

        private byte encryptKeyNr = 0x30;
        private byte encryptKeyVer = 0x01;

        private byte configuration = 0x86; // 1 0 0 0 0 1 1 0 ((16-bit Value Counter (bit7), Password Read protected, Write Protected (bit2, bit1)))
        private byte lock4 = 0x0C; // 0 0 0 0 1 1 0 0 (16-bit Value Counter (bit2, bit3))

        #endregion

        #region PUBLIC MEMBERS

        public string crt571Port = string.Empty;
        public byte crt571Addr = 0;
        public int crt571BaudRate = 38400;

        #endregion

        #region PRIVATE METHODS

        private bool Sam_ReaderTransieve(object sender, byte[] capdu, ref byte[] rapdu)
        {
            bool Result = false;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = crt571.SendAPDUSAM(capdu, ref rapdu);
            else
                Result = acs.SendAPDUSAM(capdu, ref rapdu);

            return Result;
        }

        private bool CRT571_Init(ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            crt571.Addr = crt571Addr;
            crt571.ReadTimeout = 5000;
            crt571.WriteTimeout = 3000;
            crt571.BaudRate = crt571BaudRate;

            if (Result)
            {
                if (crt571Port == string.Empty)
                    Result = CRT571_Scan(ref error);
                else
                {
                    Result = crt571.CommOpen(crt571Port);

                    if (Result)
                        Result = crt571.Reset();

                    if (!Result)
                        error = LIB_ERR.CRT571_COMM_OPEN;
                }
            }

            if (Result)
            {
                Result = crt571.SelectSAM(1);

                if (!Result)
                    error = LIB_ERR.SAM_SELECT;
            }

            if (Result)
            {
                byte type = 0x00;
                byte[] atr = new byte[28];

                Result = crt571.ActiveSAM(SAMVcc.ISO_IEC7816_3_5V, ref type, ref atr);

                if (!Result)
                    error = LIB_ERR.SAM_ACTIVATE;
            }

            if (Result)
            {
                Result = sam.Unlock(_samUnlockKey, samUnlockKeyNr, samUnlockKeyVer, ref samErr);

                error = RPTUtils.SAMToLibErr(samErr);
            }

            return Result;
        }

        private void CRT571_Done()
        {
            crt571.DeactivateRFIDCard();

            crt571.DeactivateSAM();

            crt571.CommClose();
        }

        private bool CRT571_Scan(ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            string[] portNames = SerialPort.GetPortNames();

            Result = (portNames.Length > 0);

            if (!Result)
                error = LIB_ERR.THERE_IS_NO_AVAILABLE_PORT;

            if (Result)
            {
                int pp = 0;

                do
                {
                    crt571Port = portNames[pp];

                    Result = crt571.CommOpen(crt571Port);

                    if (Result)
                        Result = crt571.Reset();

                    if (!Result)
                        crt571.CommClose();

                    pp++;
                }
                while ((pp < portNames.Length) && (!Result));

                if (!Result)
                    error = LIB_ERR.CRT571_COULD_NOT_FOUND;
            }

            return Result;
        }

        private bool CRT571_ReadSectorData(byte pageNo, ref byte[] data, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            byte sw1 = 0x00;
            byte sw2 = 0x00;

            Result = crt571.ReadSectorData(pageNo, ref sw1, ref sw2, ref data);

            if (!Result)
                error = LIB_ERR.CRT571_READ_SECTOR_DATA;

            return Result;
        }

        private bool CRT571_WriteSectorData(byte pageNo, byte[] data, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            Result = crt571.WriteSectorData(pageNo, data);

            if (!Result)
                error = LIB_ERR.CRT571_READ_SECTOR_DATA;

            return Result;
        }

        private bool CRT571_Access(byte[] password, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] wdata = new byte[1 + 4] { 0xB2, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(password, 0, wdata, 1, password.Length);

            byte[] rdata = null;

            Result = crt571.ChannelRawCmd(wdata, ref rdata);

            Result = Result & (rdata != null) && (rdata[0] == ACK);

            if (!Result)
                error = LIB_ERR.CRT571_ACCESS_DENIED;

            return Result;
        }

        private bool CRT571_SetPassword(byte[] password, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] wdata = new byte[1 + 4] { 0xB1, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(password, 0, wdata, 1, password.Length);

            byte[] rdata = null;

            Result = crt571.ChannelRawCmd(wdata, ref rdata);

            Result = Result && (rdata != null) && (rdata.Length == password.Length) && password.Compare(rdata);

            if (!Result)
                error = LIB_ERR.CRT571_SET_PASSWORD;

            return Result;
        }

        private bool CRT571_SetValueCounter(ushort counter, ref LIB_ERR error)
        {
            bool Result = false;

            error = LIB_ERR.NO_ERROR;

            byte[] value = new byte[2];
            RPTUtils.LEU16(counter, ref value, 0);

            byte[] wdata = new byte[10] { 0xA1, 0x22, value[0], (byte)(value[0] ^ 0xFF), value[1], 0x00, 0xFF, 0xFF, 0xFF, 0xFF };

            byte[] rdata = null;

            Result = crt571.ChannelRawCmd(wdata, ref rdata);

            Result = Result & (rdata != null);

            if (!Result)
                error = LIB_ERR.CRT571_SET_VALUE_COUNTER;

            return Result;
        }

        private bool CRT571_GetValueCounter(ref ushort counter, ref LIB_ERR error)
        {
            bool Result = false;

            counter = 0x00;
            error = LIB_ERR.NO_ERROR;

            byte[] wdata = new byte[3] { 0xD0, 0x00, 0x00 };

            byte[] rdata = null;

            Result = crt571.ChannelRawCmd(wdata, ref rdata);

            Result = Result & (rdata != null) && (rdata.Length == 2);

            if (Result)
                counter = BitConverter.ToUInt16(rdata, 0);
            else
                error = LIB_ERR.CRT571_GET_VALUE_COUNTER;

            return Result;
        }

        private bool CRT571_DecValueCounter(ref ushort counter, ref LIB_ERR error)
        {
            bool Result = false;

            counter = 0x00;
            error = LIB_ERR.NO_ERROR;

            byte[] wdata = new byte[3] { 0xD0, 0x01, 0x00 };

            byte[] rdata = null;

            Result = crt571.ChannelRawCmd(wdata, ref rdata);

            Result = Result & (rdata != null) && (rdata.Length == 2);

            if (Result)
                counter = BitConverter.ToUInt16(rdata, 0);
            else
                error = LIB_ERR.CRT571_DEC_VALUE_COUNTER;

            return Result;
        }

        private byte[] BuildPersoSectorBuffer(RPTPersoSector persoSector, byte[] page00, byte[] page01, ref LIB_ERR error)
        {
            byte[] Result = new byte[rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN];

            error = LIB_ERR.NO_ERROR;

            int jj = 0;

            Result[jj++] = persoSector.version;
            Result[jj++] = (byte)persoSector.ticketType;
            Array.Copy(BitConverter.GetBytes(persoSector.terminalId), 0, Result, jj, sizeof(UInt16)); jj += sizeof(UInt16);
            Array.Copy(BitConverter.GetBytes(RPTUtils.DateTimeToUnix(persoSector.dt)), 0, Result, jj, sizeof(UInt32)); jj += sizeof(UInt32);
            Array.Copy(((ITicketBuffer)persoSector.ticketData).ToBuffer(), 0, Result, jj, rptc.PERSO_DATA_PAGE_CNT * rptc.PAGE_LEN);

            byte[] MAC = new byte[rptc.MAC_LEN];
            byte[] plainData = new byte[page00.Length + page01.Length + Result.Length - rptc.MAC_LEN];

            jj = 0;

            Array.Copy(page00, 0, plainData, jj, page00.Length); jj += page00.Length;
            Array.Copy(page01, 0, plainData, jj, page01.Length); jj += page01.Length;
            Array.Copy(Result, 0, plainData, jj, rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN - rptc.MAC_LEN);
            sam.Generate_MAC(plainData, (byte)MAC.Length, ref MAC, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

            if (error == LIB_ERR.NO_ERROR)
                Array.Copy(MAC, 0, Result, Result.Length - MAC.Length, MAC.Length);

            return Result;
        }

        private void BuildTranSectorBuffer(byte[] page21, ushort counter, ref byte[] buffer, ref LIB_ERR error)
        {
            int transactionIndex = counter % rptc.TRAN_SEC_CNT;

            byte[] counterValue = new byte[2];
            RPTUtils.LEU16(counter, ref counterValue, 0);

            byte[] MAC = new byte[rptc.MAC_LEN];
            byte[] plainData = new byte[page21.Length + counterValue.Length + rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - MAC.Length];

            int jj = 0;

            Array.Copy(page21, 0, plainData, jj, page21.Length); jj += page21.Length;
            Array.Copy(counterValue, 0, plainData, jj, counterValue.Length); jj += counterValue.Length;
            Array.Copy(buffer, transactionIndex * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN, plainData, jj, rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - MAC.Length);

            sam.Generate_MAC(plainData, (byte)MAC.Length, ref MAC, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

            if (error == LIB_ERR.NO_ERROR)
                Array.Copy(MAC, 0, buffer, ((transactionIndex + 1) * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN) - MAC.Length, MAC.Length);
        }

        private byte[] GeneratePassword(byte[] uid, ref LIB_ERR error)
        {
            byte[] Result = new byte[4];

            byte[] plainData = new byte[uid.Length + 1];
            Array.Copy(uid, 0, plainData, 0, uid.Length);
            plainData[plainData.Length - 1] = (byte)(~(uid[1]));

            sam.Generate_MAC(plainData, (byte)Result.Length, ref Result, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

            return Result;
        }

        private bool CheckValueCounter(ushort counter, ref LIB_ERR error)
        {
            bool Result = (counter >= rptc.MIN_VALUE_COUNTER) && (counter < ushort.MaxValue);

            if (!Result)
                error = LIB_ERR.INVALID_VALUE_COUNTER;

            return Result;
        }

        private bool CheckValueCounterForTran(ushort counter, ref LIB_ERR error)
        {
            bool Result = (counter > 0);

            if (!Result)
                error = LIB_ERR.INVALID_VALUE_COUNTER;

            return Result;
        }

        private bool ActivatedContactlessICCard(ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            bool Result = true;

            byte type = 0x00;
            byte[] atq = null;
            byte sak = 0;
            byte[] ats = null;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
            {
                Result = crt571.ActivatedContactlessICCard(Convert.ToByte('A'), Convert.ToByte('B'),
                                                                ref type, ref atq, ref uidLen, ref uid, ref sak, ref ats);
                if (!Result)
                    error = LIB_ERR.CRT571_ACTIVATE_CARD;
            }
            else
                Result = acs.CardConnect(ref uid, ref uidLen, ref error);

            return Result;
        }

        private bool DeactivatedContactlessICCard()
        {
            bool Result = false;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = crt571.DeactivateRFIDCard();
            else
            {
                acs.EndTransparentSession();

                Result = acs.CardDisconnect();
            }

            return Result;
        }

        private bool ReadSectorData(byte pageNo, ref byte[] data, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_ReadSectorData(pageNo, ref data, ref error);
            else
                Result = acs.ReadSectorData(pageNo, ref data, ref error);

            return Result;
        }

        private bool WriteSectorData(byte pageNo, byte[] data, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_WriteSectorData(pageNo, data, ref error);
            else
                Result = acs.WriteSectorData(pageNo, data, ref error);

            return Result;
        }

        private bool Access(byte[] password, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_Access(password, ref error);
            else
                Result = acs.Access(password, ref error);

            return Result;
        }

        private bool SetPassword(byte[] password, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_SetPassword(password, ref error);
            else
                Result = acs.SetPassword(password, ref error);

            return Result;
        }

        private bool GetValueCounter(ref ushort counter, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_GetValueCounter(ref counter, ref error);
            else
                Result = acs.GetValueCounter(ref counter, ref error);

            return Result;
        }

        private bool DecValueCounter(ref ushort counter, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_DecValueCounter(ref counter, ref error);
            else
                Result = acs.DecValueCounter(ref counter, ref error);

            return Result;
        }

        private bool SetValueCounter(ushort counter, ref LIB_ERR error)
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_SetValueCounter(counter, ref error);
            else
                Result = acs.SetValueCounter(counter, ref error);

            return Result;
        }

        private bool Perso(RPTPersoSector persoSector, byte[] bufTranSectors, ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            bool Result = true;

            byte[] bufPersoSector = null;
            byte[] encryptedBufPersoSector = new byte[rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN];

            byte[] encryptedBufTranSectors = new byte[rptc.TRAN_SEC_CNT * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

            byte[] page00 = new byte[rptc.PAGE_LEN];
            byte[] page01 = new byte[rptc.PAGE_LEN];
            byte[] page02 = new byte[rptc.PAGE_LEN];
            byte[] page21 = new byte[rptc.PAGE_LEN];
            byte[] page36 = new byte[rptc.PAGE_LEN];

            byte[] password = null;

            ushort valueCounter = 0;

            Result = ActivatedContactlessICCard(ref uid, ref uidLen, ref error);

            if (Result)
                Result = ReadSectorData(0, ref page00, ref error) &&
                         ReadSectorData(1, ref page01, ref error) &&
                         ReadSectorData(2, ref page02, ref error);

            if (Result)
            {
                Result = sam.ActivateOfflineKey(passwordKeyNr, passwordKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    password = GeneratePassword(uid, ref error);

                    Result = (error == LIB_ERR.NO_ERROR);
                }
            }

            if ((Result) && ((page02[1] & 6) == 6))
                Result = Access(password, ref error);

            if (Result)
            {
                Result = ReadSectorData(36, ref page36, ref error);

                if (Result)
                {
                    if ((page36[2] & 0x0C) != 0x0C)
                    {
                        valueCounter = ushort.MaxValue - 2;

                        Result = SetValueCounter(valueCounter, ref error);
                    }
                    else
                        Result = GetValueCounter(ref valueCounter, ref error);
                }
            }

            if (Result)
                Result = CheckValueCounter(valueCounter, ref error);

            if (Result)
            {
                Result = sam.ActivateOfflineKey(persoMACKeyNr, persoMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    bufPersoSector = BuildPersoSectorBuffer(persoSector, page00, page01, ref error);

                    Result = (error == LIB_ERR.NO_ERROR);

                    if (Result)
                        Array.Copy(bufPersoSector, bufPersoSector.Length - rptc.MAC_LEN, page21, 0, rptc.MAC_LEN);
                }

                if (Result)
                {
                    Result = sam.ActivateOfflineKey(tranMACKeyNr, tranMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                    {
                        for (int ii = 0; ii < rptc.TRAN_SEC_CNT; ii++)
                            BuildTranSectorBuffer(page21, Convert.ToUInt16(valueCounter + ii), ref bufTranSectors, ref error);
                    }
                }
            }

            if (Result)
            {
                Result = sam.ActivateOfflineKey(encryptKeyNr, encryptKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    Result = sam.EnchipherOffline_Data(bufPersoSector, ref encryptedBufPersoSector, ref samErr);

                    error = RPTUtils.SAMToLibErr(samErr);
                }

                if (Result)
                {
                    byte[] plainData = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];
                    byte[] encyptedData = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

                    for (int ii = 0; ii < rptc.TRAN_SEC_CNT; ii++)
                    {
                        Array.Copy(bufTranSectors, ii * plainData.Length, plainData, 0, plainData.Length);

                        Result = sam.EnchipherOffline_Data(plainData, ref encyptedData, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                        if (Result)
                            Array.Copy(encyptedData, 0, encryptedBufTranSectors, ii * encyptedData.Length, encyptedData.Length);
                        else
                            break;
                    }
                }
            }

            if (Result)
            {
                int ii = 0;

                byte[] data = new byte[rptc.PAGE_LEN];

                do
                {
                    Array.Copy(encryptedBufPersoSector, ii * data.Length, data, 0, data.Length);

                    Result = WriteSectorData(Convert.ToByte(ii + rptc.PERSO_SEC_START_PAGE), data, ref error);

                    ii++;
                }
                while ((Result) && (error == LIB_ERR.NO_ERROR) && (ii < rptc.PERSO_SEC_PAGE_CNT));

                if (Result)
                {
                    ii = 0;

                    do
                    {
                        Array.Copy(encryptedBufTranSectors, ii * data.Length, data, 0, data.Length);

                        Result = WriteSectorData(Convert.ToByte(ii + rptc.TRAN_SECS_START_PAGE), data, ref error);

                        ii++;
                    }
                    while ((Result) && (error == LIB_ERR.NO_ERROR) && (ii < rptc.TRAN_SEC_CNT * rptc.TRAN_SEC_PAGE_CNT));
                }

                if (Result)
                    Result = SetPassword(password, ref error);

                if (Result)
                {
                    page02[1] = configuration;

                    Result = WriteSectorData(rptc.CONFIG_PAGE_NO, page02, ref error);
                }

                if (Result)
                {
                    page36[2] = lock4;

                    Result = WriteSectorData(rptc.LOCK_PAGE_NO, page36, ref error);
                }
            }

            DeactivatedContactlessICCard();

            return Result;
        }

        private bool Read(ref byte[] uid, ref byte uidLen, ref byte[] page00, ref byte[] page01,
                          ref byte[] plainPagesData, ref RPTPersoSector persoSector, ref List<RPTTranSector> tranSectors,
                          ref ushort valueCounter, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            byte[] password = null;

            tranSectors.Clear();
            valueCounter = 0;

            byte[] encryptedPagesData = new byte[(rptc.PERSO_SEC_PAGE_CNT + (rptc.TRAN_SEC_PAGE_CNT * rptc.TRAN_SEC_CNT)) * rptc.PAGE_LEN];
            plainPagesData = new byte[(rptc.PERSO_SEC_PAGE_CNT + (rptc.TRAN_SEC_PAGE_CNT * rptc.TRAN_SEC_CNT)) * rptc.PAGE_LEN];

            Result = ActivatedContactlessICCard(ref uid, ref uidLen, ref error);

            if (!Result)
                error = LIB_ERR.ACTIVATE_CONTACTLESS_ICCARD_FAILES_ON_READ;

            if (Result)
                Result = ReadSectorData(0, ref page00, ref error) &&
                         ReadSectorData(1, ref page01, ref error);

            if (Result)
            {
                Result = sam.ActivateOfflineKey(passwordKeyNr, passwordKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    password = GeneratePassword(uid, ref error);

                    Result = (error == LIB_ERR.NO_ERROR);
                }

                if (Result)
                    Result = Access(password, ref error);
            }

            if (Result)
            {
                byte ii = rptc.PERSO_SEC_START_PAGE;

                do
                {
                    byte[] data = new byte[rptc.PAGE_LEN];

                    Result = ReadSectorData(ii, ref data, ref error);

                    if (Result)
                    {
                        Array.Copy(data, 0, encryptedPagesData, (ii - 16) * rptc.PAGE_LEN, data.Length);

                        ii++;
                    }
                }
                while ((Result) && (ii <= 33) && (error == LIB_ERR.NO_ERROR));

                if (Result)
                {
                    Result = GetValueCounter(ref valueCounter, ref error);

                    error = RPTUtils.SAMToLibErr(samErr);
                }
            }

            if (Result)
            {
                Result = sam.ActivateOfflineKey(encryptKeyNr, encryptKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    byte[] encryptedDataPerso = new byte[rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN];
                    byte[] dataPerso = new byte[rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN];

                    Array.Copy(encryptedPagesData, 0, encryptedDataPerso, 0, encryptedDataPerso.Length);

                    Result = sam.DechiperOffline_Data(encryptedDataPerso, ref dataPerso, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                    {
                        persoSector = new RPTPersoSector(dataPerso);

                        Array.Copy(dataPerso, 0, plainPagesData, 0, dataPerso.Length);
                    }
                }

                if (Result)
                {
                    byte[] encryptedDataTran = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];
                    byte[] dataTran = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

                    int jj = rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN;

                    do
                    {
                        Array.Copy(encryptedPagesData, jj, encryptedDataTran, 0, encryptedDataTran.Length);

                        Result = sam.DechiperOffline_Data(encryptedDataTran, ref dataTran, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                        if (Result)
                        {
                            RPTTranSector tranSector = new RPTTranSector(dataTran);

                            tranSectors.Add(tranSector);

                            Array.Copy(dataTran, 0, plainPagesData, jj, dataTran.Length);
                        }

                        jj += encryptedDataTran.Length;
                    }
                    while ((Result) && (jj < encryptedPagesData.Length));
                }
            }

            return Result;
        }

        private bool Analiser(ref byte[] uid, ref byte uidLen,
                              ref RPTPersoSector persoSector, ref List<RPTTranSector> tranSectors, ref ushort valueCounter,
                              ref RPTAnalise analise, ref LIB_ERR error)
        {
            byte[] plainPagesData = null;

            byte[] page00 = new byte[rptc.PAGE_LEN];
            byte[] page01 = new byte[rptc.PAGE_LEN];

            bool Result = Read(ref uid, ref uidLen, ref page00, ref page01,
                               ref plainPagesData, ref persoSector, ref tranSectors, ref valueCounter,
                               ref error);

            if (Result)
            {
                byte[] MAC = new byte[rptc.MAC_LEN];

                Result = sam.ActivateOfflineKey(persoMACKeyNr, persoMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    // Fill Perso Sector MAC caculation block and compere

                    byte[] plainData = new byte[page00.Length + page01.Length + (rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN) - rptc.MAC_LEN];

                    int jj = 0;

                    Array.Copy(page00, 0, plainData, jj, page00.Length); jj += page00.Length;
                    Array.Copy(page01, 0, plainData, jj, page01.Length); jj += page01.Length;
                    Array.Copy(plainPagesData, 0, plainData, jj, rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN - rptc.MAC_LEN);

                    Result = sam.Generate_MAC(plainData, (byte)MAC.Length, ref MAC, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                        analise.persoMACValid = persoSector.MAC.SequenceEqual(MAC);
                }

                if (Result)
                {
                    Result = sam.ActivateOfflineKey(tranMACKeyNr, tranMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                    {
                        byte[] plainData = new byte[persoSector.MAC.Length + sizeof(ushort) + rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - MAC.Length];

                        int ii = 0;

                        // Fill Transaction Sectors MAC caculation block and compare

                        do
                        {
                            int jj = 0;

                            byte[] counterValue = BitConverter.GetBytes(Convert.ToUInt16(valueCounter + ii));

                            int transactionIndex = (valueCounter + ii) % rptc.TRAN_SEC_CNT;

                            Array.Copy(persoSector.MAC, 0, plainData, jj, persoSector.MAC.Length); jj += persoSector.MAC.Length;
                            Array.Copy(counterValue, 0, plainData, jj, counterValue.Length); jj += counterValue.Length;
                            Array.Copy(plainPagesData, (rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN) + (transactionIndex * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN), plainData, jj, rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - MAC.Length);

                            Result = sam.Generate_MAC(plainData, (byte)MAC.Length, ref MAC, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                            if (Result)
                            {
                                analise.tranOrder[transactionIndex] = (TRAN_ORDER)ii;
                                analise.tranMACValid[ii] = tranSectors[transactionIndex].MAC.SequenceEqual(MAC);
                            }

                            ii++;
                        }
                        while ((Result) && (ii < rptc.TRAN_SEC_CNT));
                    }
                }
            }

            if ((Result) && RPTUtils.IsRPTValid(analise))
                analise.cancelled = RPTUtils.IsThereCancelTran(tranSectors);

            return Result;
        }

        private bool InsTran(RPTTranSector tranSector, ref LIB_ERR error)
        {
            bool Result = true;

            byte[] uid = new byte[7];
            byte uidLen = 0;

            RPTPersoSector persoSector = null;
            List<RPTTranSector> tranSectors = new List<RPTTranSector>();
            ushort valueCounter = 0;

            byte[] plainPagesData = null;

            byte[] page00 = new byte[rptc.PAGE_LEN];
            byte[] page01 = new byte[rptc.PAGE_LEN];

            Result = Read(ref uid, ref uidLen, ref page00, ref page01,
                          ref plainPagesData, ref persoSector, ref tranSectors, ref valueCounter,
                          ref error);

            if (Result)
                Result = CheckValueCounterForTran(valueCounter, ref error);

            if (Result)
            {
                valueCounter--;

                Result = sam.ActivateOfflineKey(tranMACKeyNr, tranMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    // Fill Transaction Sector block and caculate MAC

                    byte[] tranData = tranSector.ToBuffer();

                    byte[] counterValue = BitConverter.GetBytes(valueCounter);

                    int jj = 0;

                    byte[] plainData = new byte[persoSector.MAC.Length + sizeof(ushort) + rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - rptc.MAC_LEN];

                    Array.Copy(persoSector.MAC, 0, plainData, jj, persoSector.MAC.Length); jj += persoSector.MAC.Length;
                    Array.Copy(counterValue, 0, plainData, jj, counterValue.Length); jj += counterValue.Length;
                    Array.Copy(tranData, 0, plainData, jj, rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - rptc.MAC_LEN);

                    Result = sam.Generate_MAC(plainData, (byte)tranSector.MAC.Length, ref tranSector.MAC, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                    {
                        Result = sam.ActivateOfflineKey(encryptKeyNr, encryptKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                        if (Result)
                        {
                            byte[] encyptedData = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

                            Result = sam.EnchipherOffline_Data(tranSector.ToBuffer(), ref encyptedData, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                            if (Result)
                            {
                                // Write Transaction Sector

                                byte[] data = new byte[rptc.PAGE_LEN];

                                int ii = 0;

                                byte pageNo = RPTUtils.GetTranStartPage(Convert.ToByte(valueCounter % rptc.TRAN_SEC_CNT));

                                do
                                {
                                    Array.Copy(encyptedData, ii * rptc.PAGE_LEN, data, 0, data.Length);

                                    Result = WriteSectorData(pageNo, data, ref error);

                                    pageNo++;

                                    ii++;
                                }
                                while ((Result) && (ii < rptc.TRAN_SEC_PAGE_CNT));
                            }
                        }
                    }
                }
            }

            // Decrease Value Counter

            if (Result)
            {
                ushort value = 0;

                Result = DecValueCounter(ref value, ref error);
            }

            DeactivatedContactlessICCard();

            return Result;
        }

        private bool Delete(byte[] uid, byte tranIndex, ushort valueCounter, RPTPersoSector persoSector, ref LIB_ERR error)
        {
            bool Result = sam.ActivateOfflineKey(tranMACKeyNr, tranMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

            if (Result)
            {
                // Empty Transaction Sector block and caculate new MAC

                byte[] plainData = new byte[persoSector.MAC.Length + sizeof(ushort) + rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - rptc.MAC_LEN];

                int jj = 0;

                byte[] counterValue = BitConverter.GetBytes(valueCounter);

                RPTTranSector tranSector = new RPTTranSector();

                byte[] tranData = tranSector.ToBuffer();

                Array.Copy(persoSector.MAC, 0, plainData, jj, persoSector.MAC.Length); jj += persoSector.MAC.Length;
                Array.Copy(counterValue, 0, plainData, jj, counterValue.Length); jj += counterValue.Length;
                Array.Copy(tranData, 0, plainData, jj, rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN - rptc.MAC_LEN);

                Result = sam.Generate_MAC(plainData, (byte)tranSector.MAC.Length, ref tranSector.MAC, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    byte[] encyptedData = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

                    Result = sam.EnchipherOffline_Data(tranSector.ToBuffer(), ref encyptedData, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                    {
                        // Write Transaction Sector

                        byte[] data = new byte[rptc.PAGE_LEN];

                        int ii = 0;

                        byte pageNo = RPTUtils.GetTranStartPage(Convert.ToByte(valueCounter % rptc.TRAN_SEC_CNT));

                        do
                        {
                            Array.Copy(encyptedData, ii * rptc.PAGE_LEN, data, 0, data.Length);

                            Result = WriteSectorData(pageNo, data, ref error);

                            pageNo++;

                            ii++;
                        }
                        while ((Result) && (ii < rptc.TRAN_SEC_PAGE_CNT));
                    }
                }
            }

            return Result;
        }

        #endregion

        #region PUBLIC METHODS

        public bool Init(DEVICE_TYPE deviceType, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            sam.ReaderTransieve += Sam_ReaderTransieve;

            this.deviceType = deviceType;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = CRT571_Init(ref error);
            else
            {
                Result = acs.Init(ref error);

                if (Result)
                {
                    Result = sam.Unlock(_samUnlockKey, samUnlockKeyNr, samUnlockKeyVer, ref samErr);

                    error = RPTUtils.SAMToLibErr(samErr);
                }
            }

            if (Result)
            {
                KeyEntryData data = new KeyEntryData();

                Result = sam.GetKeyEntry(17, ref data, ref samErr);

                if (Result)
                    tranMACKeyNr = ((data.Va != 0) && (data.Vb != 0) && (data.Vc != 0)) ? (byte)17 : (byte)16;
            }

            return Result;
        }

        public void Done()
        {
            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                CRT571_Done();
            else
                acs.Done();
        }

        public bool GetDeviceVersionInfo(ref string info)
        {
            bool Result = true;

            info = "";

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = crt571.ReadFirmwareVersion(ref info);
            else
            {
                Result = acs.ReadVersionInformation();

                if (Result)
                    info = acs.Version;
            }

            return Result;
        }

        public bool GetSAMSerialNo(ref byte[] uid, ref LIB_ERR error)
        {
            bool Result = true;

            error = LIB_ERR.NO_ERROR;

            byte[] capdu = new byte[5] { 0x80, 0x60, 0x00, 0x00, 0x00 };

            byte[] rapdu = null;

            Result = Sam_ReaderTransieve(this, capdu, ref rapdu);

            Result = Result &&
                     (rapdu != null) && (rapdu.Length >= 33);

            if (Result)
                Array.Copy(rapdu, 14, uid, 0, uid.Length);
            else
                error = LIB_ERR.SAM_GET_VERSION;

            return Result;
        }

        public bool GetSAMKeyKCV(byte KeyNo, byte KeyVer, ref byte[] KCV, ref LIB_ERR error)
        {
            bool Result = sam.ActivateOfflineKey(KeyNo, KeyVer, null, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

            if (Result)
            {
                byte[] input = new byte[8];
                byte[] output = new byte[8];

                Result = sam.EnchipherOffline_Data(input, ref output, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                    Array.Copy(output, 0, KCV, 0, KCV.Length);
            }

            return Result;
        }

        public bool GetStackerStatus(ref DISP_STAT status)
        {
            status = DISP_STAT.STACKER_UNKNOWN;

            bool Result = crt571.StatusRequest();

            if (Result)
            {
                if (crt571.St[1] == crt571.STAT_STACKER_NOCARD)
                    status = DISP_STAT.STACKER_NOCARD;
                else if (crt571.St[1] == crt571.STAT_STACKER_LACK)
                    status = DISP_STAT.STACKER_LACK;
                else if (crt571.St[1] == crt571.STAT_STACKER_FULL)
                    status = DISP_STAT.STACKER_FULL;
                else
                    status = DISP_STAT.STACKER_UNKNOWN;
            }

            return Result;
        }

        public bool GetDispenserFullStatus(ref ST0 st0, ref ST1 st1, ref ST2 st2, ref SENSOR[] sensors, ref E01 e01)
        {
            st0 = ST0.Unknown;
            st1 = ST1.Unknown;
            st2 = ST2.Unknown;

            for (int ii = 0; ii < sensors.Length; ii++)
                sensors[ii] = SENSOR.Unkown;

            e01 = E01.None;

            bool Result = crt571.StatusRequest();

            if (Result)
            {
                st0 = (ST0)crt571.St[0];
                st1 = (ST1)crt571.St[1];
                st2 = (ST2)crt571.St[2];

                for (int ii = 0; ii < sensors.Length; ii++)
                    sensors[ii] = (SENSOR)crt571.Sensor[ii];
            }
            else if ((crt571.E[0] != 0x00) && (crt571.E[1] != 0x00))
            {
                string strE = ASCIIEncoding.ASCII.GetString(crt571.E);

                e01 = (E01)Convert.ToByte(strE, 16);
            }

            return Result;
        }

        [Obsolete]
        public bool ResetDispenser(ref LIB_ERR error)
        {
            bool Result = true;

            Done();

            System.Threading.Thread.Sleep(1000);

            Result = Init(DEVICE_TYPE.CRT571_DISPENSER, ref error);

            return Result;
        }

        public bool IsThereAnyCardInStacker()
        {
            return deviceType == DEVICE_TYPE.CRT571_DISPENSER ? crt571.IsThereAnyCardInStacker() : true;
        }

        public bool IsCardInRFCardOperationPosition()
        {
            bool Result = false;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
                Result = crt571.IsCardInRFCardOperationPosition();
            else
                Result = acs.IsCardOnRFCardOperationPosition();

            return Result;
        }

        public bool MoveCardToRFCardPosition()
        {
            bool Result = true;

            if (deviceType == DEVICE_TYPE.CRT571_DISPENSER)
            {
                Result = crt571.IsCardInRFCardOperationPosition();

                if (!Result)
                    Result = crt571.MoveCardToRFCardPosition();
            }

            return Result;
        }

        public bool MoveCardtoTheGate()
        {
            return deviceType == DEVICE_TYPE.CRT571_DISPENSER ? crt571.MoveCardToTheGate() : true;
        }

        public bool MoveCardToHoldingCardPosition()
        {
            return deviceType == DEVICE_TYPE.CRT571_DISPENSER ? crt571.MoveCardToHoldingCardPosition() : true;
        }

        public bool CaptureCardToErrorCardBin()
        {
            return deviceType == DEVICE_TYPE.CRT571_DISPENSER ? crt571.CaptureCardToErrorCardBin() : true;
        }

        public bool EnableGateCardEntry()
        {
            return deviceType == DEVICE_TYPE.CRT571_DISPENSER ? crt571.EnableGateCardEntry() : true;
        }

        public bool DisableGateCardEntry()
        {
            return deviceType == DEVICE_TYPE.CRT571_DISPENSER ? crt571.DisableGateCardEntry() : true;
        }

        public bool Read(ref byte[] uid, ref byte uidLen,
                         ref RPTPersoSector persoSector, ref List<RPTTranSector> tranSectors, ref ushort valueCounter,
                         ref LIB_ERR error)
        {
            bool Result = false;

            byte[] plainPagesData = null;

            byte[] page00 = new byte[rptc.PAGE_LEN];
            byte[] page01 = new byte[rptc.PAGE_LEN];

            Result = Read(ref uid, ref uidLen, ref page00, ref page01,
                          ref plainPagesData, ref persoSector, ref tranSectors, ref valueCounter,
                          ref error);

            DeactivatedContactlessICCard();

            return Result;
        }

        public bool Analise(ref byte[] uid, ref byte uidLen,
                            ref RPTPersoSector persoSector, ref List<RPTTranSector> tranSectors, ref ushort valueCounter,
                            ref RPTAnalise analise, ref LIB_ERR error)
        {
            bool Result = false;

            Result = Analiser(ref uid, ref uidLen, ref persoSector, ref tranSectors, ref valueCounter, ref analise, ref error);

            DeactivatedContactlessICCard();

            return Result;
        }

        public bool Select(ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            bool Result = ActivatedContactlessICCard(ref uid, ref uidLen, ref error);

            DeactivatedContactlessICCard();

            return Result;
        }

        public bool Cancel(ref LIB_ERR error)
        {
            RPTTranSector tranSector = new RPTTranSector(DateTime.Now);
            tranSector.operationType = (byte)OPR_TYPE.Cancellation;

            return InsTran(tranSector, ref error);
        }

        public bool Refund(ref LIB_ERR error)
        {
            RPTTranSector tranSector = new RPTTranSector(DateTime.Now);
            tranSector.operationType = (byte)OPR_TYPE.Refund;

            return InsTran(tranSector, ref error);
        }

        public bool Insert(RPTTranSector tranSector, ref LIB_ERR error)
        {
            return InsTran(tranSector, ref error);
        }

        public bool OverStay(DateTime dt, ref LIB_ERR error)
        {
            byte[] uid = new byte[7];
            byte uidLen = 0;
            RPTPersoSector persoSector = null;
            List<RPTTranSector> tranSectors = new List<RPTTranSector>();
            ushort valueCounter = 0;

            RPTAnalise analise = new RPTAnalise();

            bool Result = Analiser(ref uid, ref uidLen, ref persoSector, ref tranSectors, ref valueCounter, ref analise, ref error);

            DeactivatedContactlessICCard();

            if (Result)
            {
                byte lastTranIndex = RPTUtils.GetLastTranIndex(analise);

                RPTTranSector tranSector = tranSectors[lastTranIndex];

                tranSector.dt = dt;
                tranSector.operationType = (byte)OPR_TYPE.OverStay;
                tranSector.usageCount = 1;

                Result = InsTran(tranSector, ref error);
            }

            return Result;
        }

        public bool EntryMismatch(ushort stationNo, ref LIB_ERR error)
        {
            RPTTranSector tranSector = new RPTTranSector();

            tranSector.dt = DateTime.Now;
            tranSector.operationType = (byte)OPR_TYPE.EntryMismatch;
            tranSector.stationNo = stationNo;

            return InsTran(tranSector, ref error);
        }

        public bool ExitMismatch(RPTTranSector tranSector, ref LIB_ERR error)
        {
            return InsTran(tranSector, ref error);
        }

        public bool OverTravelPerso(ushort stationOut, ref LIB_ERR error)
        {
            byte[] uid = new byte[7];
            byte uidLen = 0;
            RPTPersoSector persoSector = null;
            List<RPTTranSector> tranSectors = new List<RPTTranSector>();
            ushort valueCounter = 0;

            byte[] bufPersoSector = null;
            byte[] encryptedBufPersoSector = new byte[rptc.PERSO_SEC_PAGE_CNT * rptc.PAGE_LEN];

            byte[] bufTranSectors = null;
            byte[] encryptedBufTranSectors = new byte[rptc.TRAN_SEC_CNT * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

            byte[] plainPagesData = null;

            byte[] page00 = new byte[rptc.PAGE_LEN];
            byte[] page01 = new byte[rptc.PAGE_LEN];

            bool Result = Read(ref uid, ref uidLen, ref page00, ref page01,
                               ref plainPagesData, ref persoSector, ref tranSectors, ref valueCounter,
                               ref error);

            // Modify Station-Out

            if (Result)
            {
                if (persoSector.ticketData is TicketSJT)
                    ((TicketSJT)(persoSector.ticketData)).stationOut = stationOut;
                else if (persoSector.ticketData is TicketGroup)
                    ((TicketGroup)(persoSector.ticketData)).stationOut = stationOut;
                else
                {
                    Result = false;

                    error = LIB_ERR.INVALID_TICKET_TYPE;
                }
            }

            // Calculate new MAC values

            if (Result)
            {
                byte[] page21 = new byte[rptc.PAGE_LEN];

                Result = sam.ActivateOfflineKey(persoMACKeyNr, persoMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    bufPersoSector = BuildPersoSectorBuffer(persoSector, page00, page01, ref error);

                    Result = (error == LIB_ERR.NO_ERROR);

                    if (Result)
                        Array.Copy(bufPersoSector, bufPersoSector.Length - rptc.MAC_LEN, page21, 0, rptc.MAC_LEN);
                }

                bufTranSectors = RPTUtils.GetTransBuffer(tranSectors);

                if (Result)
                {
                    Result = sam.ActivateOfflineKey(tranMACKeyNr, tranMACKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                    if (Result)
                    {
                        for (int ii = 0; ii < rptc.TRAN_SEC_CNT; ii++)
                            BuildTranSectorBuffer(page21, Convert.ToUInt16(valueCounter + ii), ref bufTranSectors, ref error);
                    }
                }
            }

            // Encode Ticket Data

            if (Result)
            {
                Result = sam.ActivateOfflineKey(encryptKeyNr, encryptKeyVer, uid, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                if (Result)
                {
                    Result = sam.EnchipherOffline_Data(bufPersoSector, ref encryptedBufPersoSector, ref samErr);

                    error = RPTUtils.SAMToLibErr(samErr);
                }

                if (Result)
                {
                    byte[] plainData = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];
                    byte[] encyptedData = new byte[rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

                    for (int ii = 0; ii < rptc.TRAN_SEC_CNT; ii++)
                    {
                        Array.Copy(bufTranSectors, ii * plainData.Length, plainData, 0, plainData.Length);

                        Result = sam.EnchipherOffline_Data(plainData, ref encyptedData, ref samErr); error = RPTUtils.SAMToLibErr(samErr);

                        if (Result)
                            Array.Copy(encyptedData, 0, encryptedBufTranSectors, ii * encyptedData.Length, encyptedData.Length);
                        else
                            break;
                    }
                }
            }

            // Write all new encoded data

            if (Result)
            {
                int ii = 0;

                byte[] data = new byte[rptc.PAGE_LEN];

                do
                {
                    Array.Copy(encryptedBufPersoSector, ii * data.Length, data, 0, data.Length);

                    Result = WriteSectorData(Convert.ToByte(ii + rptc.PERSO_SEC_START_PAGE), data, ref error);

                    ii++;
                }
                while ((Result) && (error == LIB_ERR.NO_ERROR) && (ii < rptc.PERSO_SEC_PAGE_CNT));

                if (Result)
                {
                    ii = 0;

                    do
                    {
                        Array.Copy(encryptedBufTranSectors, ii * data.Length, data, 0, data.Length);

                        Result = WriteSectorData(Convert.ToByte(ii + rptc.TRAN_SECS_START_PAGE), data, ref error);

                        ii++;
                    }
                    while ((Result) && (error == LIB_ERR.NO_ERROR) && (ii < rptc.TRAN_SEC_CNT * rptc.TRAN_SEC_PAGE_CNT));
                }
            }

            DeactivatedContactlessICCard();

            return Result;
        }

        public bool OverTravelTran(ushort stationOut, ref LIB_ERR error)
        {
            RPTTranSector tranSector = new RPTTranSector();

            tranSector.dt = DateTime.Now;
            tranSector.stationNo = stationOut;
            tranSector.direction = (byte)TRAN_DIR.Exit;
            tranSector.operationType = (byte)OPR_TYPE.OverStay;

            return InsTran(tranSector, ref error);
        }

        public bool Replacement(RPTPersoSector persoSector, List<RPTTranSector> tranSectors, ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            bool Result = true;

            byte[] page02 = new byte[rptc.PAGE_LEN];
            byte[] page36 = new byte[rptc.PAGE_LEN];

            ushort valueCounter = 0;

            byte[] password = null;

            Result = ActivatedContactlessICCard(ref uid, ref uidLen, ref error) &&
                     ReadSectorData(2, ref page02, ref error);


            if (Result)
            {
                Result = sam.ActivateOfflineKey(passwordKeyNr, passwordKeyVer, uid, ref samErr);

                error = RPTUtils.SAMToLibErr(samErr);
            }

            if (Result)
            {
                password = GeneratePassword(uid, ref error);

                Result = (error == LIB_ERR.NO_ERROR);
            }

            if ((Result) && ((page02[1] & 6) == 6))
                Result = Access(password, ref error);

            if (Result)
                Result = ReadSectorData(36, ref page36, ref error);

            if (Result)
            {
                if ((page36[2] & 12) != 12)
                    valueCounter = ushort.MaxValue - 2;
                else
                    Result = GetValueCounter(ref valueCounter, ref error);
            }

            // Order all transaction sectors according to new RPT value counter

            if (Result)
            {
                // Requirement => tranSectors[0] = LAST, tranSectors[1] = PREV, tranSectors[2] = OLDEST

                byte[] bufTranSectors = new byte[rptc.TRAN_SEC_CNT * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

                for (byte ii = 0; ii < tranSectors.Count; ii++)
                {
                    tranSectors[ii].operationType = (byte)OPR_TYPE.Replacement;

                    byte transactionIndex = Convert.ToByte((valueCounter + ii) % rptc.TRAN_SEC_CNT);

                    Array.Copy(tranSectors[ii].ToBuffer(), 0, bufTranSectors, transactionIndex * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN, rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN);
                }

                DeactivatedContactlessICCard();

                Result = Perso(persoSector, bufTranSectors, ref uid, ref uidLen, ref error);
            }
            else
                DeactivatedContactlessICCard();

            return Result;
        }

        public bool FreeExited(DateTime dt, ushort stationNo, ref LIB_ERR error)
        {
            RPTTranSector tranSector = new RPTTranSector(dt);

            tranSector.stationNo = stationNo;
            tranSector.direction = (byte)TRAN_DIR.Exit;
            tranSector.operationType = (byte)OPR_TYPE.FreeExited;
            tranSector.usageCount = 1;

            return InsTran(tranSector, ref error);
        }

        public bool TopupEmployee(ref LIB_ERR error, ushort usageCount = 0)
        {
            RPTTranSector tranSector = new RPTTranSector();

            tranSector.dt = DateTime.Now;
            tranSector.operationType = (byte)OPR_TYPE.TopupEmployee;
            tranSector.usageCount = usageCount;

            return InsTran(tranSector, ref error);
        }

        public bool Do(RPTPersoSector persoSector, ref byte[] uid, ref byte uidLen, ref LIB_ERR error)
        {
            byte[] bufTranSectors = new byte[rptc.TRAN_SEC_CNT * rptc.TRAN_SEC_PAGE_CNT * rptc.PAGE_LEN];

            return Perso(persoSector, bufTranSectors, ref uid, ref uidLen, ref error);
        }

        #endregion
    }
}
