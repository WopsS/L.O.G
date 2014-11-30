using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace LOG.Client
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class ClientMain : MonoBehaviour
    {
        public static bool GameStarted = false;
        public static string Username = null;

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this);

            CheckFiles();
        }

        private void FirstRun()
        {
            string[] Arguments = Environment.GetCommandLineArgs(), Host, Port;

            if (Array.Exists(Arguments, element => element.StartsWith("-IP=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Port=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Username=")) == true)
            {
                Host = Arguments[1].Split('=');
                Port = Arguments[2].Split('=');
                Username = Arguments[3].Split('=')[1];

                if (Host.Length > 0 && Port.Length > 0)
                    ClientNetwork.Connect(Host[1], Port[1]);

                //ScreenMessages.PostScreenMessage("Test", 5f, ScreenMessageStyle.UPPER_LEFT);
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedScene == GameScenes.MAINMENU && ClientNetwork.isConnected == false)
                FirstRun();
        }

        public void FixedUpdate()
        {
            //if (ClientNetwork.NetworkClient != null)
            //{
            //    if (ClientNetwork.NetworkClient.Status == Lidgren.Network.NetPeerStatus.Running)
                    ClientNetwork.NetworkIncomingMessage();
                //else if (ClientNetwork.NetworkClient.Status == Lidgren.Network.NetPeerStatus.Running && ClientNetwork.isConnected == true)
                    //ClientVessel.SendVessel();
            //}
        }

        void OnApplicationQuit()
        {
            if (ClientNetwork.isConnected == true)
                ClientNetwork.Shutdown();

            //ClientNetwork.NetworkIncomingMessageThread.Abort();
        }

        public static void StartGame()
        {
            string SaveDirectory = Path.Combine(KSPUtil.ApplicationRootPath, Path.Combine("saves", "L.O.G"));
            ConfigNode Node = ConfigNode.Load(Path.Combine(SaveDirectory, "persistent.sfs"));

            HighLogic.CurrentGame = new Game(Node);
            HighLogic.CurrentGame.Mode = Game.Modes.SANDBOX;
            HighLogic.CurrentGame.Title = "L.O.G.";
            HighLogic.CurrentGame.Description = "L.O.G. - Multiplayer";
            HighLogic.CurrentGame.startScene = GameScenes.SPACECENTER;
            HighLogic.CurrentGame.CrewRoster = KerbalRoster.GenerateInitialCrewRoster();
            Planetarium.SetUniversalTime(0.0);

            if (HighLogic.CurrentGame.Mode != Game.Modes.SANDBOX)
                HighLogic.CurrentGame.Parameters.Difficulty.AllowStockVessels = true;

            HighLogic.CurrentGame.additionalSystems = new ConfigNode();
            HighLogic.CurrentGame.additionalSystems.AddNode("MESSAGESYSTEM");

            HighLogic.CurrentGame.flightState = new FlightState();
            HighLogic.CurrentGame.CrewRoster.ValidateAssignments(HighLogic.CurrentGame);

            HighLogic.SaveFolder = "L.O.G";
            GamePersistence.SaveGame(HighLogic.CurrentGame, "persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            HighLogic.CurrentGame.Start();

            GameStarted = true;
        }

        private void CheckFiles()
        {
            string SaveDirectory = Path.Combine(KSPUtil.ApplicationRootPath, Path.Combine("saves", "L.O.G"));
            CheckIfDirectoryExist(SaveDirectory);

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LOG.Client.Resources.DefaultSaveGame.sfs"))
            using (StreamReader reader = new StreamReader(resourceStream))
                File.WriteAllText(Path.Combine(SaveDirectory, "persistent.sfs"), reader.ReadToEnd());

            CheckIfDirectoryExist(Path.Combine(SaveDirectory, "Ships"));
            CheckIfDirectoryExist(Path.Combine(SaveDirectory, Path.Combine("Ships", "VAB")));
            CheckIfDirectoryExist(Path.Combine(SaveDirectory, Path.Combine("Ships", "SPH")));

            if (File.Exists(Path.Combine(KSPUtil.ApplicationRootPath, "client_log.txt")) == true)
            {
                File.Delete(Path.Combine(KSPUtil.ApplicationRootPath, "client_log.txt"));
                File.Create(Path.Combine(KSPUtil.ApplicationRootPath, "client_log.txt"));
            }
        }

        private void CheckIfDirectoryExist(string Path)
        {
            if (Directory.Exists(Path))
                return;

            Directory.CreateDirectory(Path);
        }
    }
}
