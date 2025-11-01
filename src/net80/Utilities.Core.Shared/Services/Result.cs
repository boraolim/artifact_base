namespace Utilities.Core.Shared.Services;

public class Result<T> : IResult<T>
{
    public bool Succeded { get; set; }
    public string TraceId { get; set; }
    public string MessageDescription { get; set; }
    public uint StatusCode { get; set; }
    [property: JsonConverter(typeof(DateTimeJsonConverter))]
    public DateTime? TimeStamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> ErrorDetail { get; set; }
    public T Data { get; set; }
    public Exception ExceptionInfo { get; set; }
    public string UrlPathDetails { get; set; }

    #region Non Async Methods

    #region Success Methods

    public static Result<T> Success() =>
        new Result<T> { Succeded = true };
    public static Result<T> Success(string messageSuccessfull) =>
        new Result<T> { Succeded = true, MessageDescription = messageSuccessfull };
    public static Result<T> Success(T data) =>
        new Result<T> { Succeded = true, Data = data };
    public static Result<T> Success(T data, string message, uint statusCode = (uint)HttpStatusCode.Created) =>
        new Result<T> { Succeded = true, MessageDescription = message, Data = data, StatusCode = statusCode };

    #endregion

    #region Failure Methods

    public static Result<T> Failure() =>
        new Result<T> { Succeded = false };
    public static Result<T> Failure(string messageFail, uint statusCode = (uint)HttpStatusCode.BadRequest) =>
        new Result<T> { Succeded = false, MessageDescription = messageFail, StatusCode = statusCode };
    public static Result<T> Failure(Dictionary<string, string> errors) =>
        new Result<T> { Succeded = false, ErrorDetail = errors };
    public static Result<T> Failure(T data) =>
        new Result<T> { Succeded = false, Data = data };
    public static Result<T> Failure(T data, string messageFail) =>
        new Result<T> { Succeded = false, MessageDescription = messageFail, Data = data };
    public static Result<T> Failure(T data, Dictionary<string, string> errors) =>
        new Result<T> { Succeded = false, ErrorDetail = errors, Data = data };
    public static Result<T> Failure(string messageFail, Dictionary<string, string> errors) =>
        new Result<T> { Succeded = false, ErrorDetail = errors, MessageDescription = messageFail };
    public static Result<T> Failure(string messageFail, Exception exceptionInfo, Dictionary<string, string> errors) =>
        new Result<T> { Succeded = false, ErrorDetail = errors, MessageDescription = messageFail, ExceptionInfo = exceptionInfo };
    public static Result<T> Failure(Exception exceptionInfo) =>
        new Result<T> { Succeded = false, ExceptionInfo = exceptionInfo };

    #endregion

    #endregion

    #region Async Methods

    #region Success Methods

    public static Task<Result<T>> SuccessAsync() =>
        Task.FromResult(Success());
    public static Task<Result<T>> SuccessAsync(string messageSuccessfull) =>
        Task.FromResult(Success(messageSuccessfull));
    public static Task<Result<T>> SuccessAsync(T data) =>
        Task.FromResult(Success(data));
    public static Task<Result<T>> SuccessAsync(T data, string messageSucessfull) =>
        Task.FromResult(Success(data, messageSucessfull));

    #endregion

    #region Failure Methods

    public static Task<Result<T>> FailureAsync() =>
        Task.FromResult(Failure());
    public static Task<Result<T>> FailureAsync(string messageFail) =>
        Task.FromResult(Failure(messageFail));
    public static Task<Result<T>> FailureAsync(Dictionary<string, string> errors) =>
        Task.FromResult(Failure(errors));
    public static Task<Result<T>> FailureAsync(T data) =>
        Task.FromResult(Failure(data));
    public static Task<Result<T>> FailureAsync(T data, string messageFail) =>
        Task.FromResult(Failure(data, messageFail));
    public static Task<Result<T>> FailureAsync(T data, Dictionary<string, string> errors) =>
        Task.FromResult(Failure(data, errors));
    public static Task<Result<T>> FailureAsync(string messageFail, Dictionary<string, string> errors) =>
        Task.FromResult(Failure(messageFail, errors));
    public static Task<Result<T>> FailureAsync(string messageFail, Exception exception, Dictionary<string, string> errors) =>
        Task.FromResult(Failure(messageFail, exception, errors));
    public static Task<Result<T>> FailureAsync(Exception exceptionInfo) =>
        Task.FromResult(Failure(exceptionInfo));

    #endregion

    #endregion

    #region "Bind"

    public Result<U> Bind<U>(Func<T, Result<U>> next)
    {
        if (!Succeded) return Result<U>.Failure(MessageDescription);
        return next(Data);
    }

    public async Task<Result<U>> BindAsync<U>(Func<T, Task<Result<U>>> next)
    {
        if (!Succeded) return await Result<U>.FailureAsync(MessageDescription);
        return await next(Data);
    }

    #endregion

    #region "Map"

    public Result<U> Map<U>(Func<T, U> transform)
    {
        if (!Succeded) return Result<U>.Failure(MessageDescription);
        return Result<U>.Success(transform(Data));
    }

    public async Task<Result<U>> MapAsync<U>(Func<T, Task<U>> transform)
    {
        if (!Succeded) return await Result<U>.FailureAsync(MessageDescription);
        var newData = await transform(Data);
        return await Result<U>.SuccessAsync(newData);
    }

    #endregion

    #region "TryCatch"

    public static Result<T> TryCatch(Func<T> next, string MessageFail = "Ocurrió un error inesperado")
    {
        try
        {
            var data = next();
            return Success(data);
        }
        catch (Exception error)
        {
            return Failure($"{MessageFail} : {error.Message}");
        }
    }

    public static async Task<Result<T>> TryCatchAsync(Func<Task<T>> next, string MessageFail = "Ocurrió un error inesperado")
    {
        try
        {
            var data = await next();
            return await SuccessAsync(data);
        }
        catch (Exception error)
        {
            return await FailureAsync($"{MessageFail}: {error.Message}");
        }
    }

    #endregion

    #region "Match"

    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (Succeded)
        {
            onSuccess?.Invoke(Data);
        }
        else
        {
            onFailure?.Invoke(MessageDescription);
        }
    }

    public async Task MatchAsync(Func<T, Task> onSuccess, Func<string, Task> onFailure)
    {
        if (Succeded)
        {
            if (onSuccess != null) await onSuccess(Data);
        }
        else
        {
            if (onFailure != null) await onFailure(MessageDescription);
        }
    }

    #endregion

}
