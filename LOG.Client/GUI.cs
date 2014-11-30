using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LOG.Client
{
    class ClientGUI
    {
        public static Rect WindowsPosition;

        private static string ServerIP = "127.0.0.1";
        private static string ServerPort = "4198";

        public static void ConnectGUI(int windowID)
        {
            GUIStyle WindowStyle = new GUIStyle(GUI.skin.button);
            WindowStyle.normal.textColor = WindowStyle.focused.textColor = Color.white;
            WindowStyle.hover.textColor = WindowStyle.active.textColor = Color.yellow;
            WindowStyle.onNormal.textColor = WindowStyle.onFocused.textColor = WindowStyle.onHover.textColor = WindowStyle.onActive.textColor = Color.green;
            WindowStyle.padding = new RectOffset(2, 2, 2, 2);

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("IP:", GUILayout.MinWidth(50));
            ServerIP = GUILayout.TextField(ServerIP, 15, GUILayout.MinWidth(90));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Port:", GUILayout.MinWidth(50));
            ServerPort = GUILayout.TextField(ServerPort, 15, GUILayout.MinWidth(90));

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Connect", WindowStyle, GUILayout.ExpandWidth(true)))
            {
                //ClientNetwork.Connect(ServerIP.Trim(), ServerPort.Trim());
                Thread.Sleep(10);
                OnGUI();
            }

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public static void DisconnectGUI(int windowID)
        {
            GUIStyle WindowStyle = new GUIStyle(GUI.skin.button);
            WindowStyle.normal.textColor = WindowStyle.focused.textColor = Color.white;
            WindowStyle.hover.textColor = WindowStyle.active.textColor = Color.yellow;
            WindowStyle.onNormal.textColor = WindowStyle.onFocused.textColor = WindowStyle.onHover.textColor = WindowStyle.onActive.textColor = Color.green;
            WindowStyle.padding = new RectOffset(2, 2, 2, 2);

            GUILayout.BeginVertical();

            if (GUILayout.Button("Disconnect", WindowStyle, GUILayout.ExpandWidth(true)))
            {
                //ClientNetwork.Shutdown();
                OnGUI();
            }

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 10));
        }

        public static void OnGUI()
        {
            GUI.skin = HighLogic.Skin;

            if (ClientNetwork.isConnected == false)
                WindowsPosition = GUILayout.Window(1, WindowsPosition, ConnectGUI, "L.O.G. Connect", GUILayout.MinWidth(200));
            else
                WindowsPosition = GUILayout.Window(1, WindowsPosition, DisconnectGUI, "L.O.G.", GUILayout.MinWidth(200));
        }
    }
}
