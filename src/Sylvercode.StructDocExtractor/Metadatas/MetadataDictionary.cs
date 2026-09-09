using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Metadatas;

/// <summary>Dictionary of <see cref="Metadata"/> entries keyed by name, attached to structural nodes.</summary>
/// <remarks>
/// Wraps an inner <see cref="Dictionary{TKey,TValue}"/> and adds typed retrieval helpers that convert values on access.
/// Use <see cref="AddMetadata"/> and <see cref="CopyMetadataFrom"/> rather than the raw <see cref="IDictionary{TKey,TValue}"/> API when building metadata collections.
/// </remarks>
public class MetadataDictionary : IDictionary<string, Metadata>
{
    private readonly Dictionary<string, Metadata> _dictionary;

    /// <summary>Initializes a new empty instance of <see cref="MetadataDictionary"/>.</summary>
    public MetadataDictionary()
    {
        _dictionary = [];
    }

    /// <summary>Initializes a new instance of <see cref="MetadataDictionary"/> from a collection of key/value pairs.</summary>
    /// <param name="collection">The key/value pairs to populate the dictionary with; values may be <see langword="null"/>.</param>
    public MetadataDictionary(IEnumerable<KeyValuePair<string, object?>> collection)
    {
        _dictionary = new(collection.Select(
            kv => new KeyValuePair<string, Metadata>(kv.Key, new Metadata(kv.Key, kv.Value))));
    }

    /// <summary>Adds or replaces a metadata entry with the specified name and value.</summary>
    /// <param name="name">The metadata key.</param>
    /// <param name="value">The metadata value; may be <see langword="null"/>.</param>
    public void AddMetadata(string name, object? value)
        => _dictionary[name] = new Metadata(name, value);

    /// <summary>Copies metadata entries from another <see cref="MetadataDictionary"/> into this one.</summary>
    /// <param name="other">The source dictionary to copy entries from.</param>
    /// <param name="newOnly">When <see langword="true"/>, only entries whose keys are absent in this dictionary are copied; when <see langword="false"/>, all entries are overwritten.</param>
    public void CopyMetadataFrom(MetadataDictionary other, bool newOnly = true)
    {
        foreach (var (name, metadata) in other)
        {
            if (newOnly)
                _dictionary.TryAdd(name, metadata);
            else
                _dictionary[name] = metadata;
        }
    }

    /// <summary>Attempts to retrieve and convert a metadata value to <typeparamref name="TValue"/>.</summary>
    /// <typeparam name="TValue">The target type for conversion.</typeparam>
    /// <param name="name">The metadata key to look up.</param>
    /// <param name="value">When this method returns <see langword="true"/>, the converted value; otherwise the default for <typeparamref name="TValue"/>.</param>
    /// <returns><see langword="true"/> if the key was found and converted successfully; otherwise <see langword="false"/>.</returns>
    public bool TryGetValue<TValue>(string name, out TValue? value)
    {
        if (!_dictionary.TryGetValue(name, out var entry))
        {
            value = default;
            return false;
        }

        value = entry.GetValue<TValue>();
        return true;
    }

    /// <summary>Attempts to retrieve and convert a metadata value to <see cref="string"/>.</summary>
    /// <param name="name">The metadata key to look up.</param>
    /// <param name="value">When this method returns <see langword="true"/>, the string value; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the key was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetStrValue(string name, out string? value)
    {
        if (!_dictionary.TryGetValue(name, out var entry))
        {
            value = null;
            return false;
        }

        value = entry.GetStrValue();
        return true;
    }

    /// <summary>Returns the string value for the given metadata key, or throws if the key is absent.</summary>
    /// <param name="name">The metadata key to retrieve.</param>
    /// <returns>The string representation of the stored value, which may be <see langword="null"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when <paramref name="name"/> does not exist in the dictionary.</exception>
    public string? GetStrValue(string name)
    {
        if (!TryGetStrValue(name, out var value))
            throw new KeyNotFoundException($"Key '{name}' not found in dictionary.");

        return value;
    }

    /// <summary>Returns the string value for the given metadata key, or a fallback value when the key is absent or the stored value is <see langword="null"/>.</summary>
    /// <param name="name">The metadata key to retrieve.</param>
    /// <param name="defaultValue">The value to return when the key is absent or the stored value is <see langword="null"/>.</param>
    /// <returns>The string value, or <paramref name="defaultValue"/> if not found or <see langword="null"/>.</returns>
    public string GetStrValueOrDefault(string name, string defaultValue = "")
    {
        if (!TryGetStrValue(name, out var value))
            return defaultValue;

        return value ?? defaultValue;
    }

    #region IDictionary<string, ResourceMetadata> implementation
    /// <inheritdoc/>
    public Metadata this[string key] { get => ((IDictionary<string, Metadata>)_dictionary)[key]; set => ((IDictionary<string, Metadata>)_dictionary)[key] = value; }

    /// <inheritdoc/>
    public ICollection<string> Keys => ((IDictionary<string, Metadata>)_dictionary).Keys;

    /// <inheritdoc/>
    public ICollection<Metadata> Values => ((IDictionary<string, Metadata>)_dictionary).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).IsReadOnly;

    /// <inheritdoc/>
    public void Add(string key, Metadata value)
        => ((IDictionary<string, Metadata>)_dictionary).Add(key, value);

    /// <inheritdoc/>
    public void Add(KeyValuePair<string, Metadata> item)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Add(item);

    /// <inheritdoc/>
    public void Clear()
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Clear();

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<string, Metadata> item)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Contains(item);

    /// <inheritdoc/>
    public bool ContainsKey(string key)
        => ((IDictionary<string, Metadata>)_dictionary).ContainsKey(key);

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<string, Metadata>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, Metadata>> GetEnumerator()
        => ((IEnumerable<KeyValuePair<string, Metadata>>)_dictionary).GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(string key)
        => ((IDictionary<string, Metadata>)_dictionary).Remove(key);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<string, Metadata> item)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Remove(item);

    /// <inheritdoc/>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Metadata value)
        => ((IDictionary<string, Metadata>)_dictionary).TryGetValue(key, out value);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_dictionary).GetEnumerator();
    #endregion
}
