using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Messages
{
    public enum GameMessageTypes : byte
    {
        DiscoveryState,
        HandShakeState,
        ChatTextState,
        UpdatePlayerState,
        UpdateVesseState
    }
}
