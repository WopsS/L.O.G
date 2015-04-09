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
    class VesselMessage : IGameMessage
    {
        /// <summary>
        /// Class constructor without parameters.
        /// </summary>
        public VesselMessage()
        {

        }

        /// <summary>
        /// Class constructor with parameters and decode message.
        /// </summary>
        /// <param name="Message">Message to be decoded.</param>
        public VesselMessage(NetIncomingMessage Message)
        {
            DecodeMessage(Message);
        }

        /// <summary>
        /// Get or set ID of the vessel.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Get or set name of the vessel.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set data about vessel.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Get message type.
        /// </summary>
        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.UpdateVesseState; }
        }

        /// <summary>
        /// Decode message.
        /// </summary>
        /// <param name="Message">Message to be decode.</param>
        public void DecodeMessage(NetIncomingMessage Message)
        {
            ID = Message.ReadString();
            Name = Message.ReadString();
            Data = Message.ReadString();
        }

        /// <summary>
        /// Encode message.
        /// </summary>
        /// <param name="Message">Message to be encode.</param>
        public void EncodeMessage(NetOutgoingMessage Message)
        {
            Message.Write(ID);
            Message.Write(Name);
            Message.Write(Data);
        }
    }
}
