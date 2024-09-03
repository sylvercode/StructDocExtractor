namespace Sylvercode.StructDocExtractor.Extraction.Factory;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IStructDocNodeFactoryProviderStack<TExtractionData, TDataDiscriminator>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultSrcNodeFactoryProvider { get; }
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetActiveSrcNodeFactoryProvider();
}
