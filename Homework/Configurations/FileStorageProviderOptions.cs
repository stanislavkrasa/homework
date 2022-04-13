namespace Homework.Configurations
{
    public class FileStorageProviderOptions : IStorageProviderOptions
    {
        public const string ConfigurationSectionName = "FileStorageProvider";

        public string BaseLocation { get; set; } = "";
    }
}
