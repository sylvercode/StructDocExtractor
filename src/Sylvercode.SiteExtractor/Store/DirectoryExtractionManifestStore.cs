using System.Text.Json;
using System.Text.Json.Serialization;
using Sylvercode.SiteExtractor.Manifest;

namespace Sylvercode.SiteExtractor.Store;

/// <summary>Strict JSON extraction-manifest store backed by an output directory.</summary>
public sealed class DirectoryExtractionManifestStore : IExtractionManifestStore
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        WriteIndented = true,
    };

    /// <inheritdoc/>
    public ExtractionManifest Load(string outputRoot)
    {
        string root = ExtractionManifestValidator.GetCanonicalOutputRoot(outputRoot, "outputDirectory");
        string manifestPath = Path.Combine(root, ExtractionManifestValidator.ManifestFileName);
        if (!File.Exists(manifestPath))
            throw ExtractionManifestErrors.InvalidManifest(root, "$", $"Required manifest '{ExtractionManifestValidator.ManifestFileName}' is missing.");

        try
        {
            using FileStream stream = File.OpenRead(manifestPath);
            ExtractionManifest? manifest = JsonSerializer.Deserialize<ExtractionManifest>(stream, _jsonOptions);
            return ExtractionManifestValidator.ValidateAndNormalize(manifest, root);
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw ExtractionManifestErrors.InvalidManifest(
                root,
                exception.Path ?? "$",
                $"Manifest JSON is invalid: {exception.Message}",
                exception);
        }
        catch (IOException exception)
        {
            throw ExtractionManifestErrors.InvalidManifest(root, "$", $"Manifest could not be read: {exception.Message}", exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw ExtractionManifestErrors.InvalidManifest(root, "$", $"Manifest could not be read: {exception.Message}", exception);
        }
    }

    /// <inheritdoc/>
    public void Write(string outputRoot, ExtractionManifest manifest)
    {
        string root = ExtractionManifestValidator.GetCanonicalOutputRoot(outputRoot, "outputDirectory");
        ExtractionManifest normalized = ExtractionManifestValidator.ValidateAndNormalize(manifest, root);
        string manifestPath = Path.Combine(root, ExtractionManifestValidator.ManifestFileName);
        string temporaryPath = Path.Combine(root, $".{ExtractionManifestValidator.ManifestFileName}.{Guid.NewGuid():N}.tmp");
        bool temporaryFileCreated = false;

        try
        {
            Directory.CreateDirectory(root);
            using (FileStream stream = new(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                temporaryFileCreated = true;
                JsonSerializer.Serialize(stream, normalized, _jsonOptions);
            }

            File.Move(temporaryPath, manifestPath, overwrite: true);
            temporaryFileCreated = false;
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw ExtractionManifestErrors.InvalidManifest(root, "$", $"Manifest could not be written: {exception.Message}", exception);
        }
        finally
        {
            if (temporaryFileCreated)
                File.Delete(temporaryPath);
        }
    }
}
