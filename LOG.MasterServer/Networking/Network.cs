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
    internal class Network
    {
        public NetPeer netPeer;
        private NetPeerConfiguration netPeerConfiguration = new NetPeerConfiguration("LOGMasterServer");
        private Thread UpdateThread = null;

        /// <summary>
        /// Keep registred servers and informations about them.
        /// </summary>
        private Dictionary<long, ServerMessage> RegistredServers = new Dictionary<long, ServerMessage>();

        /// <summary>
        /// Start local network peer.  
        /// </summary>
        public Network()
        {
            netPeerConfiguration.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            netPeerConfiguration.Port = APIMain.MasterServerPort;

            netPeer = new NetPeer(netPeerConfiguration);
            netPeer.Start();

            UpdateThread = new Thread(HandleMessage);
            UpdateThread.Start();
        }

        /// <summary>
        /// Cleanup network statements.
        /// </summary>
        ~Network()
        {
            UpdateThread.Abort();
            netPeer.Shutdown("Shutdown!");
        }

        /// <summary>
        /// Handle messages from server or client and choose what to do with them.
        /// </summary>
        private void HandleMessage()
        {
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage netIncomingMessage;

                while ((netIncomingMessage = netPeer.ReadMessage()) != null)
                {
                    switch (netIncomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.UnconnectedData:
                            switch ((ServerMessageTypes)netIncomingMessage.ReadByte())
                            {
                                case ServerMessageTypes.RegisterHost:
                                    ServerMessage serverMessage = new ServerMessage(netIncomingMessage); // Decode server message.

                                    serverMessage.IPendPoint = new IPEndPoint[]
                                        {
                                            netIncomingMessage.ReadIPEndPoint(),
										    netIncomingMessage.SenderEndPoint
                                        };
                                    if (RegistredServers.ContainsKey(serverMessage.ID) == true)
                                        RegistredServers.Add(serverMessage.ID, serverMessage);
                                    else
                                        RegistredServers[serverMessage.ID] = serverMessage;

                                    Log.HandleLog(LOGMessageTypes.Info, "Got registration for host", String.Format("{0}.", serverMessage.ID));
                                    break;

                                case ServerMessageTypes.RequestHostList:
                                    Log.HandleLog(LOGMessageTypes.Info, "Sending list of", RegistredServers.Count, "hosts to client", String.Format("{0}.", netIncomingMessage.SenderEndPoint));

                                    foreach (KeyValuePair<long, ServerMessage> RegistredServer in this.RegistredServers)
                                    {
                                        NetOutgoingMessage netOutgoingMessage = netPeer.CreateMessage();

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

                                        netPeer.SendUnconnectedMessage(netOutgoingMessage, netIncomingMessage.SenderEndPoint); // Send encoded message to client.
                                    }
                                    break;
                                case ServerMessageTypes.RequestIntroduction:
                                    IPEndPoint clientInternal = netIncomingMessage.ReadIPEndPoint();
                                    long ServerID = netIncomingMessage.ReadInt64();
                                    string Token = netIncomingMessage.ReadString();

                                    Log.HandleLog(LOGMessageTypes.Info, netIncomingMessage.SenderEndPoint, "requesting introduction to", ServerID, String.Format("(token{0}).", Token));

                                    if (this.RegistredServers.ContainsKey(ServerID))
                                    {
                                        Log.HandleLog(LOGMessageTypes.Info, "Sending introduction...");

                                        netPeer.Introduce(this.RegistredServers[ServerID].IPendPoint[0], this.RegistredServers[ServerID].IPendPoint[1], clientInternal, netIncomingMessage.SenderEndPoint, Token);
                                    }
                                    else
                                        Console.WriteLine("Client requested introduction to nonlisted host!");
                                    break;
                            }
                            break;

                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            Log.HandleLog(LOGMessageTypes.Debug, netIncomingMessage.ReadString());
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            Log.HandleLog(LOGMessageTypes.Warning, netIncomingMessage.ReadString());
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            Log.HandleLog(LOGMessageTypes.Error, netIncomingMessage.ReadString());
                            break;
                    }
                }

                Thread.Sleep(1);
            }
        }
    }
}
