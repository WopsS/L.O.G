using LOG.API.IO.Log;
using LOG.Server.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.Server
{
    class ServerMain
    {
        private static Network m_network = null;

        public static Network GetNetworkClass
        {
            get
            {
                return m_network;
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "L.O.G. Multiplayer - Server";

            Log.HandleLog(LOGMessageTypes.Info, "Press ESC to close.");
            Log.HandleLog(LOGMessageTypes.Info, "Server started at", DateTime.Now.ToString("HH:mm:ss") + ".");
            Log.HandleEmptyMessage();

            m_network = new Network();
        }
    }
}
