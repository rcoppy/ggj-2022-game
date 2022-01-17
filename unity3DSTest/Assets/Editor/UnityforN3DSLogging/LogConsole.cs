using UnityEngine;
using System.Collections.Generic;

namespace N3DSLogReceiver
{

    public class LogConsole
    {
        public static void Display()
        {
            lock (s_ConsoleLock)
            {
                if (s_Console != null)
                {
                    for (int i = 1; i <= s_Size; ++i)
                    {
                        int index = s_Console.Count - i;
                        if (index < 0)
                        {
                            break;
                        }
                        string line = s_Console[index];
                        GUILayout.Label(line);
                    }
                }
            }

        }

        public static void Clear()
        {
            lock (s_ConsoleLock)
            {
                s_Console.Clear();
            }
        }

        public static string Status
        {
            set
            {
                lock (s_ConsoleLock)
                {
                    if (s_Console == null)
                    {
                        s_Console = new List<string>();
                    }
                }

                s_Console.Add(value);
                Debug.Log(value);
            }
        }
        private static List<string> s_Console;
        private static int s_Size = 4;
        private static object s_ConsoleLock = new object();
    }

}
