using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LOG.Server
{
    class Network
    {
        private static NetServer NetworkServer;
        private static NetPeerConfiguration Configuration;
        private static Thread NetworkIncomingMessageThread;
        private static bool ServerAcceptNetworkMessage = true;

        public static void NetworkMain()
        {
            Log.HandleLog(Log.LOGMessageTypes.Info, "Configuring server...");

            Configuration = new NetPeerConfiguration("LOGMultiplayer");
            Configuration.MaximumConnections = Convert.ToInt32(ServerMain.CfgValues["maxplayers"]);
            Configuration.Port = Convert.ToInt32(ServerMain.CfgValues["port"]);
            Configuration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            Log.HandleLog(Log.LOGMessageTypes.Info, true, true, true, "Server configured.");
            Log.HandleEmptyMessage();
            Log.HandleLog(Log.LOGMessageTypes.Info, "Initializing server...");

            NetworkServer = new NetServer(Configuration);
            StartServer();

            Log.HandleLog(Log.LOGMessageTypes.Info, true, true, true, "Server initialized.");
            Log.HandleEmptyMessage();

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
                            Log.HandleLog(Log.LOGMessageTypes.Debug, text);

                            break;

                        case NetIncomingMessageType.StatusChanged:

                            NetConnectionStatus status = (NetConnectionStatus)IncomingMessage.ReadByte();

                            string reason = IncomingMessage.ReadString();
                            Log.HandleLog(Log.LOGMessageTypes.Debug, NetUtility.ToHexString(IncomingMessage.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            UpdateConnectionsList();

                            break;

                        case NetIncomingMessageType.Data:

                            string chat = IncomingMessage.ReadString();

                            SendMessage(chat, IncomingMessage.SenderConnection);

                            break;

                        default:

                            Log.HandleLog(Log.LOGMessageTypes.Error, "Unhandled type: " + IncomingMessage.MessageType + " " + IncomingMessage.LengthBytes + " bytes " + IncomingMessage.DeliveryMethod + "|" + IncomingMessage.SequenceChannel);
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

        private static void ReadServerCommand()
        {
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                string[] Result = input.Split(new char[] { ' ' }, 2);

                if(Result[0] == "/send")
                {
                    SendMessageToAll(Result[1]);
                }
            }

            ReadServerCommand();
        }

        private static void SendMessage(string Message, NetConnection Connection)
        {
            List<NetConnection> all = NetworkServer.Connections;
            all.Remove(Connection);

            if (all.Count > 0)
            {
                NetOutgoingMessage om = NetworkServer.CreateMessage(Message);
                NetworkServer.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        private static void SendMessageToAll(string Message)
        {
            NetOutgoingMessage om = NetworkServer.CreateMessage(Message);
            NetworkServer.SendToAll(om, null, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
