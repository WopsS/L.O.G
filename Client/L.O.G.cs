using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Client
{
    class LOG
    {
        private static string LOGFileName = "client_log.txt";
        public enum LOGType : int
        {
            Debug = 0,
            Warning = 1,
            Error = 2,
        }

        public static void ShowLOG(LOGType type, string text, params object[] parameters)
        {
            ShowLOG(type, false, text, parameters);
        }

        public static void ShowLOG(LOGType type, bool WriteLOG, string text, params object[] parameters)
        {
            switch (type)
            {
                case LOGType.Debug:
                    Debug.Log("L.O.G.: " + string.Format(text, parameters));
                    break;
                case LOGType.Warning:
                    Debug.LogWarning("L.O.G. Warning: " + string.Format(text, parameters));
                    break;
                case LOGType.Error:
                    Debug.LogError("L.O.G. Error: " + string.Format(text, parameters));
                    break;
            }

            if (WriteLOG == true)
            {
                FileStream fileStream = new FileStream(LOGFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine(string.Format("[{0}]: ", DateTime.Now.ToString("dd/MM/yy HH:mm:ss")) + string.Format(text, parameters));
                }
            }
        }
    }
}
