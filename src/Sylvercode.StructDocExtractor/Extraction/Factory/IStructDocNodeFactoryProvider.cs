using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>
{
    IStructDocNodeFactory<TExtractionData, TDataDiscriminator>? GetFactoryForStack(IStructDataStack<TDataDiscriminator> stackEntries);
    string DebugName => GetType().Name;
}
