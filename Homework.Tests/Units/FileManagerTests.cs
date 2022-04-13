using Homework.Convertors;
using Homework.Convertors.Formats;
using Homework.Models;
using Homework.SmtpClients;
using Homework.StorageProviders;
using Homework.Tests.Units.Formats;
using Moq;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Homework.Tests.Units
{
    public class FileManagerTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<IFormatConvertorResolver> _mockFormatConvertorResolver;
        private readonly Mock<IStorageProvider> _mockStorageProvider;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly Mock<ISmtpClient> _mockSmtpClient;

        public FileManagerTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _mockFormatConvertorResolver = _mockRepository.Create<IFormatConvertorResolver>();
            _mockStorageProvider = _mockRepository.Create<IStorageProvider>();
            _mockHttpClientFactory = _mockRepository.Create<IHttpClientFactory>();
            _mockSmtpClient = _mockRepository.Create<ISmtpClient>();

            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpClientFactory.Setup(client => client.CreateClient(FileManager.FileManagerHttpClientName)).Returns(_mockHttpHandler.CreateClient());
        }

        [Fact]
        public async Task ConvertFileFromApi_JsonInputXmlOutput_Success()
        {
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Json)).ReturnsAsync(new JsonConvertor());
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Xml)).ReturnsAsync(new XmlConvertor());

            var fileManager = new FileManager(_mockFormatConvertorResolver.Object, _mockStorageProvider.Object, _mockHttpClientFactory.Object, _mockSmtpClient.Object);
            var outputFile = await fileManager.ConvertFileFromApi(
                Encoding.UTF8.GetBytes(await GetTestFile("testfiles/formattest.json")),
                typeof(TestDocument),
                FileFormat.Json,
                FileFormat.Xml
            );

            Assert.NotNull(outputFile);
            Assert.IsType<Models.File>(outputFile);
            Assert.Equal(string.Empty, outputFile.Filename);

            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Json), Times.Once);
            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Xml), Times.Once);
        }

        [Fact]
        public async Task ConvertFileFromApi_JsonInputXmlOutput_ExceptionInvoke()
        {
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Json)).ReturnsAsync(new JsonConvertor());
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Xml)).ReturnsAsync(new XmlConvertor());

            var fileManager = new FileManager(_mockFormatConvertorResolver.Object, _mockStorageProvider.Object, _mockHttpClientFactory.Object, _mockSmtpClient.Object);
            await Assert.ThrowsAsync<JsonReaderException>(async () => await fileManager.ConvertFileFromApi(
                Encoding.UTF8.GetBytes(await GetTestFile("testfiles/corrupted-formattest.json")),
                typeof(TestDocument),
                FileFormat.Json,
                FileFormat.Xml
            ));

            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Json), Times.Once);
            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Xml), Times.Never);
        }

        [Fact]
        public async Task ConvertFileFromUrl_JsonInputXmlOutput_Success()
        {
            var model = new TestDocument()
            {
                Title = "Test title",
                Text = "Test text"
            };

            _mockHttpHandler.SetupRequest(HttpMethod.Get, "https://localhost/test.json")
                .ReturnsResponse(JsonConvert.SerializeObject(model), "application/json");

            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Json)).ReturnsAsync(new JsonConvertor());
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Xml)).ReturnsAsync(new XmlConvertor());

            var fileManager = new FileManager(_mockFormatConvertorResolver.Object, _mockStorageProvider.Object, _mockHttpClientFactory.Object, _mockSmtpClient.Object);
            var outputFile = await fileManager.ConvertFileFromUrl(
                "https://localhost/test.json",
                typeof(TestDocument),
                FileFormat.Json,
                FileFormat.Xml
            );

            Assert.NotNull(outputFile);
            Assert.IsType<Models.File>(outputFile);
            Assert.Equal(string.Empty, outputFile.Filename);

            _mockHttpHandler.VerifyAnyRequest(Times.Exactly(1));
            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Json), Times.Exactly(2));
            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Xml), Times.Exactly(1));
        }
            

        [Fact]
        public async Task ConvertFileOnStorage_JsonInputXmlOutput_Success()
        {
            var inputFileName = "testfiles/formattest.json";
            var inputContent = Encoding.UTF8.GetBytes(await GetTestFile(inputFileName));

            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Json)).ReturnsAsync(new JsonConvertor());
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Xml)).ReturnsAsync(new XmlConvertor());

            _mockStorageProvider.Setup(provider => provider.Write("testfiles/formattest.xml", "<?xml version=\"1.0\" encoding=\"utf-16\"?><TestDocument xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Title>Test title</Title><Text>Test text</Text></TestDocument>")).Returns(Task.CompletedTask);
            _mockStorageProvider.Setup(provider => provider.ReadFromString("testfiles/formattest.json")).ReturnsAsync(inputContent);

            var fileManager = new FileManager(_mockFormatConvertorResolver.Object, _mockStorageProvider.Object, _mockHttpClientFactory.Object, _mockSmtpClient.Object);
            var outputFile = await fileManager.ConvertFileOnStorage(
                inputFileName, 
                "", 
                typeof(TestDocument), 
                FileFormat.Json,
                "testfiles/formattest.xml",
                "",
                FileFormat.Xml
            );

            Assert.NotNull(outputFile);
            Assert.IsType<Models.File>(outputFile);
            Assert.Equal("testfiles/formattest.xml", outputFile.Filename);

            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Json), Times.Exactly(2));
            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Xml), Times.Exactly(2));
            _mockStorageProvider.Verify(provider => provider.ReadFromString("testfiles/formattest.json"), Times.Once);
            _mockStorageProvider.Verify(provider => provider.Write("testfiles/formattest.xml", "<?xml version=\"1.0\" encoding=\"utf-16\"?><TestDocument xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Title>Test title</Title><Text>Test text</Text></TestDocument>"), Times.Once);
        }

        [Fact]
        public async Task ConvertFileOnStorage_CorrupetedJsonInputXmlOutput_Success()
        {
            var filename = "corrupted-formattest.json";
            var content = Encoding.UTF8.GetBytes(await GetTestFile($"testfiles/{filename}"));

            var outputFilename = "formattest.xml";
            var outputContent = await GetTestFile($"testfiles/{outputFilename}");

            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Json)).ReturnsAsync(new JsonConvertor());
            _mockFormatConvertorResolver.Setup(resolver => resolver.Resolve(FileFormat.Xml)).ReturnsAsync(new XmlConvertor());
            _mockStorageProvider.Setup(provider => provider.ReadFromString("testfiles/corrupted-formattest.json")).ThrowsAsync(new JsonReaderException());
            _mockStorageProvider.Setup(provider => provider.Write("formattest.xml", "<?xml version=\"1.0\" encoding=\"utf-16\"?><TestDocument xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Title>Test title</Title><Text>Test text</Text></TestDocument>")).Returns(Task.CompletedTask);

            var fileManager = new FileManager(_mockFormatConvertorResolver.Object, _mockStorageProvider.Object, _mockHttpClientFactory.Object, _mockSmtpClient.Object);
            await Assert.ThrowsAsync<JsonReaderException>(() => fileManager.ConvertFileOnStorage(
                filename,
                "testfiles/",
                typeof(TestDocument),
                FileFormat.Json,
                outputFilename,
                "testfiles/",
                FileFormat.Xml
            ));

            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Json), Times.Once);
            _mockFormatConvertorResolver.Verify(resolver => resolver.Resolve(FileFormat.Xml), Times.Never);
            _mockStorageProvider.Verify(provider => provider.ReadFromString("testfiles/corrupted-formattest.json"), Times.Once);
            _mockStorageProvider.Verify(provider => provider.Write("formattest.xml", "<?xml version=\"1.0\" encoding=\"utf-16\"?><TestDocument xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Title>Test title</Title><Text>Test text</Text></TestDocument>"), Times.Never);
        }

        [Fact]
        public async Task SendFileAsEmail_TestFile_Success()
        {
            var file = new Models.File("test.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestDocument() { Title = "Test title", Text = "test text" })));
            _mockSmtpClient.Setup(client => client.Send(It.IsAny<System.Net.Mail.MailMessage>())).Returns(Task.CompletedTask);

            var fileManager = new FileManager(_mockFormatConvertorResolver.Object, _mockStorageProvider.Object, _mockHttpClientFactory.Object, _mockSmtpClient.Object);
            await fileManager.SendFileAsEmail(file, "somebody@homework-notino.com", "Test subject");

            _mockSmtpClient.Verify(client => client.Send(It.IsAny<System.Net.Mail.MailMessage>()), Times.Once);
        }

        private Task<string> GetTestFile(string filename)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (dir == null)
                throw new InvalidDataException();

            var testFile = Path.Combine(dir, filename);
            return Task.FromResult(System.IO.File.ReadAllText(testFile));
        }
    }
}