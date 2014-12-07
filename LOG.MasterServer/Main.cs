using LOG.API.IO.Log;
using LOG.MasterServer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LOG.MasterServer
{
    internal class MasterServerMain
    {
        private static Network network = null;

        static void Main(string[] args)
        {
            Console.Title = "L.O.G. Multiplayer - MasterServer";

            Log.HandleLog(LOGMessageTypes.Info, "Press ESC to close.");
            Log.HandleLog(LOGMessageTypes.Info, "MasterServer started at", DateTime.Now.ToString("HH:mm:ss") + ".");
            Log.HandleEmptyMessage();

            network = new Network();
        }
    }
}
