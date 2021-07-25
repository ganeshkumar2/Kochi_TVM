using Kochi_TVM.Business;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Speech.Synthesis;
using System.Windows.Media;
using System.Windows.Threading;
using ZXing.QrCode;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Utils
{
    public static class TVMUtility
    {
        private static ILog log = LogManager.GetLogger(typeof(TVMUtility).Name);

        private static MediaPlayer mplayer = new MediaPlayer();
        private static SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private static int voiceFile = 0;
        private static string langVal = "";
        private static string lastMessage = "";
        public static void PlayClick()
        {
            try
            {
                mplayer.Stop();
                mplayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\SoundFiles\MouseClick.wav", UriKind.Relative));
                mplayer.Play();
            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> PlayClick() : " + ex.ToString());
            }
        }
        public static void SpeechLastMessage(string message) // defining the function which will accept a string parameter
        {
            CancelSpeach();
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult); // to change VoiceGender and VoiceAge check out those links below
            synthesizer.Volume = 100;  // (0 - 100)
            synthesizer.Rate = -1;     // (-10 - 10)
            // Asynchronous
            synthesizer.SpeakAsync(message); // here args = pran
        }
        public static void SpeechFirstMessage(int voice, string message, string lang) // defining the function which will accept a string parameter
        {
            CancelSpeach();
            voiceFile = voice;
            langVal = lang;
            synthesizer.SpeakCompleted -= VoieEndedLoop;
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult); // to change VoiceGender and VoiceAge check out those links below
            synthesizer.Volume = 100;  // (0 - 100)
            synthesizer.Rate = -1;     // (-10 - 10)
            // Asynchronous
            synthesizer.SpeakAsync(message); // here args = pran
            synthesizer.SpeakCompleted += VoieEndedLoop;
        }

        private static void VoieEndedLoop(object sender, SpeakCompletedEventArgs e)
        {
            mplayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Resources\" + langVal + @"\" + langVal + voiceFile + ".mp3", UriKind.Relative));
            mplayer.Play();
        }

        public static void PlayVoice(int voice, string firstmessage = null, string lastmessage = null, string lang = "EN")
        {
            try
            {
                StopAudio();

                if (firstmessage != null)
                {
                    SpeechFirstMessage(voice,firstmessage, lang);
                    return;
                }
                mplayer.MediaEnded -= new EventHandler(MediaEndedLoop);
                mplayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Resources\"+ lang + @"\" + lang + voice + ".mp3", UriKind.Relative));
                mplayer.Play();
                mplayer.MediaEnded += new EventHandler(MediaEndedLoop);
                if (lastmessage != null)
                {
                    lastMessage = lastmessage;
                }
                else
                {
                    lastMessage = "";
                }

            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> PlayVoice() : " + ex.ToString());
            }
        }

        private static void MediaEndedLoop(object sender, EventArgs e)
        {
            if(lastMessage != "")
                SpeechLastMessage(lastMessage);

            lastMessage = "";
        }

        public static void StopAudio()
        {
            synthesizer.SpeakAsyncCancelAll();
            mplayer.Stop();
        }
        public static void CancelSpeach()
        {
            synthesizer.SpeakAsyncCancelAll();
        }
        public static string Hex2Binary(string value)
        {
            int num;
            num = int.Parse(value);
            string binaryval = "";
            string rem = "";
            int quot;
            while (num >= 1)
            {
                quot = num / 2;
                rem += (num % 2).ToString();
                num = quot;
            }
            for (int i = rem.Length - 1; i >= 0; i--)
            {
                binaryval = binaryval + rem[i];
            }
            return binaryval;
        }
        public static byte[] EnableBill(decimal Amount)
        {
            int ivar = 255;

            // for not accepting
            //1111 1110 -> FE 5/-
            //1111 1101 -> FD 10/-
            //1111 1011 -> FB 20/-
            //1111 0111 -> F7 50/-
            //1110 1111 -> EF 100/-
            //1101 1111 -> DF 200/-
            //1011 1111 -> BF 500/-
            //0111 1111 -> 7F 2000/-

            // for accepting all
            //1111 1111 -> FF

            if ((Constants.Cassette1NoteCont <= Constants.NoChangeAvailable && Constants.Cassette2NoteCont <= Constants.NoChangeAvailable && Constants.Cassette3NoteCont <= Constants.NoChangeAvailable) || (StockOperations.coin1 <= Constants.NoChangeAvailable && StockOperations.coin2 <= Constants.NoChangeAvailable && StockOperations.coin1 <= Constants.NoChangeAvailable))
            {
                if (Amount <= 5)
                    ivar = ivar & 0x01;
                if (Amount > 5 && Amount <= 10)
                    ivar = ivar & 0x03;
                if (Amount > 10 && Amount <= 20)
                    ivar = ivar & 0x07;
                if (Amount > 20 && Amount <= 50)
                    ivar = ivar & 0x0F;
                if (Amount > 50 && Amount <= 100)
                    ivar = ivar & 0x1F;
                if (Amount > 100 && Amount <= 200)
                    ivar = ivar & 0x3F;
                if (Amount > 200 && Amount <= 1500)
                    ivar = ivar & 0x7F;
                if (Amount > 1500)
                    ivar = ivar & 0xFF;
            }
            else
            {
                if (Amount < 50)
                    ivar = ivar & 0x0F;
                if (Amount >= 50 && Amount <= 100)
                    ivar = ivar & 0x1E;
                else if (Amount > 100 && Amount <= 200)
                    ivar = ivar & 0x3E;
                else if (Amount > 200 && Amount <= 1500)
                    ivar = ivar & 0x7E;
                else if (Amount > 1500)
                    ivar = ivar & 0xFE;
            }

            /// for not accepting 10/-
            /// byte[] byte2 = { 0xFF, 0xFF, 0xFD, 0x00, 0x00, 0x00 };

            byte byte1 = Convert.ToByte(ivar);
            byte[] byte2 = { 0xFF, 0xFF, byte1, 0x00, 0x00, 0x00 };
            return byte2;
        }
        public static System.Drawing.Bitmap PrepareQRImage(string content)
        {
            try
            {
                var qr = new ZXing.BarcodeWriter();
                qr.Format = ZXing.BarcodeFormat.QR_CODE;
                qr.Options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = 200,//200,
                    Height = 200
                };
                var result = new System.Drawing.Bitmap(qr.Write(content.Trim()));
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> PrepareQRImage() : " + ex.ToString());
                return null;
            }
        }
        public static void killExplorer()
        {
            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "taskkill.exe",
                        Arguments = "/F /IM explorer.exe",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> killExplorer() : " + ex.ToString());
            }
        }
        public static void RestartExplorer()
        {
            string strCmdText;
            strCmdText = "/c taskkill /f /im explorer.exe";

            System.Diagnostics.Process.Start("cmd.exe", strCmdText);
            System.Threading.Thread.Sleep(500);
            Process.Start(Environment.SystemDirectory + "\\..\\explorer.exe");
        }
        public static void ShutDownSystem()
        {
            Process proccess = new Process();
            proccess.StartInfo.FileName = "shutdown";
            proccess.StartInfo.Arguments = "/s /f /t 000";
            proccess.StartInfo.CreateNoWindow = true;
            proccess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proccess.EnableRaisingEvents = false;
            proccess.Start();
            proccess.WaitForExit();
        }
        public static void RestartSystem()
        {
            Process proccess = new Process();
            proccess.StartInfo.FileName = "shutdown";
            proccess.StartInfo.Arguments = "/r /f /t 000";
            proccess.StartInfo.CreateNoWindow = true;
            proccess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proccess.EnableRaisingEvents = false;
            proccess.Start();
            proccess.WaitForExit();
        }
        public static int BillTypeToBillValue(int billtype)
        {
            try
            {
                string bNRBillType = ((Enums.BNRBillType)billtype).ToString().Substring(1);
                return Convert.ToInt16(bNRBillType);
            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> BillTypeToBillValue() : " + ex.ToString());
                return 0;
            }
        }
        public static string TransactionSequenceFile = "tsid.sis";
        public static byte[] GetByteValue(long intValue, int byteCount)
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);
            Array.Reverse(intBytes);
            byte[] ret = new byte[byteCount];
            for (int i = byteCount - 1, j = 0; i >= 0; i--, j++)
                ret[j] = intBytes[intBytes.Length - i - 1];
            return ret;
        }
        private static int LoadTransactionFileSequenceID()
        {
            int tsid = 0;
            try
            {
                FileStream tw1 = null;
                if (!File.Exists(Constants.BaseAddress + TransactionSequenceFile))
                {
                    tw1 = new FileStream(Constants.BaseAddress + TransactionSequenceFile, FileMode.Create, FileAccess.Write);

                    tw1.Write(GetByteValue(0, 4), 0, 4);
                    tw1.Flush();
                }
                else
                {
                    tw1 = new FileStream(Constants.BaseAddress + TransactionSequenceFile, FileMode.Open, FileAccess.Read);
                    tsid = (int)GetValue(new byte[] { (byte)tw1.ReadByte(), (byte)tw1.ReadByte(), (byte)tw1.ReadByte(), (byte)tw1.ReadByte() }, 0, 4);
                }
                tw1.Close();
            }
            catch (Exception)
            {
            }
            return tsid;
        }
        public static ulong GetValue(byte[] bytes, int start, int len)
        {
            ulong val = 0;
            for (int i = 0; i < len; i++)
            {
                val = (val * 256) + bytes[start + i];
            }
            return val;
        }

        public static string GenerateTransactionNumber()
        {
            try
            {
                return IncreamentTransactionFileSequenceID(LoadTransactionFileSequenceID()).ToString();
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        public static int IncreamentTransactionFileSequenceID(int tranid)
        {
            try
            {
                try
                {
                    tranid = (tranid + 1) % 100000000;

                    FileStream tw1 = new FileStream(Constants.BaseAddress + TransactionSequenceFile, FileMode.OpenOrCreate, FileAccess.Write);
                    tw1.Write(GetByteValue(tranid, 4), 0, 4);
                    tw1.Flush();
                    tw1.Close();
                }
                catch (FileNotFoundException)
                {
                }
                return tranid;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static int ResetTransactionFileSequenceID(int tranid)
        {
            try
            {
                try
                {
                    tranid = (tranid) % 100000000;

                    FileStream tw1 = new FileStream(Constants.BaseAddress + TransactionSequenceFile, FileMode.OpenOrCreate, FileAccess.Write);
                    tw1.Write(GetByteValue(tranid, 4), 0, 4);
                    tw1.Flush();
                    tw1.Close();
                }
                catch (FileNotFoundException)
                {
                }
                return tranid;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static byte[] GetSamUnlockKey(SamCardType samCardType)
        {
            switch (samCardType)
            {
                case SamCardType.KochiTest:
                    return new byte[16] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };

                case SamCardType.KochiProd:
                    return new byte[16] { 0x0E, 0x34, 0xCE, 0x38, 0x4A, 0x75, 0x6D, 0x61, 0x0B, 0xF4, 0x08, 0xA2, 0x25, 0x83, 0xDF, 0x10 };

                case SamCardType.AmritsarTest:
                    return new byte[16] { 0x05, 0x9D, 0xEC, 0xA2, 0xC7, 0x21, 0x96, 0x4F, 0xA3, 0x46, 0x8A, 0xE6, 0x36, 0x4E, 0x8D, 0xA9 };

                case SamCardType.AmritsarProd:
                    return new byte[16] { 0x57, 0x0D, 0x64, 0xF4, 0x9E, 0x41, 0xE0, 0x43, 0xAE, 0x28, 0x2C, 0x58, 0xCA, 0xEC, 0x28, 0x57 };

                case SamCardType.IndoreTest:
                    return new byte[16] { 0x05, 0x9D, 0xEC, 0xA2, 0xC7, 0x21, 0x96, 0x4F, 0xA3, 0x46, 0x8A, 0xE6, 0x36, 0x4E, 0x8D, 0xA9 };

                case SamCardType.IndoreProd:
                    return new byte[16] { 0x8F, 0xF8, 0xAA, 0xB4, 0xF4, 0x48, 0x05, 0x42, 0x88, 0x63, 0x29, 0x98, 0x80, 0x8C, 0x0B, 0x3D };

                default:
                    return new byte[] { 0 };
            }
        }
    }
}
