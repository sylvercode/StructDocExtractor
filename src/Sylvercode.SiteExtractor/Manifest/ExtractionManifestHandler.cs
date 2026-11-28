using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Handles validation, loading, and persistence of extraction manifests.</summary>
/// <param name="options">The configured current extraction roots.</param>
/// <param name="manifestStore">The store used for linked and current manifests.</param>
/// <param name="catalog">The linked-manifest catalog retained for later resolution.</param>
/// <param name="queryIdentityFilter">The policy used to canonicalize persisted source identity.</param>
/// <param name="dataStore">The active output datastore supplying the canonical output base URI.</param>
/// <remarks>
/// This observer is stateless between extraction calls. It derives both current roots from
/// <see cref="IOptions{TOptions}.Value"/> for every lifecycle event.
/// </remarks>
public sealed class ExtractionManifestHandler(
    IOptions<SiteExtractorOptions> options,
    IExtractionManifestStore manifestStore,
    IExtractionManifestCatalog catalog,
    IUriQueryIdentityFilter queryIdentityFilter,
    IDataStore dataStore) : IExtractionObserver
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
                CompleteExtraction(completed);
                break;
        }
    }

    private void BeginExtraction(Uri seedUri)
    {
        ConfiguredRoots roots = GetConfiguredRoots();
        string outputRoot = roots.OutputRoot;
        Uri sourceBaseUri = roots.SourceBaseUri;
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

    private void CompleteExtraction(ExtractionCompleted completed)
    {
        ConfiguredRoots roots = GetConfiguredRoots();
        string outputRoot = roots.OutputRoot;
        Uri outputBaseUri = roots.OutputBaseUri;
        Uri sourceBaseUri = roots.SourceBaseUri;
        List<ExtractionManifestEntry> entries = [];
        foreach (Resource resource in completed.Resources)
        {
            if (resource.State.ExtractionState != ResourceExtractionState.OutputProduced)
                continue;

            Uri sourceUri = ExtractionManifestValidator.NormalizeSourceUri(
                queryIdentityFilter.Filter(resource.Uri),
                "sourceUri",
                outputRoot);
            if (resource.OutputKind == ResourceOutputKind.Document)
            {
                ExtractionManifestValidator.EnsureSourceContained(
                    sourceBaseUri,
                    sourceUri,
                    outputRoot);
            }
            string outputFile = ExtractionManifestValidator.GetRelativeOutputPath(
                outputBaseUri,
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
        completed.AddCommitAction(() => manifestStore.Write(outputRoot, manifest));
    }

    private ConfiguredRoots GetConfiguredRoots()
    {
        SiteExtractorOptions configuredOptions = options.Value;
        Uri outputBaseUri = dataStore.BaseUri;
        if (!outputBaseUri.IsAbsoluteUri)
        {
            outputBaseUri = new Uri(
                Path.TrimEndingDirectorySeparator(
                    Path.GetFullPath(outputBaseUri.OriginalString))
                + Path.DirectorySeparatorChar);
        }
        string outputRoot = ExtractionManifestValidator.GetCanonicalOutputRoot(
            outputBaseUri.IsFile
                ? outputBaseUri.LocalPath
                : outputBaseUri.AbsoluteUri,
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

        return new ConfiguredRoots(outputRoot, outputBaseUri, sourceBaseUri);
    }

    /// <summary>Represents the normalized roots required for one manifest lifecycle event.</summary>
    /// <param name="OutputRoot">The canonical datastore root identifier used as manifest key.</param>
    /// <param name="OutputBaseUri">The active datastore base URI used to resolve output identities.</param>
    /// <param name="SourceBaseUri">The normalized source base URI used to validate documents.</param>
    private sealed record ConfiguredRoots(
        string OutputRoot,
        Uri OutputBaseUri,
        Uri SourceBaseUri);

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
