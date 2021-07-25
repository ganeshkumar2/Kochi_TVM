using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Business
{
    public static class PeakTime
    {
        public static int RecId { get; set; }
        public static byte PeakTypeId { get; set; }
        public static string PeakName { get; set; }
        public static DateTime? SpecialDayDate { get; set; }
        public static TimeSpan? StartHour { get; set; }
        public static TimeSpan? EndHour { get; set; }
        public static decimal QrSjtMultiplier { get; set; }
        public static decimal QrRjtMultiplier { get; set; }
        public static decimal Rpt { get; set; }
        public static decimal Emv { get; set; }
        public static int DayTypeId { get; set; }
        public static int Version { get; set; }
    }
}
