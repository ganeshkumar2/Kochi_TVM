using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Kochi_TVM.PID
{    
    class PIDMessageLog
    {
        private static ILog log = LogManager.GetLogger(typeof(PIDMessageLog).Name);
        private static XmlDocument _doc;
        private static string folderName = AppDomain.CurrentDomain.BaseDirectory + "PIDMessageLog";
        private static string fileName = "PIDMessageLog.xml";
        private static object salesLimitLock = new object();
        private static object initialLock = new object();
        private static void CheckFolders()
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }
        public static void createFile()
        {
            lock (initialLock)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();

                    XmlElement header = doc.CreateElement("PIDMessage");

                    {
                        XmlAttribute Cassette1Value = doc.CreateAttribute("Message");
                        Cassette1Value.InnerText = "";
                        header.Attributes.Append(Cassette1Value);
                    }

                    doc.AppendChild(header);

                    //create doc content into file
                    CheckFolders();
                    XmlWriter writer = XmlWriter.Create(folderName + "\\" + fileName);
                    doc.WriteTo(writer);
                    writer.Close();

                }
                catch (Exception ex)
                {
                    log.Error("Error PIDMessageLog -> createFile() : " + ex.ToString());
                }
            }
        }

        private static bool loadFile()
        {
            lock (salesLimitLock)
            {
                try
                {
                    _doc = new XmlDocument();

                    if (File.Exists(folderName + "\\" + fileName) == false)
                        createFile();

                    _doc.Load(folderName + "\\" + fileName);
                    return true;
                }
                catch (Exception ex)
                {                   
                    return false;
                }
            }
        }

        public static void DeleteFile()
        {
            try
            {
                File.Delete(folderName + "\\" + fileName);
            }
            catch (Exception ex)
            {
                log.Error("Error PIDMessageLog -> DeleteFile() : " + ex.ToString());
            }
        }
     
        private static void saveDocToFile()
        {
            try
            {
                File.WriteAllText(folderName + "\\" + fileName, _doc.OuterXml);
            }
            catch (Exception ex1)
            {
                log.Error("Error PIDMessageLog -> saveDocToFile() 1 : " + ex1.ToString());
                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Thread.Sleep(1000);
                        File.WriteAllText(folderName + "\\" + fileName, _doc.OuterXml);
                        if (loadFile())
                            break;
                    }
                }
                catch (Exception ex2)
                {
                    
                }
            }
        }
        public static void setMessage(string val)
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    root.Attributes["Message"].Value = val.ToString();
                    checkAndSave(_doc, val.ToString(), "Message");
                }
                catch (Exception ex)
                {
                    log.Error("Error PIDMessageLog -> setMessage() : " + ex.ToString());
                }
            }
        }

        public static string getMessage()
        {
            lock (salesLimitLock)
            {
                try
                {
                    if (_doc == null)
                        loadFile();

                    XmlElement root = _doc.DocumentElement;

                    if (!root.HasAttribute("Message"))
                    {
                        root.SetAttribute("Message", "");
                        _doc.Save(fileName);
                        _doc = new XmlDocument();
                        _doc.Load(fileName);
                    }

                    string temp = root.GetAttribute("Message");
                    return temp;
                }
                catch (Exception ex)
                {
                    log.Error("Error PIDMessageLog -> getMessage() : " + ex.ToString());
                    return "";
                }
            }
        }

        private static void checkAndSave(XmlDocument _doc, string value, string valueName)
        {
            try
            {
                //_doc.Save(folderName + "\\" + fileName);
                saveDocToFile();

                if (!loadFile())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        loadFile();

                        XmlElement root = _doc.DocumentElement;

                        root.Attributes[valueName].Value = value;
                        //_doc.Save(folderName + "\\" + fileName);
                        saveDocToFile();
                        if (loadFile())
                            break;
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
