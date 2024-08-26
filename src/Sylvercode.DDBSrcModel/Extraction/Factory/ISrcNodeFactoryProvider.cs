using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactoryProvider<TDataSelectable>
{
    ISrcNodeFactory? GetFactoryForStack(IStructDataStack<TDataSelectable> staskEntries);
}

public interface ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> : ISrcNodeFactoryProvider<TDataSelectable>
{
    new ISrcNodeFactory<TExtractionData, TDataSelectable>? GetFactoryForStack(IStructDataStack<TDataSelectable> staskEntries);
    ISrcNodeFactory? ISrcNodeFactoryProvider<TDataSelectable>.GetFactoryForStack(IStructDataStack<TDataSelectable> staskEntries)
        => GetFactoryForStack(staskEntries);
}
