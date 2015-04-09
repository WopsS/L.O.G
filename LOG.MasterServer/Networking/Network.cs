using Lidgren.Network;
using LOG.API;
using LOG.API.IO.Log;
using LOG.API.Networking;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using LOG.API.Networking.Messages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace LOG.MasterServer.Networking
{
    internal class Network : IDisposable
    {
        public NetPeer m_netPeer;
        private NetPeerConfiguration m_netPeerConfiguration = new NetPeerConfiguration("LOGMasterServer");
        private Thread m_updateThread = null, m_updateServersListThread = null;
        private bool m_disposed;

        /// <summary>
        /// Keep registred servers and informations about them.
        /// </summary>
        private Dictionary<long, ServerMessage> m_registredServers = new Dictionary<long, ServerMessage>();

        /// <summary>
        /// Start local network peer.  
        /// </summary>
        public Network()
        {
            m_netPeerConfiguration.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            m_netPeerConfiguration.Port = APIMain.MasterServerPort;

            m_netPeer = new NetPeer(m_netPeerConfiguration);
            m_netPeer.Start();

            m_updateThread = new Thread(Update);
            m_updateThread.Start();

            m_updateServersListThread = new Thread(UpdateServerList);
            m_updateServersListThread.Start();
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
                this.m_netPeer.Shutdown("Shutdown!");
            }

            m_disposed = true;
        }

        private void Update()
        {
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                this.HandleMessage();

                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Handle messages from server or client and choose what to do with them.
        /// </summary>
        private void HandleMessage()
        {
            NetIncomingMessage netIncomingMessage;

            while ((netIncomingMessage = m_netPeer.ReadMessage()) != null)
            {
                switch (netIncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.UnconnectedData:
                        {
                            switch ((ServerMessageTypes)netIncomingMessage.ReadByte())
                            {
                                case ServerMessageTypes.RegisterHost:
                                    {
                                        ServerMessage serverMessage = new ServerMessage(netIncomingMessage); // Decode server message.

                                        serverMessage.IPendPoint = new IPEndPoint[]
                                        {
                                            netIncomingMessage.ReadIPEndPoint(),
										    netIncomingMessage.SenderEndPoint
                                        };

                                        if (m_registredServers.ContainsKey(serverMessage.ID) == false)
                                        {
                                            m_registredServers.Add(serverMessage.ID, serverMessage);
                                        }
                                        else
                                        {
                                            m_registredServers[serverMessage.ID] = serverMessage;
                                        }

                                        Log.HandleLog(LOGMessageTypes.Info, "Got registration for host", String.Format("{0}.", serverMessage.ID));
                                        break;
                                    }
                                case ServerMessageTypes.RequestHostList:
                                    {
                                        Log.HandleLog(LOGMessageTypes.Info, "Sending list of", m_registredServers.Count, "hosts to client", String.Format("{0}.", netIncomingMessage.SenderEndPoint));

                                        foreach (KeyValuePair<long, ServerMessage> RegistredServer in this.m_registredServers)
                                        {
                                            NetOutgoingMessage netOutgoingMessage = m_netPeer.CreateMessage();

                                            IServerMessage IserverMessage = new ServerMessage
                                            {
                                                ID = RegistredServer.Value.ID,
                                                IPAddress = RegistredServer.Value.IPAddress,
                                                Port = RegistredServer.Value.Port,
                                                Hostname = RegistredServer.Value.Hostname,
                                                Players = RegistredServer.Value.Players,
                                                MaximumPlayers = RegistredServer.Value.MaximumPlayers,

                                                Ping = 0,
                                                IPendPoint = null,
                                                MessageType = ServerMessageTypes.RequestHostList
                                            };

                                            IserverMessage.EncodeMessage(netOutgoingMessage); // Encode message.

                                            m_netPeer.SendUnconnectedMessage(netOutgoingMessage, netIncomingMessage.SenderEndPoint); // Send encoded message to client.
                                        }
                                        break;
                                    }
                                case ServerMessageTypes.RequestIntroduction:
                                    {
                                        IPEndPoint clientInternal = netIncomingMessage.ReadIPEndPoint();
                                        long ServerID = netIncomingMessage.ReadInt64();
                                        string Token = netIncomingMessage.ReadString();

                                        Log.HandleLog(LOGMessageTypes.Info, netIncomingMessage.SenderEndPoint, "requesting introduction to", ServerID, String.Format("(token{0}).", Token));

                                        if (this.m_registredServers.ContainsKey(ServerID))
                                        {
                                            Log.HandleLog(LOGMessageTypes.Info, "Sending introduction...");

                                            m_netPeer.Introduce(this.m_registredServers[ServerID].IPendPoint[0], this.m_registredServers[ServerID].IPendPoint[1], clientInternal, 
                                                netIncomingMessage.SenderEndPoint, Token);
                                        }
                                        else
                                            Console.WriteLine("Client requested introduction to nonlisted host!");
                                        break;
                                    }
                            }

                            break;
                        }
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
                }
            }
        }

        /// <summary>
        /// Update the server list in another thread because if there will be to much servers online it will take sometime to check all servers. 
        /// </summary>
        private void UpdateServerList()
        {
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                foreach (KeyValuePair<long, ServerMessage> RegistredServer in m_registredServers.ToList())
                {
                    if (RegistredServer.Value.LastRegistredTime < NetTime.Now - 120)
                    {
                        m_registredServers.Remove(RegistredServer.Key);
                    }
                }

                Thread.Sleep(120);
            }
        }
    }
}
