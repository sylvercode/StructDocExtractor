using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Extraction;

public class RouterExtractorList<TExtractionData> : IList<RouterExtractorList<TExtractionData>.Entry>
{
    public class Entry(IRouterExtractorSelector<TExtractionData> selector,
                       IExtractor<TExtractionData> extractor)
    {
        public IRouterExtractorSelector<TExtractionData> Selector { get; } = selector;
        public IExtractor<TExtractionData> Extractor { get; } = extractor;
    }

    public class SelectionResult(IExtractor<TExtractionData> extractor, [DisallowNull] TExtractionData rootData)
    {
        [NotNull] private readonly TExtractionData rootData = rootData;

        public IExtractor<TExtractionData> Extractor => extractor;
        [NotNull]
        public TExtractionData RootData => rootData;
    }

    private readonly List<Entry> _extractors = [];

    public void Add(IRouterExtractorSelector<TExtractionData> selector, IExtractor<TExtractionData> extractor)
        => _extractors.Add(new Entry(selector, extractor));

    public SelectionResult? Select(TExtractionData data)
    {
        foreach (var entry in _extractors)
        {
            var rootData = entry.Selector.Match(data);
            if (rootData is not null)
                return new SelectionResult(entry.Extractor, rootData);
        }

        return null;
    }

    #region IList<RouterExtractorList<TExtractionData>.Entry> implementation
    public int Count => ((ICollection<Entry>)_extractors).Count;

    public bool IsReadOnly => ((ICollection<Entry>)_extractors).IsReadOnly;

    public Entry this[int index] { get => ((IList<Entry>)_extractors)[index]; set => ((IList<Entry>)_extractors)[index] = value; }

    public int IndexOf(Entry item)
        => ((IList<Entry>)_extractors).IndexOf(item);

    public void Insert(int index, Entry item)
        => ((IList<Entry>)_extractors).Insert(index, item);

    public void RemoveAt(int index)
        => ((IList<Entry>)_extractors).RemoveAt(index);

    public void Add(Entry item)
        => ((ICollection<Entry>)_extractors).Add(item);

    public void Clear()
        => ((ICollection<Entry>)_extractors).Clear();

    public bool Contains(Entry item)
        => ((ICollection<Entry>)_extractors).Contains(item);

    public void CopyTo(Entry[] array, int arrayIndex)
        => ((ICollection<Entry>)_extractors).CopyTo(array, arrayIndex);

    public bool Remove(Entry item)
        => ((ICollection<Entry>)_extractors).Remove(item);

    public IEnumerator<Entry> GetEnumerator()
        => ((IEnumerable<Entry>)_extractors).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_extractors).GetEnumerator();
    #endregion
}
