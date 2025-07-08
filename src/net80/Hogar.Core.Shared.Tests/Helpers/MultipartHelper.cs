namespace Hogar.Core.Shared.Tests.Helpers;

public static class MultipartHelper
{
    public static DefaultHttpContext CreateHttpContextWithFile(long sizeBytes = 1024)
    {
        return CreateHttpContextWithMultipleFiles(1, sizeBytes);
    }

    public static DefaultHttpContext CreateHttpContextWithMultipleFiles(int fileCount, long sizeBytes = 1024)
    {
        var boundary = "----WebKitFormBoundaryTest";
        var context = new DefaultHttpContext();
        context.Request.ContentType = $"multipart/form-data; boundary={boundary}";

        var ms = new MemoryStream();
        using(var writer = new StreamWriter(ms, leaveOpen: true))
        {
            for(int i = 0; i < fileCount; i++)
            {
                writer.WriteLine($"--{boundary}");
                writer.WriteLine($"Content-Disposition: form-data; name=\"file{i}\"; filename=\"test{i}.txt\"");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine();
                writer.WriteLine(new string('a', (int)sizeBytes));
            }
            writer.WriteLine($"--{boundary}--");
        }
        ms.Position = 0;
        context.Request.Body = ms;

        return context;
    }
}
