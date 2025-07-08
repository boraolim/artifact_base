using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Hogar.Core.Shared.Services;

namespace Hogar.Core.Shared.Wrappers
{
    public interface IMultipartFileProcessor
    {
        Task<Result<ResultUploadRespose>> ProcessAsync<TResult>(HttpRequest request, Func<string, Stream, Task<TResult>> processor);
    }
}
