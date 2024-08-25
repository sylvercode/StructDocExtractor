namespace Sylvercode.DDBSrcModel.Factory;

public interface ISrcNodeFactoryProviderStack<N>
{
    public ISrcNodeFactoryProvider<N> DefaulSrcNodeFactoryProvider { get; }
    public ISrcNodeFactoryProvider<N> GetActiveSrcNodeFactoryProvider();
}
