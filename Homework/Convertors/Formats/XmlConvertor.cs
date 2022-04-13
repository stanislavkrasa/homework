using Homework.Models;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Homework.Convertors.Formats
{
    public class XmlConvertor : IFormatConvertor
    {
        public Task<ContentType> GetContentType()
        {
            return Task.FromResult(ContentType.String);
        }

        public Task<IConvertableFile> Input(byte[] content, Type typeOfConvertableDocument)
        {
            IConvertableFile? convertableFile;
            try
            {
                var xmlStream = new StreamReader(new MemoryStream(content));
                var serializer = new XmlSerializer(typeOfConvertableDocument);
                convertableFile = (IConvertableFile?)serializer.Deserialize(xmlStream);
            }
            catch (Exception)
            {
                throw;
            }

            if (convertableFile == null)
                throw new ArgumentNullException();

            return Task.FromResult(convertableFile);
        }

        public Task<byte[]> Ouput(IConvertableFile convertableFile)
        {
            byte[] xmlContent;
            try
            {
                var serializer = new XmlSerializer(convertableFile.GetType());
                string xmlStringContent = "";
                using (var stringWriter = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(stringWriter))
                    {
                        serializer.Serialize(writer, convertableFile);
                        xmlStringContent = stringWriter.ToString();
                    }
                }
                xmlContent = Encoding.UTF8.GetBytes(xmlStringContent);
            }
            catch (Exception)
            {
                throw;
            }

            return Task.FromResult(xmlContent);
        }
    }
}
