using LOG.API.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Models
{
    public class PlayerModel
    {
        public string Username { get; set; }

        public bool IsConnected { get; set; }

        public bool GameStarted { get; set; }

        public readonly MessageHandler messageHandler = new MessageHandler();
    }
}
