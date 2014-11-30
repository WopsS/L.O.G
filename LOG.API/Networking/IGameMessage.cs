using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking
{
    public enum GameMessageTypes
    {
        Chat,
        UpdatePlayer,
        HandShake
    }

    public interface IGameMessage
    {
        GameMessageTypes MessageType { get; }

        void DecodeMessage(NetIncomingMessage Message);

        void EncodeMessage(NetOutgoingMessage Message);
    }
}
