using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Markdown;

public partial class MarkdownReferencerUpdater(ILogger<MarkdownReferencerUpdater>? logger = null) : IReferencerUpdater
{
    private readonly ILogger<MarkdownReferencerUpdater> _logger = logger ?? NullLogger<MarkdownReferencerUpdater>.Instance;

    public void UpdateReferencers(Resource referencerResource,
                                  List<IStructDocReferencer> referencers,
                                  IReadOnlyDictionary<Uri, Resource> trackedResources)
    {
        Dictionary<Resource, string> uniqueFileNameRes = GetUniqueFileNameResource(trackedResources);

        foreach (IStructDocReferencer referencer in referencers)
            UpdateReferencers(referencerResource, referencer, trackedResources, uniqueFileNameRes);
    }

    private void UpdateReferencers(Resource referencerResource,
                                   IStructDocReferencer referencer,
                                   IReadOnlyDictionary<Uri, Resource> trackedResources,
                                   Dictionary<Resource, string> uniqueFileNameRes)
    {
        if (referencer.GetReference().StartsWith('#'))
        {
            UpdatelocalFragmentReferencer(referencerResource, referencer);
            return;
        }

        Uri refUri = new(referencer.GetReference());
        if (!trackedResources.TryGetValue(refUri, out Resource? reference))
        {
            LogNotFoundReference(refUri);
            return;
        }

        if (reference == referencerResource)
        {
            UpdatelocalFragmentReferencer(referencerResource, referencer);
            return;
        }

        if (uniqueFileNameRes.TryGetValue(reference, out string? uniqueFimeName))
        {
            string uniqueFimeNameWithFragment = uniqueFimeName + refUri.Fragment;
            LogUpdateToUniqueName(refUri, uniqueFimeNameWithFragment);
            referencer.UpdateReference(uniqueFimeNameWithFragment);
        }

        string translateUri = reference.TranslateUri(refUri).ToString();
        LogUpdateToNoneUniqueRes(refUri, translateUri);
        referencer.UpdateReference(translateUri);
    }

    private void UpdatelocalFragmentReferencer(Resource referencerResource, IStructDocReferencer referencers)
    {
        throw new NotImplementedException();
    }

    private static Dictionary<Resource, string> GetUniqueFileNameResource(IReadOnlyDictionary<Uri, Resource> trackedResources)
    {
        Dictionary<string, (int count, Resource resource)> fileNameMap = [];
        foreach (Resource resource in trackedResources.Values)
        {
            UriBuilder uriBuilder = new(resource.TranslatedResourceUri);
            string fileName = Path.GetFileName(uriBuilder.Path);
            if (fileNameMap.TryGetValue(fileName, out var value))
                value.count++;
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
        Message = "Update reference {RefUri} to unique file name: {UniqueFileName}")]
    private partial void LogUpdateToUniqueName(Uri refUri, string uniqueFileName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Update reference {RefUri} to no unique resource: {TranslateUri}")]
    private partial void LogUpdateToNoneUniqueRes(Uri refUri, string translateUri);
}
