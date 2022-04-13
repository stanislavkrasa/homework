using Homework.Convertors.Formats;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Homework.Tests.Units.Formats
{
    public class XmlFormatTests
    {
        public XmlFormatTests()
        {
        }

        [Fact]
        public async Task GetContentType_XmlFormat_String()
        {
            var jsonFormat = new JsonConvertor();

            Assert.Equal(ContentType.String, await jsonFormat.GetContentType());
        }

        [Fact]
        public async Task Input_UserFile_ConvertingToConvertedDocument()
        {
            var xmlFormat = new XmlConvertor();
            var convertableFile = (TestDocument)await xmlFormat.Input(
                Encoding.UTF8.GetBytes(await GetTestXmlFile("testfiles\\formattest.xml")), 
                typeof(TestDocument)
            );

            Assert.Equal("Test title", convertableFile.Title);
            Assert.Equal("Test text", convertableFile.Text);
        }

        [Fact]
        public async Task Input_UserFileWithInvalidJson_JsonReaderExceptionInvoke()
        {
            var xmlFormat = new XmlConvertor();

            await Assert.ThrowsAsync<System.InvalidOperationException>(async () => 
                await xmlFormat.Input(Encoding.UTF8.GetBytes(await GetTestXmlFile("testfiles\\corrupted-formattest.xml")), typeof(TestDocument)));
        }

        [Fact]
        public async Task Input_Null_NullReferenceExceptionInvoke()
        {
            var xmlFormat = new XmlConvertor();

#pragma warning disable CS8625 // Literál null nejde převést na odkazový typ, který nemůže mít hodnotu null.
            await Assert.ThrowsAsync<ArgumentNullException>(() => xmlFormat.Input(null, typeof(TestDocument)));
#pragma warning restore CS8625 // Literál null nejde převést na odkazový typ, který nemůže mít hodnotu null.
        }

        [Fact]
        public async Task Output_ConvertedDocument_Success()
        {
            var convertableFile = new TestDocument()
            {
                Title = "Test title",
                Text = "Test text"
            };

            var xmlFormat = new XmlConvertor();
            var content = await xmlFormat.Ouput(convertableFile);

            var newConvertableFile = (TestDocument)await xmlFormat.Input(content, typeof(TestDocument));

            Assert.NotNull(content);
            Assert.Equal(convertableFile.Title, newConvertableFile.Title);
            Assert.Equal(convertableFile.Text, newConvertableFile.Text);
        }

        private Task<string> GetTestXmlFile(string filename)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (dir == null)
                throw new InvalidDataException();

            var testFile = Path.Combine(dir, filename);
            return Task.FromResult(File.ReadAllText(testFile));
        }
    }
}
