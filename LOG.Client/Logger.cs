using LOG.API.IO.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOG.Client
{
    class Log
    {
        public static readonly string FilePath = Path.Combine(Path.Combine(Path.Combine(Environment.CurrentDirectory, "L.O.G"), "logs"), String.Format("log_{0}.log", DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss")));
        private static string Result = String.Empty;

        /// <summary>
        /// Write log to the file.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, params object[] Parameters)
        {
            HandleLog(MessageType, true, true, Parameters);
        }

        /// <summary>
        /// Display log if <see cref="toFile"/> is false.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="toFile">If this will be false, then it will won't append in log file.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, bool toFile, params object[] Parameters)
        {
            HandleLog(MessageType, toFile, false, Parameters);
        }

        /// <summary>
        ///  Write / Display log in file / debug console.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="toFile">If this will be true, then it will append message in log file.</param>
        /// <param name="toConsole">If this will be true, then it will display message in debug console.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, bool toFile, bool toConsole, params object[] Parameters)
        {
            Result = String.Empty;

            foreach (object Parameter in Parameters)
                Result += Parameter.ToString() + " ";

            if (toConsole == true)
            {
                switch (MessageType)
                {
                    case LOGMessageTypes.Debug:
                    case LOGMessageTypes.Info:
                        Debug.Log(String.Format("L.O.G.: {0}", Result));
                        break;
                    case LOGMessageTypes.Warning:
                        Debug.LogWarning(String.Format("L.O.G. Warning: {0}", Result));
                        break;
                    case LOGMessageTypes.Error:
                        Debug.LogError(String.Format("L.O.G. Error: {0}", Result));
                        break;
                }
            }

            if (toFile == true)
                File.AppendAllText(FilePath, String.Format("[{0}][{1}]: {2}", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"), MessageType.ToString(), Result) + Environment.NewLine);
        }

        /// <summary>
        /// Write an empty line to log file.
        /// </summary>
        public static void HandleEmptyMessage()
        {
            File.AppendAllText(FilePath, Environment.NewLine);
        }
    }
}
