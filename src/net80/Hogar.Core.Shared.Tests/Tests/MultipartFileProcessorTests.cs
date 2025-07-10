using System.Net.Http;

namespace Hogar.Core.Shared.Tests;

public class MultipartFileProcessorTests
{
    private readonly MultipartFileProcessor _processor;
    private readonly UploadSettings _settings = new UploadSettings
    {
        MaxFileCount = 2,
        MaxFileSizeBytes = 5 * 1024 * 1024, // 5 MB
        MaxTotalFileSizeBytes = 10 * 1024 * 1024 // 10 MB
    };

    public MultipartFileProcessorTests()
    {
        _processor = new MultipartFileProcessor(Options.Create(_settings));
    }

    private static MultipartFileProcessor CreateProcessor(long maxFileSize = 10_000, int maxFileCount = 5)
    {
        var options = Options.Create(new UploadSettings
        {
            MaxFileCount = maxFileCount,
            MaxTotalFileSizeBytes = maxFileSize
        });

        return new MultipartFileProcessor(options);
    }

    [Fact]
    public async Task ProcessAsync_Should_Throw_When_ContentType_Invalid()
    {
        var processor = CreateProcessor();
        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r => r.ContentType).Returns("application/json");

        await Assert.ThrowsAsync<UnsupportedMediaTypeException>(() =>
            processor.ProcessAsync(requestMock.Object, (f, s) => Task.FromResult(new { Succeded = true })));
    }

    [Fact]
    public async Task ProcessAsync_Should_Throw_When_Boundary_Missing()
    {
        var processor = CreateProcessor();
        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r => r.ContentType).Returns("multipart/form-data;");

        await Assert.ThrowsAsync<BadRequestException>(() =>
            processor.ProcessAsync(requestMock.Object, (f, s) => Task.FromResult(new { Succeded = true })));
    }

    [Fact]
    public async Task ProcessAsync_Should_Throw_When_No_Files()
    {
        var processor = CreateProcessor();
        var boundary = "boundary123";
        var multipartBody = Encoding.UTF8.GetBytes("--boundary123--\r\n");

        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r => r.ContentType).Returns($"multipart/form-data; boundary={boundary}");
        requestMock.Setup(r => r.Body).Returns(new MemoryStream(multipartBody));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            processor.ProcessAsync(requestMock.Object, (f, s) => Task.FromResult(new { Succeded = true })));
    }

    [Fact]
    public async Task ProcessAsync_ShouldThrow_WhenContentTypeIsInvalid()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";

        await Assert.ThrowsAsync<UnsupportedMediaTypeException>(() =>
            _processor.ProcessAsync<object>(context.Request, (f, s) => Task.FromResult<object>(null)));
    }

    [Fact]
    public async Task ProcessAsync_ShouldThrow_WhenBoundaryIsMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data";

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _processor.ProcessAsync<object>(context.Request, (f, s) => Task.FromResult<object>(null)));
    }

    [Fact]
    public async Task ProcessAsync_ShouldThrow_WhenNoFilesProvided()
    {
        var boundary = "test-boundary";
        var context = new DefaultHttpContext();
        context.Request.ContentType = $"multipart/form-data; boundary={boundary}";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("--test-boundary--"));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            _processor.ProcessAsync<object>(context.Request, (f, s) => Task.FromResult<object>(null)));
    }

    [Fact]
    public async Task ProcessAsync_ShouldThrow_WhenExceedsMaxFileCount()
    {
        // Arrange
        var boundary = "test-boundary";
        var content = new MultipartFormDataContent(boundary);

        // Agregamos 3 archivos cuando el límite es 2
        for(int i = 0; i < 3; i++)
        {
            var fileContent = new StringContent("Test content");
            content.Add(fileContent, "file", $"test{i}.txt");
        }

        var stream = new MemoryStream();
        await content.CopyToAsync(stream);
        stream.Position = 0;

        var context = new DefaultHttpContext();
        context.Request.ContentType = $"multipart/form-data; boundary={boundary}";
        context.Request.Body = stream;

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _processor.ProcessAsync<object>(context.Request, (f, s) => Task.FromResult<object>(new { Succeded = true })));
    }

    [Fact]
    public async Task ProcessAsync_ShouldProcessValidFiles()
    {
        // Arrange
        var boundary = "test-boundary";
        var content = new MultipartFormDataContent(boundary);

        // Agrega un archivo válido
        var fileContent = new StringContent("Test content");
        content.Add(fileContent, "file", "file1.txt");

        var stream = new MemoryStream();
        await content.CopyToAsync(stream);
        stream.Position = 0;

        var context = new DefaultHttpContext();
        context.Request.ContentType = $"multipart/form-data; boundary={boundary}";
        context.Request.Body = stream;

        // Act
        var result = await _processor.ProcessAsync<object>(context.Request, (f, s) => Task.FromResult<object>(new { Succeded = true }));

        // Assert
        Assert.True(result.Succeded);
        Assert.Single(result.Data.UploadesFiles.UploadesFiles);
    }

    [Fact]
    public void IsFileSizeValid_Should_AddToNotUploadedFiles_And_ReturnFalse_When_FileTooBig()
    {
        // Arrange
        var processor = CreateProcessorWithSettings(maxTotalFileSizeBytes: 10);
        var notUploadedFiles = new List<NotSuccessFileResponse>();
        var fileSize = 15;  // Mayor al límite
        var currentTotal = 0;
        var fileName = "file.txt";

        // Act
        var result = InvokeIsFileSizeValid(processor, fileSize, currentTotal, fileName, notUploadedFiles);

        // Assert
        Assert.False(result);
        Assert.Single(notUploadedFiles);
        Assert.Equal(fileName, notUploadedFiles[0].Archivo);
    }

    [Fact]
    public void IsFileSizeValid_Should_ThrowException_When_TotalSizeExceeded()
    {
        // Arrange
        var processor = CreateProcessorWithSettings(maxTotalFileSizeBytes: 10);
        var notUploadedFiles = new List<NotSuccessFileResponse>();
        var fileSize = 5;
        var currentTotal = 8;  // 5 + 8 = 13 > 10
        var fileName = "file.txt";

        // Act & Assert
        var ex = Assert.Throws<TargetInvocationException>(() =>
            InvokeIsFileSizeValid(processor, fileSize, currentTotal, fileName, notUploadedFiles));

        // La excepción real está en InnerException
        Assert.IsType<BadRequestException>(ex.InnerException);
    }

    [Fact]
    public void IsFileSizeValid_Should_ReturnTrue_When_WithinLimits()
    {
        // Arrange
        var processor = CreateProcessorWithSettings(maxTotalFileSizeBytes: 10);
        var notUploadedFiles = new List<NotSuccessFileResponse>();
        var fileSize = 3;
        var currentTotal = 5;  // 3 + 5 = 8 < 10
        var fileName = "file.txt";

        // Act
        var result = InvokeIsFileSizeValid(processor, fileSize, currentTotal, fileName, notUploadedFiles);

        // Assert
        Assert.True(result);
        Assert.Empty(notUploadedFiles);
    }

    [Fact]
    public void IsValidFileSection_Should_ReturnFalse_When_ContentDisposition_Invalid()
    {
        // Arrange
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                { "Content-Disposition", "invalid-header" }
            }
        };

        // Act
        var result = InvokeIsValidFileSection(section, out var contentDisposition, out var fileName);

        // Assert
        Assert.False(result);
        Assert.NotNull(contentDisposition);
        Assert.Equal("invalid-header", contentDisposition.DispositionType);
        Assert.True(string.IsNullOrWhiteSpace(fileName));
    }

    [Fact]
    public void IsValidFileSection_Should_ReturnFalse_When_FileName_Empty()
    {
        // Arrange
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                { "Content-Disposition", $"form-data; name=\"file\"; filename=\"\"" }
            }
        };

        // Act
        var result = InvokeIsValidFileSection(section, out var contentDisposition, out var fileName);

        // Assert
        Assert.False(result);
        Assert.NotNull(contentDisposition);
        Assert.True(string.IsNullOrWhiteSpace(fileName));
    }

    [Fact]
    public void IsValidFileSection_Should_ReturnFalse_When_DispositionType_Invalid()
    {
        // Arrange
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                { "Content-Disposition", $"attachment; name=\"file\"; filename=\"test.txt\"" }
            }
        };

        // Act
        var result = InvokeIsValidFileSection(section, out var contentDisposition, out var fileName);

        // Assert
        Assert.False(result);
        Assert.NotNull(contentDisposition);
        Assert.Equal("test.txt", fileName);
    }

    [Fact]
    public void IsValidFileSection_Should_ReturnTrue_When_Valid()
    {
        // Arrange
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                { "Content-Disposition", $"form-data; name=\"file\"; filename=\"test.txt\"" }
            }
        };

        // Act
        var result = InvokeIsValidFileSection(section, out var contentDisposition, out var fileName);

        // Assert
        Assert.True(result);
        Assert.NotNull(contentDisposition);
        Assert.Equal("test.txt", fileName);
        Assert.Equal("form-data", contentDisposition.DispositionType);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\"")]
    [InlineData("form-data; name=\"file; filename=test.txt")]
    public void IsValidFileSection_Should_ReturnFalse_When_ContentDispositionEmpty_TryParse_Fails(string headerValue)
    {
        // Arrange
        // Arrange
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                { string.Empty, headerValue }
            }
        };

        // Act
        var result = InvokeIsValidFileSection(section, out var contentDisposition, out var fileName);

        // Assert
        Assert.False(result);
        Assert.Null(contentDisposition);
        Assert.Null(fileName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\"")]
    [InlineData("form-data; name=\"file; filename=test.txt")]
    public void IsValidFileSection_Should_ReturnFalse_When_ContentDispositionLabelNotNull_TryParse_Fails(string headerValue)
    {
        // Arrange
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                {"Content-Disposition", headerValue }
            }
        };

        // Act
        var result = InvokeIsValidFileSection(section, out var contentDisposition, out var fileName);

        // Assert
        Assert.False(result);
        Assert.Null(contentDisposition);
        Assert.Null(fileName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateRequestHeaders_Should_ThrowException_When_ContentType_Is_Null_Or_Whitespace(string contentType)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.ContentType = contentType;

        // Act & Assert
        var ex = Assert.Throws<TargetInvocationException>(() => InvokeValidateRequestHeaders(context.Request));

        // Debes validar la InnerException porque el método fue invocado por reflection:
        Assert.IsType<UnsupportedMediaTypeException>(ex.InnerException);
        Assert.Equal(MessageConstantsCore.MSG_IS_NOT_MULTIPART, ex.InnerException.Message);
    }

    [Fact]
    public void IsValidFileSection_Should_ReturnFalse_When_Header_Empty()
    {
        var section = new MultipartSection
        {
            Headers = new Dictionary<string, StringValues>
            {
                {"", "" }
            }
        };

        var result = InvokeIsValidFileSection(section, out var _, out var _);

        Assert.False(result);  // Provoca continue;
    }

    private static void InvokeValidateRequestHeaders(HttpRequest request)
    {
        var method = typeof(MultipartFileProcessor)
            .GetMethod("ValidateRequestHeaders", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        method.Invoke(null, new object[] { request });
    }


    private static MultipartFileProcessor CreateProcessorWithSettings(long maxTotalFileSizeBytes)
    {
        var settings = new UploadSettings
        {
            MaxTotalFileSizeBytes = maxTotalFileSizeBytes,
            MaxFileCount = 10  // Lo que necesites para pruebas
        };

        return new MultipartFileProcessor(Microsoft.Extensions.Options.Options.Create(settings));
    }

    private static bool InvokeIsFileSizeValid(MultipartFileProcessor processor,
                                              long fileSize,
                                              long currentTotal,
                                              string fileName,
                                              List<NotSuccessFileResponse> notUploadedFiles)
    {
        var method = typeof(MultipartFileProcessor)
            .GetMethod("IsFileSizeValid", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);

        object[] parameters = new object[] { fileSize, currentTotal, fileName, notUploadedFiles };
        bool result = (bool)method.Invoke(processor, parameters)!;

        return result;
    }

    private static bool InvokeIsValidFileSection(MultipartSection section,
                                                 out ContentDispositionHeaderValue contentDisposition,
                                                 out string fileName)
    {
        var method = typeof(MultipartFileProcessor)
            .GetMethod("IsValidFileSection", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Prepara los parámetros (2 son out)
        object[] parameters = new object[] { section, null!, null! };

        bool result = (bool)method.Invoke(null, parameters)!;

        // Recupera los valores de los 'out' parameters
        contentDisposition = (ContentDispositionHeaderValue?)parameters[1]!;
        fileName = (string?)parameters[2]!;

        return result;
    }
}
