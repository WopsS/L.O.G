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
    public class ConnectionApprovalRequestMessage : IGameMessage
    {
         /// <summary>
        /// Class constructor without parameters.
        /// </summary>
        public ConnectionApprovalRequestMessage()
        {

        }

        /// <summary>
        /// Class constructor with parameters and decode message.
        /// </summary>
        /// <param name="Message">Message to be decoded.</param>
        public ConnectionApprovalRequestMessage(NetIncomingMessage Message)
        {
            DecodeMessage(Message);
        }

        /// <summary>
        /// Get or set version of the game.
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// Get or set username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Get message type.
        /// </summary>
        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.ConnectionApprovalRequest; }
        }

        /// <summary>
        /// Decode message.
        /// </summary>
        /// <param name="Message">Message to be decode.</param>
        public void DecodeMessage(NetIncomingMessage Message)
        {
            Version = Message.ReadString();
            Username = Message.ReadString();
        }

        /// <summary>
        /// Encode message.
        /// </summary>
        /// <param name="Message">Message to be encode.</param>
        public void EncodeMessage(NetOutgoingMessage Message)
        {
            Message.Write(Version);
            Message.Write(Username);
        }
    }
}
