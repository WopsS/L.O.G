using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityThreadingExtended;

namespace LOG.Client
{
    class ClientNetwork
    {

        public static NetClient NetworkClient;
        public static bool isConnected = false;
        private static string NetIMResult = null;
        //public static ActionThread NetworkIncomingMessageThread;
        //public static Thread NetworkIncomingMessageThread = new Thread(NetworkIncomingMessage);

        public static void Main()
        {
            NetPeerConfiguration Configuration = new NetPeerConfiguration("LOGMultiplayer");
            //Configuration.AutoFlushSendQueue = false;
            NetworkClient = new NetClient(Configuration);
        }

        public static string NetworkIncomingMessage()
        {
            NetIMResult = null;

            NetIncomingMessage IncomingMessage;

            while ((IncomingMessage = NetworkClient.ReadMessage()) != null)
            {
                switch (IncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:

                        string DebugMessage = IncomingMessage.ReadString();
                        Log.HandleLog(Log.LOGMessageTypes.Debug, DebugMessage);

                        break;
                    case NetIncomingMessageType.StatusChanged:

                        NetConnectionStatus Status = (NetConnectionStatus)IncomingMessage.ReadByte();

                        string StatusMessage = IncomingMessage.ReadString();

                        //if (Status == NetConnectionStatus.Connected)
                            //ClientMain.playerManagers.isConnected = true;
                        //else if (Status == NetConnectionStatus.Disconnected)
                          //  Application.Quit();

                        Log.HandleLog(Log.LOGMessageTypes.Info, true, true, true, Status.ToString() + ": " + StatusMessage);

                        break;
                    case NetIncomingMessageType.Data:

                        NetIMResult = IncomingMessage.ReadString();

                        break;
                    default:

                        Log.HandleLog(Log.LOGMessageTypes.Error, "Unhandled type: " + IncomingMessage.MessageType + " " + IncomingMessage.LengthBytes + " bytes");

                        break;
                }
            }

            return NetIMResult;
        }

        public static void Connect(string Host, string Port)
        {
            try
            {
                ClientNetwork.Main();

                NetworkClient.Start();
                NetOutgoingMessage OutgoingMessage = NetworkClient.CreateMessage();
                NetworkClient.Connect(Host, Convert.ToInt32(Port), OutgoingMessage);

                //NetworkIncomingMessageThread.Priority = System.Threading.ThreadPriority.Highest;
                //NetworkIncomingMessageThread.Start();

                //NetOutgoingMessage om = NetworkClient.CreateMessage();
                //IGameMessage gameMessage = new HandShakeMessage
                //{
                //    Version = APIMain.Version,
                //    Username = ClientMain.Username
                //};

                //om.Write((byte)gameMessage.MessageType);
                //gameMessage.EncodeMessage(om);

                ClientNetwork.NetworkIncomingMessage();

                ClientMain.StartGame();
            }
            catch (UnityException e)
            {
                Debug.LogError(e.ToString());
            }

        }

        public static void Shutdown()
        {
            NetworkClient.Disconnect("Quit");
            NetworkClient.Shutdown("Requested by user");
        }

        public static void SendMessage(string Message)
        {
            NetOutgoingMessage OutgoingMessage = NetworkClient.CreateMessage(Message);
            NetworkClient.SendMessage(OutgoingMessage, NetDeliveryMethod.ReliableUnordered);
        }

        public void SendMessageOrdered(string Message)
        {
            NetOutgoingMessage OutgoingMessage = NetworkClient.CreateMessage(Message);
            NetworkClient.SendMessage(OutgoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

    }
}
