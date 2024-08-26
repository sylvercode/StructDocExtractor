namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactoryProviderStack<N>
{
    public ISrcNodeFactoryProvider<N> DefaulSrcNodeFactoryProvider { get; }
    public ISrcNodeFactoryProvider<N> GetActiveSrcNodeFactoryProvider();
}
