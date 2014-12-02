using Lidgren.Network;
using LOG.API;
using LOG.API.IO.Log;
using LOG.API.Networking;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using LOG.Server.Networking.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LOG.Server.Networking
{
    class Network
    {
        /*      
         * TODO: There is an error 'Socket threw exception; would block - send buffer full? Increase in NetPeerConfiguration' find a way to send a minimal message, no all data about vessel. 
         * If the vessel will be a big space ship, then the server will explode (kidding).
         */

        public static NetServer netServer;
        private static readonly NetPeerConfiguration Configuration = new NetPeerConfiguration("LOGMultiplayer");

        private static readonly Thread NetworkIMThread = new Thread(NetworkIncomingMessage);
        public static Dictionary<NetConnection, SessionManager> Sessions = new Dictionary<NetConnection, SessionManager>();

        /// <summary>
        /// Initialize server.
        /// </summary>
        public static void Initialize()
        {
            Log.HandleLog(LOGMessageTypes.Info, "Configuring server...");

            Configuration.MaximumConnections = Convert.ToInt32(ServerMain.CfgValues["maxplayers"]);
            Configuration.Port = Convert.ToInt32(ServerMain.CfgValues["port"]);
            Configuration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            Log.HandleLog(LOGMessageTypes.Info, true, true, true, "Server configured.");
            Log.HandleEmptyMessage();
            Log.HandleLog(LOGMessageTypes.Info, "Initializing server...");

            netServer = new NetServer(Configuration);
            netServer.Start(); // Start the server.

            Log.HandleLog(LOGMessageTypes.Info, true, true, true, "Server initialized.");
            Log.HandleEmptyMessage();

            NetworkIMThread.Start(); // Start NetworkIMThread thread.

            ReadServerCommand();
        }

        private static void NetworkIncomingMessage()
        {
            while (netServer.Status == NetPeerStatus.Running)
            {
                NetIncomingMessage IncomingMessage;
                while ((IncomingMessage = netServer.ReadMessage()) != null)
                {
                    switch (IncomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            NetOutgoingMessage ResponseMessage = netServer.CreateMessage();

                            IGameMessage gameMessage = new DiscoveryRequestMessage
                            {
                                Hostname = ServerMain.CfgValues["hostname"],
                                Players = netServer.Connections.Count,
                                MaximumPlayers = Convert.ToInt32(ServerMain.CfgValues["maxplayers"])
                            };

                            gameMessage.EncodeMessage(ResponseMessage);

                            netServer.SendDiscoveryResponse(ResponseMessage, IncomingMessage.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = IncomingMessage.ReadString();
                            Log.HandleLog(LOGMessageTypes.Debug, text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus Status = (NetConnectionStatus)IncomingMessage.ReadByte();

                            switch(Status)
                            { 
                                case NetConnectionStatus.RespondedConnect:
                                    Log.HandleLog(LOGMessageTypes.Info, "Incoming connection from", IncomingMessage.SenderEndPoint);
                                    break;
                                case NetConnectionStatus.Connected:
                                    Log.HandleLog(LOGMessageTypes.Info, Sessions[IncomingMessage.SenderConnection].Username, "has joined the server");
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    Log.HandleLog(LOGMessageTypes.Info, Sessions[IncomingMessage.SenderConnection].Username, "has left the server");
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            GameMessageTypes gameMessageType = (GameMessageTypes)IncomingMessage.SenderConnection.RemoteHailMessage.ReadByte();

                            if (gameMessageType != GameMessageTypes.HandShakeState)
                                break;

                            HandShakeMessage Message = new HandShakeMessage(IncomingMessage.SenderConnection.RemoteHailMessage);

                            if (Message.Version == APIMain.Version && Message.Username != null && Message.Username.Length > 0)
                            {
                                IncomingMessage.SenderConnection.Approve(netServer.CreateMessage());
                                Sessions.Add(IncomingMessage.SenderConnection, new SessionManager(IncomingMessage.SenderConnection, Message.Username));
                            }
                            else
                                IncomingMessage.SenderConnection.Deny("Wrong version or username!");
                            break;
                        case NetIncomingMessageType.Data:
                            Sessions[IncomingMessage.SenderConnection].messageHandler.Handle(IncomingMessage);
                            break;

                        default:
                            Log.HandleLog(LOGMessageTypes.Error, "Unhandled type: " + IncomingMessage.MessageType + " " + IncomingMessage.LengthBytes + " bytes " + IncomingMessage.DeliveryMethod + "|" + IncomingMessage.SequenceChannel);
                            break;
                    }

                    netServer.Recycle(IncomingMessage);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Read and execute server commands typed by host in server console.
        /// </summary>
        private static void ReadServerCommand()
        {
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                string[] Result = input.Split(new char[] { ' ' }, 2);

                if (Result[0].Remove(0, 1) == "send")
                {
                    SendMessageToAll(Result[1]);
                }
            }

            ReadServerCommand();
        }

        /// <summary>
        /// Send message to all connected clients.
        /// </summary>
        /// <param name="Message">Message to be send.</param>
        private static void SendMessageToAll(string Message)
        {
            try
            {
                NetOutgoingMessage om = netServer.CreateMessage(Message);
                netServer.SendToAll(om, null, NetDeliveryMethod.ReliableOrdered, 0);
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
