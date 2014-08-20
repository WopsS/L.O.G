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

        void print(string String)
 
        {

            log.Add(String);
            if(log.Count > maxLogMessages)
                log.RemoveAt(0);
        }

        void Start()
        {
            log.Add("Alpha Build 1.0");
        }

        private List<string> log = new List<string>();
        int maxLogMessages = 200;
        bool visible = true;
        string stringToEdit = "";
        bool selectTextfield = true;
        private Vector2 scrollPos = new Vector2(0, 0);
        private int lastLogLen = 0;
        GUIStyle printGUIStyle;
        float maxLogLabelHeight = 100.0f;
        private void OnGUI()
        {
            if (visible)
            {
                GUI.SetNextControlName("chatWindow");
                stringToEdit = GUI.TextField(new Rect(0, Screen.height - 50, 200, 20), stringToEdit, 25);
                Debug.LogWarning(stringToEdit);
                if (!selectTextfield)
                {
                    GUI.FocusControl("chatWindow");

                }

                float logBoxWidth = 180;
                float[] logBoxHeights = new float[log.Count];
                float totalHeight = 0;
                int i = 0;

                foreach (string String in log)
                {

                    float logBoxHeight = Mathf.Min(maxLogLabelHeight, printGUIStyle.CalcHeight(new GUIContent(String), logBoxWidth));
                    logBoxHeights[i++] = logBoxHeight;
                    totalHeight += logBoxHeight + 10;
                }

                float innerScrollHeight = totalHeight;

                // if there's a new message, automatically scroll to bottom
                if (lastLogLen != log.Count)
                {
                    scrollPos = new Vector2(0, innerScrollHeight);
                    lastLogLen = log.Count;
                }

                scrollPos = GUI.BeginScrollView(new Rect(0, Screen.height - 150 - 50, 200, 150), scrollPos, new Rect(0, 0, 180, innerScrollHeight));

                float currY = 0;
                i = 0;

                foreach (string String in log)
                {

                    float logBoxHeight = logBoxHeights[i++];
                    GUI.Label(new Rect(10, currY, logBoxWidth, logBoxHeight), String, printGUIStyle);
                    currY += logBoxHeight + 10;
                }

                GUI.EndScrollView();
            }
        }

        void Update()
        {

            if (Input.GetKeyDown("return"))
            {
                if (selectTextfield)
                {
                    selectTextfield = !selectTextfield;
                }
                if (stringToEdit != "")
                {
                    log.Add("" + stringToEdit);
                    stringToEdit = "";
                }
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