namespace Sylvercode.SiteSource.Store;

public class MemoryDataStore(Uri baseUri) : BaseDataStop(baseUri)
{
    private readonly Dictionary<Uri, MemoryStream> _data = [];

    public override Stream GetStream(Uri uri)
    {
        Uri completeUri = GetCompleteUri(uri);
        if (_data.TryGetValue(completeUri, out MemoryStream? ms)
            && ms.CanRead)
            return ms;

        return _data[completeUri] = new MemoryStream();
    }
}
