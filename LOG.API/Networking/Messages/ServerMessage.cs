using Lidgren.Network;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LOG.API.Networking.Messages
{
    public class ServerMessage : IServerMessage
    {
        /// <summary>
        /// Class constructor without parameters.
        /// </summary>
        public ServerMessage()
        {

        }

        /// <summary>
        /// Class constructor with parameters and decode message.
        /// </summary>
        /// <param name="netIncomingMessage">Message to be decoded.</param>
        public ServerMessage(NetIncomingMessage netIncomingMessage)
        {
            DecodeMessage(netIncomingMessage);
        }

        /// <summary>
        /// Get or set server IP address.
        /// </summary>
        public long ID { get; set; }

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

        /// <summary>
        /// Get or set server endpoint.
        /// NOTE: This never will be sent in an encoded message!
        /// </summary>
        public IPEndPoint[]  IPendPoint  { get; set; }

        /// <summary>
        /// Get or set message type.
        /// </summary>
        public ServerMessageTypes MessageType { get; set; }

        /// <summary>
        /// Decode message.
        /// </summary>
        /// <param name="netIncomingMessage">Message to be decode.</param>
        public void DecodeMessage(NetIncomingMessage netIncomingMessage)
        {
            ID = netIncomingMessage.ReadInt64();
            IPAddress = netIncomingMessage.ReadString();
            Port = netIncomingMessage.ReadInt32();
            Hostname = netIncomingMessage.ReadString();
            Players = netIncomingMessage.ReadInt32();
            MaximumPlayers = netIncomingMessage.ReadInt32();
            Ping = netIncomingMessage.ReadInt32();
        }

        /// <summary>
        /// Encode message.
        /// </summary>
        /// <param name="netOutgoingMessage">Message to be encode.</param>
        public void EncodeMessage(NetOutgoingMessage netOutgoingMessage)
        {
            netOutgoingMessage.Write((byte)MessageType);

            netOutgoingMessage.Write(ID);
            netOutgoingMessage.Write(IPAddress);
            netOutgoingMessage.Write(Port);
            netOutgoingMessage.Write(Hostname);
            netOutgoingMessage.Write(Players);
            netOutgoingMessage.Write(MaximumPlayers);
            netOutgoingMessage.Write(Ping);
        }
    }
}
