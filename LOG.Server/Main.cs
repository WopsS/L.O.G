using Lidgren.Network;
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
                    Network.Shutdown();
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

            Log.HandleLog(Log.LOGMessageTypes.Info, "Server started at", DateTime.Now.ToString("HH:mm:ss") + ".");

            Log.HandleEmptyMessage();
            
            Log.HandleLog(Log.LOGMessageTypes.Info, "-------------------------------------");
            Log.HandleLog(Log.LOGMessageTypes.Info, "Log file \"server_log.txt\" created.");
            LoadCfg();
            Log.HandleLog(Log.LOGMessageTypes.Info, "Log file \"server.cfg\" loaded.");
            Log.HandleLog(Log.LOGMessageTypes.Info, "-------------------------------------");
            Log.HandleEmptyMessage();

            Log.HandleLog(Log.LOGMessageTypes.Info, "L.O.G. Multiplayer Dedicated Server");
            Log.HandleLog(Log.LOGMessageTypes.Info, "-------------------------------------");
            Log.HandleLog(Log.LOGMessageTypes.Info, "Version " + Version);
            Log.HandleEmptyMessage();

            Network.NetworkMain();
        }

        private static void LoadCfg()
        {
            if(!File.Exists(CfgPath))
                File.WriteAllText(CfgPath, "Server Config..." + Environment.NewLine + "maxplayers 30" + Environment.NewLine + "port 4198" + Environment.NewLine + string.Format("hostname L.O.G. {0} Server", Version));

            string[] CfgText = File.ReadAllLines(CfgPath), CurrentKey;

            for (int i = 0; i < CfgText.Length; i++)
            {
                CurrentKey = CfgText[i].Split(new char[] { ' ' }, 2);
                CfgValues.Add(CurrentKey[0], CurrentKey[1]);
            }
        }

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
