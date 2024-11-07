using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Sylvercode.SiteExtractor.UriUtils;

public class UriRedirector(Regex regex, string replace) : IUriRedirector
{
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
