namespace Sylvercode.DDBSrcModel.Extraction.Factory;


public interface ISrcNodeFactoryProviderStack<TDataSelectable>
{
    ISrcNodeFactoryProvider<TDataSelectable> DefaulSrcNodeFactoryProvider { get; }
    ISrcNodeFactoryProvider<TDataSelectable> GetActiveSrcNodeFactoryProvider();
}

public interface ISrcNodeFactoryProviderStack<TExtractionData, TDataSelectable>
    : ISrcNodeFactoryProviderStack<TDataSelectable>
{
    new ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> DefaulSrcNodeFactoryProvider { get; }
    ISrcNodeFactoryProvider<TDataSelectable> ISrcNodeFactoryProviderStack<TDataSelectable>.DefaulSrcNodeFactoryProvider
        => DefaulSrcNodeFactoryProvider;
    new ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> GetActiveSrcNodeFactoryProvider();
    ISrcNodeFactoryProvider<TDataSelectable> ISrcNodeFactoryProviderStack<TDataSelectable>.GetActiveSrcNodeFactoryProvider()
        => GetActiveSrcNodeFactoryProvider();
}
