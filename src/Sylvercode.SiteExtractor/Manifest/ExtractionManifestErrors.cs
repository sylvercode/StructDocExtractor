namespace Sylvercode.SiteExtractor.Manifest;

internal static class ExtractionManifestErrors
{
    public static InvalidDataException InvalidManifest(
        string outputRoot,
        string field,
        string message,
        Exception? innerException = null)
        => new(
            $"Extraction manifest error for root '{outputRoot}' at '{field}': {message}",
            innerException);

    public static InvalidOperationException InvalidConfiguration(
        string outputRoot,
        string field,
        string message,
        Exception? innerException = null)
        => new(
            $"Site extractor configuration error for root '{outputRoot}' at '{field}': {message}",
            innerException);
}
