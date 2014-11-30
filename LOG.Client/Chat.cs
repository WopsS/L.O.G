using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LOG.Client
{
    class ClientChat : MonoBehaviour
    {
        public static Rect windowRect = new Rect(200, 200, 300, 450);
        public static string messBox = "", messageToSend = "", user = "";

        public static void windowFunc(int id)
        {
            GUILayout.Box(messBox, GUILayout.Height(350), GUILayout.Width(200));

            GUILayout.BeginHorizontal();
            messageToSend = GUILayout.TextField(messageToSend);
            if (GUILayout.Button("Send", GUILayout.Width(75)))
            {
                messBox += user + ": " + messageToSend + "\n";
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("User:");
            user = GUILayout.TextField(user);

            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }
    }
}
