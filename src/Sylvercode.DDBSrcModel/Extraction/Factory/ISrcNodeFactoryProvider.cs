using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactoryProvider<N>
{
    ISrcNodeFactory? GetFactoryForStack(IStructDataStack<N> staskEntries);
}
