using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.Launcher
{
    class ServerInfo
    {
        public static List<ServerInfo> ServerDetail = new List<ServerInfo>();

        /// <summary>
        /// Get or set server IP address.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Get or set server IP address.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Get or set server hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Get or set number of players on server.
        /// </summary>
        public int Players { get; set; }

        /// <summary>
        /// Get or set maximum number of players on server.
        /// </summary>
        public int MaximumPlayers { get; set; }

        /// <summary>
        /// Get or set calculated ping.
        /// </summary>
        public int Ping { get; set; }
    }
}
