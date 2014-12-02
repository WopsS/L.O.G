using Lidgren.Network;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LOG.Client.Networking.Managers
{
    class VesselManager
    {
        public static void CreateVesselMessage()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT || FlightGlobals.ActiveVessel == null || !FlightGlobals.ActiveVessel.loaded || FlightGlobals.ActiveVessel.packed)
                return;

            NetOutgoingMessage netOutgoingMessage = ClientNetwork.netClient.CreateMessage();

            ProtoVessel protoVessel = new ProtoVessel(FlightGlobals.fetch.activeVessel); // Get active vessel as a ProtoVessel.
            ConfigNode configNode = new ConfigNode();

            protoVessel.Save(configNode); // Save ProtoVessel in empty node to get data about it and send it later.

            IGameMessage gameMessage = new VesselMessage
            {
                ID = FlightGlobals.fetch.activeVessel.id.ToString(),
                Name = FlightGlobals.fetch.activeVessel.name,
                Data = configNode.ToString() // Send data about vessel here.
            };

            netOutgoingMessage.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMessage(netOutgoingMessage);

            ClientNetwork.SendMessage(netOutgoingMessage);
        }

        public static void HandleVesselMessage(IGameMessage Message)
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT)
                return;

            VesselMessage vesselMessage = (VesselMessage)Message; // Decode vessel message.

            ConfigNode currentNode = new ConfigNode(vesselMessage.Data); // Create a new ConfigNode with vessel data recevied.
            ProtoVessel protoVessel = new ProtoVessel(currentNode, HighLogic.CurrentGame); // Create a new ProtoVessel from ConfigNode.

            UnityThreadHelperExtended.Dispatcher.Dispatch(() => Log.HandleLog(API.IO.Log.LOGMessageTypes.Debug, "Ulalala, I got the message!"));

            // TODO: Create new vessels in game and update them.
        }
    }
}
