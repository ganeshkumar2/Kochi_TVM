using Kochi_TVM.MultiLanguages;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Business
{
    public static class Ticket
    {
        private static ILog log = LogManager.GetLogger(typeof(Ticket).Name);

        //for QR Guid
        public static string ticketGUID = string.Empty;
        public static List<TSelectedTickets> listTickets = new List<TSelectedTickets>();
        //default languages
        public static Languages language = Languages.English;
        //datetime.now
        public static DateTime transactionDts = DateTime.Now;
        //weekend pass, event pass begin date, others datetime.now
        public static DateTime ticketActivateDts = DateTime.Now;
        //weekend pass, event pass end date, others 23:59:59 
        public static DateTime ticketExpiryDts = DateTime.Now;
        public static TicketType ticketType = TicketType.QR;
        public static JourneyType journeyType = JourneyType.Unknown;
        public static Station startStation = new Station();
        public static Station endStation = new Station();
        public static int ticketCount = 1; //
        public static int sellTicketCount = 0; //

        public static int passSubTypeId;
        public static int passCategory;

        public static List<EmvPassTicketTypes> listPassTypes = new List<EmvPassTicketTypes>();
        public static int peopleCount = 1; //for group ticket
        public static int passCount = 0; //for trip/period pass
        public static int dayCount = 0; //rpt
        public static int passLimit = 0; //for trip/period pass
        public static int passLimitMSB = 0;



        public static decimal ticketPrice = 0.0m;
        public static decimal totalPrice = 0.0m;
        //public static decimal change = 0.0m;
        public static MapHeaderType mapHeaderType = MapHeaderType.Unknown;
        public static String journeyTypeText = "";
        public static String ticketTypeText = "";

        //public static String ticketInfoText = "";
        //public static String ticketTotalAmountText = "";

        public static void Clear()
        {
            ticketGUID = string.Empty;
            language = Languages.English;
            transactionDts = DateTime.Now;
            ticketActivateDts = DateTime.Now;
            ticketExpiryDts = DateTime.Now;
            ticketType = TicketType.QR;
            journeyType = JourneyType.Unknown;
            startStation = new Station();
            endStation = new Station();
            ticketCount = 1; //
            sellTicketCount = 0;
            passSubTypeId = 0;
            listPassTypes.Clear();
            passCount = 0;
            dayCount = 0;
            passLimit = 0;
            passLimitMSB = 0;
            peopleCount = 1;
            ticketPrice = 0.0m;
            totalPrice = 0.0m;
            mapHeaderType = MapHeaderType.Unknown;
            journeyTypeText = string.Empty;
            ticketTypeText = string.Empty;
            listTickets.Clear();
            //ticketInfoText = string.Empty;
            //ticketTotalAmountText = string.Empty;
        }
        public static void ManageDTS()
        {
            DateTime dts = DateTime.Now;
            transactionDts = dts;
            ticketActivateDts = dts;
            ticketExpiryDts = new DateTime(dts.Year, dts.Month, dts.Day, 23, 59, 59);
            if (journeyType == JourneyType.Weekend_Pass)
            {
                while (dts.DayOfWeek != DayOfWeek.Saturday)
                {
                    dts = dts.AddDays(1);
                }
                ticketActivateDts = new DateTime(dts.Year, dts.Month, dts.Day, 0, 0, 0);
                while (dts.DayOfWeek != DayOfWeek.Sunday)
                {
                    dts = dts.AddDays(1);
                }
                ticketExpiryDts = new DateTime(dts.Year, dts.Month, dts.Day, 23, 59, 59);
            }
            if (journeyType == JourneyType.Day_Pass)
            {
                ticketActivateDts = dts;
                ticketExpiryDts = dts.AddDays(1);
                //ticketActivateDts = new DateTime(dts.Year, dts.Month, dts.Day, 0, 0, 0);
                //ticketExpiryDts = new DateTime(dts.Year, dts.Month, dts.Day, 23, 59, 59);

            }
            if (journeyType == JourneyType.Period_Pass)
            {
                ticketActivateDts = new DateTime(dts.Year, dts.Month, dts.Day, 0, 0, 0);
                dts = dts.AddDays(dayCount);
                ticketExpiryDts = new DateTime(dts.Year, dts.Month, dts.Day, 23, 59, 59);
            }
        }
        public static void ManageTicketType()
        {
            if (journeyType == JourneyType.SJT || journeyType == JourneyType.RJT || journeyType == JourneyType.Group_Ticket)
                ticketType = TicketType.QR;
            else if (journeyType == JourneyType.Day_Pass || journeyType == JourneyType.Weekend_Pass)
                ticketType = TicketType.RPT;
            else if (journeyType == JourneyType.Trip_Pass || journeyType == JourneyType.Period_Pass)
                ticketType = TicketType.SmartCard;
        }
        public static bool PrepareTicket()
        {
            bool result = false;
            result = SetJurneyTypeText();
            if (result)
            {
                result = SetTicketTypeText();
                if (result)
                {
                    result = CalculateTicketPriceGeneral();
                    //if (result)
                    //{
                    //    ArrangTicketInfo();                      
                    //}
                }
            }
            return result;
        }
        private static bool CalculateTicketPriceGeneral()
        {
            bool result = false;
            switch (ticketType)
            {
                case TicketType.RPT:
                    result = CalculateTicketPriceRPT();
                    break;
                case TicketType.QR:
                    result = CalculateTicketPriceQR();
                    break;
                case TicketType.SmartCard:
                    result = CalculateTicketPricePass();
                    break;
                default:
                    break;
            }
            return result;
        }
        private static bool GetPrice(out decimal? rpResult, String fareMediaCode)
        {
            bool result = false;
            using (var context = new Models.TVM_Entities())
            {               
                decimal? val = 0;
                try
                {
                     val = context.sp_GetPrices(fareMediaCode, journeyType.ToString(), startStation.code, endStation.code, ticketActivateDts, MultiLanguage.GetCurrentLanguage(), passSubTypeId).FirstOrDefault();
                    if (val != null)
                        result = true;
                    //rp = Parameters.db.SelTicketPrice(fareMediaCode, journeyType.ToString(), startStation.code, endStation.code, ticketActivateDts, MultiLanguage.GetCurrentLanguage(), passSubTypeId);
                    //result = (Validation.IsValidRP(rp) && (rp.Data.Tables.Count == 1) && (rp.Result == 0));
                }
                catch (Exception ex)
                {
                    result = false;
                    ticketPrice = 0.00m;
                    totalPrice = 0.00m;
                    log.Error("GetPrice : " + ex.Message);
                }
                rpResult = val;
            }
            return result;
        }
        private static bool CalculateTicketPriceQR()
        {
            decimal? rp = null;
            bool result = false;
            try
            {
                result = GetPrice(out rp, "QRCODE");
                var dtPrices = rp;
                if (result)
                {
                    if (dtPrices != null)
                    {
                        ticketPrice = Convert.ToDecimal(rp);
                        if (journeyType == JourneyType.Group_Ticket)
                            totalPrice = ticketPrice * Convert.ToDecimal(peopleCount);
                        else
                            totalPrice = ticketPrice * Convert.ToDecimal(ticketCount);
                    }
                    else
                    {
                        result = false;
                        ticketPrice = 0.00m;
                        totalPrice = 0.00m;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ticketPrice = 0.00m;
                totalPrice = 0.00m;
                log.Error(": " + ex.Message);
            }
            return result;
        }
        private static bool CalculateTicketPriceRPT()
        {
            decimal? rp = null;
            bool result = false;
            try
            {
                result = GetPrice(out rp, "RFID");
                var dtPrices = rp;
                if (result)
                {
                    if (dtPrices != null)
                    {
                        ticketPrice = Convert.ToDecimal(rp);
                        totalPrice = ticketPrice * Convert.ToDecimal(ticketCount);
                    }
                    else
                    {
                        result = false;
                        ticketPrice = 0.00m;
                        totalPrice = 0.00m;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ticketPrice = 0.00m;
                totalPrice = 0.00m;
                log.Error(": " + ex.Message);
            }
            return result;
        }
        private static bool CalculateTicketPricePass()
        {
            decimal? rp = null;
            bool result = false;
            try
            {
                result = GetPrice(out rp, "QRCODE");
                var dtPrices = rp;
                if (result)
                {
                    if (dtPrices != null)
                    {
                        ticketPrice = Convert.ToDecimal(rp);
                        totalPrice = ticketPrice * Convert.ToDecimal(passCount);
                    }
                    else
                    {
                        result = false;
                        ticketPrice = 0.00m;
                        totalPrice = 0.00m;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ticketPrice = 0.00m;
                totalPrice = 0.00m;
                log.Error(": " + ex.Message);
            }
            return result;
        }
        private static bool SetTicketTypeText()
        {
            bool result = false;
            try
            {
                switch (ticketType)
                {
                    case TicketType.QR:
                        ticketTypeText = "QR Code Ticket";
                        break;
                    case TicketType.RPT:
                        ticketTypeText = "RFID Paper Ticket";
                        break;
                    case TicketType.SmartCard:
                        ticketTypeText = "Ticket Load Smart Card";
                        break;
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        private static bool SetJurneyTypeText()
        {
            bool result = false;
            try
            {
                switch (journeyType)
                {
                    case JourneyType.SJT:
                        journeyTypeText = MultiLanguage.GetText("SJT");
                        break;
                    case JourneyType.RJT:
                        journeyTypeText = MultiLanguage.GetText("RJT");
                        break;
                    case JourneyType.Group_Ticket:
                        journeyTypeText = MultiLanguage.GetText("GRO");
                        break;
                    case JourneyType.EventTicket:
                        journeyTypeText = "EVENT TICKET";
                        break;
                    case JourneyType.Day_Pass:
                        journeyTypeText = MultiLanguage.GetText("onedaypass");
                        break;
                    case JourneyType.Weekend_Pass:
                        journeyTypeText = MultiLanguage.GetText("weekenddaypass");
                        break;
                    case JourneyType.Trip_Pass:
                        journeyTypeText = "TRIP PASS";
                        break;
                    case JourneyType.Period_Pass:
                        journeyTypeText = "PERIOD PASS";
                        break;
                    case JourneyType.Topup:
                        journeyTypeText = "TOP UP";
                        break;
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public static bool AddTicketsToList(DataTable dt)
        {
            bool result = false;
            try
            {
                listTickets.Clear();
                for (var ii = 0; ii < dt.Rows.Count; ii++)
                {
                    var list = new TSelectedTickets
                    {
                        ticketType = Convert.ToInt32(dt.Rows[ii]["ticketType"]),
                        price = Convert.ToDecimal(dt.Rows[ii]["ticketPrice"]),
                        FromId = Convert.ToInt32(dt.Rows[ii]["fromId"]),
                        ToId = Convert.ToInt32(dt.Rows[ii]["toId"]),
                        activeFrom = Convert.ToDateTime(dt.Rows[ii]["activeFrom"]),
                        activeTo = Convert.ToDateTime(dt.Rows[ii]["activeTo"]),
                        TicketGUID = dt.Rows[ii]["QRCodeId"].ToString(),
                        peopleCount = Convert.ToInt16(dt.Rows[ii]["peopleCount"]),
                        alias = Convert.ToInt32(dt.Rows[ii]["alias"]),
                        From = dt.Rows[ii]["from"].ToString(),
                        To = dt.Rows[ii]["to"].ToString()

                    };
                    switch (list.ticketType)
                    {
                        case (int)JourneyType.Group_Ticket:
                            list.explanation = "Group Ticket";
                            break;

                        case (int)JourneyType.RJT:
                            list.explanation = "RJT";
                            break;
                        case (int)JourneyType.SJT:
                            list.explanation = "SJT";
                            break;
                    }
                    listTickets.Add(list);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("AddTicketsToList : " + ex.ToString());
            }
            return result;
        }
        public static void PassTypeList(int type)
        {
            try
            {
                listPassTypes.Clear();

                using (var context = new Models.TVM_Entities())
                {
                    var val = context.sp_SelEmvPass(type).ToList().OrderBy(x => x.tripCount);

                    foreach(var item in val)
                    {
                        var emvTicket = new EmvPassTicketTypes
                        {
                            Explanation = item.explanation.ToString(),
                            Id = Convert.ToInt32(item.recId),
                            TripCount = Convert.ToInt32(item.tripCount),
                            Validity = Convert.ToDateTime(DateTime.Now.AddDays(Convert.ToInt32(item.validDayCount) - 1).ToString("dd/MM/yyyy")).AddHours(23).AddMinutes(59).AddSeconds(59)
                        };
                        listPassTypes.Add(emvTicket);
                    }
                }
            }
            catch (Exception Ex)
            {
            }
        }
    }

    public class TSelectedTickets : IComparable<TSelectedTickets>
    {
        public DateTime activeFrom = DateTime.Now; /*Geçerlilik Başlangıç Tarih*/
        public DateTime activeTo = DateTime.Now; /*Geçerlilik Son Tarih*/
        public int alias; /*Bilet için alias*/
        public decimal amount = 0.00m; /*Toplam tutar = Birim Fiyat * Kisi*/
        public int count = 0; /*Bilet sayisi*/
        public string explanation = string.Empty; /*Bilet aciklamasi*/
        public string From = string.Empty; /*From aciklamasi*/
        public int FromId = 0; /*From Id*/
        public string FromDesc = string.Empty;
        public string ToDesc = string.Empty;
        public string fareMediaType = string.Empty;
        public short peopleCount = 1; /*Grup Bileti İçin*/
        public decimal price = 0.00m; /*Birim Fiyat*/
        public int rowNo = 0; /*Satir numarasi*/
        public DateTime ticketDT = DateTime.Now; /*Bilet basım tarihi*/
        public string TicketGUID = string.Empty;
        public int ticketType = 0; /*Bilet tipi*/
        public string To = string.Empty; /*To aciklamasi*/
        public int ToId = 0; /*To Id*/
        public int weekendPassType = 0;
        public int recId = 0;
        public int tranType = 0;
        public int CompareTo(TSelectedTickets value)
        {
            return price.CompareTo(value.price);
        }
    }
}
