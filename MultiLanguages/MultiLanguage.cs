using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kochi_TVM.MultiLanguages
{
    public class MultiLanguage
    {
        private static string defaultLanguage = "EN";
        private static string currentLanguage;
        private static string resPath;
        private static ResourceDictionary resDict;
        private static Dictionary<string, ResourceDictionary> languageDict;
        private static Dictionary<string, string> supportedLanguages;
        private static LocalSettings cnv;



        public static void Init(string language)
        {

            languageDict = new Dictionary<string, ResourceDictionary>();
            supportedLanguages = new Dictionary<string, string>();

            currentLanguage = defaultLanguage;
            if (!String.IsNullOrEmpty(language))
                currentLanguage = language;

            resPath = AppDomain.CurrentDomain.BaseDirectory + "Resources";

            supportedLanguages.Add("EN", "EN-en");
            supportedLanguages.Add("ML", "ML-ml");
            supportedLanguages.Add("IN", "IN-in");

            Load();
            SetCulturelFunctions();
        }
        public static bool ChangeLanguage(string lang)
        {
            bool result = false;

            try
            {
                if (supportedLanguages.ContainsKey(lang))
                {
                    currentLanguage = lang;
                    SetCulturelFunctions();
                    result = true;
                }
            }
            catch (Exception e) { result = false; }
            return result;
        }
        public static string GetCurrentLanguage()
        {
            return currentLanguage;
        }
        public static string GetText(string key)
        {
            string value = string.Empty;

            try
            {
                if (languageDict.ContainsKey(currentLanguage))
                {
                    if (languageDict[currentLanguage].Contains(key))
                    {
                        value = languageDict[currentLanguage][key].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                value = string.Empty;
            }

            return value;
        }
        private static void Clear()
        {
            currentLanguage = string.Empty;
            resDict.Clear();
        }
        private static bool Load()
        {
            foreach (KeyValuePair<string, string> lang in supportedLanguages)
            {
                resDict = new ResourceDictionary();
                resDict.Source = new System.Uri(resPath + "\\" + lang.Key + "\\Resource.xaml", UriKind.Absolute);
                languageDict.Add(lang.Key, resDict);
            }

            return true;
        }
        private static void SetCulturelFunctions()
        {
            switch (currentLanguage)
            {
                //case "IN":                
                //    cnv = new IdiaConversion();               
                //    break;
                //case "ML":               
                //    cnv = new MalayalamConversion();               
                //    break;
                //case "EN":
                //    cnv = new EnglishConversion();
                //    break;
                default:
                    cnv = new DefaultConversion();
                    break;
            }
        }

        private static bool Change2DefaultLanguage()
        {
            currentLanguage = defaultLanguage;
            return true;
        }
        private static bool SetDefaultLanguage(string lang)
        {
            bool result = false;

            try
            {
                if (supportedLanguages.ContainsKey(lang))
                {
                    defaultLanguage = lang;
                    result = true;
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;

        }
        private static string GetSoundPath()
        {
            try
            {
                string path = resPath + "\\" + currentLanguage + "\\Sound";
                if (System.IO.Directory.Exists(path))
                    path = path + "\\";
                else
                    path = string.Empty;

                return path;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        private static string GetSound(string sound)
        {
            return GetSoundPath() + sound;
        }
        private static string GetDate()
        {
            return cnv.GetDate();
        }
        private static string GetTime()
        {
            return cnv.GetTime();
        }
        private static string GetCurrency()
        {
            return cnv.GetCurrency();
        }
        private static string NumberConversion(string numericValue)
        {
            return cnv.NumberConversion(numericValue);
        }

    }
}
