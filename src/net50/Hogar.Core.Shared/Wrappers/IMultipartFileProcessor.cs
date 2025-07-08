using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Bankaool.Core.Shared.Services;

namespace Bankaool.Core.Shared.Wrappers
{
    public interface IMultipartFileProcessor
    {
        Task<Result<ResultUploadRespose>> ProcessAsync<TResult>(HttpRequest request, Func<string, Stream, Task<TResult>> processor);
    }
}
