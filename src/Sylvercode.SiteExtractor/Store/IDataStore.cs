namespace Sylvercode.SiteExtractor.Store;

/// <summary>Defines the contract for reading and writing extracted document data to a persistent store.</summary>
public interface IDataStore
{
    /// <summary>Gets the base URI that identifies the root of this data store.</summary>
    Uri BaseUri { get; }

    /// <summary>Returns a writable <see cref="Stream"/> for the specified URI within this store.</summary>
    /// <param name="uri">The URI of the resource to write to.</param>
    /// <returns>A writable <see cref="Stream"/> targeting the resource location.</returns>
    Stream GetStream(Uri uri);

    /// <summary>Returns a <see cref="StreamWriter"/> for writing text content to the specified URI.</summary>
    /// <param name="uri">The URI of the resource to write text to.</param>
    /// <returns>A <see cref="StreamWriter"/> wrapping the stream for the resource location.</returns>
    StreamWriter GetStreamWriter(Uri uri);
}
