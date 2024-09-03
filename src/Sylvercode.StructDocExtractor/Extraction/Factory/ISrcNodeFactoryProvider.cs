using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator>
{
    ISrcNodeFactory<TExtractionData, TDataDiscriminator>? GetFactoryForStack(IStructDataStack<TDataDiscriminator> stackEntries);
    string DebugName => GetType().Name;
}
