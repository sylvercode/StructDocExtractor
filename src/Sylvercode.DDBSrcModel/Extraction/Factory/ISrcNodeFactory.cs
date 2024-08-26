using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactory
{
    IProcessTaskResult NewNode(object data);
}

public interface ISrcNodeFactory<TExtractionData, TDataSelectable> : ISrcNodeFactory
{
    IProcessTaskResult<TExtractionData, TDataSelectable> NewNode(TExtractionData data);
    IProcessTaskResult ISrcNodeFactory.NewNode(object data)
        => NewNode(data);

}
