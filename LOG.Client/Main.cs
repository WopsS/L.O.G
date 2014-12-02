using LOG.Client.Networking.Managers;
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
        // TODO: Chat messages.

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this);

            CheckFiles();
        }

        private void Main()
        {
            string[] Arguments = Environment.GetCommandLineArgs();

            if (Array.Exists(Arguments, element => element.StartsWith("-IP=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Port=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Username=")) == true)
            {
                string Host, Port;

                Host = Arguments[1].Split('=')[1];
                Port = Arguments[2].Split('=')[1];
                PlayerManager.Username = Arguments[3].Split('=')[1];

                if (Host.Length > 0 && Port.Length > 0)
                {
                    ClientNetwork.Initialize(Host, Port);
                }
            }
        }

        public void Update()
        {
            if (PlayerManager.isConnected == true && PlayerManager.GameStarted == false && HighLogic.LoadedScene == GameScenes.MAINMENU)
                StartGame();

        }

        public void FixedUpdate()
        {

        }

        void OnApplicationQuit()
        {
            if (PlayerManager.isConnected == true)
                ClientNetwork.Shutdown();

            if (ClientNetwork.NetworkIMThread.IsAlive == true)
                ClientNetwork.NetworkIMThread.Abort();

            if (ClientNetwork.NetworkMessagesThread.IsAlive == true)
                ClientNetwork.NetworkMessagesThread.Abort();
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

            PlayerManager.GameStarted = true;
        }

        /// <summary>
        /// Check if necessary files exist.
        /// </summary>
        private void CheckFiles()
        {
            CheckIfDirectoryExist(Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "L.O.G"), "logs"));

            string SaveDirectory = Path.Combine(KSPUtil.ApplicationRootPath, Path.Combine("saves", "L.O.G"));
            CheckIfDirectoryExist(SaveDirectory);

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LOG.Client.Utilities.Resources.DefaultSaveGame.sfs"))
            using (StreamReader reader = new StreamReader(resourceStream))
                File.WriteAllText(Path.Combine(SaveDirectory, "persistent.sfs"), reader.ReadToEnd());

            CheckIfDirectoryExist(Path.Combine(SaveDirectory, "Ships"));
            CheckIfDirectoryExist(Path.Combine(SaveDirectory, Path.Combine("Ships", "VAB")));
            CheckIfDirectoryExist(Path.Combine(SaveDirectory, Path.Combine("Ships", "SPH")));
        }

        /// <summary>
        /// Check if a specific directory exist.
        /// </summary>
        /// <param name="Path">Path to the directory.</param>
        private void CheckIfDirectoryExist(string Path)
        {
            if (Directory.Exists(Path))
                return;

            Directory.CreateDirectory(Path);
        }
    }
}
