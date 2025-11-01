using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Utilities.Core.Shared.Services;

namespace Utilities.Core.Shared.Wrappers
{
    #region "Core"

    public interface ICommand<TResponse> { Type ResponseType => typeof(TResponse); }
    public interface IQueryParams { }
    public abstract record CommandWithoutParams<TResponse> : ICommand<Result<TResponse>>;
    public abstract record CommandWithOptionalParams<TResponse> : ICommand<Result<TResponse>>;
    public abstract record CommandFromQuery<TQueryParams, TResponse>(TQueryParams Query)
        : ICommand<Result<TResponse>> where TQueryParams : IQueryParams;

    #endregion

    #region "Common requests"

    public sealed record JsonToChainRequest(object Payload) : ICommand<Result<CommonResponse>>;
    public sealed record ChainToStringRequest(string Payload) : ICommand<Result<object>>;

    public sealed record UploadRequest
    (
        string Descripcion,
        Stream Archivo
    ) : ICommand<Result<SuccessFileResponse>>;

    #endregion

    #region "Common responses"

    public sealed record CommonResponse(string cadena);

    public sealed record ResultUploadRespose
    (
        DetailSuccessFileListResponse UploadesFiles,
        List<NotSuccessFileResponse> NotUploadesFiles
    );

    public sealed record SuccessFileResponse
    (
        string Archivo,
        long Tamanio
    );

    public sealed record DetailSuccessFileListResponse
    (
        List<SuccessFileResponse> UploadesFiles,
        int TotalFilesUploaded,
        long TotalSizeUploaded
    );

    public sealed record NotSuccessFileResponse
    (
        string Archivo
    );

    #endregion

    public interface ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        TResponse Handle(TCommand command);
        Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
