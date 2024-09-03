using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface ISrcNodeFactory<TExtractionData, TDataSelectable>
{
    IProcessTaskResult<TExtractionData, TDataSelectable> NewNode(TExtractionData data);
}
