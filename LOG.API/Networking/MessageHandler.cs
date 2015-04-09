using Lidgren.Network;
using LOG.API.IO.Log;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using LOG.API.Networking.Messages.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LOG.API.Networking
{
    public class MessageHandler
    {
        public delegate void GameMessageHandlerDelegate(IGameMessage aGameMessage);

        public event GameMessageHandlerDelegate OnVesselUpdateState;

        /// <summary>
        /// Handle game message and choose what to do with it.
        /// </summary>
        /// <param name="Message">Message to handle.</param>
        public void HandleGameMessage(NetIncomingMessage Message)
        {
            GameMessageTypes gameMessage = (GameMessageTypes)Message.ReadByte();

            switch (gameMessage)
            {
                case GameMessageTypes.UpdateVesseState:
                    {
                        this.OnVesselUpdateState(new VesselMessage(Message));
                        break;
                    }
                default:
                    {
                        Log.HandleLog(LOGMessageTypes.Error, "Unhandled game message type " + gameMessage);
                        break;
                    }
            }
        }

    }
}
