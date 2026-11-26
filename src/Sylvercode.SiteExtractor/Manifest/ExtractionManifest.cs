using System.Text.Json.Serialization;

namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Describes the exact source-to-output mapping produced by one extraction.</summary>
/// <remarks>Initializes a new extraction manifest.</remarks>
[method: JsonConstructor]
/// <summary>Describes the exact source-to-output mapping produced by one extraction.</summary>
public sealed class ExtractionManifest(
    int schemaVersion,
    Uri sourceBaseUri,
    IReadOnlyList<ExtractionManifestEntry> entries)
{
    /// <summary>The current schema version.</summary>
    public const int CurrentSchemaVersion = 1;

    /// <summary>Gets the schema version.</summary>
    [JsonRequired]
    public int SchemaVersion { get; init; } = schemaVersion;

    /// <summary>Gets the absolute source base URI.</summary>
    [JsonRequired]
    public Uri SourceBaseUri { get; init; } = sourceBaseUri;

    /// <summary>Gets the generated source-to-output entries.</summary>
    [JsonRequired]
    public IReadOnlyList<ExtractionManifestEntry> Entries { get; init; } = entries is null ? null! : Array.AsReadOnly(entries.ToArray());
}

/// <summary>Maps one absolute source URI to a file below an extraction output root.</summary>
/// <remarks>Initializes a new extraction manifest entry.</remarks>
[method: JsonConstructor]
/// <summary>Maps one absolute source URI to a file below an extraction output root.</summary>
public sealed class ExtractionManifestEntry(Uri sourceUri, string outputFile)
{

    /// <summary>Gets the fragment-free source URI.</summary>
    [JsonRequired]
    public Uri SourceUri { get; init; } = sourceUri;

    /// <summary>Gets the portable path relative to the extraction output root.</summary>
    [JsonRequired]
    public string OutputFile { get; init; } = outputFile;
}
