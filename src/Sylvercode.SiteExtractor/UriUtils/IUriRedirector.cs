using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.UriUtils;

public interface IUriRedirector
{
    bool RedirectUri(string uri, [NotNullWhen(true)] out string? redirectedUri);
}
