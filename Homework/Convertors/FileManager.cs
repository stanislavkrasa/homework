using Homework.Convertors.Formats;
using Homework.Models;
using Homework.SmtpClients;
using Homework.StorageProviders;
using System.Text;
using File = Homework.Models.File;

namespace Homework.Convertors
{
    public class FileManager : IFileManager
    {
        private readonly IFormatConvertorResolver _formatConvertorResolver;
        private readonly IStorageProvider _storageProvider;
        private readonly HttpClient _httpClient;
        private readonly ISmtpClient _smtpClient;

        public const string FileManagerHttpClientName = "FileManagerHttpClient";

        public FileManager(
            IFormatConvertorResolver resolver, 
            IStorageProvider storageProvider, 
            IHttpClientFactory httpClientFactory,
            ISmtpClient smtpClient
        )
        {
            _formatConvertorResolver = resolver;
            _storageProvider = storageProvider;
            _httpClient = httpClientFactory.CreateClient(FileManagerHttpClientName);
            _smtpClient = smtpClient;
        }

        public async Task<IFile> ConvertFileFromApi(
            byte[] content, Type typeOfConvertableDocument, FileFormat inputFormat, FileFormat outputFormat)
            => await CreateConvertedFileDTO(String.Empty, content, typeOfConvertableDocument, inputFormat, outputFormat);

        public async Task<IFile> ConvertFileFromUrl(string url, Type typeOfConvertableDocument, FileFormat inputFormat, FileFormat outputFormat)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException();

                byte[] content;
                var inputFormater = await _formatConvertorResolver.Resolve(inputFormat);
                switch (await inputFormater.GetContentType())
                {
                    case ContentType.String:
                        content = Encoding.UTF8.GetBytes(await response.Content.ReadAsStringAsync());
                        break;

                    case ContentType.Byte:
                        content = await response.Content.ReadAsByteArrayAsync();
                        break;

                    default:
                        content = await response.Content.ReadAsByteArrayAsync();
                        break;
                }

                return await CreateConvertedFileDTO(
                    String.Empty,
                    content, 
                    typeOfConvertableDocument, 
                    inputFormat, 
                    outputFormat
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IFile> ConvertFileOnStorage(
            string inputFilename, string inputFileLocation, Type typeOfConvertableDocument, FileFormat inputFormat, string outputFilename, string outputFileLocation, FileFormat outputFormat)
        {
            var inputFormatter = await _formatConvertorResolver.Resolve(inputFormat);
            byte[] content;
            switch (await inputFormatter.GetContentType())
            {
                case ContentType.Byte:
                    content = await _storageProvider.ReadFromBytes(Path.Combine(inputFileLocation, inputFilename));
                    break;

                case ContentType.String:
                    content = await _storageProvider.ReadFromString(Path.Combine(inputFileLocation, inputFilename));
                    break;

                default:
                    throw new InvalidOperationException();
            }

            var file = await CreateConvertedFileDTO(outputFilename, content, typeOfConvertableDocument, inputFormat, outputFormat);

            var outputFormatter = await _formatConvertorResolver.Resolve(outputFormat);
            switch (await outputFormatter.GetContentType())
            {
                case ContentType.Byte:
                    await _storageProvider.Write(Path.Combine(outputFileLocation, file.Filename), file.Content);
                    break;

                case ContentType.String:
                    await _storageProvider.Write(Path.Combine(outputFileLocation, file.Filename), Encoding.UTF8.GetString(file.Content));
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return file;
        }

        private async Task<IFile> CreateConvertedFileDTO(string filename, byte[] content, Type fileType, FileFormat inputFormat, FileFormat outputFormat)
        {
            var inputFormater = await _formatConvertorResolver.Resolve(inputFormat);
            var convertableFile = await inputFormater.Input(content, fileType);

            var outputFormater = await _formatConvertorResolver.Resolve(outputFormat);
            var outputContent = await outputFormater.Ouput(convertableFile);

            return new File(filename, outputContent);
        }

        public async Task SendFileAsEmail(IFile file, string recipientEmail, string subject)
        {
            var message = new System.Net.Mail.MailMessage();

            await _smtpClient.Send(message);
        }
    }
}
