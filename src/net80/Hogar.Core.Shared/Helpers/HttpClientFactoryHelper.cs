using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Hogar.Core.Shared.Helpers;

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

    public static async Task<HttpResponseMessage> SendAsync(IHostEnvironment env,
                                                            ILogger logger,
                                                            HttpMethod method,
                                                            string url,
                                                            object? body = null,
                                                            Dictionary<string, string>? headers = null,
                                                            IEnumerable<(string Name, Stream Content, string FileName)>? files = null,
                                                            CancellationToken cancellationToken = default,
                                                            int maxLogRequestLength = 1024,
                                                            int maxLogResponseLength = 1024,
                                                            bool logBodies = true)// 🔹 Habilita/deshabilita logging de cuerpos
    {
        using var client = CreateHttpClient(env);
        using var request = new HttpRequestMessage(method, url);

        if(!headers.CheckIsNull())
        {
            foreach(var h in headers)
                request.Headers.TryAddWithoutValidation(h.Key, h.Value);
        }

        string? requestBodyLog = null;

        // Contenido del request
        if(files != null && files.Any())
        {
            var form = new MultipartFormDataContent();
            foreach(var file in files)
            {
                var streamContent = new StreamContent(file.Content);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream");
                form.Add(streamContent, file.Name, file.FileName);
            }

            if(!body.CheckIsNull())
            {
                var json = JsonSerializer.Serialize(body);
                form.Add(new StringContent(json, Encoding.UTF8, "application/json"), "data");
                if(logBodies) requestBodyLog = Truncate(json, maxLogRequestLength);
            }

            request.Content = form;
        }
        else if(!body.CheckIsNull())
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            if(logBodies) requestBodyLog = Truncate(json, maxLogRequestLength);
        }

        var stopwatch = Stopwatch.StartNew();

        if(logBodies)
            logger.LogInformation("➡️ Iniciando solicitud {Method} {Url} RequestBody: {RequestBody}", method, url, requestBodyLog);
        else
            logger.LogInformation("➡️ Iniciando solicitud {Method} {Url}", method, url);

        try
        {
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            stopwatch.Stop();

            string? responseBodyLog = null;
            if(logBodies && response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                responseBodyLog = Truncate(content, maxLogResponseLength);
            }

            if(logBodies)
            {
                logger.LogInformation(
                    "✅ Respuesta {Method} {Url} - {StatusCode} en {Elapsed} ms ResponseBody: {ResponseBody}",
                    method, url, (int)response.StatusCode, stopwatch.ElapsedMilliseconds, responseBodyLog
                );
            }
            else
            {
                logger.LogInformation(
                    "✅ Respuesta {Method} {Url} - {StatusCode} en {Elapsed} ms",
                    method, url, (int)response.StatusCode, stopwatch.ElapsedMilliseconds
                );
            }

            if(!response.IsSuccessStatusCode && logBodies)
            {
                logger.LogWarning(
                    "⚠️ Respuesta no exitosa ({StatusCode}) para {Method} {Url}: {ResponseBody}",
                    (int)response.StatusCode, method, url, responseBodyLog
                );
            }

            return response;
        }
        catch(TaskCanceledException ex) when(!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            logger.LogWarning("⚠️ Timeout ({Method} {Url}) después de {Elapsed} ms", method, url, stopwatch.ElapsedMilliseconds);
            throw new TimeoutException($"Tiempo de espera agotado en la solicitud {method} {url}", ex);
        }
        catch(HttpRequestException ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "❌ Error HTTP ({Method} {Url}) después de {Elapsed} ms", method, url, stopwatch.ElapsedMilliseconds);
            throw;
        }
        catch(Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "🔥 Error inesperado ({Method} {Url}) después de {Elapsed} ms: {Message}", method, url, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }

    private static string Truncate(string text, int maxLength)
    {
        if(string.IsNullOrEmpty(text)) return text!;
        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...(truncated)";
    }
}
