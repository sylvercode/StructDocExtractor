using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Handles validation, loading, and persistence of extraction manifests.</summary>
public sealed class ExtractionManifestHandler(
    IOptions<SiteExtractorOptions> options,
    IExtractionManifestStore manifestStore,
    IExtractionManifestCatalog catalog) : IExtractionObserver
{
    /// <inheritdoc/>
    public void OnNext(ExtractionEvent value)
    {
        switch (value)
        {
            case ExtractionStarted started:
                BeginExtraction(started.SeedUri);
                break;
            case ExtractionCompleted completed:
                CompleteExtraction(completed.Resources);
                break;
        }
    }

    private void BeginExtraction(Uri seedUri)
    {
        (string outputRoot, Uri sourceBaseUri) = GetConfiguredRoots();
        ExtractionManifestValidator.EnsureSourceContained(
            sourceBaseUri,
            seedUri,
            outputRoot);

        Dictionary<string, ExtractionManifest> linkedManifests = new(StringComparer.Ordinal);
        List<string> configuredLinkedRoots = GetConfiguredLinkedRoots(outputRoot);
        List<(string Root, Uri SourceBase)> linkedRoots = [];
        foreach (string linkedRoot in configuredLinkedRoots)
        {
            ExtractionManifest linkedManifest = manifestStore.Load(linkedRoot);
            linkedManifest = ExtractionManifestValidator.ValidateAndNormalize(
                linkedManifest,
                linkedRoot);
            if (ExtractionManifestValidator.SourceBasesOverlap(
                    sourceBaseUri,
                    linkedManifest.SourceBaseUri)
                || linkedRoots.Any(existing =>
                    ExtractionManifestValidator.SourceBasesOverlap(
                        existing.SourceBase,
                        linkedManifest.SourceBaseUri)))
            {
                throw ExtractionManifestErrors.InvalidConfiguration(
                    linkedRoot,
                    "sourceBaseUri",
                    $"Source base '{linkedManifest.SourceBaseUri}' overlaps another configured extraction.");
            }

            linkedRoots.Add((linkedRoot, linkedManifest.SourceBaseUri));
            linkedManifests.Add(linkedRoot, linkedManifest);
        }

        ValidateCrossManifestMappings(linkedManifests, outputRoot);
        catalog.SetLinkedManifests(linkedManifests);
    }

    private void CompleteExtraction(IReadOnlyResourceRepository resources)
    {
        (string outputRoot, Uri sourceBaseUri) = GetConfiguredRoots();
        List<ExtractionManifestEntry> entries = [];
        foreach (Resource resource in resources)
        {
            if (resource.State.ExtractionState != ResourceExtractionState.OutputProduced)
                continue;

            Uri sourceUri = ExtractionManifestValidator.NormalizeSourceUri(
                resource.Uri,
                "sourceUri",
                outputRoot);
            ExtractionManifestValidator.EnsureSourceContained(
                sourceBaseUri,
                sourceUri,
                outputRoot);
            string outputFile = ExtractionManifestValidator.GetOutputFile(
                outputRoot,
                resource.TranslatedResourceUri);
            entries.Add(new ExtractionManifestEntry(sourceUri, outputFile));
        }

        ExtractionManifest manifest = ExtractionManifestValidator.ValidateAndNormalize(
            new ExtractionManifest(
                ExtractionManifest.CurrentSchemaVersion,
                sourceBaseUri,
                entries),
            outputRoot);
        ValidateCrossManifestMappings(catalog.LinkedManifests, outputRoot, manifest);
        manifestStore.Write(outputRoot, manifest);
    }

    private (string OutputRoot, Uri SourceBaseUri) GetConfiguredRoots()
    {
        SiteExtractorOptions configuredOptions = options.Value;
        string outputRoot = ExtractionManifestValidator.GetCanonicalOutputRoot(
            configuredOptions.OutputDirectory,
            nameof(SiteExtractorOptions.OutputDirectory));
        Uri sourceBaseUri;
        try
        {
            sourceBaseUri = configuredOptions.GetSourceBaseUri();
        }
        catch (Exception exception) when (
            exception is InvalidOperationException or UriFormatException or ArgumentException)
        {
            throw ExtractionManifestErrors.InvalidConfiguration(
                outputRoot,
                nameof(SiteExtractorOptions.SourceAuthority),
                "The source base URI is invalid.",
                exception);
        }

        try
        {
            sourceBaseUri = ExtractionManifestValidator.NormalizeSourceBaseUri(
                sourceBaseUri,
                "sourceBaseUri",
                outputRoot);
        }
        catch (InvalidDataException exception)
        {
            throw ExtractionManifestErrors.InvalidConfiguration(
                outputRoot,
                "sourceBaseUri",
                "The configured source base URI is invalid.",
                exception);
        }

        return (outputRoot, sourceBaseUri);
    }

    private List<string> GetConfiguredLinkedRoots(string outputRoot)
    {
        SiteExtractorOptions configuredOptions = options.Value;
        List<string> configuredLinkedRoots = [];
        foreach ((LinkedExtractionOptions linked, int index) in
            (configuredOptions.LinkedExtractions ?? []).Select(
                static (linked, index) => (linked, index)))
        {
            if (linked is null)
            {
                throw ExtractionManifestErrors.InvalidConfiguration(
                    outputRoot,
                    $"linkedExtractions[{index}]",
                    "The linked extraction entry is required.");
            }

            string linkedRoot = ExtractionManifestValidator.GetCanonicalOutputRoot(
                linked.OutputDirectory,
                $"linkedExtractions[{index}].outputDirectory");
            if (ExtractionManifestValidator.OutputRootsOverlap(outputRoot, linkedRoot))
            {
                throw ExtractionManifestErrors.InvalidConfiguration(
                    linkedRoot,
                    $"linkedExtractions[{index}].outputDirectory",
                    $"Output root overlaps current or another configured root '{outputRoot}'.");
            }
            if (configuredLinkedRoots.Any(existing =>
                ExtractionManifestValidator.OutputRootsOverlap(existing, linkedRoot)))
            {
                throw ExtractionManifestErrors.InvalidConfiguration(
                    linkedRoot,
                    $"linkedExtractions[{index}].outputDirectory",
                    "A duplicate or overlapping linked output root was configured.");
            }

            configuredLinkedRoots.Add(linkedRoot);
        }

        return configuredLinkedRoots;
    }

    private static void ValidateCrossManifestMappings(
        IReadOnlyDictionary<string, ExtractionManifest> manifests,
        string currentRoot,
        ExtractionManifest? currentManifest = null)
    {
        Dictionary<string, string> sourceMappings = new(StringComparer.Ordinal);
        Dictionary<string, string> outputMappings = new(StringComparer.OrdinalIgnoreCase);

        IEnumerable<KeyValuePair<string, ExtractionManifest>> allManifests = manifests;
        if (currentManifest is not null)
        {
            allManifests = allManifests.Append(
                new KeyValuePair<string, ExtractionManifest>(currentRoot, currentManifest));
        }

        foreach (var (root, manifest) in allManifests)
        {
            foreach (ExtractionManifestEntry entry in manifest.Entries)
            {
                if (sourceMappings.TryGetValue(
                    entry.SourceUri.AbsoluteUri,
                    out string? sourceRoot))
                {
                    throw ExtractionManifestErrors.InvalidManifest(
                        root,
                        "sourceUri",
                        $"Source URI '{entry.SourceUri}' is already mapped by root '{sourceRoot}'.");
                }
                sourceMappings.Add(entry.SourceUri.AbsoluteUri, root);

                string outputIdentity = Path.Combine(
                    root,
                    entry.OutputFile.Replace('/', Path.DirectorySeparatorChar));
                if (outputMappings.TryGetValue(outputIdentity, out string? outputRoot))
                {
                    throw ExtractionManifestErrors.InvalidManifest(
                        root,
                        "outputFile",
                        $"Output file '{entry.OutputFile}' is already mapped by root '{outputRoot}'.");
                }
                outputMappings.Add(outputIdentity, root);
            }
        }
    }
}
