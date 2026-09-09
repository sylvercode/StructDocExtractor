namespace Sylvercode.StructDocExtractor.Extraction.Factory;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
/// <summary>Defines the contract for a layered stack of factory providers, enabling ordered resolution of the active provider for the current extraction context.</summary>
/// <typeparam name="TExtractionData">The type of source data being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The type of the data discriminator used to route factory selection.</typeparam>
public interface IStructDocNodeFactoryProviderStack<TExtractionData, TDataDiscriminator>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    /// <summary>Gets the default factory provider used as a fallback when no stacked provider matches the current context.</summary>
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultSrcNodeFactoryProvider { get; }
    /// <summary>Returns the currently active factory provider, which may be the default or a context-specific override pushed onto the stack.</summary>
    /// <returns>The active <see cref="IStructDocNodeFactoryProvider{TExtractionData, TDataDiscriminator}"/>.</returns>
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetActiveSrcNodeFactoryProvider();
}
