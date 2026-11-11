using System.Net;
using AngleSharp.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Network;
using Microsoft.Extensions.DependencyInjection;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.AngleSharp.Tests;

public class AngleSharpWebSourceHttpClientTests
{
    [Fact]
    public void AddAngleSharpWebSource_UsesFactoryHttpClientAndSharedCookieContainer()
    {
        Uri requestUri = new("https://example.test/page");
        string? cookieHeader = null;
        ServiceCollection services = new();
        services.AddLogging();
        services.Configure<SiteExtractorOptions>(options => options.SourceAuthority = "https://example.test");
        services.AddAngleSharpWebSource();
        services.AddHttpClient<HttpClientRequester>()
            .ConfigurePrimaryHttpMessageHandler(() => new CallbackHttpMessageHandler(request =>
            {
                cookieHeader = request.Headers.GetValues("Cookie").Single();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("<html><body>loaded</body></html>")
                };
            }));

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        CookieContainer cookieContainer = serviceProvider.GetRequiredService<CookieContainer>();
        cookieContainer.Add(requestUri, new Cookie("CobaltSession", "shared"));
        ISiteSource<INode> source = serviceProvider.GetRequiredService<ISiteSource<INode>>();

        INode documentBody = source.GetData(requestUri);

        MemoryCookieProvider cookieProvider = serviceProvider.GetRequiredService<MemoryCookieProvider>();
        Assert.NotNull(documentBody);
        Assert.Contains("CobaltSession=shared", cookieHeader);
        Assert.Same(cookieContainer, cookieProvider.Container);
        cookieProvider.SetCookie(new Url(requestUri.ToString()), "ServerCookie=updated; Path=/; Secure");
        Assert.Contains("ServerCookie=updated", cookieContainer.GetCookieHeader(requestUri));
    }

    private sealed class CallbackHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> send)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(send(request));
    }
}
