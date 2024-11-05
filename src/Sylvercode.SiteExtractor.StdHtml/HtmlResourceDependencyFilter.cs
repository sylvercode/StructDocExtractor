using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.StdHtml;

public partial class HtmlResourceDependencyFilter(
    IOptions<HtmlResourceDependencyFilterOptions> filterOptions,
    IOptions<SiteExtractorOptions> siteExtractorOptions,
    ILogger<HtmlResourceDependencyFilter>? logger) :
    IResourceDependancyFilter

{
    private readonly ILogger<HtmlResourceDependencyFilter> _logger =
        logger ?? NullLogger<HtmlResourceDependencyFilter>.Instance;

    private readonly static ImageUriMatcher _imageUriMatcher = new();

    public bool IsAccepted(IStructDocReferencer referencer)
    {
        string nodeReference = referencer.GetReference();
        if (string.IsNullOrWhiteSpace(nodeReference))
            return false;

        HtmlResourceDependencyFilterMode filterMode =
            filterOptions.Value.GetFilterMode(referencer.GetReferenceType());

        using var scope = _logger.BeginScope((
            reference: nodeReference,
             type: referencer.GetReferenceType(),
             filterMode
        ));

        if (filterMode is HtmlResourceDependencyFilterMode.None)
        {
            LogRejectedSinceInNoneMode();
            return false;
        }

        if (filterMode.HasFlag(HtmlResourceDependencyFilterMode.InSourceBase))
        {
            Uri refUri = new(nodeReference);
            Uri sourceBaseUri = siteExtractorOptions.Value.GetSourceBaseUri();
            if (!refUri.IsAbsoluteUri)
                refUri = new(sourceBaseUri, refUri);

            if (!sourceBaseUri.IsBaseOf(refUri))
            {
                LogRejectedSinceNotInBase();
                return false;
            }
            LogBaseMatched();
        }

        if (filterMode.HasFlag(HtmlResourceDependencyFilterMode.IsImage))
        {
            if (!_imageUriMatcher.IsMatching(new Uri(nodeReference)))
            {
                LogRejectedSinceNotAnImage();
                return false;
            }
            LogIsAnImage();
        }

        if (filterMode.HasFlag(HtmlResourceDependencyFilterMode.IsNotImage))
        {
            if (_imageUriMatcher.IsMatching(new Uri(nodeReference)))
            {
                LogRejectedSinceIsAnImage();
                return false;
            }
            LogIsNotAnImage();
        }

        LogDependencyAccepted();
        return true;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Rejected since in None mode")]
    private partial void LogRejectedSinceInNoneMode();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Rejected since not in base")]
    private partial void LogRejectedSinceNotInBase();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Base matched")]
    private partial void LogBaseMatched();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Rejected since not an image")]
    private partial void LogRejectedSinceNotAnImage();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Is an image")]
    private partial void LogIsAnImage();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Rejected since is an image")]
    private partial void LogRejectedSinceIsAnImage();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Is not an image")]
    private partial void LogIsNotAnImage();

    [LoggerMessage(Level = LogLevel.Trace, Message = "Dependency accepted")]
    private partial void LogDependencyAccepted();
}
