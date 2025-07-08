using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

using Hogar.Core.Shared.Services;
using Hogar.Core.Shared.Wrappers;
using Hogar.Core.Shared.Tests.Records;

namespace Hogar.Core.Shared.Tests
{
    public class CommandTests
    {
        [Fact]
        public void CommonResponse_ShouldSetPropertiesCorrectly()
        {
            var response = new CommonResponse("cadena prueba");
            Assert.Equal("cadena prueba", response.cadena);
        }

        [Fact]
        public void UploadRequest_ShouldSetPropertiesCorrectly()
        {
            using var ms = new MemoryStream();
            var request = new UploadRequest("Descripcion archivo", ms);

            Assert.Equal("Descripcion archivo", request.Descripcion);
            Assert.Same(ms, request.Archivo);
        }

        [Fact]
        public void CommonResponse_ShouldBeEqual_WhenValuesAreSame()
        {
            var r1 = new CommonResponse("igual");
            var r2 = new CommonResponse("igual");

            Assert.Equal(r1, r2);
        }

        [Fact]
        public void JsonToChainRequestHandler_ShouldReturnExpectedResult()
        {
            var handler = new Handlers.JsonToChainHandler();
            var request = new JsonToChainRequest("test payload");

            var result = handler.Handle(request);

            Assert.True(result.Succeded);
            Assert.Equal("test payload", result.Data.cadena);
        }

        [Fact]
        public async Task JsonToChainRequestHandler_ShouldReturnExpectedResultAsync()
        {
            var handler = new Handlers.JsonToChainHandler();
            var request = new JsonToChainRequest("async payload");

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.True(result.Succeded);
            Assert.Equal("async payload", result.Data.cadena);
        }

        [Fact]
        public void SampleCommand_ShouldStoreQueryParamsCorrectly()
        {
            var queryParams = new SampleQueryParams { Param = "test param" };
            var command = new SampleCommand(queryParams);

            Assert.Equal("test param", command.Query.Param);
        }

        [Fact]
        public void JsonToChainRequest_ShouldHaveCorrectResponseType()
        {
            var command = new JsonToChainRequest(new { });
            Assert.Equal(typeof(Result<CommonResponse>), ((ICommand<Result<CommonResponse>>)command).ResponseType);
        }

        [Fact]
        public void CommonResponse_ShouldSerializeAndDeserializeCorrectly()
        {
            var response = new CommonResponse("cadena");
            var json = JsonSerializer.Serialize(response);
            var deserialized = JsonSerializer.Deserialize<CommonResponse>(json);

            Assert.Equal(response, deserialized);
        }
    }
}
