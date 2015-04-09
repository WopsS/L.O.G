using Lidgren.Network;
using LOG.API;
using LOG.API.IO.Log;
using LOG.API.Models;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using LOG.Client.Networking.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOG.MasterServer.Networking
{
    internal class ClientNetwork : IDisposable
    {
        public NetClient m_netClient;

        private NetPeerConfiguration m_netConfiguration;
        private PlayerModel m_playerModel = null;
        private VesselManager m_vesselManager = null;
        private bool m_disposed;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="aPlayerModel">PlayerModel class stored in this class from ClientMain.</param>
        /// <param name="ServerIP">Server IP to connect on it.</param>
        /// <param name="ServerPort">Server port to connect on it.</param>
        public ClientNetwork(PlayerModel aPlayerModel, string ServerIP, int ServerPort)
        {
            this.m_netConfiguration = new NetPeerConfiguration("LOGMultiplayer");
            this.m_netClient = new NetClient(m_netConfiguration);

            this.m_netClient.Start(); // Start netClient.

            this.m_playerModel = aPlayerModel;

            NetOutgoingMessage netOutgoingMessage = this.m_netClient.CreateMessage();

            IGameMessage gameMessage = new ConnectionApprovalRequestMessage
            {
                Version = APIMain.Version,
                Username = this.m_playerModel.Username
            };

            netOutgoingMessage.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMessage(netOutgoingMessage);

            this.m_netClient.Connect(ServerIP, Convert.ToInt32(ServerPort), netOutgoingMessage);

            this.m_vesselManager = new VesselManager(this.m_netClient);
            this.m_playerModel.messageHandler.OnVesselUpdateState += this.m_vesselManager.HandleVesselMessage;
        }

        /// <summary>
        /// Cleanup network statements.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleanup network statements.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                this.m_playerModel.messageHandler.OnVesselUpdateState -= this.m_vesselManager.HandleVesselMessage;
                this.m_netClient.Shutdown("Request by user.");
            }

            m_disposed = true;
        }

        /// <summary>
        /// Update current instance of network class.
        /// </summary>
        public void Update()
        {
            if (this.m_netClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage netOutgoingMessage = this.m_vesselManager.CreateVesselMessage();

                if (netOutgoingMessage != null)
                {
                    this.SendMessage(netOutgoingMessage);
                }
            }

            NetIncomingMessage netIncomingMessage;

            while ((netIncomingMessage = this.m_netClient.ReadMessage()) != null)
            {
                switch (netIncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                        {
                            Log.HandleLog(LOGMessageTypes.Debug, netIncomingMessage.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.WarningMessage:
                        {
                            Log.HandleLog(LOGMessageTypes.Warning, netIncomingMessage.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.ErrorMessage:
                        {
                            Log.HandleLog(LOGMessageTypes.Error, netIncomingMessage.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus Status = (NetConnectionStatus)netIncomingMessage.ReadByte();

                            switch (Status)
                            {
                                case NetConnectionStatus.Connected:
                                    {
                                        Log.HandleLog(LOGMessageTypes.Info, "Connected to server.");
                                        this.m_playerModel.IsConnected = true;
                                    }
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    {
                                        Log.HandleLog(LOGMessageTypes.Info, "Disconnected from server.");
                                        Application.Quit();
                                    }
                                    break;
                                case NetConnectionStatus.Disconnecting:
                                case NetConnectionStatus.InitiatedConnect:
                                case NetConnectionStatus.RespondedConnect:
                                    {
                                        Log.HandleLog(LOGMessageTypes.Info, Status.ToString());
                                    }
                                    break;
                            }
                            break;
                        }
                    case NetIncomingMessageType.Data:
                        {
                            this.m_playerModel.messageHandler.HandleGameMessage(netIncomingMessage);
                            break;
                        }
                    default:
                        {
                            Log.HandleLog(LOGMessageTypes.Error, "Unhandled type: " + netIncomingMessage.MessageType + " " + netIncomingMessage.LengthBytes + " bytes");
                            break;
                        }
                }

                this.m_netClient.Recycle(netIncomingMessage);
            }
        }

        /// <summary>
        /// Send unordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public void SendMessage(string Message)
        {
            NetOutgoingMessage OutgoingMessage = this.m_netClient.CreateMessage(Message);
            this.m_netClient.SendMessage(OutgoingMessage, NetDeliveryMethod.ReliableUnordered);
        }

        /// <summary>
        /// Send unordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public void SendMessage(NetOutgoingMessage Message)
        {
            this.m_netClient.SendMessage(Message, NetDeliveryMethod.ReliableUnordered);
        }

        /// <summary>
        /// Send ordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public void SendMessageOrdered(string Message)
        {
            NetOutgoingMessage OutgoingMessage = this.m_netClient.CreateMessage(Message);
            this.m_netClient.SendMessage(OutgoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Send ordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public void SendMessageOrdered(NetOutgoingMessage Message)
        {
            this.m_netClient.SendMessage(Message, NetDeliveryMethod.ReliableOrdered);
        }
    }
}