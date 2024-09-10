namespace Sylvercode.SiteSource.Store;

public interface IDataStore
{
    Uri BaseUri { get; }
    Stream GetStream(Uri uri);
    StreamWriter GetStreamWriter(Uri uri);
}
