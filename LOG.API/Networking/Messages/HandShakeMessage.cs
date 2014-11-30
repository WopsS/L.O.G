using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Messages
{
    public class HandShakeMessage : IGameMessage
    {
        public HandShakeMessage()
        {
        }

        public HandShakeMessage(NetIncomingMessage Message)
        {
            DecodeMessage(Message);
        }

        public string Version { get; set; }

        public string Username { get; set; }

        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.HandShake; }
        }

        public void DecodeMessage(NetIncomingMessage Message)
        {
            Version = Message.ReadString();
            Username = Message.ReadString();
        }

        public void EncodeMessage(NetOutgoingMessage Message)
        {
            Message.Write(Version);
            Message.Write(Username);
        }
    }
}
