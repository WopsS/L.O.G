using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class LOG
    {
        private static string ServerLog = AppDomain.CurrentDomain.BaseDirectory + "server_log.txt";

        public static void DisplayLOG(string text, params object[] parameters)
        {
            DisplayLOG(false, true, true, false, text, parameters);
        }

        public static void DisplayLOG(bool WriteInLOG, string text, params object[] parameters)
        {
            DisplayLOG(WriteInLOG, true, true, false, text, parameters);
        }

        public static void DisplayLOG(bool WriteInLOG, bool ShowDate, string text, params object[] parameters)
        {
            DisplayLOG(WriteInLOG, true, ShowDate, false, text, parameters);
        }

        public static void DisplayLOG(bool WriteInLOG, bool ShowDate, bool NewLine, string text, params object[] parameters)
        {
            DisplayLOG(WriteInLOG, true, ShowDate, NewLine, text, parameters);
        }

        public static void DisplayLOG(bool WriteInLOG, bool WriteConsoleLine, bool ShowDate, bool NewLine, string text, params object[] parameters)
        {
            if (WriteConsoleLine == true)
                Console.WriteLine(string.Format(text, parameters) + (NewLine ? Environment.NewLine : ""));

            if (WriteInLOG == true)
            {
                FileStream fileStream = new FileStream(ServerLog, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                using (StreamWriter Writer = new StreamWriter(fileStream))
                {
                    if (ShowDate == true)
                        Writer.WriteLine(string.Format("[{0}]: ", DateTime.Now.ToString("HH:mm:ss")) + string.Format(text, parameters) + (NewLine ? Environment.NewLine : ""));
                    else
                        Writer.WriteLine(string.Format(text, parameters) + (NewLine ? Environment.NewLine : ""));
                }
            }
        }
    }
}