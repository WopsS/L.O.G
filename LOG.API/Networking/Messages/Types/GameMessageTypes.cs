using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.Networking.Messages.Types
{
    /// <summary>
    /// Types of game messages.
    /// </summary>
    public enum GameMessageTypes
    {
        DiscoveryState = 1,
        ConnectionApprovalRequest,

        UpdateVesseState,
    }
}
