using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.AngleSharp;

public abstract class BaseAngleSharpSiteSource : ISiteSource<INode>
{
    protected IConfiguration Config { get; }

    protected IBrowsingContext BrowsingContext { get; }

    public Uri BaseUri { get; protected set; }

    public BaseAngleSharpSiteSource(IOptions<SiteExtractorOptions> options, IConfiguration? config, IBrowsingContext? browsingContext)
    {
        BaseUri = options.Value.GetSourceBaseUri();
        Config = config ?? Configuration.Default.WithRequesters().WithDefaultLoader();
        BrowsingContext = browsingContext ?? global::AngleSharp.BrowsingContext.New(Config);
    }

    protected static Url AsAngleSharpUrl(Uri uri)
        => new(uri.ToString());

    public virtual bool CanGetFrom(Uri uri)
        => BrowsingContext.GetNavigationHandler(AsAngleSharpUrl(uri)) is not null;

    public abstract bool DataExists(Uri uri);

    public virtual INode GetData(Uri uri)
    {
        IDocument document = GetDocument(uri); ;
        return document?.Body ?? throw new InvalidOperationException("Document has no body");
    }

    protected abstract IDocument GetDocument(Uri uri);
}
