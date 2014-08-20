using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server
{
    class Network
    {
        private static NetServer NetworkServer;
        private static NetPeerConfiguration Configuration;
        private static Thread NetworkIncomingMessageThread;
        private static bool ServerAcceptNetworkMessage = true;

        public static void NetworkMain()
        {
            LOG.DisplayLOG(true, true, false, "Configuring server...");

            Configuration = new NetPeerConfiguration("LOGMultiplayer");
            Configuration.MaximumConnections = Convert.ToInt32(ServerMain.CfgValues["maxplayers"]);
            Configuration.Port = Convert.ToInt32(ServerMain.CfgValues["port"]);
            Configuration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            LOG.DisplayLOG(true, true, true, "Server configured.");
            LOG.DisplayLOG(true, true, false, "Initializing server...");

            NetworkServer = new NetServer(Configuration);
            StartServer();

            LOG.DisplayLOG(true, true, true, "Server initialized.");

            NetworkIncomingMessageThread = new Thread(NetworkIncomingMessage);
            NetworkIncomingMessageThread.Start();

            ReadServerCommand();
        }

        private static void NetworkIncomingMessage()
        {
            while (ServerAcceptNetworkMessage == true)
            {
                NetIncomingMessage IncomingMessage;
                while ((IncomingMessage = NetworkServer.ReadMessage()) != null)
                {
                    switch (IncomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:

                            NetOutgoingMessage response = NetworkServer.CreateMessage();
                            response.Write(string.Format("HostName:{0}#Players:{1}#MaxPlayers:{2}", ServerMain.CfgValues["hostname"], NetworkServer.Connections.Count, Convert.ToInt32(ServerMain.CfgValues["maxplayers"])));

                            NetworkServer.SendDiscoveryResponse(response, IncomingMessage.SenderEndPoint);
                            break;

                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:

                            string text = IncomingMessage.ReadString();
                            LOG.DisplayLOG(true, true, text);

                            break;

                        case NetIncomingMessageType.StatusChanged:

                            NetConnectionStatus status = (NetConnectionStatus)IncomingMessage.ReadByte();

                            string reason = IncomingMessage.ReadString();
                            LOG.DisplayLOG(true, true, NetUtility.ToHexString(IncomingMessage.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            UpdateConnectionsList();

                            break;

                        case NetIncomingMessageType.Data:

                            string chat = IncomingMessage.ReadString();

                            List<NetConnection> all = NetworkServer.Connections; 
                            all.Remove(IncomingMessage.SenderConnection);

                            if (all.Count > 0)
                            {
                                NetOutgoingMessage om = NetworkServer.CreateMessage();
                                om.Write(NetUtility.ToHexString(IncomingMessage.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                                NetworkServer.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            }

                            break;

                        default:

                            LOG.DisplayLOG(true, true, "Unhandled type: " + IncomingMessage.MessageType + " " + IncomingMessage.LengthBytes + " bytes " + IncomingMessage.DeliveryMethod + "|" + IncomingMessage.SequenceChannel);
                            break;
                    }
                    NetworkServer.Recycle(IncomingMessage);
                }

                Thread.Sleep(1);
            }
        }

        private static void UpdateConnectionsList()
        {
            foreach (NetConnection conn in NetworkServer.Connections)
            {
                string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                //s_form.listBox1.Items.Add(str);
            }
        }

        public static void StartServer()
        {
            NetworkServer.Start();
        }

        public static void Shutdown()
        {
            ServerAcceptNetworkMessage = false;
            NetworkServer.Shutdown("Requested by user");
        }

        static void ReadServerCommand()
        {
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                // Do something with this commands.
            }

            ReadServerCommand();
        }
    }
}
