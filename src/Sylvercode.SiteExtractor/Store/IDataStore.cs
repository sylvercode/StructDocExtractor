namespace Sylvercode.SiteExtractor.Store;

public interface IDataStore
{
    Uri BaseUri { get; }
    Stream GetStream(Uri uri);
    StreamWriter GetStreamWriter(Uri uri);
}
