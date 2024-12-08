using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Server
{
	internal class ConnectedClient
	{
		public int m_ID;
        Socket m_Socket;
		NetworkStream m_Stream;
		StreamReader m_Reader;
		StreamWriter m_Writer;

        public ConnectedClient(Socket socket, int id)
		{
			m_ID = id;
			m_Socket = socket;
            m_Stream = new NetworkStream(socket, false);
			m_Reader = new StreamReader(m_Stream, Encoding.UTF8);
			m_Writer = new StreamWriter(m_Stream, Encoding.UTF8);
		}

		public void Close()
		{
			m_Socket.Close();
		}

        public Packet? Read()
		{
			string? message = m_Reader.ReadLine();

			if (message == null)
			{
				return null;
			}

            return Packet.Deserialise(message);
        }
        public void Send(Packet packet)
        {
			string message = packet.ToJson();

            m_Writer.WriteLine(message);
            m_Writer.Flush();
        }
    }
}
