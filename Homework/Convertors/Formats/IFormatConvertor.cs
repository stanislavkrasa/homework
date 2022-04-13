using Homework.Models;

namespace Homework.Convertors.Formats
{
    public interface IFormatConvertor
    {
        Task<ContentType> GetContentType();
        Task<IConvertableFile> Input(byte[] content, Type typeOfConvertableDocument);
        Task<byte[]> Ouput(IConvertableFile convertableFile);
    }
}
