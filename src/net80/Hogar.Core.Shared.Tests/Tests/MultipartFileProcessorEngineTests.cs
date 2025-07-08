namespace Hogar.Core.Shared.Tests;

public class MultipartFileProcessorEngineTests
{
    private static MultipartFileProcessor CreateProcessor()
    {
        var settings = new UploadSettings
        {
            MaxFileCount = 2,
            MaxTotalFileSizeBytes = 1024 * 1024 * 5, // 5MB
            AllowedFileExtensions = new[] { ".txt" },
            UploadDirectory = "/uploads"
        };

        return new MultipartFileProcessor(Options.Create(settings));
    }

    [Fact]
    public async Task ProcessAsync_Should_Handle_Processor_Failure()
    {
        var processor = CreateProcessor();
        var context = MultipartHelper.CreateHttpContextWithMultipleFiles(1);

        var result = await processor.ProcessAsync<FakeResult<object>>(
            context.Request,
            (_, _) => Task.FromResult(new FakeResult<object> { Succeded = false }));

        Assert.True(result.Succeded);
        Assert.Empty(result.Data.UploadesFiles.UploadesFiles);
        Assert.Single(result.Data.NotUploadesFiles);
    }

    [Fact]
    public async Task ProcessAsync_Should_Throw_Exception_When_FileCount_Exceeded()
    {
        var processor = CreateProcessor();
        var context = MultipartHelper.CreateHttpContextWithMultipleFiles(3); // Excede límite

        await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await processor.ProcessAsync<FakeResult<object>>(
                context.Request,
                (_, _) => Task.FromResult(new FakeResult<object> { Succeded = true }));
        });
    }

    [Fact]
    public async Task ProcessAsync_Should_Throw_Exception_When_ContentType_Invalid()
    {
        var processor = CreateProcessor();
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json"; // No es multipart

        await Assert.ThrowsAsync<UnsupportedMediaTypeException>(async () =>
        {
            await processor.ProcessAsync<FakeResult<object>>(
                context.Request,
                (_, _) => Task.FromResult(new FakeResult<object> { Succeded = true }));
        });
    }

    [Fact]
    public async Task ProcessAsync_Should_Throw_Exception_When_EmptyRequest()
    {
        var processor = CreateProcessor();
        var context = MultipartHelper.CreateHttpContextWithMultipleFiles(0); // Sin archivos

        await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await processor.ProcessAsync<FakeResult<object>>(
                context.Request,
                (_, _) => Task.FromResult(new FakeResult<object> { Succeded = true }));
        });
    }

    [Fact]
    public void NotSuccessFileResponse_Should_Set_Archivo_Property()
    {
        var response = new NotSuccessFileResponse("archivo_fallo.txt");

        Assert.Equal("archivo_fallo.txt", response.Archivo);
    }

    [Fact]
    public void NotSuccessFileResponse_Two_Instances_With_Same_Values_Should_Be_Equal()
    {
        var response1 = new NotSuccessFileResponse("archivo_duplicado.txt");
        var response2 = new NotSuccessFileResponse("archivo_duplicado.txt");

        Assert.Equal(response1, response2);
        Assert.True(response1.Equals(response2));
    }

    [Fact]
    public void NotSuccessFileResponse_Should_Serialize_And_Deserialize_Correctly()
    {
        var response = new NotSuccessFileResponse("archivo_serializado.txt");
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<NotSuccessFileResponse>(json);

        Assert.Equal(response, deserialized);
    }

    [Fact]
    public void NotSuccessFileResponse_Should_Be_Usable_In_Collections()
    {
        var list = new List<NotSuccessFileResponse>
        {
            new NotSuccessFileResponse("archivo1.txt"),
            new NotSuccessFileResponse("archivo2.txt")
        };

        Assert.Contains(list, item => item.Archivo == "archivo2.txt");
    }

    [Fact]
    public void SuccessFileResponse_Should_Set_Properties_Correctly()
    {
        var response = new SuccessFileResponse("archivo_exitoso.txt", 1024);

        Assert.Equal("archivo_exitoso.txt", response.Archivo);
        Assert.Equal(1024, response.Tamanio);
    }

    [Fact]
    public void SuccessFileResponse_Two_Instances_With_Same_Values_Should_Be_Equal()
    {
        var response1 = new SuccessFileResponse("archivo_igual.txt", 2048);
        var response2 = new SuccessFileResponse("archivo_igual.txt", 2048);

        Assert.Equal(response1, response2);
        Assert.True(response1.Equals(response2));
    }

    [Fact]
    public void SuccessFileResponse_Should_Serialize_And_Deserialize_Correctly()
    {
        var response = new SuccessFileResponse("archivo_serializado.txt", 4096);
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<SuccessFileResponse>(json);

        Assert.Equal(response, deserialized);
    }

    [Fact]
    public void SuccessFileResponse_Should_Be_Usable_In_Collections()
    {
        var list = new List<SuccessFileResponse>
        {
            new SuccessFileResponse("archivo1.txt", 1000),
            new SuccessFileResponse("archivo2.txt", 2000)
        };

        Assert.Contains(list, item => item.Archivo == "archivo2.txt" && item.Tamanio == 2000);
    }

    [Fact]
    public void DetailSuccessFileListResponse_Should_Set_Properties_Correctly()
    {
        var uploadedFiles = new List<SuccessFileResponse>
        {
            new SuccessFileResponse("file1.txt", 1000),
            new SuccessFileResponse("file2.txt", 2000)
        };

        var response = new DetailSuccessFileListResponse(uploadedFiles, uploadedFiles.Count, 3000);

        Assert.Equal(uploadedFiles, response.UploadesFiles);
        Assert.Equal(2, response.TotalFilesUploaded);
        Assert.Equal(3000, response.TotalSizeUploaded);
    }

    [Fact]
    public void DetailSuccessFileListResponse_Two_Instances_With_Same_Values_Should_Be_Equal()
    {
        var list1 = new List<SuccessFileResponse>
        {
            new SuccessFileResponse("file1.txt", 1000),
            new SuccessFileResponse("file2.txt", 2000)
        };
        var list2 = new List<SuccessFileResponse>
        {
            new SuccessFileResponse("file1.txt", 1000),
            new SuccessFileResponse("file2.txt", 2000)
        };

        var response1 = new DetailSuccessFileListResponse(list1, 2, 3000);
        var response2 = new DetailSuccessFileListResponse(list2, 2, 3000);

        Assert.Equal(response1.TotalFilesUploaded, response2.TotalFilesUploaded);
        Assert.Equal(response1.TotalSizeUploaded, response2.TotalSizeUploaded);

        Assert.Equal(response1.UploadesFiles.Count, response2.UploadesFiles.Count);
        for(int i = 0; i < response1.UploadesFiles.Count; i++)
        {
            Assert.Equal(response1.UploadesFiles[i], response2.UploadesFiles[i]);
        }
    }

    [Fact]
    public void DetailSuccessFileListResponse_Should_Serialize_And_Deserialize_Correctly()
    {
        var uploadedFiles = new List<SuccessFileResponse>
        {
            new SuccessFileResponse("file1.txt", 1000),
            new SuccessFileResponse("file2.txt", 2000)
        };

        var response = new DetailSuccessFileListResponse(uploadedFiles, 2, 3000);
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<DetailSuccessFileListResponse>(json);

        Assert.NotNull(deserialized);
        Assert.Equal(response.TotalFilesUploaded, deserialized.TotalFilesUploaded);
        Assert.Equal(response.TotalSizeUploaded, deserialized.TotalSizeUploaded);
        Assert.Equal(response.UploadesFiles.Count, deserialized.UploadesFiles.Count);

        for(int i = 0; i < response.UploadesFiles.Count; i++)
        {
            Assert.Equal(response.UploadesFiles[i], deserialized.UploadesFiles[i]);
        }
    }

    [Fact]
    public void DetailSuccessFileListResponse_List_Should_Contain_Expected_Elements()
    {
        var uploadedFiles = new List<SuccessFileResponse>
        {
            new SuccessFileResponse("file1.txt", 1000),
            new SuccessFileResponse("file2.txt", 2000)
        };

        var response = new DetailSuccessFileListResponse(uploadedFiles, 2, 3000);

        Assert.Contains(response.UploadesFiles, file => file.Archivo == "file2.txt" && file.Tamanio == 2000);
        Assert.DoesNotContain(response.UploadesFiles, file => file.Archivo == "file3.txt");
    }
}
