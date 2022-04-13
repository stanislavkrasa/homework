using Homework.Models;

namespace Homework.Convertors.Formats
{
    public interface IFormatConvertorResolver
    {
        public Task<IFormatConvertor> Resolve(FileFormat fileFormat);
    }
}
