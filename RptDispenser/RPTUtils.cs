using SAMOprLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.RptDispenser
{
    public static class RPTUtils
    {
        public static void LEU16(UInt16 value, ref byte[] array, int startIndex)
        {
            byte[] valAry = BitConverter.GetBytes(value);

            Array.Copy(valAry, 0, array, startIndex, 2);
        }

        public static UInt32 DateTimeToUnix(DateTime dt)
        {
            var duration = dt - new DateTime(2010, 1, 1, 0, 0, 0);

            return (uint)duration.TotalSeconds;
        }

        public static byte GetTranStartPage(int index)
        {
            return Convert.ToByte(rptc.TRAN_SECS_START_PAGE + (index * rptc.TRAN_SEC_PAGE_CNT));
        }

        public static bool IsThereCancelTran(List<RPTTranSector> tranSectors)
        {
            bool Result = false;

            for (int ii = 0; ii < tranSectors.Count; ii++)
            {
                Result = (tranSectors[ii].stationNo == 0xFFFF) &&
                         (tranSectors[ii].gateNo == 0xFFFF) &&
                         (tranSectors[ii].direction == (byte)TRAN_DIR.Unknown) &&
                         (tranSectors[ii].operationType == (byte)OPR_TYPE.Cancellation) &&
                         (tranSectors[ii].usageCount == 0xFFFF);

                if (Result)
                    break;
            }

            return Result;
        }

        public static bool IsRPTValid(RPTAnalise analise)
        {
            bool Result = analise.persoMACValid;

            if (Result)
            {
                for (int ii = 0; ii < analise.tranMACValid.Length; ii++)
                {
                    Result = analise.tranMACValid[ii];

                    if (!Result)
                        break;
                }
            }

            return Result;
        }

        public static byte GetTranIndex(RPTAnalise analise, TRAN_ORDER tranOrder)
        {
            byte Result = 0;

            for (byte ii = 0; ii < analise.tranOrder.Length; ii++)
            {
                if (analise.tranOrder[ii] == tranOrder)
                {
                    Result = ii;

                    break;
                }
            }

            return Result;
        }

        public static byte GetLastTranIndex(RPTAnalise analise)
        {
            return GetTranIndex(analise, TRAN_ORDER.LAST);
        }

        public static byte GetPreviousTranIndex(RPTAnalise analise)
        {
            return GetTranIndex(analise, TRAN_ORDER.PREVIOUS);
        }

        public static byte GetOldestTranIndex(RPTAnalise analise)
        {
            return GetTranIndex(analise, TRAN_ORDER.OLDEST);
        }

        public static byte[] GetTransBuffer(List<RPTTranSector> tranSectors)
        {
            List<byte> Result = new List<byte>();

            for (int ii = 0; ii < tranSectors.Count; ii++)
                Result.AddRange(tranSectors[ii].ToBuffer());

            return Result.ToArray();
        }

        public static LIB_ERR SAMToLibErr(SAM_OPR_ERR err)
        {
            LIB_ERR Result = LIB_ERR.UNKNOWN;

            switch (err)
            {
                case SAM_OPR_ERR.NO_ERROR:
                    Result = LIB_ERR.NO_ERROR;
                    break;
                case SAM_OPR_ERR.UNLOCK_PART_1:
                    Result = LIB_ERR.SAM_UNLOCK_PART_1;
                    break;
                case SAM_OPR_ERR.UNLOCK_PART_2:
                    Result = LIB_ERR.SAM_UNLOCK_PART_2;
                    break;
                case SAM_OPR_ERR.UNLOCK_PART_3:
                    Result = LIB_ERR.SAM_UNLOCK_PART_3;
                    break;
                case SAM_OPR_ERR.UNLOCK_MAC_VERIFY:
                    Result = LIB_ERR.SAM_UNLOCK_MAC_VERIFY;
                    break;
                case SAM_OPR_ERR.ACTIVATE_OFFLINE_KEY:
                    Result = LIB_ERR.SAM_ACTIVATE_OFFLINE_KEY;
                    break;
                case SAM_OPR_ERR.GENERATE_MAC:
                    Result = LIB_ERR.SAM_GENERATE_MAC;
                    break;
                case SAM_OPR_ERR.ENCHIPHER_OFFLINE_DATA:
                    Result = LIB_ERR.SAM_ENCHIPHER_OFFLINE_DATA;
                    break;
                case SAM_OPR_ERR.DECHIPER_OFFLINE_DATA:
                    Result = LIB_ERR.SAM_DECHIPER_OFFLINE_DATA;
                    break;
                default:
                    Result = LIB_ERR.UNKNOWN;
                    break;
            }

            return Result;
        }
    }
}
