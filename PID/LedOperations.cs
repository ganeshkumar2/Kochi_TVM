using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.PID
{
    public static class LedOperations
    {
        static string test = "Test";
        static string welcome = "In Service";
        static string closed = "Out Of Service";
        static string emergency = "Emergency";
        static string stationClosed = "Station Closed";
        static string maintance = "Maintenance";

        static bool SendGreenText(string text)
        {
            const int MaxLength = 90;
            if (text.Length > MaxLength)
                text = text.Substring(0, MaxLength);
            return LedPanel.Instance.ChangeText(text, Speed.Speed1,Color.Green);
        }

        static bool SendRedText(string text)
        {
            const int MaxLength = 90;
            if (text.Length > MaxLength)
                text = text.Substring(0, MaxLength);
            return LedPanel.Instance.ChangeText(text, Speed.Speed1, Color.Red);
        }

        public static void Open()
        {
            SendGreenText(welcome);
        }

        public static void Close()
        {
            SendRedText(closed);
        }

        public static void StationClose()
        {
            SendRedText(stationClosed);
        }

        public static void GreenText(string text)
        {
            SendGreenText(text);
        }

        public static void RedText(string text)
        {
            SendRedText(text);
        }

        public static void Emergency()
        {
            SendRedText(emergency);
        }

        public static void Maintance()
        {
            SendRedText(maintance);
        }
        public static void DeviceError(string device)
        {
            SendRedText(device+" DEVICE ERROR");
        }
    }
}
