using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityThreadingExtended;

namespace Client
{
    class ClientNetwork
    {
        public static NetClient NetworkClient;
        static ActionThread NetworkIncomingMessageThread;
        public static bool isConnected = false;

        public static void Main()
        {
            NetPeerConfiguration Configuration = new NetPeerConfiguration("LOGMultiplayer");
            Configuration.AutoFlushSendQueue = false;
            NetworkClient = new NetClient(Configuration);
        }

        public static void NetworkIncomingMessage()
        {
            NetIncomingMessage IncomingMessage;

            while ((IncomingMessage = NetworkClient.ReadMessage()) != null)
            {
                switch (IncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:

                        string text = IncomingMessage.ReadString();
                         UnityThreadHelperExtended.Dispatcher.Dispatch(() => Debug.Log(text));

                        break;
                    case NetIncomingMessageType.StatusChanged:

                        NetConnectionStatus status = (NetConnectionStatus)IncomingMessage.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            isConnected = true;

                        else
                            isConnected = false;

                        string reason = IncomingMessage.ReadString();
                         UnityThreadHelperExtended.Dispatcher.Dispatch(() => Debug.Log(status.ToString() + ": " + reason));

                        break;
                    case NetIncomingMessageType.Data:

                        string Data = IncomingMessage.ReadString();
                        UnityThreadHelperExtended.Dispatcher.Dispatch(() => Debug.Log(Data));
                        //ClientVessel.GetProtoVessel(Data);

                        break;
                    default:

                        UnityThreadHelperExtended.Dispatcher.Dispatch(() => Debug.Log("Unhandled type: " + IncomingMessage.MessageType + " " + IncomingMessage.LengthBytes + " bytes"));

                        break;
                }
            }

            Thread.Sleep(10);
            NetworkIncomingMessage();
        }

        public static void Connect(string host, int port)
        {
            try
            {
                ClientNetwork.Main();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            NetworkClient.Start();
            NetOutgoingMessage hail = NetworkClient.CreateMessage("Hail message!");
            NetworkClient.Connect(host, port, hail);

            Thread.Sleep(10);

            NetworkIncomingMessageThread = UnityThreadHelperExtended.CreateThread(new Action(NetworkIncomingMessage));
            ClientMain.StartGame();
        }

        public static void Shutdown()
        {
            NetworkClient.Disconnect("Quit");
            NetworkClient.Shutdown("Bye");
        }

        public static void Send(string text)
        {
            NetOutgoingMessage om = NetworkClient.CreateMessage(text);
            NetworkClient.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            //Debug.Log("Sending '" + text + "'");
            NetworkClient.FlushSendQueue();
        }
    }
}
