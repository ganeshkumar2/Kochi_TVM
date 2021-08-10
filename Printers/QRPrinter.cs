using Kochi_TVM.Business;
using Kochi_TVM.Utils;
using log4net;
using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Printers
{
    public class QRPrinter
    {
        private static ILog log = LogManager.GetLogger(typeof(QRPrinter).Name);

        #region Defines
        string id = "QR";
        string name = "QR Printer";
        string description = "Custom KPM150H";
        private static Printer_ErrorCodes errCode = Printer_ErrorCodes.Success;
        private static string errDesc = String.Empty;
        public static bool qrPrinterInit = false;
        public static PrinterStatus status = PrinterStatus.Unknown;

        string printerName = String.Empty;

        QRCPrint qrPrinter = new QRCPrint();
        #endregion

        #region DeviceBase
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private static QRPrinter _instance = null;
        public static QRPrinter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QRPrinter();
                return _instance;
            }
        }
        public QRPrinter()
        {
            Init();
        }

        public bool Init()
        {
            bool result = false;
            string port = String.Empty;
            string logo = String.Empty;
            Bitmap image = null;
            string error = String.Empty;
            try
            {
                string headerAddress = "Images\\kmrl_icon.png";
                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                image = new System.Drawing.Bitmap(img);
                qrPrinter.printerPort = Parameters.TVMConst.QRPrinterPort;//ConfigurationManager.AppSettings["QR_PORT_NAME"];
                if (qrPrinter.Init("", image, ref status, ref error))
                {
                    result = true;
                    qrPrinterInit = true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error QRPrinter -> Init() : " + ex.ToString());
                result = false;
                qrPrinterInit = false;
            }

            return result;
        }

        private void SetErrCode(PrinterStatus errStatus)
        {
            errCode = Printer_ErrorCodes.Success;

            if (errStatus == PrinterStatus.CutterError)
                errCode = Printer_ErrorCodes.CutterError;
            else if (errStatus == PrinterStatus.HeadingOverTemperatureError)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.NotchError)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.PaperEnd)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.PowerSupplyVoltageError)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.WrongCommand)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.Unknown)
                errCode = Printer_ErrorCodes.UnknownError;
            log.Debug("Debug SetErrCode -> SetErrCode() : " + errCode.ToString());
        }
        public bool GetStatus(ref PrinterStatus status)
        {
            errCode = Printer_ErrorCodes.Success;
            errDesc = String.Empty;
            String error = String.Empty;
            PrinterStatus printerStatus = PrinterStatus.NoError;
            bool result = false;
            try
            {
                result = qrPrinter.GetStatus(ref printerStatus, ref error);
                if (printerStatus != PrinterStatus.NoError)
                {
                    result = false;
                    SetErrCode(printerStatus);
                    log.Error("Exception Message : " + error);
                    errDesc = "Exception Message : " + error;
                }
            }
            catch (Exception ex)
            {
                result = false;
                errCode = Printer_ErrorCodes.UnknownError;
                errDesc = "Exception Message : " + ex.Message;
                log.Error("Error QRPrinter -> Init() : " + errDesc);
            }
            status = printerStatus;
            return result;
        }

        public Enums.PRINTER_STATE CheckQrPrinterStatus()
        {
            //return Enums.PRINTER_STATE.OK;
            bool result = false;
            PrinterStatus getStat = PrinterStatus.Unknown;
            result = GetStatus(ref getStat);
            if (getStat == PrinterStatus.NoError)
            {
                result = true;
                log.Debug("CheckQrPrinterStatus : " + result);
                return Enums.PRINTER_STATE.OK;
            }
            else
            {
                return Enums.PRINTER_STATE.ERROR;
            }
        }

        #endregion

        #region PrinterBase

        public bool PrintQR(string ticketGUID,string qr_date, string journeyType, string startStation, string endStation, int peopleCount, decimal totalPrice, string ticketID)
        {
            bool result = false;
            //List<string> result = new List<string>();
            try
            {
                string err = String.Empty;
                Bitmap imageOF = new Bitmap(new QRCodeEncoder().Encode(ticketGUID), new Size(150, 150));
                bool resultQr = qrPrinter.PrintG(imageOF, "KOCHI METRO", ticketGUID, qr_date, journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);//TVM'deki halin TOM'a benzetilmesi
                //qrPrinter.PrintK("KOCHI METRO", ticketGUID, DateTime.Now.ToString(), journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, "Please retain till the end of journey!", ref err);//TVM'deki halin TOM'a benzetilmesi
                //bool resultQr = qrPrinter.Print("KOCHI METRO", ticketGUID, qr_date, journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);//TVM orjinali
                ////Task responseTask = Task.Run(() => {
                ////    resultQr = qrPrinter.PrintG(imageOF, "KOCHI METRO", ticketGUID, qr_date, journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);
                ////    Console.WriteLine("In task");
                ////});
                ////responseTask.ContinueWith(t => Console.WriteLine("In ContinueWith"));
                ////responseTask.Wait();
                result = resultQr;                                                                                                                                                                                                                        //return qrPrinter.Print("KOCHI METRO", t.TicketGUID, PayPointConst.QRCodeCreateDate.ToString(), t.explanation, t.From, t.To, (t.count == 0) ? "" : t.count.ToString(), $"Rs.{t.price}", $"{PayPointConst.qrDayCount}.{unitParams.stationId.ToString("D2")}.{ unitParams.unitId.ToString("D2")}.{t.alias}", "Please retain till the end of journey!", ref err);//TOM'daki şekli
                return result;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("Error QRPrinter -> PrintQR() : " + ex.ToString());
                return result;
            }
        }

        public bool PrintQRTest(string ticketGUID, string qr_date, string journeyType, string startStation, string endStation, int peopleCount, decimal totalPrice, string ticketID)
        {
            bool result = false;
            //List<string> result = new List<string>();
            try
            {
                string err = String.Empty;

                Bitmap imageOF = new Bitmap(new QRCodeEncoder().Encode(ticketGUID), new Size(150, 150));

                bool resultQr = qrPrinter.Print("KOCHI METRO", ticketGUID, qr_date, journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);//TVM'deki halin TOM'a benzetilmesi
                //qrPrinter.PrintK("KOCHI METRO", ticketGUID, DateTime.Now.ToString(), journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, "Please retain till the end of journey!", ref err);//TVM'deki halin TOM'a benzetilmesi
                //bool resultQr = qrPrinter.Print("KOCHI METRO", ticketGUID, qr_date, journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);//TVM orjinali
                ////Task responseTask = Task.Run(() => {
                ////    resultQr = qrPrinter.PrintG(imageOF, "KOCHI METRO", ticketGUID, qr_date, journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);
                ////    Console.WriteLine("In task");
                ////});
                ////responseTask.ContinueWith(t => Console.WriteLine("In ContinueWith"));
                ////responseTask.Wait();
                result = resultQr;                                                                                                                                                                                                                        //return qrPrinter.Print("KOCHI METRO", t.TicketGUID, PayPointConst.QRCodeCreateDate.ToString(), t.explanation, t.From, t.To, (t.count == 0) ? "" : t.count.ToString(), $"Rs.{t.price}", $"{PayPointConst.qrDayCount}.{unitParams.stationId.ToString("D2")}.{ unitParams.unitId.ToString("D2")}.{t.alias}", "Please retain till the end of journey!", ref err);//TOM'daki şekli
                return result;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("Error QRPrinter -> PrintQR() : " + ex.ToString());
                return result;
            }
        }

        #endregion
    }

    public enum Printer_ErrorCodes
    {
        //device err : 10, device Code : 05, err Code : 1 - 99 => ex : 100312
        Success = 0,
        ParameterError = 100501,
        GetStatusError = 100502,
        ResetError = 100503,
        DeviceBussyError = 100504,
        NotAvailable = 100505,
        OfflineError = 100506,
        PaperError = 100507,
        JammedError = 100508,
        PaperEnd = 100409,
        NoError = 100410,
        WrongCommand = 100411,
        NotchError = 100412,
        HeadingOverTemperatureError = 100413,
        PowerSupplyVoltageError = 100414,
        CutterError = 100415,
        UnknownError = 100599
    }
}
