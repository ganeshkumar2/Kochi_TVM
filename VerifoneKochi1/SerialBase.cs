using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.VerifoneKochi1
{
    public class SerialBase
    {

        int maxBuffSize = 256;
        //int minRcvBuffSize = 8;
        int maxTimeOut = 1000; //milisecond
        int maxTryCount = 5;
        bool isInit = false;
        SerialPort sp;


        //for async try
        //int index = 0;
        //bool messageSent = false; //control dublicate send
        //byte[] tempBuff;
        //public bool serialReadCompleted = false; //read data to end0

        public string comPort = String.Empty;
        public int baudrate = 115200;
        public bool isConnect = false;

        public SerialBase()
        { }

        public SerialBase(string ComPort)
        {
            Init(ComPort.ToUpper());
        }

        public SerialBase(int ComPort)
        {
            Init("COM" + ComPort.ToString());
        }

        public bool Send(byte[] sendBuff, int offset, int len)
        {
            bool result = false;

            try
            {
                if (isInit && isConnect)
                {
                    if (len > 0 && sendBuff.Length > 0)
                    {
                        sp.Write(sendBuff, offset, len);
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }


        public bool Receive(out byte[] rcvBuff, out int rcvLen)
        {
            bool result = false;
            rcvBuff = null;
            rcvLen = 0;
            try
            {
                if (isInit && isConnect)
                {
                    maxBuffSize = sp.BytesToRead;
                    byte[] data = new byte[maxBuffSize];
                    int dataLen = 0;
                    try
                    {
                        byte[] tempBuff = new byte[maxBuffSize];
                        int tempLen = sp.Read(tempBuff, 0, maxBuffSize);
                        if (tempLen > 0)
                        {
                            Array.Copy(tempBuff, 0, data, dataLen, tempLen);
                            dataLen += tempLen;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    if (dataLen > 0)
                    {
                        rcvBuff = data;
                        rcvLen = dataLen;
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        //private async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        await ReadDataAsync();
        //    }
        //    catch (Exception E)
        //    {
        //        throw E;
        //    }
        //}

        //private async Task ReadDataAsync()
        //{
        //    try
        //    {
        //        byte[] buffer = new byte[maxBuffSize];
        //        Task<int> readStringTask = sp.BaseStream.ReadAsync(buffer, 0, maxBuffSize);

        //        while (!readStringTask.IsCompleted)
        //        {
        //            System.Threading.Thread.Sleep(10);
        //        }

        //        int bytesRead = await readStringTask;
        //        if (bytesRead > minRcvBuffSize)
        //        {
        //            serialReadCompleted = true;
        //            rcvBuff = new byte[bytesRead];
        //            Array.Copy(buffer, 0, rcvBuff, 0, bytesRead);
        //            sp.BaseStream.Flush();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //private bool AddToBuffer(byte[] data, int dataLen)
        //{
        //    bool result = false;

        //    try
        //    {
        //        if (data != null && dataLen > 0)
        //        {
        //            Array.Copy(data, 0, tempBuff, index, dataLen);
        //            index += dataLen;

        //            if ((index >= minRcvBuffSize) && (tempBuff[index - 2] == 0x1A && tempBuff[index - 1] == 0xAF))
        //            {
        //                //message end
        //                rcvBuff = new byte[index];
        //                Array.Copy(tempBuff, 0, rcvBuff, 0, index);
        //                index = 0;
        //                tempBuff = new byte[maxBuffSize];
        //                serialReadCompleted = true;
        //                messageSent = false;
        //                result = true;
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //    return result;
        //}

        public void Close()
        {
            if (isConnect)
            {
                sp.Close();
            }

            Clear();
        }

        bool Open()
        {
            bool result = false;

            try
            {
                if (!isConnect)
                {
                    if (!String.IsNullOrEmpty(comPort))
                    {
                        sp = new SerialPort(comPort, baudrate);
                        sp.ReadTimeout = maxTimeOut;
                        //sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler); 
                        sp.Open();
                        result = isConnect = sp.IsOpen;
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        void Clear()
        {
            isInit = false;
            isConnect = false;
            comPort = String.Empty;
        }

        void Init(string port)
        {
            try
            {
                if (!isInit)
                {
                    if (!String.IsNullOrEmpty(port))
                    {
                        comPort = port;
                        isInit = true;
                        Open();
                    }
                    else
                    {
                        isInit = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }
    }

    public class CRC
    {
        public byte CalculateCRC(byte[] data, int dataLen)
        {
            byte crc = 0x00;

            try
            {
                if (data != null)
                {
                    for (int i = 0; i < dataLen; i++)
                    {
                        crc = (byte)((crc ^ data[i]) & 0xFF);
                    }
                }
            }
            catch (Exception e)
            {
                crc = 0x00;
            }

            return crc;
        }

    }
}
