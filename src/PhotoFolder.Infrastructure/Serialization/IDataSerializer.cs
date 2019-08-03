namespace PhotoFolder.Infrastructure.Serialization
{
    public interface IDataSerializer
    {
        T Deserialize<T>(string value);
        string Serialize(object obj);
    }
}
