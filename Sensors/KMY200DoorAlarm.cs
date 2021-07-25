using Kochi_TVM.Business;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace Kochi_TVM.Sensors
{
    public class KMY200DoorAlarm
    {
        private static ILog log = LogManager.GetLogger(typeof(KMY200DoorAlarm).Name);
        private bool isConnect = false;
        private static SerialPort port;
        private static Timer checkPortTimer;
        private static TimerCallback checkPortTimerDelegate;
        private byte[] DoorData = new byte[] { 0x02, 0x01, 0x73, 0x72, 0x03 };
        private byte[] NormalOpenData = new byte[] { 0x1B, 0x70 };
        private byte[] SetAlarmData = new byte[] { 0x1B, 0x71 };
        private byte[] CloseAlarmData = new byte[] { 0x1B, 0x72 };

        public delegate void DoorOpenInputEventHandler();
        public static event DoorOpenInputEventHandler DoorOpenInputEvent;

        public delegate void HelpButtonInputEventHandler();
        public static event HelpButtonInputEventHandler HelpButtonInputEvent;

        public delegate void VibrationSensorInputEventHandler();
        public static event VibrationSensorInputEventHandler VibrationSensorInputEvent;

        private static KMY200DoorAlarm _instance = null;
        public static KMY200DoorAlarm Instance
        {
            get
            {
                if (_instance == null)
                {
                    string COMPort = Parameters.TVMConst.SensorPort;//ConfigurationManager.AppSettings["PID_PORT_NAME"];
                    _instance = new KMY200DoorAlarm(COMPort);
                }
                return _instance;
            }
        }
        private void Initialize(string portName)
        {
            try
            {
                if (port == null || !port.IsOpen)
                {
                    port = new SerialPort();
                    port.BaudRate = 9600;
                    port.PortName = portName;
                    port.ReadTimeout = 500;
                    port.Open();
                    isConnect = true;
                }
                else
                {
                    try
                    {
                        port.Close();
                    }
                    catch (Exception ex1) { }
                    port = new SerialPort();
                    port.BaudRate = 9600;
                    port.PortName = portName;
                    port.ReadTimeout = 500;
                    port.Open();
                    isConnect = true;
                }
                checkPortTimerDelegate = new TimerCallback(KeepAliveDoor);
                checkPortTimer = new Timer(checkPortTimerDelegate, null, 1000, 30000);
                //keepAliveThread = new Thread(KeepAliveDoor);
                //keepAliveThread.Start();
            }
            catch (Exception ex)
            {
                isConnect = false;
            }
        }
        private void KeepAliveDoor(object o)
        {
            try
            {
                if (port == null || !port.IsOpen)
                    return;

                if (port.BytesToRead > 0)
                {
                    byte[] a = new byte[2];
                    a[0] = (byte)port.ReadByte();
                    a[1] = (byte)port.ReadByte();

                    if (a[0] == 1 && a[1] == 0)
                    {
                        if (DoorOpenInputEvent != null)
                            DoorOpenInputEvent();
                    }

                    if (a[0] == 27 && a[1] == 90)
                    {
                        if (VibrationSensorInputEvent != null)
                            VibrationSensorInputEvent();
                    }

                    if (a[0] == 27 && a[1] == 91)
                    {
                        if (HelpButtonInputEvent != null)
                            HelpButtonInputEvent();
                    }

                    log.Debug("KMY200DoorAlarm BytesToRead 0 " + a[0] + " 1 " + a[1]);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public KMY200DoorAlarm(string portName)
        {
            try
            {
                Initialize(portName);
            }
            catch (Exception ex)
            {
                log.Error("KMY200DoorAlarm Port Error " + ex.ToString());
            }
        }

        public void Disconnect()
        {
            try
            {
                port.Close();
                isConnect = false;
            }
            catch (Exception ex)
            {
                log.Error("KMY200DoorAlarm -> Disconnect() " + ex.ToString());
            }
        }

        public int GetStatus()
        {
            int result = 0;
            try
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Write(DoorData, 0, DoorData.Length);
                log.Debug("GetStatus data:" + BitConverter.ToString(DoorData));
                Thread.Sleep(30);

                byte[] res = new byte[1];
                if (port.BytesToRead > 0)
                {
                    int cnt = port.Read(res, 0, 1);
                    log.Debug("res cnt :" + cnt);
                    log.Debug("res data:" + res[0]);
                    if (cnt == 1)
                    {
                        log.Debug("res data:" + res[0]);

                        int num = res[0];

                        result = num;
                    }
                }
                log.Debug("KMY200DoorAlarm -> GetStatus() " + result);                
            }
            catch (Exception ex)
            {
                log.Error("KMY200DoorAlarm -> GetStatus() " + ex.ToString());
            }
            return result;
        }

        public bool SetNormalOpen()
        {
            bool result = false;
            try
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Write(NormalOpenData, 0, NormalOpenData.Length);
                log.Debug("SetNormalOpen data:" + BitConverter.ToString(NormalOpenData));
                Thread.Sleep(30);

                byte[] res = new byte[1];
                if (port.BytesToRead > 0)
                {
                    int cnt = port.Read(res, 0, 1);
                    log.Debug("res cnt :" + cnt);
                    log.Debug("res data:" + res[0]);
                    if (cnt == 1)
                    {
                        if (res[0] == 112)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        log.Debug("res data:" + res[0]);
                    }
                }
                log.Debug("KMY200DoorAlarm -> SetNormalOpen() " + result);
            }
            catch (Exception ex)
            {
                log.Error("KMY200DoorAlarm -> SetNormalOpen() " + ex.ToString());
            }
            return result;
        }

        public bool SetAlarm()
        {
            bool result = false;
            try
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Write(SetAlarmData, 0, SetAlarmData.Length);
                log.Debug("SetAlarm data:" + BitConverter.ToString(SetAlarmData));
                Thread.Sleep(30);

                byte[] res = new byte[1];
                if (port.BytesToRead > 0)
                {
                    int cnt = port.Read(res, 0, 1);
                    log.Debug("SetAlarm res cnt :" + cnt);
                    log.Debug("SetAlarm res data:" + res[0]);
                    if (cnt == 1)
                    {
                        if (res[0] == 113)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        log.Debug("SetAlarm res data:" + res[0]);
                    }
                }
                log.Debug("KMY200DoorAlarm -> SetAlarm() " + result);
            }
            catch (Exception ex)
            {
                log.Error("KMY200DoorAlarm -> SetAlarm() " + ex.ToString());
            }
            return result;
        }

        public bool SetAlarmClose()
        {
            bool result = false;
            try
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Write(CloseAlarmData, 0, CloseAlarmData.Length);
                log.Debug("SetAlarmClose data:" + BitConverter.ToString(CloseAlarmData));
                Thread.Sleep(30);

                byte[] res = new byte[1];
                if (port.BytesToRead > 0)
                {
                    int cnt = port.Read(res, 0, 1);
                    log.Debug("SetAlarmClose res cnt :" + cnt);
                    log.Debug("SetAlarmClose res data:" + res[0]);
                    if (cnt == 1)
                    {
                        if (res[0] == 114)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        log.Debug("SetAlarmClose res data:" + res[0]);
                    }
                }
                log.Debug("KMY200DoorAlarm -> SetAlarmClose() " + result);
            }
            catch (Exception ex)
            {
                log.Error("KMY200DoorAlarm -> SetAlarmClosemz/() " + ex.ToString());
            }
            return result;
        }
    }
}
