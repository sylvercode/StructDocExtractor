using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Metadatas;

public class MetadataDictionary : IDictionary<string, Metadata>
{
    private readonly Dictionary<string, Metadata> _dictionary = [];

    public void AddMetadata(string name, object? value)
        => _dictionary[name] = new Metadata(name, value);

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

    public string? GetStrValue(string name)
    {
        if (!TryGetStrValue(name, out var value))
            throw new KeyNotFoundException($"Key '{name}' not found in dictionary.");

        return value;
    }

    public string GetStrValueOrDefault(string name, string defaultValue = "")
    {
        if (!TryGetStrValue(name, out var value))
            return defaultValue;

        return value ?? defaultValue;
    }

    #region IDictionary<string, ResourceMetadata> implementation
    public Metadata this[string key] { get => ((IDictionary<string, Metadata>)_dictionary)[key]; set => ((IDictionary<string, Metadata>)_dictionary)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, Metadata>)_dictionary).Keys;

    public ICollection<Metadata> Values => ((IDictionary<string, Metadata>)_dictionary).Values;

    public int Count => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).IsReadOnly;

    public void Add(string key, Metadata value)
        => ((IDictionary<string, Metadata>)_dictionary).Add(key, value);

    public void Add(KeyValuePair<string, Metadata> item)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Add(item);

    public void Clear()
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Clear();

    public bool Contains(KeyValuePair<string, Metadata> item)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Contains(item);

    public bool ContainsKey(string key)
        => ((IDictionary<string, Metadata>)_dictionary).ContainsKey(key);

    public void CopyTo(KeyValuePair<string, Metadata>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, Metadata>> GetEnumerator()
        => ((IEnumerable<KeyValuePair<string, Metadata>>)_dictionary).GetEnumerator();

    public bool Remove(string key)
        => ((IDictionary<string, Metadata>)_dictionary).Remove(key);

    public bool Remove(KeyValuePair<string, Metadata> item)
        => ((ICollection<KeyValuePair<string, Metadata>>)_dictionary).Remove(item);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Metadata value)
        => ((IDictionary<string, Metadata>)_dictionary).TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_dictionary).GetEnumerator();
    #endregion
}
