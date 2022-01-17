#if DEVELOPMENT_BUILD

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace N3DSLogger
{

    public class NetworkLogger
    {
        const int NN_ERROR_OFFSET = -100;
        public enum Network_Log_Error
        {
            E2BIG = -1,
            EACCES = -2,
            EADDRINUSE = -3,
            EADDRNOTAVAIL = -4,
            EAFNOSUPPORT = -5,
            EAGAIN = -6,                //!< You must try again, because a non-blocking control was performed or a time-out occurred. (This error is the same as <tt>EWOULDBLOCK</tt>.)
            EALREADY = -7,              //!< Already connected.
            EBADF = -8,                 //!< Invalid socket descriptor.
            EBADMSG = -9,
            EBUSY = -10,                //!< Busy.  Socket is blocked on another call (Probably Recieve.)
            ECANCELED = -11,
            ECHILD = -12,
            ECONNABORTED = -13,         //!< Connection interrupted.
            ECONNREFUSED = -14,         //!< Connection refused.
            ECONNRESET = -15,           //!< Connection has been reset. 
            EDEADLK = -16,
            EDESTADDRREQ = -17,
            EDOM = -18,
            EDQUOT = -19,
            EEXIST = -20,
            EFAULT = -21,
            EFBIG = -22,
            EHOSTUNREACH = -23,
            EIDRM = -24,
            EILSEQ = -25,
            EINPROGRESS = -26,          //!< Connection has not yet completed.
            EINTR = -27,                //!< Suspended.
            EINVAL = -28,
            EIO = -29,
            EISCONN = -30,
            EISDIR = -31,
            ELOOP = -32,
            EMFILE = -33,               //!< Cannot create any more socket descriptors.			/* Initialize return case when creating sockets.  Someone else already initialized libraby most likely and there is no room for another socket */
            EMLINK = -34,
            EMSGSIZE = -35,             //!< The data is too large to send.			/* DebugServer_Send return case; The max possible was sent though */
            EMULTIHOP = -36,
            ENAMETOOLONG = -37,
            ENETDOWN = -38,             //!< The relevant instance is down.			/* Initialize return case */
            ENETRESET = -39,            //!< The socket library has not been initialized.			/* Initialize return case */
            ENETUNREACH = -40,
            ENFILE = -41,
            ENOBUFS = -42,              //!< Insufficient resources.
            ENODATA = -43,
            ENODEV = -44,
            ENOENT = -45,
            ENOEXEC = -46,
            ENOLCK = -47,
            ENOLINK = -48,
            ENOMEM = -49,               //!< Insufficient memory.			/* Initialize return case */
            ENOMSG = -50,
            ENOPROTOOPT = -51,
            ENOSPC = -52,
            ENOSR = -53,
            ENOSTR = -54,
            ENOSYS = -55,
            ENOTCONN = -56,             //!< Not connected.
            ENOTDIR = -57,
            ENOTEMPTY = -58,
            ENOTSOCK = -59,
            ENOTSUP = -60,
            ENOTTY = -61,
            ENXIO = -62,
            EOPNOTSUPP = -63,
            EOVERFLOW = -64,
            EPERM = -65,
            EPIPE = -66,
            EPROTO = -67,
            EPROTONOSUPPORT = -68,
            EPROTOTYPE = -69,
            ERANGE = -70,
            EROFS = -71,
            ESPIPE = -72,
            ESRCH = -73,
            ESTALE = -74,
            ETIME = -75,
            ETIMEDOUT = -76,            //!< Timeout.
            ETXTBSY = -77,
            EWOULDBLOCK = EAGAIN,       //!< Cannot execute until the requested operation is blocked. (This error is the same as <tt>EAGAIN</tt>.) /* Posix.1g */
            EXDEV = -78,

            /* The following are possible return values from call DebugServer_Send. */
            E_INVALID_MESSAGE_INDEX = -79,  // Requested to send a message which we no longer have
            E_NULL_MESSAGE = -80,   // Requested to send bytes > 0 with a null message
            E_EXCESIVE_DATA_SEND_RATE = -81,    // Requesting to send a message with a full unsent send buffer.
            E_NOT_INITIALIZED = -82,    // The UdpDebugPlugin was never initialized.
            E_NEGATIVE_SIZE = -83,  // Requesting to send a negative size.


            SUCCESS = 0,  //!< Succeeded.

            DESCRIPTION_INVALID_SELECTION = NN_ERROR_OFFSET - 23,           //!< An invalid value was specified (when a specifiable value is discrete).
            DESCRIPTION_TOO_LARGE = NN_ERROR_OFFSET - 22,                   //!< The value is too large.
            DESCRIPTION_NOT_AUTHORIZED = NN_ERROR_OFFSET - 21,              //!< Unauthorized operation.
            DESCRIPTION_ALREADY_DONE = NN_ERROR_OFFSET - 20,            //!< The internal status has already been specified.
            DESCRIPTION_INVALID_SIZE = NN_ERROR_OFFSET - 19,                //!< Invalid size.
            DESCRIPTION_INVALID_ENUM_VALUE = NN_ERROR_OFFSET - 18,          //!< The value is outside the range for enum values.
            DESCRIPTION_INVALID_COMBINATION = NN_ERROR_OFFSET - 17,         //!< Invalid parameter combination.
            DESCRIPTION_NO_DATA = NN_ERROR_OFFSET - 16,                     //!< No data.
            DESCRIPTION_BUSY = NN_ERROR_OFFSET - 15,                    //!< Could not be run because another process was already being performed.
            DESCRIPTION_MISALIGNED_ADDRESS = NN_ERROR_OFFSET - 14,          //!< Invalid address alignment.
            DESCRIPTION_MISALIGNED_SIZE = NN_ERROR_OFFSET - 13,             //!< Invalid size alignment.
            DESCRIPTION_OUT_OF_MEMORY = NN_ERROR_OFFSET - 12,           //!< Insufficient memory.			/* Initialize return case */
            DESCRIPTION_NOT_IMPLEMENTED = NN_ERROR_OFFSET - 11,             //!< Not yet implemented.
            DESCRIPTION_INVALID_ADDRESS = NN_ERROR_OFFSET - 10,             //!< Invalid address.
            DESCRIPTION_INVALID_POINTER = NN_ERROR_OFFSET - 9,              //!< Invalid pointer.
            DESCRIPTION_INVALID_HANDLE = NN_ERROR_OFFSET - 8,               //!< Invalid handle.
            DESCRIPTION_NOT_INITIALIZED = NN_ERROR_OFFSET - 7,          //!< Not initialized.			/* Initialize return case */
            DESCRIPTION_ALREADY_INITIALIZED = NN_ERROR_OFFSET - 6,      //!< Already initialized.			/* Initialize return case */
            DESCRIPTION_NOT_FOUND = NN_ERROR_OFFSET - 5,                    //!< The object does not exist.
            DESCRIPTION_CANCEL_REQUESTED = NN_ERROR_OFFSET - 4,             //!< Request canceled.
            DESCRIPTION_ALREADY_EXISTS = NN_ERROR_OFFSET - 3,           //!< The object already exists.
            DESCRIPTION_OUT_OF_RANGE = NN_ERROR_OFFSET - 2,                 //!< The value is outside of the defined range.
            DESCRIPTION_TIMEOUT = NN_ERROR_OFFSET - 1,                  //!< The process timed out.
            DESCRIPTION_INVALID_RESULT_VALUE = NN_ERROR_OFFSET - 0          //!< These values are not used.
        }

        [DllImport("__Internal")]
        private static extern int DebugServer_Initialize();

        [DllImport("__Internal")]
        private static extern int DebugServer_Send(IntPtr text, int length_wchars);

        [DllImport("__Internal")]
        private static extern int DebugServer_Finalize();

        public static Network_Log_Error Initialize()
        {
            return (Network_Log_Error)DebugServer_Initialize();
        }

        public static Network_Log_Error Deactivate()
        {
            return (Network_Log_Error)DebugServer_Finalize();
        }

        public static Network_Log_Error Log(string logString)
        {
            Network_Log_Error result;
            byte[] logBytes = Encoding.UTF8.GetBytes(logString);
            IntPtr ptr = Marshal.AllocHGlobal(logBytes.Length);
            try
            {
                Marshal.Copy(logBytes, 0, ptr, logBytes.Length);
                result = (Network_Log_Error)DebugServer_Send(ptr, logBytes.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return result;
        }

        public static string GetErrorMessage(Network_Log_Error error)
        {
            switch (error)
            {
                case Network_Log_Error.SUCCESS:
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", No error has occurred.";
                    }
                case Network_Log_Error.EAGAIN: //!< You must try again, because a non-blocking control was performed or a time-out occurred. (This error is the same as EWOULDBLOCK.)
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Internal error caused from congestion usually due to high send and receive calls.  Try sending less.";
                    }
                case Network_Log_Error.EALREADY: //!< Already connected.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Already initialized and connected.  May be caused from initializing twice.";
                    }
                case Network_Log_Error.EBADF: //!< Invalid socket descriptor.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Internal error which shouldn't accure since you are not setting up the socket.  If this accures, must be from all 8 sockets already being accounted for.";
                    }
                case Network_Log_Error.EBUSY: //!< Busy.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Caused from too much data being sent and received.  Most likely station is sending 3DS lots of resend requests due to poor internet connectivity.";
                    }
                case Network_Log_Error.ECONNABORTED: //!< Connection interrupted.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", The network was requested to shut down by the user.";
                    }
                case Network_Log_Error.ECONNREFUSED: //!< Connection refused.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.ECONNRESET: //!< Connection has been reset.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.EINPROGRESS: //!< Connection has not yet completed.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.EINTR: //!< Suspended.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.EMFILE: //!< Cannot create any more socket descriptors.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", All available sockets in use.  Someone else already initialized the library most likely and there is no room for another socket.";
                    }
                case Network_Log_Error.EMSGSIZE: //!< The data is too large to send.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", The data is too large to send.  Please keep data under 149100 bytes. The max possible was sent though.";
                    }
                case Network_Log_Error.ENETDOWN: //!< The relevant instance is down.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.ENETRESET: //!< The socket library has not been initialized.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", The socket library wasn't initialized.  Try initializing first.";
                    }
                case Network_Log_Error.ENOBUFS: //!< Insufficient resources.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't happen.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.ENOMEM: //!< Insufficient memory.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", There wasn't enough memory available for networking.  Need more space.";
                    }
                case Network_Log_Error.ENOTCONN: //!< Not connected.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.ETIMEDOUT: //!< Timeout.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This shouldn't be returned.  Please post to the NDP forum if it does.";
                    }
                case Network_Log_Error.DESCRIPTION_ALREADY_DONE: //!< The internal status has already been specified.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", The network was already set up with different values.  Try not initializing the network outside of this debug logger";
                    }
                case Network_Log_Error.DESCRIPTION_BUSY: //!< Could not be run because another process was already being performed.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Could not be run because another process was already being performed.";
                    }
                case Network_Log_Error.DESCRIPTION_OUT_OF_MEMORY: //!< Insufficient memory.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Not enough memory to initialize.  Free more memory.";
                    }
                case Network_Log_Error.DESCRIPTION_NOT_INITIALIZED: //!< Not initialized.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Not initialized.  Please initialize first.";
                    }
                case Network_Log_Error.DESCRIPTION_ALREADY_INITIALIZED: //!< Already initialized.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Already initialized.  Try initializing only once.";
                    }
                case Network_Log_Error.DESCRIPTION_ALREADY_EXISTS: //!< The object already exists.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", This object already exits... most likely a socket or library already created.";
                    }
                case Network_Log_Error.DESCRIPTION_TIMEOUT: //!< The process timed out.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Timed out.  The resources are possibly being hogged.";
                    }
                case Network_Log_Error.DESCRIPTION_INVALID_RESULT_VALUE: //!< These values are not used.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", These values are not used.  This should not happen.";
                    }
                case Network_Log_Error.E_INVALID_MESSAGE_INDEX:  // Requested to send a message which we no longer have
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", A request was recieved to send a message which we no longer have. Can happen with older messages or when there is to much data being sent.  Check log for missing message.";
                    }
                case Network_Log_Error.E_NULL_MESSAGE:   // Requested to send bytes > 0 with a null message
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Requested to send bytes with a null pointer to the data.  Dont do this.";
                    }
                case Network_Log_Error.E_EXCESIVE_DATA_SEND_RATE:    // Requesting to send a message with a full unsent send buffer.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", Messages are being sent to quickly.  Try packing messages together and sending them once per frame.";
                    }
                case Network_Log_Error.E_NOT_INITIALIZED:    // The UdpDebugPlugin was never initialized.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", You can not send messages without initializing first.";
                    }
                case Network_Log_Error.E_NEGATIVE_SIZE: // Requesting to send a negative size.
                    {
                        return "Network Log Error: " + error.ToString()
                                + ", You cannot send a message with a negative size.";
                    }
                default:
                    {
                        return "Network Log Error: An unexpected error, which you should post onto the NDP forum. The error is " + error.ToString();
                    }
            }
        }
    }

}

#endif // DEVELOPMENT_BUILD
