using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Utilities.Core.Shared.Services;

namespace Utilities.Core.Shared.Wrappers
{
    public interface IMultipartFileProcessor
    {
        Task<Result<ResultUploadRespose>> ProcessAsync<TResult>(HttpRequest request, Func<string, Stream, Task<TResult>> processor);
    }
}
