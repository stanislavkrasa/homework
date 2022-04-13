using Homework.Models;

namespace Homework.Convertors
{
    public interface IFileManager
    {
        Task<IFile> ConvertFileFromApi(
            byte[] content, Type typeOfConvertableDocument, FileFormat inputFormat, FileFormat outputFormat);

        Task<IFile> ConvertFileFromUrl(string url, Type typeOfConvertableDocument, FileFormat inputFormat, FileFormat outputFormat);

        Task<IFile> ConvertFileOnStorage(
             string inputFilename, string inputFileLocation, Type typeOfConvertableDocument, FileFormat inputFormat, string outputFilename, string outputFileLocation, FileFormat outputFormat);

        Task SendFileAsEmail(IFile file, string recipientEmail, string subject);
    }
}
