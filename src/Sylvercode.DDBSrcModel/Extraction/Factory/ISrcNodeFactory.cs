using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactory
{
    IProcessTaskResult NewNode(object data);
}

public interface ISrcNodeFactory<TExtractionData> : ISrcNodeFactory
{
    IProcessTaskResult<TExtractionData> NewNode(TExtractionData data);
    IProcessTaskResult ISrcNodeFactory.NewNode(object data) => NewNode((TExtractionData)data);
}

public interface ISrcNodeFactory<TExtractionData, TDataSelectable> : ISrcNodeFactory<TExtractionData>
{
    new IProcessTaskResult<TExtractionData, TDataSelectable> NewNode(TExtractionData data);
    IProcessTaskResult<TExtractionData> ISrcNodeFactory<TExtractionData>.NewNode(TExtractionData data)
        => NewNode(data);

}
