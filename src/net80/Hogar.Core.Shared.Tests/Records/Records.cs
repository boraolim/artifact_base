using Hogar.Core.Shared.Services;
using Hogar.Core.Shared.Wrappers;

namespace Hogar.Core.Shared.Tests.Records
{
    public sealed record SampleQueryParams : IQueryParams
    {
        public string Param { get; set; }
    }

    public sealed record SampleCommand(SampleQueryParams Query)
        : CommandFromQuery<SampleQueryParams, string>(Query);

    public record TestCommand(string Data) : ICommand<Result<string>>;
}
