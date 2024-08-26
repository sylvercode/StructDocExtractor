namespace Sylvercode.DDBSrcModel.Extraction.Factory;

public interface ISrcNodeFactoryProviderStack<TExtractionData, TDataSelectable>
{
    ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> DefaulSrcNodeFactoryProvider { get; }
    ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> GetActiveSrcNodeFactoryProvider();
}
