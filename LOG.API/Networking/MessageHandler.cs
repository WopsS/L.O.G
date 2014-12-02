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
        public delegate void PacketHandlerDelegate(IGameMessage message);

        public event PacketHandlerDelegate OnVesselUpdateState;

        /// <summary>
        /// Handle message and choose what to do with it.
        /// </summary>
        /// <param name="Message">Message to handle.</param>
        public void Handle(NetIncomingMessage Message)
        {
            GameMessageTypes gameMessage = (GameMessageTypes)Message.ReadByte();

            switch (gameMessage)
            {
                case GameMessageTypes.UpdateVesseState:
                    OnVesselUpdateState(new VesselMessage(Message));
                    break;
            }
        }
    }
}
