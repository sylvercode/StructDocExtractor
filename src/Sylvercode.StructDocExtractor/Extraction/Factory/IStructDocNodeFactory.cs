namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface IStructDocNodeFactory<TExtractionData, TDataDiscriminator>
{
    IProcessTaskResult<TExtractionData, TDataDiscriminator> NewNode(TExtractionData data);
}
