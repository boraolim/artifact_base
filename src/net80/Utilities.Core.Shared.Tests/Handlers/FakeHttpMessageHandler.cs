namespace Utilities.Core.Shared.Tests.Handlers;

public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }
    private readonly HttpResponseMessage _response;
    private readonly Exception? _exception;

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    public FakeHttpMessageHandler(Exception ex)
    {
        _exception = ex;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        if(_exception != null)
            throw _exception;

        return Task.FromResult(_response);
    }
}
