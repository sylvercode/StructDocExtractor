using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

public class UriNode(string uri) : BaseStructDocNode(uri)
{
    public Uri Uri { get; } = new(uri);
}
