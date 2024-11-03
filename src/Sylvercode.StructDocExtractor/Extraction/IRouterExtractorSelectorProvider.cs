namespace Sylvercode.StructDocExtractor.Extraction;

public interface IRouterExtractorSelectorProvider<TExtractionData>
{
    public IRouterExtractorSelector<TExtractionData> DefaultRouterSelector { get; }
}
