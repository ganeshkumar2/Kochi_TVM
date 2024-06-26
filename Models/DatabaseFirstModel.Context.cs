﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kochi_TVM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class TVM_Entities : DbContext
    {
        public TVM_Entities()
            : base("name=TVM_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<StockType> StockTypes { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<UpdateType> UpdateTypes { get; set; }
        public virtual DbSet<MoneyDetails1> MoneyDetails1 { get; set; }
        public virtual DbSet<MoneyStatu> MoneyStatus { get; set; }
        public virtual DbSet<QRCodeTicket> QRCodeTickets { get; set; }
        public virtual DbSet<StockDetail> StockDetails { get; set; }
        public virtual DbSet<StockStatu> StockStatus { get; set; }
        public virtual DbSet<TicketTransactionDetail> TicketTransactionDetails { get; set; }
        public virtual DbSet<Transactions1> Transactions1 { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Command> Commands { get; set; }
        public virtual DbSet<ErrorCode> ErrorCodes { get; set; }
        public virtual DbSet<InfoLog> InfoLogs { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Limit> Limits { get; set; }
        public virtual DbSet<MoneyDetail> MoneyDetails { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<ResourceText> ResourceTexts { get; set; }
        public virtual DbSet<ResourceType> ResourceTypes { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<TicketDetail> TicketDetails { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionType1> TransactionTypes1 { get; set; }
        public virtual DbSet<TVMInfo> TVMInfoes { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<FareMediaType> FareMediaTypes { get; set; }
        public virtual DbSet<FareTable> FareTables { get; set; }
        public virtual DbSet<SalePointDevice> SalePointDevices { get; set; }
        public virtual DbSet<SalePoint> SalePoints { get; set; }
        public virtual DbSet<Stations1> Stations1 { get; set; }
        public virtual DbSet<SystemParam> SystemParams { get; set; }
        public virtual DbSet<TicketSubType> TicketSubTypes { get; set; }
        public virtual DbSet<TicketType> TicketTypes { get; set; }
        public virtual DbSet<RFIDTicket> RFIDTickets { get; set; }
        public virtual DbSet<Adjustment> Adjustments { get; set; }
        public virtual DbSet<PenaltyTicket> PenaltyTickets { get; set; }
        public virtual DbSet<Refund> Refunds { get; set; }
        public virtual DbSet<Replacement> Replacements { get; set; }
        public virtual DbSet<SyncUpdDate> SyncUpdDates { get; set; }
        public virtual DbSet<Topup> Topups { get; set; }
        public virtual DbSet<TripTopup> TripTopups { get; set; }
        public virtual DbSet<Authority> Authorities { get; set; }
        public virtual DbSet<AuthTran> AuthTrans { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<UserAuth> UserAuths { get; set; }
        public virtual DbSet<UserAuth_test> UserAuth_test { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<CancelTicket> CancelTickets { get; set; }
        public virtual DbSet<FreeExitTicket> FreeExitTickets { get; set; }
        public virtual DbSet<TVMAlarm> TVMAlarms { get; set; }
    
        public virtual int GetSequence(string key, ObjectParameter value)
        {
            var keyParameter = key != null ?
                new ObjectParameter("Key", key) :
                new ObjectParameter("Key", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetSequence", keyParameter, value);
        }
    
        public virtual ObjectResult<Nullable<decimal>> sp_GetPrices(string fareMediaCode, string fareMediaType, string fromValue, string toValue, Nullable<System.DateTime> ticketDate, string lang, Nullable<int> ticketSubTypeId)
        {
            var fareMediaCodeParameter = fareMediaCode != null ?
                new ObjectParameter("fareMediaCode", fareMediaCode) :
                new ObjectParameter("fareMediaCode", typeof(string));
    
            var fareMediaTypeParameter = fareMediaType != null ?
                new ObjectParameter("fareMediaType", fareMediaType) :
                new ObjectParameter("fareMediaType", typeof(string));
    
            var fromValueParameter = fromValue != null ?
                new ObjectParameter("fromValue", fromValue) :
                new ObjectParameter("fromValue", typeof(string));
    
            var toValueParameter = toValue != null ?
                new ObjectParameter("toValue", toValue) :
                new ObjectParameter("toValue", typeof(string));
    
            var ticketDateParameter = ticketDate.HasValue ?
                new ObjectParameter("ticketDate", ticketDate) :
                new ObjectParameter("ticketDate", typeof(System.DateTime));
    
            var langParameter = lang != null ?
                new ObjectParameter("lang", lang) :
                new ObjectParameter("lang", typeof(string));
    
            var ticketSubTypeIdParameter = ticketSubTypeId.HasValue ?
                new ObjectParameter("ticketSubTypeId", ticketSubTypeId) :
                new ObjectParameter("ticketSubTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("sp_GetPrices", fareMediaCodeParameter, fareMediaTypeParameter, fromValueParameter, toValueParameter, ticketDateParameter, langParameter, ticketSubTypeIdParameter);
        }
    
        public virtual ObjectResult<sp_SelEmvPass_Result> sp_SelEmvPass(Nullable<int> ticketTypeId)
        {
            var ticketTypeIdParameter = ticketTypeId.HasValue ?
                new ObjectParameter("ticketTypeId", ticketTypeId) :
                new ObjectParameter("ticketTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelEmvPass_Result>("sp_SelEmvPass", ticketTypeIdParameter);
        }
    
        public virtual ObjectResult<sp_SelEmvPassTypes_Result> sp_SelEmvPassTypes(Nullable<int> ticketTypeId)
        {
            var ticketTypeIdParameter = ticketTypeId.HasValue ?
                new ObjectParameter("ticketTypeId", ticketTypeId) :
                new ObjectParameter("ticketTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelEmvPassTypes_Result>("sp_SelEmvPassTypes", ticketTypeIdParameter);
        }
    
        public virtual ObjectResult<sp_SelSalePointByMac_Result> sp_SelSalePointByMac(string macAddress)
        {
            var macAddressParameter = macAddress != null ?
                new ObjectParameter("macAddress", macAddress) :
                new ObjectParameter("macAddress", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelSalePointByMac_Result>("sp_SelSalePointByMac", macAddressParameter);
        }
    
        public virtual ObjectResult<sp_SelStations_Result> sp_SelStations(string lang)
        {
            var langParameter = lang != null ?
                new ObjectParameter("lang", lang) :
                new ObjectParameter("lang", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelStations_Result>("sp_SelStations", langParameter);
        }
    
        public virtual ObjectResult<sp_SelSysParams_Result> sp_SelSysParams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelSysParams_Result>("sp_SelSysParams");
        }
    
        public virtual int sp_InsMoney(Nullable<long> trxId, Nullable<int> stockType, Nullable<int> deviceType, Nullable<int> updType, Nullable<decimal> amount)
        {
            var trxIdParameter = trxId.HasValue ?
                new ObjectParameter("trxId", trxId) :
                new ObjectParameter("trxId", typeof(long));
    
            var stockTypeParameter = stockType.HasValue ?
                new ObjectParameter("stockType", stockType) :
                new ObjectParameter("stockType", typeof(int));
    
            var deviceTypeParameter = deviceType.HasValue ?
                new ObjectParameter("deviceType", deviceType) :
                new ObjectParameter("deviceType", typeof(int));
    
            var updTypeParameter = updType.HasValue ?
                new ObjectParameter("updType", updType) :
                new ObjectParameter("updType", typeof(int));
    
            var amountParameter = amount.HasValue ?
                new ObjectParameter("amount", amount) :
                new ObjectParameter("amount", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_InsMoney", trxIdParameter, stockTypeParameter, deviceTypeParameter, updTypeParameter, amountParameter);
        }
    
        public virtual int sp_InsMoneyManagementDetail(string ticketType, Nullable<int> addAmount, Nullable<int> lessAmount, Nullable<int> emptyAmount, Nullable<int> stationId, Nullable<int> salePointId, Nullable<int> userId)
        {
            var ticketTypeParameter = ticketType != null ?
                new ObjectParameter("ticketType", ticketType) :
                new ObjectParameter("ticketType", typeof(string));
    
            var addAmountParameter = addAmount.HasValue ?
                new ObjectParameter("addAmount", addAmount) :
                new ObjectParameter("addAmount", typeof(int));
    
            var lessAmountParameter = lessAmount.HasValue ?
                new ObjectParameter("lessAmount", lessAmount) :
                new ObjectParameter("lessAmount", typeof(int));
    
            var emptyAmountParameter = emptyAmount.HasValue ?
                new ObjectParameter("emptyAmount", emptyAmount) :
                new ObjectParameter("emptyAmount", typeof(int));
    
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var salePointIdParameter = salePointId.HasValue ?
                new ObjectParameter("salePointId", salePointId) :
                new ObjectParameter("salePointId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_InsMoneyManagementDetail", ticketTypeParameter, addAmountParameter, lessAmountParameter, emptyAmountParameter, stationIdParameter, salePointIdParameter, userIdParameter);
        }
    
        public virtual int sp_InsQRCodeTicket(Nullable<int> ticketType, Nullable<decimal> ticketPrice, Nullable<int> fromId, Nullable<int> toId, Nullable<System.DateTime> activeFrom, Nullable<System.DateTime> activeTo, string qRCodeId, Nullable<short> peopleCount, Nullable<int> alias, string from, string to, string ticketStringValue, string qRCreateMode, Nullable<int> qRCreateStation, Nullable<int> qRCreateSalePointId, Nullable<int> transactionId, Nullable<int> qRCreateUserId, Nullable<System.DateTime> insDT)
        {
            var ticketTypeParameter = ticketType.HasValue ?
                new ObjectParameter("ticketType", ticketType) :
                new ObjectParameter("ticketType", typeof(int));
    
            var ticketPriceParameter = ticketPrice.HasValue ?
                new ObjectParameter("ticketPrice", ticketPrice) :
                new ObjectParameter("ticketPrice", typeof(decimal));
    
            var fromIdParameter = fromId.HasValue ?
                new ObjectParameter("fromId", fromId) :
                new ObjectParameter("fromId", typeof(int));
    
            var toIdParameter = toId.HasValue ?
                new ObjectParameter("toId", toId) :
                new ObjectParameter("toId", typeof(int));
    
            var activeFromParameter = activeFrom.HasValue ?
                new ObjectParameter("activeFrom", activeFrom) :
                new ObjectParameter("activeFrom", typeof(System.DateTime));
    
            var activeToParameter = activeTo.HasValue ?
                new ObjectParameter("activeTo", activeTo) :
                new ObjectParameter("activeTo", typeof(System.DateTime));
    
            var qRCodeIdParameter = qRCodeId != null ?
                new ObjectParameter("QRCodeId", qRCodeId) :
                new ObjectParameter("QRCodeId", typeof(string));
    
            var peopleCountParameter = peopleCount.HasValue ?
                new ObjectParameter("peopleCount", peopleCount) :
                new ObjectParameter("peopleCount", typeof(short));
    
            var aliasParameter = alias.HasValue ?
                new ObjectParameter("alias", alias) :
                new ObjectParameter("alias", typeof(int));
    
            var fromParameter = from != null ?
                new ObjectParameter("from", from) :
                new ObjectParameter("from", typeof(string));
    
            var toParameter = to != null ?
                new ObjectParameter("to", to) :
                new ObjectParameter("to", typeof(string));
    
            var ticketStringValueParameter = ticketStringValue != null ?
                new ObjectParameter("ticketStringValue", ticketStringValue) :
                new ObjectParameter("ticketStringValue", typeof(string));
    
            var qRCreateModeParameter = qRCreateMode != null ?
                new ObjectParameter("QRCreateMode", qRCreateMode) :
                new ObjectParameter("QRCreateMode", typeof(string));
    
            var qRCreateStationParameter = qRCreateStation.HasValue ?
                new ObjectParameter("QRCreateStation", qRCreateStation) :
                new ObjectParameter("QRCreateStation", typeof(int));
    
            var qRCreateSalePointIdParameter = qRCreateSalePointId.HasValue ?
                new ObjectParameter("QRCreateSalePointId", qRCreateSalePointId) :
                new ObjectParameter("QRCreateSalePointId", typeof(int));
    
            var transactionIdParameter = transactionId.HasValue ?
                new ObjectParameter("transactionId", transactionId) :
                new ObjectParameter("transactionId", typeof(int));
    
            var qRCreateUserIdParameter = qRCreateUserId.HasValue ?
                new ObjectParameter("QRCreateUserId", qRCreateUserId) :
                new ObjectParameter("QRCreateUserId", typeof(int));
    
            var insDTParameter = insDT.HasValue ?
                new ObjectParameter("insDT", insDT) :
                new ObjectParameter("insDT", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_InsQRCodeTicket", ticketTypeParameter, ticketPriceParameter, fromIdParameter, toIdParameter, activeFromParameter, activeToParameter, qRCodeIdParameter, peopleCountParameter, aliasParameter, fromParameter, toParameter, ticketStringValueParameter, qRCreateModeParameter, qRCreateStationParameter, qRCreateSalePointIdParameter, transactionIdParameter, qRCreateUserIdParameter, insDTParameter);
        }
    
        public virtual int sp_InsRFIDTicket(Nullable<int> ticketType, Nullable<decimal> ticketPrice, Nullable<int> fromId, Nullable<int> toId, Nullable<int> passType, Nullable<System.DateTime> activeFrom, Nullable<System.DateTime> activeTo, string rFIDTicketId, Nullable<short> peopleCount, string rFIDCreateMode, Nullable<int> rFIDCreateStation, Nullable<int> rFIDCreateSalePointId, Nullable<int> transactionId, Nullable<int> rFIDCreateUserId)
        {
            var ticketTypeParameter = ticketType.HasValue ?
                new ObjectParameter("ticketType", ticketType) :
                new ObjectParameter("ticketType", typeof(int));
    
            var ticketPriceParameter = ticketPrice.HasValue ?
                new ObjectParameter("ticketPrice", ticketPrice) :
                new ObjectParameter("ticketPrice", typeof(decimal));
    
            var fromIdParameter = fromId.HasValue ?
                new ObjectParameter("fromId", fromId) :
                new ObjectParameter("fromId", typeof(int));
    
            var toIdParameter = toId.HasValue ?
                new ObjectParameter("toId", toId) :
                new ObjectParameter("toId", typeof(int));
    
            var passTypeParameter = passType.HasValue ?
                new ObjectParameter("passType", passType) :
                new ObjectParameter("passType", typeof(int));
    
            var activeFromParameter = activeFrom.HasValue ?
                new ObjectParameter("activeFrom", activeFrom) :
                new ObjectParameter("activeFrom", typeof(System.DateTime));
    
            var activeToParameter = activeTo.HasValue ?
                new ObjectParameter("activeTo", activeTo) :
                new ObjectParameter("activeTo", typeof(System.DateTime));
    
            var rFIDTicketIdParameter = rFIDTicketId != null ?
                new ObjectParameter("RFIDTicketId", rFIDTicketId) :
                new ObjectParameter("RFIDTicketId", typeof(string));
    
            var peopleCountParameter = peopleCount.HasValue ?
                new ObjectParameter("peopleCount", peopleCount) :
                new ObjectParameter("peopleCount", typeof(short));
    
            var rFIDCreateModeParameter = rFIDCreateMode != null ?
                new ObjectParameter("RFIDCreateMode", rFIDCreateMode) :
                new ObjectParameter("RFIDCreateMode", typeof(string));
    
            var rFIDCreateStationParameter = rFIDCreateStation.HasValue ?
                new ObjectParameter("RFIDCreateStation", rFIDCreateStation) :
                new ObjectParameter("RFIDCreateStation", typeof(int));
    
            var rFIDCreateSalePointIdParameter = rFIDCreateSalePointId.HasValue ?
                new ObjectParameter("RFIDCreateSalePointId", rFIDCreateSalePointId) :
                new ObjectParameter("RFIDCreateSalePointId", typeof(int));
    
            var transactionIdParameter = transactionId.HasValue ?
                new ObjectParameter("transactionId", transactionId) :
                new ObjectParameter("transactionId", typeof(int));
    
            var rFIDCreateUserIdParameter = rFIDCreateUserId.HasValue ?
                new ObjectParameter("RFIDCreateUserId", rFIDCreateUserId) :
                new ObjectParameter("RFIDCreateUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_InsRFIDTicket", ticketTypeParameter, ticketPriceParameter, fromIdParameter, toIdParameter, passTypeParameter, activeFromParameter, activeToParameter, rFIDTicketIdParameter, peopleCountParameter, rFIDCreateModeParameter, rFIDCreateStationParameter, rFIDCreateSalePointIdParameter, transactionIdParameter, rFIDCreateUserIdParameter);
        }
    
        public virtual int sp_InsStock(Nullable<long> trxId, Nullable<int> stockType, Nullable<int> deviceType, Nullable<int> updType, Nullable<int> quantity)
        {
            var trxIdParameter = trxId.HasValue ?
                new ObjectParameter("trxId", trxId) :
                new ObjectParameter("trxId", typeof(long));
    
            var stockTypeParameter = stockType.HasValue ?
                new ObjectParameter("stockType", stockType) :
                new ObjectParameter("stockType", typeof(int));
    
            var deviceTypeParameter = deviceType.HasValue ?
                new ObjectParameter("deviceType", deviceType) :
                new ObjectParameter("deviceType", typeof(int));
    
            var updTypeParameter = updType.HasValue ?
                new ObjectParameter("updType", updType) :
                new ObjectParameter("updType", typeof(int));
    
            var quantityParameter = quantity.HasValue ?
                new ObjectParameter("quantity", quantity) :
                new ObjectParameter("quantity", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_InsStock", trxIdParameter, stockTypeParameter, deviceTypeParameter, updTypeParameter, quantityParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_InsTicketTransaction(Nullable<long> sessionNo, Nullable<byte> isActive, Nullable<decimal> totalAmount, Nullable<byte> tranType, Nullable<byte> paymentType, Nullable<decimal> cashAmount, Nullable<decimal> creditCardAmount, Nullable<decimal> cashPayment, Nullable<decimal> creditCardPayment, Nullable<decimal> change, Nullable<int> insUserId, Nullable<int> salePointId, Nullable<long> oCCTransactionId, Nullable<int> stationId, Nullable<long> shiftId)
        {
            var sessionNoParameter = sessionNo.HasValue ?
                new ObjectParameter("sessionNo", sessionNo) :
                new ObjectParameter("sessionNo", typeof(long));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("isActive", isActive) :
                new ObjectParameter("isActive", typeof(byte));
    
            var totalAmountParameter = totalAmount.HasValue ?
                new ObjectParameter("totalAmount", totalAmount) :
                new ObjectParameter("totalAmount", typeof(decimal));
    
            var tranTypeParameter = tranType.HasValue ?
                new ObjectParameter("tranType", tranType) :
                new ObjectParameter("tranType", typeof(byte));
    
            var paymentTypeParameter = paymentType.HasValue ?
                new ObjectParameter("paymentType", paymentType) :
                new ObjectParameter("paymentType", typeof(byte));
    
            var cashAmountParameter = cashAmount.HasValue ?
                new ObjectParameter("cashAmount", cashAmount) :
                new ObjectParameter("cashAmount", typeof(decimal));
    
            var creditCardAmountParameter = creditCardAmount.HasValue ?
                new ObjectParameter("creditCardAmount", creditCardAmount) :
                new ObjectParameter("creditCardAmount", typeof(decimal));
    
            var cashPaymentParameter = cashPayment.HasValue ?
                new ObjectParameter("cashPayment", cashPayment) :
                new ObjectParameter("cashPayment", typeof(decimal));
    
            var creditCardPaymentParameter = creditCardPayment.HasValue ?
                new ObjectParameter("creditCardPayment", creditCardPayment) :
                new ObjectParameter("creditCardPayment", typeof(decimal));
    
            var changeParameter = change.HasValue ?
                new ObjectParameter("change", change) :
                new ObjectParameter("change", typeof(decimal));
    
            var insUserIdParameter = insUserId.HasValue ?
                new ObjectParameter("insUserId", insUserId) :
                new ObjectParameter("insUserId", typeof(int));
    
            var salePointIdParameter = salePointId.HasValue ?
                new ObjectParameter("salePointId", salePointId) :
                new ObjectParameter("salePointId", typeof(int));
    
            var oCCTransactionIdParameter = oCCTransactionId.HasValue ?
                new ObjectParameter("OCCTransactionId", oCCTransactionId) :
                new ObjectParameter("OCCTransactionId", typeof(long));
    
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var shiftIdParameter = shiftId.HasValue ?
                new ObjectParameter("shiftId", shiftId) :
                new ObjectParameter("shiftId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_InsTicketTransaction", sessionNoParameter, isActiveParameter, totalAmountParameter, tranTypeParameter, paymentTypeParameter, cashAmountParameter, creditCardAmountParameter, cashPaymentParameter, creditCardPaymentParameter, changeParameter, insUserIdParameter, salePointIdParameter, oCCTransactionIdParameter, stationIdParameter, shiftIdParameter);
        }
    
        public virtual ObjectResult<sp_SelMoneyStatus_Result> sp_SelMoneyStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelMoneyStatus_Result>("sp_SelMoneyStatus");
        }
    
        public virtual ObjectResult<Nullable<decimal>> sp_SelTrxId(Nullable<long> trxTypeId)
        {
            var trxTypeIdParameter = trxTypeId.HasValue ?
                new ObjectParameter("trxTypeId", trxTypeId) :
                new ObjectParameter("trxTypeId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("sp_SelTrxId", trxTypeIdParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> UpdTransactionStatus(Nullable<long> transactionId)
        {
            var transactionIdParameter = transactionId.HasValue ?
                new ObjectParameter("transactionId", transactionId) :
                new ObjectParameter("transactionId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("UpdTransactionStatus", transactionIdParameter);
        }
    
        public virtual ObjectResult<sp_ControlAppLogon_Result> sp_ControlAppLogon(string userId, string channelCd, string password, string ipAddress)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var channelCdParameter = channelCd != null ?
                new ObjectParameter("channelCd", channelCd) :
                new ObjectParameter("channelCd", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var ipAddressParameter = ipAddress != null ?
                new ObjectParameter("ipAddress", ipAddress) :
                new ObjectParameter("ipAddress", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ControlAppLogon_Result>("sp_ControlAppLogon", userIdParameter, channelCdParameter, passwordParameter, ipAddressParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_SelUserId(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_SelUserId", userIdParameter);
        }
    
        public virtual ObjectResult<sp_TomAppLogon_Result> sp_TomAppLogon(string userId, string channelCd, string password, string ipAddress, Nullable<int> stationId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var channelCdParameter = channelCd != null ?
                new ObjectParameter("channelCd", channelCd) :
                new ObjectParameter("channelCd", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var ipAddressParameter = ipAddress != null ?
                new ObjectParameter("ipAddress", ipAddress) :
                new ObjectParameter("ipAddress", typeof(string));
    
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_TomAppLogon_Result>("sp_TomAppLogon", userIdParameter, channelCdParameter, passwordParameter, ipAddressParameter, stationIdParameter);
        }
    
        public virtual ObjectResult<sp_userAuth_Result> sp_userAuth(Nullable<int> userId, string channelCode)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var channelCodeParameter = channelCode != null ?
                new ObjectParameter("channelCode", channelCode) :
                new ObjectParameter("channelCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_userAuth_Result>("sp_userAuth", userIdParameter, channelCodeParameter);
        }
    
        public virtual ObjectResult<Nullable<System.DateTime>> sp_GetSrvDT()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.DateTime>>("sp_GetSrvDT");
        }
    
        public virtual ObjectResult<Nullable<int>> sp_SelAlias()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_SelAlias");
        }
    
        public virtual ObjectResult<sp_SelShiftPaymentReport_Result> sp_SelShiftPaymentReport(Nullable<int> salePointId, Nullable<int> stationId, Nullable<int> userId, Nullable<short> shiftorall, Nullable<System.DateTime> startDt, Nullable<System.DateTime> endDt)
        {
            var salePointIdParameter = salePointId.HasValue ?
                new ObjectParameter("salePointId", salePointId) :
                new ObjectParameter("salePointId", typeof(int));
    
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var shiftorallParameter = shiftorall.HasValue ?
                new ObjectParameter("shiftorall", shiftorall) :
                new ObjectParameter("shiftorall", typeof(short));
    
            var startDtParameter = startDt.HasValue ?
                new ObjectParameter("startDt", startDt) :
                new ObjectParameter("startDt", typeof(System.DateTime));
    
            var endDtParameter = endDt.HasValue ?
                new ObjectParameter("endDt", endDt) :
                new ObjectParameter("endDt", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelShiftPaymentReport_Result>("sp_SelShiftPaymentReport", salePointIdParameter, stationIdParameter, userIdParameter, shiftorallParameter, startDtParameter, endDtParameter);
        }
    
        public virtual ObjectResult<sp_SelQRCodeTicket_Result> sp_SelQRCodeTicket()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelQRCodeTicket_Result>("sp_SelQRCodeTicket");
        }
    
        public virtual ObjectResult<sp_SelRFIDTicket_Result> sp_SelRFIDTicket()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelRFIDTicket_Result>("sp_SelRFIDTicket");
        }
    
        public virtual ObjectResult<sp_SelTicketTransactionDetails_Result> sp_SelTicketTransactionDetails()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelTicketTransactionDetails_Result>("sp_SelTicketTransactionDetails");
        }
    
        public virtual ObjectResult<Nullable<int>> sp_UpdQRCodeTicket(Nullable<long> recId, Nullable<System.DateTime> syncDT)
        {
            var recIdParameter = recId.HasValue ?
                new ObjectParameter("recId", recId) :
                new ObjectParameter("recId", typeof(long));
    
            var syncDTParameter = syncDT.HasValue ?
                new ObjectParameter("syncDT", syncDT) :
                new ObjectParameter("syncDT", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_UpdQRCodeTicket", recIdParameter, syncDTParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_UpdRFIDTicket(Nullable<long> recId, Nullable<System.DateTime> syncDT)
        {
            var recIdParameter = recId.HasValue ?
                new ObjectParameter("recId", recId) :
                new ObjectParameter("recId", typeof(long));
    
            var syncDTParameter = syncDT.HasValue ?
                new ObjectParameter("syncDT", syncDT) :
                new ObjectParameter("syncDT", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_UpdRFIDTicket", recIdParameter, syncDTParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_UpdTicketTransactionDetails(Nullable<long> recId, Nullable<System.DateTime> syncDT)
        {
            var recIdParameter = recId.HasValue ?
                new ObjectParameter("recId", recId) :
                new ObjectParameter("recId", typeof(long));
    
            var syncDTParameter = syncDT.HasValue ?
                new ObjectParameter("syncDT", syncDT) :
                new ObjectParameter("syncDT", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_UpdTicketTransactionDetails", recIdParameter, syncDTParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_UpdTvmAlarm(Nullable<long> recId, Nullable<System.DateTime> syncDT)
        {
            var recIdParameter = recId.HasValue ?
                new ObjectParameter("recId", recId) :
                new ObjectParameter("recId", typeof(long));
    
            var syncDTParameter = syncDT.HasValue ?
                new ObjectParameter("syncDT", syncDT) :
                new ObjectParameter("syncDT", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_UpdTvmAlarm", recIdParameter, syncDTParameter);
        }
    
        public virtual int sp_insertTvmAlarm(Nullable<int> stationId, Nullable<int> deviceNo, Nullable<int> alarmTypeId, string alarmMessage)
        {
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var deviceNoParameter = deviceNo.HasValue ?
                new ObjectParameter("deviceNo", deviceNo) :
                new ObjectParameter("deviceNo", typeof(int));
    
            var alarmTypeIdParameter = alarmTypeId.HasValue ?
                new ObjectParameter("alarmTypeId", alarmTypeId) :
                new ObjectParameter("alarmTypeId", typeof(int));
    
            var alarmMessageParameter = alarmMessage != null ?
                new ObjectParameter("alarmMessage", alarmMessage) :
                new ObjectParameter("alarmMessage", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_insertTvmAlarm", stationIdParameter, deviceNoParameter, alarmTypeIdParameter, alarmMessageParameter);
        }
    
        public virtual ObjectResult<sp_SelStockStatus_Result> sp_SelStockStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelStockStatus_Result>("sp_SelStockStatus");
        }
    
        public virtual ObjectResult<sp_EmvDiscount1_Result> sp_EmvDiscount(Nullable<int> fareMediaType, string ticketType, Nullable<decimal> amount, Nullable<int> institutionId)
        {
            var fareMediaTypeParameter = fareMediaType.HasValue ?
                new ObjectParameter("fareMediaType", fareMediaType) :
                new ObjectParameter("fareMediaType", typeof(int));
    
            var ticketTypeParameter = ticketType != null ?
                new ObjectParameter("ticketType", ticketType) :
                new ObjectParameter("ticketType", typeof(string));
    
            var amountParameter = amount.HasValue ?
                new ObjectParameter("amount", amount) :
                new ObjectParameter("amount", typeof(decimal));
    
            var institutionIdParameter = institutionId.HasValue ?
                new ObjectParameter("institutionId", institutionId) :
                new ObjectParameter("institutionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_EmvDiscount1_Result>("sp_EmvDiscount", fareMediaTypeParameter, ticketTypeParameter, amountParameter, institutionIdParameter);
        }
    
        public virtual ObjectResult<sp_SelPeakTime_Result> sp_SelPeakTime(Nullable<int> institutionId)
        {
            var institutionIdParameter = institutionId.HasValue ?
                new ObjectParameter("institutionId", institutionId) :
                new ObjectParameter("institutionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelPeakTime_Result>("sp_SelPeakTime", institutionIdParameter);
        }
    }
}
