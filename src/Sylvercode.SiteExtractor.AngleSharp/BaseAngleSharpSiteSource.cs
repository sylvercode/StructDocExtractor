using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.AngleSharp;

/// <summary>
/// Abstract base class that implements <see cref="ISiteSource{TData}"/> using AngleSharp to parse
/// and expose HTML documents as strongly-typed <see cref="INode"/> objects.
/// </summary>
/// <remarks>
/// Encapsulates AngleSharp configuration and <see cref="IBrowsingContext"/> lifecycle, handling
/// URI conversion and document-body extraction. Concrete subclasses must override
/// <see cref="DataExists"/> and <see cref="GetDocument"/> to define how documents are sourced
/// (live HTTP, proxy, file system, etc.) while the base class manages INode conversion and
/// common availability checks via the browsing context.
/// </remarks>
public abstract class BaseAngleSharpSiteSource : ISiteSource<INode>
{
    /// <summary>Gets the AngleSharp configuration used to initialise the browsing context.</summary>
    protected IConfiguration Config { get; }

    /// <summary>Gets the AngleSharp browsing context used for document navigation and parsing.</summary>
    protected IBrowsingContext BrowsingContext { get; }

    /// <summary>Gets the base URI that this source serves content from.</summary>
    public Uri BaseUri { get; protected set; }

    /// <summary>Initializes a new instance of <see cref="BaseAngleSharpSiteSource"/>.</summary>
    /// <param name="options">Site extractor options supplying the source base URI.</param>
    /// <param name="config">AngleSharp configuration; falls back to the default loader configuration when <see langword="null"/>.</param>
    /// <param name="browsingContext">AngleSharp browsing context; a new context is created from <paramref name="config"/> when <see langword="null"/>.</param>
    public BaseAngleSharpSiteSource(IOptions<SiteExtractorOptions> options, IConfiguration? config, IBrowsingContext? browsingContext)
    {
        BaseUri = options.Value.GetSourceBaseUri();
        Config = config ?? Configuration.Default.WithRequesters().WithDefaultLoader();
        BrowsingContext = browsingContext ?? global::AngleSharp.BrowsingContext.New(Config);
    }

    /// <summary>Converts a <see cref="Uri"/> to an AngleSharp <see cref="Url"/>.</summary>
    /// <param name="uri">The URI to convert.</param>
    /// <returns>An AngleSharp <see cref="Url"/> for use with browsing-context navigation.</returns>
    protected static Url AsAngleSharpUrl(Uri uri)
        => new(uri.ToString());

    /// <summary>
    /// Determines whether this source can provide content for the specified URI by checking the
    /// browsing context for a suitable navigation handler.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <returns><see langword="true"/> if a navigation handler is available; otherwise, <see langword="false"/>.</returns>
    public virtual bool CanGetFrom(Uri uri)
        => BrowsingContext.GetNavigationHandler(AsAngleSharpUrl(uri)) is not null;

    /// <summary>When overridden, determines whether data exists in this source for the specified URI.</summary>
    /// <param name="uri">The URI to check for existing data.</param>
    /// <returns><see langword="true"/> if data is available; otherwise, <see langword="false"/>.</returns>
    public abstract bool DataExists(Uri uri);

    /// <summary>
    /// Returns the <see cref="INode"/> body of the parsed document for the specified URI.
    /// </summary>
    /// <param name="uri">The URI of the resource to retrieve.</param>
    /// <returns>The <see cref="INode"/> representing the document body.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the parsed document has no body element.</exception>
    public virtual INode GetData(Uri uri)
    {
        IDocument document = GetDocument(uri); ;
        return document?.Body ?? throw new InvalidOperationException("Document has no body");
    }

    /// <summary>When overridden, retrieves the fully parsed <see cref="IDocument"/> for the given URI.</summary>
    /// <param name="uri">The URI of the resource to fetch and parse.</param>
    /// <returns>The AngleSharp <see cref="IDocument"/> representing the loaded page.</returns>
    protected abstract IDocument GetDocument(Uri uri);
}
