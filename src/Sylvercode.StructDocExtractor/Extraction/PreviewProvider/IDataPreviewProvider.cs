namespace Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

/// <summary>Defines the untyped contract for producing a human-readable preview string from source data, used in logging and diagnostics.</summary>
public interface IDataPreviewProvider
{
    /// <summary>Returns a short preview string for <paramref name="data"/>, or an empty string if <paramref name="data"/> is <see langword="null"/>.</summary>
    /// <param name="data">The source data object to preview.</param>
    /// <returns>A short human-readable string representing the data.</returns>
    string GetPreview(object? data);
}

/// <summary>Typed extension of <see cref="IDataPreviewProvider"/> that generates preview strings from strongly-typed source data.</summary>
/// <typeparam name="TData">The type of source data this provider handles.</typeparam>
public interface IDataPreviewProvider<TData> : IDataPreviewProvider
{
    /// <summary>Returns a short preview string for the typed <paramref name="data"/>.</summary>
    /// <param name="data">The typed source data to preview; may be <see langword="null"/>.</param>
    /// <returns>A short human-readable string, or empty if <paramref name="data"/> is <see langword="null"/>.</returns>
    string GetPreview(TData? data);
    /// <inheritdoc/>
    string IDataPreviewProvider.GetPreview(object? data) => GetPreview((TData?)data);
}
