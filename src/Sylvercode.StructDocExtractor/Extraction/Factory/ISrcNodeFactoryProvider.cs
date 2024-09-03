using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>
{
    ISrcNodeFactory<TExtractionData, TDataSelectable>? GetFactoryForStack(IStructDataStack<TDataSelectable> stackEntries);
    string DebugName => GetType().Name;
}
