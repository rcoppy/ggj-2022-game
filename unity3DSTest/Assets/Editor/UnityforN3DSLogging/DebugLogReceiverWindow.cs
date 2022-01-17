using UnityEngine;
using UnityEditor;
using System.Net;
using System.Linq;

using System.Threading;     // Thread
using System.Collections.Generic;               // Exception

namespace N3DSLogReceiver
{

    public class DebugLogReceiverWindow : EditorWindow
    {
        const int MAX_WIDTH_CHECKBOX = 140;
        const int MAX_WIDTH_BUTTON = 200;

        [MenuItem("N3DS Logging/Open Network Log Receiver")]
        static void Init()
        {
            DebugLogReceiverWindow window = (DebugLogReceiverWindow)EditorWindow.GetWindow(typeof(DebugLogReceiverWindow), false, "Log Receiver");
            window.Show();
        }

        void OnGUI()
        {
            UdpDebuggerWindow();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public static int Tabs(string[] Options, int Selected)
        {
            const float DarkGray = 0.4f;
            const float LightGray = 0.9f;
            const float StartSpace = 10.0f;

            GUILayout.Space(StartSpace);
            Color storeColor = GUI.backgroundColor;
            Color highlightCol = new Color(LightGray, LightGray, LightGray);
            Color bgCol = new Color(DarkGray, DarkGray, DarkGray);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.bottom = 8;

            GUILayout.BeginHorizontal();
            {
                // Create a row of buttons
                for (int i = 0; i < Options.Length; ++i)
                {
                    GUI.backgroundColor = i == Selected ? bgCol : highlightCol;
                    if (GUILayout.Button(Options[i], buttonStyle))
                    {
                        Selected = i; // Tab click
                    }
                }
            }
            GUILayout.EndHorizontal();

            // Restore Color
            GUI.backgroundColor = storeColor;

            return Selected;
        }

        public void Disconnect()
        {
            client.Disconnect();
            m_IsConnected = false;
        }

        public void EstablishedTcpConnection()
        {

        }

        public void UdpDebuggerWindow()
        {
            GUILayout.BeginHorizontal();
            if (!m_IsConnected)
            {
                if (GUILayout.Button("Connect"))
                {
                    m_IsConnected = true;
                    if (client == null)
                    {
                        client = new N3DSLogReceiver.ChatClient();
                    }
                    client.Setup();
                    Thread chatRec = new Thread(new ThreadStart(ChatReceiveThread));
                    chatRec.Start();
                }
            }

            if (m_IsConnected)
            {
                if (GUILayout.Button("Disconnect!"))
                {
                    Disconnect();
                }
            }

            if (client != null)
            {
                List<IPAddress> addresses = client.m_ChatSessions.ListOfChatSessions();

                if (addresses.Count > 0)
                {

                    if (GUILayout.Button("Save Current Log"))
                    {
                        string saveLocation = EditorUtility.SaveFilePanel("Save Log File", "", "", "txt");
                        if (!string.IsNullOrEmpty(saveLocation))
                        {
                            client.m_ChatSessions.PrintChatLogForAddress(saveLocation, addresses[m_SelectedTab]);
                        }
                    }

                    if (GUILayout.Button("Clear Current Log"))
                    {
                        if (EditorUtility.DisplayDialog("Clear Current Log?", "Are you sure you want to clear the current log?", "Yes", "Cancel"))
                        {
                            client.m_ChatSessions.ClearLogForAddress(addresses[m_SelectedTab]);
                        }
                    }

                    if (GUILayout.Button("Clear All"))
                    {
                        if (EditorUtility.DisplayDialog("Clear All Logs?", "Are you sure you want to clear the logs for all connected 3DS devices?", "Yes", "Cancel"))
                        {
                            client.m_ChatSessions.ClearAllLogs();
                            m_SelectedTab = 0;
                        }
                    }
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            m_ScrollToBottom = GUILayout.Toggle(m_ScrollToBottom, "Auto Scroll", GUILayout.MaxWidth(MAX_WIDTH_CHECKBOX));
            m_HideProgressConsole = GUILayout.Toggle(m_HideProgressConsole, "Hide Progress Log", GUILayout.MaxWidth(MAX_WIDTH_CHECKBOX));
            GUILayout.EndHorizontal();

            if (!m_HideProgressConsole)
            {
                N3DSLogReceiver.LogConsole.Display();
            }

            LogWindow();
            if (m_ScrollToBottom)
            {
                //float total = scrollPosition.magnitude;
                scrollPosition.y = float.MaxValue;
            }
        }

        public void LogWindow()
        {
            if (client != null)
            {
                List<IPAddress> addresses = client.m_ChatSessions.ListOfChatSessions();

                if (addresses.Count > 0)
                {
                    string[] tabs = addresses.Select(x => x.ToString()).ToArray();
                    m_SelectedTab = Tabs(tabs, m_SelectedTab);

                    List<string> log = client.m_ChatSessions.ReturnChatLogForAddress(addresses[m_SelectedTab]);

                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                    foreach (var line in log)
                    {
                        GUILayout.Label(line);
                    }
                    GUILayout.EndScrollView();
                }
            }
        }

        void ChatReceiveThread()
        {
            N3DSLogReceiver.LogConsole.Status = "Opening Listen Thread";
            client.ReceiveMessageThread();
            Disconnect();
            N3DSLogReceiver.LogConsole.Status = "Closing Listen Thread";
        }

        private bool m_IsConnected = false;
        public Vector2 scrollPosition;
        public int m_SelectedTab = 0;
        public bool m_ScrollToBottom = false;
        public bool m_HideProgressConsole = false;
        private N3DSLogReceiver.ChatClient client;
    }

}
