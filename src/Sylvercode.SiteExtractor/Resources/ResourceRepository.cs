using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Writable repository that stores and indexes all discovered <see cref="Resource"/> objects, implementing <see cref="IReadOnlyResourceRepository"/> for read-only consumers.</summary>
/// <remarks>
/// Resources are keyed by their fragment-stripped URI; fragment URIs and their base URI resolve to the same entry.
/// Adding an existing URI returns the stored resource without creating a duplicate; attempting to re-add with a
/// different <c>isPullable</c> flag throws <see cref="InvalidOperationException"/>.
/// </remarks>
public class ResourceRepository() : IReadOnlyResourceRepository
{
    private readonly Dictionary<Uri, Resource> _resources = [];

    /// <summary>Adds a resource for the specified URI, or returns the existing one if already tracked.</summary>
    /// <param name="uri">The resource URI to add (fragment portion is stripped before storage).</param>
    /// <param name="isPullable">Whether the resource requires downloading.</param>
    /// <returns>The existing or newly created <see cref="Resource"/>, and a flag indicating whether it was newly added.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the URI is already tracked with a different pullability setting.</exception>
    public (Resource resource, bool isNew) Add(Uri uri, bool isPullable = true)
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

    /// <summary>Initializes a new <see cref="ResourceRepository"/> pre-populated with non-pullable resources for each URI string.</summary>
    /// <param name="uris">The URI strings to pre-populate as non-pullable resources.</param>
    public ResourceRepository(IEnumerable<string> uris) : this()
    {
        foreach (var uriString in uris)
            Add(new Uri(uriString));
    }

    /// <inheritdoc/>
    public Resource this[Uri key]
    {
        get
        {
            (Uri uriWithoutFragment, _) = key.GetUriAndFragment();
            var result = _resources[uriWithoutFragment];
            return result;
        }
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<Uri, Resource> AsDictionary() => _resources.AsReadOnly();

    /// <inheritdoc/>
    public IEnumerable<Uri> UriResources => _resources.Keys;

    /// <inheritdoc/>
    public int Count => _resources.Count;

    /// <inheritdoc/>
    public bool ContainsResourceForUri(Uri key)
    {
        (Uri uriWithoutFragment, _) = key.GetUriAndFragment();
        return _resources.ContainsKey(uriWithoutFragment);
    }

    /// <inheritdoc/>
    public bool TryGetResourceForUri(Uri key, [MaybeNullWhen(false)] out Resource value)
        => _resources.TryGetValue(key, out value);

    #region IEnumerable<Resource> implementation
    /// <inheritdoc/>
    public IEnumerator<Resource> GetEnumerator()
        => _resources.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => _resources.Values.GetEnumerator();
    #endregion
}
