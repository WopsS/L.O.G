using LOG.API.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.Client.Networking.Managers
{
    class PlayerManager
    {
        public static string Username { get; set; }

        public static bool isConnected { get; set; }

        public static bool GameStarted { get; set; }

        public static readonly MessageHandler messageHandler = new MessageHandler();
    }
}
