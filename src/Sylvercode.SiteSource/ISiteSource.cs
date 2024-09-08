namespace Sylvercode.SiteSource;

public interface ISiteSource
{
    bool CanGetFrom(Uri uri);
    bool DataExists(Uri uri);
    object? GetData(Uri uri);
}

public interface ISiteSource<TData> : ISiteSource
{
    new TData GetData(Uri uri);
    object? ISiteSource.GetData(Uri uri) => GetData(uri);
}
