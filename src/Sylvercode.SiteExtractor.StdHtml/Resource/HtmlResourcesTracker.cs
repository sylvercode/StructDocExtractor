using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.StdHtml.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Resource;

public partial class HtmlResourcesTracker(ResourceDictionary resourceDictionary, IHtmlResourcesTrackerConfig config, ILogger<HtmlResourcesTracker>? logger)
    : BaseResourcesTracker(resourceDictionary, config.PullConfig)
{
    private readonly ILogger<HtmlResourcesTracker> _logger = logger ?? NullLogger<HtmlResourcesTracker>.Instance;

    protected override Uri? GetUri(IStructDocNode node)
    {
        if (node is not BaseHtmlHref href)
            return null;

        if (!Uri.TryCreate(href.Href, UriKind.RelativeOrAbsolute, out Uri? hrefUri))
        {
            LogInvalidHrefUri(href.Href);
            return null;
        }

        if (hrefUri.IsAbsoluteUri
            || config.BaseUri is null)
        {
            if (hrefUri.IsAbsoluteUri)
                LogAbsoluteHrefUri(href.Href);
            else
                LogNoBaseUri(href.Href);
            return hrefUri;
        }

        if (Uri.TryCreate(config.BaseUri, hrefUri, out Uri? fullUri))
        {
            LogFullUri(href.Href, fullUri.ToString());
            return fullUri;
        }

        LogIncompatibleBaseUri(href.Href, config.BaseUri);
        return hrefUri;
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Invalid href URI: {href}")]
    private partial void LogInvalidHrefUri(string href);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "The href URI is absolute: {href}")]
    private partial void LogAbsoluteHrefUri(string href);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "No base URI provided for the href URI: {href}")]
    private partial void LogNoBaseUri(string href);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "The full URI of <{href}> is: {fullUri}")]
    private partial void LogFullUri(string href, string fullUri);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "The base URI <{baseUri}> is incompatible with the href URI: {href}")]
    private partial void LogIncompatibleBaseUri(string href, Uri baseUri);
}
