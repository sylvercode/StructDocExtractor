namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Formats Markdown link and wiki-link syntax from a URI and optional display text.</summary>
/// <remarks>Supports both standard <c>[text](uri)</c> links and Obsidian-style <c>[[wikiref]]</c> wiki-links; the <c>wiki://</c> scheme is used to identify wiki-link URIs.</remarks>
public class MarkdownLinkFormater(bool useWikilink = true)
{
    /// <summary>The URI scheme used to identify wiki-link targets.</summary>
    public const string WikiScheme = "wiki";

    /// <summary>Formats the given <paramref name="href"/> and optional <paramref name="text"/> as a Markdown link, choosing wiki-link syntax when applicable.</summary>
    /// <param name="href">The target URI or path to link to.</param>
    /// <param name="text">The display text, or <see langword="null"/> to use <paramref name="href"/> as the text.</param>
    /// <returns>A formatted Markdown link string.</returns>
    public string Format(string href, string? text = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            text = href;

        if (!useWikilink)
            return FormatWikiLink(href, text);

        Uri hrefUri = new(href, UriKind.RelativeOrAbsolute);
        if (!hrefUri.IsAbsoluteUri)
            return FormatStandardLink(href, text);

        if (hrefUri.Scheme != WikiScheme)
            return FormatStandardLink(href, text);

        string wikiRef = WikiPath(hrefUri) + hrefUri.Fragment;
        return FormatWikiLink(wikiRef, text);
    }

    /// <summary>Formats <paramref name="href"/> as a standard Markdown link using the URI itself as display text.</summary>
    /// <param name="href">The target URI.</param>
    /// <returns>A <c>[href](href)</c> Markdown link string.</returns>
    public static string FormatStandardLink(string href)
        => FormatStandardLink(href, href);

    /// <summary>Formats <paramref name="href"/> and <paramref name="text"/> as a standard Markdown link.</summary>
    /// <param name="href">The target URI.</param>
    /// <param name="text">The display text.</param>
    /// <returns>A <c>[text](href)</c> Markdown link string.</returns>
    public static string FormatStandardLink(string href, string text)
        => $"[{text}]({href})";

    /// <summary>Formats <paramref name="href"/> and <paramref name="text"/> as an Obsidian-style wiki-link.</summary>
    /// <param name="href">The wiki reference path.</param>
    /// <param name="text">The display text; if equal to <paramref name="href"/>, a simple <c>[[ref]]</c> form is used.</param>
    /// <returns>A <c>[[ref]]</c> or <c>[[ref|text]]</c> wiki-link string.</returns>
    public static string FormatWikiLink(string href, string text)
    {
        string unescapeHref = Uri.UnescapeDataString(href);
        if (unescapeHref == text)
            return $"[[{unescapeHref}]]";
        else
            return $"[[{unescapeHref}|{text}]]";
    }

    private static string WikiPath(Uri uri)
    {
        string localPath = uri.LocalPath;
        if (localPath == "/")
            return string.Empty;
        return localPath;
    }
}
