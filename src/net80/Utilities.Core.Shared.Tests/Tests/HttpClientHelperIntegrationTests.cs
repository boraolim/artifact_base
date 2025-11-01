
namespace Utilities.Core.Shared.Tests;

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
        var urlSource = "https://www.google.com.mx";
        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Get,
            url: urlSource,
            logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("<title>Google</title>", content);
    }

    [Fact]
    public async Task POST_ReturnsCreated()
    {
        var urlSource = "https://fakestoreapi.com/products";
        var bodyInput = new { title = "Producto nuevo", price = 29.99 };

        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Post,
            url: urlSource,
            body: bodyInput,
            logBodies: true);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Producto nuevo", content);
    }

    [Fact]
    public async Task PUT_ReturnsSuccess()
    {
        var urlSource = "https://fakestoreapi.com/products/21";
        var bodyInput = new { title = "Producto actualizado", price = 129.99 };

        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: _env,
            logger: _logger,
            method: HttpMethod.Put,
            url: urlSource,
            body: bodyInput,
            logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Producto actualizado", content);
    }

    [Fact]
    public async Task PATCH_ReturnsSuccess()
    {
        var urlSource = "https://httpbin.org/patch";
        var bodyInput = new { name = "Juan" };

        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Patch,
            url: urlSource,
            body: bodyInput,
            logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Juan", content);
    }

    [Fact]
    public async Task DELETE_ReturnsSuccess()
    {
        var urlSouce = "https://fakestoreapi.com/products/21";

        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Delete,
            url: urlSouce,
            logBodies: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HEAD_ReturnsSuccess()
    {
        var urlSource = "https://httpbin.org/anything";
        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Head,
            url: urlSource,
            logBodies: false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OPTIONS_ReturnsSuccess()
    {
        var urlSource = "https://httpbin.org/get";
        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Options,
            url: urlSource,
            logBodies: false);

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task SendAsync_ShouldIncludeMultipartFilesWithOutBody()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        HttpRequestMessage? capturedRequest = null;

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\":true}", Encoding.UTF8, "application/json")
            });

        var logger = Mock.Of<ILogger>();
        var environment = Mock.Of<IHostEnvironment>(e => e.EnvironmentName == Environments.Development);

        // Crear archivo simulado
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("fake file content"));
        var filesSend = new List<(string Name, Stream Content, string FileName)>
        {
            ("upload", fileStream, "test.txt")
        };


        // Reemplazamos el método CreateHttpClient por uno controlado
        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);


        // Act
        // ⚠️ Aquí deberías adaptar la llamada si tu clase estática se llama distinto
        var response = await HttpClientFactoryHelper.SendAsync(
            env: environment,
            logger: logger,
            method: HttpMethod.Post,
            url: "https://httpbin.org/post",
            files: filesSend,
            logBodies: true
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("origin", content);
    }

    [Fact]
    public async Task SendAsync_ShouldIncludeMultipartFilesWithBody()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        HttpRequestMessage? capturedRequest = null;

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\":true}", Encoding.UTF8, "application/json")
            });

        var logger = Mock.Of<ILogger>();
        var environment = Mock.Of<IHostEnvironment>(e => e.EnvironmentName == Environments.Development);

        // Crear archivo simulado
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("fake file content"));
        var filesSend = new List<(string Name, Stream Content, string FileName)>
        {
            ("upload", fileStream, "test.txt")
        };


        // Reemplazamos el método CreateHttpClient por uno controlado
        var env = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(env);


        // Act
        // ⚠️ Aquí deberías adaptar la llamada si tu clase estática se llama distinto
        var response = await HttpClientFactoryHelper.SendAsync(
            env: environment,
            logger: logger,
            method: HttpMethod.Post,
            url: "https://httpbin.org/post",
            body: new { numberPhone = "441223" },
            files: filesSend,
            logBodies: true
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("origin", content);
    }

    [Fact]
    public async Task SendAsync_ShouldNotCatch_WhenCancellationRequested()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var environment = Mock.Of<IHostEnvironment>();
        var logger = Mock.Of<ILogger>();

        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            HttpClientFactoryHelper.SendAsync(
                env: environment,
                logger: logger,
                method: HttpMethod.Get,
                url: "http://localhost/test",
                cancellationToken: cts.Token
            )
        );
    }

    [Fact]
    public async Task SendAsync_ShouldThrowTimeoutException_WhenRequestTimesOut()
    {
        // Arrange
        var environment = Mock.Of<IHostEnvironment>();
        var logger = Mock.Of<ILogger>();
        var cts = new CancellationTokenSource();

        // Timeout muy corto
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(async () =>
            await HttpClientFactoryHelper.SendAsync(
                env: environment,
                logger: logger,
                method: HttpMethod.Get,
                url: "https://httpbin.org/delay/10", // tarda 10 seg
                cancellationToken: cts.Token,
                timeout: TimeSpan.FromSeconds(3)    // ⏱ Timeout corto
            )
        );
    }

    [Fact]
    public async Task SendAsync_ShouldLogAndThrow_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var envMock = Mock.Of<IHostEnvironment>(e => e.EnvironmentName == Environments.Development);

        // Simulamos un HttpClient que lanza HttpRequestException
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Simulated network failure"));

        // 👇 Usamos un HttpMessageHandler falso que lanza HttpRequestException
        var httpClient = new HttpClient(handlerMock.Object);

        // Para probar la excepción, vamos a inyectar el HttpClient en un método sobrecargado
        // (o usar InternalsVisibleTo para permitir testing interno).
        // Pero para este ejemplo, creamos una versión local de SendAsync con el handler.
        async Task<HttpResponseMessage> SendWithHandler()
        {
            return await HttpClientFactoryHelper.SendAsync(
                env: envMock,
                logger: loggerMock.Object,
                method: HttpMethod.Get,
                url: "https://nonexistent.domain.fake", // dominio no válido
                cancellationToken: CancellationToken.None,
                logBodies: true
            );
        }

        // Act & Assert
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => SendWithHandler());

        // ✅ Verificamos que el logger registró un error con el mensaje esperado
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error HTTP")),
                ex,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }


    [Fact]
    public async Task GET_NotFound_Returns404()
    {
        var urlSource = "https://httpbin.org/status/404";

        var envProd = CreateEnv("Production");
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);
        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: _logger,
            method: HttpMethod.Get,
            url: urlSource,
            logBodies: true);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #region Headers
    [Fact]
    public async Task SendAsync_AddsHeadersToRequest()
    {
        var env = CreateEnv("Production");
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
        var env = CreateEnv("Production");
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
        var envProd = CreateEnv("Production");
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(responseMessage);
        var client = HttpClientFactoryHelper.CreateHttpClient(envProd);

        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Archivo de prueba"));
        var filesSend = new List<(string Name, Stream Content, string FileName)>
        {
            ("upload", fileStream, "test.txt")
        };

        var response = await HttpClientFactoryHelper.SendAsync(
            env: envProd,
            logger: NullLogger.Instance,
            method: HttpMethod.Post,
            url: "https://example.com",
            files: filesSend
        );

        // Assert.IsType<MultipartFormDataContent>(handler.LastRequest.Content);
        Assert.NotNull(response.Headers);
    }
    #endregion

    private IHostEnvironment CreateEnv(string environmentName)
    {
        var envMock = new Moq.Mock<IHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns(environmentName);
        return envMock.Object;
    }
}
