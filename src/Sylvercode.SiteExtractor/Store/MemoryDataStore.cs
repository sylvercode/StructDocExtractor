using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

/// <summary>In-memory <see cref="IDataStore"/> that stores each resource as a <see cref="MemoryStream"/>, implementing <see cref="IReadOnlyDictionary{TKey,TValue}"/> for inspection.</summary>
/// <remarks>Suitable for testing or temporary buffering; data does not persist beyond the lifetime of this instance.</remarks>
#pragma warning disable CA1710 // Identifiers should have correct suffix
public class MemoryDataStore(IOptions<SiteExtractorOptions> options) : BaseDataStore(options), IReadOnlyDictionary<Uri, MemoryStream>
#pragma warning restore CA1710 // Identifiers should have correct suffix
{
    private readonly Dictionary<Uri, MemoryStream> _data = [];

    /// <summary>Returns a writable <see cref="MemoryStream"/> for the specified URI, creating one if it does not already exist.</summary>
    /// <param name="uri">The URI of the resource to write to.</param>
    /// <returns>A <see cref="MemoryStream"/> for the resolved URI.</returns>
    public override Stream GetStream(Uri uri)
    {
        Uri completeUri = GetCompleteUri(uri);
        if (_data.TryGetValue(completeUri, out MemoryStream? ms)
            && ms.CanRead)
            return ms;

        return _data[completeUri] = new MemoryStream();
    }

    #region IReadOnlyDictionary<Uri, MemoryStream> implementation
    /// <inheritdoc/>
    public MemoryStream this[Uri key] => ((IReadOnlyDictionary<Uri, MemoryStream>)_data)[key];

    /// <inheritdoc/>
    public IEnumerable<Uri> Keys => ((IReadOnlyDictionary<Uri, MemoryStream>)_data).Keys;

    /// <inheritdoc/>
    public IEnumerable<MemoryStream> Values => ((IReadOnlyDictionary<Uri, MemoryStream>)_data).Values;

    /// <inheritdoc/>
    public int Count => ((IReadOnlyCollection<KeyValuePair<Uri, MemoryStream>>)_data).Count;

    /// <inheritdoc/>
    public bool ContainsKey(Uri key)
    {
        return ((IReadOnlyDictionary<Uri, MemoryStream>)_data).ContainsKey(key);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Uri, MemoryStream>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Uri, MemoryStream>>)_data).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out MemoryStream value)
    {
        return ((IReadOnlyDictionary<Uri, MemoryStream>)_data).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }
    #endregion
}
