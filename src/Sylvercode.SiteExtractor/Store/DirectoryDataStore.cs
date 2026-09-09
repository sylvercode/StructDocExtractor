using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;


namespace Sylvercode.SiteExtractor.Store;

/// <summary>File-system <see cref="IDataStore"/> that writes each extracted resource to a file within a configurable output directory.</summary>
/// <remarks>
/// Resolves relative base URIs against the current working directory.
/// Parent directories are created automatically on first write.
/// </remarks>
public partial class DirectoryDataStore(
    IOptions<SiteExtractorOptions> siteProcessorOptions,
    ILogger<DirectoryDataStore>? logger = null)
    : BaseDataStore(siteProcessorOptions, logger)
{
    private readonly ILogger<DirectoryDataStore> _logger = logger ?? NullLogger<DirectoryDataStore>.Instance;

    /// <summary>Gets the absolute base URI of the output directory, resolving relative paths against the current working directory.</summary>
    public override Uri BaseUri
    {
        get
        {
            Uri baseUri = base.BaseUri;
            if (baseUri.IsAbsoluteUri)
                return baseUri;

            return new Uri(Path.Combine(Directory.GetCurrentDirectory(), baseUri.ToString()));
        }
    }

    /// <summary>Opens a file for writing at the path corresponding to the specified URI, creating parent directories as needed.</summary>
    /// <param name="uri">The URI of the resource to write.</param>
    /// <returns>A <see cref="Stream"/> open for writing to the resolved file path.</returns>
    public override Stream GetStream(Uri uri)
    {
        Uri completeUri = GetCompleteUri(uri);
        EnsureDirectoryExists(completeUri);
        LogOpenFile(completeUri);
        return File.OpenWrite(Path.GetFullPath(Uri.UnescapeDataString(completeUri.LocalPath)));
    }

    private void EnsureDirectoryExists(Uri uri)
    {
        string path = Path.GetDirectoryName(uri.LocalPath) ?? throw new InvalidOperationException("Destionation is not a directory");
        path = Uri.UnescapeDataString(path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            LogDirectoryCreated(path);
        }
        else
            LogDirectoryExists(path);
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Directory {Directory} already exists")]
    private partial void LogDirectoryExists(string directory);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Directory {Directory} created")]
    private partial void LogDirectoryCreated(string directory);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Open file {File}")]
    private partial void LogOpenFile(Uri file);
}
