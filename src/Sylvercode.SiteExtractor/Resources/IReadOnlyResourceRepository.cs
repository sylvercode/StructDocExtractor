using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Read-only contract for querying the set of tracked site resources.</summary>
public interface IReadOnlyResourceRepository : IEnumerable<Resource>
{
    /// <summary>Gets the <see cref="Resource"/> associated with the specified URI.</summary>
    /// <param name="key">The source URI of the resource to retrieve (fragment is ignored).</param>
    /// <returns>The <see cref="Resource"/> registered for <paramref name="key"/>.</returns>
    Resource this[Uri key] { get; }

    /// <summary>Gets the collection of all tracked resource URIs.</summary>
    IEnumerable<Uri> UriResources { get; }

    /// <summary>Gets the total number of tracked resources.</summary>
    int Count { get; }

    /// <summary>Determines whether a resource is registered for the specified URI.</summary>
    /// <param name="key">The URI to test (fragment is ignored).</param>
    /// <returns><see langword="true"/> if a resource exists for <paramref name="key"/>; otherwise <see langword="false"/>.</returns>
    bool ContainsResourceForUri(Uri key);

    /// <summary>Attempts to retrieve the resource associated with the specified URI.</summary>
    /// <param name="key">The URI to look up.</param>
    /// <param name="value">When this method returns, contains the matching <see cref="Resource"/>, or <see langword="null"/> if not found.</param>
    /// <returns><see langword="true"/> if a resource was found; otherwise <see langword="false"/>.</returns>
    bool TryGetResourceForUri(Uri key, [MaybeNullWhen(false)] out Resource value);

    /// <summary>Returns the underlying resource map as a read-only dictionary keyed by URI.</summary>
    /// <returns>A read-only view of all tracked resources.</returns>
    IReadOnlyDictionary<Uri, Resource> AsDictionary();
}
