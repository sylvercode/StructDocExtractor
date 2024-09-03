using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface ISrcNodeFactory<TExtractionData, TDataDiscriminator>
{
    IProcessTaskResult<TExtractionData, TDataDiscriminator> NewNode(TExtractionData data);
}
