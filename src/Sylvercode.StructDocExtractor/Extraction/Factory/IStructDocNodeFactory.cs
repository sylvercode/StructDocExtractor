namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Defines the contract for creating an extraction task result from a discriminated source element.</summary>
/// <typeparam name="TExtractionData">The type of source data being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The type of the data discriminator that matched this factory.</typeparam>
public interface IStructDocNodeFactory<TExtractionData, TDataDiscriminator>
{
    /// <summary>Creates a new <see cref="IProcessTaskResult{TExtractionData, TDataDiscriminator}"/> for <paramref name="data"/>, using <paramref name="discriminator"/> to guide node construction.</summary>
    /// <param name="discriminator">The discriminator value that caused this factory to be selected.</param>
    /// <param name="data">The raw source data from which to produce a structural node.</param>
    /// <returns>A typed result describing the produced node, metadata, and any child task data.</returns>
    IProcessTaskResult<TExtractionData, TDataDiscriminator> NewNode(TDataDiscriminator discriminator, TExtractionData data);
}
