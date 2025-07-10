using System.Net.Http;

namespace Hogar.Core.Shared.Tests.Helpers;

public static class MultipartHelper
{
    public static DefaultHttpContext CreateHttpContextWithFile(long sizeBytes = 1024)
    {
        return CreateHttpContextWithMultipleFiles(1, sizeBytes);
    }

    public static DefaultHttpContext CreateHttpContextWithMultipleFiles(int fileCount, long sizeBytes = 1024)
    {
        var boundary = "test-boundary";
        var content = new MultipartFormDataContent(boundary);

        if(fileCount > 0)
        {
            for(int i = 0; i < fileCount; i++)
            {
                var fileContent = new StringContent("Dummy file content");
                content.Add(fileContent, "file", $"file{i + 1}.txt");
            }
        }

        var stream = new MemoryStream();
        content.CopyToAsync(stream).Wait();
        stream.Position = 0;

        var context = new DefaultHttpContext();
        context.Request.ContentType = $"multipart/form-data; boundary={boundary}";
        context.Request.Body = stream;

        return context;
    }
}
