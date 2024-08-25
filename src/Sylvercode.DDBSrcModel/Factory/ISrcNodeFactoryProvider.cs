using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Factory;

public interface ISrcNodeFactoryProvider<N>
{
    ISrcNodeFactory? GetFactoryForStack(IStructDataStack<N> staskEntries);
}
