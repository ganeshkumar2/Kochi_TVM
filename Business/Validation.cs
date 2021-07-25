using KioskFramework.OccSrv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Business
{
    public static class Validation
    {
        public static bool IsValidRP(LocalDbPackage rp)
        {
            var Result = ((rp != null) &&
                          (rp.Data != null) &&
                          (rp.Data.Tables.Count > 0) &&
                          (rp.Data.Tables[0].Rows != null) &&
                          (rp.Data.Tables[0].Rows.Count > 0));

            return Result;
        }
        public static bool IsValidAFCRP(KioskFramework.PayPointSrv.ReturnPackage rp)
        {
            var Result = ((rp != null) &&
                          (rp.Data != null) &&
                          (rp.Data.Tables.Count > 0) &&
                          (rp.Data.Tables[0].Rows != null) &&
                          (rp.Data.Tables[0].Rows.Count > 0));

            return Result;
        }
        public static bool IsValidOCCRP(ReturnPackage rp)
        {
            var Result = ((rp != null) &&
                          (rp.Data != null) &&
                          (rp.Data.Tables.Count > 0) &&
                          (rp.Data.Tables[0].Rows != null) &&
                          (rp.Data.Tables[0].Rows.Count > 0));

            return Result;
        }

        public static string PassValidityVOL(string operatorChar, string operatorCode, string vehicleChar, string vehicleCode,
      string lineChar, string lineNo, int fromStage, int fromStationId, int toStage, int toStationId)
        {
            var passValidityVOL = "";

            #region Operator Char
            var ba = Encoding.ASCII.GetBytes(operatorChar);
            var hexStringOperator = BitConverter.ToString(ba);
            hexStringOperator = hexStringOperator.Replace("-", "");
            #endregion

            #region Operator Code
            ba = Encoding.ASCII.GetBytes(operatorCode);
            var hexStringOperatorCode = BitConverter.ToString(ba);
            hexStringOperatorCode = hexStringOperatorCode.Replace("-", "");
            var operatorCodeLength = ba.Length.ToString("X");
            if (operatorCodeLength.Length == 1)
            {
                operatorCodeLength = "0" + operatorCodeLength;
            }

            hexStringOperatorCode = operatorCodeLength + hexStringOperatorCode;
            #endregion

            #region Vehicle Char
            ba = Encoding.ASCII.GetBytes(vehicleChar);
            var hexStringVehicleChar = BitConverter.ToString(ba);
            hexStringVehicleChar = hexStringVehicleChar.Replace("-", "");
            #endregion

            #region Vehicle Code
            ba = Encoding.ASCII.GetBytes(vehicleCode);
            var hexStringVehicleCode = BitConverter.ToString(ba);
            hexStringVehicleCode = hexStringVehicleCode.Replace("-", "");

            var vehicleCodeLength = ba.Length.ToString("X");
            if (vehicleCodeLength.Length == 1)
            {
                vehicleCodeLength = "0" + vehicleCodeLength;
            }

            hexStringVehicleCode = vehicleCodeLength + hexStringVehicleCode;
            #endregion

            #region Line Char
            ba = Encoding.ASCII.GetBytes(lineChar);
            var hexStringlineChar = BitConverter.ToString(ba);
            hexStringlineChar = hexStringlineChar.Replace("-", "");
            #endregion

            #region From Stage
            var hexStringFromStage = fromStage.ToString("X");
            if (hexStringFromStage.Length == 1)
            {
                hexStringFromStage = "0" + hexStringFromStage;
            }
            #endregion

            #region From Station Id
            var hexStringFrom = fromStationId.ToString("X");
            if (hexStringFrom.Length == 1)
            {
                hexStringFrom = "0" + hexStringFrom;
            }
            #endregion

            #region To Stage
            var hexStringToStage = toStage.ToString("X");
            if (hexStringToStage.Length == 1)
            {
                hexStringToStage = "0" + hexStringToStage;
            }
            #endregion

            #region To Station Id
            var hexStringTo = toStationId.ToString("X");
            if (hexStringTo.Length == 1)
            {
                hexStringTo = "0" + hexStringTo;
            }
            #endregion

            #region Line Process
            var LineTotal = "";
            LineTotal = lineNo + hexStringFromStage + hexStringFrom + hexStringToStage + hexStringTo;

            var hexStringLineLength = LineTotal.Length / 2;
            var hexStringToLineCount = hexStringLineLength.ToString("X");
            if (hexStringToLineCount.Length == 1)
            {
                hexStringToLineCount = "0" + hexStringToLineCount;
            }
            passValidityVOL = hexStringOperator + hexStringOperatorCode + hexStringVehicleChar + hexStringVehicleCode +
                              hexStringlineChar + hexStringToLineCount + LineTotal;
            #endregion

            return passValidityVOL + "0000000000000000";
        }

    }
    public class LocalDbPackage
    {
        public int Result { get; set; }

        public string Description { get; set; }

        public DataSet Data { get; set; }

    }
}
