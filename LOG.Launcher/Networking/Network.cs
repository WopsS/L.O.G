using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using LOG.API;
using LOG.API.Networking.Messages.Types;
using System.Threading;
using System.Windows.Forms;
using LOG.API.Networking.Messages;
using LOG.API.Networking.Interfaces;

namespace LOG.MasterServer.Networking
{
    internal class Network
    {
        public NetClient m_netClient;

        public Dictionary<long, ServerMessage> GetServersList
        {
            get
            {
                return this.m_serverInfo;
            }
        }

        private NetPeerConfiguration m_netConfiguration;
        private IPEndPoint m_masterServer;
        private Dictionary<long, ServerMessage> m_serverInfo = new Dictionary<long, ServerMessage>();
        private Thread m_updateThread = null;
        private bool m_disposed;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public Network()
        {
            this.m_netConfiguration = new NetPeerConfiguration("LOGMultiplayerLauncher");
            this.m_netConfiguration.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            this.m_netClient = new NetClient(m_netConfiguration);

            this.m_masterServer = new IPEndPoint(NetUtility.Resolve(APIMain.MasterServerIP), APIMain.MasterServerPort);
            
            this.m_netClient.Start();

            this.m_updateThread = new Thread(Update);
            this.m_updateThread.Start();

            this.RequestServersList();
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
                this.m_netClient.Shutdown("Request by user.");
                m_updateThread.Abort();
            }

            m_disposed = true;
        }

        private void Update()
        {
            while (this.m_netClient.Status == NetPeerStatus.Running && this.m_updateThread.ThreadState == ThreadState.Running)
            {
                NetIncomingMessage netIncomingMessage;

                while ((netIncomingMessage = this.m_netClient.ReadMessage()) != null)
                {
                    switch (netIncomingMessage.MessageType)
                    {
                        //case NetIncomingMessageType.VerboseDebugMessage:
                        //case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            {
                                MessageBox.Show(netIncomingMessage.ReadString());
                                break;
                            }
                        case NetIncomingMessageType.UnconnectedData:
                            {
                                switch ((ServerMessageTypes)netIncomingMessage.ReadByte())
                                {
                                    case ServerMessageTypes.RequestHostList:
                                        {
                                            ServerMessage serverMessage = new ServerMessage();
                                            serverMessage.DecodeMessage(netIncomingMessage);

                                            if (this.m_serverInfo.ContainsKey(serverMessage.ID) == false)
                                            {
                                                this.m_serverInfo.Add(serverMessage.ID, serverMessage);
                                            }
                                            else
                                            {
                                                this.m_serverInfo[serverMessage.ID].Hostname = serverMessage.Hostname;
                                                this.m_serverInfo[serverMessage.ID].Players = serverMessage.Players;
                                                this.m_serverInfo[serverMessage.ID].MaximumPlayers = serverMessage.MaximumPlayers;
                                                this.m_serverInfo[serverMessage.ID].LastMessageSendTime = serverMessage.LastMessageSendTime;
                                            }

                                            break;
                                        }
                                    case ServerMessageTypes.RequestPing:
                                        {
                                            long ServerID = netIncomingMessage.ReadInt64();

                                            this.m_serverInfo[ServerID].Ping = Convert.ToInt32((0.25 * this.m_serverInfo[ServerID].Ping) + ((1 - 0.25) * (netIncomingMessage.ReceiveTime - this.m_serverInfo[ServerID].LastMessageSendTime)));
                                            break;
                                        }
                                }
                                break;
                            }
                    }

                    this.m_netClient.Recycle(netIncomingMessage);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Request servers list from master server.
        /// </summary>
        public void RequestServersList()
        {
            NetOutgoingMessage netOutgoingMessage = this.m_netClient.CreateMessage();
            netOutgoingMessage.Write((byte)ServerMessageTypes.RequestHostList);
            this.m_netClient.SendUnconnectedMessage(netOutgoingMessage, m_masterServer);
        }

        /// <summary>
        /// Send unconnected message to servers and calculate ping for them.
        /// </summary>
        public void CalculatePing()
        {
            NetOutgoingMessage netOutgoingMessage = this.m_netClient.CreateMessage();
            netOutgoingMessage.Write((byte)ServerMessageTypes.RequestPing);

            foreach (KeyValuePair<long, ServerMessage> serverInfo in this.m_serverInfo)
            {
                serverInfo.Value.LastMessageSendTime = NetTime.Now;
                this.m_netClient.SendUnconnectedMessage(netOutgoingMessage, NetUtility.Resolve(serverInfo.Value.IPAddress, serverInfo.Value.Port));
            }
        }
    }
}
