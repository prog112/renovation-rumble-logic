namespace RenovationRumble.Logic.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string json);

        byte[] SerializeBytes<T>(T data);
        T DeserializeBytes<T>(byte[] bytes);
    }
}