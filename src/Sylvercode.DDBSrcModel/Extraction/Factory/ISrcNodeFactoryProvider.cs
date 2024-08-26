using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>
{
    ISrcNodeFactory<TExtractionData, TDataSelectable>? GetFactoryForStack(IStructDataStack<TDataSelectable> staskEntries);
}
