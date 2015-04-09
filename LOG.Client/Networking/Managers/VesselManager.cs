using Lidgren.Network;
using LOG.API.IO.Log;
using LOG.API.Networking.Interfaces;
using LOG.API.Networking.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LOG.Client.Networking.Managers
{
    class VesselManager : MonoBehaviour
    {
        private NetClient m_netClient = null;

        public VesselManager(NetClient aNetClient)
        {
            GameObject.DontDestroyOnLoad(this);

            this.m_netClient = aNetClient;
        }

        public NetOutgoingMessage CreateVesselMessage()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT || FlightGlobals.ActiveVessel == null || !FlightGlobals.ActiveVessel.loaded || FlightGlobals.ActiveVessel.packed)
                return null;

            NetOutgoingMessage netOutgoingMessage = this.m_netClient.CreateMessage();

            ProtoVessel protoVessel = new ProtoVessel(FlightGlobals.fetch.activeVessel); // Get active vessel as a ProtoVessel.
            ConfigNode configNode = HighLogic.CurrentGame.config;

            protoVessel.Save(configNode); // Save ProtoVessel in empty node to get data about it and send it later.

            IGameMessage gameMessage = new VesselMessage
            {
                ID = FlightGlobals.fetch.activeVessel.id.ToString(),
                Name = FlightGlobals.fetch.activeVessel.name,
                Data = configNode.ToString(), // Send data about vessel here.
            };

            netOutgoingMessage.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMessage(netOutgoingMessage);

            return netOutgoingMessage;
        }

        public void HandleVesselMessage(IGameMessage Message)
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT)
                return;

            // TODO: Create vessel from network message.

            VesselMessage vesselMessage = (VesselMessage)Message; // Decode vessel message.

            ConfigNode currentNode = new ConfigNode(vesselMessage.Data); // Create a new ConfigNode with vessel data recevied.
            ProtoVessel protoVessel = new ProtoVessel(currentNode, HighLogic.CurrentGame); // Create a new ProtoVessel from ConfigNode.
            //Vessel currentVessel = FlightGlobals.fetch.activeVessel;

            Log.HandleLog(LOGMessageTypes.Debug, currentNode);

            //string tempFile = Environment.CurrentDirectory + "\\LOG\\Test" + currentVessel.id.ToString().Replace('-', '0') + DateTime.Now.Second + ".craft";
            //Log.HandleLog(LOGMessageTypes.Debug, tempFile);
            //File.AppendAllText(tempFile, vesselMessage.Data);

            //ShipConstruct shipConstruct = ShipConstruction(tempFile);
            //currentNode = shipConstruct.SaveShip();

            //Vector3 offset = Vector3.up * 1.0f;

            //Transform TransformPlayerVessel = currentVessel.transform;
            //TransformPlayerVessel.position = new Vector3(5f, 5f, 5f);

            //string landedAt = "Launchpad";
            //string flag = "default";
            //Game state = FlightDriver.FlightStateCache;
            //VesselCrewManifest crew = new VesselCrewManifest();

            //GameObject LaunchPosition = new GameObject();
            //LaunchPosition.transform.position = TransformPlayerVessel.position;
            //LaunchPosition.transform.position += TransformPlayerVessel.TransformDirection(offset);
            //LaunchPosition.transform.rotation = TransformPlayerVessel.rotation;
            //ShipConstruction.CreateBackup(shipConstruct);
            //ShipConstruction.PutShipToGround(shipConstruct, LaunchPosition.transform);
            //Destroy(LaunchPosition);
            //ShipConstruction.AssembleForLaunch(shipConstruct, landedAt, flag, state, crew);
            //FlightGlobals.SetActiveVessel(currentVessel);

            //Log.HandleLog(LOGMessageTypes.Debug, protoVessel.values.Count);
        }
    }
}
