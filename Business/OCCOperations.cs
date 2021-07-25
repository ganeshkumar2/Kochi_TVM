using KioskFramework.OccSrv;
using Kochi_TVM.Models;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;
using TransactionType = Kochi_TVM.Utils.Enums.TransactionType;

namespace Kochi_TVM.Business
{
    public class OCCOperations
    {
        private static ILog log = LogManager.GetLogger(typeof(OCCOperations).Name);
        private static bool InsTicketTransactionOcc(out ReturnPackage rpSub)
        {
            bool result = false;
            ReturnPackage rpTemp = null;

            try
            {
                log.Debug("unitID =" + Parameters.TVMDynamic.GetParameter("unitId") + "InsTicketTransactionOcc 22.");
                int stationId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId"));
                int unitId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId").ToString());
                rpTemp = Parameters.occ.InsTicketTransaction(0, 0, Ticket.totalPrice, Convert.ToInt16(TransactionType.TT_QR),
                   (short)PaymentType.cash, Ticket.totalPrice, 0, Ticket.totalPrice, 0, Constants.Change,
                   0, unitId, stationId);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() " + ex.ToString());

            }
            rpSub = rpTemp;
            return result;
        }
        private static bool InsQRCodeTicketOcc(out ReturnPackage rp, DataTable dt, long transactionId)
        {
            bool result = false;
            ReturnPackage rpTemp = null;
            try
            {
                log.Debug("unitID =" + Parameters.TVMDynamic.GetParameter("unitId") + "InsTicketTransactionOcc 46.");
                rpTemp = Parameters.occ.InsQRCodeTicket(dt, "0", Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId")), Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId").ToString()), (int)transactionId, 0, 0);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() " + ex.ToString());
            }
            rp = rpTemp;
            return result;
        }
        private static bool InsOcc(out long transactionId, out DataTable dt)
        {
            bool result = false;
            long transactionIdTemp = 0;
            DataTable dtTemp = null;
            log.Debug("LogTypes.Info.ToString() Start InsOcc");
            try
            {
                result = InsTicketTransactionOcc(out ReturnPackage rpSub);
                if (result)
                {
                    log.Debug("LogTypes.Info.ToString()  InsTicketTransactionOcc return true ");
                    result = Validation.IsValidOCCRP(rpSub) && (rpSub.Result == 0);
                    if (result)
                    {
                        log.Debug("LogTypes.Info.ToString() InsTicketTransactionOcc return valid data");
                        transactionIdTemp = Convert.ToInt64(rpSub.Data.Tables[0].Rows[0]["recId"]);
                        if (transactionIdTemp > 0)
                        {
                            log.Debug("LogTypes.Info.ToString() InsTicketTransactionOcc return transactionId >0 ");
                            result = GetTicketTable(out dtTemp);
                            if (result)
                            {
                                log.Debug("LogTypes.Info.ToString() GetTicketTable return true");
                                ReturnPackage rp = null;
                                result = InsQRCodeTicketOcc(out rp, dtTemp, transactionIdTemp);
                                if (result)
                                {
                                    log.Debug("LogTypes.Info.ToString() InsQRCodeTicketOcc return true ");
                                    result = (Validation.IsValidOCCRP(rp) && (rp.Result == 0));
                                    if (result)
                                    {
                                        log.Debug("LogTypes.Info.ToString() InsQRCodeTicketOcc return valid data");
                                        Ticket.ticketActivateDts = Convert.ToDateTime(rp.Data.Tables[0].Rows[0]["qrCreateDate"]);
                                        Ticket.dayCount = Convert.ToInt32(rp.Data.Tables[0].Rows[0]["dayCount"]);
                                    }
                                    else
                                    {
                                        result = false;
                                        log.Debug("LogTypes.Warning.ToString() InsQRCodeTicketOcc return invalid data");
                                    }
                                }
                                else
                                {
                                    result = false;
                                    log.Debug("LogTypes.Warning.ToString() InsQRCodeTicketOcc return false");
                                }
                            }
                            else
                            {
                                result = false;
                                log.Debug("LogTypes.Warning.ToString() GetTicketTable return false");
                            }
                        }
                        else
                        {
                            result = false;
                            log.Debug("LogTypes.Warning.ToString() InsTicketTransactionOcc return transactionId=0");
                        }
                    }
                    else
                    {
                        result = false;
                        log.Debug("LogTypes.Warning.ToString() InsTicketTransactionOcc return invalid data");
                    }
                }
                else
                {
                    result = false;
                    log.Debug("LogTypes.Warning.ToString() InsTicketTransactionOcc return false");
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() " + ex.ToString());
                log.Error("Error:: row position....");
            }
            transactionId = transactionIdTemp;
            dt = dtTemp;
            return result;
        }
        private static bool InsTicketTransactionLocal(long transactionId)
        {
            bool result = false;
            try
            {
                log.Debug("unitID =" + Parameters.TVMDynamic.GetParameter("unitId") + "InsTicketTransactionLocal 144.");
                int unitId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId").ToString());
                int stationId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId"));
                using (var context = new TVM_Entities())
                {
                    var data = context.sp_InsTicketTransaction(0, 0, Ticket.totalPrice, Convert.ToByte(TransactionType.TT_QR),
                                    (byte)PaymentType.cash, Ticket.totalPrice, 0, Ticket.totalPrice,
                                    0,
                                    Constants.Change,
                                   0, unitId, transactionId,
                                    stationId, 0);
                    result = true;
                }                
            }
            catch (Exception ex)
            {
                log.Error("LogTypes.Error.ToString() " + ex.Message + Environment.NewLine + ex.StackTrace);
                result = false;
            }
            return result;
        }
        private static bool InsQRLocal(long transactionId, DataTable dt)
        {
            bool result = false;
            try
            {
                
                log.Debug("unitID =" + Parameters.TVMDynamic.GetParameter("unitId").ToString() + "InsQRLocal 167.");
                int unitId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId").ToString());
                int stationId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId"));
                using (var context = new TVM_Entities())
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        log.Debug("ticketType =" + dt.Rows[i]["ticketType"]);
                        var data = context.sp_InsQRCodeTicket(Convert.ToInt16(dt.Rows[i]["ticketType"]), Convert.ToDecimal(dt.Rows[i]["ticketPrice"]), Convert.ToInt16(dt.Rows[i]["fromId"]), Convert.ToInt16(dt.Rows[i]["toId"]), Convert.ToDateTime(dt.Rows[i]["activeFrom"]), Convert.ToDateTime(dt.Rows[i]["activeTo"]),
                            Convert.ToString(dt.Rows[i]["QRCodeId"]),(short)Ticket.peopleCount, Convert.ToInt16(dt.Rows[i]["alias"]), Convert.ToString(dt.Rows[i]["from"]), Convert.ToString(dt.Rows[i]["to"]), Convert.ToString(dt.Rows[i]["ticketStringValue"]), "0", stationId, unitId, (int)transactionId, 0, Ticket.ticketActivateDts);
                    }
                    result = true;
                }
                result = true;
            }
            catch (Exception ex)
            {
                log.Error("LogTypes.Error.ToString() " + ex.Message + Environment.NewLine + ex.StackTrace);
                result = false;
            }
            return result;
        }
        private static bool InsLocal(long transactionId, DataTable dt)
        {
            bool result = false;
            try
            {
                result = InsTicketTransactionLocal(transactionId);
                if (result)
                {
                    log.Debug("LogTypes.Info.ToString() InsTicketTransactionLocal return true ");
                    result = InsQRLocal(transactionId, dt);
                    if (result)
                    {
                        log.Debug("LogTypes.Info.ToString() InsQRLocal return true ");
                    }
                    else
                    {
                        result = false;
                        log.Debug("LogTypes.Warning.ToString() InsQRLocal return false");
                    }
                }
                else
                {
                    result = false;
                    log.Debug("LogTypes.Warning.ToString() InsTicketTransactionLocal return false");
                }
            }
            catch (Exception ex)
            {
                log.Error("LogTypes.Error.ToString() " + ex.ToString());
                result = false;
            }
            return result;
        }
        public static bool InsertQRTransaction()
        {
            log.Debug("LogTypes.Info.ToString() Start InsertQRTransaction");
            bool result = false;
            long transactionId = 0;
            DataTable dt = null;
            try
            {                
                if (QRPrinter.Instance.CheckQrPrinterStatus() == Enums.PRINTER_STATE.OK)
                    result = true;
                else
                    result = false;

                if (result)
                {
                    //#if DEBUG
                    //                    return true;
                    //#endif
                    result = InsOcc(out transactionId, out dt);
                    if (result)
                    {
                        log.Debug("LogTypes.Info.ToString() InsOcc return true ");
                        result = InsLocal(transactionId, dt);
                        if (result)
                        {
                            log.Debug("LogTypes.Info.ToString() InsLocal return true ");
                            result = Ticket.AddTicketsToList(dt);
                            if (result)
                            {
                                log.Debug("LogTypes.Info.ToString() AddTicketsToList return true ");
                            }
                            else
                            {
                                log.Debug("LogTypes.Warning.ToString() AddTicketsToList return false ");
                            }
                        }
                        else
                        {
                            log.Debug("LogTypes.Warning.ToString() InsLocal return false ");
                        }
                    }
                    else
                    {
                        log.Debug("LogTypes.Warning.ToString()  InsOcc return false ");
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.ToString());
            }
            return result;
        }
        static bool GetAlias(out ReturnPackage rp)
        {
            bool result = false;
            ReturnPackage rpTemp = null;
            try
            {
                rpTemp = (Parameters.occ.ExecSP("def.sp_SelAlias"));
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.ToString());
            }
            rp = rpTemp;
            return result;
        }
        static DataTable CreateTable()
        {
            DataTable table = null;
            try
            {
                table = new DataTable { TableName = "TicketsTable" };
                table.Columns.Add("ticketType", typeof(int));
                table.Columns.Add("ticketPrice", typeof(decimal));
                table.Columns.Add("fromId", typeof(int));
                table.Columns.Add("toId", typeof(int));
                table.Columns.Add("activeFrom", typeof(DateTime));
                table.Columns.Add("activeTo", typeof(DateTime));
                table.Columns.Add("QRCodeId", typeof(string));
                table.Columns.Add("peopleCount", typeof(short));
                table.Columns.Add("alias", typeof(int));
                table.Columns.Add("from", typeof(string));
                table.Columns.Add("to", typeof(string));
                table.Columns.Add("ticketStringValue", typeof(string));
                //table.Columns.Add("fromRouteNo", typeof(int));
                //table.Columns.Add("toRouteNo", typeof(int));
            }
            catch (Exception ex)
            {
                log.Error("LogTypes.Error.ToString() " + ex.ToString());
            }
            return table;
        }
        static bool FillTable(out DataTable table, int ticketAlias)
        {
            bool result = false;
            DataTable tableTemp = null;
            try
            {
                tableTemp = CreateTable();
                int ticketType;
                decimal ticketPrice;
                int fromId;
                int toId;
                decimal price;
                string ticketGUID;
                DateTime activeTo;
                short peopleCount;
                string from;
                string to;
                //int fromRouteNo;
                //int toRouteNo;
                var alias = ticketAlias;

                for (int j = 0; j < Ticket.ticketCount; j++)
                {
                    ticketType = (int)Ticket.journeyType;
                    ticketPrice = (Ticket.journeyType == JourneyType.Group_Ticket) ? (Ticket.totalPrice) : Ticket.ticketPrice;//insert db
                    fromId = Ticket.startStation.id;
                    toId = Ticket.endStation.id;
                    var activeFrom = Ticket.ticketActivateDts;
                    activeTo = Ticket.ticketExpiryDts;
                    peopleCount = (short)Ticket.peopleCount;
                    price = Ticket.ticketPrice;//to GenerateQR
                    from = Ticket.startStation.name;
                    to = Ticket.endStation.name;
                    //fromRouteNo = 0;
                    //toRouteNo = 0;
                    string ticketStringValue = String.Empty;
                    log.Debug("unitID =" + Parameters.TVMDynamic.GetParameter("unitId").ToString() + "FillTable 344.");
                    Ticket.ticketGUID = ticketGUID = QROperations.GenerateQR(Ticket.transactionDts, ticketType, fromId, toId, peopleCount, activeFrom, activeTo, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId").ToString()), alias, price, ref ticketStringValue);
                    tableTemp.Rows.Add(ticketType, ticketPrice, fromId, toId, activeFrom, activeTo, ticketGUID, peopleCount, alias, from, to, ticketStringValue);
                    //tableTemp.Rows.Add(ticketType, ticketPrice, fromId, toId, activeFrom, activeTo, ticketGUID, peopleCount, alias, from, to, ticketStringValue, fromRouteNo, toRouteNo);
                    alias = alias + 1;
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.ToString());
            }
            table = tableTemp;
            return result;
        }
        static bool GetTicketTable(out DataTable dt)
        {
            bool result = false;
            DataTable table = null;
            int alias = 0;
            try
            {

                result = GetAliasLocal(out alias);
                if (result)
                {
                    log.Debug("LogTypes.Info.ToString() GetAlias return true");
                    result = FillTable(out table, alias);
                    if (result)
                    {
                        log.Debug("LogTypes.Info.ToString() FillTable return true");
                    }
                    else
                    {
                        result = false;
                        log.Debug("LogTypes.Warning.ToString() FillTable return false");
                    }
                }
                else
                {
                    result = false;
                    log.Debug("LogTypes.Warning.ToString() GetAlias return false");
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() " + ex.ToString());
            }
            dt = table;
            return result;
        }

        static bool GetAliasLocal(out int alias)
        {
            bool result = false;
            try
            {
                using (var context = new TVM_Entities())
                {
                    int? data = context.sp_SelAlias().FirstOrDefault();
                    log.Debug("Debug - GetAliasLocal() : " + data.ToString());
                    alias = Convert.ToInt16(data);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                alias = 0;
                log.Error("LogTypes.Error.ToString() : " + ex.ToString());
            }      

            return result;
        }
    }
}
