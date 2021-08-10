using Kochi_TVM.Printers;
using System;
using System.Collections.Generic;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Utils
{
    public static class Constants
    {
        public static ApplicationVersion ApplicationVersion = ApplicationVersion.Rail;

        public static string BaseAddress = AppDomain.CurrentDomain.BaseDirectory;
        public static string appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static int CheckDeviceTime = 20000;

        public static bool IsVoiceEnabled = false;
        //BNR
        public static string BNRStatus = "";
        public static bool IsBNRAvalable = false;
        public static int Cassette1Note = 0;
        public static int Cassette2Note = 0;
        public static int Cassette3Note = 0;
        public static int Cassette1NoteCont = 0;
        public static int Cassette2NoteCont = 0;
        public static int Cassette3NoteCont = 0;
        public static int EscrowCassetteNo = 0;
        public static int EscrowAmount = 50;
        public static Cassette[] CassettesInfo = new Cassette[3];
        public static byte[] EnableBillNotes = new byte[6];
        public static bool NoChangeMode = false;
        public static bool NoReceiptMode = false;

        public static string Hopper1Level = "";
        public static string Hopper2Level = "";
        public static string Hopper3Level = "";

        public static int HopperAddress3Coin = 5;
        public static int HopperAddress2Coin = 2;
        public static int HopperAddress1Coin = 1;
        public static int NoChangeAvailable = 0;

        public static string CurrencySymbol = "₹";

        public static int Change = 0;
        public static SamCardType SamCardType { get; set; } = SamCardType.KochiTest;

        public static int SystemIdleTimeout = 1;
        public static bool IsMaintenanceActive = false;
        public static int MaintenanceSeq = 0;
        public static bool IsMapPageActive = false;

        public static string CoinAvailable = "1,2,5,10,20";

        public static bool BNRTest = false; 
        public static bool HopperTest = false; 
        public static List<CoinOperation> CoinAddOperations = new List<CoinOperation>(); 
        public static List<CoinOperation> CoinDispOperations = new List<CoinOperation>();
        public static List<CoinOperation> CoinEmptyOperations = new List<CoinOperation>();
        public static List<QR_RPT_Operation> QR_RPT_OperationsAdd = new List<QR_RPT_Operation>();
        public static List<QR_RPT_Operation> QR_RPT_OperationsDisp = new List<QR_RPT_Operation>();
        public static List<QR_RPT_Operation> QR_RPT_OperationsEmpty = new List<QR_RPT_Operation>();
        public static List<SendToBox> SendToBoxes = new List<SendToBox>();
        public static List<SendToBox> Removecashbox = new List<SendToBox>();
        public static List<AddNote> AddNotes = new List<AddNote>();
    }
}
