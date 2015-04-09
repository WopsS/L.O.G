using Lidgren.Network;
using LOG.API.IO.Log;
using LOG.API.Networking;
using LOG.API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOG.Server.Networking.Models
{
    class PlayerModel
    {
        /// <summary>
        /// Get user connection.
        /// </summary>
        public readonly NetConnection netConnection;

        /// <summary>
        /// Get username of the player.
        /// </summary>
        public readonly string Username = String.Empty;

        /// <summary>
        /// Use message handler for the current user.
        /// </summary>
        public readonly MessageHandler messageHandler = new MessageHandler();

        /// <summary>
        /// Store network class.
        /// </summary>
        private Network m_networkClass = ServerMain.GetNetworkClass;

        /// <summary>
        /// Create a new session for user.
        /// </summary>
        /// <param name="Connection">Connection of the new user.</param>
        /// <param name="Username">Username of the new user.</param>
        public PlayerModel(NetConnection Connection, string Username)
        {
            this.netConnection = Connection;
            this.Username = Username;

            messageHandler.OnVesselUpdateState += HandleVesselUpdateState;
        }

        public void HandleVesselUpdateState(IGameMessage Message)
        {
            this.SendMessage(Message);
        }

        /// <summary>
        /// Send message to all connected clients except sender of the message.
        /// </summary>
        /// <param name="Message">Message to be send.</param>
        public void SendMessage(string Message)
        {
            try
            {
                List<NetConnection> AllConnections = this.m_networkClass.m_netServer.Connections;
                AllConnections.Remove(this.netConnection);

                if (AllConnections.Count > 0)
                {
                    NetOutgoingMessage netOutgoingMessage = this.m_networkClass.m_netServer.CreateMessage(Message);
                    this.m_networkClass.m_netServer.SendMessage(netOutgoingMessage, AllConnections, NetDeliveryMethod.ReliableOrdered, 0);
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
        public void SendMessage(IGameMessage Message)
        {
            try
            {
                List<NetConnection> AllConnections = this.m_networkClass.m_netServer.Connections;
                AllConnections.Remove(this.netConnection);

                if (AllConnections.Count > 0)
                {
                    NetOutgoingMessage netOutgoingMessage = this.m_networkClass.m_netServer.CreateMessage();

                    netOutgoingMessage.Write((byte)Message.MessageType);
                    Message.EncodeMessage(netOutgoingMessage);

                    this.m_networkClass.m_netServer.SendMessage(netOutgoingMessage, AllConnections, NetDeliveryMethod.ReliableOrdered, 0);
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
