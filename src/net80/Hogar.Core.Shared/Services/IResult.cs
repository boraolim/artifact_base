namespace Hogar.Core.Shared.Services;

public interface IResult<T>
{
    bool Succeded { get; set; }
    string TraceId { get; set; }
    string MessageDescription { get; set; }
    uint StatusCode { get; set; }
    DateTime? TimeStamp { get; set; }
    Exception ExceptionInfo { get; set; }
    T Data { get; set; }
    Dictionary<string, string> ErrorDetail { get; set; }
    string UrlPathDetails { get; set; }
}
