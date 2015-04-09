using Lidgren.Network;
using LOG.API;
using LOG.API.IO.Log;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using LOG.API.Networking.Messages.Types;
using LOG.Server.Networking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace LOG.Server.Networking
{
    internal class Network : IDisposable
    {
        public readonly NetServer m_netServer;

        private NetPeerConfiguration m_netPeerConfiguration = new NetPeerConfiguration("LOGServer");
        private IPEndPoint m_masterServer;
        private Thread m_updateThread = null;
        private Dictionary<NetConnection, PlayerModel> m_playersList = new Dictionary<NetConnection, PlayerModel>();
        private bool m_disposed;
        private double m_lastRegistered = -60.0f;

        /// <summary>
        /// Start local network peer.  
        /// </summary>
        public Network()
        {
            m_netPeerConfiguration.MaximumConnections = 32;
            m_netPeerConfiguration.Port = APIMain.ServerPort;
            m_netPeerConfiguration.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            //m_netPeerConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest); Commented since client will get list of the servers from master server.
            m_netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            this.m_masterServer = new IPEndPoint(NetUtility.Resolve(APIMain.MasterServerIP), APIMain.MasterServerPort);

            Log.HandleLog(LOGMessageTypes.Info, true, true, true, "Server configured.");
            Log.HandleEmptyMessage();
            Log.HandleLog(LOGMessageTypes.Info, "Initializing server...");

            m_netServer = new NetServer(m_netPeerConfiguration);
            m_netServer.Start();

            Log.HandleLog(LOGMessageTypes.Info, true, true, true, "Server initialized.");
            Log.HandleEmptyMessage();

            m_updateThread = new Thread(Update);
            m_updateThread.Start();
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
                this.m_updateThread.Abort();
                this.m_netServer.Shutdown("Shutdown!");
            }

            m_disposed = true;
        }

        private void Update()
        {
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                this.SendRegistration();
                this.HandleMessage();

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Handle messages from server or client and choose what to do with them.
        /// </summary>
        private void HandleMessage()
        {
            NetIncomingMessage netIncomingMessage;

            while ((netIncomingMessage = m_netServer.ReadMessage()) != null)
            {
                switch (netIncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
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
                    case NetIncomingMessageType.UnconnectedData:
                        {
                            switch ((ServerMessageTypes)netIncomingMessage.ReadByte())
                            {
                                case ServerMessageTypes.RequestPing:
                                    {
                                        NetOutgoingMessage netOutgoingMessage = this.m_netServer.CreateMessage();

                                        netOutgoingMessage.Write((byte)ServerMessageTypes.RequestPing);
                                        netOutgoingMessage.Write(this.m_netServer.UniqueIdentifier);

                                        this.m_netServer.SendUnconnectedMessage(netOutgoingMessage, netIncomingMessage.SenderEndPoint);
                                        break;
                                    }
                            }

                            break;
                        }
                    /*case NetIncomingMessageType.DiscoveryRequest:
                        {
                            NetOutgoingMessage ResponseMessage = this.m_netServer.CreateMessage();

                            IGameMessage gameMessage = new DiscoveryRequestMessage
                            {
                                Hostname = "L.O.G. Server",
                                Players = this.m_netServer.Connections.Count,
                                MaximumPlayers = 32
                            };

                            gameMessage.EncodeMessage(ResponseMessage);

                            this.m_netServer.SendDiscoveryResponse(ResponseMessage, netIncomingMessage.SenderEndPoint);

                            break;
                        }*/
                    case NetIncomingMessageType.StatusChanged:
                        {
                            switch ((NetConnectionStatus)netIncomingMessage.ReadByte())
                            {
                                case NetConnectionStatus.RespondedConnect:
                                    Log.HandleLog(LOGMessageTypes.Info, "Incoming connection from", netIncomingMessage.SenderEndPoint);
                                    break;
                                case NetConnectionStatus.Connected:
                                    Log.HandleLog(LOGMessageTypes.Info, this.m_playersList[netIncomingMessage.SenderConnection].Username, "has joined the server");
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    Log.HandleLog(LOGMessageTypes.Info, this.m_playersList[netIncomingMessage.SenderConnection].Username, "has left the server");
                                    break;
                            }

                            break;
                        }
                    case NetIncomingMessageType.ConnectionApproval:
                        {
                            GameMessageTypes gameMessageType = (GameMessageTypes)netIncomingMessage.SenderConnection.RemoteHailMessage.ReadByte();

                            if (gameMessageType != GameMessageTypes.ConnectionApprovalRequest)
                                break;

                            ConnectionApprovalRequestMessage Message = new ConnectionApprovalRequestMessage(netIncomingMessage.SenderConnection.RemoteHailMessage);

                            if (Message.Version == APIMain.Version && Message.Username != null && Message.Username.Length > 0)
                            {
                                netIncomingMessage.SenderConnection.Approve(this.m_netServer.CreateMessage());
                                this.m_playersList.Add(netIncomingMessage.SenderConnection, new PlayerModel(netIncomingMessage.SenderConnection, Message.Username));
                            }
                            else
                            {
                                netIncomingMessage.SenderConnection.Deny("Wrong version or username!");
                            }

                            break;
                        }
                    case NetIncomingMessageType.Data:
                        {
                            this.m_playersList[netIncomingMessage.SenderConnection].messageHandler.HandleGameMessage(netIncomingMessage);
                            break;
                        }
                    default:
                        {
                            Log.HandleLog(LOGMessageTypes.Error,
                                String.Format("Unhandled type: {0} {1} bytes {2} |  {3}", netIncomingMessage.MessageType, netIncomingMessage.LengthBytes, netIncomingMessage.DeliveryMethod, 
                                netIncomingMessage.SequenceChannel));
                            break;
                        }
                }

                this.m_netServer.Recycle(netIncomingMessage);
            }
        }

        private void SendRegistration()
        {
            if (NetTime.Now > m_lastRegistered + 30)
            {
                NetOutgoingMessage netOutgoingMessage = this.m_netServer.CreateMessage();

                IPAddress IPAddressMask;
                IPAddress IPaddress = NetUtility.GetMyAddress(out IPAddressMask);

                ServerMessage serverMessage = new ServerMessage
                {
                    ID = this.m_netServer.UniqueIdentifier,
                    IPAddress = IPaddress.ToString(),
                    Port = APIMain.ServerPort,
                    Hostname = "L.O.G. Server",
                    Players = this.m_playersList.Count,
                    MaximumPlayers = 30,
                    Ping = 0,

                    MessageType = ServerMessageTypes.RegisterHost
                };

                serverMessage.EncodeMessage(netOutgoingMessage);
                netOutgoingMessage.Write(new IPEndPoint(IPaddress, APIMain.ServerPort));

                this.m_netServer.SendUnconnectedMessage(netOutgoingMessage, this.m_masterServer);

                m_lastRegistered = (float)NetTime.Now;
            }
        }
    }
}
