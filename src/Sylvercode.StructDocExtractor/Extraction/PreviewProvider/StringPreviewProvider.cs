namespace Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

/// <summary>Preview provider that returns a raw string value directly, truncated to a configurable character limit.</summary>
/// <param name="characterCountForPreview">The maximum number of characters to include in the preview; defaults to 20.</param>
public class StringPreviewProvider(int characterCountForPreview = 20) : IDataPreviewProvider<string?>
{
    /// <inheritdoc/>
    public string GetPreview(string? data)
        => data is null
            ? string.Empty
            : data.Length <= characterCountForPreview
                ? data
                : data[..characterCountForPreview];
}
