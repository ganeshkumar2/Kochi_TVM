using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Utils
{
    public class DateTimeUtil
    {
        /**
         * Logger
         */
        private static readonly ILog log = LogManager.GetLogger(typeof(DateTimeUtil).Name);

        private static DateTimeUtil _instance = null;
        public static DateTimeUtil Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DateTimeUtil();
                return _instance;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        public struct SYSTEMTIME
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;
        }
        [DllImport("kernel32.dll", EntryPoint = "SetLocalTime", SetLastError = true)]
        public static extern int SetLocalTime(ref SYSTEMTIME lpSystemTime);
        [DllImport("kernel32.dll", EntryPoint = "GetLocalTime", SetLastError = true)]
        public static extern int GetLocalTime(ref SYSTEMTIME lpSystemTime);

        static DateTime lastTimeUpdate = DateTime.MinValue;
        private DateTime lastTryTosynchDate = DateTime.MinValue;
        public void SetSystemDateTime(DateTime datetime, bool timeFromSc = false)
        {
            try
            {
                if (Math.Abs((DateTime.Now - lastTryTosynchDate).TotalSeconds) < 5)
                {
                    return;
                }
                lastTryTosynchDate = DateTime.Now;
                log.Debug("Time Received in SetSystemDateTime : " + datetime);
                lastTimeUpdate = datetime;
                if (Math.Abs((DateTime.Now - lastTimeUpdate).TotalSeconds) > 10)
                {
                    SYSTEMTIME current = new SYSTEMTIME();
                    current.Year = (ushort)datetime.Year;
                    current.Month = (ushort)datetime.Month;
                    current.Day = (ushort)datetime.Day;
                    current.Hour = (ushort)datetime.Hour;
                    current.Minute = (ushort)datetime.Minute;
                    current.Second = (ushort)datetime.Second;
                    SetLocalTime(ref current);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error DateTimeUtil -> SetSystemDateTime() : " + ex.ToString());
            }
        }
    }
}
