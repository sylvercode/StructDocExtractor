using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Resolves a URI by inspecting HTTP redirect responses.</summary>
public partial class HttpUriRedirector(HttpClient httpClient, ILogger<HttpUriRedirector>? logger = null) : IUriRedirector
{
    private const int MaxRedirects = 10;

    private readonly ILogger<HttpUriRedirector> _logger = logger ?? NullLogger<HttpUriRedirector>.Instance;

    /// <inheritdoc/>
    public bool RedirectUri(string uri, [NotNullWhen(true)] out string? redirectedUri)
    {
        redirectedUri = null;
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri? currentUri) || !IsHttpUri(currentUri))
            return false;

        Uri originalUri = currentUri;
        HashSet<Uri> visitedUris = [originalUri];
        bool hasRedirected = false;

        for (int redirectCount = 0; redirectCount <= MaxRedirects; redirectCount++)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, currentUri);
            HttpResponseMessage response;
            try
            {
                response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException)
            {
                LogProbeFailed(currentUri, exception);
                return false;
            }

            using (response)
            {
                if (!IsRedirect(response.StatusCode) || response.Headers.Location is not Uri location)
                {
                    if (hasRedirected)
                        redirectedUri = currentUri.AbsoluteUri;

                    return hasRedirected;
                }

                if (redirectCount == MaxRedirects)
                {
                    LogRedirectLimit(originalUri);
                    return false;
                }

                Uri targetUri = location.IsAbsoluteUri ? location : new Uri(currentUri, location);
                if (!IsHttpUri(targetUri))
                {
                    redirectedUri = targetUri.AbsoluteUri;
                    return true;
                }

                if (!visitedUris.Add(targetUri))
                {
                    LogRedirectLoop(originalUri, targetUri);
                    return false;
                }

                currentUri = targetUri;
                hasRedirected = true;
            }
        }

        return false;
    }

    private static bool IsHttpUri(Uri uri)
        => uri.Scheme is "http" or "https";

    private static bool IsRedirect(HttpStatusCode statusCode)
        => statusCode is HttpStatusCode.MultipleChoices
            or HttpStatusCode.MovedPermanently
            or HttpStatusCode.Found
            or HttpStatusCode.SeeOther
            or HttpStatusCode.TemporaryRedirect
            or HttpStatusCode.PermanentRedirect;

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to inspect redirect response from {Uri}")]
    private partial void LogProbeFailed(Uri uri, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Redirect limit reached while resolving {Uri}")]
    private partial void LogRedirectLimit(Uri uri);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Redirect loop detected while resolving {Uri} to {TargetUri}")]
    private partial void LogRedirectLoop(Uri uri, Uri targetUri);
}
