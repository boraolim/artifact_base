namespace Utilities.Core.Shared.Tests.Handlers;

public class FakeOtherHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpResponseMessage> _responseFunc;
    private readonly Action? _throwAction;

    public FakeOtherHttpMessageHandler(Func<HttpResponseMessage> responseFunc)
    {
        _responseFunc = responseFunc;
    }

    public FakeOtherHttpMessageHandler(Action throwAction)
    {
        _throwAction = throwAction;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _throwAction?.Invoke();
        return Task.FromResult(_responseFunc());
    }
}
