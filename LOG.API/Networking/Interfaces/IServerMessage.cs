using Lidgren.Network;
using LOG.API.Networking.Messages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Interfaces
{
    public interface IServerMessage
    {
        ServerMessageTypes MessageType { get; set; }

        void DecodeMessage(NetIncomingMessage Message);

        void EncodeMessage(NetOutgoingMessage Message);
    }
}
