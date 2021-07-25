using Kochi_TVM.Business;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Printers
{
    class CustomKPM150HPrinter
    {
        private static ILog log = LogManager.GetLogger(typeof(CustomKPM150HPrinter).Name);

        Image Qrimg;

        static int widthPoits = 144;

        static int lineGap = 13;
        static int space = 5;
        static int seperatorPoint = 120;
        static int lineY = 2;
        static string seperatorChar = ":";
        PrintDocument printDocument = null;
        List<PrintObject> printList = new List<PrintObject>();
        Graphics printGraphics = null;
        Font printFont = new Font("Arial", 8);
        Font fontBold = new Font("Arial", 8, FontStyle.Bold);
        public enum ContentType
        {
            Text = 0,
            Image = 1,
            BoldText = 2
        }
        private struct PrintObject
        {
            public StringAlignment align;
            public int startX;
            public int startY;
            public ContentType contentType;
            public string text;
            public Bitmap image;
        }

        private static CustomKPM150HPrinter _instance = null;
        public static CustomKPM150HPrinter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomKPM150HPrinter();
                return _instance;
            }
        }
        public CustomKPM150HPrinter()
        {

        }
        string PrinterName = ConfigurationManager.AppSettings["QRPrinterName"];//"CUSTOM KPM150";//"Microsoft Print to PDF";
        public Enums.PRINTER_STATE getStatusWithUsb()
        {
            try
            {
                string query = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", PrinterName);

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                using (ManagementObjectCollection coll = searcher.Get())
                {
                    try
                    {
                        foreach (ManagementObject printer in coll)
                        {
                            foreach (PropertyData property in printer.Properties)
                            {
                                if (property.Name == "WorkOffline")
                                {
                                    if ((bool)property.Value)
                                    {
                                        return Enums.PRINTER_STATE.ERROR;
                                    }
                                    else
                                    {
                                        var server = new LocalPrintServer();

                                        PrintQueue queue = server.GetPrintQueue(PrinterName, new string[0] { });

                                        queue.Refresh();

                                        if (queue.IsOffline)
                                            return Enums.PRINTER_STATE.ERROR;

                                        if (queue.IsPaused)
                                            return Enums.PRINTER_STATE.ERROR;

                                        if (queue.IsOutOfPaper)
                                            return Enums.PRINTER_STATE.NO_PAPER;

                                        if (queue.HasPaperProblem)
                                            return Enums.PRINTER_STATE.LOW_PAPER;

                                        if (!queue.IsOffline)
                                            return Enums.PRINTER_STATE.OK;

                                        if (!queue.IsOutOfPaper)
                                            return Enums.PRINTER_STATE.OK;

                                        return Enums.PRINTER_STATE.OTHER;
                                    }
                                }
                            }
                        }
                        return Enums.PRINTER_STATE.ERROR;
                    }
                    catch (ManagementException ex)
                    {
                        return Enums.PRINTER_STATE.ERROR;
                    }
                }                
            }
            catch (Exception ex)
            {
                return Enums.PRINTER_STATE.ERROR;
            }
        }


        void AddPrintObject(int startX, int startY, ContentType contentType, string printText, Bitmap image, StringAlignment align)
        {
            PrintObject po = new PrintObject();
            po.startX = startX;
            po.startY = startY;
            po.contentType = contentType;
            po.text = printText;
            po.align = align;
            po.image = image;

            printList.Add(po);
        }
        public bool AddLine(int startX, int startY, string printText, StringAlignment align)
        {
            AddPrintObject(startX, startY, ContentType.Text, printText, null, align);
            return true;
        }

        public bool AddLine(int startX, int startY, string printText)
        {
            AddPrintObject(startX, startY, ContentType.Text, printText, null, StringAlignment.Far);
            return true;
        }
        public bool AddLineBold(int startX, int startY, string printText)
        {
            AddPrintObject(startX, startY, ContentType.BoldText, printText, null, StringAlignment.Far);
            return true;
        }
        public bool AddLine(int startX, int startY, Bitmap image)
        {
            AddPrintObject(startX, startY, ContentType.Image, String.Empty, image, StringAlignment.Far);
            return true;
        }
        public void AddTextCenter(string caption)
        {
            lineY += 1;
            AddLineBold(60, lineY, caption);
        }
        public void AddTextFar(string caption)
        {
            lineY += lineGap;
            AddLine(0, lineY, caption, System.Drawing.StringAlignment.Far);
        }
        public void AddTextAfterImage(string firstPart, string secondPart, int seperatorPoint)
        {
            lineY += lineGap;
            //lineY = 170;
            AddLine(space, lineY, firstPart);
            AddLine(seperatorPoint, lineY, seperatorChar);
            AddLine(space + seperatorPoint, lineY, secondPart);
        }
        public void AddText(string firstPart, string secondPart, int seperatorPoint)
        {
            lineY += lineGap;
            AddLine(space, lineY, firstPart);
            AddLine(seperatorPoint, lineY, seperatorChar);
            AddLine(space + seperatorPoint, lineY, secondPart);
        }
        public void AddTextBold(string firstPart)
        {
            lineY += lineGap;
            AddLineBold(space, lineY, firstPart);
        }
        public void AddLogoImage(Bitmap image)
        {
            lineY += 10;
            AddLine(20, lineY, image);
            lineY += image.Height;
        }
        public void AddImage(Bitmap image)
        {
            lineY += 5;
            AddLine(50, lineY, image);
            lineY += image.Height;
        }
        int AlignCenter(PrintObject po)
        {
            int startPosition = 0;
            int objectSize = 0;
            int pageSize = 150;

            if (po.contentType == ContentType.Text)
            {
                SizeF textSize = printGraphics.MeasureString(po.text, printFont);
                objectSize = (int)textSize.Width;
            }
            else if (po.contentType == ContentType.BoldText)
            {
                SizeF textSize = printGraphics.MeasureString(po.text, printFont);
                objectSize = (int)textSize.Width;
            }
            else if (po.contentType == ContentType.Image)
            {
                objectSize = po.image.Height;
            }

            startPosition = ((pageSize - objectSize) / 2);
            return startPosition;
        }
        public void PrintQRTicket(Bitmap imgPrint, string dt, string type, string from, string to, string count, string price, string no)//, TicketGrid ticketGrid)
        {
            try
            {
                lineY = 2;
                printList = new List<PrintObject>();
                string headerAddress = "Images\\kmlr_QR_logo.png";
                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
                logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(180, 40));
                AddLogoImage(logo);

                System.Drawing.Bitmap qrimg = new System.Drawing.Bitmap(imgPrint);
                qrimg = new System.Drawing.Bitmap(qrimg, new System.Drawing.Size(100, 100));
                AddImage(qrimg);

                AddTextAfterImage("Date/Time", dt, 80);
                AddText("Type", type, 80);
                AddText("From", from, 80);
                AddText("To", to, 80);
                AddText("Price", price, 80);
                AddText("Ticket No", no, 80);
                AddText("Fare Mode", "Normal", 80);
                AddTextFar("------------------------------------------------------------------------------");
                AddTextFar("Kochi1 card holder saved Rs");                
                AddTextFar("on this trip.Get your card now!!");
                AddTextBold("Please retain till the end of journey!");
                AddTextFar("------------------------------------------------------------------------------");

                PrintDocument Document = new PrintDocument();
                PrintController printController = new StandardPrintController();
                Document.PrintController = printController;
                Document.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
                Document.PrinterSettings.PrinterName = PrinterName;
                Document.Print();


            }
            catch (Exception ex)
            {
                log.Error("Error CustomKPM150HPrinter -> PrintQRTicket() : " + ex.ToString());
            }
        }
        public void PrintTestQRTicket(Bitmap imgPrint)//, TicketGrid ticketGrid)
        {
            try
            {
                lineY = 2;
                printList = new List<PrintObject>();
                string headerAddress = "Images\\kmlr_QR_logo.png";
                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
                logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(180, 40));
                AddLogoImage(logo);
                System.Drawing.Bitmap qrimg = new System.Drawing.Bitmap(imgPrint);
                qrimg = new System.Drawing.Bitmap(qrimg, new System.Drawing.Size(100, 100));
                AddImage(qrimg);

                AddTextAfterImage("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
                AddText("Type", Ticket.journeyType.ToString(), 80);
                AddText("From", Ticket.startStation.name, 80);
                AddText("To", Ticket.endStation.name, 80);
                AddText("Price", Convert.ToString(Ticket.totalPrice), 80);
                AddText("Ticket No", "000001111", 80);
                AddText("Fare Mode", "Normal", 80);
                AddTextFar("------------------------------------------------------------------------------");
                AddTextBold("");
                AddTextBold("Please retain till the end of journey!");
                AddTextBold("");
                AddTextFar("------------------------------------------------------------------------------");

                PrintDocument Document = new PrintDocument();
                PrintController printController = new StandardPrintController();
                Document.PrintController = printController;
                Document.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
                Document.PrinterSettings.PrinterName = PrinterName;
                Document.Print();


            }
            catch (Exception ex)
            {
                log.Error("Error CustomKPM150HPrinter -> PrintQRTicket() : " + ex.ToString());
            }
        }
        void printDocumentPrintPage(object sender, PrintPageEventArgs e)
        {
            printGraphics = e.Graphics;
            PrintObject po;

            for (int i = 0; i < printList.Count; i++)
            {
                po = printList[i];

                if (po.align == StringAlignment.Center)
                {
                    po.startX = AlignCenter(po);
                }

                if (po.contentType == ContentType.Text)
                {
                    printGraphics.DrawString(po.text, printFont, new SolidBrush(System.Drawing.Color.Black), po.startX, po.startY);
                }
                else if(po.contentType == ContentType.BoldText)
                {
                    printGraphics.DrawString(po.text, fontBold, new SolidBrush(System.Drawing.Color.Black), po.startX, po.startY);
                }
                else if (po.contentType == ContentType.Image)
                {
                    printGraphics.DrawImage(po.image, po.startX, po.startY);
                }
            }
        }
    }
}
