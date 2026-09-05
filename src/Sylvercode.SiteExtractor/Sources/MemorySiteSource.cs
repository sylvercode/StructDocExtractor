using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Sources;

/// <summary>In-memory <see cref="ISiteSource{TData}"/> backed by a dictionary of pre-loaded documents, with an optional default value returned for unregistered URIs.</summary>
/// <typeparam name="TData">The type of document data stored in this source.</typeparam>
/// <remarks>
/// Implements <see cref="IDictionary{TKey,TValue}"/> so callers can populate entries directly.
/// When <see cref="Options.DefaultData"/> is set, any URI not found in the dictionary returns that fallback value.
/// </remarks>
#pragma warning disable CA1710 // Identifiers should have correct suffix
public class MemorySiteSource<TData>(IOptions<MemorySiteSource<TData>.Options> options) : ISiteSource<TData>, IDictionary<Uri, TData>
#pragma warning restore CA1710 // Identifiers should have correct suffix
{
    /// <summary>Configuration options for <see cref="MemorySiteSource{TData}"/>.</summary>
    public class Options
    {
        /// <summary>Gets or sets a fallback value returned when the requested URI is not found in the source.</summary>
        public TData? DefaultData { get; set; }

        /// <summary>Gets or sets the base URI reported by this source; defaults to <c>memory://</c> if not set.</summary>
        public Uri? BaseUri { get; set; }
    }
    private readonly Dictionary<Uri, TData> _data = [];

    /// <summary>Gets the base URI that this source serves content from.</summary>
    public Uri BaseUri { get; } = options.Value.BaseUri ?? new Uri("memory://");

    /// <summary>Determines whether this source can provide content for the specified URI.</summary>
    /// <param name="uri">The URI to check availability for.</param>
    /// <returns><see langword="true"/> if a default value is configured or the URI is registered; otherwise, <see langword="false"/>.</returns>
    public bool CanGetFrom(Uri uri) => options.Value.DefaultData is not null || _data.ContainsKey(uri);

    /// <summary>Determines whether data exists for the specified URI.</summary>
    /// <param name="uri">The URI to check.</param>
    /// <returns><see langword="true"/> if data is available for the URI; otherwise, <see langword="false"/>.</returns>
    public bool DataExists(Uri uri) => CanGetFrom(uri);

    /// <summary>Returns the typed data for the specified URI, falling back to <see cref="Options.DefaultData"/> if the URI is not registered.</summary>
    /// <param name="uri">The URI of the resource to retrieve.</param>
    /// <returns>The data associated with the URI, or <see cref="Options.DefaultData"/> if the URI is not found.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the URI is not found and no default data is configured.</exception>
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
    /// <inheritdoc/>
    public TData this[Uri key] { get => ((IDictionary<Uri, TData>)_data)[key]; set => ((IDictionary<Uri, TData>)_data)[key] = value; }

    /// <inheritdoc/>
    public ICollection<Uri> Keys => ((IDictionary<Uri, TData>)_data).Keys;

    /// <inheritdoc/>
    public ICollection<TData> Values => ((IDictionary<Uri, TData>)_data).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<Uri, TData>>)_data).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<Uri, TData>>)_data).IsReadOnly;

    /// <inheritdoc/>
    public void Add(Uri key, TData value)
    {
        ((IDictionary<Uri, TData>)_data).Add(key, value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<Uri, TData> item)
    {
        ((ICollection<KeyValuePair<Uri, TData>>)_data).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<Uri, TData>>)_data).Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<Uri, TData> item)
    {
        return ((ICollection<KeyValuePair<Uri, TData>>)_data).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(Uri key)
    {
        return ((IDictionary<Uri, TData>)_data).ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<Uri, TData>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<Uri, TData>>)_data).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Uri, TData>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Uri, TData>>)_data).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(Uri key)
    {
        return ((IDictionary<Uri, TData>)_data).Remove(key);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<Uri, TData> item)
    {
        return ((ICollection<KeyValuePair<Uri, TData>>)_data).Remove(item);
    }

    /// <inheritdoc/>
    public bool TryGetValue(Uri key, [MaybeNullWhen(false)] out TData value)
    {
        return ((IDictionary<Uri, TData>)_data).TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }
    #endregion
}
