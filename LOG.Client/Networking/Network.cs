using Lidgren.Network;
using LOG.API;
using LOG.API.IO.Log;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using LOG.Client.Networking.Managers;
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

        public static NetClient netClient;
        public static ActionThread NetworkIMThread = UnityThreadHelperExtended.CreateThread(new Action(NetworkIncomingMessage), false);
        public static ActionThread NetworkMessagesThread = UnityThreadHelperExtended.CreateThread(new Action(NetworkMessages), false);

        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="Hostname">IP address to connect.</param>
        /// <param name="Port">Server port to connect.</param>
        public static void Initialize(object Hostname, object Port)
        {
            NetPeerConfiguration Configuration = new NetPeerConfiguration("LOGMultiplayer");
            netClient = new NetClient(Configuration);
 
            netClient.Start(); // Start netClient.

            NetOutgoingMessage netOutgoingMessage = netClient.CreateMessage();

            IGameMessage gameMessage = new HandShakeMessage
            {
                Version = APIMain.Version,
                Username = PlayerManager.Username
            };

            netOutgoingMessage.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMessage(netOutgoingMessage);

            netClient.Connect(Hostname.ToString(), Convert.ToInt32(Port), netOutgoingMessage);
            NetworkIMThread.Start(); // Start IncomingMessage thread.
            NetworkMessagesThread.Start(); // Start OutgoingMessage thread.
        }

        public static void NetworkIncomingMessage()
        {
            while (netClient.Status == NetPeerStatus.Running)
            {
                NetIncomingMessage IncomingMessage;

                while ((IncomingMessage = netClient.ReadMessage()) != null)
                {
                    switch (IncomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:

                            NetConnectionStatus Status = (NetConnectionStatus)IncomingMessage.ReadByte();

                            switch (Status)
                            {
                                case NetConnectionStatus.Connected:
                                    NetIncomingMessage asd = IncomingMessage;
                                        UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(LOGMessageTypes.Info, "Connected to server."));
                                        PlayerManager.isConnected = true;
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(LOGMessageTypes.Info, "Disconnected from server."));
                                    Application.Quit();
                                    break;
                                case NetConnectionStatus.Disconnecting:
                                case NetConnectionStatus.InitiatedConnect:
                                case NetConnectionStatus.RespondedConnect:
                                    UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(LOGMessageTypes.Info, Status.ToString()));
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            PlayerManager.messageHandler.Handle(IncomingMessage);
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            string ErrorMessage = IncomingMessage.ReadString();
                            UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(LOGMessageTypes.Error, ErrorMessage));
                            break;
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string DebugMessage = IncomingMessage.ReadString();
                            UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(LOGMessageTypes.Debug, DebugMessage));
                            break;
                        default:
                            UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(LOGMessageTypes.Error, "Unhandled type: " + IncomingMessage.MessageType + " " + IncomingMessage.LengthBytes + " bytes"));
                            break;
                    }

                    netClient.Recycle(IncomingMessage);
                }

                if (NetworkIMThread.ShouldStop == true)
                    break;
            }
        }

        public static void NetworkMessages()
        {
            PlayerManager.messageHandler.OnVesselUpdateState += VesselManager.HandleVesselMessage;

            while (netClient.Status == NetPeerStatus.Running)
            {
                VesselManager.CreateVesselMessage();

                if (NetworkMessagesThread.ShouldStop == true)
                    break;

                Thread.Sleep(1);
            }

            PlayerManager.messageHandler.OnVesselUpdateState -= VesselManager.HandleVesselMessage;
        }

        /// <summary>
        /// Disconect from the server.
        /// </summary>
        public static void Shutdown()
        {
            netClient.Disconnect("Quit");
            netClient.Shutdown("Requested by user");
        }

        /// <summary>
        /// Send unordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public static void SendMessage(string Message)
        {
            NetOutgoingMessage OutgoingMessage = netClient.CreateMessage(Message);
            netClient.SendMessage(OutgoingMessage, NetDeliveryMethod.ReliableUnordered);
        }

        /// <summary>
        /// Send unordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public static void SendMessage(NetOutgoingMessage Message)
        {
            netClient.SendMessage(Message, NetDeliveryMethod.ReliableUnordered);
        }

        /// <summary>
        /// Send ordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public void SendMessageOrdered(string Message)
        {
            NetOutgoingMessage OutgoingMessage = netClient.CreateMessage(Message);
            netClient.SendMessage(OutgoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Send ordered messages.
        /// </summary>
        /// <param name="Message">The message to be send.</param>
        public static void SendMessageOrdered(NetOutgoingMessage Message)
        {
            netClient.SendMessage(Message, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
