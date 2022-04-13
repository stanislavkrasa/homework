using Homework.Models;
using Newtonsoft.Json;
using System.Text;

namespace Homework.Convertors.Formats
{
    public class JsonConvertor : IFormatConvertor
    {
        public Task<ContentType> GetContentType()
        {
            return Task.FromResult(ContentType.String);
        }

        public Task<IConvertableFile> Input(byte[] content, Type typeOfConvertableDocument)
        {
            var stringContent = Encoding.UTF8.GetString(content);
            var convertableDocument = JsonConvert.DeserializeObject(stringContent, typeOfConvertableDocument);

            if (convertableDocument == null)
                throw new ArgumentNullException(nameof(convertableDocument));

            return Task.FromResult((IConvertableFile)convertableDocument);
        }

        public Task<byte[]> Ouput(IConvertableFile convertableFile)
        {
            return Task.FromResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(convertableFile)));
        }
    }
}
