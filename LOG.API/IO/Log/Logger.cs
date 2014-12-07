using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LOG.API.IO.Log
{
    public static class Log
    {
        private static readonly string FilePath = Path.Combine(Path.Combine(Environment.UserInteractive == false ? Path.Combine(Environment.CurrentDirectory, "L.O.G") : Environment.CurrentDirectory, "logs"), String.Format("log_{0}.log", DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss")));
        private static string Result = String.Empty;

        /// <summary>
        /// Write log to the file. (default write to log file, display message in server or KSP debug console and write new line)
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, params object[] Parameters)
        {
            HandleLog(MessageType, true, true, true, Parameters);
        }

        /// <summary>
        /// Display log if <see cref="toFile"/> is false. (default display message in server or KSP debug console)
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="toFile">If this will be false, then it will won't append in log file.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, bool toFile, params object[] Parameters)
        {
            HandleLog(MessageType, toFile, true, false, Parameters);
        }

        /// <summary>
        ///  Write / Display log in file / server or KSP debug console.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="toFile">If this will be true, then it will append message in log file.</param>
        /// <param name="toConsole">If this will be true, then it will display message in server or KSP debug console.</param>
        /// <param name="newLine">If this will be true, then it will write a new line in server console and log file.</param>
        /// <param name="Parameters">Messages parameters. (unlimited)</param>
        public static void HandleLog(LOGMessageTypes MessageType, bool toFile, bool toConsole, bool newLine, params object[] Parameters)
        {
            Result = String.Empty;

            foreach (object Parameter in Parameters)
                Result += Parameter + " ";

            if (toConsole == true)
            {
                if (Environment.UserInteractive == true) // Check if running program is console or not.
                    ShowConsoleLine(MessageType, newLine);
                else
                    ShowKSPLine(MessageType);
            }

            Directories.CheckDirectory(Path.GetDirectoryName(FilePath));

            if (toFile == true)
                File.AppendAllText(FilePath, String.Format("[{0}][{1}]: {2}", DateTime.Now.ToString("HH:mm:ss"), MessageType.ToString(), Result) + (newLine == true ? Environment.NewLine : String.Empty));
        }

        /// <summary>
        /// Write an empty line to log file and to console.
        /// </summary>
        public static void HandleEmptyMessage()
        {
            if (Environment.UserInteractive == true) // Check if running program is console.
                Console.WriteLine();

            Directories.CheckDirectory(Path.GetDirectoryName(FilePath));

            File.AppendAllText(FilePath, Environment.NewLine);
        }

        /// <summary>
        /// Write text in server console.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        /// <param name="newLine">If this will be true, then it will write a new line in server console and log file.</param>
        private static void ShowConsoleLine(LOGMessageTypes MessageType, bool newLine)
        {
            switch (MessageType)
            {
                case LOGMessageTypes.Debug:
                case LOGMessageTypes.Info:

                    if (newLine == true)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(String.Format("{0}", Result));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(String.Format("{0}", Result));
                        Console.ResetColor();
                    }

                    break;
                case LOGMessageTypes.Warning:

                    if (newLine == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(String.Format("{0}", Result));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(String.Format("{0}", Result));
                        Console.ResetColor();
                    }

                    break;
                case LOGMessageTypes.Error:

                    if (newLine == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(String.Format("{0}", Result));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(String.Format("{0}", Result));
                        Console.ResetColor();
                    }

                    break;
            }
        }

        /// <summary>
        /// Write text in KSP debug console.
        /// </summary>
        /// <param name="MessageType">Log type of current message.</param>
        private static void ShowKSPLine(LOGMessageTypes MessageType)
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
    }
}