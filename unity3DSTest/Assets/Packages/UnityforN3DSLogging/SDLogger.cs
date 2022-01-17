#if DEVELOPMENT_BUILD

using System;
using System.Runtime.InteropServices;

namespace N3DSLogger
{

    public static class SDLogger
    {
        public enum SD_Log_Error
        {
            SUCCESS = 0, //< No error
            ERROR_NO_SD_CARD_FOUND = 1, //< No working SD card detected. Perhaps broken card inserted, or nothing inserted
            ERROR_BAD_SD_CARD_FORMAT = 2, //< Card formatted wrong (FAT16, NTFS etc)
            ERROR_SD_CARD_WRITE_LOCKED = 3, //< Card Write Lock switch is on
            ERROR_MUST_RESET_AND_REINSERT = 4, //< Hardware or driver error, possibly empty adapter card inserted. Must power cycle and reinsert card
            ERROR_UNKNOWN_TRY_AGAIN = 5, //< No idea what's wrong
            ERROR_ALREADY_MOUNTED = 6, //< Trying to mount when already mounted. Happens if Unity mounted SD card already
            ERROR_ACCESS_DENIED = 7, //< Couldn't access output log file, maybe it's set to read-only somehow?
            ERROR_SD_CARD_REMOVED = 8, //< Card was removed after we started out okay
            ERROR_OUT_OF_SPACE = 9, //< No space left on SD card for this file, or can't increase file size
            ERROR_OUTPUT_DIRECTORY_NOT_FOUND = 10, //< We tried to create output file in a directory that doesn't exist
            ERROR_FILE_ALREADY_EXISTS = 11, //< Output log file already exists with the same name. This shouldn't happen since we try to delete it first
            ERROR_TEXT_POINTER_NULL = 12, //< Argument to SD_Log_Write function had null text pointer
            ERROR_TEXT_LENGTH_NEGATIVE = 13,  //< Argument to SD_Log_Write function had negative text length
            ERROR_LOGGER_ALREADY_INITIALIZED = 14, //< SD_Log_Initialize() was called again without being finalized
            ERROR_LOGGER_NOT_YET_INITIALIZED = 15, //< SD_Log_Write() was called without the logger being initialized
            ERROR_LOGGER_ALREADY_FINALIZED = 16  //< SD_Log_Finalize() was called again without being initialized
        };

        [DllImport("__Internal")]
        private static extern int SD_Log_Initialize([MarshalAs(UnmanagedType.LPWStr)]string fileName);

        [DllImport("__Internal")]
        private static extern int SD_Log_Write([MarshalAs(UnmanagedType.LPWStr)]string text, int length_wchars);

        [DllImport("__Internal")]
        private static extern int SD_Log_Finalize();

        public static SD_Log_Error Initialize()
        {
            string fileName = "sdmcwo:/UnityLog_" + DateTime.Now.ToString("MMddyyyy_HHmmss") + ".log";
            SD_Log_Error result = (SD_Log_Error)SD_Log_Initialize(fileName);

            // if a file with this exact same timestamp exists due to an improperly set system time, try a new name a few times
            int numFailures = 0;
            while (result == SD_Log_Error.ERROR_FILE_ALREADY_EXISTS && numFailures < 10)
            {
                numFailures++;
                result = (SD_Log_Error)SD_Log_Initialize(fileName + "_" + numFailures.ToString());
            }

            return result;
        }

        public static SD_Log_Error Deactivate()
        {
            return (SD_Log_Error)SD_Log_Finalize();
        }

        public static SD_Log_Error Log(string logString)
        {
            return (SD_Log_Error)SD_Log_Write(logString, logString.Length);
        }

        public static string GetErrorMessage(SD_Log_Error error)
        {
            switch (error)
            {
                case SD_Log_Error.SUCCESS: //< No error
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", No error has occurred.";
                    }
                case SD_Log_Error.ERROR_NO_SD_CARD_FOUND: //< No working SD card detected. Perhaps broken card inserted, or nothing inserted
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", No SD card is detected. Make sure an SD card is properly inserted into your kit.";
                    }
                case SD_Log_Error.ERROR_BAD_SD_CARD_FORMAT: //< Card formatted wrong (FAT16, NTFS etc)
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The SD card is not formatted correctly. Please try formatting your SD card to FAT32.";
                    }
                case SD_Log_Error.ERROR_SD_CARD_WRITE_LOCKED: //< Card Write Lock switch is on
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The write lock switch on the SD card is enabled. Please remove the SD card and set the write lock switch to the unlocked position.";
                    }
                case SD_Log_Error.ERROR_MUST_RESET_AND_REINSERT: //< Hardware or driver error, possibly empty adapter card inserted. Must power cycle and reinsert card
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", An error occurred. Please turn off the kit, remove the SD card, reinsert the SD card, and power the kit back on."
                            + "If this doesn't fix the issue, please post this error on the forums.";
                    }
                case SD_Log_Error.ERROR_UNKNOWN_TRY_AGAIN: //< No idea what's wrong
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", An unknown error occurred. If this error message persists, please post on the forums.";
                    }
                case SD_Log_Error.ERROR_ALREADY_MOUNTED: //< Trying to mount when already mounted. Happens if Unity mounted SD card already
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The SD card has already been mounted. This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_ACCESS_DENIED: //< Couldn't access output log file, maybe it's set to read-only somehow?
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", Access was denied to the log file. Please post this error on the forums.";
                    }
                case SD_Log_Error.ERROR_SD_CARD_REMOVED: //< Card was removed after we started out okay
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The SD Card was removed. Please reinsert the card.";
                    }
                case SD_Log_Error.ERROR_OUT_OF_SPACE: //< No space left on SD card for this file, or can't increase file size
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The SD card is out of space. Please free up space or insert a different SD card.";
                    }
                case SD_Log_Error.ERROR_OUTPUT_DIRECTORY_NOT_FOUND: //< We tried to create output file in a directory that doesn't exist
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The log output directory does not exist. This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_FILE_ALREADY_EXISTS: //< Output log file already exists with the same name. This shouldn't happen since we try to delete it first
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", A log file with this name already exists. This error should not occur. If it does, please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_TEXT_POINTER_NULL: //< Argument to SD_Log_Write function had null text pointer
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", Log message is null. This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_TEXT_LENGTH_NEGATIVE:  //< Argument to SD_Log_Write function had negative text length
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", The log length is negative. This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_LOGGER_ALREADY_INITIALIZED: //< SD_Log_Initialize() was called again without being finalized
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_LOGGER_NOT_YET_INITIALIZED: //< SD_Log_Write() was called without the logger being initialized
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                case SD_Log_Error.ERROR_LOGGER_ALREADY_FINALIZED:  //< SD_Log_Finalize() was called again without being initialized
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
                default:
                    {
                        return "SD Log Error: " + error.ToString()
                            + ", This is not a valid error number. This indicates an error in our SD Logging implementation. Please post the error on the forums.";
                    }
            }
        }

    }

}

#endif // DEVELOPMENT_BUILD
