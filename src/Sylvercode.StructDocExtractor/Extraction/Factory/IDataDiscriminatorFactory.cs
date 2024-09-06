namespace Sylvercode.StructDocExtractor.Extraction.Factory;

public interface IDataDiscriminatorFactory
{
    object? CreateDataDiscriminator(object data);
}

public interface IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator> : IDataDiscriminatorFactory
{
    TDataDiscriminator CreateDataDiscriminator(TExtractionData data);
    object? IDataDiscriminatorFactory.CreateDataDiscriminator(object data)
        => CreateDataDiscriminator((TExtractionData)data);
}
