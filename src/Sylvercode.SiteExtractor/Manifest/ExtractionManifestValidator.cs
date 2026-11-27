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
            if (!IsSourceWithinBase(sourceBase, sourceUri))
            {
                throw ExtractionManifestErrors.InvalidManifest(
                    outputRoot,
                    $"entries[{index}].sourceUri",
                    $"The source URI is outside sourceBaseUri '{sourceBase}'.");
            }
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
        => IsOutputRootOf(left, right) || IsOutputRootOf(right, left);

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

    /// <summary>Builds a portable output path from a finalized translated URI.</summary>
    public static string GetOutputFile(string outputRoot, Uri translatedUri)
    {
        bool hasQueryOrFragment = translatedUri.IsAbsoluteUri
            ? !string.IsNullOrEmpty(translatedUri.Fragment) || !string.IsNullOrEmpty(translatedUri.Query)
            : translatedUri.OriginalString.IndexOfAny(['?', '#']) >= 0;
        if (hasQueryOrFragment)
        {
            throw ExtractionManifestErrors.InvalidManifest(
                outputRoot,
                "outputFile",
                "The finalized output URI cannot contain a query or fragment.");
        }
        if (translatedUri.IsAbsoluteUri && translatedUri.Scheme != Uri.UriSchemeFile)
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, "outputFile", $"Output URI '{translatedUri}' is not a file URI.");

        string path = translatedUri.IsAbsoluteUri
            ? translatedUri.LocalPath
            : Uri.UnescapeDataString(translatedUri.OriginalString.Replace('/', Path.DirectorySeparatorChar));

        string completePath = translatedUri.IsAbsoluteUri
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(outputRoot, path));
        if (!IsOutputRootOf(outputRoot, completePath) || string.Equals(completePath, outputRoot, StringComparison.Ordinal))
            throw ExtractionManifestErrors.InvalidManifest(outputRoot, "outputFile", $"Output URI '{translatedUri}' escapes the output root.");

        string relative = Path.GetRelativePath(outputRoot, completePath).Replace(Path.DirectorySeparatorChar, '/');
        return NormalizeOutputFile(relative, "outputFile", outputRoot);
    }
}
