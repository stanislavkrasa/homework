using Homework.Configurations;
using Microsoft.Extensions.Options;
using System.Text;

namespace Homework.StorageProviders
{
    public class FileStorageProvider : IStorageProvider
    {
        private readonly FileStorageProviderOptions _options;

        public FileStorageProvider(IOptions<FileStorageProviderOptions> options)
        {
            _options = options.Value;
        }

        public Task Delete(string filename)
        {
            File.Delete(Path.Combine(_options.BaseLocation, filename));

            return Task.CompletedTask;
        }

        public Task<byte[]> ReadFromBytes(string filename)
        {
            FileStream sourceStream = File.Open(Path.Combine(_options.BaseLocation, filename), FileMode.Open);
            var reader = new BinaryReader(sourceStream);
            var content = reader.ReadBytes(Convert.ToInt32(new FileInfo(filename).Length));
            reader.Close();

            return Task.FromResult(content);
        }

        public async Task<byte[]> ReadFromString(string filename)
        {
            FileStream sourceStream = File.Open(Path.Combine(_options.BaseLocation, filename), FileMode.Open);
            var reader = new StreamReader(sourceStream);
            var content = Encoding.UTF8.GetBytes(await reader.ReadToEndAsync());
            reader.Close();

            return content;
        }

        public Task Write(string filename, string content)
        {
            var file = File.Open(Path.Combine(_options.BaseLocation, filename), FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(file);
            writer.Write(content);
            writer.Close();

            return Task.CompletedTask;
        }

        public Task Write(string filename, byte[] content)
        {
            var file = File.Open(Path.Combine(_options.BaseLocation, filename), FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(file);
            writer.Write(content);
            writer.Close();

            return Task.CompletedTask;
        }
    }
}
