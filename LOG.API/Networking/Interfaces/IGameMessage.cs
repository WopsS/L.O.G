using Lidgren.Network;
using LOG.API.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Interfaces
{
    public interface IGameMessage
    {
        GameMessageTypes MessageType { get; }

        void DecodeMessage(NetIncomingMessage Message);

        void EncodeMessage(NetOutgoingMessage Message);
    }
}
