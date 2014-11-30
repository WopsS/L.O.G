using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.Launcher
{
    class ServerInfo
    {
        public static List<ServerInfo> ServerDetail = new List<ServerInfo>();

        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public int Ping { get; set; }
    }
}
