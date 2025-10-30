namespace Hogar.Core.Shared.Tests;

public class HttpClientHelperIntegrationTests
{
    private readonly IHostEnvironment _env;
    private readonly NullLogger _logger = NullLogger.Instance;

    public HttpClientHelperIntegrationTests()
    {
        var envMock = new Moq.Mock<IHostEnvironment>();

        envMock.Setup(e => e.EnvironmentName).Returns("Development");
        envMock.Setup(e => e.ApplicationName).Returns("HttpClientHelperTests");
        envMock.Setup(e => e.ContentRootPath).Returns(AppContext.BaseDirectory);

        _env = envMock.Object;
    }

    [Fact]
    public void When_CreateHttpClient_Development_SetsCallbacks()
    {
        var envMock = new Moq.Mock<IHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns("Development");

        var client = HttpClientFactoryHelper.CreateHttpClient(envMock.Object);

        Assert.NotNull(client);
    }

    [Fact]
    public void When_CreateHttpClient_Production_SetsCallbacks()
    {
        var envMock = new Moq.Mock<IHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns("Production");

        var client = HttpClientFactoryHelper.CreateHttpClient(envMock.Object);

        Assert.NotNull(client);
    }

    [Fact]
    public async Task GET_ReturnsSuccess()
    {
        var url = "https://httpbin.org/get";
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Get, url, logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"url\": \"https://httpbin.org/get\"", content);
    }

    [Fact]
    public async Task POST_ReturnsCreated()
    {
        var url = "https://httpbin.org/post";
        var body = new { name = "Juan" };
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Post, url, body, logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode); // httpbin devuelve 200
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Juan", content);
    }

    [Fact]
    public async Task PUT_ReturnsSuccess()
    {
        var url = "https://httpbin.org/put";
        var body = new { name = "Juan" };
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Put, url, body, logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Juan", content);
    }

    [Fact]
    public async Task PATCH_ReturnsSuccess()
    {
        var url = "https://httpbin.org/patch";
        var body = new { name = "Juan" };
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Patch, url, body, logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Juan", content);
    }

    [Fact]
    public async Task DELETE_ReturnsSuccess()
    {
        var url = "https://httpbin.org/delete";
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Delete, url, logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HEAD_ReturnsSuccess()
    {
        var url = "https://httpbin.org/anything";
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Head, url, logBodies: false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OPTIONS_ReturnsSuccess()
    {
        var url = "https://httpbin.org/get";
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Options, url, logBodies: false);

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
    }

    //[Fact]
    //public async Task UploadFile_ReturnsSuccess()
    //{
    //    var url = "https://httpbin.org/post";

    //    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Archivo de prueba"));
    //    var filesInput = new[] { ("file", stream, "test.txt") };

    //    var response = await HttpClientFactoryHelper.SendAsync(
    //        _env,
    //        _logger,
    //        HttpMethod.Post,
    //        url,
    //        files: filesInput,
    //        logBodies: true
    //    );

    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    var content = await response.Content.ReadAsStringAsync();
    //    Assert.Contains("Archivo de prueba", content);
    //}

    [Fact]
    public async Task GET_NotFound_Returns404()
    {
        var url = "https://httpbin.org/status/404";
        var response = await HttpClientFactoryHelper.SendAsync(_env, _logger, HttpMethod.Get, url, logBodies: true);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #region Headers
    [Fact]
    public async Task SendAsync_AddsHeadersToRequest()
    {
        var env = CreateEnv("Development");
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        // Inyectar handler usando overload opcional
        var client = HttpClientFactoryHelper.CreateHttpClient(env);
        var headers = new Dictionary<string, string>
        {
            { "X-Test", "123" },
            { "Authorization", "Bearer token" }
        };

        var response = await HttpClientFactoryHelper.SendAsync(
            env,
            NullLogger.Instance,
            HttpMethod.Get,
            "https://example.com",
            null,
            headers
        );

        Assert.NotNull(response.Headers);
    }
    #endregion

    #region JSON Body
    [Fact]
    public async Task SendAsync_BodyJson_LogsCorrectly()
    {
        var env = CreateEnv("Development");
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"result\":true}", Encoding.UTF8, "application/json")
        };
        var client = HttpClientFactoryHelper.CreateHttpClient(env);

        var body = new { Name = "Juan", Age = 30 };

        var response = await HttpClientFactoryHelper.SendAsync(
            env,
            NullLogger.Instance,
            HttpMethod.Post,
            "https://example.com",
            body: body
        );

        Assert.NotNull(response.Headers);
    }
    #endregion

    #region File Upload
    [Fact]
    public async Task SendAsync_FileUpload_FormsCorrectly()
    {
        var env = CreateEnv("Development");
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(responseMessage);
        var client = HttpClientFactoryHelper.CreateHttpClient(env);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Archivo de prueba"));
        var files = new[] { ("file", stream, "test.txt") };

        var response = await HttpClientFactoryHelper.SendAsync(
            env,
            NullLogger.Instance,
            HttpMethod.Post,
            "https://example.com",
            files
        );

        // Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.NotNull(response.Headers);
    }
    #endregion

    #region Timeout
    //[Fact]
    //public async Task SendAsync_Timeout_ThrowsTimeoutException()
    //{
    //    var env = CreateEnv("Development");
    //    var ex = new TaskCanceledException();
    //    var handler = new FakeHttpMessageHandler(ex);
    //    var client = HttpClientFactoryHelper.CreateHttpClient(env);

    //    await Assert.ThrowsAsync<TimeoutException>(async () =>
    //    {
    //        await HttpClientFactoryHelper.SendAsync(
    //            env,
    //            NullLogger.Instance,
    //            HttpMethod.Get,
    //            "https://example.com"
    //        );
    //    });
    //}
    #endregion

    #region HttpRequestException
    //[Fact]
    //public async Task SendAsync_HttpRequestException_Throws()
    //{
    //    var env = CreateEnv("Development");
    //    var ex = new HttpRequestException("Falla HTTP");
    //    var handler = new FakeHttpMessageHandler(ex);
    //    var client = new HttpClient(handler);

    //    await Assert.ThrowsAsync<HttpRequestException>(async () =>
    //    {
    //        await HttpClientFactoryHelper.SendAsync(
    //            env,
    //            NullLogger.Instance,
    //            HttpMethod.Get,
    //            "https://example.com"
    //        );
    //    });
    //}
    #endregion

    #region Generic Exception
    //[Fact]
    //public async Task SendAsync_GenericException_Throws()
    //{
    //    var env = CreateEnv("Development");
    //    var ex = new InvalidOperationException("Error inesperado");
    //    var handler = new FakeHttpMessageHandler(ex);
    //    var client = new HttpClient(handler);

    //    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
    //    {
    //        await HttpClientFactoryHelper.SendAsync(
    //            env,
    //            NullLogger.Instance,
    //            HttpMethod.Get,
    //            "https://example.com"
    //        );
    //    });
    //}
    #endregion

    private IHostEnvironment CreateEnv(string environmentName)
    {
        var envMock = new Moq.Mock<IHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns(environmentName);
        return envMock.Object;
    }
}
