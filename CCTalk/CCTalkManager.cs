using dk.CctalkLib.Devices;
using Kochi_TVM.Business;
using log4net;
using System;
using System.Configuration;
using System.IO.Ports;

namespace Kochi_TVM.CCTalk
{
    class CCTalkManager
    {
        private static ILog log = LogManager.GetLogger(typeof(CCTalkManager).Name);

        private SerialPort port;

        public CoinHopperEV4000 coinHopperEV4000_1;
        public CoinHopperEV4000 coinHopperEV4000_2;
        public CoinHopperEV4000 coinHopperEV4000_3;

        public CCTalkManager()
        {
            InitialPort();
            InitialCoinHoppers();
        }
        private void InitialPort()
        {
            try
            {
                port = new SerialPort();
                port.BaudRate = 9600;
                port.PortName = Parameters.TVMConst.Hopper1Port;//ConfigurationManager.AppSettings["CoinHopper_Port_Name"];
                port.ReadTimeout = 3000;
                port.Open();
            }
            catch (Exception ex)
            {
                log.Error("Error CCTalkManager -> InitialPort() : " + ex.ToString());
                port.Dispose();
            }
        }


        private static CCTalkManager _instance = null;
        public static CCTalkManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CCTalkManager();
                return _instance;
            }
        }

        private void InitialCoinHoppers()
        {
            try
            {
                coinHopperEV4000_1 = new CoinHopperEV4000(port, 0x03);
                coinHopperEV4000_2 = new CoinHopperEV4000(port, 0x04);
                coinHopperEV4000_3 = new CoinHopperEV4000(port, 0x05);
            }
            catch (Exception ex)
            {
                log.Error("Error CCTalkManager -> InitialCoinHoppers() : " + ex.ToString());
                port.Dispose();
            }
        }
    }
}
