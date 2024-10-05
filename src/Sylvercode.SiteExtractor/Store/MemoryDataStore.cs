using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

public class MemoryDataStore(IOptions<SiteExtractorOptions> options) : BaseDataStore(options), IReadOnlyDictionary<Uri, MemoryStream>
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

    #region IReadOnlyDictionary<Uri, MemoryStream> implementation
    public MemoryStream this[Uri key] => ((IReadOnlyDictionary<Uri, MemoryStream>)_data)[key];

    public IEnumerable<Uri> Keys => ((IReadOnlyDictionary<Uri, MemoryStream>)_data).Keys;

    public IEnumerable<MemoryStream> Values => ((IReadOnlyDictionary<Uri, MemoryStream>)_data).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<Uri, MemoryStream>>)_data).Count;

    public bool ContainsKey(Uri key)
    {
        return ((IReadOnlyDictionary<Uri, MemoryStream>)_data).ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<Uri, MemoryStream>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Uri, MemoryStream>>)_data).GetEnumerator();
    }

    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out MemoryStream value)
    {
        return ((IReadOnlyDictionary<Uri, MemoryStream>)_data).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }
    #endregion
}
