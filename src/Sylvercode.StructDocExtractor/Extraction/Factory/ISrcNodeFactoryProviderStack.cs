namespace Sylvercode.StructDocExtractor.Extraction.Factory;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface ISrcNodeFactoryProviderStack<TExtractionData, TDataDiscriminator>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultSrcNodeFactoryProvider { get; }
    ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetActiveSrcNodeFactoryProvider();
}
