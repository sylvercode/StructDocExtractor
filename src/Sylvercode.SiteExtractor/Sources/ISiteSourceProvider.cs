namespace Sylvercode.SiteExtractor.Sources;

public interface ISiteSourceProvider
{
    ISiteSource GetSource(Uri uri);
}
