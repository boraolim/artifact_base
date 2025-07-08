namespace Hogar.Core.Shared.Wrappers;

public interface IMultipartFileProcessor
{
    Task<Result<ResultUploadRespose>> ProcessAsync<TResult>(HttpRequest request, Func<string, Stream, Task<TResult>> processor);
}
