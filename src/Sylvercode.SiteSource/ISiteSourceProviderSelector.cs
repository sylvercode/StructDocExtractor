namespace Sylvercode.SiteSource;

public interface ISiteSourceProviderSelector
{
    bool IsValid(Uri uri);
}
