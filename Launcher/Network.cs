using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LOG.Launcher
{
    class LauncherNetwork
    {
        public static NetClient NetworkClient;

        public static void FirstLaunch()
        {
            NetPeerConfiguration Configuration = new NetPeerConfiguration("LOGMultiplayer");
            Configuration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            Configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            NetworkClient = new NetClient(Configuration);

            NetworkClient.Start();
        }

        public static string DiscoverServers(string host, int port)
        {
            bool isDiscovered = false;
            string ResponseMessage = null;
            int Attempts = 0;

            NetworkClient.DiscoverKnownPeer(host, port);

            while (isDiscovered == false)
            {
                NetIncomingMessage IncomingMessage;

                while ((IncomingMessage = NetworkClient.ReadMessage()) != null)
                {
                    switch (IncomingMessage.MessageType)
                    {

                        case NetIncomingMessageType.DiscoveryResponse:

                            float rtt = (float)NetTime.Now - (float)IncomingMessage.ReceiveTime;

                            ResponseMessage = IncomingMessage.ReadString() + string.Format("#Ping:{0}", ((int)(rtt * 1000.0)).ToString()) ;
                            isDiscovered = true;
                            break;
                    }
                }

                Thread.Sleep(10);
                Attempts++;

                if (Attempts == 4)
                {
                    ResponseMessage = string.Format("HostName:(Retrieving info...) {0}:{1}#Players:0#MaxPlayers:0#Ping:0", host, port.ToString());
                    isDiscovered = true;
                }
            }

            NetworkClient.Disconnect("Requested by launcher");

            return ResponseMessage;
        }
    }
}
