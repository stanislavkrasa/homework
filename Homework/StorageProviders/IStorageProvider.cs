namespace Homework.StorageProviders
{
    public interface IStorageProvider
    {
        Task<byte[]> ReadFromString(string filename);
        Task<byte[]> ReadFromBytes(string filename);
        Task Write(string filename, string content);
        Task Write(string filename, byte[] content);
        Task Delete(string filename);
    }
}
