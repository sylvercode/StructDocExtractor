namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Defines the untyped contract for producing a data discriminator object from raw source data, used to select the appropriate node factory.</summary>
public interface IDataDiscriminatorFactory
{
    /// <summary>Creates a discriminator value from <paramref name="data"/> that will be used to route factory selection.</summary>
    /// <param name="data">The raw source data object to discriminate.</param>
    /// <returns>A discriminator object, or <see langword="null"/> if the data cannot be discriminated.</returns>
    object? CreateDataDiscriminator(object data);
}

/// <summary>Typed extension of <see cref="IDataDiscriminatorFactory"/> that produces strongly-typed discriminators from typed source data.</summary>
/// <typeparam name="TExtractionData">The type of source data being discriminated.</typeparam>
/// <typeparam name="TDataDiscriminator">The type of discriminator produced.</typeparam>
public interface IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator> : IDataDiscriminatorFactory
{
    /// <summary>Creates a typed discriminator from <paramref name="data"/> to guide factory selection.</summary>
    /// <param name="data">The typed source data to discriminate.</param>
    /// <returns>A <typeparamref name="TDataDiscriminator"/> value derived from <paramref name="data"/>.</returns>
    TDataDiscriminator CreateDataDiscriminator(TExtractionData data);
    /// <inheritdoc/>
    object? IDataDiscriminatorFactory.CreateDataDiscriminator(object data)
        => CreateDataDiscriminator((TExtractionData)data);
}
