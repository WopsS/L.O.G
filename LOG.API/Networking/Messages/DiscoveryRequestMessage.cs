using Lidgren.Network;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOG.API.Networking.Messages
{
    public class DiscoveryRequestMessage : IGameMessage
    {
        /// <summary>
        /// Class constructor without parameters.
        /// </summary>
        public DiscoveryRequestMessage()
        {

        }

        /// <summary>
        /// Class constructor with parameters and decode message.
        /// </summary>
        /// <param name="Message">Message to be decoded.</param>
        public DiscoveryRequestMessage(NetIncomingMessage Message)
        {
            DecodeMessage(Message);
        }

        /// <summary>
        /// Get or set hostname received from the server.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Get or set players number received from the server.
        /// </summary>
        public int Players { get; set; }

        /// <summary>
        /// Get or set maximum players received from the server.
        /// </summary>
        public int MaximumPlayers { get; set; }

        /// <summary>
        /// Get or set ping calculated by client or server.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Get message type.
        /// </summary>
        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.DiscoveryState; }
        }

        /// <summary>
        /// Decode message.
        /// </summary>
        /// <param name="Message">Message to be decode.</param>
        public void DecodeMessage(NetIncomingMessage Message)
        {
            Hostname = Message.ReadString();
            Players = Message.ReadInt32();
            MaximumPlayers = Message.ReadInt32();
        }

        /// <summary>
        /// Encode message.
        /// </summary>
        /// <param name="Message">Message to be encode.</param>
        public void EncodeMessage(NetOutgoingMessage Message)
        {
            Message.Write(Hostname);
            Message.Write(Players);
            Message.Write(MaximumPlayers);
        }
    }
}
