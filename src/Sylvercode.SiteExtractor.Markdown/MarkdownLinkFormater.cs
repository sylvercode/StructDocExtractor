namespace Sylvercode.SiteExtractor.Markdown;

public class MarkdownLinkFormater(bool useWikilink = true)
{
    public string Format(string text, string href)
    {
        if (!useWikilink)
            return FormatStandard(text, href);

        if (href == text)
            return $"[[{text}]]";

        if (href.EndsWith(MarkdownUriTranslater.MarkdownExtension, StringComparison.OrdinalIgnoreCase))
            return FormatStandard(text, href);

        return $"[{href}|{text}]";
    }

    public static string FormatStandard(string text, string href) => $"[{text}]({href})";
}
