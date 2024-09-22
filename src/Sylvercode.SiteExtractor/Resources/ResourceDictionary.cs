using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourceDictionary() : IReadOnlyDictionary<Uri, Resource>
{
    private readonly Dictionary<Uri, Resource> _resources = [];

    public (Resource resource, bool isNew) Add(Uri uri, bool isPullable)
    {
        (Uri uriWithoutFragment, _) = uri.GetUriAndFragment();
        if (_resources.TryGetValue(uriWithoutFragment, out Resource? resource))
        {
            if (resource.State.IsPullable != isPullable)
                throw new InvalidOperationException("The resource already exists with a different pull type.");
            return (resource, false);
        }

        var result = (resource: new Resource(uriWithoutFragment, isPullable), isNew: true);
        _resources[uriWithoutFragment] = result.resource;

        return result;
    }

    #region IReadOnlyDictionary<Uri, Resource> implementation
    public Resource this[Uri key]
    {
        get
        {
            (Uri uriWithoutFragment, _) = key.GetUriAndFragment();
            var result = _resources[uriWithoutFragment];
            return result;
        }
    }

    public IEnumerable<Uri> Keys => ((IReadOnlyDictionary<Uri, Resource>)_resources).Keys;

    public IEnumerable<Resource> Values => ((IReadOnlyDictionary<Uri, Resource>)_resources).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<Uri, Resource>>)_resources).Count;

    public bool ContainsKey(Uri key)
    {
        (Uri uriWithoutFragment, _) = key.GetUriAndFragment();
        return _resources.ContainsKey(uriWithoutFragment);
    }

    public IEnumerator<KeyValuePair<Uri, Resource>> GetEnumerator()
        => ((IEnumerable<KeyValuePair<Uri, Resource>>)_resources).GetEnumerator();

    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out Resource value)
        => ((IReadOnlyDictionary<Uri, Resource>)_resources).TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_resources).GetEnumerator();
    #endregion
}
