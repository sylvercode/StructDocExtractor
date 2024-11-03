using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Markdown;

public partial class MarkdownReferencerUpdater(ILogger<MarkdownReferencerUpdater>? logger = null) : IReferencerUpdater
{
    private readonly ILogger<MarkdownReferencerUpdater> _logger = logger ?? NullLogger<MarkdownReferencerUpdater>.Instance;

    public void UpdateReferencers(Resource referencerResource,
                                  List<IStructDocReferencer> referencers,
                                  IReadOnlyResourceRepository resourceRepository)
    {
        Dictionary<Resource, string> uniqueFileNameRes = GetUniqueFileNameResource(resourceRepository);

        foreach (IStructDocReferencer referencer in referencers)
            UpdateReferencers(referencerResource, referencer, resourceRepository, uniqueFileNameRes);
    }

    private void UpdateReferencers(Resource referencerResource,
                                   IStructDocReferencer referencer,
                                   IReadOnlyResourceRepository resourceRepository,
                                   Dictionary<Resource, string> uniqueFileNameRes)
    {
        string refStr = referencer.GetReference();
        if (refStr.StartsWith('#'))
        {
            UriBuilder uriBuilder = new()
            {
                Scheme = LinkSerializer.WikiScheme,
                Host = string.Empty,
                Fragment = FragmentAsBlockReference(refStr),
                Path = string.Empty,
            };
            LogFragmentReferencer(uriBuilder.Uri);
            referencer.UpdateReference(uriBuilder.Uri.ToString());
            return;
        }

        Uri refUri = new(refStr);
        if (!resourceRepository.TryGetResourceForUri(refUri, out Resource? reference))
        {
            LogNotFoundReference(refUri);
            return;
        }

        if (!reference.State.IsPullable)
        {
            LogNotPullableReference(refUri);
            return;
        }

        if (reference == referencerResource)
        {
            UriBuilder uriBuilder = new()
            {
                Scheme = LinkSerializer.WikiScheme,
                Host = string.Empty,
                Path = string.Empty,
                Fragment = FragmentAsBlockReference(refUri.Fragment)
            };
            LogKeepFragmentOnly(uriBuilder.Uri);
            referencer.UpdateReference(uriBuilder.Uri.ToString());
            return;
        }

        if (uniqueFileNameRes.TryGetValue(reference, out string? uniqueFimeName))
        {
            UriBuilder uriBuilder = new()
            {
                Scheme = LinkSerializer.WikiScheme,
                Host = string.Empty,
                Path = uniqueFimeName,
                Fragment = FragmentAsBlockReference(refUri.Fragment)
            };
            LogUpdateToUniqueName(refUri, uriBuilder.Uri);
            referencer.UpdateReference(uriBuilder.Uri.ToString());
            return;
        }

        Uri translateUri = reference.TranslateUri(refUri);
        LogUpdateToNoneUniqueRes(refUri, translateUri);
        referencer.UpdateReference(translateUri.ToString());
    }

    private static string FragmentAsBlockReference(string fragment) => fragment.Replace("#", "#^");

    private static Dictionary<Resource, string> GetUniqueFileNameResource(IReadOnlyResourceRepository resourceRepository)
    {
        Dictionary<string, (int count, Resource resource)> fileNameMap = [];
        foreach (Resource resource in resourceRepository)
        {
            UriBuilder uriBuilder = new(resource.TranslatedResourceUri);
            string fileName = Path.GetFileName(uriBuilder.Path);
            if (fileNameMap.TryGetValue(fileName, out var value))
            {
                value.count++;
                fileNameMap[fileName] = value;
            }
            else
                fileNameMap[fileName] = (1, resource);
        }

        return fileNameMap.Where(pair => pair.Value.count == 1).Select((kv) => (kv.Value.resource, kv.Key)).ToDictionary();
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Referenced Uri not found in tracked resources: {RefUri}")]
    private partial void LogNotFoundReference(Uri refUri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Referenced Uri not pullable: {RefUri}")]
    private partial void LogNotPullableReference(Uri refUri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Update reference {RefUri} to unique file name: {UniqueFileName}")]
    private partial void LogUpdateToUniqueName(Uri refUri, Uri uniqueFileName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Update reference {RefUri} to no unique resource: {TranslateUri}")]
    private partial void LogUpdateToNoneUniqueRes(Uri refUri, Uri translateUri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Skip fragmentonly  referencer: {Referencer}")]
    private partial void LogFragmentReferencer(Uri referencer);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Updated to keep fragment only: {RefUri}")]
    private partial void LogKeepFragmentOnly(Uri refUri);
}
