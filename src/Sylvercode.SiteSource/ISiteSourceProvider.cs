namespace Sylvercode.SiteSource;

public interface ISiteSourceProvider
{
    ISiteSource GetSource(Uri uri);
}
