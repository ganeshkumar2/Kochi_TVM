using KioskFramework.OccSrv;
using KioskFramework.PayPointSrv;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kochi_TVM.Business
{
    public static class Parameters
    {
        private static ILog log = LogManager.GetLogger(typeof(Parameters).Name);
        public static PayPointClient sr = new PayPointClient();
        public static OccClient occ = new OccClient();
        //public static Db db = new Db();
        public static TVMConsts TVMConst = new TVMConsts();
        public static ParameterOperations TVMStatic = new StaticParameterOpr();
        public static DynamicParameterOpr TVMDynamic = new DynamicParameterOpr();
        public static AFCDataTransactionService.ServiceSoapClient AfcDataTransactionClient = null;
        //public static BackgroundWorker bwQrcPrinterStatus = null;
        //public static BackgroundWorker bwDispStatus = null;
        //public static BackgroundWorker bwBnaStatus = null;
        //public static BackgroundWorker bwAfcStatus = null;
        public static DateTime lastSync;
        public static string userId;
        public static List<string> menuItems = new List<string>();
        //public static TopupInfo tuData = new TopupInfo();
        public static TvmMonitoring TvmMonitoringData = new TvmMonitoring();
        public static class MenuStrings
        {
            public const string QrRep = "QR Replenishment";
            public const string RptRep = "RPT Replenishment";
            public const string RcptRep = "Receipt Paper Replenishment";
            public const string CoinRep = "Coin Replenishment";
            public const string CoinEmpt = "Coin Hopper Emptying";
            public const string CashDump = "Cash Recycler Dumping";
            public const string CashBoxRem = "Cash Box Removing";
            public const string QRTest = "QR Test Printing";
            public const string RptTest = "RPT Test Printing";
            public const string HopTest = "Hopper Test Dispensing";
            public const string CashEscCheck = "Cash Escrow Checking";
            public const string K1CardRdrWrk = "Kochi1 Card Reader Working";
            public const string CreditDebitCardRdWrk = "Credit/Debit Card Reader Working";
            public const string TouchScreenTest = "Touch Screen Test";
            public const string Keypad1Test = "Keypad1 Test";
            public const string Keypad2Test = "Keypad2 Test";
            public const string ScrlDispTest = "Scroll Display Test(PID)";
            public const string RcptPrntTest = "Receipt Printer Testing";
            public const string SpkrTest = "Speaker Test";
            public const string VibTest = "Vibration Test";
            public const string RFIDRdTest = "RFID Reader Test";
        };

        static XmlDocument _xml;
        static XmlElement _tvmNode;

        static string TVMInfoXml()
        {
            
            _xml = new XmlDocument();
            try
            {
                _tvmNode = _xml.CreateElement(string.Empty, "tvmSummary", string.Empty);
                _xml.AppendChild(_tvmNode);
                /*******************************************TOM INFORMATION**********************************************************************/
                _tvmNode.SetAttribute("tvmId", $"{TvmMonitoringData.tvmId}"); //$"{}"
                _tvmNode.SetAttribute("operationDate", DateTime.Now.ToString("yyyyMMdd"));
                _tvmNode.SetAttribute("lastTransactionDate", Convert.ToDateTime(TvmMonitoringData.lastTransactionDate).ToString("yyyyMMdd HH:mm:ss"));
                _tvmNode.SetAttribute("lastSynchronizationDate", Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd HH:mm:ss"));
                _tvmNode.SetAttribute("appVersion", $"{TvmMonitoringData.appVersion}");
                _tvmNode.SetAttribute("updateDt", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                _tvmNode.SetAttribute("stationId", TvmMonitoringData.stationId.ToString());
                _tvmNode.SetAttribute("currentWorkMode", TvmMonitoringData.SpecialMode);
                _tvmNode.SetAttribute("CCConnectionStatus", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connected" : "Not Connected");
                /*****************************************DEVICE STATUS*********************************************************************************/
                AddDeviceInfoXmlNode("bnrStatus", "0", TvmMonitoringData.bnrStatus, "1");
                AddDeviceInfoXmlNode("hopperStatus1", "0", $"{TvmMonitoringData.hopperStatus1}", "1");
                AddDeviceInfoXmlNode("hopperStatus2", "0", $"{TvmMonitoringData.hopperStatus2}", "1");
                AddDeviceInfoXmlNode("hopperStatus5", "0", $"{TvmMonitoringData.hopperStatus5}", "1");
                AddDeviceInfoXmlNode("qrPrinter", "0", $"{TvmMonitoringData.qrPrinterStatus}", "1");
                AddDeviceInfoXmlNode("receiptPrinter", "0", $"{TvmMonitoringData.receiptPrinterStatus}", "1");
                AddDeviceInfoXmlNode("ledPanelStatus", "0", $"{TvmMonitoringData.ledPanelStatus}", "1");
                AddDeviceInfoXmlNode("doorSensorStatus", "0", $"{TvmMonitoringData.doorSensorStatus}", "1");
                AddDeviceInfoXmlNode("speakerStatus", "0", $"{TvmMonitoringData.speakerStatus}", "1");
                /**************************************************TRANSACTION STATUS*******************************************************************/
                AddAccountInfoXmlNode("QRSJT", TvmMonitoringData.QRSJT_Count, TvmMonitoringData.QRSJT_Amount.ToString());
                AddAccountInfoXmlNode("QRRJT", TvmMonitoringData.QRRJT_Count, TvmMonitoringData.QRRJT_Amount.ToString());
                AddAccountInfoXmlNode("Total", TvmMonitoringData.Total_Count, TvmMonitoringData.Total_Amount.ToString());
                /********************************************************STOCK MANAGEMENT*******************************************************************************/
                AddStockInfoXmlNode("NumberofQR", TvmMonitoringData.numberOfQr);
                AddStockInfoXmlNode("hopperCoins1", Convert.ToInt16(TvmMonitoringData.hopperCoins1));
                AddStockInfoXmlNode("hopperCoins2", Convert.ToInt16(TvmMonitoringData.hopperCoins2));
                AddStockInfoXmlNode("hopperCoins5", Convert.ToInt16(TvmMonitoringData.hopperCoins5));
                /*******************************************************************************************************************************************************/
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return _xml.OuterXml;
            }
            return _xml.OuterXml; 
        }

        private static bool AddDeviceInfoXmlNode(string name, string alarm, string desc, string status)
        {
            try
            {
                var deviceNode = _xml.CreateElement(name);

                deviceNode.SetAttribute("alarmLevel", alarm);
                deviceNode.SetAttribute("description", desc);
                deviceNode.SetAttribute("isOk", status);

                _tvmNode.AppendChild(deviceNode);

                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        private static bool AddAccountInfoXmlNode(string name, int count, string amount)
        {
            try
            {
                var deviceNode = _xml.CreateElement(name);

                deviceNode.SetAttribute("count", Convert.ToString(count));
                deviceNode.SetAttribute("amount", amount);

                _tvmNode.AppendChild(deviceNode);

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        private static bool AddStockInfoXmlNode(string name, int count)
        {
            try
            {
                var deviceNode = _xml.CreateElement(name);

                deviceNode.SetAttribute("count", Convert.ToString(count));
                _tvmNode.AppendChild(deviceNode);

                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public static string InsertLog()
        {
            var resp = "";
            try
            {
                string ip = Properties.Settings.Default.ip;
                var remoteAddress = new System.ServiceModel.EndpointAddress(ip);
                KioskFramework.NStationService.ServiceSoapClient service = new KioskFramework.NStationService.ServiceSoapClient(new System.ServiceModel.BasicHttpBinding(), remoteAddress);

                var requestHeader = new KioskFramework.NStationService.WSRequestHeader
                {
                    Username = "NServiceUser",
                    Password = "n3ER7!cEn3Er"
                };
                string val = TVMInfoXml();
                if (val != null && val != "")
                {
                    resp = service.InsertMonitoring(requestHeader, val);

                    return resp;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return resp;
            }
            return resp;
        }
        public static bool CheckScConnection()
        {
            bool retVal = false;
            string ip = Properties.Settings.Default.ip;
            var remoteAddress = new System.ServiceModel.EndpointAddress(ip);
            KioskFramework.NStationService.ServiceSoapClient service = new KioskFramework.NStationService.ServiceSoapClient(new System.ServiceModel.BasicHttpBinding(), remoteAddress);

            try
            {
                var requestHeader = new KioskFramework.NStationService.WSRequestHeader
                {
                    Username = "NServiceUser",
                    Password = "n3ER7!cEn3Er"
                };

                DataSet ds = service.ExecuteNonQuery(requestHeader, "SELECT GETDATE()");
                log.Debug(ds.Tables.ToString());
                if (ds.Tables != null)
                {
                    return retVal=true;
                }
                else
                {
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return retVal;
            }
        }

        public static bool InsNStationAlarm(int stationId, int deviceId, int alarmType, string message)
        {
            bool retVal = false;
            try
            {
                string ip = Properties.Settings.Default.ip;
                var remoteAddress = new System.ServiceModel.EndpointAddress(ip);
                KioskFramework.NStationService.ServiceSoapClient service = new KioskFramework.NStationService.ServiceSoapClient(new System.ServiceModel.BasicHttpBinding(), remoteAddress);
                KioskFramework.NStationService.TvmAlarm model = new KioskFramework.NStationService.TvmAlarm();


                var requestHeader = new KioskFramework.NStationService.WSRequestHeader
                {
                    Username = "NServiceUser",
                    Password = "n3ER7!cEn3Er"
                };

                model.alarmMessage = message;
                model.alarmTypeId = alarmType;
                model.deviceNo = deviceId;
                model.stationId = stationId;

                retVal = service.InsTvmAlarm(requestHeader, model);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return retVal;
            }
            return retVal;
        }

        public static string insTVMMonitoring()
        {
            string retVal = "";

            try
            {
                string ip = Properties.Settings.Default.ip;
                var remoteAddress = new System.ServiceModel.EndpointAddress(ip);
                KioskFramework.NStationService.ServiceSoapClient service = new KioskFramework.NStationService.ServiceSoapClient(new System.ServiceModel.BasicHttpBinding(), remoteAddress);
                KioskFramework.NStationService.TvmMonitoring model = new KioskFramework.NStationService.TvmMonitoring();


                var requestHeader = new KioskFramework.NStationService.WSRequestHeader
                {
                    Username = "NServiceUser",
                    Password = "n3ER7!cEn3Er"
                };

                model.appVersion = TvmMonitoringData.appVersion;
                model.banknote10 = TvmMonitoringData.banknote10;
                model.banknote20 = TvmMonitoringData.banknote20;
                model.bnrStatus = TvmMonitoringData.bnrStatus;
                model.doorSensorStatus = TvmMonitoringData.doorSensorStatus;
                model.hopperCoins1 = TvmMonitoringData.hopperCoins1;
                model.hopperCoins2 = TvmMonitoringData.hopperCoins2;
                model.hopperCoins5 = TvmMonitoringData.hopperCoins5;
                model.hopperStatus1 = TvmMonitoringData.hopperStatus1;
                model.hopperStatus2 = TvmMonitoringData.hopperStatus2;
                model.hopperStatus5 = TvmMonitoringData.hopperStatus5;
                model.lastTransactionDate = TvmMonitoringData.lastTransactionDate;
                model.ledPanelStatus = TvmMonitoringData.ledPanelStatus;
                model.numberOfQr = TvmMonitoringData.numberOfQr;
                model.qrPrinterStatus = TvmMonitoringData.qrPrinterStatus;
                model.QRRJT_Amount = TvmMonitoringData.QRRJT_Amount;
                model.QRRJT_Count = TvmMonitoringData.QRSJT_Count;
                model.receiptPrinterStatus = TvmMonitoringData.receiptPrinterStatus;
                model.speakerStatus = TvmMonitoringData.speakerStatus;
                model.stationId = TvmMonitoringData.stationId;
                model.Total_Amount = TvmMonitoringData.Total_Amount;
                model.Total_Count = TvmMonitoringData.Total_Count;
                model.tvmId = TvmMonitoringData.tvmId;
                string val = TVMInfoXml();
                if (val != null && val != "")
                {
                    retVal = service.InsertMonitoring(requestHeader, val);
                }
                log.Debug(retVal);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return retVal;
            }

            return retVal;
        }

        public static string insTVMStatusReport()
        {
            string retVal = "";

            try
            {
                if (AfcDataTransactionClient == null)
                    AfcDataTransactionClient = new AFCDataTransactionService.ServiceSoapClient();

                var requestHeader = new AFCDataTransactionService.WSRequestHeader
                {
                    Username = "NServiceUser",
                    Password = "n3ER7!cEn3Er"
                };

                string val = TVMInfoXml();
                if (val != null && val != "")
                {
                    var commandAfc = AfcDataTransactionClient.StatusReport(requestHeader, val);
                    log.Debug(val);
                    retVal = commandAfc;
                }                
                log.Debug(retVal);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }

            return retVal;
        }
    }
    public class TVMConsts
    {
        private static ILog log = LogManager.GetLogger(typeof(TVMConsts).Name);
        bool isInit = false;
        public IniFileOperations iniReader = new IniFileOperations();
        public TVMConsts()
        {
            if (!isInit)
                isInit = Init();
        }
        //#region Devices id  
        //public int TVMID = 1001;
        //#endregion
        #region Devices seial ports       
        public string BNAPort = "COM3";
        public string Hopper5Port = "COM8";
        public string Hopper2Port = "COM3";
        public string Hopper1Port = "COM2";
        public string LedPort = "COM4";
        public string SensorPort = "COM1";
        public string QRPrinterPort = "";
        public string RPTDispenserPort = "";
        public string UxPosPort = "COM9";
        public string macAddress = "";
        #endregion
        #region Devices name
        public string StationId = "1";
        public string TVMName = "Aluva Station TVM";
        public string QRPrinterName = "";
        public string ReceiptPrinterName = "";
        #endregion
        #region Devices brand  
        public string BNABrand = "BillToBill";
        #endregion
        #region Hopper address 
        public int Hopper5Address = 3;
        public int Hopper2Address = 4;
        public int Hopper1Address = 5;
        #endregion
        bool Init()
        {
            bool result = false;
            try
            {
                //#region Devices id  
                //TVMID = int.Parse(iniReader.ReadParamByKey("DEVICES_ID", "TVMID"));
                //#endregion
                #region Devices name
                StationId = iniReader.ReadParamByKey("DEVICES_NAME", "StationId");
                TVMName = iniReader.ReadParamByKey("DEVICES_NAME", "TVMName");
                QRPrinterName = iniReader.ReadParamByKey("DEVICES_NAME", "QRPrinterName");
                ReceiptPrinterName = iniReader.ReadParamByKey("DEVICES_NAME", "ReceiptPrinterName");
                macAddress = iniReader.ReadParamByKey("DEVICES_NAME", "MacAddress");
                #endregion
                #region Devices brand  
                BNABrand = iniReader.ReadParamByKey("DEVICES_BRAND", "BNABrand");
                #endregion
                #region Hopper address 
                Hopper5Address = int.Parse(iniReader.ReadParamByKey("HOPPER_ADDRESS", "Hopper5Address"));
                Hopper2Address = int.Parse(iniReader.ReadParamByKey("HOPPER_ADDRESS", "Hopper2Address"));
                Hopper1Address = int.Parse(iniReader.ReadParamByKey("HOPPER_ADDRESS", "Hopper1Address"));
                #endregion
                #region Devices seial ports      
                BNAPort = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "BNAPort");
                Hopper5Port = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "Hopper5Port");
                Hopper2Port = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "Hopper2Port");
                Hopper1Port = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "Hopper1Port");
                LedPort = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "LedPort");
                SensorPort = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "SensorPort");
                QRPrinterPort = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "QRPrinterPort");
                RPTDispenserPort = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "RPTDispenserPort");
                UxPosPort = iniReader.ReadParamByKey("DEVICES_SERIAL_PORT", "UxPosPort");
                #endregion
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error(ex.ToString());
            }

            return result;
        }
    }
    public abstract class ParameterOperations
    {
        public Dictionary<string, string> parameterList = new Dictionary<string, string>();
        public abstract bool FillOrUpdateParameters();
        public abstract string GetParameter(string key);
        public abstract bool Init();
        public bool AddOrUpdateParameter(String descCode, String paramValue)
        {
            bool result = false;
            try
            {
                if (!parameterList.ContainsKey(descCode))
                {
                    if (!String.IsNullOrEmpty(paramValue))
                    {
                        parameterList.Add(descCode, paramValue);
                    }
                    else
                    {
                        //Log.log.Write(LogTypes.Info.ToString() + "Parameter.cs AddOrUpdateParameter line 164 " + ":" + descCode + " is Null");
                    }
                }
                else if (parameterList.ContainsKey(descCode))
                {
                    if (!String.IsNullOrEmpty(paramValue))
                    {
                        parameterList[descCode] = paramValue;
                    }
                    else
                    {
                        //Log.log.Write(LogTypes.Info.ToString() + "Parameter.cs AddOrUpdateParameter line 171 " + ":" + descCode + " is Null");
                    }
                }
                result = true;
            }
            catch (Exception e)
            { }

            return result;
        }
    }
    public class DynamicParameterOpr : ParameterOperations
    {
        private static ILog log = LogManager.GetLogger(typeof(DynamicParameterOpr).Name);
        public DateTime SysTime = DateTime.Now;
        bool isInit = false;
        public DynamicParameterOpr()
        {
            if (!isInit)
            {
                isInit = Init();
            }
        }
        public override bool Init()
        {
            bool result = false;
            try
            {
                FillOrUpdateParameters();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error(ex.ToString());
            }
            return result;
        }
        public override bool FillOrUpdateParameters()
        {
            bool result = false;
            try
            {
                //parameterList.Clear();

                FillOrUpdateAfcConnStatus();
                FillOrUpdateScConnStatus();
                FillOrUpdateLocalParams();

                if (FillOrUpdateSysParams())
                {
                    if (GetParameter("AfcConn") == "1")
                    {
                        if (FillOrUpdateAfcConnParams())
                        {
                            result = true;
                        }
                    }
                    else if (GetParameter("SCConn") == "1")
                    {
                        if (FillOrUpdateScConnParams())
                        {
                            result = true;
                        }
                    }
                    else if (FillOrUpdateLocalParams())
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("LogTypes.Error.ToString() : " + ex.Message);
            }
            return result;
        }
        public override string GetParameter(string key)
        {
            string value = String.Empty;
            try
            {
                if (parameterList.ContainsKey(key))
                {
                    value = parameterList[key];
                }
            }
            catch (Exception ex)
            {
                value = String.Empty;
                log.Error("LogTypes.Error.ToString() : " + ex.Message);
            }
            return value;
        }
        private bool FillOrUpdateSysParams()
        {
            bool result = false;
            try
            {
                using (var context = new Models.TVM_Entities())
                {
                    var rp = context.sp_SelSysParams().ToList();
                    //var rp = Parameters.db.ExecSP("def.sp_SelSysParams");
                    if (rp.Count > 0)
                    {
                        foreach (var item in rp)
                        {
                            AddOrUpdateParameter(item.descCode.ToString(), item.paramValue.ToString());
                        }
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.Message);
            }
            return result;
        }
        public bool FillOrUpdateAfcConnParams()
        {
            bool result = false;
            try
            {
                var rpc = Parameters.sr.SelUnitByUID(Parameters.TVMStatic.GetParameter("macAddress"));
                //log.Write("MacID : " + Parameters.TVMStatic.GetParameter("macAddress"));
                result = ((Validation.IsValidAFCRP(rpc)) && (rpc.Result == 1));
                if (result)
                {
                    AddOrUpdateParameter("macAddress", rpc.Data.Tables[0].Rows[0]["macAddress"].ToString());
                    AddOrUpdateParameter("explanation", rpc.Data.Tables[0].Rows[0]["salePointCode"].ToString());
                    AddOrUpdateParameter("unitId", rpc.Data.Tables[0].Rows[0]["recId"].ToString());
                    AddOrUpdateParameter("stationId", rpc.Data.Tables[0].Rows[0]["stationId"].ToString());
                    AddOrUpdateParameter("unitType", rpc.Data.Tables[0].Rows[0]["salePointTypeId"].ToString());
                    AddOrUpdateParameter("descCode", rpc.Data.Tables[0].Rows[0]["descCode"].ToString());
                    AddOrUpdateParameter("localAuth", "0");
                    AddOrUpdateParameter("localAuthTimesUp", "0");
                    AddOrUpdateParameter("localAuthClosed", "0");
                }
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.Message);
            }

            return result;
        }
        private bool FillOrUpdateScConnParams()
        {
            bool result = false;

            return result;
        }
        private bool FillOrUpdateAfcConnStatus()
        {
            bool result = false;
            try
            {
                if (GetAfcConnStatus())
                {
                    AddOrUpdateParameter("AfcConn", "1");
                    result = true;
                }
                else
                {
                    AddOrUpdateParameter("AfcConn", "0");
                    result = false;
                }
            }
            catch (Exception ex)
            {

                AddOrUpdateParameter("AfcConn", "0");
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.Message);
            }

            //Parameters.lastSync = DateTime.Now;
            return result;
        }
        public bool GetAfcConnStatus()
        {
            bool result = false;
            try
            {
                var rpAfc = Parameters.sr.ExecSP("def.sp_GetSrvDT");
                var AfcConn = Validation.IsValidAFCRP(rpAfc);
                log.Debug("Debug - GetAfcConnStatus() : " + AfcConn);
                if (AfcConn)
                {
                    log.Debug("Debug - rpAfc.Result() : " + rpAfc.Result);

                    bool isOk = (Validation.IsValidAFCRP(rpAfc) && (rpAfc.Result == 0));
                    if (isOk)
                    {
                        log.Debug("Debug - SysTime() : " + rpAfc.Data.Tables[0].Rows[0]["srv_dt"]);
                        SysTime = Convert.ToDateTime((rpAfc.Data.Tables[0].Rows[0]["srv_dt"]));
                    }
                }
                else
                {
                    SysTime = DateTime.Now;
                }
                if (AfcConn)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("LogTypes.Error.ToString() : " + ex.Message);
            }
            //Parameters.lastSync = DateTime.Now;
            return result;
        }
        private bool FillOrUpdateLocalParams()
        {
            bool result = false;

            try
            {
                using (var context = new Models.TVM_Entities())
                {
                    var rp = context.sp_SelSalePointByMac(Parameters.TVMStatic.GetParameter("macAddress")).ToList();
                    if (rp.Count > 0)
                    {
                        foreach (var item in rp)
                        {
                            AddOrUpdateParameter("macAddress", item.macAddress.ToString());
                            AddOrUpdateParameter("explanation", item.salePointCode.ToString());
                            AddOrUpdateParameter("unitId", item.recId.ToString());
                            //Log.log.Write("stationId");
                            //Log.log.Write("stationId :" + rpl.Data.Tables[0].Rows[0]["stationId"].ToString());
                            AddOrUpdateParameter("stationId", item.stationId.ToString());
                            AddOrUpdateParameter("descCode", item.descCode.ToString());
                            AddOrUpdateParameter("unitType", item.salePointTypeId.ToString());
                            AddOrUpdateParameter("localAuth", "1");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("FillOrUpdateLocalParams() : " + ex.Message);
                result = false;                
            }

            //Parameters.lastSync = DateTime.Now;

            return result;
        }
        private bool FillOrUpdateScConnStatus()
        {
            bool result = false;
            try
            {
                result = Parameters.CheckScConnection();
                log.Debug("FillOrUpdateScConnStatus() : " + result);
                if (result)
                    AddOrUpdateParameter("SCConn", "1");
                else
                    AddOrUpdateParameter("SCConn", "0");
            }
            catch (Exception ex)
            {
                result = false;
                AddOrUpdateParameter("SCConn", "0");
                log.Error("FillOrUpdateScConnStatus() : " + ex.Message);
            }
            //Parameters.lastSync = DateTime.Now;
            return result;
        }
    }
    public class StaticParameterOpr : ParameterOperations
    {
        bool isInit = false;
        public StaticParameterOpr()
        {
            if (!isInit)
            {
                isInit = Init();
            }
        }
        public override bool Init()
        {
            bool result = false;
            try
            {
                FillOrUpdateParameters();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write(ex.ToString());
            }
            return result;
        }
        public override bool FillOrUpdateParameters()
        {
            bool result = false;
            try
            {
                //parameterList.Clear();
                AddOrUpdateParameter("cashPaymentRemainingTime", "60");
                AddOrUpdateParameter("macAddress", GetMacAddress());
                AddOrUpdateParameter("appVersion", GetAppVersion());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write(ex.ToString());
            }
            return result;
        }
        public override string GetParameter(string key)
        {
            string value = String.Empty;
            try
            {
                if (parameterList.ContainsKey(key))
                    value = parameterList[key];
            }
            catch (Exception ex)
            {
                //Logger.Log.log.Write("Error:" + ex.ToString());
                value = String.Empty;
            }
            return value;
        }
        public static string GetMacAddress()
        {
            //var Result = "";
            //var macAddress = string.Empty;
            //var firstPhisicalAdress = string.Empty;

            //foreach (
            //    var nic in
            //        NetworkInterface.GetAllNetworkInterfaces()
            //            .Where(
            //                nic =>
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx)))
            //{
            //    if (firstPhisicalAdress == string.Empty)
            //        firstPhisicalAdress = nic.GetPhysicalAddress().ToString();
            //    if (nic.OperationalStatus != OperationalStatus.Up) continue;
            //    macAddress = nic.GetPhysicalAddress().ToString();
            //    break;
            //}

            //if (macAddress == string.Empty)
            //    macAddress = firstPhisicalAdress;
            //if (macAddress == string.Empty) return Result;
            //var lenMacAddress = macAddress.Length;
            //for (var ii = 0; ii < lenMacAddress; ii++)
            //{
            //    if ((ii > 0) && ((ii % 2) == 0))
            //        Result += "-";
            //    Result += macAddress.Substring(ii, 1);
            //}
            //return Result;
            return Parameters.TVMConst.macAddress;
        }
        public static string GetAppVersion()
        {
            var assem = Assembly.GetExecutingAssembly();
            var assemName = assem.GetName();
            var ver = assemName.Version;
            var Result = ver.ToString();
            return Result;
        }
    }
    public class IniFileOperations
    {
        string filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "Setup.ini";
        //[DllImport("kernel32")]
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        //public void Write(string section, string key, string value)
        //{
        //    WritePrivateProfileString(section, key, value.ToLower(), filePath);
        //}
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        public string ReadParamByKey(string section, string key)
        {
            StringBuilder SB = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return SB.ToString();
        }
        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value.ToLower(), filePath);
        }
    }
    public class TvmMonitoring
    {
        public string bnrStatus { get; set; }
        public string speakerStatus { get; set; }
        public string hopperCoins1 { get; set; }
        public string hopperCoins2 { get; set; }
        public string hopperCoins5 { get; set; }
        public string hopperStatus1 { get; set; }
        public string hopperStatus2 { get; set; }
        public string hopperStatus5 { get; set; }
        public string banknote10 { get; set; }
        public string banknote20 { get; set; }
        public int numberOfQr { get; set; }
        public string qrPrinterStatus { get; set; }
        public string receiptPrinterStatus { get; set; }
        public string ledPanelStatus { get; set; }
        public string doorSensorStatus { get; set; }
        public int QRSJT_Count { get; set; }
        public int QRSJT_Amount { get; set; }
        public int QRRJT_Count { get; set; }
        public int QRRJT_Amount { get; set; }
        public int Total_Count { get; set; }
        public int Total_Amount { get; set; }
        public int tvmId { get; set; }
        public int stationId { get; set; }
        public string appVersion { get; set; }
        public DateTime lastTransactionDate { get; set; }
        public string SpecialMode { get; set; }
    }
}
