using System;
using System.Threading;
using System.Threading.Tasks;

using Bankaool.Core.Shared.Services;
using Bankaool.Core.Shared.Wrappers;
using Bankaool.Core.Shared.Tests.Records;


namespace Bankaool.Core.Shared.Tests.Handlers
{
    public sealed class JsonToChainHandler : ICommandHandler<JsonToChainRequest, Result<CommonResponse>>
    {
        public Result<CommonResponse> Handle(JsonToChainRequest command)
        {
            var cadena = command.Payload.ToString();
            return Result<CommonResponse>.Success(new CommonResponse(cadena));
        }

        public Task<Result<CommonResponse>> HandleAsync(JsonToChainRequest command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Handle(command));
        }
    }

    public sealed class TestCommandHandler : ICommandHandler<TestCommand, Result<string>>
    {
        public Result<string> Handle(TestCommand command) =>
            Result<string>.Success($"Handled: {command.Data}");

        public Task<Result<string>> HandleAsync(TestCommand command, CancellationToken cancellationToken) =>
            Task.FromResult(Result<string>.Success($"HandledAsync: {command.Data}"));
    }

    public sealed class TestChainToStringHandler : ICommandHandler<ChainToStringRequest, Result<object>>
    {
        public Result<object> Handle(ChainToStringRequest command)
            => throw new NotImplementedException(); // Solo probamos Async en esta prueba

        public async Task<Result<object>> HandleAsync(ChainToStringRequest command, CancellationToken cancellationToken)
        {
            return await Result<object>.SuccessAsync(new { Message = $"HandledAsync: {command.Payload}" }, $"HandledAsync: {command.Payload}");
        }
    }
}
