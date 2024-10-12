using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourceMetadataDictionary : IDictionary<string, ResourceMetadata>
{
    private readonly Dictionary<string, ResourceMetadata> _dictionary = [];

    public void AddMetadata(string name, object? value)
    {
        _dictionary.Add(name, new ResourceMetadata(name, value));
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
            value = "";
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
    public ResourceMetadata this[string key] { get => ((IDictionary<string, ResourceMetadata>)_dictionary)[key]; set => ((IDictionary<string, ResourceMetadata>)_dictionary)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, ResourceMetadata>)_dictionary).Keys;

    public ICollection<ResourceMetadata> Values => ((IDictionary<string, ResourceMetadata>)_dictionary).Values;

    public int Count => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).IsReadOnly;

    public void Add(string key, ResourceMetadata value)
        => ((IDictionary<string, ResourceMetadata>)_dictionary).Add(key, value);

    public void Add(KeyValuePair<string, ResourceMetadata> item)
        => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).Add(item);

    public void Clear()
        => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).Clear();

    public bool Contains(KeyValuePair<string, ResourceMetadata> item)
        => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).Contains(item);

    public bool ContainsKey(string key)
        => ((IDictionary<string, ResourceMetadata>)_dictionary).ContainsKey(key);

    public void CopyTo(KeyValuePair<string, ResourceMetadata>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, ResourceMetadata>> GetEnumerator()
        => ((IEnumerable<KeyValuePair<string, ResourceMetadata>>)_dictionary).GetEnumerator();

    public bool Remove(string key)
        => ((IDictionary<string, ResourceMetadata>)_dictionary).Remove(key);

    public bool Remove(KeyValuePair<string, ResourceMetadata> item)
        => ((ICollection<KeyValuePair<string, ResourceMetadata>>)_dictionary).Remove(item);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out ResourceMetadata value)
        => ((IDictionary<string, ResourceMetadata>)_dictionary).TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_dictionary).GetEnumerator();
    #endregion
}
