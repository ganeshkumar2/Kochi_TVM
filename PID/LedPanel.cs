using Kochi_TVM.Business;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.PID
{
    enum Brightness
    {
        Percent0 = 0x00,
        Percent25 = 0x02,
        Percent50 = 0x05,
        Percent75 = 0x07,
        Percent100 = 0x0A
    }

    enum TextAction
    {
        Auto = 0x00,
        ShowNow = 0x01,
        ContinusLeft = 0x02,
        ContinusRight = 0x03,
        Snow = 0x12,
        Flick = 0x14,
        Jiggle = 0x15
    }

    public enum Speed
    {
        //longest Speed10
        Speed1 = 0x10,
        Speed2 = 0x25,
        Speed3 = 0x40,
        Speed4 = 0x55,
        Speed5 = 0x70,
        Speed6 = 0x85,
        Speed7 = 0xA0,
        Speed8 = 0xB5,
        Speed9 = 0xD0,
        Speed10 = 0xF0
    }

    enum Border
    {
        NoBorder = 0x00,
        Auto = 0x01,
        FixedLine = 0x02,
        Dotted1R = 0x03,
        Dotted1RGY = 0x04,
        Dotted2R = 0x05,
        Dotted2RGY = 0x06
    }

    public enum Color
    {
        Yellow = 0x00,
        Red = 0x01,
        Green = 0x02
    }
    public enum LedPanel_ErrorCodes
    {
        //device err : 10, device Code : 06, err Code : 1 - 99 => ex : 100312
        Success = 0,
        ParameterError = 100601,
        GetStatusError = 100602,
        ResetError = 100603,
        CommOpenError = 100604,
        EnableError = 100605,
        DisableError = 100606,
        UnknownError = 100499
    }
    enum Command
    {
        SendText = 0x77,
        RemoveAllText = 0x44,
        RemoveSelectedText = 0x64,
        ChooseText = 0x58,
        SetCommId = 0x1D
    }

    public class LedPanel
    {
        private static ILog log = LogManager.GetLogger(typeof(LedPanel).Name);

        private static LedPanel _instance = null;
        public static LedPanel Instance
        {
            get
            {
                if (_instance == null)
                {
                    string LED_COMPort = Parameters.TVMConst.LedPort;//ConfigurationManager.AppSettings["PID_PORT_NAME"];
                    _instance = new LedPanel(LED_COMPort);
                }
                return _instance;
            }
        }

        #region Definations
        private string id = "LedPanel";
        private string name = "LedPanel";
        private string description = "Led Panel";
        private SerialPort port;
        private string comPort;

        private bool isConnect = false;
        private bool isInit = false;
        private LedPanel_ErrorCodes errCode = LedPanel_ErrorCodes.Success;
        private string errDesc = String.Empty;

        Speed speed = Speed.Speed5;
        string text = String.Empty;
        byte[] data = new byte[99]; //max 100
        byte[] rcvData = new byte[2];
        byte[] sendData = new byte[512];
        Color color = Color.Red;
        #endregion

        #region interface functions
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
        public LedPanel(string portName)
        {
            OpenPort(portName);
        }

        private Boolean OpenPort(string portname)
        {
            try
            {
                port = new SerialPort(portname);
                port.StopBits = StopBits.One;
                port.Parity = Parity.None;
                port.BaudRate = 19200;
                port.ReadTimeout = 500;
                if (!port.IsOpen)
                    port.Open();
                log.Debug("LED Port Open");
                return true;
            }
            catch (Exception ex)
            {
                log.Error("LED Port Error " + ex.ToString());
                return false;
            }
        }
        public bool ChangeText(string Text, Speed spd, Color textcolor)
        {
            speed = spd;
            color = textcolor;
            if (!String.IsNullOrEmpty(Text))
            {
                text = Text.ToUpper();
                try
                {
                    if (PrepareBuff())
                    {
                        //Fix me : Open serial plug
                        //if(sp.SendReceive(sendData, ref rcvData, 100))
                        return true;
                    }
                }
                catch(Exception ex)
                {
                    log.Error("LedPanel - ChangeText " + ex.ToString());
                }
            }

            return false;
        }

        public bool Check()
        {
            return true;
        }

        public bool Connect()
        {
            bool result = true;
            errCode = LedPanel_ErrorCodes.Success;
            errDesc = String.Empty;
            return result;
        }

        public void Clear()
        {
            errCode = LedPanel_ErrorCodes.Success;
            errDesc = String.Empty;
            comPort = String.Empty;
            isInit = isConnect = false;
        }

        public void Disconnect()
        {
            try
            {
                port.Close();
                isConnect = false;
            }
            catch (Exception E)
            {
                errCode = LedPanel_ErrorCodes.UnknownError;
                errDesc = "Exception Message : " + E.Message;
            }
        }

        public bool GetError(ref int Code, ref string Desc)
        {
            Code = (int)errCode;
            Desc = errDesc;

            //return HasError();
            return errCode == LedPanel_ErrorCodes.Success;
        }

        #endregion

        bool PrepareBuff()
        {
            sendData = new byte[512];

            int index = 0;
            byte stx_etx = 0x79;
            byte rsType = 0x00; //send rs type
            byte address = 0x01; //comminication address
            byte page = 0x01; //show recorded page

            #region fix commands
            //stx
            sendData[index++] = stx_etx;
            //command
            sendData[index++] = 0x77;
            //fix data
            sendData[index++] = 0x00;
            //cominication address
            sendData[index++] = address;
            //rs type
            sendData[index++] = rsType;
            //fix data
            sendData[index++] = 0x00;
            #endregion

            //page id
            sendData[index++] = page;
            //dispilay data
            int dataLen = PrepareShowData();
            Array.Copy(data, 0, sendData, index, dataLen);
            index += dataLen;
            //check sum
            sendData[index++] = CheckSum(sendData, 1, index);
            //etx
            sendData[index++] = stx_etx;

            Array.Resize(ref sendData, index);

            port.DiscardInBuffer();
            port.DiscardOutBuffer();
            port.Write(sendData, 0, sendData.Length);           

            return true;
        }

        int PrepareShowData()
        {
            data = new byte[300];

            int index = 21;
            Brightness br = Brightness.Percent100;
            TextAction action = TextAction.ContinusLeft;
            Speed stay = (Speed)speed;
            
            Border border = Border.NoBorder;
            data[index++] = (byte)br;
            data[index++] = 0x00;
            data[index++] = (byte)action;
            data[index++] = (byte)0x04;//speed;
            data[index++] = (byte)0x00;//stay;
            data[index++] = (byte)border;

            for (int i = 0; i < text.Length; i++)
            {
                data[index++] = (byte)color;
                data[index++] = 0x00;
                data[index++] = (byte)((char)text[i]);
            }

            data[index++] = 0xFF;

            return index;
        }

        byte CheckSum(byte[] data, int index, int len)
        {
            byte sum = 0x00;

            for (int i = index; i < len; i++)
                sum += data[i];

            return sum;
        }

        public void Done()
        {

        }
    }
}
