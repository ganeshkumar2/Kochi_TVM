using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Business
{
    public class QROperations
    {
        private const byte TICKET_VERSION = 0x01;

        public static void LEU16(ushort value, ref byte[] array, int startIndex)
        {
            var valAry = BitConverter.GetBytes(value);

            Array.Copy(valAry, 0, array, startIndex, 2);
        }

        public static void LEU32(uint value, ref byte[] array, int startIndex)
        {
            var valAry = BitConverter.GetBytes(value);

            Array.Copy(valAry, 0, array, startIndex, 4);
        }

        public static void LEU64(ulong value, ref byte[] array, int startIndex)
        {
            var valAry = BitConverter.GetBytes(value);

            Array.Copy(valAry, 0, array, startIndex, 8);
        }

        private static uint DateTimeToUnix(DateTime d)
        {
            var duration = d - new DateTime(2010, 1, 1, 0, 0, 0);

            return (uint)duration.TotalSeconds;
        }

        private static void Crc16(byte data, ref ushort crc)
        {
            for (byte i = 0; i < 8; i++)
            {
                if (((data & 0x01) ^ (crc & 0x0001)) != 0)
                {
                    crc = (ushort)(crc >> 1);
                    crc = (ushort)(crc ^ 0xA001);
                }
                else
                    crc = (ushort)(crc >> 1);

                data = (byte)(data >> 1);
            }
        }

        static ushort CalcCheckSum(byte[] array, int startIndex, int len)
        {
            ushort Result = 0xC0DE;

            for (var ii = startIndex; ii < len; ii++)
                Crc16(array[ii], ref Result);

            return Result;
        }

        public static string GenerateQR(DateTime ticketDT, int ticketTypeId, int from, int to, short peopleCount, DateTime activeFrom, DateTime activeTo, int terminalId, int alias, decimal price, ref string qrString)
        {
            var ticketCode = new byte[41];
            var ticketId = new byte[16];
            var ticketData = new byte[24];

            /* TICKET ID */
            var guid = Guid.NewGuid();
            var ticketGuid = guid.ToByteArray();
            var ticketIdText = BitConverter.ToString(ticketGuid, 0);
            qrString = ticketIdText.Replace("-", "");

            for (var ii = 0; ii < ticketId.Length; ii++)
            {
                ticketId[ii] = Convert.ToByte(ticketIdText.Substring(ii * 3, 2), 16);
            }

            /* TICKET DATA */

            var jj = 0;

            LEU32(DateTimeToUnix(ticketDT), ref ticketData, jj);
            jj += 4;

            var ticketType = Convert.ToByte(ticketTypeId);
            ticketData[jj++] = ticketType;

            switch (ticketType)
            {
                case (byte)((int)JourneyType.Group_Ticket):
                    {
                        LEU16((ushort)from, ref ticketData, jj);
                        jj += 2;
                        LEU16((ushort)to, ref ticketData, jj);
                        jj += 2;
                        ticketData[jj++] = (byte)peopleCount;

                        jj += 3;
                    }
                    break;
                case (byte)((int)JourneyType.SJT):
                    {
                        LEU16((ushort)from, ref ticketData, jj);
                        jj += 2;
                        LEU16((ushort)to, ref ticketData, jj);
                        jj += 2;

                        jj += 4;
                    }
                    break;
                case (byte)((int)JourneyType.RJT):
                    {
                        LEU16((ushort)from, ref ticketData, jj);
                        jj += 2;
                        LEU16((ushort)to, ref ticketData, jj);
                        jj += 2;

                        jj += 4;
                    }
                    break;
                case (byte)((int)JourneyType.Day_Pass):
                    {
                        jj += 8;
                    }
                    break;
                case (byte)((int)JourneyType.EventTicket):
                    {
                        LEU32(DateTimeToUnix(activeFrom), ref ticketData, jj);
                        jj += 4;
                        LEU32(DateTimeToUnix(activeTo), ref ticketData, jj);
                        jj += 4;
                    }
                    break;
            }

            LEU16((ushort)terminalId, ref ticketData, jj);
            jj += 2;

            LEU16((ushort)alias, ref ticketData, jj);
            jj += 2;

            LEU16((ushort)price, ref ticketData, jj);
            jj += 2;

            jj += 3;

            LEU16(CalcCheckSum(ticketData, 0, ticketData.Length - 2), ref ticketData, ticketData.Length - 2);

            /* TICKET CODE */

            jj = 0;

            ticketCode[jj++] = TICKET_VERSION;
            Array.Copy(ticketId, 0, ticketCode, jj, ticketId.Length);
            jj += ticketId.Length;
            Array.Copy(ticketData, 0, ticketCode, jj, ticketData.Length);
            jj += ticketData.Length;

            var code64 = Convert.ToBase64String(ticketCode);
            // Bu değer DB ye QRCodeId için kayıt atacağım değer..
            //var ticket = $"{code64.Length:X2}{code64}"; 
            var ticket = String.Format("{0:X2}{1}", code64.Length, code64);

            return ticket;
        }

    }
}
