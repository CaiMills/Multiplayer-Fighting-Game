using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using Multiplayer_Games_Programming_Packet_Library;
using System.Text;
using System.Linq.Expressions;
using System.Numerics;

namespace Multiplayer_Games_Programming_Server
{
	internal class Server
	{
        private readonly TcpListener m_TcpListener;

        //ConcurrentDictionarys help prevent Race Conditions from happening
        private readonly ConcurrentDictionary<int, ConnectedClient> m_Clients = new();

        public Server(string ipAddress, int port)
        {
            IPAddress iP = IPAddress.Parse(ipAddress);
            m_TcpListener = new TcpListener(iP, port);

            //ConcurrentDictionarys are built to help protect against race conditions
            m_Clients = new ConcurrentDictionary<int, ConnectedClient>();
        }

        public void Start()
        {
            try
            {
                int index = 0;
                m_TcpListener.Start();
                Console.WriteLine("Server is listening");

                while (true)
                {
                    Socket socket = m_TcpListener.AcceptSocket();
                    Console.WriteLine($"Connection made from {socket.RemoteEndPoint}");

                    int clientId = index++; 
                    Console.WriteLine("ID - " + clientId);

                    //will fail if key id is already in use
                    m_Clients.TryAdd(clientId, new ConnectedClient(socket, clientId));

                    Thread client = new Thread(() => ClientMethod(clientId));
                    client.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            m_TcpListener.Stop();
            Console.WriteLine("Server has stopped listening");
        }

        private void ClientMethod(int ID)
        {
            try
            {
                Packet? packet;

                while ((packet = m_Clients[ID].Read()) != null)
                {
                    switch(packet.m_Type)
                    {
                        case PacketType.MESSAGE:
                            MessagePacket messagePacket = (MessagePacket)packet;
                            Console.WriteLine("Recieved Message - " + messagePacket.m_Message);
                            m_Clients[ID].Send(new MessagePacket("Logged in"));
                            break;
                        case PacketType.LOGIN:
                            m_Clients[ID].Send(new IDPacket(ID));
                            break;
                        case PacketType.POSITION:
                            PositionPacket positionPacket = (PositionPacket)packet;

                            foreach (ConnectedClient client in m_Clients.Values)
                            {
                                //sends the position packet to every client except the one where the packet originated from (only really good for one or two players)
                                if (client.m_ID != ID)
                                {
                                    client.Send(positionPacket);
                                }
                            }
                            break;
                        case PacketType.STATE:
                            StatePacket statePacket = (StatePacket)packet;

                            foreach (ConnectedClient client in m_Clients.Values)
                            {
                                if (client.m_ID != ID)
                                {
                                    client.Send(statePacket);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message); 
            }
            finally
            {
                m_Clients[ID].Close();
            }
        }
    }
}
