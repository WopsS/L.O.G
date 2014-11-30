using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOG.Server
{
    class Log
    {
        public static string FilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, "logs"), String.Format("log_{0}.log", DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss")));
        private static string Result = String.Empty;

        public enum LOGMessageTypes : int
        {
            Info = 0,
            Debug = 1,
            Warning = 2,
            Error = 3
        }

        /// <summary>
        /// Write log to the file. (default write to log file, display message in server console and write new line)
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, params object[] Parameters)
        {
            HandleLog(MessageType, true, true, true, Parameters);
        }

        /// <summary>
        /// Display log if <see cref="toFile"/> is false. (default display message in server console)
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="toFile">If this will be false, then it will won't append in log file.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, bool toFile, params object[] Parameters)
        {
            HandleLog(MessageType, toFile, true, false, Parameters);
        }

        /// <summary>
        ///  Write / Display log in file / server console.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="toFile">If this will be true, then it will append message in log file.</param>
        /// <param name="toConsole">If this will be true, then it will display message in server console.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, bool toFile, bool toConsole, bool newLine, params object[] Parameters)
        {
            Result = String.Empty;

            foreach (object Parameter in Parameters)
                Result += Parameter + " ";

            if (toConsole == true)
            {
                switch (MessageType)
                {
                    case LOGMessageTypes.Debug:
                    case LOGMessageTypes.Info:

                        if (newLine == true)
                            Console.WriteLine(String.Format("{0}", Result));
                        else
                            Console.Write(String.Format("{0}", Result));

                        break;
                    case LOGMessageTypes.Warning:

                        if (newLine == true)
                            Console.WriteLine(String.Format("{0}", Result));
                        else
                            Console.Write(String.Format("{0}", Result));

                        break;
                    case LOGMessageTypes.Error:

                        if (newLine == true)
                            Console.WriteLine(String.Format("{0}", Result));
                        else
                            Console.Write(String.Format("{0}", Result));

                        break;
                }
            }

            if (toFile == true)
                File.AppendAllText(FilePath, String.Format("[{0}][{1}]: {2}", DateTime.Now.ToString("HH:mm:ss"), MessageType.ToString(), Result) + (newLine == true ? Environment.NewLine : String.Empty));
        }


        public static void HandleEmptyMessage()
        {
            Console.WriteLine();

            File.AppendAllText(FilePath, Environment.NewLine);
        }
    }
}