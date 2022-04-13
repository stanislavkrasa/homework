using Homework.Configurations;
using Homework.Convertors;
using Homework.Convertors.Formats;
using Homework.SmtpClients;
using Homework.StorageProviders;

namespace Homework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConfigurationOptions(this IServiceCollection services)
        {
            services.AddOptions<FileStorageProviderOptions>().BindConfiguration(FileStorageProviderOptions.ConfigurationSectionName);
            services.AddOptions<SmtpClientWrapperOptions>().BindConfiguration(SmtpClientWrapperOptions.ConfigurationSectionName);
        }

        public static void AddDependecies(this IServiceCollection services)
        {
            services.AddSingleton<IFormatConvertorResolver, FormatConvertorResolver>();
            services.AddSingleton<IFileManager, FileManager>();
            services.AddSingleton<IStorageProvider, FileStorageProvider>();
            services.AddSingleton<ISmtpClient, SmtpClientWrapper>();
        }

        public static void AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient(FileManager.FileManagerHttpClientName);
        }
    }
}
