using Kochi_TVM.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.VerifoneKochi1
{
    public class EmvService
    {
        #region Variables

        private readonly SerialPort _port = new SerialPort();

        #endregion

        #region AVM Data Methods

        public ServiceDataMultiple GetServiceData(string portName)
        {
            ServiceDataMultiple serviceDataMultiple = new ServiceDataMultiple();

            try
            {
                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    serviceDataMultiple.RespResult.Result = "Failed";
                    serviceDataMultiple.RespResult.ReturnCode = -1;
                    serviceDataMultiple.RespResult.ReturnDescription = "Failed to open Connection";

                    return serviceDataMultiple;
                }
                #endregion

                #region Sending Data to Pos

                byte[] buffer = new byte[] { 250, 161, 30, 0, 69, 26, 175 };

                string sendDataString = EmvService.ByteArrayToHexString(buffer);

                ServiceLog("SEND MESSAGE FOR READ DATA SERVICE :", sendDataString, GetType().Name);

                _port.Write(buffer, 0, buffer.Length);

                #endregion

                for (int i = 0; i < 2; i++)
                {
                    serviceDataMultiple.ServiceDatas[i] = new ServiceData();

                    #region Code taking less time

                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    do
                    {
                        k += 1;

                        Thread.Sleep(500);

                        dataLength = _port.BytesToRead;

                        receiveData = new byte[dataLength];

                        recDataLen = _port.Read(receiveData, 0, dataLength);

                        if (k == 72) //k==4
                        {
                            break;
                        }

                    }
                    while (recDataLen < 132);// 1A AF geldimi

                    #endregion

                    #region Get Data

                    ServiceLog($"RESPONSE MESSAGE FOR READ DATA SERVICE FOR {i + 1} : ", ByteArrayToHexString(receiveData), GetType().Name);

                    ServiceLog("recDataLen :", recDataLen.ToString(), GetType().Name);

                    if (recDataLen != 0)
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data                  
                                                                                                             //Actual Data  in Bytes
                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        string msgData = EmvService.ByteArrayToHexString(recMsgData);
                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                        if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {
                            #region Commented
                            //StringBuilder s2=new StringBuilder();
                            //StringBuilder s3 = new StringBuilder();
                            //for (int j = 0; j < recDataLen; j++)
                            //{
                            //    Console.Write(receiveData[j] + " ");

                            //    s2.Append(receiveData[j] + " ");

                            //}
                            //s3 = s2;
                            //string S1 = EmvService.ByteArrayToHexString(receiveData);

                            #endregion

                            byte[] cardData = recMsgData;//.Skip(32).ToArray();
                            string actualData = EmvService.ByteArrayToHexString(cardData);

                            #region Decleration of variable
                            string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _panTokenNo, _balance, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                            string trmId, trmStan, trmBatchno, trmKM = "";
                            string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                            string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                            string toStnExit, toStnStage, toStnId, toStncode = "";
                            string loadStnId, loadSalePointId;

                            #endregion

                            #region Details of All Info
                            _passCounter = actualData.Substring(0, 2);
                            serviceDataMultiple.ServiceDatas[i].PassCounter = _passCounter;
                            _passLimit = actualData.Substring(2, 2);
                            serviceDataMultiple.ServiceDatas[i].PassLimit = _passLimit;
                            _expirayDate = actualData.Substring(4, 4);
                            serviceDataMultiple.ServiceDatas[i].PassExpiryDate = _expirayDate;
                            _effictiveDate = actualData.Substring(8, 4);
                            serviceDataMultiple.ServiceDatas[i].PassEffectiveDate = _effictiveDate;
                            _passType = actualData.Substring(12, 2);
                            serviceDataMultiple.ServiceDatas[i].PassType = _passType;
                            _terminalInfo = actualData.Substring(14, 30);
                            _trxInfo = actualData.Substring(44, 30);
                            _fromStation = actualData.Substring(74, 16);
                            _toStation = actualData.Substring(90, 16);
                            _lastRoute = actualData.Substring(106, 2);
                            serviceDataMultiple.ServiceDatas[i].LastRouteNo = _lastRoute;
                            _passValidity = actualData.Substring(108, 46);
                            serviceDataMultiple.ServiceDatas[i].PassValidityVol = _passValidity;
                            _passCounterMBS = actualData.Substring(154, 2);
                            serviceDataMultiple.ServiceDatas[i].PassCounterMsb = _passCounterMBS;
                            _passLimitMBS = actualData.Substring(156, 2);
                            serviceDataMultiple.ServiceDatas[i].PassLimitMsb = _passLimitMBS;
                            _passProductCode = actualData.Substring(158, 2);
                            serviceDataMultiple.ServiceDatas[i].PassProductCode = _passProductCode;
                            _loadStation = actualData.Substring(160, 2);
                            serviceDataMultiple.ServiceDatas[i].LoadStationId = _loadStation;
                            _loadSalePoint = actualData.Substring(162, 4);
                            serviceDataMultiple.ServiceDatas[i].LoadSalePointId = _loadSalePoint;
                            _general = actualData.Substring(166, 26);
                            serviceDataMultiple.ServiceDatas[i].Rfu = _general;
                            _panTokenNo = actualData.Substring(192, 64);//actualData.Substring(192, 64);
                            serviceDataMultiple.ServiceDatas[i].PanToken = _panTokenNo;
                            _balance = actualData.Substring(256, 12);//actualData.Substring(256, 12);
                            serviceDataMultiple.ServiceDatas[i].Balance = _balance;
                            //_general = actualData.Substring(154, 32);//actualData.Substring(154, 38); eski hali rfu dan 3 byte aldık
                            //servData.rfu = _general;
                            //_panTokenNo = actualData.Substring(192, 64);//actualData.Substring(192, 64);
                            //servData.panToken = _panTokenNo;
                            //_balance = actualData.Substring(256, 12);//actualData.Substring(256, 12);
                            //servData.balance = _balance;
                            // Kart haritasındaki değişiklikleri ekliyoruz

                            #endregion

                            #region Details Of Terminal Info                       
                            trmId = _terminalInfo.Substring(0, 16);
                            serviceDataMultiple.ServiceDatas[i].LastTerminalId = EmvService.HEX2ASCII(trmId);

                            trmStan = _terminalInfo.Substring(16, 6);
                            serviceDataMultiple.ServiceDatas[i].LastStan = trmStan;

                            trmBatchno = _terminalInfo.Substring(22, 4);
                            serviceDataMultiple.ServiceDatas[i].LastBatch = trmBatchno;

                            trmKM = _terminalInfo.Substring(26, 4);
                            serviceDataMultiple.ServiceDatas[i].LastCity = EmvService.HEX2ASCII(trmKM);
                            #endregion

                            #region Details of Transaction info                     
                            tnxTapInDate = _trxInfo.Substring(0, 12);
                            serviceDataMultiple.ServiceDatas[i].LastTapInDt = tnxTapInDate;

                            tnxAmt = _trxInfo.Substring(12, 12);
                            serviceDataMultiple.ServiceDatas[i].LastFareAmount = tnxAmt;

                            tnxM = _trxInfo.Substring(24, 2);
                            serviceDataMultiple.ServiceDatas[i].LastVehicleType = EmvService.HEX2ASCII(tnxM);

                            tnxdata = _trxInfo.Substring(26, 2);
                            serviceDataMultiple.ServiceDatas[i].LastFareType = tnxdata;

                            tnxType = _trxInfo.Substring(28, 2);
                            serviceDataMultiple.ServiceDatas[i].LastTransactionType = tnxType;

                            #endregion

                            #region Details of From Station                        
                            fmStnEntry = _fromStation.Substring(0, 4);
                            serviceDataMultiple.ServiceDatas[i].FromLineCode = fmStnEntry;

                            fmStnStage = _fromStation.Substring(4, 2);
                            serviceDataMultiple.ServiceDatas[i].FromStageCode = fmStnStage;

                            fmStnId = _fromStation.Substring(6, 2);
                            serviceDataMultiple.ServiceDatas[i].FromStationId = fmStnId;

                            fmStncode = _fromStation.Substring(8, 8);
                            serviceDataMultiple.ServiceDatas[i].FromStationShortCode = EmvService.HEX2ASCII(fmStncode);
                            #endregion

                            #region Details of to Station                      
                            toStnExit = _toStation.Substring(0, 4);
                            serviceDataMultiple.ServiceDatas[i].ToLineCode = toStnExit;

                            toStnStage = _toStation.Substring(4, 2);
                            serviceDataMultiple.ServiceDatas[i].ToStageCode = toStnStage;

                            toStnId = _toStation.Substring(6, 2);
                            serviceDataMultiple.ServiceDatas[i].ToStationId = toStnId;

                            toStncode = _toStation.Substring(8, 8);
                            serviceDataMultiple.ServiceDatas[i].ToStationShortCode = EmvService.HEX2ASCII(toStncode);
                            #endregion

                            #region Details of Last Load Info

                            loadStnId = _loadStation.Substring(0, 2);
                            serviceDataMultiple.ServiceDatas[i].LoadStationId = loadStnId;

                            loadSalePointId = _loadSalePoint.Substring(0, 4);
                            serviceDataMultiple.ServiceDatas[i].LoadSalePointId = loadSalePointId;

                            #endregion

                            serviceDataMultiple.ServiceDatas[i].RespResult.Result = "Success";
                            serviceDataMultiple.ServiceDatas[i].RespResult.ReturnCode = 2;
                            serviceDataMultiple.ServiceDatas[i].RespResult.ReturnDescription = "Success";
                        }
                        else
                        {
                            serviceDataMultiple.ServiceDatas[i].RespResult.Result = "Failed";
                            serviceDataMultiple.ServiceDatas[i].RespResult.ReturnCode = -1;
                            serviceDataMultiple.ServiceDatas[i].RespResult.ReturnDescription = "";
                        }
                    }

                    else
                    {
                        serviceDataMultiple.ServiceDatas[i].RespResult.Result = "Failed";
                        serviceDataMultiple.ServiceDatas[i].RespResult.ReturnCode = -1;
                        serviceDataMultiple.ServiceDatas[i].RespResult.ReturnDescription = "No Card has swiped or Taped";
                    }

                    #endregion
                }

                serviceDataMultiple.RespResult.Result = "Success";
                serviceDataMultiple.RespResult.ReturnCode = 2;
                serviceDataMultiple.RespResult.ReturnDescription = "Success";

                return serviceDataMultiple; ;
            }
            catch (Exception ex)
            {
                serviceDataMultiple.RespResult.Result = "Failed";
                serviceDataMultiple.RespResult.ReturnCode = -1;
                serviceDataMultiple.RespResult.ReturnDescription = ex.ToString();

                return serviceDataMultiple;
            }
            finally
            {
                _port.Close();
            }
        }

        public ServiceData GetServiceData(string portName, ApplicationVersion applicationVersion)
        {
            ServiceData serviceData = new ServiceData();

            try
            {
                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    serviceData.RespResult.Result = "Failed";
                    serviceData.RespResult.ReturnCode = -1;
                    serviceData.RespResult.ReturnDescription = "Failed to open Connection";
                    return serviceData;
                }
                #endregion

                #region Sending Data to Pos

                byte[] crcData = null;

                byte crc = 0;

                byte[] buffer = null;

                byte applicationId = 0;

                switch (applicationVersion)
                {
                    case ApplicationVersion.Rail:
                        applicationId = 0x50;
                        break;
                    case ApplicationVersion.Water:
                        applicationId = 0x51;
                        break;
                }

                //crcData = new byte[] { 250, 161, 13, 1, applicationId };

                crcData = new byte[] { 250, 161, 13, 0 };

                crc = EmvService.CalCheckSum(crcData, crcData.Count());

                //buffer = new byte[] { 250, 161, 13, 1, applicationId, crc, 26, 175 };

                buffer = new byte[] { 250, 161, 13, 0, crc, 26, 175 };

                string sendDataString = EmvService.ByteArrayToHexString(buffer);

                ServiceLog("SEND MESSAGE FOR READ DATA SERVICE :", sendDataString, GetType().Name);

                _port.Write(buffer, 0, buffer.Length);

                #endregion

                #region Code taking less time

                int dataLength = 0;
                int recDataLen = 0;
                byte[] receiveData;
                int k = 0;
                do
                {
                    k += 1;
                    Thread.Sleep(500);
                    dataLength = _port.BytesToRead;
                    receiveData = new byte[dataLength];
                    recDataLen = _port.Read(receiveData, 0, dataLength);
                    if (k == 72) //k==4
                    {
                        break;
                    }

                }
                while (recDataLen < 132);// 1A AF geldimi

                #endregion

                ServiceLog("RESPONSE MESSAGE FOR READ DATA SERVICE :", ByteArrayToHexString(receiveData), GetType().Name);

                ServiceLog("recDataLen :", recDataLen.ToString(), GetType().Name);

                if (recDataLen != 0)
                {
                    //Header
                    string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                    //Msg Type
                    string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                    //Status
                    string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                    //Footer
                    string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                    //CheckSum
                    string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                    //Data Length converted from byte to Hex then to decimal
                    int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data                  
                    //Actual Data  in Bytes
                    byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                    //Actual Data Length in HexaDecimal
                    string msgData = EmvService.ByteArrayToHexString(recMsgData);
                    byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                    string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                    string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                    if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                    {

                        #region Commented
                        //StringBuilder s2=new StringBuilder();
                        //StringBuilder s3 = new StringBuilder();
                        //for (int j = 0; j < recDataLen; j++)
                        //{
                        //    Console.Write(receiveData[j] + " ");

                        //    s2.Append(receiveData[j] + " ");

                        //}
                        //s3 = s2;
                        //string S1 = EmvService.ByteArrayToHexString(receiveData);

                        #endregion

                        byte[] cardData = recMsgData;//.Skip(32).ToArray();
                        string actualData = EmvService.ByteArrayToHexString(cardData);

                        #region Decleration of variable
                        string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _panTokenNo, _balance, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                        string trmId, trmStan, trmBatchno, trmKM = "";
                        string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                        string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                        string toStnExit, toStnStage, toStnId, toStncode = "";
                        string loadStnId, loadSalePointId;

                        #endregion

                        #region Details of All Info
                        _passCounter = actualData.Substring(0, 2);
                        serviceData.PassCounter = _passCounter;
                        _passLimit = actualData.Substring(2, 2);
                        serviceData.PassLimit = _passLimit;
                        _expirayDate = actualData.Substring(4, 4);
                        serviceData.PassExpiryDate = _expirayDate;
                        _effictiveDate = actualData.Substring(8, 4);
                        serviceData.PassEffectiveDate = _effictiveDate;
                        _passType = actualData.Substring(12, 2);
                        serviceData.PassType = _passType;
                        _terminalInfo = actualData.Substring(14, 30);
                        _trxInfo = actualData.Substring(44, 30);
                        _fromStation = actualData.Substring(74, 16);
                        _toStation = actualData.Substring(90, 16);
                        _lastRoute = actualData.Substring(106, 2);
                        serviceData.LastRouteNo = _lastRoute;
                        _passValidity = actualData.Substring(108, 46);
                        serviceData.PassValidityVol = _passValidity;
                        _passCounterMBS = actualData.Substring(154, 2);
                        serviceData.PassCounterMsb = _passCounterMBS;
                        _passLimitMBS = actualData.Substring(156, 2);
                        serviceData.PassLimitMsb = _passLimitMBS;
                        _passProductCode = actualData.Substring(158, 2);
                        serviceData.PassProductCode = _passProductCode;
                        _loadStation = actualData.Substring(160, 2);
                        serviceData.LoadStationId = _loadStation;
                        _loadSalePoint = actualData.Substring(162, 4);
                        serviceData.LoadSalePointId = _loadSalePoint;
                        _general = actualData.Substring(166, 26);
                        serviceData.Rfu = _general;
                        _panTokenNo = actualData.Substring(192, 64);//actualData.Substring(192, 64);
                        serviceData.PanToken = _panTokenNo;
                        _balance = actualData.Substring(256, 12);//actualData.Substring(256, 12);
                        serviceData.Balance = _balance;
                        //_general = actualData.Substring(154, 32);//actualData.Substring(154, 38); eski hali rfu dan 3 byte aldık
                        //servData.rfu = _general;
                        //_panTokenNo = actualData.Substring(192, 64);//actualData.Substring(192, 64);
                        //servData.panToken = _panTokenNo;
                        //_balance = actualData.Substring(256, 12);//actualData.Substring(256, 12);
                        //servData.balance = _balance;
                        // Kart haritasındaki değişiklikleri ekliyoruz

                        #endregion

                        #region Details Of Terminal Info                       
                        trmId = _terminalInfo.Substring(0, 16);
                        serviceData.LastTerminalId = EmvService.HEX2ASCII(trmId);

                        trmStan = _terminalInfo.Substring(16, 6);
                        serviceData.LastStan = trmStan;

                        trmBatchno = _terminalInfo.Substring(22, 4);
                        serviceData.LastBatch = trmBatchno;

                        trmKM = _terminalInfo.Substring(26, 4);
                        serviceData.LastCity = EmvService.HEX2ASCII(trmKM);
                        #endregion

                        #region Details of Transaction info                     
                        tnxTapInDate = _trxInfo.Substring(0, 12);
                        serviceData.LastTapInDt = tnxTapInDate;

                        tnxAmt = _trxInfo.Substring(12, 12);
                        serviceData.LastFareAmount = tnxAmt;

                        tnxM = _trxInfo.Substring(24, 2);
                        serviceData.LastVehicleType = EmvService.HEX2ASCII(tnxM);

                        tnxdata = _trxInfo.Substring(26, 2);
                        serviceData.LastFareType = tnxdata;

                        tnxType = _trxInfo.Substring(28, 2);
                        serviceData.LastTransactionType = tnxType;

                        #endregion

                        #region Details of From Station                        
                        fmStnEntry = _fromStation.Substring(0, 4);
                        serviceData.FromLineCode = fmStnEntry;

                        fmStnStage = _fromStation.Substring(4, 2);
                        serviceData.FromStageCode = fmStnStage;

                        fmStnId = _fromStation.Substring(6, 2);
                        serviceData.FromStationId = fmStnId;

                        fmStncode = _fromStation.Substring(8, 8);
                        serviceData.FromStationShortCode = EmvService.HEX2ASCII(fmStncode);
                        #endregion

                        #region Details of to Station                      
                        toStnExit = _toStation.Substring(0, 4);
                        serviceData.ToLineCode = toStnExit;

                        toStnStage = _toStation.Substring(4, 2);
                        serviceData.ToStageCode = toStnStage;

                        toStnId = _toStation.Substring(6, 2);
                        serviceData.ToStationId = toStnId;

                        toStncode = _toStation.Substring(8, 8);
                        serviceData.ToStationShortCode = EmvService.HEX2ASCII(toStncode);
                        #endregion

                        #region Details of Last Load Info

                        loadStnId = _loadStation.Substring(0, 2);
                        serviceData.LoadStationId = loadStnId;

                        loadSalePointId = _loadSalePoint.Substring(0, 4);
                        serviceData.LoadSalePointId = loadSalePointId;

                        #endregion

                        serviceData.RespResult.Result = "Success";
                        serviceData.RespResult.ReturnCode = 2;
                        serviceData.RespResult.ReturnDescription = "Success";

                        return serviceData;
                    }
                    else
                    {
                        serviceData.RespResult.Result = "Failed";
                        serviceData.RespResult.ReturnCode = -1;
                        serviceData.RespResult.ReturnDescription = "";
                        return serviceData;
                    }
                }
                else
                {
                    serviceData.RespResult.Result = "Failed";
                    serviceData.RespResult.ReturnCode = -1;
                    serviceData.RespResult.ReturnDescription = "No Card has swiped or Taped";
                    return serviceData;
                }
            }
            catch (Exception ex)
            {
                serviceData.RespResult.Result = "Failed";
                serviceData.RespResult.ReturnCode = -1;
                serviceData.RespResult.ReturnDescription = ex.ToString();
                return serviceData;
            }
            finally
            {

                _port.Close();

            }
        }

        public ResponseUpdateService UpdateServiceData(string portName, ServiceData servData)
        {
            ResponseUpdateService respUpdateservice = new ResponseUpdateService();
            // ServiceData servData = new ServiceData();
            try
            {
                ServiceData updatData = new ServiceData();

                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respUpdateservice.RespResult.Result = "Failed";
                    respUpdateservice.RespResult.ReturnCode = -1;
                    respUpdateservice.RespResult.ReturnDescription = "Failed to open Connection";
                    return respUpdateservice;
                }
                #endregion

                #region Variable Declareation
                string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                string trmId, trmStan, trmBatchno, trmKM = "";
                string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                string toStnExit, toStnStage, toStnId, toStncode = "";
                #endregion

                #region Assigning value to Variable

                _passCounter = servData.PassCounter;
                _passLimit = servData.PassLimit;
                _expirayDate = servData.PassExpiryDate;
                _effictiveDate = servData.PassEffectiveDate;
                _passType = servData.PassType;
                _lastRoute = servData.LastRouteNo;
                _passValidity = servData.PassValidityVol;
                _passCounterMBS = servData.PassCounterMsb;
                _passLimitMBS = servData.PassLimitMsb;
                _passProductCode = servData.PassProductCode;
                _loadStation = servData.LoadStationId;
                _loadSalePoint = servData.LoadSalePointId;
                _general = servData.Rfu;

                trmId = servData.LastTerminalId;
                trmStan = servData.LastStan;
                trmBatchno = servData.LastBatch;
                trmKM = servData.LastCity;

                tnxTapInDate = servData.LastTapInDt;
                tnxAmt = servData.LastFareAmount;
                tnxM = servData.LastVehicleType;
                tnxdata = servData.LastFareType;
                tnxType = servData.LastTransactionType;

                fmStnEntry = servData.FromLineCode;
                fmStnStage = servData.FromStageCode;
                fmStnId = servData.FromStationId;
                fmStncode = servData.FromStationShortCode;

                toStnExit = servData.ToLineCode;
                toStnStage = servData.ToStageCode;
                toStnId = servData.ToStationId;
                toStncode = servData.ToStationShortCode;


                #endregion

                _terminalInfo = EmvService.ASCIITOHex(trmId) + trmStan + trmBatchno + EmvService.ASCIITOHex(trmKM);
                _trxInfo = tnxTapInDate + tnxAmt + EmvService.ASCIITOHex(tnxM) + tnxdata + tnxType;
                _fromStation = fmStnEntry + fmStnStage + fmStnId + EmvService.ASCIITOHex(fmStncode);
                _toStation = toStnExit + toStnStage + toStnId + EmvService.ASCIITOHex(toStncode);

                byte[] headerDetails = new byte[] { 0xFA, 0xA1, 0x0F, 96 };

                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails);

                byte[] dataDetails = new byte[100];
                //"FAA10F60"

                string all_ = headerDetailshexa + _passCounter + _passLimit + _expirayDate + _effictiveDate + _passType + _terminalInfo + _trxInfo + _fromStation + _toStation + _lastRoute + _passValidity + _passCounterMBS + _passLimitMBS + _passProductCode + _loadStation + _loadSalePoint + _general;

                dataDetails = EmvService.HexStringToByteArray(all_);

                if (dataDetails.Count() == 100)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());

                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };

                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 

                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);

                    //ms.Write(headerDetails, 0, headerDetails.Length);  

                    ms.Write(dataDetails, 0, dataDetails.Length);

                    ms.Write(fotterDetails, 0, fotterDetails.Length);

                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);

                    byte[] buffer = HexStringToByteArray(sendCrc);

                    _port.Write(buffer, 0, buffer.Length);

                    ServiceLog("SEND MESSAGE FOR UPDATE DATA SERVICE :", sendCrc, GetType().Name);

                    #region Code taking less time

                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    String resultByte = "";
                    int resultNumber;

                    do
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 72)//k==16
                        {
                            // Console.WriteLine("Card Not Tap");
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR UPDATE IN WHILE BEFORE PIN SCREEN :", ByteArrayToHexString(receiveData), GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[2]);
                        }
                    }
                    while (resultByte != "16");

                    resultNumber = k;
                    k = 1;

                    //PIN ENTERED
                    while (resultNumber != 72 && resultByte != "17")
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 72)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR UPDATE IN WHILE PIN ENTERED :", ByteArrayToHexString(receiveData), GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[2]);
                        }
                    }

                    resultNumber = k;
                    k = 1;

                    //LISTEN FOR UPDATe
                    while (resultNumber != 72 && recDataLen < 30)
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 144)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR UPDATE IN WHILE UPDATE:", ByteArrayToHexString(receiveData), GetType().Name);
                        }
                    }

                    ServiceLog("RESPONSE MESSAGE FOR UPDATE DATA SERVICE :", ByteArrayToHexString(receiveData), GetType().Name);

                    #endregion

                    if (resultNumber != 72 && recDataLen == 40)
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data                  
                                                                                                             //Actual Data  in Bytes
                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        string msgData = EmvService.ByteArrayToHexString(recMsgData);

                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();

                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);

                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                        if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {
                            //string panTokenhex = EmvService.ByteArrayToHexString(recMsgData);
                            respUpdateservice.PanTokenNo = EmvService.ByteArrayToHexString(recMsgData);
                            respUpdateservice.RespResult.Result = "Success";
                            respUpdateservice.RespResult.ReturnCode = 2;
                            respUpdateservice.RespResult.ReturnDescription = "Success";
                            return respUpdateservice;
                        }
                    }

                    else if (recDataLen > 0 && recDataLen < 40)
                    {
                        respUpdateservice.RespResult.Result = "Failed";
                        respUpdateservice.RespResult.ReturnCode = -1;
                        respUpdateservice.RespResult.ReturnDescription = "There is problem in Card";
                        respUpdateservice.RespResult.ErrorDetails = "The Response Data must be 40 bytes but in response we have recived " + recDataLen.ToString();
                        return respUpdateservice;
                    }

                    else if (recDataLen == 0)
                    {
                        respUpdateservice.RespResult.Result = "Failed";
                        respUpdateservice.RespResult.ReturnCode = -1;
                        respUpdateservice.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respUpdateservice.RespResult.ErrorDetails = " Default time for Tap & Update service data has given 1 min";
                        return respUpdateservice;
                    }
                }
                else
                {
                    respUpdateservice.RespResult.Result = "Failed";
                    respUpdateservice.RespResult.ReturnCode = -1;
                    respUpdateservice.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respUpdateservice.RespResult.ErrorDetails = "The input bytes must be 100  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respUpdateservice;
                }
                return respUpdateservice;
            }
            catch (Exception ex)
            {
                respUpdateservice.RespResult.Result = "Failed";
                respUpdateservice.RespResult.ReturnCode = -1;
                respUpdateservice.RespResult.ReturnDescription = ex.ToString();
                return respUpdateservice;
            }
            finally
            {
                _port.Close();
            }
        }

        public ResponseUpdateService UpdateServiceDataV2(string portName, ServiceData servData, byte tranType, int transactionId, ApplicationVersion applicationVersion)
        {
            ResponseUpdateService respUpdateservice = new ResponseUpdateService();
            // ServiceData servData = new ServiceData();
            try
            {
                ServiceData updatData = new ServiceData();

                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respUpdateservice.RespResult.Result = "Failed";
                    respUpdateservice.RespResult.ReturnCode = -1;
                    respUpdateservice.RespResult.ReturnDescription = "Failed to open Connection";
                    return respUpdateservice;
                }
                #endregion

                #region Variable Declareation
                string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                string trmId, trmStan, trmBatchno, trmKM = "";
                string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                string toStnExit, toStnStage, toStnId, toStncode = "";
                #endregion

                #region Assigning value to Variable

                _passCounter = servData.PassCounter;
                _passLimit = servData.PassLimit;
                _expirayDate = servData.PassExpiryDate;
                _effictiveDate = servData.PassEffectiveDate;
                _passType = servData.PassType;
                _lastRoute = servData.LastRouteNo;
                _passValidity = servData.PassValidityVol;
                _passCounterMBS = servData.PassCounterMsb;
                _passLimitMBS = servData.PassLimitMsb;
                _passProductCode = servData.PassProductCode;
                _loadStation = servData.LoadStationId;
                _loadSalePoint = servData.LoadSalePointId;
                _general = servData.Rfu;

                trmId = servData.LastTerminalId;
                trmStan = servData.LastStan;
                trmBatchno = servData.LastBatch;
                trmKM = servData.LastCity;

                tnxTapInDate = servData.LastTapInDt;
                tnxAmt = servData.LastFareAmount;
                tnxM = servData.LastVehicleType;
                tnxdata = servData.LastFareType;
                tnxType = servData.LastTransactionType;

                fmStnEntry = servData.FromLineCode;
                fmStnStage = servData.FromStageCode;
                fmStnId = servData.FromStationId;
                fmStncode = servData.FromStationShortCode;

                toStnExit = servData.ToLineCode;
                toStnStage = servData.ToStageCode;
                toStnId = servData.ToStationId;
                toStncode = servData.ToStationShortCode;


                #endregion

                _terminalInfo = EmvService.ASCIITOHex(trmId) + trmStan + trmBatchno + EmvService.ASCIITOHex(trmKM);
                _trxInfo = tnxTapInDate + tnxAmt + EmvService.ASCIITOHex(tnxM) + tnxdata + tnxType;
                _fromStation = fmStnEntry + fmStnStage + fmStnId + EmvService.ASCIITOHex(fmStncode);
                _toStation = toStnExit + toStnStage + toStnId + EmvService.ASCIITOHex(toStncode);

                byte[] headerDetails = null;

                string headerDetailshexa = "";

                int dataCount = 0;

                string applicationId = "";

                dataCount = 111;

                headerDetails = new byte[] { 0xFA, 0xA1, 0x0F, 107 };

                switch (applicationVersion)
                {
                    case ApplicationVersion.Rail:
                        applicationId = "50";
                        break;
                    case ApplicationVersion.Water:
                        applicationId = "51";
                        break;
                }

                headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails);

                //"FAA10F60"

                string strTranType = EmvService.ByteToHexString(tranType);

                if (strTranType.Length == 1)
                {
                    strTranType = "0" + strTranType;
                }

                string tranId = Convert.ToString(transactionId);

                var tranIdMessage = tranId.PadLeft(9, '0');

                string all = headerDetailshexa + applicationId + strTranType + EmvService.ASCIITOHex(tranIdMessage) + _passCounter + _passLimit + _expirayDate + _effictiveDate + _passType + _terminalInfo + _trxInfo + _fromStation + _toStation + _lastRoute + _passValidity + _passCounterMBS + _passLimitMBS + _passProductCode + _loadStation + _loadSalePoint + _general; ;

                byte[] dataDetails = EmvService.HexStringToByteArray(all);

                if (dataDetails.Count() == dataCount)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());

                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };

                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 

                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);

                    //ms.Write(headerDetails, 0, headerDetails.Length);  

                    ms.Write(dataDetails, 0, dataDetails.Length);

                    ms.Write(fotterDetails, 0, fotterDetails.Length);

                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);

                    byte[] buffer = HexStringToByteArray(sendCrc);

                    _port.Write(buffer, 0, buffer.Length);

                    ServiceLog("SEND MESSAGE FOR UPDATE DATA SERVICE :", sendCrc, GetType().Name);

                    #region Code taking less time

                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    String resultByte = "";
                    int resultNumber;

                    do
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 72)//k==16
                        {
                            // Console.WriteLine("Card Not Tap");
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR UPDATE IN WHILE BEFORE PIN SCREEN :", ByteArrayToHexString(receiveData), GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[2]);
                        }
                    }
                    while (resultByte != "16");

                    resultNumber = k;
                    k = 1;

                    //PIN ENTERED
                    while (resultNumber != 72 && resultByte != "17")
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 72)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR UPDATE IN WHILE PIN ENTERED :", ByteArrayToHexString(receiveData), GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[2]);
                        }
                    }

                    resultNumber = k;
                    k = 1;

                    //LISTEN FOR UPDATe
                    while (resultNumber != 72 && recDataLen < 30)
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 144)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR UPDATE IN WHILE UPDATE:", ByteArrayToHexString(receiveData), GetType().Name);
                        }
                    }

                    ServiceLog("RESPONSE MESSAGE FOR UPDATE DATA SERVICE :", ByteArrayToHexString(receiveData), GetType().Name);

                    #endregion

                    if (resultNumber != 72 && recDataLen == 40)
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data                  
                                                                                                             //Actual Data  in Bytes
                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        string msgData = EmvService.ByteArrayToHexString(recMsgData);

                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();

                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);

                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                        if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {
                            //string panTokenhex = EmvService.ByteArrayToHexString(recMsgData);
                            respUpdateservice.PanTokenNo = EmvService.ByteArrayToHexString(recMsgData);
                            respUpdateservice.RespResult.Result = "Success";
                            respUpdateservice.RespResult.ReturnCode = 2;
                            respUpdateservice.RespResult.ReturnDescription = "Success";
                            return respUpdateservice;
                        }
                    }

                    else if (recDataLen > 0 && recDataLen < 40)
                    {
                        respUpdateservice.RespResult.Result = "Failed";
                        respUpdateservice.RespResult.ReturnCode = -1;
                        respUpdateservice.RespResult.ReturnDescription = "There is problem in Card";
                        respUpdateservice.RespResult.ErrorDetails = "The Response Data must be 40 bytes but in response we have recived " + recDataLen.ToString();
                        return respUpdateservice;
                    }

                    else if (recDataLen == 0)
                    {
                        respUpdateservice.RespResult.Result = "Failed";
                        respUpdateservice.RespResult.ReturnCode = -1;
                        respUpdateservice.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respUpdateservice.RespResult.ErrorDetails = " Default time for Tap & Update service data has given 1 min";
                        return respUpdateservice;
                    }
                }
                else
                {
                    respUpdateservice.RespResult.Result = "Failed";
                    respUpdateservice.RespResult.ReturnCode = -1;
                    respUpdateservice.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respUpdateservice.RespResult.ErrorDetails = "The input bytes must be 100  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respUpdateservice;
                }
                return respUpdateservice;
            }
            catch (Exception ex)
            {
                respUpdateservice.RespResult.Result = "Failed";
                respUpdateservice.RespResult.ReturnCode = -1;
                respUpdateservice.RespResult.ReturnDescription = ex.ToString();
                return respUpdateservice;
            }
            finally
            {
                _port.Close();
            }
        }

        public ResponseTopUp TopUp(string portName, Double amount, PosMessageType posMessageType)
        {

            ResponseTopUp respTopup = new ResponseTopUp();
            try
            {
                ServiceLog("INFO", $"BEGIN OF TOP-UP TRANSACTION", GetType().Name);
                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Failed to open Connection";
                    return respTopup;
                }
                #endregion

                string baseAmt = "000000000000";
                byte[] headerDetails = new byte[] { 250, 161, (byte)posMessageType, 12 };
                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails);

                string msgData = Convert.ToString(Convert.ToInt32(amount) * 100);
                msgData = baseAmt.Remove(baseAmt.Length - msgData.Length, msgData.Length) + msgData;
                string all_ = headerDetailshexa + EmvService.ASCIITOHex(msgData);
                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(msgData));

                if (dataDetails.Count() == 16)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());// footer ve crc hariç
                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };
                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 
                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);
                    ms.Write(dataDetails, 0, dataDetails.Length);
                    ms.Write(fotterDetails, 0, fotterDetails.Length);
                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);
                    byte[] buffer = HexStringToByteArray(sendCrc);
                    _port.Write(buffer, 0, buffer.Length);
                    ServiceLog("SEND MESSAGE FOR TOP-UP :", sendCrc, GetType().Name);
                    #region Code taking less time
                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    int resultNumber = 0;
                    String resultByte = "";
                    do
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 72)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR TOP-UP IN WHILE BEFORE PIN SCREEN :", ByteArrayToHexString(receiveData), GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[2]);
                        }

                    }
                    while (resultByte != "16");

                    resultNumber = k;
                    k = 1;

                    //PIN ENTERED
                    while (resultNumber != 72 && resultByte != "17")
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 72)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR TOP-UP IN WHILE PIN ENTERED :", ByteArrayToHexString(receiveData), GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[2]);
                        }
                    }

                    resultNumber = k;
                    k = 1;

                    //LISTEN HOST TOPUP
                    while (resultNumber != 72 && recDataLen < 60)
                    {
                        k += 1;
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 240)//k==16
                        {
                            break;
                        }
                        if (recDataLen != 0)
                        {
                            ServiceLog("RESPONSE MESSAGE FOR TOP-UP IN WHILE HOST :", ByteArrayToHexString(receiveData), GetType().Name);
                        }
                    }

                    ServiceLog("K && Result Number && recDataLen After Host :", $"{k} - {resultNumber} - {recDataLen}", GetType().Name);
                    #endregion

                    ServiceLog("RESPONSE MESSAGE FOR TOP-UP :", ByteArrayToHexString(receiveData), GetType().Name);
                    if (resultNumber != 72 && recDataLen > 62) //64 toplam data length header+data+footer
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data                  
                                                                                                             //Actual Data  in Bytes
                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                        if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {
                            string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Take(32).ToArray());
                            string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(32).Take(12).ToArray());
                            string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(44).Take(12).ToArray());
                            #region Respons of Topup
                            respTopup.Amt = amt;
                            respTopup.PanTokenNo = panTokenNo;
                            respTopup.Rrn = rrnNo;
                            respTopup.RespResult.Result = "Success";
                            respTopup.RespResult.ReturnCode = 2;
                            respTopup.RespResult.ReturnDescription = "Success";
                            #endregion
                            return respTopup;
                        }
                        else
                        {
                            respTopup.RespResult.Result = "Failed";
                            respTopup.RespResult.ReturnCode = -1;
                            respTopup.RespResult.ReturnDescription = "Lrc not matched";
                            return respTopup;
                        }
                    }
                    else if (recDataLen > 0 && recDataLen < 64)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "There is problem in Card";
                        respTopup.RespResult.ErrorDetails = "The Response Data must be 64 bytes but in response we have recived " + recDataLen.ToString();
                        return respTopup;
                    }
                    else if (recDataLen == 0)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respTopup.RespResult.ErrorDetails = "Default time for Tap & TopUp service data has given 2 min";
                        return respTopup;
                    }
                }
                else
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respTopup.RespResult.ErrorDetails = "The input bytes must be 100  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respTopup;
                }
                return respTopup;
            }
            catch (Exception ex)
            {
                respTopup.RespResult.Result = "Failed";
                respTopup.RespResult.ReturnCode = -1;
                respTopup.RespResult.ReturnDescription = ex.ToString();
                return respTopup;
            }
            finally
            {
                ServiceLog("INFO", $"END OF TOP-UP TRANSACTION", GetType().Name);
                _port.Close();
            }
        }

        public ResponseTopUp TopUpV2(string portName, int transactionId, Double amount, string paymentType, PosMessageType posMessageType)
        {

            ResponseTopUp respTopup = new ResponseTopUp();
            try
            {
                ServiceLog("TOP-UP", $"BEGIN OF TOP-UP TRANSACTION", GetType().Name);

                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Failed to open Connection";
                    return respTopup;
                }
                #endregion

                byte[] headerDetails = new byte[] { 250, 161, (byte)posMessageType, 27 };
                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails);

                string tranId = Convert.ToString(transactionId);
                string amountStr = Convert.ToString(Convert.ToInt32(amount) * 100);

                var tranIdMessage = tranId.PadLeft(9, '0');
                var amountStrMessage = amountStr.PadLeft(12, '0');

                var msgData = tranIdMessage + amountStrMessage + paymentType;

                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(msgData));

                if (dataDetails.Count() == 31)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());// footer ve crc hariç
                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };
                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 
                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);
                    ms.Write(dataDetails, 0, dataDetails.Length);
                    ms.Write(fotterDetails, 0, fotterDetails.Length);
                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);
                    byte[] buffer = HexStringToByteArray(sendCrc);
                    _port.Write(buffer, 0, buffer.Length);
                    ServiceLog("TOP-UP", $"SEND MESSAGE FOR TOP-UP : {sendCrc}", GetType().Name);


                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData = new byte[] { };
                    int k = 0;
                    int resultNumber = 0;
                    String resultByte = "";
                    String resultHostMessageByte = "";

                    while (k < 72)
                    {
                        Thread.Sleep(500);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (recDataLen != 0)
                        {
                            ServiceLog("TOP-UP", $"RESPONSE MESSAGE FOR TOP-UP IN WHILE BEFORE PIN SCREEN : {ByteArrayToHexString(receiveData)}", GetType().Name);
                            resultByte = EmvService.ByteToHexString(receiveData[3]); // Success : 00    - Error : 88
                            if (resultByte == "00" || resultByte == "88")
                            {
                                break;
                            }
                        }

                        k++;
                    }

                    if (resultByte == "00")
                    {
                        resultNumber = k;
                        k = 1;

                        //PIN ENTERED
                        while (resultNumber != 72 && resultByte != "17")
                        {
                            k += 1;
                            Thread.Sleep(500);
                            dataLength = _port.BytesToRead;
                            receiveData = new byte[dataLength];
                            recDataLen = _port.Read(receiveData, 0, dataLength);
                            if (k == 72)
                            {
                                break;
                            }
                            if (recDataLen != 0)
                            {
                                ServiceLog("TOP-UP", $"RESPONSE MESSAGE FOR TOP-UP IN WHILE PIN ENTERED : {ByteArrayToHexString(receiveData)}", GetType().Name);
                                resultByte = EmvService.ByteToHexString(receiveData[2]);
                            }
                        }

                        resultNumber = k;
                        k = 1;

                        //LISTEN HOST TOPUP
                        while (resultNumber != 72 && recDataLen < 60)
                        {
                            k += 1;
                            Thread.Sleep(500);
                            dataLength = _port.BytesToRead;
                            receiveData = new byte[dataLength];
                            recDataLen = _port.Read(receiveData, 0, dataLength);
                            if (k == 240)//k==16
                            {
                                break;
                            }
                            if (recDataLen != 0)
                            {
                                ServiceLog("TOP-UP", $"RESPONSE MESSAGE FOR TOP-UP IN WHILE HOST : {ByteArrayToHexString(receiveData)}", GetType().Name);
                                resultHostMessageByte = EmvService.ByteToHexString(receiveData[3]);//00 success - 88 error
                                if (resultHostMessageByte == "00" || resultHostMessageByte == "88")
                                {
                                    break;
                                }
                            }
                        }

                        ServiceLog("TOP-UP", $"Count : {k} - resultNumber : {resultNumber} - recDataLen : {recDataLen} - resultHostMessageByte : {resultHostMessageByte}", GetType().Name);
                        ServiceLog("TOP-UP", $"RESPONSE MESSAGE FOR TOP-UP : {ByteArrayToHexString(receiveData)}", GetType().Name);

                        if (resultNumber != 72 && recDataLen > 62) //64 toplam data length header+data+footer
                        {
                            //Header
                            string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                       //Msg Type
                            string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                           //Status
                            string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                             //Footer
                            string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                                 //CheckSum
                            string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                            //Data Length converted from byte to Hex then to decimal
                            int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data                  
                                                                                                                 //Actual Data  in Bytes
                            byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                            byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                            string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                            string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                            if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                            {
                                string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Take(32).ToArray());
                                string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(32).Take(12).ToArray());
                                string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(44).Take(12).ToArray());
                                #region Respons of Topup
                                respTopup.Amt = amt;
                                respTopup.PanTokenNo = panTokenNo;
                                respTopup.Rrn = rrnNo;
                                respTopup.RespResult.Result = "Success";
                                respTopup.RespResult.ReturnCode = 2;
                                respTopup.RespResult.ReturnDescription = "Success";
                                #endregion
                                return respTopup;
                            }
                            else
                            {
                                respTopup.RespResult.Result = "Failed";
                                respTopup.RespResult.ReturnCode = -1;
                                respTopup.RespResult.ReturnDescription = "Lrc not matched";
                                return respTopup;
                            }
                        }
                        else if (recDataLen > 0 && recDataLen < 64)
                        {
                            respTopup.RespResult.Result = "Failed";
                            respTopup.RespResult.ReturnCode = -1;
                            respTopup.RespResult.ReturnDescription = "Topup failed or message couldn't send";
                            respTopup.RespResult.ErrorDetails = "Topup failed or message couldn't send" + " recDataLen : " + recDataLen.ToString();
                            return respTopup;
                        }
                        else if (recDataLen == 0)
                        {
                            respTopup.RespResult.Result = "Failed";
                            respTopup.RespResult.ReturnCode = -1;
                            respTopup.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                            respTopup.RespResult.ErrorDetails = "Default time for Tap & TopUp service data has given 2 min";
                            return respTopup;
                        }
                    }
                    else
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "Topup failed or message couldn't send";
                        respTopup.RespResult.ErrorDetails = "Topup failed or message couldn't send";
                        return respTopup;
                    }
                }
                else
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respTopup.RespResult.ErrorDetails = "The input bytes must be 100  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respTopup;
                }
                return respTopup;
            }
            catch (Exception ex)
            {
                respTopup.RespResult.Result = "Failed";
                respTopup.RespResult.ReturnCode = -1;
                respTopup.RespResult.ReturnDescription = ex.ToString();
                return respTopup;
            }
            finally
            {
                ServiceLog("TOP-UP", $"END OF TOP-UP TRANSACTION", GetType().Name);
                _port.Close();
            }
        }

        public ResponseTopUp OnlineDebits(string portName, ServiceData servData, Double amount)
        {
            ResponseTopUp respTopup = new ResponseTopUp();

            try
            {
                ServiceData updatData = new ServiceData();

                #region PortDetails

                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;

                #endregion

                #region Connection Code

                if (_port.IsOpen)
                    _port.Close();

                _port.Open();

                if (!_port.IsOpen)
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Failed to open Connection";
                    return respTopup;
                }

                #endregion

                #region Variable Declareation

                string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                string trmId, trmStan, trmBatchno, trmKM = "";
                string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                string toStnExit, toStnStage, toStnId, toStncode = "";

                #endregion

                #region Assigning value to Variable

                _passCounter = servData.PassCounter;
                _passLimit = servData.PassLimit;
                _expirayDate = servData.PassExpiryDate;
                _effictiveDate = servData.PassEffectiveDate;
                _passType = servData.PassType;
                _lastRoute = servData.LastRouteNo;
                _passValidity = servData.PassValidityVol;
                _passCounterMBS = servData.PassCounterMsb;
                _passLimitMBS = servData.PassLimitMsb;
                _passProductCode = servData.PassProductCode;
                _loadStation = servData.LoadStationId;
                _loadSalePoint = servData.LoadSalePointId;
                _general = servData.Rfu;

                trmId = servData.LastTerminalId;
                trmStan = servData.LastStan;
                trmBatchno = servData.LastBatch;
                trmKM = servData.LastCity;

                tnxTapInDate = servData.LastTapInDt;
                tnxAmt = servData.LastFareAmount;
                tnxM = servData.LastVehicleType;
                tnxdata = servData.LastFareType;
                tnxType = servData.LastTransactionType;

                fmStnEntry = servData.FromLineCode;
                fmStnStage = servData.FromStageCode;
                fmStnId = servData.FromStationId;
                fmStncode = servData.FromStationShortCode;

                toStnExit = servData.ToLineCode;
                toStnStage = servData.ToStageCode;
                toStnId = servData.ToStationId;
                toStncode = servData.ToStationShortCode;
                #endregion

                _terminalInfo = EmvService.ASCIITOHex(trmId) + trmStan + trmBatchno + EmvService.ASCIITOHex(trmKM);
                _trxInfo = tnxTapInDate + tnxAmt + EmvService.ASCIITOHex(tnxM) + tnxdata + tnxType;
                _fromStation = fmStnEntry + fmStnStage + fmStnId + EmvService.ASCIITOHex(fmStncode);
                _toStation = toStnExit + toStnStage + toStnId + EmvService.ASCIITOHex(toStncode);

                string baseAmt = "000000000000";
                byte[] headerDetails = new byte[] { 250, 161, 0x09, 0x6C };
                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails) + _passCounter + _passLimit + _expirayDate + _effictiveDate + _passType + _terminalInfo + _trxInfo + _fromStation + _toStation + _lastRoute + _passValidity + _passCounterMBS + _passLimitMBS + _passProductCode + _loadStation + _loadSalePoint + _general;

                string msgData = Convert.ToString(Convert.ToInt32(amount) * 100);
                msgData = baseAmt.Remove(baseAmt.Length - msgData.Length, msgData.Length) + msgData;
                string all_ = headerDetailshexa + EmvService.ASCIITOHex(msgData);
                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(msgData));

                ServiceLog("INFO", $"DATA COUNT FOR ONLINE : {dataDetails.Length}", GetType().Name);

                if (dataDetails.Count() == 112)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());
                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };
                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 
                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);
                    ms.Write(dataDetails, 0, dataDetails.Length);
                    ms.Write(fotterDetails, 0, fotterDetails.Length);
                    byte[] sendData = ms.GetBuffer();
                    string sendCrc = EmvService.ByteArrayToHexString(sendData);
                    byte[] buffer = HexStringToByteArray(sendCrc);
                    _port.Write(buffer, 0, buffer.Length);
                    ServiceLog("SEND MESSAGE FOR ONLINE DEBIT :", sendCrc, GetType().Name);

                    #region Code taking less time

                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    do
                    {
                        k += 1;
                        Thread.Sleep(5000);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 36)//k==16
                        {
                            // Console.WriteLine("Card Not Tap");
                            break;
                        }
                    }
                    while (recDataLen < 8);

                    #endregion

                    ServiceLog("RESPONSE MESSAGE FOR ONLINE DEBIT :", ByteArrayToHexString(receiveData), GetType().Name);
                    if (recDataLen > 7) //64
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data  Actual Data  in Bytes                 

                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        //string msgData = EmvService.ByteArrayToHexString(recMsgData);
                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));
                        if (recHeader == "FAA1" && recFooter == "1AAF" && recCmdStatus != "88" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {
                            string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Take(32).ToArray());
                            string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(32).Take(12).ToArray());
                            string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(44).Take(6).ToArray());
                            #region Respons of Topup

                            respTopup.Rrn = rrnNo;
                            respTopup.Amt = amt;
                            respTopup.PanTokenNo = panTokenNo;
                            respTopup.RespResult.Result = "Success";
                            respTopup.RespResult.ReturnCode = 2;
                            respTopup.RespResult.ReturnDescription = "Success";
                            #endregion
                            #region Details of All Info
                            //respTopup.panTokenNo = actualData.Substring(0, 64);
                            //respTopup.rrn = actualData.Substring(64, 24);
                            //respTopup.amt = actualData.Substring(88, 24);

                            //respTopup.respResult.Result = "Success";
                            //respTopup.respResult.ReturnCode = 2;
                            //respTopup.respResult.ReturnDescription = "Success";
                            #endregion
                            return respTopup;
                        }
                        else
                        {
                            respTopup.RespResult.Result = "Failed";
                            respTopup.RespResult.ReturnCode = -1;
                            respTopup.RespResult.ReturnDescription = "Crc not matched";
                            return respTopup;
                        }
                    }
                    else if (recDataLen > 0 && recDataLen < 64)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "There is problem in Card";
                        respTopup.RespResult.ErrorDetails = "The Response Data must be 96 bytes but in response we have recived " + recDataLen.ToString();
                        return respTopup;
                    }
                    else if (recDataLen == 0)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respTopup.RespResult.ErrorDetails = "Default time for Tap & TopUp service data has given 2 min";
                        return respTopup;
                    }
                }
                else
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respTopup.RespResult.ErrorDetails = "The input bytes must be 112  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respTopup;
                }
                return respTopup;
            }
            catch (Exception ex)
            {
                respTopup.RespResult.Result = "Failed";
                respTopup.RespResult.ReturnCode = -1;
                respTopup.RespResult.ReturnDescription = ex.ToString();
                return respTopup;
            }
            finally
            {
                _port.Close();
            }
        }

        public ResponseTopUp OnlineDebitsV2(string portName, int tranType, int transactionId, ServiceData servData, Double amount)
        {
            ResponseTopUp respTopup = new ResponseTopUp();

            try
            {
                ServiceLog("TOP-UP", $"BEGIN OF ONLINE DEBIT TRANSACTION", GetType().Name);

                ServiceData updatData = new ServiceData();

                #region PortDetails

                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;

                #endregion

                #region Connection Code

                if (_port.IsOpen)
                    _port.Close();

                _port.Open();

                if (!_port.IsOpen)
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Failed to open Connection";
                    return respTopup;
                }
                #endregion

                #region Variable Declareation

                string _transactionId, _tranType;
                string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _passCounterMBS, _passLimitMBS, _passProductCode, _loadStation, _loadSalePoint;
                string trmId, trmStan, trmBatchno, trmKM;
                string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType;
                string fmStnEntry, fmStnStage, fmStnId, fmStncode;
                string toStnExit, toStnStage, toStnId, toStncode;
                #endregion

                #region Assigning value to Variable

                _passCounter = servData.PassCounter;
                _passLimit = servData.PassLimit;
                _expirayDate = servData.PassExpiryDate;
                _effictiveDate = servData.PassEffectiveDate;
                _passType = servData.PassType;
                _lastRoute = servData.LastRouteNo;
                _passValidity = servData.PassValidityVol;
                _passCounterMBS = servData.PassCounterMsb;
                _passLimitMBS = servData.PassLimitMsb;
                _passProductCode = servData.PassProductCode;
                _loadStation = servData.LoadStationId;
                _loadSalePoint = servData.LoadSalePointId;
                _general = servData.Rfu;

                trmId = servData.LastTerminalId;
                trmStan = servData.LastStan;
                trmBatchno = servData.LastBatch;
                trmKM = servData.LastCity;

                tnxTapInDate = servData.LastTapInDt;
                tnxAmt = servData.LastFareAmount;
                tnxM = servData.LastVehicleType;
                tnxdata = servData.LastFareType;
                tnxType = servData.LastTransactionType;

                fmStnEntry = servData.FromLineCode;
                fmStnStage = servData.FromStageCode;
                fmStnId = servData.FromStationId;
                fmStncode = servData.FromStationShortCode;

                toStnExit = servData.ToLineCode;
                toStnStage = servData.ToStageCode;
                toStnId = servData.ToStationId;
                toStncode = servData.ToStationShortCode;
                #endregion

                _terminalInfo = EmvService.ASCIITOHex(trmId) + trmStan + trmBatchno + EmvService.ASCIITOHex(trmKM);
                _trxInfo = tnxTapInDate + tnxAmt + EmvService.ASCIITOHex(tnxM) + tnxdata + tnxType;
                _fromStation = fmStnEntry + fmStnStage + fmStnId + EmvService.ASCIITOHex(fmStncode);
                _toStation = toStnExit + toStnStage + toStnId + EmvService.ASCIITOHex(toStncode);

                _transactionId = Convert.ToString(transactionId);
                _transactionId = _transactionId.PadLeft(9, '0');
                _transactionId = ASCIITOHex(_transactionId);

                _tranType = tranType.ToString("X");

                string baseAmt = "000000000000";

                byte[] headerDetails = null;
                string headerDetailshexa = "";
                int dataCount = 0;

                dataCount = 123;

                headerDetails = new byte[] { 250, 161, 0x09, 0x77 };

                string applicationId = "50";

                switch (Constants.ApplicationVersion)
                {
                    case ApplicationVersion.Rail:
                        applicationId = "50";
                        break;
                    case ApplicationVersion.Water:
                        applicationId = "51";
                        break;
                }

                headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails) + applicationId + _tranType + _transactionId + _passCounter + _passLimit + _expirayDate + _effictiveDate + _passType + _terminalInfo + _trxInfo + _fromStation + _toStation + _lastRoute + _passValidity + _passCounterMBS + _passLimitMBS + _passProductCode + _loadStation + _loadSalePoint + _general;

                string msgData = Convert.ToString(Convert.ToInt32(amount) * 100);

                msgData = baseAmt.Remove(baseAmt.Length - msgData.Length, msgData.Length) + msgData;

                string all = headerDetailshexa + EmvService.ASCIITOHex(msgData);

                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(msgData));

                ServiceLog("INFO", $"DATA COUNT FOR ONLINE : {dataDetails.Length}", GetType().Name);

                if (dataDetails.Count() == dataCount)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());

                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };

                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 

                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);

                    ms.Write(dataDetails, 0, dataDetails.Length);

                    ms.Write(fotterDetails, 0, fotterDetails.Length);

                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);

                    byte[] buffer = HexStringToByteArray(sendCrc);

                    _port.Write(buffer, 0, buffer.Length);

                    ServiceLog("SEND MESSAGE FOR ONLINE DEBIT :", sendCrc, GetType().Name);

                    #region Code taking less time

                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    do
                    {
                        k += 1;
                        Thread.Sleep(5000);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 36)//k==16
                        {
                            // Console.WriteLine("Card Not Tap");
                            break;
                        }
                    }
                    while (recDataLen < 8);

                    #endregion

                    ServiceLog("RESPONSE MESSAGE FOR ONLINE DEBIT :", ByteArrayToHexString(receiveData), GetType().Name);

                    if (recDataLen > 7) //64
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data  Actual Data  in Bytes                 

                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        //string msgData = EmvService.ByteArrayToHexString(recMsgData);
                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));
                        if (recHeader == "FAA1" && recFooter == "1AAF" && recCmdStatus != "88" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {
                            string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Take(32).ToArray());
                            string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(32).Take(12).ToArray());
                            string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(44).Take(6).ToArray());
                            #region Respons of Topup

                            respTopup.Rrn = rrnNo;
                            respTopup.Amt = amt;
                            respTopup.PanTokenNo = panTokenNo;
                            respTopup.RespResult.Result = "Success";
                            respTopup.RespResult.ReturnCode = 2;
                            respTopup.RespResult.ReturnDescription = "Success";
                            #endregion
                            #region Details of All Info
                            //respTopup.panTokenNo = actualData.Substring(0, 64);
                            //respTopup.rrn = actualData.Substring(64, 24);
                            //respTopup.amt = actualData.Substring(88, 24);

                            //respTopup.respResult.Result = "Success";
                            //respTopup.respResult.ReturnCode = 2;
                            //respTopup.respResult.ReturnDescription = "Success";
                            #endregion
                            return respTopup;
                        }
                        else
                        {
                            respTopup.RespResult.Result = "Failed";
                            respTopup.RespResult.ReturnCode = -1;
                            respTopup.RespResult.ReturnDescription = "Lrc not matched";
                            return respTopup;
                        }
                    }
                    else if (recDataLen > 0 && recDataLen < 64)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "There is problem in Card";
                        respTopup.RespResult.ErrorDetails = "The Response Data must be 96 bytes but in response we have recived " + recDataLen.ToString();
                        return respTopup;
                    }
                    else if (recDataLen == 0)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respTopup.RespResult.ErrorDetails = "Default time for Tap & TopUp service data has given 2 min";
                        return respTopup;
                    }
                }
                else
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respTopup.RespResult.ErrorDetails = "The input bytes must be 122  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respTopup;
                }
                return respTopup;
            }
            catch (Exception ex)
            {
                respTopup.RespResult.Result = "Failed";
                respTopup.RespResult.ReturnCode = -1;
                respTopup.RespResult.ReturnDescription = ex.ToString();
                return respTopup;
            }
            finally
            {
                ServiceLog("TOP-UP", $"END OF ONLINE DEBIT TRANSACTION", GetType().Name);
                _port.Close();

            }
        }

        public ResponseTopUp AvmTransactionSync(string portName, int transactionId)
        {
            ResponseTopUp respTopup = new ResponseTopUp();

            try
            {
                ServiceLog("SYNC", $"BEGIN OF SYNC TRANSACTION", GetType().Name);
                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Failed to open Connection";
                    return respTopup;
                }
                #endregion

                byte[] headerDetails = new byte[] { 250, 161, 0x1C, 0x09 };
                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails);

                string tranId = Convert.ToString(transactionId);
                var tranIdMessage = tranId.PadLeft(9, '0');
                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(tranIdMessage));
                if (dataDetails.Length == 13)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());
                    byte[] footerDetails = new byte[] { crcField, 26, 175 };
                    int messageLength = dataDetails.Length + footerDetails.Length;
                    var ms = new MemoryStream(new byte[messageLength], 0, messageLength, true, true);
                    ms.Write(dataDetails, 0, dataDetails.Length);
                    ms.Write(footerDetails, 0, footerDetails.Length);
                    byte[] sendData = ms.GetBuffer();

                    string sendMessage = EmvService.ByteArrayToHexString(sendData);
                    byte[] buffer = HexStringToByteArray(sendMessage);
                    _port.Write(buffer, 0, buffer.Length);

                    String resultByte = "";
                    byte[] receiveData = new byte[] { };
                    int recDataLen = 0;

                    for (int i = 0; i < 72; i++)
                    {
                        Thread.Sleep(500);
                        var dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (recDataLen != 0)
                        {
                            resultByte = EmvService.ByteToHexString(receiveData[3]);
                            if (resultByte == "00")
                                break;
                        }
                    }
                    ServiceLog("SYNC", $"RESPONSE MESSAGE FOR TOP-UP : {ByteArrayToHexString(receiveData)}", GetType().Name);

                    if (resultByte == "00")
                    {
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16);
                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();
                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));


                        string tomTransacationRecId = EmvService.ByteArrayToHexString(recMsgData.Take(9).ToArray());
                        string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Skip(9).Take(32).ToArray());
                        string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(41).Take(12).ToArray());
                        string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(53).Take(12).ToArray());

                        #region Respons of Topup
                        respTopup.Amt = amt;
                        respTopup.PanTokenNo = panTokenNo;
                        respTopup.Rrn = rrnNo;
                        respTopup.RespResult.Result = "Success";
                        respTopup.RespResult.ReturnCode = 2;
                        respTopup.RespResult.ReturnDescription = "Success";
                        #endregion
                    }
                }

                return respTopup;
            }
            catch (Exception ex)
            {
                respTopup.RespResult.Result = "Failed";
                respTopup.RespResult.ReturnCode = -1;
                respTopup.RespResult.ReturnDescription = ex.ToString();
                return respTopup;
            }
            finally
            {
                ServiceLog("SYNC", $"END OF SYNC TRANSACTION", GetType().Name);
                _port.Close();
            }
        }

        public ResponseResult CheckEvmConnection(string portName)
        {
            ResponseResult responseResult = new ResponseResult();

            try
            {
                ServiceLog("SYNC", $"BEGIN OF CHECK AVM CONNECTION", GetType().Name);

                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }

                _port.Open();

                if (!_port.IsOpen)
                {
                    responseResult.Result = "Failed";
                    responseResult.ReturnCode = -1;
                    responseResult.ReturnDescription = "Failed to open Connection";
                    return responseResult;
                }
                #endregion

                byte[] data = new byte[] { 0xFA, 0xA1, 0x14, 0x00, 0x4F, 0x1A, 0xAF };

                string dataStr = EmvService.ByteArrayToHexString(data);

                if (data.Length == 7)
                {
                    _port.Write(data, 0, data.Length);

                    string resultByte = "";

                    byte[] receiveData = new byte[] { };

                    for (int i = 0; i < 72; i++)
                    {
                        Thread.Sleep(500);

                        var dataLength = _port.BytesToRead;

                        receiveData = new byte[dataLength];

                        var recDataLen = _port.Read(receiveData, 0, dataLength);

                        if (recDataLen != 0)
                        {
                            resultByte = EmvService.ByteToHexString(receiveData[3]);

                            if (resultByte == "00")
                                break;
                        }
                    }

                    ServiceLog("SYNC", $"RESPONSE MESSAGE FOR CHECK AVM CONNECTION : {ByteArrayToHexString(receiveData)}", GetType().Name);

                    if (resultByte == "00")
                    {
                        #region Response of connection

                        responseResult.Result = "Success";
                        responseResult.ReturnCode = 2;
                        responseResult.ReturnDescription = "Success";

                        #endregion
                    }
                    else
                    {
                        responseResult.Result = "Received message not matched...";
                        responseResult.ReturnCode = -1;
                        responseResult.ReturnDescription = "Failed";
                    }
                }

                return responseResult;
            }
            catch (Exception ex)
            {
                responseResult.Result = "Failed";
                responseResult.ReturnCode = -1;
                responseResult.ReturnDescription = ex.ToString();
                return responseResult;
            }
            finally
            {
                ServiceLog("SYNC", $"END OF SYNC TRANSACTION", GetType().Name);
                _port.Close();
            }
        }

        public ResponseResult Disconnect(string portName)
        {
            ResponseResult respResult = new ResponseResult();

            try
            {

                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respResult.Result = "Failed";
                    respResult.ReturnCode = -1;
                    respResult.ReturnDescription = "Failed to open Connection";
                    return respResult;
                }
                #endregion

                byte[] buffe = HexStringToByteArray("FAA10E00551AAF");
                _port.Write(buffe, 0, buffe.Length);

                respResult.Result = "Success";
                respResult.ReturnCode = 2;
                respResult.ReturnDescription = "Connection Closed Successfully";

                return respResult;
            }
            catch (Exception ex)
            {
                respResult.Result = "Failed";
                respResult.ReturnCode = -1;
                respResult.ReturnDescription = ex.ToString();
                return respResult;
            }
            finally
            {
                _port.Close();
            }
        }

        public ResponseResult OfflineDebit(string portName, byte[] serviceData, Double amount)
        {
            if (serviceData.Length != 96)
            {
                //TODO: Data Len Error.
            }

            ResponseResult respResult = new ResponseResult();
            try
            {
                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respResult.Result = "Failed";
                    respResult.ReturnCode = -1;
                    respResult.ReturnDescription = "Failed to open Connection";
                    return respResult;
                }
                #endregion

                //byte[] buffe = HexStringToByteArray("FAA1100C303030303030303230303030451AAF");
                //port.Write(buffe, 0, buffe.Length);

                string baseAmt = "000000000000";
                byte[] headerDetails = new byte[] { 250, 161, 0x06, 0x6C };
                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails) + EmvService.ByteArrayToHexString(serviceData);

                string msgData = Convert.ToString(Convert.ToInt32(amount) * 100);
                msgData = baseAmt.Remove(baseAmt.Length - msgData.Length, msgData.Length) + msgData;
                string all_ = headerDetailshexa + EmvService.ASCIITOHex(msgData);
                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(msgData));
                if (dataDetails.Count() == 112)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());
                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };
                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 
                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);
                    //  ms.Write(headerDetails, 0, headerDetails.Length);                   
                    ms.Write(dataDetails, 0, dataDetails.Length);
                    ms.Write(fotterDetails, 0, fotterDetails.Length);
                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);
                    byte[] buffer = HexStringToByteArray(sendCrc);
                    _port.Write(buffer, 0, buffer.Length);

                    #region Code taking less time
                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    do
                    {
                        k += 1;
                        Thread.Sleep(2000);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 36)//k==16
                        {
                            // Console.WriteLine("Card Not Tap");
                            break;
                        }

                    }
                    while (recDataLen < 8);
                    #endregion

                    if (recDataLen > 7) //64
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data  Actual Data  in Bytes                 

                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        //string msgData = EmvService.ByteArrayToHexString(recMsgData);
                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                        if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {

                            string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Take(32).ToArray());
                            string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(32).Take(6).ToArray());
                            string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(38).Take(6).ToArray());
                            #region Respons of Topup
                            //respTopup.amt = amt;
                            //respTopup.panTokenNo = panTokenNo;
                            //respTopup.rrn = rrnNo;
                            if (recCmdStatus == "00")
                            {
                                respResult.Result = "Success";
                                respResult.ReturnCode = 2;
                                respResult.ReturnDescription = "Success";
                            }
                            else
                            {
                                respResult.Result = "Failed";
                                respResult.ReturnCode = -1;
                                respResult.ReturnDescription = "Failed";
                            }

                            #endregion
                            #region Details of All Info
                            //respTopup.panTokenNo = actualData.Substring(0, 64);
                            //respTopup.rrn = actualData.Substring(64, 24);
                            //respTopup.amt = actualData.Substring(88, 24);

                            //respTopup.respResult.Result = "Success";
                            //respTopup.respResult.ReturnCode = 2;
                            //respTopup.respResult.ReturnDescription = "Success";
                            #endregion
                            return respResult;
                        }
                        else
                        {
                            respResult.Result = "Failed";
                            respResult.ReturnCode = -1;
                            respResult.ReturnDescription = "Lrc not matched";
                            return respResult;
                        }
                    }
                    else if (recDataLen > 0 && recDataLen < 7)
                    {
                        respResult.Result = "Failed";
                        respResult.ReturnCode = -1;
                        respResult.ReturnDescription = "There is problem in Card";
                        respResult.ErrorDetails = "The Response Data must be 64 bytes but in response we have recived " + recDataLen.ToString();
                        return respResult;
                    }
                    else if (recDataLen == 0)
                    {
                        respResult.Result = "Failed";
                        respResult.ReturnCode = -1;
                        respResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respResult.ErrorDetails = "Default time for Tap & TopUp service data has given 2 min";
                        return respResult;
                    }
                }
                else
                {
                    respResult.Result = "Failed";
                    respResult.ReturnCode = -1;
                    respResult.ReturnDescription = "Input bytes not match fro Request";
                    respResult.ErrorDetails = "The input bytes must be 100  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respResult;
                }
                return respResult;
            }
            catch (Exception ex)
            {
                respResult.Result = "Failed";
                respResult.ReturnCode = -1;
                respResult.ReturnDescription = ex.ToString();
                return respResult;
            }
            finally
            {

                _port.Close();

            }
        }

        public ResponseTopUp OfflineDebits(string portName, ServiceData servData, Double amount)
        {
            ResponseTopUp respTopup = new ResponseTopUp();

            try
            {
                ServiceData updatData = new ServiceData();

                #region PortDetails
                _port.PortName = portName;
                _port.BaudRate = 115200;
                _port.Parity = Parity.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                #endregion

                #region Connection Code

                //port.Close();
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Open();

                if (!_port.IsOpen)
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Failed to open Connection";
                    return respTopup;
                }
                #endregion

                #region Variable Declareation
                string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastRoute, _passValidity, _general, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                string trmId, trmStan, trmBatchno, trmKM = "";
                string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                string toStnExit, toStnStage, toStnId, toStncode = "";
                #endregion

                #region Assigning value to Variable

                _passCounter = servData.PassCounter;
                _passLimit = servData.PassLimit;
                _expirayDate = servData.PassExpiryDate;
                _effictiveDate = servData.PassEffectiveDate;
                _passType = servData.PassType;
                _lastRoute = servData.LastRouteNo;
                _passValidity = servData.PassValidityVol;
                _passCounterMBS = servData.PassCounterMsb;
                _passLimitMBS = servData.PassLimitMsb;
                _passProductCode = servData.PassProductCode;
                _loadStation = servData.LoadStationId;
                _loadSalePoint = servData.LoadSalePointId;
                _general = servData.Rfu;

                trmId = servData.LastTerminalId;
                trmStan = servData.LastStan;
                trmBatchno = servData.LastBatch;
                trmKM = servData.LastCity;

                tnxTapInDate = servData.LastTapInDt;
                tnxAmt = servData.LastFareAmount;
                tnxM = servData.LastVehicleType;
                tnxdata = servData.LastFareType;
                tnxType = servData.LastTransactionType;

                fmStnEntry = servData.FromLineCode;
                fmStnStage = servData.FromStageCode;
                fmStnId = servData.FromStationId;
                fmStncode = servData.FromStationShortCode;

                toStnExit = servData.ToLineCode;
                toStnStage = servData.ToStageCode;
                toStnId = servData.ToStationId;
                toStncode = servData.ToStationShortCode;

                #endregion

                #region Hard Code Value Assigned
                //_passCounter = updatData.passCounter = "00";
                //_passLimit = updatData.passLimit = "00";
                //_expirayDate = updatData.passExpiryDate = "0000";
                //_effictiveDate = updatData.passEffectiveDate = "0000";
                //_passType = updatData.passType = "00";
                //_lastRoute = updatData.lastRouteNo = "00";
                //_passValidity = updatData.passValidityVOL = "0000000000000000000000000000000000000000000000";
                //_general = updatData.rfu = "00000000000000000000000000000000";//3 byte azaldı
                ////Rfu dan aldığımız 3 byte'lık alanı 1 er byte olarak aşağıda tanımladım
                //_passCounterMBS = updatData.passCounterMSB = "00";
                //_passLimitMBS = updatData.passLimitMSB = "00";
                //_passProductCode = updatData.passProductCode = "00";

                //trmId = updatData.lastTerminalId = "00000003"; //00000003
                //trmStan = updatData.lastStan = "000001";
                //trmBatchno = updatData.lastBatch = "0001";
                //trmKM = updatData.lastCity = "KM";

                //tnxTapInDate = updatData.lastTapInDT = "170508193115";
                //tnxAmt = updatData.lastFareAmount = "000000000000";
                //tnxM = updatData.lastVehicleType = "M";
                //tnxdata = updatData.lastFareType = "31";
                //tnxType = updatData.lastTransactionType = "00";

                //fmStnEntry = updatData.fromLineCode = "0000";
                //fmStnStage = updatData.fromStageCode = "00";
                //fmStnId = updatData.fromStationId = "03";
                //fmStncode = updatData.fromStationShortCode = "CPPY";

                //toStnExit = updatData.toLineCode = "0000";
                //toStnStage = updatData.toStageCode = "00";
                //toStnId = updatData.toStationId = "00";
                //toStncode = updatData.toStationShortCode = "CPPY";
                #endregion

                //byte[] buffe = HexStringToByteArray("FAA1100C303030303030303230303030451AAF");
                //port.Write(buffe, 0, buffe.Length);
                _terminalInfo = EmvService.ASCIITOHex(trmId) + trmStan + trmBatchno + EmvService.ASCIITOHex(trmKM);
                _trxInfo = tnxTapInDate + tnxAmt + EmvService.ASCIITOHex(tnxM) + tnxdata + tnxType;
                _fromStation = fmStnEntry + fmStnStage + fmStnId + EmvService.ASCIITOHex(fmStncode);
                _toStation = toStnExit + toStnStage + toStnId + EmvService.ASCIITOHex(toStncode);

                string baseAmt = "000000000000";
                byte[] headerDetails = new byte[] { 250, 161, 0x06, 0x6C };
                string headerDetailshexa = EmvService.ByteArrayToHexString(headerDetails) + _passCounter + _passLimit + _expirayDate + _effictiveDate + _passType + _terminalInfo + _trxInfo + _fromStation + _toStation + _lastRoute + _passValidity + _passCounterMBS + _passLimitMBS + _passProductCode + _loadStation + _loadSalePoint + _general;

                string msgData = Convert.ToString(Convert.ToInt32(amount) * 100);
                msgData = baseAmt.Remove(baseAmt.Length - msgData.Length, msgData.Length) + msgData;
                string all_ = headerDetailshexa + EmvService.ASCIITOHex(msgData);
                byte[] dataDetails = EmvService.HexStringToByteArray(headerDetailshexa + EmvService.ASCIITOHex(msgData));

                if (dataDetails.Count() == 112)
                {
                    byte crcField = EmvService.CalCheckSum(dataDetails, dataDetails.Count());
                    byte[] fotterDetails = new byte[] { crcField, 26, 175 };
                    int crcDetailsLength = dataDetails.Length + fotterDetails.Length;//+headerDetails.Length 
                    var ms = new MemoryStream(new byte[crcDetailsLength], 0, crcDetailsLength, true, true);
                    //  ms.Write(headerDetails, 0, headerDetails.Length);                   
                    ms.Write(dataDetails, 0, dataDetails.Length);
                    ms.Write(fotterDetails, 0, fotterDetails.Length);
                    byte[] sendData = ms.GetBuffer();

                    string sendCrc = EmvService.ByteArrayToHexString(sendData);
                    byte[] buffer = HexStringToByteArray(sendCrc);
                    _port.Write(buffer, 0, buffer.Length);
                    ServiceLog("SEND MESSAGE FOR OFFLINE DEBIT :", sendCrc, GetType().Name);
                    #region Code taking less time
                    int dataLength = 0;
                    int recDataLen = 0;
                    byte[] receiveData;
                    int k = 0;
                    do
                    {
                        k += 1;
                        Thread.Sleep(5000);
                        dataLength = _port.BytesToRead;
                        receiveData = new byte[dataLength];
                        recDataLen = _port.Read(receiveData, 0, dataLength);
                        if (k == 36)//k==16
                        {
                            // Console.WriteLine("Card Not Tap");
                            break;
                        }

                    }
                    while (recDataLen < 8);
                    #endregion
                    ServiceLog("RESPONSE MESSAGE FOR OFFLINE DEBIT :", ByteArrayToHexString(receiveData), GetType().Name);
                    if (recDataLen > 7) //64
                    {
                        //Header
                        string recHeader = EmvService.ByteToHexString(receiveData[0]) + EmvService.ByteToHexString(receiveData[1]);//FAA1                   
                                                                                                                                   //Msg Type
                        string recMsgtype = EmvService.ByteToHexString(receiveData[2]);//0D                   
                                                                                       //Status
                        string recCmdStatus = EmvService.ByteToHexString(receiveData[3]);//00                   
                                                                                         //Footer
                        string recFooter = EmvService.ByteToHexString(receiveData[recDataLen - 2]) + EmvService.ByteToHexString(receiveData[recDataLen - 1]);//1AAF                   
                                                                                                                                                             //CheckSum
                        string recCrc = EmvService.ByteToHexString(receiveData[recDataLen - 3]);//crc CA             

                        //Data Length converted from byte to Hex then to decimal
                        int recMsgDataLen = Convert.ToInt32(EmvService.ByteToHexString(receiveData[4]), 16); //Length of Data  Actual Data  in Bytes                 

                        byte[] recMsgData = receiveData.Skip(5).Take(recMsgDataLen).ToArray();

                        //Actual Data Length in HexaDecimal
                        //string msgData = EmvService.ByteArrayToHexString(recMsgData);
                        byte[] chkSumbyte = receiveData.Take(recDataLen - 3).ToArray();
                        string hexacec = EmvService.ByteArrayToHexString(chkSumbyte);
                        string calculateCrc = EmvService.ByteToHexString(EmvService.CalCheckSum(chkSumbyte, chkSumbyte.Count()));

                        if (recHeader == "FAA1" && recFooter == "1AAF" && calculateCrc == recCrc)//&& recMsgData.Count()<134
                        {

                            string panTokenNo = EmvService.ByteArrayToHexString(recMsgData.Take(32).ToArray());
                            string rrnNo = EmvService.ByteArrayToHexString(recMsgData.Skip(32).Take(6).ToArray());//hosttan posa geliyor hiç olmayacak
                            string amt = EmvService.ByteArrayToHexString(recMsgData.Skip(38).Take(6).ToArray());
                            #region Respons of Topup
                            respTopup.Amt = amt;
                            respTopup.PanTokenNo = panTokenNo;
                            //respTopup.rrn = rrnNo;
                            respTopup.RespResult.Result = "Success";
                            respTopup.RespResult.ReturnCode = 2;
                            respTopup.RespResult.ReturnDescription = "Success";
                            #endregion
                            #region Details of All Info
                            //respTopup.panTokenNo = actualData.Substring(0, 64);
                            //respTopup.rrn = actualData.Substring(64, 24);
                            //respTopup.amt = actualData.Substring(88, 24);

                            //respTopup.respResult.Result = "Success";
                            //respTopup.respResult.ReturnCode = 2;
                            //respTopup.respResult.ReturnDescription = "Success";
                            #endregion
                            return respTopup;
                        }
                        else
                        {
                            respTopup.RespResult.Result = "Failed";
                            respTopup.RespResult.ReturnCode = -1;
                            respTopup.RespResult.ReturnDescription = "Lrc not matched";
                            return respTopup;
                        }
                    }
                    else if (recDataLen > 0 && recDataLen < 64)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "There is problem in Card";
                        respTopup.RespResult.ErrorDetails = "The Response Data must be 96 bytes but in response we have recived " + recDataLen.ToString();
                        return respTopup;
                    }
                    else if (recDataLen == 0)
                    {
                        respTopup.RespResult.Result = "Failed";
                        respTopup.RespResult.ReturnCode = -1;
                        respTopup.RespResult.ReturnDescription = "No Card has swiped or Taped or Card tap time must be exceed";
                        respTopup.RespResult.ErrorDetails = "Default time for Tap & TopUp service data has given 2 min";
                        return respTopup;
                    }
                }
                else
                {
                    respTopup.RespResult.Result = "Failed";
                    respTopup.RespResult.ReturnCode = -1;
                    respTopup.RespResult.ReturnDescription = "Input bytes not match fro Request";
                    respTopup.RespResult.ErrorDetails = "The input bytes must be 112  but the bytes came from input is " + dataDetails.Count().ToString();
                    return respTopup;
                }
                return respTopup;
            }
            catch (Exception ex)
            {
                respTopup.RespResult.Result = "Failed";
                respTopup.RespResult.ReturnCode = -1;
                respTopup.RespResult.ReturnDescription = ex.ToString();
                return respTopup;
            }
            finally
            {

                _port.Close();

            }
        }

        #endregion

        #region Helper Methods

        private static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }

        private static string ByteToHexString(byte bytes)
        {
            StringBuilder hex = new StringBuilder(bytes * 2);
            //foreach (byte b in bytes)
            hex.AppendFormat("{0:x2}", bytes);
            return hex.ToString().ToUpper();
        }

        public static byte HexStringToByte(string hex)
        {
            return Convert.ToByte(hex, 16);
        }

        private static byte CalCheckSum(byte[] _PacketData, int PacketLength)
        {
            Byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketLength; i++)
                _CheckSumByte ^= _PacketData[i];
            return _CheckSumByte;
        }

        private static string HEX2ASCII(string hex)
        {

            string res = String.Empty;

            for (int a = 0; a < hex.Length; a = a + 2)
            {

                string Char2Convert = hex.Substring(a, 2);

                int n = Convert.ToInt32(Char2Convert, 16);

                char c = (char)n;

                res += c.ToString();

            }

            return res;

        }

        private static string ASCIITOHex(string ascii)
        {

            StringBuilder sb = new StringBuilder();

            byte[] inputBytes = Encoding.UTF8.GetBytes(ascii);

            foreach (byte b in inputBytes)
            {

                sb.Append(string.Format("{0:x2}", b));

            }

            return sb.ToString().ToUpper();

        }

        private static void ServiceLog(string LogType, string Log, string className, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {

            var log = "";
            switch (LogType)
            {
                case "ERROR":
                    log = "[" + $"{DateTime.Now:HH:mm:ss}" + "]" + "[" + LogType + "]" + "[" + className + "]" + "[" + caller + "]" + " " + Log + " " + " " + "Line :" + lineNumber;
                    break;
                case "INFO":
                    log = "[" + $"{DateTime.Now:HH:mm:ss}" + "]" + "[" + LogType + "]" + "[" + className + "]" + "[" + caller + "]" + " " + Log + " " + " ";
                    break;
                default:
                    log = "[" + $"{DateTime.Now:HH:mm:ss}" + "]" + "[" + LogType + "]" + "[" + className + "]" + "[" + caller + "]" + " " + Log + " " + " ";
                    break;
            }
            var logDir = @"C:\POSLOG";
            if (!Directory.Exists(logDir))
            {
                var di = Directory.CreateDirectory(logDir);
            }

            FlushLog(logDir, "LOG", log);

        }

        private static void FlushLog(string logDir, string logFile, string log)
        {
            string fileName = $"{logDir}\\{logFile}_{DateTime.Now:dd.MM.yyyy}.log";

            try
            {
                if (File.Exists(fileName))
                {
                    var fileAge = DateTime.Now - File.GetLastWriteTime(fileName);

                    if (fileAge.Days > 1)
                        File.WriteAllText(fileName, string.Empty);
                }

                using (var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(log);

                        sw.Flush();
                    }

                    fs.Close();
                }
            }
            catch (Exception)
            {
                //...
            }
        }

        public static string PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("byte[] = ");

            foreach (var b in bytes)
                sb.Append(b + ", ");

            return sb.ToString();
        }

        #endregion

        #region Enum

        public enum PosMessageType
        {
            Topup = 16,
            Refund = 07
        }

        #endregion
    }

}
