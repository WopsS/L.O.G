using LOG.API.IO;
using LOG.API.Models;
using LOG.MasterServer.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LOG.Client
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ClientMain : MonoBehaviour
    {
        private PlayerModel m_playerModel = new PlayerModel();
        private ClientNetwork m_clientNetwork = null;

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this);

            string LOGSaveDirectory = Path.Combine(KSPUtil.ApplicationRootPath, Path.Combine("saves", "L.O.G"));

            Directories.CheckDirectory(Path.Combine(Path.Combine(KSPUtil.ApplicationRootPath, "L.O.G"), "logs"));
            Directories.CheckDirectory(LOGSaveDirectory);

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LOG.Client.Resources.DefaultSaveGame.sfs"))
            {
                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    File.WriteAllText(Path.Combine(LOGSaveDirectory, "persistent.sfs"), reader.ReadToEnd());
                }
            }

            Directories.CheckDirectory(Path.Combine(LOGSaveDirectory, "Ships"));
            Directories.CheckDirectory(Path.Combine(LOGSaveDirectory, Path.Combine("Ships", "VAB")));
            Directories.CheckDirectory(Path.Combine(LOGSaveDirectory, Path.Combine("Ships", "SPH")));
        }

        private void Main()
        {
            string[] Arguments = Environment.GetCommandLineArgs();

            if (Array.Exists(Arguments, element => element.StartsWith("-IP=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Port=")) == true && Array.Exists(Arguments, element => element.StartsWith("-Username=")) == true)
            {
                string Host, Port;

                Host = Arguments[1].Split('=')[1];
                Port = Arguments[2].Split('=')[1];
                this.m_playerModel.Username = Arguments[3].Split('=')[1];

                if (Host.Length > 0 && Port.Length > 0)
                {
                    this.m_clientNetwork = new ClientNetwork(this.m_playerModel, Host, Convert.ToInt32(Port));
                }
            }
        }

        public void Update()
        {
            if (this.m_playerModel.IsConnected == true && this.m_playerModel.GameStarted == false && HighLogic.LoadedScene == GameScenes.MAINMENU)
                this.StartGame();
        }

        public void FixedUpdate()
        {
            this.m_clientNetwork.Update();
        }

        void OnApplicationQuit()
        {
            if (this.m_playerModel.IsConnected == true)
                this.m_clientNetwork.Dispose();
        }

        public void StartGame()
        {
            string SaveDirectory = Path.Combine(KSPUtil.ApplicationRootPath, Path.Combine("saves", "L.O.G"));
            ConfigNode Node = ConfigNode.Load(Path.Combine(SaveDirectory, "persistent.sfs"));

            HighLogic.CurrentGame = new Game(Node);
            HighLogic.CurrentGame.Mode = Game.Modes.SANDBOX;
            HighLogic.CurrentGame.Title = "L.O.G.";
            HighLogic.CurrentGame.Description = "L.O.G. - Multiplayer";
            HighLogic.CurrentGame.startScene = GameScenes.SPACECENTER;
            HighLogic.CurrentGame.CrewRoster = KerbalRoster.GenerateInitialCrewRoster(Game.Modes.SANDBOX);
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

            this.m_playerModel.GameStarted = true;
        }
    }
}
