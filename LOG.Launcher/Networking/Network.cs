using Lidgren.Network;
using LOG.API.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LOG.Launcher
{
    class Network
    {
        public static NetClient netClient;
        private static NetPeerConfiguration Configuration = new NetPeerConfiguration("LOGMultiplayer");

        private static double Ping = 0, SendedTime;

        /// <summary>
        /// Initialize client.
        /// </summary>
        public static void Initialize()
        {
            Configuration = new NetPeerConfiguration("LOGMultiplayer");
            Configuration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            netClient = new NetClient(Configuration);

            netClient.Start();
        }

        /// <summary>
        /// Discover a server and get details about it.
        /// </summary>
        /// <param name="Host">Server IP to be discovered.</param>
        /// <param name="Port">Server port to be discovered.</param>
        public static void DiscoverServers(string Host, int Port)
        {
            netClient.DiscoverKnownPeer(Host, Port);
            SendedTime = NetTime.Now; // Keet time when the discover request has been send.

            Program.main.DiscoveryMessage = new DiscoveryRequestMessage // We assume that the server was not found.
            {
                Hostname = String.Format("(Retrieving info...) {0}", Host),
                Players = 0,
                MaximumPlayers = 0,
                Ping = 0
            };

            for (int i = 1; i <= 3; i++)
            {
                NetIncomingMessage IncomingMessage;

                while ((IncomingMessage = netClient.ReadMessage()) != null)
                {
                    switch (IncomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            Ping = (0.25 * Ping) + ((1 - 0.25) * (IncomingMessage.ReceiveTime - SendedTime)); // Calculate ping.

                            Program.main.DiscoveryMessage = new DiscoveryRequestMessage(IncomingMessage);
                            Program.main.DiscoveryMessage.Ping = Convert.ToInt32(Ping * 1000.0);
                            break;
                    }
                }

                if (Program.main.DiscoveryMessage.Ping > 0)
                    break;

                Thread.Sleep(100);
            }

            netClient.Disconnect("Requested by launcher");
        }
    }
}
