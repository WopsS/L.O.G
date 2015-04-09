using Lidgren.Network;
using LOG.API;
using LOG.API.IO.Log;
using LOG.API.Networking.Messages;
using LOG.API.Networking.Messages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LOG.Server.Networking
{
    internal class Network
    {
        public NetPeer netPeer;
        private NetPeerConfiguration netPeerConfiguration = new NetPeerConfiguration("LOGServer");
        private Thread UpdateThread = null;

        /// <summary>
        /// Start local network peer.  
        /// </summary>
        public Network()
        {
            netPeerConfiguration.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            netPeerConfiguration.Port = APIMain.ServerPort;

            netPeer = new NetPeer(netPeerConfiguration);
            netPeer.Start();

            UpdateThread = new Thread(Update);
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

        private void Update()
        {
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                this.HandleMessage();

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Handle messages from server or client and choose what to do with them.
        /// </summary>
        private void HandleMessage()
        {
            NetIncomingMessage netIncomingMessage;

            while ((netIncomingMessage = netPeer.ReadMessage()) != null)
            {
                switch (netIncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.UnconnectedData:
                        {
                            // ...
                            break;
                        }
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {
                            Log.HandleLog(LOGMessageTypes.Debug, netIncomingMessage.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.WarningMessage:
                        {
                            Log.HandleLog(LOGMessageTypes.Warning, netIncomingMessage.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.ErrorMessage:
                        {
                            Log.HandleLog(LOGMessageTypes.Error, netIncomingMessage.ReadString());
                            break;
                        }
                }
            }
        }
    }
}
