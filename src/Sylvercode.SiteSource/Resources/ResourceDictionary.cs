using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteSource.UriTransformer;

namespace Sylvercode.SiteSource.Resources;

public class ResourceDictionary() : IReadOnlyDictionary<Uri, Resource>
{
    private readonly Dictionary<Uri, Resource> _resources = [];

    public void Add(Uri uri, ResourcePullType pullType)
    {
        (Uri uriWithoutFragment, string fragment) = GetUriAndFragment(uri);
        if (!_resources.TryGetValue(uriWithoutFragment, out Resource? resource))
            _resources[uriWithoutFragment] = new Resource(pullType, uriWithoutFragment, fragment);
        else
        {
            if (resource.State.PullType != pullType)
                throw new InvalidOperationException("The resource already exists with a different pull type.");
            resource.AddFragment(fragment);
        }
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
            (Uri uriWithoutFragment, string fragment) = GetUriAndFragment(key);
            var result = _resources[uriWithoutFragment];
            if (!result.Fragments.Contains(fragment))
                throw new KeyNotFoundException("The fragment is not part of the resource.");
            return result;
        }
    }

    public IEnumerable<Uri> Keys => ((IReadOnlyDictionary<Uri, Resource>)_resources).Keys;

    public IEnumerable<Resource> Values => ((IReadOnlyDictionary<Uri, Resource>)_resources).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<Uri, Resource>>)_resources).Count;

    public bool ContainsKey(Uri key)
    {
        (Uri uriWithoutFragment, string fragment) = GetUriAndFragment(key);
        if (_resources.TryGetValue(uriWithoutFragment, out Resource? resource))
            return resource.Fragments.Contains(fragment);
        return false;
    }

    public IEnumerator<KeyValuePair<Uri, Resource>> GetEnumerator()
        => ((IEnumerable<KeyValuePair<Uri, Resource>>)_resources).GetEnumerator();

    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out Resource value)
        => ((IReadOnlyDictionary<Uri, Resource>)_resources).TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_resources).GetEnumerator();
    #endregion
}
