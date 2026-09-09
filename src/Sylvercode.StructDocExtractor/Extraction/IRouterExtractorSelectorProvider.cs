namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Defines the contract for providing the default <see cref="IRouterExtractorSelector{TExtractionData}"/> that routes data to a specific extractor.</summary>
/// <typeparam name="TExtractionData">The type of source data the selector routes.</typeparam>
public interface IRouterExtractorSelectorProvider<TExtractionData>
{
    /// <summary>Gets the default selector used to match source data against this extractor's routing criteria.</summary>
    public IRouterExtractorSelector<TExtractionData> DefaultRouterSelector { get; }
}
