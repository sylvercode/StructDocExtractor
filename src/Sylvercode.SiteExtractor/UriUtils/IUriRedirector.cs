using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Defines the contract for resolving a URI to the destination reported by an HTTP redirect response.</summary>
public interface IUriRedirector
{
    /// <summary>Checks whether the server redirects <paramref name="uri"/> to a different location.</summary>
    /// <param name="uri">The absolute HTTP or HTTPS URI to inspect.</param>
    /// <param name="redirectedUri">When this method returns <see langword="true"/>, contains the final redirected URI; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the server returned a redirect; otherwise <see langword="false"/>.</returns>
    bool RedirectUri(string uri, [NotNullWhen(true)] out string? redirectedUri);
}
