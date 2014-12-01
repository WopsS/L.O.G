using Lidgren.Network;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.Server.Networking.Managers
{
    class VesselManager
    {
        public static void HandleVesselUpdateState(IGameMessage Message, NetConnection SenderConnection)
        {
            List<NetConnection> AllConnections = Network.netServer.Connections;
            AllConnections.Remove(SenderConnection);

            if (AllConnections.Count > 0)
            {
                NetOutgoingMessage netOutgoingMessage = Network.netServer.CreateMessage();

                netOutgoingMessage.Write((byte)Message.MessageType);
                Message.EncodeMessage(netOutgoingMessage);

                Network.netServer.SendMessage(netOutgoingMessage, AllConnections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
    }
}
