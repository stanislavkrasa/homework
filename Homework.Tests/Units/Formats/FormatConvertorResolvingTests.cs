using Homework.Convertors.Formats;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Homework.Tests.Units.Formats
{
    public class FormatConvertorResolvingTests
    {
        [Fact]
        public async Task Resolve_Json_Success()
        {
            var resolver = new FormatConvertorResolver();
            var format = await resolver.Resolve(Models.FileFormat.Json);

            Assert.NotNull(format);
            Assert.IsType<JsonConvertor>(format);
        }

        [Fact]
        public async Task Resolve_Xml_Success()
        {
            var resolver = new FormatConvertorResolver();
            var format = await resolver.Resolve(Models.FileFormat.Xml);

            Assert.NotNull(format);
            Assert.IsType<XmlConvertor>(format);
        }

        [Fact]
        public async Task Resolve_UndefinedType_NotImplementedExceptionInvoke()
        {
            var resolver = new FormatConvertorResolver();

            await Assert.ThrowsAsync<NotImplementedException>(() => resolver.Resolve(Models.FileFormat.Undefined) );
        }
    }
}
