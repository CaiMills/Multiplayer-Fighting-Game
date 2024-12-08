using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Numerics;

namespace Multiplayer_Games_Programming_Packet_Library
{
	public enum PacketType
	{
		MESSAGE,
        ID,
        LOGIN,
        POSITION,
        STATE,
        HITBOX,
	}

    public enum PlayerState
    {
        IDLE,
        ATTACKING,
        DEAD,
    };

    public abstract class Packet
	{
		[JsonPropertyName("Type")]
		public PacketType m_Type { get; set; }

        [JsonPropertyName("ID")]
        public int m_ID;

        public virtual void PrintPacketType()
		{
			Console.WriteLine("Packet Type: {0}");
		}

        /// <summary>
        /// used to convert the object to a JSON formatted string
        /// </summary>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new PacketConverter() },
                IncludeFields = true,
            };
            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// used to convert a JSON formatted string to an object of type Animal
        /// </summary>
        public static Packet? Deserialise(string json)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new PacketConverter() },
                IncludeFields = true,
            };
            return JsonSerializer.Deserialize<Packet>(json, options);
        }
    }

	public class MessagePacket : Packet
	{
		[JsonPropertyName("Message")]
		public string? m_Message;

        public MessagePacket()
        {
            m_Type = PacketType.MESSAGE;
        }

        public MessagePacket(string message)
        {
            m_Type = PacketType.MESSAGE;
            m_Message = message;
        }

        public override void PrintPacketType()
        {
            Console.WriteLine("Packet Type: {0}, Message: {1}", m_Type, m_Message);
        }
    }

    public class IDPacket : Packet
    {
        public IDPacket()
        {
            m_Type = PacketType.ID;
        }

        public IDPacket(int ID)
        {
            m_Type = PacketType.ID;
            m_ID = ID;
        }

        public override void PrintPacketType()
        {
            Console.WriteLine("Packet Type: {0}, Client Id: {1}", m_Type, m_ID);
        }
    }

    public class LoginPacket : Packet
    {
        public LoginPacket()
        {
            m_Type = PacketType.LOGIN;
        }
    }

    public class PositionPacket : Packet
    {
        [JsonPropertyName("Position")]
        public Vector2 m_Position;

        public PositionPacket()
        {
            m_Type = PacketType.POSITION;
        }

        public PositionPacket(int ID, Vector2 position)
        {
            m_Type = PacketType.POSITION;
            m_ID = ID;
            m_Position = position;
        }

        public override void PrintPacketType()
        {
            Console.WriteLine("Packet Type: {0}, Client Id: {1}, Position X: {2}, Position Y: {3}", m_Type, m_ID, m_Position.X, m_Position.Y);
        }
    }

    public class StatePacket : Packet
    {
        [JsonPropertyName("State")]
        public PlayerState m_State;

        public StatePacket()
        {
            m_Type = PacketType.STATE;
        }

        public StatePacket(int ID, PlayerState State)
        {
            m_Type = PacketType.STATE;
            m_ID = ID;
            m_State = State;
        }

        public override void PrintPacketType()
        {
            Console.WriteLine("Packet Type: {0}, Client Id: {1}, Player State: {2}", m_Type, m_ID, m_State);
        }
    }

    // This is a custom converter
    // We need to use this to allow the JSON to be deserialised to a base class and then cast to a derrived class
    // This is because the JSON does not contain the type information
    // We use this in the options when serialising and deserialising
    class PacketConverter : JsonConverter<Packet>
	{
		public override Packet? Read(ref Utf8JsonReader reader, Type typeToCovert, JsonSerializerOptions options)
		{
			using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				var root = doc.RootElement;

				if (root.TryGetProperty("Type", out var typeProperty))
				{
					if (typeProperty.GetByte() == (byte)PacketType.MESSAGE)
					{
						return JsonSerializer.Deserialize<MessagePacket>(root.GetRawText(), options);
					}
                    if (typeProperty.GetByte() == (byte)PacketType.ID)
                    {
                        return JsonSerializer.Deserialize<IDPacket>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.LOGIN)
                    {
                        return JsonSerializer.Deserialize<LoginPacket>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.POSITION)
                    {
                        return JsonSerializer.Deserialize<PositionPacket>(root.GetRawText(), options);
                    }
                    if (typeProperty.GetByte() == (byte)PacketType.STATE)
                    {
                        return JsonSerializer.Deserialize<StatePacket>(root.GetRawText(), options);
                    }
                }
			}
            throw new JsonException("Unknown type");
        }

        public override void Write(Utf8JsonWriter writer, Packet value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}