using System.Collections.Generic;
using System.IO;
using Utilities.Core.Shared.Exceptions;
using Utilities.Core.Shared.Tests.Constants;
using Utilities.Core.Shared.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using MainConstantsCore = Utilities.Core.Shared.Constants.MainConstants;

namespace Utilities.Core.Shared.Tests.Helpers
{
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

        public static bool TryParseFileSection(string contentDispositionHeader,
                                               out ContentDispositionHeaderValue contentDisposition,
                                               out string fileName)
        {
            fileName = null;
            contentDisposition = null;

            if(!ContentDispositionHeaderValue.TryParse(contentDispositionHeader, out contentDisposition))
                return false;

            fileName = Path.GetFileName(contentDisposition.FileName.Value ?? string.Empty);

            return !string.IsNullOrWhiteSpace(fileName) &&
                   contentDisposition.DispositionType == MainConstantsCore.CFG_PREFIX_FORM_DATA;
        }
    }
}
