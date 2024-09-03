namespace Sylvercode.StructDocExtractor.Extraction.Factory;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface ISrcNodeFactoryProviderStack<TExtractionData, TDataSelectable>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> DefaultSrcNodeFactoryProvider { get; }
    ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> GetActiveSrcNodeFactoryProvider();
}
