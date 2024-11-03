namespace Sylvercode.SiteExtractor.StdHtml;

[Flags]
public enum HtmlResourceDependencyFilterMode
{
    None = 0,
    InSourceBase = 1,
    IsImage = 2,
    IsNotImage = 4,
}
