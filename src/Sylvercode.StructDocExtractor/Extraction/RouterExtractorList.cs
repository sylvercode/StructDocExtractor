using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Metadatas;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Ordered list of <see cref="Entry"/> pairs consulted in priority order to select the appropriate sub-extractor for a given source element.</summary>
/// <typeparam name="TExtractionData">The type of source data element to match and route.</typeparam>
public class RouterExtractorList<TExtractionData> : IList<RouterExtractorList<TExtractionData>.Entry>
{
    /// <summary>Associates an <see cref="IRouterExtractorSelector{TExtractionData}"/> with the <see cref="IExtractor{TExtractionData}"/> to invoke when the selector matches.</summary>
    public class Entry(IRouterExtractorSelector<TExtractionData> selector,
                       IExtractor<TExtractionData> extractor)
    {
        /// <summary>Gets the selector that determines whether this entry handles a given source element.</summary>
        public IRouterExtractorSelector<TExtractionData> Selector { get; } = selector;
        /// <summary>Gets the extractor to invoke when the selector matches.</summary>
        public IExtractor<TExtractionData> Extractor { get; } = extractor;
    }

    /// <summary>Captures the matched extractor and the root data normalised by the selector for passing to <see cref="IExtractor{TExtractionData}.Extract"/>.</summary>
    public class SelectionResult(IExtractor<TExtractionData> extractor, [DisallowNull] TExtractionData rootData)
    {
        [NotNull] private readonly TExtractionData rootData = rootData;

        /// <summary>Gets the extractor selected for this match.</summary>
        public IExtractor<TExtractionData> Extractor => extractor;
        /// <summary>Gets the non-null root data to pass to the selected extractor.</summary>
        [NotNull]
        public TExtractionData RootData => rootData;

        /// <summary>Gets the metadata dictionary that will be merged into the extraction result.</summary>
        public MetadataDictionary Metadatas { get; } = [];
    }

    private readonly List<Entry> _extractors = [];

    /// <summary>Adds a new selector/extractor pair to the end of the routing list.</summary>
    /// <param name="selector">The selector that determines whether this entry handles a given source element.</param>
    /// <param name="extractor">The extractor to invoke when the selector matches.</param>
    public void Add(IRouterExtractorSelector<TExtractionData> selector, IExtractor<TExtractionData> extractor)
        => _extractors.Add(new Entry(selector, extractor));

    /// <summary>Iterates the entries in priority order and returns the first <see cref="SelectionResult"/> whose selector matches, or <see langword="null"/> if none match.</summary>
    /// <param name="data">The source element to match against registered selectors.</param>
    /// <returns>A <see cref="SelectionResult"/> for the first matching entry, or <see langword="null"/>.</returns>
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
    /// <inheritdoc/>
    public int Count => ((ICollection<Entry>)_extractors).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<Entry>)_extractors).IsReadOnly;

    /// <inheritdoc/>
    public Entry this[int index] { get => ((IList<Entry>)_extractors)[index]; set => ((IList<Entry>)_extractors)[index] = value; }

    /// <inheritdoc/>
    public int IndexOf(Entry item)
        => ((IList<Entry>)_extractors).IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, Entry item)
        => ((IList<Entry>)_extractors).Insert(index, item);

    /// <inheritdoc/>
    public void RemoveAt(int index)
        => ((IList<Entry>)_extractors).RemoveAt(index);

    /// <inheritdoc/>
    public void Add(Entry item)
        => ((ICollection<Entry>)_extractors).Add(item);

    /// <inheritdoc/>
    public void Clear()
        => ((ICollection<Entry>)_extractors).Clear();

    /// <inheritdoc/>
    public bool Contains(Entry item)
        => ((ICollection<Entry>)_extractors).Contains(item);

    /// <inheritdoc/>
    public void CopyTo(Entry[] array, int arrayIndex)
        => ((ICollection<Entry>)_extractors).CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Remove(Entry item)
        => ((ICollection<Entry>)_extractors).Remove(item);

    /// <inheritdoc/>
    public IEnumerator<Entry> GetEnumerator()
        => ((IEnumerable<Entry>)_extractors).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_extractors).GetEnumerator();
    #endregion
}
