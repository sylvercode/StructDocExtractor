using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactory<TExtractionData, TDataSelectable>
{
    IProcessTaskResult<TExtractionData, TDataSelectable> NewNode(TExtractionData data);
}
