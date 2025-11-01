namespace Utilities.Core.Shared.Helpers;

public static class HttpClientFactoryHelper
{
    public static HttpClient CreateHttpClient(IHostEnvironment environment)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        var handler = new HttpClientHandler();

        if(environment.IsDevelopment())
        {
            handler.ServerCertificateCustomValidationCallback =
                (sender, cert, chain, SslPolicyErrors) => true;
        }
        else
        {
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        var client = new HttpClient(handler)
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.ConnectionClose = true;
        client.DefaultRequestHeaders.Add("Connection", "Close");
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        return client;
    }

    public static async Task<HttpResponseMessage> SendAsync(
            IHostEnvironment env,
            ILogger logger,
            HttpMethod method,
            string url,
            TimeSpan? timeout = default,
            Dictionary<string, string>? headers = null,
            object? body = default,
            IEnumerable<(string Name, Stream Content, string FileName)>? files = default,
            CancellationToken cancellationToken = default,
            bool logBodies = true,
            int maxLogRequestLength = 1024,
            int maxLogResponseLength = 1024)
    {
        using var client = CreateHttpClient(env);
        using var request = new HttpRequestMessage(method, url);

        if(timeout.HasValue)
            client.Timeout = timeout.Value;

        AddHeaders(request, headers);

        string? requestBodyLog = BuildRequestContent(request, body, files, logBodies, maxLogRequestLength);

        LogRequestStart(logger, method, url, logBodies, requestBodyLog);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            stopwatch.Stop();

            await LogResponseAsync(logger, response, method, url, stopwatch.ElapsedMilliseconds, logBodies, maxLogResponseLength);

            return response;
        }
        catch(TaskCanceledException ex) when(!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            LogTimeout(logger, method, url, stopwatch.ElapsedMilliseconds);
            throw new TimeoutException($"Tiempo de espera agotado en la solicitud {method} {url}", ex);
        }
        catch(HttpRequestException ex)
        {
            stopwatch.Stop();
            LogHttpError(logger, ex, method, url, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch(Exception ex)
        {
            stopwatch.Stop();
            LogUnexpectedError(logger, ex, method, url, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    // 🔹 Submétodos privados — reducen complejidad

    private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
    {
        if(headers.CheckIsNull()) return;

        foreach(var h in headers)
            request.Headers.TryAddWithoutValidation(h.Key, h.Value);
    }

    private static string? BuildRequestContent(
        HttpRequestMessage request,
        object? body,
        IEnumerable<(string Name, Stream Content, string FileName)>? files,
        bool logBodies,
        int maxLogRequestLength)
    {
        string? requestBodyLog = null;

        if(files != null && files.Any())
        {
            var form = new MultipartFormDataContent();

            foreach(var file in files)
            {
                var streamContent = new StreamContent(file.Content);
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream");
                form.Add(streamContent, file.Name, file.FileName);
            }

            if(!body.CheckIsNull())
            {
                var json = JsonSerializer.Serialize(body);
                form.Add(new StringContent(json, Encoding.UTF8, "application/json"), "data");
                if(logBodies) requestBodyLog = Functions.TruncateText(json, maxLogRequestLength);
            }

            request.Content = form;
        }
        else if(!body.CheckIsNull())
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            if(logBodies) requestBodyLog = Functions.TruncateText(json, maxLogRequestLength);
        }

        return requestBodyLog;
    }

    private static void LogRequestStart(ILogger logger, HttpMethod method, string url, bool logBodies, string? body)
    {
        if(logBodies)
            logger.LogInformation("➡️ Iniciando solicitud {Method} {Url} RequestBody: {RequestBody}", method, url, body);
        else
            logger.LogInformation("➡️ Iniciando solicitud {Method} {Url}", method, url);
    }

    private static async Task LogResponseAsync(ILogger logger,
                                               HttpResponseMessage response,
                                               HttpMethod method,
                                               string url,
                                               long elapsedMs,
                                               bool logBodies,
                                               int maxLogResponseLength)
    {
        string? responseBodyLog = null;

        if(logBodies && response.Content != null)
        {
            var content = await response.Content.ReadAsStringAsync();
            responseBodyLog = Functions.TruncateText(content, maxLogResponseLength);
        }

        if(logBodies)
        {
            logger.LogInformation("✅ Respuesta {Method} {Url} - {StatusCode} en {Elapsed} ms ResponseBody: {ResponseBody}",
                method, url, (int)response.StatusCode, elapsedMs, responseBodyLog);
        }
        else
        {
            logger.LogInformation("✅ Respuesta {Method} {Url} - {StatusCode} en {Elapsed} ms",
                method, url, (int)response.StatusCode, elapsedMs);
        }

        if(!response.IsSuccessStatusCode && logBodies)
        {
            logger.LogWarning("⚠️ Respuesta no exitosa ({StatusCode}) para {Method} {Url}: {ResponseBody}",
                (int)response.StatusCode, method, url, responseBodyLog);
        }
    }

    private static void LogTimeout(ILogger logger, HttpMethod method, string url, long elapsedMs)
        => logger.LogWarning("⚠️ Timeout ({Method} {Url}) después de {Elapsed} ms", method, url, elapsedMs);

    private static void LogHttpError(ILogger logger, HttpRequestException ex, HttpMethod method, string url, long elapsedMs)
        => logger.LogError(ex, "❌ Error HTTP ({Method} {Url}) después de {Elapsed} ms", method, url, elapsedMs);

    private static void LogUnexpectedError(ILogger logger, Exception ex, HttpMethod method, string url, long elapsedMs)
        => logger.LogError(ex, "🔥 Error inesperado ({Method} {Url}) después de {Elapsed} ms: {Message}", method, url, elapsedMs, ex.Message);
}
