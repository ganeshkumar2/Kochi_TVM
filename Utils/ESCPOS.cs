using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Utils
{
    public class ESCPOS
    {
        public string ParseROMVersionId(byte[] data)
        {
            return ASCIIEncoding.ASCII.GetString(data);
        }

        public string ParsePrinterModelId(byte[] data)
        {
            string Result = "";

            if ((data[0] == 0x02) && (data[1] == 0x03))
                Result = "KPM150H/B202H";
            else if ((data[0] == 0x02) && (data[1] == 0x04))
                Result = "VKP11H"; ;

            return Result;
        }

        #region MISCELLANEOUS COMMANDS

        public byte[] ChangePrinterEmulationToSVELTA()
        {
            // ASCII: FS<SVEL>

            return new byte[] { 0x1C, 0x3C, 0x53, 0x56, 0x45, 0x4C, 0x3E };
        }

        public byte[] TransmitPrinterId(byte value)
        {
            // ASCII: GS I n

            return new byte[] { 0x1D, 0x49, value };
        }

        public byte[] GetPrinterModelId()
        {
            return TransmitPrinterId(255);
        }

        public byte[] GetROMVersionId()
        {
            return TransmitPrinterId(51);
        }

        public byte[] TotalCut()
        {
            return new byte[] { 0x1B, 0x69 };
        }

        public byte[] Justification()
        {
            return new byte[] { 0x1B, 0x61, 49 };
        }

        #endregion
    }
}
