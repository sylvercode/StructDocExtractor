using System.Net;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class UriRedirectorTests
{
    [Fact]
    public void RedirectUri_FollowsRelativeAndAbsoluteRedirects()
    {
        Uri initialUri = new("https://example.test/old");
        Uri finalUri = new("https://cdn.example.test/new");
        List<Uri?> requestedUris = [];
        using HttpClient client = new(new CallbackHttpMessageHandler((request, _) =>
        {
            requestedUris.Add(request.RequestUri);
            HttpResponseMessage response = requestedUris.Count switch
            {
                1 => new(HttpStatusCode.Found)
                {
                    Headers = { Location = new Uri("/middle", UriKind.Relative) }
                },
                2 => new(HttpStatusCode.MovedPermanently)
                {
                    Headers = { Location = finalUri }
                },
                _ => new(HttpStatusCode.OK)
            };

            return Task.FromResult(response);
        }));
        HttpUriRedirector redirector = new HttpUriRedirector(client);

        bool redirected = redirector.RedirectUri(initialUri.ToString(), out string? redirectedUri);

        Assert.True(redirected);
        Assert.Equal(finalUri.ToString(), redirectedUri);
        Assert.Equal(
            [initialUri, new Uri("https://example.test/middle"), finalUri],
            requestedUris);
    }

    [Fact]
    public void RedirectUri_ReturnsFalseWhenResponseIsNotRedirect()
    {
        using HttpClient client = new(new CallbackHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        HttpUriRedirector redirector = new HttpUriRedirector(client);

        bool redirected = redirector.RedirectUri("https://example.test/page", out string? redirectedUri);

        Assert.False(redirected);
        Assert.Null(redirectedUri);
    }

    [Fact]
    public void RedirectUri_ReturnsFalseForRedirectLoop()
    {
        using HttpClient client = new(new CallbackHttpMessageHandler((request, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.Found)
            {
                Headers =
                {
                    Location = request.RequestUri!.AbsolutePath == "/first"
                        ? new Uri("/second", UriKind.Relative)
                        : new Uri("/first", UriKind.Relative)
                }
            })));
        HttpUriRedirector redirector = new HttpUriRedirector(client);

        bool redirected = redirector.RedirectUri("https://example.test/first", out string? redirectedUri);

        Assert.False(redirected);
        Assert.Null(redirectedUri);
    }

    [Fact]
    public void RedirectUri_ReturnsFalseWhenRedirectLimitIsReached()
    {
        int requestCount = 0;
        using HttpClient client = new(new CallbackHttpMessageHandler((_, _) =>
        {
            requestCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Found)
            {
                Headers = { Location = new Uri($"/path-{requestCount}", UriKind.Relative) }
            });
        }));
        HttpUriRedirector redirector = new HttpUriRedirector(client);

        bool redirected = redirector.RedirectUri("https://example.test/path-0", out string? redirectedUri);

        Assert.False(redirected);
        Assert.Null(redirectedUri);
        Assert.Equal(11, requestCount);
    }

    [Fact]
    public void RedirectUri_ReturnsFalseWhenRequestFails()
    {
        using HttpClient client = new(new CallbackHttpMessageHandler((_, _) =>
            Task.FromException<HttpResponseMessage>(new HttpRequestException("Request failed"))));
        HttpUriRedirector redirector = new HttpUriRedirector(client);

        bool redirected = redirector.RedirectUri("https://example.test/page", out string? redirectedUri);

        Assert.False(redirected);
        Assert.Null(redirectedUri);
    }

    private sealed class CallbackHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => sendAsync(request, cancellationToken);
    }
}
