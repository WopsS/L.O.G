using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOG.Client
{
    class LOG
    {
        private static string LOGFileName = "client_log.txt", Result = null;

        public enum LOGType : int
        {
            Debug = 0,
            Warning = 1,
            Error = 2,
        }

        public static void ShowLOG(LOGType Type, params object[] Parameters)
        {
            ShowLOG(Type, false, false, Parameters);
        }

        public static void ShowLOG(LOGType Type, bool WriteLOG, params object[] Parameters)
        {
            ShowLOG(Type, WriteLOG, false, Parameters);
        }

        public static void ShowLOG(LOGType Type, bool WriteLOG, bool ShowInConsole, params object[] Parameters)
        {
            Result = String.Empty;

            foreach (object Parameter in Parameters)
                Result += Parameter + " ";

            if (ShowInConsole == true)
            {
                switch (Type)
                {
                    case LOGType.Debug:
                        Debug.Log(String.Format("L.O.G.: {0}", Result));
                        break;
                    case LOGType.Warning:
                        Debug.LogWarning(String.Format("L.O.G. Warning: {0}", Result));
                        break;
                    case LOGType.Error:
                        Debug.LogError(String.Format("L.O.G. Error: {0}", Result));
                        break;
                }
            }

            if (WriteLOG == true)
                File.AppendAllText(LOGFileName, String.Format("[{0}][{1}]: {2}", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"), Type.ToString(), Result) + Environment.NewLine);
        }
    }
}
