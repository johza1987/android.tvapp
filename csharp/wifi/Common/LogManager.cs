using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace SinetWifi.Common
{
    public class LogManager
    {
        public static readonly object lockerError = new object();
        public static readonly object lockerInfo = new object();

        private static void Init()
        {
            string logPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }
        public static void Error(string message)
        {
            new Thread(() => WriteError(message)).Start();
        }

        public static void Info(string message)
        {
            new Thread(() => WriteInfo(message)).Start();
        }

        public static void More(string message, string fileName)
        {
            new Thread(() => WriteMore(message, fileName)).Start();
        }


        private static void WriteError(string message)
        {
            lock (lockerError)
            {
                Init();
                string fileName = string.Empty;
                fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\" + String.Format("Error-{0:yyyyMMdd}", DateTime.Now) + ".txt";
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                }
            }
        }

        private static void WriteInfo(string message)
        {
            lock (lockerInfo)
            {
                Init();
                string fileName = string.Empty;
                fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\" + String.Format("Info-{0:yyyyMMdd}", DateTime.Now) + ".txt";
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                }
            }
        }

        private static void WriteMore(string message, string fileName)
        {
            lock (lockerInfo)
            {
                Init();
                fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + fileName + ".txt";
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, true))
                {
                    sw.Write(String.Format("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now));
                    sw.WriteLine(message);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
    }
}