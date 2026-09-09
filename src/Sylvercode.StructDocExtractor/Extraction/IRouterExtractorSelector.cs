namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Defines the contract for matching source data against a router's routing criteria to determine if a specific extractor should handle it.</summary>
/// <typeparam name="TExtractionData">The type of source data being matched.</typeparam>
public interface IRouterExtractorSelector<TExtractionData>
{
    /// <summary>Tests <paramref name="data"/> against this selector's routing criteria and returns the data if matched, or <see langword="null"/> if not matched.</summary>
    /// <param name="data">The source data item to match against the routing rule.</param>
    /// <returns>The matched data value if this selector accepts it; otherwise <see langword="null"/>.</returns>
    public TExtractionData? Match(TExtractionData data);
}
