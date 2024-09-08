using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteFetcher.Resource;

public class ResourceDictionary : IReadOnlyDictionary<Uri, Resource>
{
    private readonly Dictionary<Uri, Resource> _resources = [];

    public void Add(Uri uri, ResourcePullType pullType)
        => _resources.TryAdd(uri, new Resource(uri, pullType));

    #region IReadOnlyDictionary<Uri, Resource> implementation
    public Resource this[Uri key] => ((IReadOnlyDictionary<Uri, Resource>)_resources)[key];

    public IEnumerable<Uri> Keys => ((IReadOnlyDictionary<Uri, Resource>)_resources).Keys;

    public IEnumerable<Resource> Values => ((IReadOnlyDictionary<Uri, Resource>)_resources).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<Uri, Resource>>)_resources).Count;

    public bool ContainsKey(Uri key)
    {
        return ((IReadOnlyDictionary<Uri, Resource>)_resources).ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<Uri, Resource>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Uri, Resource>>)_resources).GetEnumerator();
    }

    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out Resource value)
    {
        return ((IReadOnlyDictionary<Uri, Resource>)_resources).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_resources).GetEnumerator();
    }
    #endregion
}
