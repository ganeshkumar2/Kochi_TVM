using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.VerifoneKochi1
{
    public class PosOperation
    {
        POSMessage message = new POSMessage();
        public void ReadServiceData(string uxPosPort, out ServiceData servData)
        {
            servData = new ServiceData();
            try
            {
                if (message.SendReceive(MessageTypes.ServiceDataRead, null, out MessageLayout msgLayout))
                {
                    #region CommandStatus OK
                    if (msgLayout.commandStatus == CommandStatus.OK)
                    {

                        byte[] cardData = msgLayout.msgData;//.Skip(32).ToArray();
                        string actualData = Hex2String(cardData);

                        #region Decleration of variable
                        string _passCounter, _passLimit, _expirayDate, _effictiveDate, _passType, _terminalInfo, _trxInfo, _fromStation, _toStation, _lastToStn, _lastRoute, _passValidity, _general, _panTokenNo, _balance, _passCounterMBS, _passLimitMBS, _passProductCode = "", _loadStation, _loadSalePoint;
                        string trmId, trmStan, trmBatchno, trmKM = "";
                        string tnxTapInDate, tnxAmt, tnxM, tnxdata, tnxType = "";
                        string fmStnEntry, fmStnStage, fmStnId, fmStncode = "";
                        string toStnExit, toStnStage, toStnId, toStncode = "";
                        string loadStnId, loadSalePointId;

                        #endregion

                        #region Details of All Info
                        _passCounter = actualData.Substring(0, 2);
                        servData.passCounter = _passCounter;
                        _passLimit = actualData.Substring(2, 2);
                        servData.passLimit = _passLimit;
                        _expirayDate = actualData.Substring(4, 4);
                        servData.passExpiryDate = _expirayDate;
                        _effictiveDate = actualData.Substring(8, 4);
                        servData.passEffectiveDate = _effictiveDate;
                        _passType = actualData.Substring(12, 2);
                        servData.passType = _passType;
                        _terminalInfo = actualData.Substring(14, 30);
                        _trxInfo = actualData.Substring(44, 30);
                        _fromStation = actualData.Substring(74, 16);
                        _toStation = actualData.Substring(90, 16);
                        _lastRoute = actualData.Substring(106, 2);
                        servData.lastRouteNo = _lastRoute;
                        _passValidity = actualData.Substring(108, 46);
                        servData.passValidityVOL = _passValidity;
                        _passCounterMBS = actualData.Substring(154, 2);
                        servData.passCounterMSB = _passCounterMBS;
                        _passLimitMBS = actualData.Substring(156, 2);
                        servData.passLimitMSB = _passLimitMBS;
                        _passProductCode = actualData.Substring(158, 2);
                        servData.passProductCode = _passProductCode;
                        _loadStation = actualData.Substring(160, 2);
                        servData.loadStationId = _loadStation;
                        _loadSalePoint = actualData.Substring(162, 4);
                        servData.loadSalePointId = _loadSalePoint;
                        _general = actualData.Substring(166, 26);
                        servData.rfu = _general;
                        _panTokenNo = actualData.Substring(192, 64);//actualData.Substring(192, 64);
                        servData.panToken = _panTokenNo;
                        _balance = actualData.Substring(256, 12);//actualData.Substring(256, 12);
                        servData.balance = _balance;
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
                        servData.lastTerminalId = Hex2Ascii(trmId);

                        trmStan = _terminalInfo.Substring(16, 6);
                        servData.lastStan = trmStan;

                        trmBatchno = _terminalInfo.Substring(22, 4);
                        servData.lastBatch = trmBatchno;

                        trmKM = _terminalInfo.Substring(26, 4);
                        servData.lastCity = Hex2Ascii(trmKM);
                        #endregion

                        #region Details of Transaction info                     
                        tnxTapInDate = _trxInfo.Substring(0, 12);
                        servData.lastTapInDT = tnxTapInDate;

                        tnxAmt = _trxInfo.Substring(12, 12);
                        servData.lastFareAmount = tnxAmt;

                        tnxM = _trxInfo.Substring(24, 2);
                        servData.lastVehicleType = Hex2Ascii(tnxM);

                        tnxdata = _trxInfo.Substring(26, 2);
                        servData.lastFareType = tnxdata;

                        tnxType = _trxInfo.Substring(28, 2);
                        servData.lastTransactionType = tnxType;

                        #endregion

                        #region Details of From Station                        
                        fmStnEntry = _fromStation.Substring(0, 4);
                        servData.fromLineCode = fmStnEntry;

                        fmStnStage = _fromStation.Substring(4, 2);
                        servData.fromStageCode = fmStnStage;

                        fmStnId = _fromStation.Substring(6, 2);
                        servData.fromStationId = fmStnId;

                        fmStncode = _fromStation.Substring(8, 8);
                        servData.fromStationShortCode = Hex2Ascii(fmStncode);
                        #endregion

                        #region Details of to Station                      
                        toStnExit = _toStation.Substring(0, 4);
                        servData.toLineCode = toStnExit;

                        toStnStage = _toStation.Substring(4, 2);
                        servData.toStageCode = toStnStage;

                        toStnId = _toStation.Substring(6, 2);
                        servData.toStationId = toStnId;

                        toStncode = _toStation.Substring(8, 8);
                        servData.toStationShortCode = Hex2Ascii(toStncode);
                        #endregion

                        #region Details of Last Load Info

                        loadStnId = _loadStation.Substring(0, 2);
                        servData.loadStationId = loadStnId;

                        loadSalePointId = _loadSalePoint.Substring(0, 4);
                        servData.loadSalePointId = loadSalePointId;

                        #endregion

                        servData.respResult.Result = "Success";
                        servData.respResult.ReturnCode = 2;
                        servData.respResult.ReturnDescription = "Success";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

            }
        }
        public string Hex2Ascii(string hex)
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
        public bool OnnlineDebits(string uxPosPort, ServiceData servData, double totalPrice, out PosDataLayout posDataLayout)
        {
            bool result = false;
            posDataLayout = new PosDataLayout();


            return result;
        }
        public bool TopUp(string portName, Double amount, out PosDataLayout posDataLayout)
        {
            bool result = false;
            posDataLayout = new PosDataLayout();
            try
            {
                //sample for 200.00 
                //FAA1100C303030303030303230303030451AAF
                int temp = (int)(amount * 100);
                byte[] data = String2Hex(temp.ToString().PadLeft(12, '0'));
                if (message.SendReceive(MessageTypes.TopUp, data, out MessageLayout msgLayout))
                {
                    posDataLayout.messageType = msgLayout.messageType;
                    posDataLayout.commandStatus = msgLayout.commandStatus;
                    string msgData = Hex2String(msgLayout.msgData);

                    #region CommandStatus OK
                    if (posDataLayout.commandStatus == CommandStatus.OK)
                    {
                        if (posDataLayout.messageType == MessageTypes.PresentCard)
                        {
                            System.Threading.Thread.Sleep(10);
                            if (msgLayout.msgData.Length == 1)
                            {
                                if (msgData == "01")
                                {
                                    posDataLayout.oprResponse = OperationResponse.CardNotPresent;
                                }
                            }
                            else if (msgLayout.msgData.Length > 1)
                            {
                                posDataLayout.cardType = (CardTypes)Convert.ToInt32(msgData.Substring(2, 2));
                                posDataLayout.panToken = msgData.Substring(4, 64);
                                posDataLayout.pan = msgData.Substring(68, 16);
                                if (posDataLayout.cardType == CardTypes.Contact)
                                {
                                    posDataLayout.oprResponse = OperationResponse.Contact;
                                    posDataLayout.RRN = msgData.Substring(84, 12);
                                    posDataLayout.totalBalance = (Int32.Parse(msgData.Substring(96, 12))) / 100;
                                    posDataLayout.e_Balance = (Int32.Parse(msgData.Substring(108, 12))) / 100;
                                    posDataLayout.newBalance = (Int32.Parse(msgData.Substring(120, 12))) / 100;
                                }
                                if (posDataLayout.cardType == CardTypes.Contactless)
                                {
                                    posDataLayout.oprResponse = OperationResponse.Contactless;
                                    posDataLayout.serviceData = msgData.Substring(84, 192);
                                    posDataLayout.newBalance = (Int32.Parse(msgData.Substring(276, 12))) / 100;
                                }
                            }
                            else
                            {
                                //fix me  : data bozuk
                            }
                        }
                        if (posDataLayout.messageType == MessageTypes.PINRequired)
                        {
                            posDataLayout.oprResponse = OperationResponse.PinExpected;
                        }
                        if (posDataLayout.messageType == MessageTypes.PINTypeResult)
                        {
                            posDataLayout.oprResponse = OperationResponse.PinEntered;
                        }
                        if (posDataLayout.messageType == MessageTypes.PINBtnPressed)
                        {
                            posDataLayout.numPinPressed = Convert.ToInt32(msgData.Substring(0, 2));
                            posDataLayout.oprResponse = OperationResponse.PinBtnPressed;
                        }
                    }
                    #endregion

                    #region CommandStatus NOK
                    if (posDataLayout.commandStatus == CommandStatus.NOK)
                    {
                        posDataLayout.errorCodes = (ErrorCodes)msgLayout.msgData[0];
                    }
                    #endregion
                    return true;
                }
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }
        public byte[] String2Hex(string data)
        {
            byte[] result = null;

            try
            {
                if (data != null)
                {
                    if (data.Length > 0)
                    {
                        result = new byte[data.Length];
                        int index = 0;
                        foreach (var t in data)
                        {
                            result[index++] = (byte)((char)t);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = null;
            }

            return result;
        }
        public bool OnlyReceive(out PosDataLayout posDataLayout)
        {
            bool result = false;
            posDataLayout = new PosDataLayout();
            try
            {
                if (message.OnlyReceive(out MessageLayout msgLayout))
                {
                    posDataLayout.messageType = msgLayout.messageType;
                    posDataLayout.commandStatus = msgLayout.commandStatus;
                    string msgData = Hex2String(msgLayout.msgData);

                    #region CommandStatus OK
                    if (posDataLayout.commandStatus == CommandStatus.OK)
                    {
                        if (posDataLayout.messageType == MessageTypes.PresentCard)
                        {
                            System.Threading.Thread.Sleep(10);
                            if (msgLayout.msgData.Length == 1)
                            {
                                if (msgData == "01")
                                {
                                    posDataLayout.oprResponse = OperationResponse.CardNotPresent;
                                }
                            }
                            else if (msgLayout.msgData.Length > 1)
                            {
                                posDataLayout.cardType = (CardTypes)Convert.ToInt32(msgData.Substring(2, 2));
                                posDataLayout.panToken = msgData.Substring(4, 64);
                                posDataLayout.pan = msgData.Substring(68, 16);
                                if (posDataLayout.cardType == CardTypes.Contact)
                                {
                                    posDataLayout.oprResponse = OperationResponse.Contact;
                                    posDataLayout.RRN = msgData.Substring(84, 12);
                                    posDataLayout.totalBalance = (Int32.Parse(msgData.Substring(96, 12))) / 100;
                                    posDataLayout.e_Balance = (Int32.Parse(msgData.Substring(108, 12))) / 100;
                                    posDataLayout.newBalance = (Int32.Parse(msgData.Substring(120, 12))) / 100;
                                }
                                if (posDataLayout.cardType == CardTypes.Contactless)
                                {
                                    posDataLayout.oprResponse = OperationResponse.Contactless;
                                    posDataLayout.serviceData = msgData.Substring(84, 192);
                                    posDataLayout.newBalance = (Int32.Parse(msgData.Substring(276, 12))) / 100;
                                }
                            }
                            else
                            {
                                //fix me  : data bozuk
                            }
                        }
                        if (posDataLayout.messageType == MessageTypes.PINRequired)
                        {
                            posDataLayout.oprResponse = OperationResponse.PinExpected;
                        }
                        if (posDataLayout.messageType == MessageTypes.PINTypeResult)
                        {
                            posDataLayout.oprResponse = OperationResponse.PinEntered;
                        }
                        if (posDataLayout.messageType == MessageTypes.PINBtnPressed)
                        {
                            posDataLayout.numPinPressed = Convert.ToInt32(msgData.Substring(0, 2));
                            posDataLayout.oprResponse = OperationResponse.PinBtnPressed;
                        }
                    }
                    #endregion

                    #region CommandStatus NOK
                    if (posDataLayout.commandStatus == CommandStatus.NOK)
                    {
                        posDataLayout.errorCodes = (ErrorCodes)msgLayout.msgData[0];
                    }
                    #endregion
                    return true;
                }
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }
        public string Hex2String(byte[] data)
        {
            var sb = new StringBuilder();
            string result = String.Empty;

            try
            {
                if (data != null)
                {
                    if (data.Length > 0)
                    {
                        foreach (var t in data)
                        {
                            sb.Append(t.ToString("X2"));
                        }

                        result = sb.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                result = String.Empty;
            }

            return result;
        }
        public bool PresentCard(params object[] paramList)
        {
            bool result = false;
            try
            {
                MessageLayout layout;
                if (message.SendReceive(MessageTypes.OfflineBalanceInquery, null, out layout)) ;
                {
                    paramList[0] = layout.msgData;
                    return true;
                }
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        //        public bool CheckCard(out int cardType)
        //        {
        //            bool result = false;
        //            MessageTypes messageType = MessageTypes.Unknown;
        //            byte[] dataBuff;
        //            cardType = 0; //0 : no card, 1 : contact, 2 : contactless

        ////#if DEBUG

        ////            return true;
        ////#endif


        //            try
        //            {
        //                //send : FAA115004E1AAF              
        //                if (message.SendReceive(MessageTypes.PresentCard, null, out messageType, out dataBuff))
        //                {

        //                    //if (messageType == MessageTypes.PresentCard)
        //                    //{
        //                    //    System.Threading.Thread.Sleep(10);

        //                    //    //timeout, no card
        //                    //    if (dataBuff.Length == 1)
        //                    //    {
        //                    //        if (dataBuff[0] == 0x01)
        //                    //        {
        //                    //            cardType = 0;
        //                    //        }
        //                    //    }
        //                    //    else if (dataBuff.Length == 2)
        //                    //    {
        //                    //        if ((dataBuff[0] == 0x00) && (dataBuff[1] == 0x04))
        //                    //        {
        //                    //            cardType = 1;
        //                    //            result = true;
        //                    //        }
        //                    //        else if ((dataBuff[0] == 0x00) && (dataBuff[1] == 0x08))
        //                    //        {
        //                    //            cardType = 2;
        //                    //            result = true;
        //                    //        }
        //                    //    }

        //                    //}
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                result = false;
        //            }

        //            return result;

        //        }

        //        public bool ReadCardID(out string cardID)
        //        {
        //            bool result = false;
        //            MessageTypes messageType = MessageTypes.Unknown;
        //            byte[] dataBuff;
        //            cardID = String.Empty;

        ////#if DEBUG

        ////                cardID = "165468";
        ////                return true;
        ////#endif


        //            try
        //            {
        //                //send : FAA100005B1AAF
        //                if (message.SendReceive(MessageTypes.CheckCard, null, out messageType, out dataBuff))
        //                {
        //                    if (messageType == MessageTypes.CheckCard)
        //                    {
        //                        System.Threading.Thread.Sleep(10);

        //                        //1 byte card ID, 8 byte PAN
        //                        if (dataBuff.Length >= 9)
        //                        {
        //                            byte[] tempPAN = new byte[8];
        //                            Array.Copy(dataBuff, 1, tempPAN, 0, 8);
        //                            cardID = convert.Hex2String(tempPAN);

        //                            result = true;
        //                        }
        //                        else
        //                        {
        //                            //fix me : data bozuk
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                result = false;
        //            }

        //            return result;

        //        }

        //        public bool ReadCardBalance(out decimal cardBalance)
        //        {
        //            bool result = false;
        //            byte[] dataBuff;
        //            MessageTypes messageType;
        //            cardBalance = 0;

        ////            #if DEBUG
        ////            {
        ////                cardBalance = 1000;

        ////                return true;
        ////            }
        ////#endif
        //            try
        //            {
        //                if (message.SendReceive(MessageTypes.OfflineBalanceInquery, null, out messageType, out dataBuff))
        //                {
        //                    if (messageType == MessageTypes.OfflineBalanceInquery)
        //                    {
        //                        System.Threading.Thread.Sleep(10);

        //                        //1 byte card ID, 8 byte PAN
        //                        if (dataBuff.Length > 0)
        //                        {
        //                            string tempBalance = convert.Hex2String(dataBuff);
        //                            //last 2 decimal
        //                            cardBalance = Int32.Parse(tempBalance) / 100;
        //                            result = true;
        //                        }
        //                        else
        //                        {
        //                            //fix me : data bozuk
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                result = false;
        //            }

        //            return result;
        //        }

        //pan token use for banking service query
        //rrn bank reference number for check
        //        public bool OnlineTopup(decimal topupAmount, out string panToken, out string RRN, out decimal newBalance)
        //        {
        //            bool result = false;
        //            byte[] dataBuff;
        //            MessageTypes messageType = MessageTypes.Unknown;

        ////#if DEBUG

        ////            panToken = "12354";
        ////            RRN = "54864";
        ////            newBalance = 2500.0m;

        ////            return true;

        ////#endif
        //            panToken = String.Empty;
        //            RRN = String.Empty;
        //            newBalance = 0.0m;

        //            try
        //            {
        //                //sample for 200.00 
        //                //FAA1100C303030303030303230303030451AAF
        //                int temp = (int)(topupAmount * 100);
        //                byte[] data = convert.String2Hex(temp.ToString().PadLeft(12, '0'));
        //                if (message.SendReceive(MessageTypes.OnlineBalanceInquery, data, out messageType, out dataBuff))
        //                {
        //                    if (messageType == MessageTypes.OnlineBalanceInquery)
        //                    {
        //                        System.Threading.Thread.Sleep(10);

        //                        if (dataBuff.Length > 0)
        //                        {
        //                            //32 byte panToken
        //                            //12 byte RRN
        //                            //12 Byte balance
        //                            string tempStr = convert.Hex2String(dataBuff);
        //                            panToken = tempStr.Substring(0, 32);
        //                            RRN = tempStr.Substring(32, 12);
        //                            newBalance = (Int32.Parse(tempStr.Substring(44, 12))) / 100;
        //                            result = true;
        //                        }
        //                        else
        //                        {
        //                            //fix me : data bozuk
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                result = false;
        //            }

        //            return result;
        //        }

        //public bool EOD()
        //{
        //    bool result = false;
        //    MessageTypes messageType = MessageTypes.Unknown;
        //    byte[] dataBuff;

        //    try
        //    {
        //        //send : FAA100005B1AAF
        //        if (message.SendReceive(MessageTypes.EOD, null, out messageType, out dataBuff))
        //        {
        //            if (messageType == MessageTypes.EOD)
        //            {
        //                System.Threading.Thread.Sleep(10);

        //                //1 byte Result
        //                if (dataBuff.Length > 0)
        //                {
        //                    if (dataBuff[0] == 0x00)
        //                    {
        //                        result = true;
        //                    }
        //                }
        //                else
        //                {
        //                    //fix me : data bozuk
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        result = false;
        //    }

        //    return result;
        //}

        //pan token use for banking service query
        //rrn bank reference number for check
        //public bool POSSettings(string terminalID, string merchantID, string hostIP, string hostPort, string bankTCPIP, string bankTCPPort, string APN)
        //{
        //    bool result = false;

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(terminalID))
        //        {
        //            result = POSSettings(0x06, terminalID);
        //        }
        //        else if (!String.IsNullOrEmpty(merchantID))
        //        {
        //            result = POSSettings(0x07, merchantID);
        //        }
        //        else if (!String.IsNullOrEmpty(hostIP))
        //        {
        //            result = POSSettings(0x01, hostIP);
        //        }
        //        else if (!String.IsNullOrEmpty(hostPort))
        //        {
        //            result = POSSettings(0x02, hostPort);
        //        }
        //        else if (!String.IsNullOrEmpty(bankTCPIP))
        //        {
        //            result = POSSettings(0x03, bankTCPIP);
        //        }
        //        else if (!String.IsNullOrEmpty(bankTCPPort))
        //        {
        //            result = POSSettings(0x04, bankTCPPort);
        //        }
        //        else if (!String.IsNullOrEmpty(APN))
        //        {
        //            result = POSSettings(0x05, APN);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        result = false;
        //    }

        //    return result;
        //}

        //private bool POSSettings(byte settingType, string settingValue)
        //{
        //    bool result = false;
        //    byte[] dataBuff;
        //    byte[] sendData = null;
        //    byte[] tempData = null;
        //    MessageTypes messageType = MessageTypes.Unknown;

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(settingValue))
        //        {
        //            tempData = convert.String2Hex(settingValue);
        //            sendData = new byte[tempData.Length + 1];
        //            sendData[0] = settingType;
        //            Array.Copy(tempData, 0, sendData, 1, tempData.Length);

        //            if (message.SendReceive(MessageTypes.ChangePosSetting, sendData, out messageType, out dataBuff))
        //            {
        //                if (messageType == MessageTypes.ChangePosSetting)
        //                {
        //                    System.Threading.Thread.Sleep(10);

        //                    if (dataBuff.Length > 0)
        //                    {
        //                        if (dataBuff[0] == 0x00)
        //                            result = true;
        //                    }
        //                    else
        //                    {
        //                        //fix me : data bozuk
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        result = false;
        //    }

        //    return result;
        //}
    }
}
