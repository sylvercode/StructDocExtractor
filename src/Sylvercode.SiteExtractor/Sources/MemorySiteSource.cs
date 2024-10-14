using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Sources;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class MemorySiteSource<TData>(IOptions<MemorySiteSource<TData>.Options> options) : ISiteSource<TData>, IDictionary<Uri, TData>
#pragma warning restore CA1710 // Identifiers should have correct suffix
{
    public class Options
    {
        public TData? DefaultData { get; set; }
        public Uri? BaseUri { get; set; }
    }
    private readonly Dictionary<Uri, TData> _data = [];

    public Uri BaseUri { get; } = options.Value.BaseUri ?? new Uri("memory://");

    public bool CanGetFrom(Uri uri) => options.Value.DefaultData is not null || _data.ContainsKey(uri);
    public bool DataExists(Uri uri) => CanGetFrom(uri);
    public TData GetData(Uri uri)
    {
        if (!_data.TryGetValue(uri, out TData? data))
        {
            if (options.Value.DefaultData is not null)
                return options.Value.DefaultData;
            throw new InvalidOperationException($"No data found for {uri}");
        }

        return data;
    }

    #region IDictionary
    public TData this[Uri key] { get => ((IDictionary<Uri, TData>)_data)[key]; set => ((IDictionary<Uri, TData>)_data)[key] = value; }

    public ICollection<Uri> Keys => ((IDictionary<Uri, TData>)_data).Keys;

    public ICollection<TData> Values => ((IDictionary<Uri, TData>)_data).Values;

    public int Count => ((ICollection<KeyValuePair<Uri, TData>>)_data).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<Uri, TData>>)_data).IsReadOnly;

    public void Add(Uri key, TData value)
    {
        ((IDictionary<Uri, TData>)_data).Add(key, value);
    }

    public void Add(KeyValuePair<Uri, TData> item)
    {
        ((ICollection<KeyValuePair<Uri, TData>>)_data).Add(item);
    }


    public void Clear()
    {
        ((ICollection<KeyValuePair<Uri, TData>>)_data).Clear();
    }

    public bool Contains(KeyValuePair<Uri, TData> item)
    {
        return ((ICollection<KeyValuePair<Uri, TData>>)_data).Contains(item);
    }

    public bool ContainsKey(Uri key)
    {
        return ((IDictionary<Uri, TData>)_data).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<Uri, TData>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<Uri, TData>>)_data).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<Uri, TData>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Uri, TData>>)_data).GetEnumerator();
    }

    public bool Remove(Uri key)
    {
        return ((IDictionary<Uri, TData>)_data).Remove(key);
    }

    public bool Remove(KeyValuePair<Uri, TData> item)
    {
        return ((ICollection<KeyValuePair<Uri, TData>>)_data).Remove(item);
    }

    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out TData value)
    {
        return ((IDictionary<Uri, TData>)_data).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }
    #endregion
}
