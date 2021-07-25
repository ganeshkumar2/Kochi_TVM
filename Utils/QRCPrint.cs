using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kochi_TVM.Utils
{
    public class QRCPrint
    {
        private static ILog log = LogManager.GetLogger(typeof(QRCPrint).Name);

        #region PRIVATE MEMBERS

        private SerialPort sp = null;
        private Svelta svelta = new Svelta();
        private ESCPOS escpos = new ESCPOS();

        #endregion

        #region PUBLIC MEMBERS

        public string printerPort = string.Empty;
        public int printerBaudRate = 115200;
        public Handshake printerHandsdhake = Handshake.None;

        #endregion

        #region PRIVATE METHODS

        public bool CommOpen(string port, ref string err)
        {
            bool Result = true;

            err = "";

            try
            {
                sp = new SerialPort();

                /* Initialize COM Port */

                sp.PortName = port;
                sp.BaudRate = printerBaudRate;
                sp.Handshake = printerHandsdhake;
                sp.Parity = Parity.None;
                sp.DataBits = 8;
                sp.StopBits = StopBits.One;
                sp.ReadTimeout = 500;

                sp.Open();

                Result = sp.IsOpen;
            }
            catch (Exception E)
            {
                err = E.Message;

                Result = false;
            }

            return Result;
        }

        private void CommClose(ref string err)
        {
            if (sp != null)
            {
                try
                {
                    sp.Close();

                    sp = null;
                }
                catch (Exception E)
                {
                    err = E.Message;

                    sp = null;
                }
            }
        }

        private bool Transmit(byte[] tx, int timeout, ref string err)
        {
            bool Result = true;

            err = "";

            try
            {
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();

                sp.WriteTimeout = timeout;
                log.Debug("Transmit :" + BitConverter.ToString(tx));
                sp.Write(tx, 0, tx.Length);
            }
            catch (Exception E)
            {
                err = E.Message;

                Result = false;
            }

            return Result;
        }

        private bool Receive(int waitLen, ref byte[] rx, ref int rxLen, ref string err)
        {
            bool Result = false;

            err = "";
            rxLen = 0;
            Array.Clear(rx, 0, rx.Length);

            try
            {
                DateTime startDT = DateTime.Now;

                do
                {
                    Result = (sp.BytesToRead >= waitLen);

                    if (Result)
                    {
                        rxLen = sp.Read(rx, 0, sp.BytesToRead);

                        Result = (rxLen >= waitLen);
                    }
                    else
                        Thread.Sleep(100);
                }

                while ((!Result) && (startDT.AddMilliseconds(sp.ReadTimeout) > DateTime.Now));
            }
            catch (Exception E)
            {
                Result = false;

                err = E.Message;
            }

            return Result;
        }

        private byte[] HexStrToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private void ArraySum(ref byte[] vector, byte[] toadd)
        {
            if (vector != null)
            {
                int start_copy = vector.Length;
                Array.Resize(ref vector, vector.Length + toadd.Length);
                Array.ConstrainedCopy(toadd, 0, vector, start_copy, toadd.Length);
            }
            else
            {
                vector = new byte[toadd.Length];
                Array.ConstrainedCopy(toadd, 0, vector, 0, toadd.Length);
            }
        }

        private byte CreateByteFromPixel(ref Bitmap image, long start_byte, long y)
        {
            byte ret = 0;

            if (image.Width - start_byte <= 7)
            {
                for (int i = 0; i < image.Width - start_byte; i++)
                {
                    if (image.GetPixel(Convert.ToInt32(start_byte + i), Convert.ToInt32(y)).B == 0)
                        ret += Convert.ToByte(Math.Pow(Convert.ToInt32(2), Convert.ToInt32(7 - i)));
                }
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (image.GetPixel(Convert.ToInt32(start_byte + i), Convert.ToInt32(y)).B == 0)
                        ret += Convert.ToByte(Math.Pow(2, 7 - i));
                }
            }

            return ret;
        }

        public static byte[] FromBitmapToGam(Bitmap image)
        {
            byte[] cmd_print = { 0x1b, 0x2a, 0x72, 0x42 };
            byte[] cmd_res = { 0x1b, 0x2a, 0x74, 0x32, 0x30, 0x34, 0x00 };
            byte[] cmd_comp = { 0x1b, 0x2a, 0x62, 0x30, 0x4d };
            byte[] _vector_prn = null;
            byte[] cmd_width = null;
            cmd_width = new byte[3 + image.Width.ToString().Length + 1 + 1];
            cmd_width[0] = 0x1b;
            cmd_width[1] = 0x26;
            cmd_width[2] = 0x6c;
            for (int i = 0; i < image.Width.ToString().Length; i++)
                cmd_width[3 + i] = Convert.ToByte((image.Width.ToString().Substring(i, 1)));
            cmd_width[cmd_width.Length - 1] = 0x50;
            byte[] cmd_lenght = null;
            cmd_lenght = new byte[3 + image.Height.ToString().Length + 1 + 1];
            cmd_lenght[0] = 0x1b;
            cmd_lenght[1] = 0x26;
            cmd_lenght[2] = 0x6c;
            for (Int32 i = 0; i < image.Height.ToString().Length; i++)
                cmd_lenght[3 + i] = (byte)(Convert.ToByte((image.Height.ToString().Substring(i, 1))) + 0x30);
            cmd_lenght[cmd_lenght.Length - 1] = 0x52;

            byte[][] dotline = null;
            dotline = new byte[image.Height][];
            for (int z = 0; z < dotline.Length; z++)
            {
                byte[] arr = new byte[(image.Width / 8) + 1];
                dotline[z] = arr;
            }
            for (int linea = 0; linea < image.Height; linea++)
            {
                for (int nbite = 0; nbite <= (image.Width) - 1; nbite += 8)
                    dotline[linea][nbite / 8] = create_byte_from_pixel(ref image, nbite, linea);
            }

            _vector_prn = new byte[1];
            _vector_prn[0] = 0x18;
            Array_Sum(ref _vector_prn, ref cmd_res);
            Array_Sum(ref _vector_prn, ref cmd_lenght);
            Array_Sum(ref _vector_prn, ref cmd_comp);

            for (int i = 0; i <= image.Height - 1; i++)
            {
                byte[] move_y = null;
                byte temp = 0;
                //byte[] move_x = { 0x1b, 0x2a, 0x70, 0x31, 0x31, 0x33, 0x58 };
                byte[] move_x = { 0x1b, 0x2a, 0x70, 0x31, 0x33, 0x26, 0x58 };
                Array_Sum(ref _vector_prn, ref move_x);

                move_y = new byte[3 + i.ToString().Length + 1];
                move_y[0] = 0x1b;
                move_y[1] = 0x2a;
                move_y[2] = 0x70;
                for (int z = 0; z < i.ToString().Length; z++)
                    move_y[3 + z] = Convert.ToByte((i.ToString().Substring(z, 1)));
                move_y[move_y.Length - 1] = 0x59;
                byte[] cmd_dot = { 0x1b, 0x2a, 0x72 };
                string sss = (dotline[0].Length).ToString();
                cmd_dot = new byte[3 + sss.Length + 1];
                cmd_dot[0] = 0x1b;
                cmd_dot[1] = 0x2a;
                cmd_dot[2] = 0x62;
                for (int c = 0; c < sss.Length; c++)
                    cmd_dot[3 + c] = (byte)(Convert.ToByte(sss.Substring(c, 1)) + 0x30);
                cmd_dot[cmd_dot.Length - 1] = 0x57;
                Array_Sum(ref _vector_prn, ref cmd_dot);
                Array_Sum(ref _vector_prn, ref dotline[i]);
            }

            Array_Sum(ref _vector_prn, ref cmd_print);
            return _vector_prn;
        }

        private static byte create_byte_from_pixel(ref Bitmap image, long start_byte, long y)
        {
            byte ret_byte = 0;
            if (image.Width - start_byte <= 7)
            {
                for (int i = 0; i < image.Width - start_byte; i++)
                {
                    if (image.GetPixel(Convert.ToInt32(start_byte + i), Convert.ToInt32(y)).B == 0)
                        ret_byte += Convert.ToByte(Math.Pow(Convert.ToInt32(2), Convert.ToInt32(7 - i)));
                }
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (image.GetPixel(Convert.ToInt32(start_byte + i), Convert.ToInt32(y)).B == 0)
                        ret_byte += Convert.ToByte(Math.Pow(2, 7 - i));
                }
            }
            return ret_byte;
        }
        private static void Array_Sum(ref byte[] vector, ref byte[] toadd)
        {
            if (vector != null)
            {
                int start_copy = vector.Length;
                Array.Resize(ref vector, vector.Length + toadd.Length);
                Array.ConstrainedCopy(toadd, 0, vector, start_copy, toadd.Length);
            }
            else
            {
                vector = new byte[toadd.Length];
                Array.ConstrainedCopy(toadd, 0, vector, 0, toadd.Length);
            }

        }


        //private byte[] FromBitmapToGam(Bitmap image)
        //{
        //    byte[] _vector_prn = null;

        //    byte[][] dotline = null;
        //    dotline = new byte[image.Height][];
        //    for (int z = 0; z < dotline.Length; z++)
        //    {
        //        byte[] arr = new byte[(image.Width / 8) + 1];
        //        dotline[z] = arr;
        //    }
        //    for (int linea = 0; linea < image.Height; linea++)
        //    {
        //        for (int nbite = 0; nbite <= (image.Width) - 1; nbite += 8)
        //            dotline[linea][nbite / 8] = CreateByteFromPixel(ref image, nbite, linea);
        //    }

        //    _vector_prn = new byte[1];
        //    _vector_prn[0] = 0x18;
        //    //ArraySum(ref _vector_prn, escpos.SetPrinterResolution());
        //    //ArraySum(ref _vector_prn, escpos.SetPageMaxLength(Convert.ToUInt16(image.Height)));
        //    ArraySum(ref _vector_prn, escpos.SetCompression());

        //    for (int i = 0; i <= image.Height - 1; i++)
        //    {                
        //        ArraySum(ref _vector_prn, escpos.ReceiveGraphicData(Convert.ToUInt16(dotline[i].Length)));
        //        ArraySum(ref _vector_prn, dotline[i]);
        //    }

        //    ArraySum(ref _vector_prn, escpos.Print());

        //    return _vector_prn;
        //}       

        #endregion

        #region PUBLIC METHODS

        public bool FontTest(ref string err)
        {
            bool Result = true;

            const int offsetRow = 30;
            const int heightCol = 36;
            ushort col = 376;

            List<byte> tx = new List<byte>();

            tx.AddRange(svelta.ClearBuffer());
            tx.AddRange(svelta.SetTicketDimension(736, 448, 5, 92));
            tx.AddRange(svelta.PrintRotatedImage(1, 640, 160));
            tx.AddRange(svelta.Rotate90Clockwise());

            for (byte ii = 1; ii < 20; ii++)
            {
                tx.AddRange(svelta.FontSelect(ii));

                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText(string.Format("{0:d}. Font", ii)));

                col -= heightCol;
            }

            tx.AddRange(svelta.PrintCut());

            Result = Transmit(tx.ToArray(), 5000, ref err);

            Thread.Sleep(200);

            byte[] rx = new byte[8];
            int rxLen = 0;

            Result = Receive(1, ref rx, ref rxLen, ref err);
            if (Result)
            {
                Result = (rx[0] == (byte)PrinterResponse.ACK);
                if (!Result)
                    err = rx[0].ToString("X2");
            }

            return Result;
        }

        public bool GetVersion(ref string info, ref string err)
        {
            info = "";

            bool Result = Transmit(svelta.ChangePrinterEmulationToESC_POS(), 3000, ref err);

            if (Result)
            {
                byte[] rx = new byte[32];
                int rxLen = 0;

                Result = Transmit(escpos.GetPrinterModelId(), 3000, ref err);
                if (Result)
                {
                    byte[] modelId = new byte[2];
                    byte[] versionId = new byte[4];

                    Result = Receive(modelId.Length, ref rx, ref rxLen, ref err);
                    if (Result)
                    {
                        Array.Copy(rx, 0, modelId, 0, modelId.Length);

                        Result = Transmit(escpos.GetROMVersionId(), 3000, ref err);
                        if (Result)
                        {
                            Result = Receive(versionId.Length, ref rx, ref rxLen, ref err);
                            if (Result)
                            {
                                Array.Copy(rx, 0, versionId, 0, versionId.Length);

                                info = string.Format("{0} v{1}", escpos.ParsePrinterModelId(modelId), escpos.ParseROMVersionId(versionId));
                            }
                        }
                    }
                }
            }

            Transmit(escpos.ChangePrinterEmulationToSVELTA(), 3000, ref err);

            return Result;
        }

        public bool GetStatus(ref PrinterStatus status, ref string err)
        {
            status = PrinterStatus.Unknown;
            bool Result = false;
            try
            {
                Result = Transmit(svelta.StatusRequest(), 500, ref err);
                byte[] rx = new byte[32];
                int rxLen = 0;

                Result = Receive(1, ref rx, ref rxLen, ref err);

                if (Result)
                    status = (PrinterStatus)rx[0];
            }
            catch(Exception ex)
            {
                log.Debug("QRCPrinter -> GetStatus() " + ex);
            }
            return Result;
        }

        public bool GetFullStatus(ref string err, ref List<FullStatus> fullStatus, ref List<UserStatus> userStatus, ref List<RecoverebleErrorStatus> recoverebleErrorStatus, ref List<UnrecoverebleErrorStatus> unrecoverebleErrorStatus)
        {
            err = "";

            fullStatus.Clear();
            userStatus.Clear();
            recoverebleErrorStatus.Clear();
            unrecoverebleErrorStatus.Clear();

            bool Result = Transmit(svelta.FullStatusRequest(), 5000, ref err);

            byte[] rx = new byte[64];
            int rxLen = 0;

            Result = Receive(6 + 16 + 1, ref rx, ref rxLen, ref err);

            if (Result)
            {
                string statASCII = ASCIIEncoding.ASCII.GetString(rx, 6, 16);

                byte[] status = HexStrToByteArray(statASCII);

                svelta.ParseStatus(status, ref fullStatus, ref userStatus, ref recoverebleErrorStatus, ref unrecoverebleErrorStatus);
            }

            return Result;
        }

        public bool LogoExist(string logo, ref string err)
        {
            bool Result = Transmit(svelta.GetPictureHeaderInfo(1), 1000, ref err);

            byte[] rx = new byte[32];
            int rxLen = 0;

            Result = Receive(4 + logo.Length + 1, ref rx, ref rxLen, ref err);

            if (Result)
            {
                string info = ASCIIEncoding.ASCII.GetString(rx, 4, logo.Length);

                Result = (info == logo);

                if (!Result)
                    err = info;
            }

            return Result;
        }

        public bool Init(string logo, Bitmap image, ref PrinterStatus status, ref string err)
        {
            bool Result = true;

            status = PrinterStatus.Unknown;
            err = "";

            if (printerPort == string.Empty)
            {
                string[] portNames = SerialPort.GetPortNames();

                Result = (portNames.Length > 0);

                int pp = 0;

                do
                {
                    printerPort = portNames[pp];

                    Result = CommOpen(printerPort, ref err);
                    if (Result)
                        Result = GetStatus(ref status, ref err);

                    if (!Result)
                        CommClose(ref err);

                    pp++;
                }
                while ((pp < portNames.Length) && (!Result));
            }
            else
            {
                Result = CommOpen(printerPort, ref err);
                if (Result)
                    Result = GetStatus(ref status, ref err);
            }

            if ((Result) && (logo != ""))
            {
                Result = LogoExist(logo, ref err);
                if (!Result)
                    Result = SaveImage(1, logo, image, ref err);
            }

            GetStatus(ref status, ref err);

            return Result;
        }

        public bool SaveImage(ushort no, string id, Bitmap image, ref string err)
        {
            bool Result = false;

            ImageConverter converter = new ImageConverter();

            byte[] bmpData = (byte[])converter.ConvertTo(image, typeof(byte[]));

            byte[] imageData = new byte[bmpData.Length - 62];

            Array.Copy(bmpData, 62, imageData, 0, imageData.Length);

            for (int ii = 0; ii < imageData.Length; ii++)
                imageData[ii] = Convert.ToByte(0xFF - imageData[ii]);

            Result = Transmit(svelta.SaveImage(no, (ushort)image.Size.Width, (ushort)image.Size.Height, id, imageData), 30 * 1000, ref err);

            if (Result)
            {
                Thread.Sleep(200);

                byte[] rx = new byte[16];
                int rxLen = 0;

                Result = Receive(6, ref rx, ref rxLen, ref err);

                if (Result)
                {
                    Result = (rx[4] == (byte)PrinterResponse.PROGRAMMING_DONE);

                    if (!Result)
                        err = rx[4].ToString("X2");
                }
            }

            return Result;
        }

        public bool Print(string header, string id, string dt, string type, string from, string to, string count, string price, string no, string footer, ref string err)
        {
            bool Result = true;

            const int offsetRow = 30;
            const int heightCol = 36;
            ushort col = 376;

            List<byte> tx = new List<byte>();

            tx.AddRange(svelta.ClearBuffer());

            tx.AddRange(svelta.SetTicketDimension(736, 448, 5, 92));

            tx.AddRange(svelta.PrintRotatedImage(1, 640, 160));

            tx.AddRange(svelta.Rotate90Clockwise());

            tx.AddRange(svelta.FontSelect(18));

            tx.AddRange(svelta.PositionCursor(180, 670));
            tx.AddRange(svelta.PrintText(header));

            tx.AddRange(svelta.PositionCursor(160, 570));
            tx.AddRange(svelta.QREncodeScheme(QREncodeScheme.QRcode));
            tx.AddRange(svelta.QRModuleSize(4));
            tx.AddRange(svelta.QRSize(0));
            tx.AddRange(svelta.QRECCLevel(0));

            tx.AddRange(svelta.QRPrint(id));

            tx.AddRange(svelta.FontSelect(5));

            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("Date/Time    : " + dt));

            col -= heightCol;
            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("Type         : " + type));

            if (from != "")
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("From         : " + from));
            }

            if (to != "")
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("To           : " + to));
            }

            //if (count != "")
            //{
            //    col -= heightCol;
            //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
            //    tx.AddRange(svelta.PrintText("Count        : " + count));
            //}

            if (price != "")
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("Price        : " + price));
            }

            string[] footerLines = new string[0];
            if (footer != "")
                footerLines = footer.Split('\n');

            if (footerLines.Length < 2)
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("---------------------------------------"));
            }

            col -= heightCol;
            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("Ticket No    : " + no));

            col -= heightCol;
            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("---------------------------------------"));

            for (byte ii = 0; ii < footerLines.Length; ii++)
            {
                if (footerLines[ii].Trim() == "")
                    continue;

                if ((ii + 1) == footerLines.Length)
                    tx.AddRange(svelta.FontSelect(3));

                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow - 18, col));
                tx.AddRange(svelta.PrintText(footerLines[ii]));
            }

            tx.AddRange(svelta.PrintCut());

            Result = Transmit(tx.ToArray(), 5000, ref err);

            //Thread.Sleep(600);

            byte[] rx = new byte[8];
            int rxLen = 0;

            Result = Receive(1, ref rx, ref rxLen, ref err);
            if (Result)
            {
                Result = (rx[0] == (byte)PrinterResponse.ACK);
                if (!Result)
                    err = rx[0].ToString("X2");
            }

            return Result;
        }

        public bool PrintG(Bitmap qr, string header, string id, string dt, string type, string from, string to, string count, string price, string no, string footer, ref string err)
        {
            bool Result = true;

            /*
            List<byte> tx = new List<byte>();
            tx.AddRange(svelta.ChangePrinterEmulationToESC_POS());
            tx.AddRange(new byte[] {0x1B, 0x26, 0x6C, 0x31, 0x37, 0x39, 0x32, 0x52 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x74, 0x32, 0x30, 0x34, 0x52 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x31, 0x4D });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x35, 0x30, 0x58 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x32, 0x35, 0x59 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x35, 0x30, 0x57, 0xF2, 0x0F });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x2B, 0x31, 0x30, 0x30, 0x58 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x35, 0x30, 0x59 });
            tx.AddRange(new byte[] {0x50, 0x52, 0x49, 0x4E, 0x54, 0x20, 0x54, 0x45, 0x53, 0x54 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x35, 0x30, 0x58 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x39, 0x39, 0x59 });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x35, 0x30, 0x57, 0xF2, 0x0F });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x30, 0x4D });
            tx.AddRange(new byte[] {0x1B, 0x2A, 0x72, 0x42 });
            tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());
            tx.AddRange(svelta.PrintCut());

            Result = Transmit(tx.ToArray(), 5000, ref err);
            */
          
            const int offsetRow = 0;
            const int heightCol = 30;
            ushort col = 360;
              
            List<byte> tx = new List<byte>();
            List<byte> tx1 = new List<byte>();

            //tx.AddRange(svelta.ClearBuffer());

            //tx.AddRange(svelta.PrintRotatedImage(1, 640, 160));
            //Transmit(tx.ToArray(), 5000, ref err);
            //tx.Clear();

            tx.AddRange(svelta.ClearBuffer());

            tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());

            tx.AddRange(svelta.SetTicketDimension(368, 448, 0, 432));
            //tx.AddRange(svelta.SetTicketDimension(816, 440, 40, 768));

            tx.AddRange(svelta.Rotate90Clockwise());

            tx.AddRange(svelta.FontSelect(18));

            //tx.AddRange(svelta.PositionCursor(180, 670));
            //tx.AddRange(svelta.PrintText(header));

            tx.AddRange(svelta.FontSelect(5));

            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("Date/Time : " + dt));

            col -= heightCol;
            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("Type      : " + type));

            if (from != "")
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("From      : " + from));
            }

            if (to != "")
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("To        : " + to));
            }

            //if (count != "")
            //{
            //    col -= heightCol;
            //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
            //    tx.AddRange(svelta.PrintText("Count     : " + count));
            //}

            if (price != "")
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("Price     : " + price));
            }

            string[] footerLines = new string[0];
            if (footer != "")
                footerLines = footer.Split('\n');

            if (footerLines.Length < 2)
            {
                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText("---------------------------------------"));
            }

            col -= heightCol;
            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("Ticket No : " + no));

            col -= heightCol;
            tx.AddRange(svelta.PositionCursor(offsetRow, col));
            tx.AddRange(svelta.PrintText("---------------------------------------"));

            for (byte ii = 0; ii < footerLines.Length; ii++)
            {
                if (footerLines[ii].Trim() == "")
                    continue;

                if ((ii + 1) == footerLines.Length)
                    tx.AddRange(svelta.FontSelect(3));

                col -= heightCol;
                tx.AddRange(svelta.PositionCursor(offsetRow, col));
                tx.AddRange(svelta.PrintText(footerLines[ii]));
            }

            tx.AddRange(svelta.Print());
            Transmit(tx.ToArray(), 5000, ref err);

            Thread.Sleep(500);

            tx.Clear();
            tx.AddRange(svelta.ChangePrinterEmulationToESC_POS());
            tx.AddRange(new byte[] { 0x1B, 0x64, 0x01 });
            tx.AddRange(FromBitmapToGam(qr));
            //tx.AddRange(new byte[] { 0x1B, 0x64, 0x02 });
            Bitmap logo = new Bitmap("D:\\logo\\kmrl_logo.bmp");
            logo.RotateFlip(RotateFlipType.Rotate180FlipX);

            tx.AddRange(FromBitmapToGam(new Bitmap(logo, new Size(500, 50))));
            ////tx.AddRange(new byte[] { 0x1D, 0x57, 1, 0, 0x1D, 0xF8, 0x1D, 0x64, 0x00, 0x1D, 0xF6 });
            //tx.AddRange(new byte[] { 0x1B, 0x7B, 0x01 });
            //tx.AddRange(ASCIIEncoding.ASCII.GetBytes("      KOCHI METRO"));
            //tx.AddRange(new byte[] { 0x1B, 0x7B, 0x00 });
            tx.AddRange(new byte[] { 0x1B, 0x64, 0x06, 0x1B, 0x69 });
            Transmit(tx.ToArray(), 5000, ref err);

            Thread.Sleep(500);

            tx.Clear();
            tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());
            //tx.AddRange(svelta.SetTicketDimension(1, 448, 0, 432));
            //tx.AddRange(svelta.Rotate90Clockwise());
            //tx.AddRange(svelta.PrintCut());
            this.Transmit(tx.ToArray(), 5000, ref err);
            Thread.Sleep(500);


            byte[] rx = new byte[8];
            int rxLen = 0;

            Result = Receive(1, ref rx, ref rxLen, ref err);
            if (Result)
            {
                Result = (rx[0] == (byte)PrinterResponse.ACK);
                if (!Result)
                    err = rx[0].ToString("X2");
            }


            return Result;
        }


        //public bool PrintG(Bitmap qr, string header, string id, string dt, string type, string from, string to, string count, string price, string no, string footer, ref string err)
        //{
        //    bool Result = true;

        //    /*
        //    List<byte> tx = new List<byte>();
        //    tx.AddRange(svelta.ChangePrinterEmulationToESC_POS());
        //    tx.AddRange(new byte[] {0x1B, 0x26, 0x6C, 0x31, 0x37, 0x39, 0x32, 0x52 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x74, 0x32, 0x30, 0x34, 0x52 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x31, 0x4D });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x35, 0x30, 0x58 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x32, 0x35, 0x59 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x35, 0x30, 0x57, 0xF2, 0x0F });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x2B, 0x31, 0x30, 0x30, 0x58 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x35, 0x30, 0x59 });
        //    tx.AddRange(new byte[] {0x50, 0x52, 0x49, 0x4E, 0x54, 0x20, 0x54, 0x45, 0x53, 0x54 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x35, 0x30, 0x58 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x70, 0x39, 0x39, 0x59 });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x35, 0x30, 0x57, 0xF2, 0x0F });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x62, 0x30, 0x4D });
        //    tx.AddRange(new byte[] {0x1B, 0x2A, 0x72, 0x42 });
        //    tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());
        //    tx.AddRange(svelta.PrintCut());

        //    Result = Transmit(tx.ToArray(), 5000, ref err);
        //    */
        //    const int offsetRow = 0;
        //    const int heightCol = 30;
        //    ushort col = 360;

        //    List<byte> tx = new List<byte>();
        //    List<byte> tx1 = new List<byte>();

        //    //tx.AddRange(svelta.ClearBuffer());

        //    //tx.AddRange(svelta.PrintRotatedImage(1, 640, 160));
        //    //Transmit(tx.ToArray(), 5000, ref err);
        //    //tx.Clear();

        //    tx.AddRange(svelta.ClearBuffer());

        //    tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());

        //    tx.AddRange(svelta.SetTicketDimension(368, 448, 0, 432));

        //    tx.AddRange(svelta.Rotate90Clockwise());

        //    tx.AddRange(svelta.FontSelect(18));

        //    //tx.AddRange(svelta.PositionCursor(180, 670));
        //    //tx.AddRange(svelta.PrintText(header));

        //    tx.AddRange(svelta.FontSelect(5));

        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("Date/Time : " + dt));

        //    col -= heightCol;
        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("Type      : " + type));

        //    if (from != "")
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("From      : " + from));
        //    }

        //    if (to != "")
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("To        : " + to));
        //    }

        //    //if (count != "")
        //    //{
        //    //    col -= heightCol;
        //    //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    //    tx.AddRange(svelta.PrintText("Count     : " + count));
        //    //}

        //    if (price != "")
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("Price     : " + price));
        //    }

        //    string[] footerLines = new string[0];
        //    if (footer != "")
        //        footerLines = footer.Split('\n');

        //    if (footerLines.Length < 2)
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("---------------------------------------"));
        //    }

        //    col -= heightCol;
        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("Ticket No : " + no));

        //    col -= heightCol;
        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("---------------------------------------"));

        //    for (byte ii = 0; ii < footerLines.Length; ii++)
        //    {
        //        if (footerLines[ii].Trim() == "")
        //            continue;

        //        if ((ii + 1) == footerLines.Length)
        //            tx.AddRange(svelta.FontSelect(3));

        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText(footerLines[ii]));
        //    }

        //    tx.AddRange(svelta.Print());
        //    Transmit(tx.ToArray(), 5000, ref err);

        //    Thread.Sleep(600);

        //    tx.Clear();
        //    tx.AddRange(svelta.ChangePrinterEmulationToESC_POS());
        //    tx.AddRange(FromBitmapToGam(qr));
        //    Bitmap logo = new Bitmap("D:\\logo\\kmrl_logo.bmp");
        //    logo.RotateFlip(RotateFlipType.Rotate180FlipX);

        //    tx.AddRange(FromBitmapToGam(new Bitmap(logo, new Size(150, 62))));
        //    //tx.AddRange(new byte[] { 0x1D, 0x57, 1, 0, 0x1D, 0xF8, 0x1D, 0x64, 0x00, 0x1D, 0xF6 });
        //    tx.AddRange(new byte[] { 0x1B, 0x7B, 0x01 });
        //    tx.AddRange(ASCIIEncoding.ASCII.GetBytes("      KOCHI METRO"));
        //    tx.AddRange(new byte[] { 0x1B, 0x7B, 0x00 });
        //    tx.AddRange(new byte[] { 0x1B, 0x64, 0x01, 0x1B, 0x69 });
        //    Transmit(tx.ToArray(), 5000, ref err);

        //    Thread.Sleep(600);
        //    tx.Clear();
        //    tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());
        //    tx.AddRange(svelta.SetTicketDimension(1,448,0,432));
        //    tx.AddRange(svelta.Rotate90Clockwise());
        //    tx.AddRange(svelta.PrintCut());
        //    Transmit(tx.ToArray(), 5000, ref err);
        //    //Thread.Sleep(600);

        //    //tx.Clear();
        //    //tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());

        //    //Transmit(tx.ToArray(), 5000, ref err);

        //    Thread.Sleep(1000);

        //    byte[] rx = new byte[8];
        //    int rxLen = 0;

        //    Result = Receive(1, ref rx, ref rxLen, ref err);
        //    if (Result)
        //    {
        //        Result = (rx[0] == (byte)PrinterResponse.ACK);
        //        if (!Result)
        //            err = rx[0].ToString("X2");
        //    }


        //    return Result;
        //}

        //public bool PrintG(Bitmap qr, string header, string id, string dt, string type, string from, string to, string count, string price, string no, string footer, ref string err)
        //{
        //    bool Result = true;

        //    svelta = new Svelta();
        //    escpos = new ESCPOS();

        //    const int offsetRow = 0;
        //    const int heightCol = 30;
        //    ushort col = 360;

        //    List<byte> tx = new List<byte>();

        //    tx.AddRange(svelta.ClearBuffer());

        //    tx.AddRange(escpos.ChangePrinterEmulationToSVELTA());

        //    tx.AddRange(svelta.SetTicketDimension(368, 448, 0, 432));

        //    tx.AddRange(svelta.Rotate90Clockwise());

        //    tx.AddRange(svelta.FontSelect(18));

        //    tx.AddRange(svelta.PositionCursor(180, 670));
        //    tx.AddRange(svelta.PrintText(header));

        //    tx.AddRange(svelta.FontSelect(5));

        //    tx.AddRange(svelta.ChangePrinterEmulationToESC_POS());
        //    tx.AddRange(FromBitmapToGam(qr));
        //    string headerAddress = "Images\\kmrl_reve_logo.bmp";
        //    Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
        //    Bitmap logo = new System.Drawing.Bitmap(img);
        //    //Bitmap logo = new Bitmap("D:\\logo\\kmrl_logo.bmp");
        //    logo.RotateFlip(RotateFlipType.Rotate180FlipX);

        //    tx.AddRange(FromBitmapToGam(new Bitmap(logo, new Size(150, 62))));
        //    //tx.AddRange(new byte[] { 0x1D, 0x57, 1, 0, 0x1D, 0xF8, 0x1D, 0x64, 0x00, 0x1D, 0xF6 });
        //    tx.AddRange(new byte[] { 0x1B, 0x7B, 0x01 });
        //    tx.AddRange(ASCIIEncoding.ASCII.GetBytes("      KOCHI METRO"));
        //    tx.AddRange(new byte[] { 0x1B, 0x7B, 0x00 });
        //    tx.AddRange(new byte[] { 0x1B, 0x64, 0x01, 0x1B, 0x69 });

        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("Date/Time : " + dt));

        //    col -= heightCol;
        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("Type      : " + type));

        //    if (from != "")
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("From      : " + from));
        //    }

        //    if (to != "")
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("To        : " + to));
        //    }

        //    //if (count != "")
        //    //{
        //    //    col -= heightCol;
        //    //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    //    tx.AddRange(svelta.PrintText("Count     : " + count));
        //    //}

        //    if (price != "")
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("Price     : " + price));
        //    }

        //    string[] footerLines = new string[0];
        //    if (footer != "")
        //        footerLines = footer.Split('\n');

        //    if (footerLines.Length < 2)
        //    {
        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText("---------------------------------------"));
        //    }

        //    col -= heightCol;
        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("Ticket No : " + no));

        //    col -= heightCol;
        //    tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //    tx.AddRange(svelta.PrintText("---------------------------------------"));

        //    for (byte ii = 0; ii < footerLines.Length; ii++)
        //    {
        //        if (footerLines[ii].Trim() == "")
        //            continue;

        //        if ((ii + 1) == footerLines.Length)
        //            tx.AddRange(svelta.FontSelect(3));

        //        col -= heightCol;
        //        tx.AddRange(svelta.PositionCursor(offsetRow, col));
        //        tx.AddRange(svelta.PrintText(footerLines[ii]));
        //    }


        //    Transmit(tx.ToArray(), 5000, ref err);

        //    tx.AddRange(svelta.Print());
        //    Transmit(tx.ToArray(), 5000, ref err);

        //    //Thread.Sleep(600);

        //    tx.Clear();


        //    Thread.Sleep(200);

        //    //tx.Clear();
        //    //tx.AddRange(svelta.ChangePrinterEmulationToESC_POS());

        //    //Transmit(tx.ToArray(), 5000, ref err);

        //    //Thread.Sleep(1000);

        //    byte[] rx = new byte[8];
        //    int rxLen = 0;

        //    Result = Receive(1, ref rx, ref rxLen, ref err);
        //    if (Result)
        //    {
        //        Result = (rx[0] == (byte)PrinterResponse.ACK);
        //        if (!Result)
        //            err = rx[0].ToString("X2");
        //    }
        //    return Result;
        //}

        public void Done(ref string err)
        {
            CommClose(ref err);
        }

        #endregion
    }
}
