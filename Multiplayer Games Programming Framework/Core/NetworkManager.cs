using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Multiplayer_Games_Programming_Packet_Library;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.GameCode.Components;
using System.Security.Cryptography.X509Certificates;
using Multiplayer_Games_Programming_Framework.GameCode;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;

namespace Multiplayer_Games_Programming_Framework.Core
{
    internal class NetworkManager
    {
        private static NetworkManager Instance;

        public static NetworkManager m_Instance
        {
            get
            {
                if (Instance == null)
                {
                    return Instance = new NetworkManager();
                }

                return Instance;
            }
        }

        private TcpClient m_TcpClient;
        private NetworkStream m_Stream;
        private StreamReader m_Reader;
        private StreamWriter m_Writer;

        public Dictionary<int, Action<Vector2>> m_PlayerPositions;
        public Dictionary<int, Action<PlayerState>> m_PlayerStates;

        public PlayerNetworkController m_PlayerController;

        public int m_ID { get; private set; }
        public Vector2 m_Position { get; private set; }

        NetworkManager()
        {
            m_TcpClient = new TcpClient();
        }

        public bool Connect(string ip, int port)
        {
            try
            {
                m_TcpClient.Connect(ip, port);
                m_Stream = m_TcpClient.GetStream();
                m_Reader = new StreamReader(m_Stream, Encoding.UTF8);
                m_Writer = new StreamWriter(m_Stream, Encoding.UTF8);
                m_PlayerPositions = new Dictionary<int, Action<Vector2>>();
                m_PlayerStates = new Dictionary<int, Action<PlayerState>>();

                Run();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        public void Run()
        {
            Thread TcpServerResponseProcessing = new Thread(() => TcpProcessServerResponse());
            TcpServerResponseProcessing.Name = "TCP Reader";
            TcpServerResponseProcessing.Start();
        }

        private void TcpProcessServerResponse()
        {
            try
            {
                while (m_TcpClient.Connected)
                {
                    string message = m_Reader.ReadLine();

                    if (message == null)
                    {
                        continue;
                    }

                    Packet packet = Packet.Deserialise(message);

                    if (packet == null)
                    {
                        continue;
                    }

                    switch (packet.m_Type)
                    {
                        case PacketType.MESSAGE:
                            {
                                MessagePacket messagePacket = (MessagePacket)packet;
                                Debug.WriteLine(messagePacket.m_Message);
                                break;
                            }
                        case PacketType.ID:
                            {
                                IDPacket IDPacket = (IDPacket)packet;
                                Debug.WriteLine("ID: " + IDPacket.m_ID);
                                m_ID = IDPacket.m_ID;
                                break;
                            }
                        case PacketType.POSITION:
                            {
                                PositionPacket positionPacket = (PositionPacket)packet;

                                if (m_PlayerPositions.ContainsKey(positionPacket.m_ID))
                                {
                                    m_PlayerPositions[positionPacket.m_ID]?.Invoke(new Vector2(positionPacket.m_Position.X, positionPacket.m_Position.Y));
                                }
                                break;
                            }
                        case PacketType.STATE:
                            {
                                StatePacket statePacket = (StatePacket)packet;

                                if (m_PlayerStates.ContainsKey(statePacket.m_ID))
                                {
                                    m_PlayerStates[statePacket.m_ID]?.Invoke(statePacket.m_State);
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void TCPSendMessage(Packet packet)
        {
            try
            {
                string message = packet.ToJson();

                m_Writer.WriteLine(message);
                m_Writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Login()
        {
            Packet packet = new LoginPacket();
            
            TCPSendMessage(packet);
        }
    }
}
