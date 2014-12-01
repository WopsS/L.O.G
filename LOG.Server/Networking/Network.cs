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
        private static NetPeerConfiguration Configuration;

        private static Thread NetworkIMThread = new Thread(NetworkIncomingMessage);
        private static MessageHandler messageHandler = new MessageHandler();

        /// <summary>
        /// Initialize server.
        /// </summary>
        public static void Initialize()
        {
            Log.HandleLog(LOGMessageTypes.Info, "Configuring server...");

            Configuration = new NetPeerConfiguration("LOGMultiplayer");
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

            messageHandler.OnVesselUpdateState += VesselManager.HandleVesselUpdateState;

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
                            NetOutgoingMessage response = netServer.CreateMessage();
                            response.Write(string.Format("HostName:{0}#Players:{1}#MaxPlayers:{2}", ServerMain.CfgValues["hostname"], netServer.Connections.Count, Convert.ToInt32(ServerMain.CfgValues["maxplayers"])));

                            netServer.SendDiscoveryResponse(response, IncomingMessage.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = IncomingMessage.ReadString();
                            Log.HandleLog(LOGMessageTypes.Debug, text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)IncomingMessage.ReadByte();

                            string reason = IncomingMessage.ReadString();
                            Log.HandleLog(LOGMessageTypes.Debug, NetUtility.ToHexString(IncomingMessage.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            // Check if GameMessageTypes is Handshake.
                            GameMessageTypes opcode = (GameMessageTypes)IncomingMessage.SenderConnection.RemoteHailMessage.ReadByte();
                            if (opcode != GameMessageTypes.HandShakeState)
                                break;

                            HandShakeMessage msg = new HandShakeMessage(IncomingMessage.SenderConnection.RemoteHailMessage);

                            if (msg.Version == APIMain.Version && msg.Username != null && msg.Username.Length > 0)
                                IncomingMessage.SenderConnection.Approve(netServer.CreateMessage());
                            else
                                IncomingMessage.SenderConnection.Deny("Wrong version !");
                            break;
                        case NetIncomingMessageType.Data:
                            messageHandler.Handle(IncomingMessage, IncomingMessage.SenderConnection);
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
        /// Send message to all connected clients except sender of the message.
        /// </summary>
        /// <param name="Message">Message to be send.</param>
        /// <param name="Connection">NetConnection of sender.</param>
        private static void SendMessage(string Message, NetConnection Connection)
        {
            List<NetConnection> all = netServer.Connections;
            all.Remove(Connection);

            if (all.Count > 0)
            {
                NetOutgoingMessage om = netServer.CreateMessage(Message);
                netServer.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        /// <summary>
        /// Send message to all connected clients.
        /// </summary>
        /// <param name="Message">Message to be send.</param>
        private static void SendMessageToAll(string Message)
        {
            NetOutgoingMessage om = netServer.CreateMessage(Message);
            netServer.SendToAll(om, null, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
