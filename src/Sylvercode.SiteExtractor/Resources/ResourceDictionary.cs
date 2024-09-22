using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourceDictionary() : IReadOnlyDictionary<Uri, Resource>
{
    private readonly Dictionary<Uri, Resource> _resources = [];

    public (Resource resource, bool isNew) Add(Uri uri, ResourcePullType pullType)
    {
        (Uri uriWithoutFragment, _) = GetUriAndFragment(uri);
        var result = (resource: default(Resource), isNew: false);
        if (!_resources.TryGetValue(uriWithoutFragment, out result.resource))
        {
            _resources[uriWithoutFragment] = result.resource = new Resource(pullType, uriWithoutFragment);
            result.isNew = true;
        }
        else
        {
            if (result.resource.State.PullType != pullType)
                throw new InvalidOperationException("The resource already exists with a different pull type.");
        }

        return result!;
    }
    public Uri GetDestinationFor(Uri uri)
    {
        (Uri uriWithoutFragment, _) = GetUriAndFragment(uri);
        if (!_resources.TryGetValue(uriWithoutFragment, out Resource? resource))
            throw new InvalidOperationException("The resource does not exist.");
        return resource.GetDestinationFor(uri);
    }

    private static Tuple<Uri, string> GetUriAndFragment(Uri uri)
    {
        string fragment = uri.IsAbsoluteUri ? uri.Fragment : string.Empty;
        Uri uriWithoutFragment = new(uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Fragment, UriFormat.Unescaped));
        return new Tuple<Uri, string>(uriWithoutFragment, fragment);
    }

    #region IReadOnlyDictionary<Uri, Resource> implementation
    public Resource this[Uri key]
    {
        get
        {
            (Uri uriWithoutFragment, _) = GetUriAndFragment(key);
            var result = _resources[uriWithoutFragment];
            return result;
        }
    }

    public IEnumerable<Uri> Keys => ((IReadOnlyDictionary<Uri, Resource>)_resources).Keys;

    public IEnumerable<Resource> Values => ((IReadOnlyDictionary<Uri, Resource>)_resources).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<Uri, Resource>>)_resources).Count;

    public bool ContainsKey(Uri key)
    {
        (Uri uriWithoutFragment, _) = GetUriAndFragment(key);
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
