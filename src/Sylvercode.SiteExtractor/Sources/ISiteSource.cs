namespace Sylvercode.SiteExtractor.Sources;

/// <summary>Defines the contract for fetching the raw document content of a resource by URI.</summary>
public interface ISiteSource
{
    /// <summary>Gets the base URI that this source serves content from.</summary>
    Uri BaseUri { get; }

    /// <summary>Determines whether this source can provide content for the specified URI.</summary>
    /// <param name="uri">The URI to check availability for.</param>
    /// <returns><see langword="true"/> if this source can serve the URI; otherwise, <see langword="false"/>.</returns>
    bool CanGetFrom(Uri uri);

    /// <summary>Determines whether data exists in this source for the specified URI.</summary>
    /// <param name="uri">The URI to check for existing data.</param>
    /// <returns><see langword="true"/> if data is available for the URI; otherwise, <see langword="false"/>.</returns>
    bool DataExists(Uri uri);

    /// <summary>Returns the raw document data for the specified URI.</summary>
    /// <param name="uri">The URI of the resource to retrieve.</param>
    /// <returns>The raw data object, or <see langword="null"/> if unavailable.</returns>
    object? GetData(Uri uri);
}

/// <summary>Strongly-typed contract for fetching document data of type <typeparamref name="TData"/> by URI.</summary>
/// <typeparam name="TData">The type of document data this source provides.</typeparam>
public interface ISiteSource<TData> : ISiteSource
{
    /// <summary>Returns the typed document data for the specified URI.</summary>
    /// <param name="uri">The URI of the resource to retrieve.</param>
    /// <returns>The typed data associated with the URI.</returns>
    new TData GetData(Uri uri);
    object? ISiteSource.GetData(Uri uri) => GetData(uri);
}
