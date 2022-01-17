using UnityEngine;
using System.Text;

namespace N3DSLogger
{

    public class LogManager : MonoBehaviour
    {
        // the logging method that the user wants to use
        public LoggingMethod loggingMethod = LoggingMethod.None;

        public enum LoggingMethod
        {
            None,
            LogToScreen,
            LogToSD,
            LogOverNetwork
        }

#if DEVELOPMENT_BUILD

        private class ScreenLogHelper
        {
            public string[] LoggedStrings
            {
                get
                {
                    return loggedStrings;
                }
            }

            public int StartIndex
            {
                get
                {
                    // if the calculated start index is negative, add the length to it to get the correct non-negative index
                    return ((currentIndex - 1) < 0) ? (currentIndex - 1 + loggedStrings.Length) : (currentIndex - 1);
                }
            }

            public int EndIndex
            {
                get
                {
                    return currentIndex;
                }
            }

            public bool LogDirty { get; set; }

            // list of all of the currently registered strings for display
            // this is treated as a circular array
            private string[] loggedStrings;
            private int currentIndex = 0;

            public ScreenLogHelper(int logSize)
            {
                loggedStrings = new string[logSize];
                LogDirty = true;
            }

            public void StoreLog(string newLog)
            {
                loggedStrings[currentIndex] = newLog;
                currentIndex = (currentIndex + 1) % loggedStrings.Length;
                LogDirty = true;
            }
        }

        // the logging method currently being used
        // may not match the logging method the user requested if errors are encountered
        private LoggingMethod currentLoggingMethod = LoggingMethod.None;

        private const int DebugLogSize = 17;
        private const int ErrorLogSize = 2;

        private ScreenLogHelper debugLogs = new ScreenLogHelper(DebugLogSize);
        private ScreenLogHelper errorLogs = new ScreenLogHelper(ErrorLogSize);

        private StringBuilder guiDisplayText = new StringBuilder();

        // ensure that we only ever have one logger alive
        private static LogManager instance;

        #region MonoBehavior implementation
        void Awake()
        {
            // ensure only one log master is alive
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            // keep this object from being destroyed on level load
            DontDestroyOnLoad(gameObject);

            // register the log handler
            Application.logMessageReceived += HandleLogEvent;
        }

        void Start()
        {
            bool success = InitializeLogging(loggingMethod);

            if (!success)
            {
                if (loggingMethod != LoggingMethod.LogToScreen)
                {
                    // Try to initialize screen logging. If this doesn't work, there's nothing else we can do.
                    success = InitializeLogging(LoggingMethod.LogToScreen);
                    if (success)
                    {
                        errorLogs.StoreLog("Switching to screen logging. To use another logging method, fix the printed error and restart the application.\n");
                    }
                }
            }

            // if this isn't a debug build, show a warning
            if (!Debug.isDebugBuild)
            {
                errorLogs.StoreLog("This application is not a Development build. This GameObject and the native plugins for SD and "
                          + "Network logging must be removed from the build before submission or your application will fail Lotcheck.\n");
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                DeactivateLogging();
            }
        }

        void OnGUI()
        {
            if (debugLogs.LogDirty || errorLogs.LogDirty)
            {
                RefreshLogDisplay();
            }

            GUI.Label(new Rect(5.0f, 0.0f, 310, 240), guiDisplayText.ToString());
        }
        #endregion

        #region Logging management functions
        private void HandleLogEvent(string logString, string stackTrace, LogType logType)
        {
            bool success = HandleLog(logString);

            if (!success)
            {
                if (currentLoggingMethod != LoggingMethod.LogToScreen)
                {
                    // Try to initialize screen logging. If this doesn't work, there's nothing else we can do.
                    success = InitializeLogging(LoggingMethod.LogToScreen);
                    if (success)
                    {
                        errorLogs.StoreLog("Switching to screen logging. To use another logging method, fix the printed error and restart the application.\n");

                        // now that the issue is sidestepped, rehandle the original message
                        HandleLog(logString);
                    }
                }
            }
        }

        private bool HandleLog(string logString)
        {
            switch (currentLoggingMethod)
            {
                case LoggingMethod.LogToScreen:
                    {
                        debugLogs.StoreLog(logString + "\n");
                        return true;
                    }
                case LoggingMethod.LogToSD:
                    {
                        SDLogger.SD_Log_Error result = SDLogger.Log(logString + "\n");
                        if (result != SDLogger.SD_Log_Error.SUCCESS)
                        {
                            errorLogs.StoreLog(SDLogger.GetErrorMessage(result));
                            return false;
                        }

                        return true;
                    }
                case LoggingMethod.LogOverNetwork:
                    {
                        NetworkLogger.Network_Log_Error result = NetworkLogger.Log(logString + "\n");
                        if (result != NetworkLogger.Network_Log_Error.SUCCESS)
                        {
                            errorLogs.StoreLog(NetworkLogger.GetErrorMessage(result));
                            return false;
                        }

                        return true;
                    }
                case LoggingMethod.None:
                    {
                        // when set to none, do nothing
                        return true;
                    }
                default:
                    {
                        errorLogs.StoreLog("LogManager.HandleLog Error: Invalid logging method specified.\n");
                        return false;
                    }
            }
        }

        /// <summary>
        /// Initializes one of the 3DS logging methods.
        /// </summary>
        /// <param name="newLoggingMethod">The logging method to initialize.</param>
        /// <returns>true for successful initialization, false for unsuccessful initialization.</returns>
        private bool InitializeLogging(LoggingMethod newLoggingMethod)
        {
            switch (newLoggingMethod)
            {
                case LoggingMethod.LogToScreen:
                    {
                        // no initialization to do
                        break;
                    }
                case LoggingMethod.LogToSD:
                    {
                        SDLogger.SD_Log_Error result = SDLogger.Initialize();
                        if (result != SDLogger.SD_Log_Error.SUCCESS)
                        {
                            errorLogs.StoreLog(SDLogger.GetErrorMessage(result));
                            return false;
                        }

                        break;
                    }
                case LoggingMethod.LogOverNetwork:
                    {
                        NetworkLogger.Network_Log_Error result = NetworkLogger.Initialize();
                        if (result != NetworkLogger.Network_Log_Error.SUCCESS)
                        {
                            errorLogs.StoreLog(NetworkLogger.GetErrorMessage(result));
                            return false;
                        }

                        break;
                    }
                case LoggingMethod.None:
                    {
                        // when set to none, do nothing
                        break;
                    }
                default:
                    {
                        errorLogs.StoreLog("LogManager.InitializeLogging Error: Invalid logging method specified.\n");
                        return false;
                    }
            }

            currentLoggingMethod = newLoggingMethod;
            return true;
        }

        private bool DeactivateLogging()
        {
            switch (currentLoggingMethod)
            {
                case LoggingMethod.LogToScreen:
                    {
                        return true;
                    }
                case LoggingMethod.LogToSD:
                    {
                        SDLogger.SD_Log_Error result = SDLogger.Deactivate();
                        if (result != SDLogger.SD_Log_Error.SUCCESS)
                        {
                            errorLogs.StoreLog(SDLogger.GetErrorMessage(result));
                            return false;
                        }

                        break;
                    }
                case LoggingMethod.LogOverNetwork:
                    {
                        NetworkLogger.Network_Log_Error result = NetworkLogger.Deactivate();
                        if (result != NetworkLogger.Network_Log_Error.SUCCESS)
                        {
                            errorLogs.StoreLog(NetworkLogger.GetErrorMessage(result));
                            return false;
                        }

                        break;
                    }
                case LoggingMethod.None:
                    {
                        // when set to none, do nothing
                        break;
                    }
                default:
                    {
                        errorLogs.StoreLog("LogManager.DeactivateLogging Error: Invalid logging method specified.\n");
                        return false;
                    }
            }

            currentLoggingMethod = LoggingMethod.None;
            return true;
        }
        #endregion

        #region Screen logging functions
        private bool DeactivateScreenLogging()
        {
            debugLogs = new ScreenLogHelper(DebugLogSize);
            errorLogs = new ScreenLogHelper(ErrorLogSize);

            return true;
        }

        private bool RefreshLogDisplay()
        {
            guiDisplayText.Remove(0, guiDisplayText.Length);

            int currentIndex;
            // always display errors on top
            // display oldest errors first
            for (int count = 0; count < errorLogs.LoggedStrings.Length; ++count)
            {
                currentIndex = (errorLogs.EndIndex + count) % errorLogs.LoggedStrings.Length;
                guiDisplayText.Append(errorLogs.LoggedStrings[(currentIndex)]);
            }

            // display non-error logs
            // display newest logs first
            for (int count = 0; count < debugLogs.LoggedStrings.Length; ++count)
            {
                currentIndex = debugLogs.StartIndex - count;
                if (currentIndex < 0)
                {
                    currentIndex = debugLogs.LoggedStrings.Length + currentIndex;
                }

                guiDisplayText.Append(debugLogs.LoggedStrings[(currentIndex)]);
            }

            errorLogs.LogDirty = false;
            debugLogs.LogDirty = false;

            return true;
        }
        #endregion

#endif // DEVELOPMENT_BUILD
    }

}
