using System.Net;
using System.Net.Sockets;
using System.Text;                  // Encoding
using System;                       // Exception
using System.Collections.Generic;
using System.IO;
using System.Threading;             // List

namespace N3DSLogReceiver
{

    static class DEBUG_CLIENT_CONSTANTS
    {
        public const int BUFFER_LENGTH = 1500;
        public const int MAX_HISTORY = 100;
        public const byte HEADER_PATTERN = 0xDB;
    };

    public class SocketMessage
    {
        public SocketMessage(string Message, int SequenceNumber)
        {
            m_Message = Message;
            m_SequenceNumber = SequenceNumber;
        }
        public int m_SequenceNumber;
        public string m_Message;
    }

    public class SocketMessages
    {
        public SocketMessages(SocketMessage Message, IPAddress Address)
        {
            m_LastOrderedSequence = -1;
            m_Messages = new SortedList<int, string>();
            m_MissedMessages = new List<int>();
            m_Address = new IPAddress(Address.GetAddressBytes());
            AddMessage(Message);
        }
        public void AddMessage(SocketMessage Message)
        {
            try
            {
                m_Messages.Add(Message.m_SequenceNumber, Message.m_Message);
            }
            catch (Exception)
            {
                // We added an object with same key so it failed, but that's ok.
            }

            // Did we get a new most recent message?
            if (m_HighestSequenceAdded < Message.m_SequenceNumber)
            {
                m_HighestSequenceAdded = Message.m_SequenceNumber;
            }

            // Is this the next sorted message?
            if (m_LastOrderedSequence + 1 == Message.m_SequenceNumber)
            {
                m_LastOrderedSequence = Message.m_SequenceNumber;
            }

            // Is there no chance of getting the next ordered message at this point?
            if (m_LastOrderedSequence < m_HighestSequenceAdded - DEBUG_CLIENT_CONSTANTS.MAX_HISTORY + 1)
            {
                m_LastOrderedSequence = m_HighestSequenceAdded - DEBUG_CLIENT_CONSTANTS.MAX_HISTORY + 1;
            }
        }

        public List<int> GetObtainableMissedMessages()
        {
            m_MissedMessages = new List<int>();
            // If we're currently caught up, there's no point checking.
            if (m_LastOrderedSequence == m_HighestSequenceAdded)
            {
                return m_MissedMessages;
            }

            // All the messages we received.
            var recMsgs = m_Messages.Keys;

            int i = m_LastOrderedSequence;
            // Add missed slots in messages
            foreach (int key in recMsgs)
            {
                if (i < key)
                {
                    //if(m_MissedMessages.Count > 20)
                    //{
                    //    return m_MissedMessages;
                    //}
                    ++i;
                    while (i < key)
                    {
                        m_MissedMessages.Add(i);
                        ++i;
                    }
                }
            }
            return m_MissedMessages;
        }

        public bool IsInMissedMessageList(int Sequence)
        {
            return m_MissedMessages.Contains(Sequence) ? true : false;
        }

        public List<int> m_MissedMessages;
        public SortedList<int, string> m_Messages;
        private int m_LastOrderedSequence;
        private int m_HighestSequenceAdded;
        public IPAddress m_Address;
    }

    public class MessageLog
    {
        public MessageLog()
        {
            m_Log = new List<SocketMessages>();
        }

        // Add a Udp Message we Received to the Log
        public void AddMessage(string Message, int SequenceNumber, IPAddress Address)
        {
            lock (m_LogLock)
            {
                SocketMessage udpMsg = new SocketMessage(Message, SequenceNumber);
                SocketMessages msg = m_Log.Find(m => m.m_Address.Equals(Address));
                if (msg != null)
                {
                    msg.AddMessage(udpMsg);
                }
                else
                {
                    SocketMessages newMsgs = new SocketMessages(udpMsg, Address);
                    m_Log.Add(newMsgs);
                    LogConsole.Status = ("New 3DS Message Log Created from: " + Address.ToString());
                }
            }
        }

        // Return a list of all our stored Sessions
        public List<IPAddress> ListOfChatSessions()
        {
            lock (m_LogLock)
            {
                List<IPAddress> sessions = new List<IPAddress>();
                foreach (var msgs in m_Log)
                {
                    sessions.Add(msgs.m_Address);
                }
                return sessions;
            }
        }

        public List<string> ReturnChatLogForAddress(IPAddress Address)
        {
            lock (m_LogLock)
            {
                SocketMessages msgs = m_Log.Find(m => m.m_Address.Equals(Address));

                List<string> lines = new List<string>(msgs.m_Messages.Count);

                foreach (var key in msgs.m_Messages.Keys)
                {
                    lines.Add(key + " : " + msgs.m_Messages[key]);
                }
                return lines;
            }
        }

        public void ClearAllLogs()
        {
            lock (m_LogLock)
            {
                m_Log.Clear();
            }
        }

        public void PrintChatLogForAddress(string filePath, IPAddress Address)
        {
            SocketMessages msgs = m_Log.Find(m => m.m_Address.Equals(Address));

            try
            {
                List<string> lines = new List<string>(msgs.m_Messages.Count);

                foreach (var key in msgs.m_Messages.Keys)
                {
                    lines.Add(key + " : " + msgs.m_Messages[key]);
                }

                File.WriteAllLines(filePath, lines.ToArray());
                LogConsole.Status = "Printed log to file: " + filePath;
            }
            catch (IOException ex)
            {
                LogConsole.Status = ex.Message;
            }

        }

        public SocketMessages GetMessagesForAddress(IPAddress Address)
        {
            lock (m_LogLock)
            {
                return m_Log.Find(m => m.m_Address.Equals(Address));
            }
        }

        public void ClearLogForAddress(IPAddress Address)
        {
            lock (m_LogLock)
            {
                SocketMessages msgs = m_Log.Find(m => m.m_Address.Equals(Address));
                msgs.m_Messages.Clear();
            }
        }
        private object m_LogLock = new object();
        public List<SocketMessages> m_Log;
    }

    public class ChatClient : PrintWrapper
    {

        // Use this for initialization
        public void Setup()
        {
            m_Listening = true;

            m_MissedBuffer = new byte[9];
            m_MissedBuffer[0] = DEBUG_CLIENT_CONSTANTS.HEADER_PATTERN;
            BitConverter.GetBytes(9).CopyTo(m_MissedBuffer, 1);

            if (m_ChatSessions == null)
            {
                m_ChatSessions = new MessageLog();
            }
        }

        public void Disconnect()
        {
            m_Listening = false;
            m_Client.Close();
        }

        public void ReceiveMessageThread()
        {
            // Receiving message from broadcast
            //client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_Client = new UdpClient(m_Port);
            m_AllAddress = new IPEndPoint(IPAddress.Any, m_Port);
            //m_UdpSocket = new Socket(m_AllAddress.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //m_UdpSocket.Blocking = false;
            //m_Client = new UdpClie
            string received_data;
            byte[] receive_byte_array; // = new byte[DEBUG_CLIENT_CONSTANTS.BUFFER_LENGTH];
            int headerSize = sizeof(byte) + sizeof(int) * 2;

            LogConsole.Status = ("Starting to Listen for messages.");

            // If we're connect to UDP
            while (m_Listening)
            {
                try
                {
                    if (m_Client.Available > 0)
                    {
                        //receive_byte_array = m_Client.Receive(ref m_AllAddress);
                        receive_byte_array = m_Client.Receive(ref m_AllAddress);

                        // Is the message even big enough
                        if (receive_byte_array.Length >= headerSize)
                        {
                            if (receive_byte_array[0] == DEBUG_CLIENT_CONSTANTS.HEADER_PATTERN)
                            {
                                int lengthCheck = BitConverter.ToInt32(receive_byte_array, 1);
                                if (receive_byte_array.Length == lengthCheck)
                                {
                                    int sequence = BitConverter.ToInt32(receive_byte_array, sizeof(int) + 1);
                                    int length = receive_byte_array.Length - headerSize;
                                    received_data = Encoding.UTF8.GetString(receive_byte_array, headerSize, length);
                                    //Debug.Log("Message: " + received_data);
                                    m_ChatSessions.AddMessage(received_data, sequence, m_AllAddress.Address);
                                    if (ShouldRequestMissedMessages(m_AllAddress, sequence))
                                    {
                                        RequestMissedMessages(m_AllAddress);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Interrupted) // WSACancelBlockingCall
                    {
                        // We're closing the client.
                        LogConsole.Status = ("Udp socket closed.");
                    }
                }
                catch (Exception e)
                {
                    LogConsole.Status = ("Caught Exception: " + e.ToString());
                }
            }
        }

        public void RequestMessage(int Sequence, IPEndPoint Address)
        {
            BitConverter.GetBytes(Sequence).CopyTo(m_MissedBuffer, 5);
            m_Client.Send(m_MissedBuffer, 9, Address);
        }

        public void RequestMissedMessages(IPEndPoint Address)
        {
            SocketMessages msgs = m_ChatSessions.GetMessagesForAddress(Address.Address);
            List<int> missed = msgs.GetObtainableMissedMessages();
            foreach (int miss in missed)
            {
                RequestMessage(miss, Address);
            }
        }

        public bool ShouldRequestMissedMessages(IPEndPoint Address, int Sequence)
        {
            SocketMessages msgs = m_ChatSessions.GetMessagesForAddress(Address.Address);
            return msgs.IsInMissedMessageList(Sequence) ? false : true;
        }

        public bool AttemptConnect(IPAddress Address)
        {
            return false;
        }

        public void CloseUdp()
        {
            m_Client.Close();
            m_Listening = false;
        }

        UdpClient m_Client;
        IPEndPoint m_AllAddress;
        const int m_Port = 23456;

        bool m_Listening = true;
        byte[] m_MissedBuffer;

        public MessageLog m_ChatSessions;
    }

}
