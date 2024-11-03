namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

public class MarkdownLinkFormater(bool useWikilink = true)
{
    public const string WikiScheme = "wiki";

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

    public static string FormatStandardLink(string href)
        => FormatStandardLink(href, href);

    public static string FormatStandardLink(string href, string text)
        => $"[{text}]({href})";

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
