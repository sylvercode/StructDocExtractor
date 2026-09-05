namespace Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

/// <summary>Generic preview provider that calls <see cref="object.ToString"/> on the source data and truncates the result to a configurable character limit.</summary>
/// <typeparam name="TData">The type of source data this provider handles.</typeparam>
/// <param name="characterCountForPreview">The maximum number of characters to include in the preview; defaults to 20.</param>
public class ToStringPreviewProvider<TData>(int characterCountForPreview = 20) : IDataPreviewProvider<TData>
{

    /// <inheritdoc/>
    public string GetPreview(TData? data)
    {
        var dataToString = data?.ToString();
        if (dataToString is null)
            return string.Empty;

        if (dataToString.Length <= characterCountForPreview)
            return dataToString;

        return dataToString[..characterCountForPreview];
    }
}
