using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Client
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]

    class ClientMain : MonoBehaviour
    {
        public static bool GameStarted = false;

        public void Awake()
        {
            SetupDirectories();
        }

        private void Main()
        {
            string[] Arguments = Environment.GetCommandLineArgs(), Host, Port;

            if (Array.Exists(Arguments, element => element.StartsWith("-IP=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Port=")) == true)
            {
                Host = Arguments[1].Split('=');
                Port = Arguments[2].Split('=');

                if (Host.Length > 0 && Port.Length > 0)
                    if ((GameScenes)Application.loadedLevel == GameScenes.MAINMENU && GameStarted == false)
                        ClientNetwork.Connect(Host[1], Convert.ToInt32(Port[1]));

                //ScreenMessages.PostScreenMessage("ma-ta", 5f, ScreenMessageStyle.UPPER_LEFT);
            }
        }

        void OnApplicationQuit()
        {
            if (ClientNetwork.isConnected == true)
                ClientNetwork.Shutdown();
        }

        public static void StartGame()
        {
            HighLogic.CurrentGame = new Game();
            HighLogic.CurrentGame.Mode = Game.Modes.SANDBOX;
            HighLogic.CurrentGame.Title = "L.O.G.";
            HighLogic.CurrentGame.Description = "L.O.G. - Multiplayer";
            HighLogic.CurrentGame.startScene = GameScenes.SPACECENTER;
            HighLogic.CurrentGame.CrewRoster = KerbalRoster.GenerateInitialCrewRoster();
            Planetarium.SetUniversalTime(0.0);

            HighLogic.CurrentGame.additionalSystems = new ConfigNode();
            HighLogic.CurrentGame.additionalSystems.AddNode("MESSAGESYSTEM");

            HighLogic.CurrentGame.flightState = new FlightState();

            GamePersistence.SaveGame(HighLogic.CurrentGame, "persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            HighLogic.CurrentGame.Start();

            GameStarted = true;
        }

        private void SetupDirectories()
        {
            string SaveDirectory = Path.Combine(KSPUtil.ApplicationRootPath, Path.Combine("saves", "L.O.G"));
            CheckIfDirectoryExist(SaveDirectory);

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Client.Resources.DefaultSaveGame.sfs"))
            using (StreamReader reader = new StreamReader(resourceStream))
                File.WriteAllText(Path.Combine(SaveDirectory, "persistent.sfs"), reader.ReadToEnd());

            CheckIfDirectoryExist(Path.Combine(SaveDirectory, "Ships"));
            CheckIfDirectoryExist(Path.Combine(SaveDirectory, Path.Combine("Ships", "VAB")));
            CheckIfDirectoryExist(Path.Combine(SaveDirectory, Path.Combine("Ships", "SPH")));
        }

        private void CheckIfDirectoryExist(string Path)
        {
            if (Directory.Exists(Path))
                return;

            Directory.CreateDirectory(Path);
        }
    }
}
