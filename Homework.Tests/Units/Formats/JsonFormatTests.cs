using Homework.Convertors.Formats;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Homework.Tests.Units.Formats
{
    public class JsonFormatTests
    {
        public JsonFormatTests()
        {
        }

        [Fact]
        public async Task GetContentType_JsonFormat_String()
        {
            var jsonFormat = new JsonConvertor();

            Assert.Equal(ContentType.String, await jsonFormat.GetContentType());
        }

        [Fact]
        public async Task Input_UserFile_ConvertingToConvertedDocument()
        {
            var jsonFormat = new JsonConvertor();
            var convertableDocument = (TestDocument)await jsonFormat.Input(
                Encoding.UTF8.GetBytes(await GetTestJsonFile("testfiles\\formattest.json")), 
                typeof(TestDocument)
            );

            Assert.Equal("Test title", convertableDocument.Title);
            Assert.Equal("Test text", convertableDocument.Text);
        }

        [Fact]
        public async Task Input_UserFileWithInvalidJson_JsonReaderExceptionInvoke()
        {
            var jsonFormat = new JsonConvertor();

            await Assert.ThrowsAsync<Newtonsoft.Json.JsonReaderException>(async () => 
                await jsonFormat.Input(Encoding.UTF8.GetBytes(await GetTestJsonFile("testfiles\\corrupted-formattest.json")), typeof(TestDocument)));
        }

        [Fact]
        public async Task Input_Null_NullReferenceExceptionInvoke()
        {
            var jsonFormat = new JsonConvertor();

#pragma warning disable CS8625 // Literál null nejde převést na odkazový typ, který nemůže mít hodnotu null.
            await Assert.ThrowsAsync<ArgumentNullException>(() => jsonFormat.Input(null, typeof(TestDocument)));
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

            var jsonFormat = new JsonConvertor();
            var content = await jsonFormat.Ouput(convertableFile);

            var newConvertableFile = (TestDocument)await jsonFormat.Input(content, typeof(TestDocument));

            Assert.NotNull(content);
            Assert.Equal(convertableFile.Title, newConvertableFile.Title);
            Assert.Equal(convertableFile.Text, newConvertableFile.Text);
        }

        private Task<string> GetTestJsonFile(string filename)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (dir == null)
                throw new InvalidDataException();

            var testFile = Path.Combine(dir, filename);
            return Task.FromResult(File.ReadAllText(testFile));
        }
    }
}
