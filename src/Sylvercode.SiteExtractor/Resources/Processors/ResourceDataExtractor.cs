using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Implementation of <see cref="IResourceDataExtractor{TExtractionData}"/> that fetches a resource from a site source, runs the structural extraction pipeline, and serializes the result to the data store in a two-phase process.</summary>
/// <remarks>
/// Processing is split into two phases: <see cref="Extract"/> fetches and extracts the document, returning a
/// <see cref="DataExtractedProcessorResult{TExtractionData}"/> (unfinished); <see cref="ContinueExtraction"/>
/// then updates URI references via <see cref="IReferencerUpdater"/> and serializes the node tree.
/// An optional <see cref="IResourceDependancyFilter"/> controls which discovered referencer nodes are tracked as
/// dependencies, and an optional <see cref="IUriRedirector"/> rewrites reference URIs before they are recorded.
/// </remarks>
/// <typeparam name="TExtractionData">The raw data type fetched from the site source and passed to the extractor.</typeparam>
public partial class ResourceDataExtractor<TExtractionData>(
    ISiteSource<TExtractionData> siteSource,
    IExtractor<TExtractionData> extractor,
    IDataStore dataStore,
    IStructDocSerializer serisalizer,
    IResourceUriTranslater? uriTranslater = null,
    IReferencerUpdater? referencerUpdater = null,
    IResourceDependancyFilter? resourceDependancyFilter = null,
    IUriRedirector? uriRedirector = null,
    ILogger<ResourceDataExtractor<TExtractionData>>? logger = null) : IResourceDataExtractor<TExtractionData>
{
    private sealed class ExtractionTaskObserver(
        IResourceDependancyFilter? resourceDependancyFilter,
        IUriRedirector? uriRedirector
    ) : IObserver<ExtractionTask>
    {
        public List<IStructDocReferencer> Referencers { get; } = [];

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(ExtractionTask value)
        {
            if (value.TaskResult?.SrcNode is not IStructDocReferencer referencer)
                return;

            if (uriRedirector?.RedirectUri(referencer.GetReference(), out string? redirectedUri) ?? false)
                referencer.UpdateReference(redirectedUri);

            if (resourceDependancyFilter?.IsAccepted(referencer) ?? true)
                Referencers.Add(referencer);
        }
    }

    private readonly ILogger<ResourceDataExtractor<TExtractionData>> _logger =
        logger ?? new NullLogger<ResourceDataExtractor<TExtractionData>>();

    private IResourceUriTranslater UriTransler { get; } =
        uriTranslater ?? ResourceUriTranslater.NewBaseTranslater(siteSource.BaseUri, dataStore.BaseUri);

    /// <inheritdoc/>
    public IResourceProcessorResult Extract(Resource resource, IReadOnlyResourceRepository resourceRepository)
    {
        using var scope = _logger.BeginScope((ResourceUri: resource.Uri, SiteUri: siteSource.BaseUri));
        TExtractionData extractionData = siteSource.GetData(resource.Uri);
        if (extractionData is null)
        {
            LogNoDataFromSource();
            return new FinishedProcessResult(this, resource);
        }

        ExtractionTaskObserver observer = new(resourceDependancyFilter, uriRedirector);
        ExtractionResult result = extractor.Extract(extractionData, observer);

        if (result.Metadatas.Count != 0)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                foreach (var metadata in result.Metadatas)
                {
                    string? metadataValue = metadata.Value.GetStrValue();
                    LogMetadatas(metadata.Key, metadataValue);
                }
            }
        }

        if (result.StructDocNodes.Count == 0)
        {
            LogNoNodeFromSource();
            return new FinishedProcessResult(this, resource);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            foreach (IStructDocReferencer referencer in observer.Referencers)
            {
                string reference = referencer.GetReference();
                LogReferencer(reference);
            }
        }

        return new DataExtractedProcessorResult<TExtractionData>(
            this,
            resource,
            resourceRepository,
            result,
            UriTransler,
            observer.Referencers,
            result.Metadatas);
    }

    /// <summary>Performs the second processing phase: updates URI references and serializes the extracted document tree to the data store.</summary>
    /// <param name="lastResult">The unfinished result from the first <see cref="Extract"/> call.</param>
    /// <returns>A finished <see cref="FinishedProcessResult"/> once serialization is complete.</returns>
    public IResourceProcessorResult ContinueExtraction(DataExtractedProcessorResult<TExtractionData> lastResult)
    {
        referencerUpdater?.UpdateReferencers(
            lastResult.Resource,
            lastResult.Referencers,
            lastResult.ResourceRepository);

        using TextWriter writer = GetTextWriter(
            lastResult.Resource.TranslateUri(),
            serisalizer.TextWriterProvider);

        serisalizer.Serialize(writer, lastResult.Result.StructDocNodes[0]);

        return new FinishedProcessResult(this, lastResult.Resource);
    }

    private TextWriter GetTextWriter(Uri uri, ITextWriterProvider? textWriterProvider)
    {
        if (textWriterProvider is null)
            return dataStore.GetStreamWriter(uri);

        return textWriterProvider.GetTextWriter(dataStore.GetStream(uri));
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No data found.")]
    private partial void LogNoDataFromSource();

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Trace,
        Message = "Metadata: {Key} = {Value}.")]
    private partial void LogMetadatas(string key, string? value);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No node found.")]
    private partial void LogNoNodeFromSource();

    [LoggerMessage(
        SkipEnabledCheck = true,
        Level = LogLevel.Debug,
        Message = "Referencer: {Referencer}.")]
    private partial void LogReferencer(string referencer);

    #region IResourceProcessor
    /// <inheritdoc/>
    public IResourceProcessorResult Process(Resource resource, IReadOnlyResourceRepository resourceRepository) => Extract(resource, resourceRepository);
    #endregion
}
