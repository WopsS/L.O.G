using Lidgren.Network;
using LOG.API.IO.Log;
using LOG.API.Networking;
using LOG.API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.Server.Networking.Managers
{
    class SessionManager
    {
        /// <summary>
        /// Get user connection.
        /// </summary>
        public readonly NetConnection netConnection;

        /// <summary>
        /// Get or set username of the player.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Use message handler for the current user.
        /// </summary>
        public readonly MessageHandler messageHandler = new MessageHandler();

        /// <summary>
        /// Create a new session for user.
        /// </summary>
        /// <param name="Connection">Connection of the new user.</param>
        /// <param name="Username">Username of the new user.</param>
        public SessionManager(NetConnection Connection, string Username)
        {
            this.netConnection = Connection;
            this.Username = Username;

            messageHandler.OnVesselUpdateState += HandleVesselUpdateState;
        }

        public void HandleVesselUpdateState(IGameMessage Message)
        {
            this.SendMessageToOthers(Message);
        }

        /// <summary>
        /// Send message to all connected clients except sender of the message.
        /// </summary>
        /// <param name="Message">Message to be send.</param>
        public void SendMessageToOthers(string Message)
        {
            try
            {
                List<NetConnection> AllConnections = Network.netServer.Connections;
                AllConnections.Remove(this.netConnection);

                if (AllConnections.Count > 0)
                {
                    NetOutgoingMessage netOutgoingMessage = Network.netServer.CreateMessage(Message);
                    Network.netServer.SendMessage(netOutgoingMessage, AllConnections, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
            catch (Exception e)
            {
                Log.HandleEmptyMessage();
                Log.HandleLog(LOGMessageTypes.Error, e.ToString());
                Log.HandleEmptyMessage();
            }
        }

        /// <summary>
        /// Send an IGameMessage message to all connected clients except sender of the message.
        /// </summary>
        /// <param name="Message">Message to be send.</param>
        public void SendMessageToOthers(IGameMessage Message)
        {
            try
            {
                List<NetConnection> AllConnections = Network.netServer.Connections;
                AllConnections.Remove(this.netConnection);

                if (AllConnections.Count > 0)
                {
                    NetOutgoingMessage netOutgoingMessage = Network.netServer.CreateMessage();

                    netOutgoingMessage.Write((byte)Message.MessageType);
                    Message.EncodeMessage(netOutgoingMessage);

                    Network.netServer.SendMessage(netOutgoingMessage, AllConnections, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
            catch (Exception e)
            {
                Log.HandleEmptyMessage();
                Log.HandleLog(LOGMessageTypes.Error, e.ToString());
                Log.HandleEmptyMessage();
            }
        }
    }
}
