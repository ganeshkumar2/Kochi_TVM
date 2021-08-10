using Kochi_TVM.Business;
using Kochi_TVM.Models;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.RptDispenser
{
    public class RPTOperations
    {
        private static ILog log = LogManager.GetLogger(typeof(RPTOperations).Name);
        static bool isInit = false;

        public static bool Init()
        {
            bool result = true;
            try
            {
                if (!isInit)
                {
                    isInit = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error : " + ex.ToString());
            }
            return result;
        }

        static bool Move()
        {
            return Dispenser.Instance.Move();
        }

        static bool Reject()
        {
            return Dispenser.Instance.Reject();
        }

        static bool Trash()
        {
            return Dispenser.Instance.Trash();
        }

        static bool Personalize(out byte[] uid)
        {
            bool result = false;
            object ticketObject = null;

            switch (Ticket.journeyType)
            {
                case JourneyType.SJT:
                    ticketObject = new TicketSJT { stationIn = Convert.ToByte(Ticket.startStation.id), stationOut = Convert.ToByte(Ticket.endStation.id), price = Convert.ToUInt16(Ticket.ticketPrice) };
                    break;
                case JourneyType.RJT:
                    ticketObject = new TicketRJT { stationInOut = Convert.ToByte(Ticket.startStation.id), stationOutIn = Convert.ToByte(Ticket.endStation.id), price = Convert.ToUInt16(Ticket.ticketPrice) };
                    break;
                case JourneyType.Day_Pass:
                    ticketObject = new TicketDayPass { institutionId = Convert.ToByte(Constants.ApplicationVersion), price = Convert.ToUInt16(Ticket.ticketPrice) };
                    break;
                case JourneyType.Weekend_Pass:
                    ticketObject = new TicketWeekendPass { institutionId = Convert.ToByte(Constants.ApplicationVersion), passType = Convert.ToByte(0), price = Convert.ToUInt16(Ticket.ticketPrice) };
                    break;
                case JourneyType.Group_Ticket:
                    ticketObject = new TicketGroup { stationIn = Convert.ToByte(Ticket.startStation.id), stationOut = Convert.ToByte(Ticket.endStation.id), peopleCount = Convert.ToByte(Ticket.peopleCount), price = Convert.ToUInt16(Ticket.ticketPrice) };
                    break;
                case JourneyType.EventTicket:
                    ticketObject = new TicketEventPass { startDT = Ticket.ticketActivateDts, endDT = Ticket.ticketExpiryDts, price = Convert.ToUInt16(Ticket.ticketPrice) };
                    break;
            }

            log.Debug("pass ticketObject TicketDayPass : "+ ticketObject.ToString() + " startStation id - " + Ticket.startStation.id + " startStation byte - " + Convert.ToByte(Ticket.startStation.id) + " institutionId "+ Constants.ApplicationVersion + " institutionByte " + Convert.ToByte(Constants.ApplicationVersion));

            var persoSector = new RPTPersoSector(Convert.ToByte(Stations.currentStation.id), DateTime.Now, ticketObject);

            result = Dispenser.Instance.Personalize(out uid, persoSector);

            return result;
        }

        public static bool GiveRPTTicket()
        {
            bool ret = false, result = false;
            try
            {
                byte[] uid = null;
                log.Debug("pass GiveRPTTicket");
                if (Init())
                {
                    log.Debug("pass Start");
                    if (InsTicketTranDet(out long transactionId))
                    {
                        log.Debug("pass InsTicketTranDet");
                        for (int i = 0; i < Ticket.ticketCount; i++)
                        {
                            ret = false;
                            uid = null;
                            if (Move())
                            {
                                log.Debug("pass Move");
                                if (Personalize(out uid))
                                {
                                    ret = true;
                                    log.Debug("pass Personalize");
                                    if (Reject())
                                    {
                                        log.Debug("pass Reject");
                                        InsRFIDTicket(transactionId, ret, uid);
                                        result = true;
                                    }
                                    else
                                        Trash();
                                }
                                else
                                    Trash();
                            }
                        }

                        if (result)
                            using (var context = new TVM_Entities())
                            {
                                var courses = context.UpdTransactionStatus(transactionId).ToList();
                            }
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                log.Error("LogTypes.Error GiveRPTTicket() : " + ex.ToString());
                return result;
            }            
        }
        public static bool InsTicketTranDet(out long transactionId)
        {
            bool result = false;
            long tempTransactionId = 0;
            try
            {
                using (var context = new TVM_Entities())
                {

                    int unitId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId").ToString());
                    int stationId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId"));
                    int? courses = 0;
                    courses = context.sp_InsTicketTransaction(0, 0, Ticket.totalPrice, Convert.ToByte(Utils.Enums.TransactionType.TT_RPT),
                                    (byte)PaymentType.cash, Ticket.totalPrice, 0, Ticket.totalPrice,
                                    0,
                                    Constants.Change,
                                   0, unitId, tempTransactionId,
                                    stationId, 0).FirstOrDefault();

                    //var courses = context.sp_InsTicketTransaction(i, j, k, Convert.ToByte(Utils.Enums.TransactionType.TT_RPT), (byte)PaymentType.cash, Ticket.totalPrice, 0, Ticket.totalPrice,
                    //    0, Constants.Change,
                    //    0, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), tempTransactionId,
                    //    Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId")), 0);
                    
                    tempTransactionId = (int)courses;

                    if (tempTransactionId > 0)
                        result = true;
                    else
                        result = false;
                    //result = true;
                }                    
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error " + ex.ToString());
            }
            transactionId = tempTransactionId;
            return result;
        }
        public static bool InsRFIDTicket(long transactionId, bool ret, byte[] uid)
        {
            bool result = false;
            var cardId = "";
            try
            {
                cardId = BitConverter.ToString(uid).Replace("-", string.Empty);
                using (var context = new TVM_Entities())
                {
                    int courses = context.sp_InsRFIDTicket((int)Ticket.journeyType, Ticket.ticketPrice, Ticket.startStation.id, Ticket.endStation.id, 0, Ticket.ticketActivateDts, Ticket.ticketExpiryDts, cardId, (short)Ticket.peopleCount, "N", Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId")), Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), Convert.ToInt32(transactionId), 0);
                    
                    //if(courses >= 0)
                        result = true;
                }               
                
            }
            catch (Exception ex)
            {
                result = false;
                log.Debug("LogTypes.Error.ToString() : " + ex.ToString());
            }
            return result;
        }

        public static void GetStatus(ref DISP_STAT status)
        {
            bool result = false;
            result = Init();
            if (result)
            {
                result = Dispenser.Instance.GetStackerStatus(ref status);
                if (!result)
                {
                    status = DISP_STAT.STACKER_UNKNOWN;
                    ResetDispenser();
                }
                else
                {
                    if (status == DISP_STAT.STACKER_UNKNOWN)
                        ResetDispenser();
                }
                if (result && status != DISP_STAT.STACKER_UNKNOWN)
                {

                    if (!GetSAMSerialNo())
                        status = DISP_STAT.STACKER_UNKNOWN;
                }
            }
        }
        public static void ResetDispenser()
        {

            Done();
            Thread.Sleep(100);
            Init();

        }
        public static bool Done()
        {
            bool result = true;
            try
            {
                isInit = false;
                Dispenser.Instance.Done();
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error : " + ex.ToString());
            }

            return result;
        }
        public static bool GetSAMSerialNo()
        {
            bool result = false;
            var errSam = LIB_ERR.NO_ERROR;
            var samId = new byte[7];
            try
            {
                result = Dispenser.Instance.GetSAMSerialNo(ref samId, ref errSam);
            }
            catch (Exception ex)
            {

            }

            return result;
        }
        public static bool IsCardInRFCardOperationPosition()
        {
            return Dispenser.Instance.IsCardInRFCardOperationPosition();
        }
    }
}
