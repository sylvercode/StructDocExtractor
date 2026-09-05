using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Default implementation of <see cref="IUriRedirector"/> that applies a regex-based replace rule to a URI string</summary>
public class UriRedirector(Regex regex, string replace) : IUriRedirector
{
    /// <summary>Attempts to redirect <paramref name="uri"/> by applying the configured regex replacement</summary>
    /// <param name="uri">The original URI string to evaluate</param>
    /// <param name="redirectedUri">When this method returns <see langword="true"/>, contains the replaced URI string; otherwise <see langword="null"/></param>
    /// <returns><see langword="true"/> if the regex matched and the URI was changed; otherwise <see langword="false"/></returns>
    public bool RedirectUri(string uri, [NotNullWhen(true)] out string? redirectedUri)
    {
        bool hasChanged = false;
        string result = regex.Replace(uri, (match) =>
        {
            hasChanged = true;
            return replace;
        });

        redirectedUri = hasChanged
            ? result
            : null;

        return hasChanged;
    }
}
