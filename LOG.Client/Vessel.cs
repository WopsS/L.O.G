using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LOG.Client
{
    class ClientVessel : MonoBehaviour
    {
        public static string PrepareVesselForSending(ProtoVessel PlayerVessel)
        {
            string TemporaryFile = Path.GetTempFileName();
            ConfigNode CurrentNode = new ConfigNode();

            PlayerVessel.Save(CurrentNode);
            CurrentNode.Save(TemporaryFile);

            string TemporaryVesselString = File.ReadAllText(TemporaryFile, Encoding.Default);

            File.Delete(TemporaryFile);

            return TemporaryVesselString;
        }

        public static void SendVessel()
        {
            CreateVessel();
        }

        static Vessel vessel;
        static Vessel pl_vessel = new Vessel();
        static Vector3 prebuild_pos = new Vector3(); //Just a save for the position of the Vessel vessel, so that it won't be shot to Jool when Build() happens. :D

        public static string path = Path.Combine(Environment.CurrentDirectory, Path.Combine("saves", "L.O.G")) + "/Ships/VAB/Rover + Skycrane.craft";
        static bool otherPlayerConnected = false;
        public static float SpawnHeightOffset = 1.0f;
        public static void BuildVessel()
        {
            ShipConstruct LoadedShip = ShipConstruction.LoadShip(path);

            Vector3 offset = Vector3.up * SpawnHeightOffset;

            Transform TransformPlayerVessel = vessel.transform;

            TransformPlayerVessel.position = new Vector3(5f, 5f, 5f);

            string landedAt = "Launchpad";
            string flag = "default";
            Game state = FlightDriver.FlightStateCache;
            VesselCrewManifest crew = new VesselCrewManifest();

            GameObject LaunchPosition = new GameObject();
            LaunchPosition.transform.position = TransformPlayerVessel.position;
            LaunchPosition.transform.position += TransformPlayerVessel.TransformDirection(offset);
            LaunchPosition.transform.rotation = TransformPlayerVessel.rotation;
            ShipConstruction.CreateBackup(LoadedShip);
            ShipConstruction.PutShipToGround(LoadedShip, LaunchPosition.transform);
            Destroy(LaunchPosition);
            ShipConstruction.AssembleForLaunch(LoadedShip, landedAt, flag, state, crew);
            FlightGlobals.SetActiveVessel(vessel);

            otherPlayerConnected = true;
        }

        public static string ConvertVessel(Vessel vessel)
        {
            try
            {
                StringBuilder VesselBuldier = new StringBuilder();

                VesselBuldier.Append(vessel.GetWorldPos3D().x.ToString()).Append(" ");
                VesselBuldier.Append(vessel.GetWorldPos3D().y.ToString()).Append(" ");
                VesselBuldier.Append(vessel.GetWorldPos3D().z.ToString()).Append(" ");
                VesselBuldier.Append(FlightGlobals.ActiveVessel.mainBody.position.x.ToString()).Append(" ");
                VesselBuldier.Append(FlightGlobals.ActiveVessel.mainBody.position.y.ToString()).Append(" ");
                VesselBuldier.Append(FlightGlobals.ActiveVessel.mainBody.position.z.ToString()).Append(" ");
                VesselBuldier.Append(vessel.transform.rotation.x.ToString()).Append(" ");
                VesselBuldier.Append(vessel.transform.rotation.y.ToString()).Append(" ");
                VesselBuldier.Append(vessel.transform.rotation.z.ToString()).Append(" ");
                VesselBuldier.Append(vessel.transform.rotation.w.ToString());

                return VesselBuldier.ToString();
            }
            catch (Exception ex)
            {
                LOG.ShowLOG(LOG.LOGType.Error, true, true, "L.O.G. ERROR: " + ex.InnerException);
            }
            return "";
        }
        public static string[] messageParts;
        public static bool isCreating = false;
        public static void CreateVessel()
        {
            try
            {
                if (isCreating == true)
                    return;

                
                if (HighLogic.LoadedScene != GameScenes.FLIGHT || FlightGlobals.ActiveVessel == null || !FlightGlobals.ActiveVessel.loaded || FlightGlobals.ActiveVessel.packed)
                    return;

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#1");

                string vesselData = "";           //Data to be sent
                string RecvMsg = String.Empty;
                float RecvX = 0f;           //Recivied X pos
                float RecvY = 0f;           //Recivied Y pos
                float RecvZ = 0f;           //Recivied Z pos
                float RecvRootX = 0f;
                float RecvRootY = 0f;
                float RecvRootZ = 0f;
                float RecvRotW = 0f;
                float RecvRotX = 0f;
                float RecvRotY = 0f;
                float RecvRotZ = 0f;

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#2");

                vessel = FlightGlobals.fetch.activeVessel;
                pl_vessel = FlightGlobals.FindNearestControllableVessel(vessel);

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#3");

                vesselData = ConvertVessel(vessel);
                ClientNetwork.SendMessage(vesselData);

                isCreating = true;

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#4");

                RecvMsg = ClientNetwork.NetworkIncomingMessage();

                if (RecvMsg == null)
                {
                    isCreating = false;
                    return;
                }

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#5");

                messageParts = RecvMsg.Split(' ');

                if (messageParts.Length != 10)
                {
                    isCreating = false;
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "LOGclient: Received invalid message <" + RecvMsg + ">");
                    return;
                }

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#6");

                RecvX = (float)double.Parse(messageParts[0], System.Globalization.CultureInfo.InvariantCulture);
                RecvY = (float)double.Parse(messageParts[1], System.Globalization.CultureInfo.InvariantCulture);
                RecvZ = (float)double.Parse(messageParts[2], System.Globalization.CultureInfo.InvariantCulture);
                RecvRootX = (float)double.Parse(messageParts[3], System.Globalization.CultureInfo.InvariantCulture);
                RecvRootY = (float)double.Parse(messageParts[4], System.Globalization.CultureInfo.InvariantCulture);
                RecvRootZ = (float)double.Parse(messageParts[5], System.Globalization.CultureInfo.InvariantCulture);
                RecvRotX = (float)double.Parse(messageParts[6], System.Globalization.CultureInfo.InvariantCulture);
                RecvRotY = (float)double.Parse(messageParts[7], System.Globalization.CultureInfo.InvariantCulture);
                RecvRotZ = (float)double.Parse(messageParts[8], System.Globalization.CultureInfo.InvariantCulture);
                RecvRotW = (float)double.Parse(messageParts[9], System.Globalization.CultureInfo.InvariantCulture);
                //LOG.ShowLOG(LOG.LOGType.Debug, true, true, RecvX.ToString(), RecvY.ToString(), RecvZ.ToString(), RecvRootZ.ToString(), RecvRootY.ToString(), RecvRootZ.ToString(), RecvRotX.ToString(), RecvRotY.ToString(), RecvRotZ.ToString(), RecvRotW.ToString());

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#7");

                if (otherPlayerConnected == false)
                {
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.1");

                    prebuild_pos = vessel.GetWorldPos3D();
                    vessel.GoOffRails();
                    BuildVessel();
                    vessel.GoOnRails();

                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.2");

                    //FlightGlobals flightGlobals = new FlightGlobals();
                    vessel = FlightGlobals.ActiveVessel;

                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.2.1");
                    pl_vessel = FlightGlobals.FindNearestControllableVessel(vessel); //after building, the vessels have to be set new.
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.2.2");
                    pl_vessel.SetPosition(vessel.mainBody.position - new Vector3d(RecvRootX, RecvRootY, RecvRootZ) + new Vector3d(RecvX, RecvY, RecvZ));
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.2.3");
                    pl_vessel.SetWorldVelocity(new Vector3d(0, 0, 0));
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.2.4");
                    vessel.SetWorldVelocity(new Vector3d(0, 0, 0));
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.2.5");
                    vessel.SetPosition(prebuild_pos);
                    pl_vessel.useGUILayout = true;
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.1.3");
                }
                else
                {
                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.2.1");

                    pl_vessel.SetPosition(vessel.mainBody.position - new Vector3d(RecvRootX, RecvRootY, RecvRootZ) + new Vector3d(RecvX, RecvY, RecvZ));
                    pl_vessel.SetRotation(new Quaternion(RecvRotX, RecvRotY, RecvRotZ, RecvRotW));

                    LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#8.2.2");

                    pl_vessel.SetWorldVelocity(new Vector3d(0, 0, 0));
                }

                LOG.ShowLOG(LOG.LOGType.Debug, true, true, "#9");

                RecvMsg = null;
                isCreating = false;

            }
            catch (UnityException e)
            {
                isCreating = false;
                LOG.ShowLOG(LOG.LOGType.Error, true, true, "UnityException: ", e.InnerException.ToString());
            }
            catch (Exception e)
            {
                isCreating = false;
                LOG.ShowLOG(LOG.LOGType.Error, true, true, "Exception: ", e.InnerException.ToString());
            }
        }
    }
}
