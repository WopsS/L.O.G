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
        private static Network network = null;

        static void Main(string[] args)
        {
            Console.Title = "L.O.G. Multiplayer - Server";

            Log.HandleLog(LOGMessageTypes.Info, "Press ESC to close.");
            Log.HandleLog(LOGMessageTypes.Info, "Server started at", DateTime.Now.ToString("HH:mm:ss") + ".");
            Log.HandleEmptyMessage();

            network = new Network();
        }
    }
}
