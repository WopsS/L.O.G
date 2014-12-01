using Lidgren.Network;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking
{
    public class MessageHandler
    {
        public delegate void PacketHandlerDelegate(IGameMessage message, NetConnection Connection = null);

        public event PacketHandlerDelegate OnVesselUpdateState;

        /// <summary>
        /// Handle message and choose what to do with it.
        /// </summary>
        /// <param name="Message">Message to handle.</param>
        public void Handle(NetIncomingMessage Message, NetConnection Connection = null)
        {
            GameMessageTypes gameMessage = (GameMessageTypes)Message.ReadByte();

            switch (gameMessage)
            {
                case GameMessageTypes.UpdateVesseState:
                    OnVesselUpdateState(new VesselMessage(Message), Connection);
                    break;
            }
        }
    }
}
