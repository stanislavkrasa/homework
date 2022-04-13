using Homework.Models;

namespace Homework.Convertors.Formats
{
    public class FormatConvertorResolver : IFormatConvertorResolver
    {
        public Task<IFormatConvertor> Resolve(FileFormat fileFormat)
        {
            switch (fileFormat)
            {
                case FileFormat.Json:
                    return Task.FromResult<IFormatConvertor>(new JsonConvertor());

                case FileFormat.Xml:
                    return Task.FromResult<IFormatConvertor>(new XmlConvertor());

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
