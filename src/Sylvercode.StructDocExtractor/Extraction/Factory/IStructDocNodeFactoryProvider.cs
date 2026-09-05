using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Defines the contract for resolving an <see cref="IStructDocNodeFactory{TExtractionData, TDataDiscriminator}"/> based on the current discriminator stack state.</summary>
/// <typeparam name="TExtractionData">The type of source data being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The type of the data discriminator used to route factory selection.</typeparam>
public interface IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>
{
    /// <summary>Resolves the appropriate factory for the given stack of discriminated entries, or <see langword="null"/> if this provider does not handle the current stack state.</summary>
    /// <param name="stackEntries">The current discriminator stack describing the path from root to the node being processed.</param>
    /// <returns>A matching <see cref="IStructDocNodeFactory{TExtractionData, TDataDiscriminator}"/>, or <see langword="null"/>.</returns>
    IStructDocNodeFactory<TExtractionData, TDataDiscriminator>? GetFactoryForStack(IStructDataStack<TDataDiscriminator> stackEntries);
    /// <summary>Gets the debug name of this provider, used in diagnostics and logging; defaults to the type name.</summary>
    string DebugName => GetType().Name;
}
