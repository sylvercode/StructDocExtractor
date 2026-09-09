using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Defines the contract for redirecting a URI string to a different target location</summary>
public interface IUriRedirector
{
    /// <summary>Attempts to redirect <paramref name="uri"/> to a new location</summary>
    /// <param name="uri">The original URI string to evaluate</param>
    /// <param name="redirectedUri">When this method returns <see langword="true"/>, contains the redirected URI string; otherwise <see langword="null"/></param>
    /// <returns><see langword="true"/> if a redirect was applied; otherwise <see langword="false"/></returns>
    bool RedirectUri(string uri, [NotNullWhen(true)] out string? redirectedUri);
}
