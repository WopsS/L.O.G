using Lidgren.Network;
using LOG.API.IO.Log;
using LOG.Server.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LOG.Server
{
    public class ServerMain
    {
        #region Close Event

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    Network.netServer.Shutdown("Requested by user.");
                    return false;
            }
        }

        #endregion

        static string CfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.cfg");
        public static Dictionary<string, string> CfgValues = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            #region Close Event
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);
            #endregion

            Console.Title = "L.O.G. Multiplayer - Server";

            if (Directory.Exists(Path.GetDirectoryName(Log.FilePath)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(Log.FilePath));

            Log.HandleLog(LOGMessageTypes.Info, "Server started at", DateTime.Now.ToString("HH:mm:ss") + ".");

            Log.HandleEmptyMessage();
            
            Log.HandleLog(LOGMessageTypes.Info, "-------------------------------------");
            Log.HandleLog(LOGMessageTypes.Info, "Log file \"server_log.txt\" created.");
            LoadCfg();
            Log.HandleLog(LOGMessageTypes.Info, "Config file \"server.cfg\" loaded.");
            Log.HandleLog(LOGMessageTypes.Info, "-------------------------------------");
            Log.HandleEmptyMessage();

            Log.HandleLog(LOGMessageTypes.Info, "L.O.G. Multiplayer Dedicated Server");
            Log.HandleLog(LOGMessageTypes.Info, "-------------------------------------");
            Log.HandleLog(LOGMessageTypes.Info, "Version " + Version);
            Log.HandleEmptyMessage();

            Network.Initialize();
        }

        /// <summary>
        /// Load Cfg file.
        /// </summary>
        private static void LoadCfg()
        {
            if (!File.Exists(CfgPath))
                CreateCfg();

            string[] CfgText = File.ReadAllLines(CfgPath), CurrentKey;

            for (int i = 0; i < CfgText.Length; i++)
            {
                if (CfgText[i].Length == 0)
                    continue;

                CurrentKey = CfgText[i].Split(new char[] { ' ' }, 2);
                CfgValues.Add(CurrentKey[0], CurrentKey[1]);
            }

            if (!CfgValues.ContainsKey("maxplayers") || !CfgValues.ContainsKey("port") || !CfgValues.ContainsKey("hostname"))
            {
                Log.HandleLog(LOGMessageTypes.Info, "Config file \"server.cfg\" isn't correct, create a new one.");
                CreateCfg();
                CfgValues.Clear();
                LoadCfg();
            }
        }

        /// <summary>
        /// Create default cfg file.
        /// </summary>
        private static void CreateCfg()
        {
            string[] DefaultConfig = { "Server Config...", "", "maxplayers 100", "port 4198", string.Format("hostname L.O.G. {0} Server", Version) };
            File.WriteAllLines(CfgPath, DefaultConfig);
        }

        /// <summary>
        /// Get current version of server.
        /// </summary>
        public static string Version
        {
            get
            {
                FileVersionInfo Program = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return string.Format("{0}.{1}", Program.ProductMajorPart, Program.ProductMinorPart) + string.Format("{0}", (Program.ProductBuildPart != 0) ? "." + Program.ProductBuildPart.ToString() : "");
            }
        }
    }
}
