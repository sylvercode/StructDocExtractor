using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Validates and normalizes extraction manifest values.</summary>
public static class ExtractionManifestValidator
{
    /// <summary>The manifest filename reserved below every extraction output root.</summary>
    public const string ManifestFileName = "extraction-manifest.json";

    /// <summary>Returns whether an output path is the reserved root manifest filename.</summary>
    public static bool IsReservedManifestFile(string outputFile)
        => string.Equals(
            outputFile,
            ManifestFileName,
            StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns a canonical absolute output root.</summary>
    public static string GetCanonicalOutputRoot(string outputDirectory, string field)
    {
        string root = outputDirectory ?? string.Empty;
        if (string.IsNullOrWhiteSpace(outputDirectory))
            throw ExtractionManifestErrors.InvalidConfiguration(root, field, "The output directory is required.");

        try
        {
            if (Uri.TryCreate(outputDirectory, UriKind.Absolute, out Uri? outputUri)
                && !outputUri.IsFile)
            {
                UriBuilder builder = new(outputUri);
                if (!builder.Path.EndsWith('/'))
                    builder.Path += "/";
                builder.Query = string.Empty;
                builder.Fragment = string.Empty;
                return builder.Uri.AbsoluteUri;
            }

            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(outputDirectory));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw ExtractionManifestErrors.InvalidConfiguration(root, field, "The output directory is invalid.", exception);
        }
    }

    /// <summary>Normalizes an absolute source URI and removes no identity-bearing query data.</summary>
    public static Uri NormalizeSourceUri(Uri? uri, string field, string outputRoot)
    {
        if (uri is null || !uri.IsAbsoluteUri)
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "The URI must be absolute.");
        if (!string.IsNullOrEmpty(uri.Fragment))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "URI fragments are not allowed.");

        UriBuilder builder = new(uri) { Fragment = string.Empty };
        return builder.Uri;
    }

    /// <summary>Normalizes a source base URI to directory semantics.</summary>
    public static Uri NormalizeSourceBaseUri(Uri? uri, string field, string outputRoot)
    {
        Uri normalized = NormalizeSourceUri(uri, field, outputRoot);
        if (!normalized.AbsolutePath.EndsWith('/'))
        {
            UriBuilder builder = new(normalized) { Path = normalized.AbsolutePath + "/" };
            normalized = builder.Uri;
        }

        return normalized;
    }

    /// <summary>Normalizes and validates a portable path relative to an output root.</summary>
    public static string NormalizeOutputFile(string? outputFile, string field, string outputRoot)
    {
        if (string.IsNullOrWhiteSpace(outputFile))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "The output file is required.");
        if (outputFile.Contains('\\', StringComparison.Ordinal))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "Use '/' as the output path separator.");
        if (Path.IsPathRooted(outputFile)
            || outputFile.StartsWith('/')
            || Uri.TryCreate(outputFile, UriKind.Absolute, out _))
        {
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "The output file must be relative.");
        }

        string[] segments = outputFile.Split('/');
        if (segments.Any(string.IsNullOrEmpty))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "The output path is not normalized.");
        if (segments.Any(segment => segment is "." or ".."))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "The output path cannot contain '.' or '..' traversal.");
        if (IsReservedManifestFile(outputFile))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, $"The output path '{ManifestFileName}' is reserved.");

        string normalized = string.Join('/', segments);
        if (Uri.TryCreate(outputRoot, UriKind.Absolute, out Uri? rootUri)
            && !rootUri.IsFile)
        {
            return normalized;
        }

        string combined = Path.GetFullPath(Path.Combine(outputRoot, normalized.Replace('/', Path.DirectorySeparatorChar)));
        if (!IsOutputRootOf(outputRoot, combined) || string.Equals(combined, outputRoot, StringComparison.Ordinal))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, field, "The output path escapes its root.");

        return normalized;
    }

    /// <summary>Validates a complete manifest and returns a canonical, deterministically ordered copy.</summary>
    public static ExtractionManifest ValidateAndNormalize(ExtractionManifest? manifest, string outputRoot)
    {
        if (manifest is null)
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, "$", "The manifest is missing.");
        if (manifest.SchemaVersion != ExtractionManifest.CurrentSchemaVersion)
        {
            throw ExtractionManifestErrors.InvalidManifest(
                outputRoot,
                "schemaVersion",
                $"Unsupported schema version {manifest.SchemaVersion}; expected {ExtractionManifest.CurrentSchemaVersion}.");
        }

        Uri sourceBase = NormalizeSourceBaseUri(manifest.SourceBaseUri, "sourceBaseUri", outputRoot);
        if (manifest.Entries is null)
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, "entries", "The entries collection is required.");

        List<ExtractionManifestEntry> entries = [];
        HashSet<string> sourceUris = new(StringComparer.Ordinal);
        HashSet<string> outputFiles = new(StringComparer.OrdinalIgnoreCase);
        for (int index = 0; index < manifest.Entries.Count; index++)
        {
            ExtractionManifestEntry entry = manifest.Entries[index]
                ?? throw ExtractionManifestErrors.InvalidManifest(outputRoot, $"entries[{index}]", "The entry is missing.");
            Uri sourceUri = NormalizeSourceUri(entry.SourceUri, $"entries[{index}].sourceUri", outputRoot);
            string outputFile = NormalizeOutputFile(entry.OutputFile, $"entries[{index}].outputFile", outputRoot);
            if (!sourceUris.Add(sourceUri.AbsoluteUri))
                throw ExtractionManifestErrors.InvalidManifest(outputRoot, $"entries[{index}].sourceUri", $"Duplicate source URI '{sourceUri}'.");
            if (!outputFiles.Add(outputFile))
                throw ExtractionManifestErrors.InvalidManifest(outputRoot, $"entries[{index}].outputFile", $"Duplicate output file '{outputFile}'.");

            entries.Add(new(sourceUri, outputFile));
        }

        entries.Sort(static (left, right) =>
        {
            int sourceComparison = StringComparer.Ordinal.Compare(left.SourceUri.AbsoluteUri, right.SourceUri.AbsoluteUri);
            return sourceComparison != 0
                ? sourceComparison
                : StringComparer.Ordinal.Compare(left.OutputFile, right.OutputFile);
        });

        return new ExtractionManifest(ExtractionManifest.CurrentSchemaVersion, sourceBase, entries);
    }

    /// <summary>Validates that a resource source is contained by a configured source base.</summary>
    public static void EnsureSourceContained(Uri sourceBaseUri, Uri sourceUri, string outputRoot)
    {
        Uri normalizedBase = NormalizeSourceBaseUri(sourceBaseUri, "sourceBaseUri", outputRoot);
        Uri normalizedSource = NormalizeSourceUri(sourceUri, "sourceUri", outputRoot);
        if (!IsSourceWithinBase(normalizedBase, normalizedSource))
        {
            throw ExtractionManifestErrors.InvalidConfiguration(
                outputRoot,
                "sourceBaseUri",
                $"Source URI '{normalizedSource}' is outside '{normalizedBase}'.");
        }
    }

    /// <summary>Returns whether one canonical output root equals or contains another.</summary>
    public static bool OutputRootsOverlap(string left, string right)
    {
        if (Uri.TryCreate(left, UriKind.Absolute, out Uri? leftUri)
            && !leftUri.IsFile
            && Uri.TryCreate(right, UriKind.Absolute, out Uri? rightUri)
            && !rightUri.IsFile)
        {
            return leftUri.IsBaseOf(rightUri) || rightUri.IsBaseOf(leftUri);
        }

        return IsOutputRootOf(left, right) || IsOutputRootOf(right, left);
    }

    /// <summary>Returns whether source bases overlap at the same or a descendant URI.</summary>
    public static bool SourceBasesOverlap(Uri left, Uri right)
        => left.IsBaseOf(right) || right.IsBaseOf(left);

    internal static bool IsSourceWithinBase(Uri sourceBase, Uri sourceUri)
    {
        if (sourceBase.IsBaseOf(sourceUri))
            return true;

        UriBuilder baseWithoutSlash = new(sourceBase)
        {
            Path = sourceBase.AbsolutePath.TrimEnd('/'),
        };
        return sourceUri == baseWithoutSlash.Uri;
    }

    internal static bool IsOutputRootOf(string root, string path)
    {
        string relative = Path.GetRelativePath(root, path);
        return relative == "." || (!Path.IsPathRooted(relative) && relative != ".."
            && !relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal));
    }

    /// <summary>Builds the portable path that the datastore resolves from an output URI.</summary>
    /// <param name="outputBaseUri">The canonical base URI of the active datastore.</param>
    /// <param name="outputUri">The output URI passed to the datastore after resource translation.</param>
    /// <returns>A normalized path relative to <paramref name="outputBaseUri"/>.</returns>
    /// <remarks>
    /// Query and fragment never identify a physical file and are removed regardless of the
    /// configurable source-identity policy.
    /// </remarks>
    public static string GetRelativeOutputPath(Uri outputBaseUri, Uri outputUri)
    {
        string outputRoot = outputBaseUri.IsFile
            ? outputBaseUri.LocalPath
            : outputBaseUri.AbsoluteUri;
        try
        {
            return NormalizeOutputFile(
                DataStorePathResolver.Resolve(outputBaseUri, outputUri).RelativePath,
                "outputFile",
                outputRoot);
        }
        catch (ArgumentException exception)
        {
            throw ExtractionManifestErrors.InvalidManifest(
                outputRoot,
                "outputFile",
                $"Output URI '{outputUri}' is invalid: {exception.Message}",
                exception);
        }
    }

}
