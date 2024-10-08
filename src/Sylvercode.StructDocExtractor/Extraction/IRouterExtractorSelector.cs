namespace Sylvercode.StructDocExtractor.Extraction;

public interface IRouterExtractorSelector<TExtractionData>
{
    public TExtractionData? Match(TExtractionData data);
}
