namespace Hogar.Core.Shared.Wrappers;

public interface ICommandDispatcher
{
    Task<Result<TResponse>> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<Result<TResponse>>;

    Result<TResponse> Dispatch<TCommand, TResponse>(TCommand command)
            where TCommand : ICommand<Result<TResponse>>;
}

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _contextAccessor;

    public CommandDispatcher(IServiceProvider serviceProvider,
                             IHttpContextAccessor contextAccessor) =>
        (_serviceProvider, _contextAccessor) =
        (Guard.AgainstNull(serviceProvider, nameof(serviceProvider)),
         Guard.AgainstNull(contextAccessor, nameof(contextAccessor)));

    public Result<TResponse> Dispatch<TCommand, TResponse>(TCommand command)
        where TCommand : ICommand<Result<TResponse>>
    {
        Validate(command);
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, Result<TResponse>>>();
        var result = handler.Handle(command);
        result.TraceId = GetTraceId();
        return result;
    }

    public async Task<Result<TResponse>> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<Result<TResponse>>
    {
        await ValidateAsync(command, cancellationToken);
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, Result<TResponse>>>();
        var result = await handler.HandleAsync(command, cancellationToken);
        result.TraceId = GetTraceId();
        return result;
    }

    private void Validate<TCommand>(TCommand command)
    {
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();

        if (validator is not null)
        {
            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
                throw new CommonValidationException(validationResult.Errors.ToList());
        }
    }

    private async Task ValidateAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                throw new CommonValidationException(validationResult.Errors.ToList());
        }
    }

    private string GetTraceId()
    {
        _contextAccessor.HttpContext.Request.Headers.TryGetValue(MainConstantsCore.CFG_TRACE_ID_HEADER, out var traceId);
        return string.IsNullOrWhiteSpace(traceId) ? null : traceId.ToString();
    }
}
