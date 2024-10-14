using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

public partial class ReferencerUpdater(ILogger<ReferencerUpdater>? logger = null) : IReferencerUpdater
{
    private readonly ILogger<ReferencerUpdater> _logger = logger ?? NullLogger<ReferencerUpdater>.Instance;

    public void UpdateReferencers(Resource referencerResource,
                                  List<IStructDocReferencer> referencers,
                                  IReadOnlyDictionary<Uri, Resource> trackedResources)
    {
        foreach (var referencer in referencers)
        {
            Uri refUri = new(referencer.GetReference());
            if (trackedResources.TryGetValue(refUri, out var resource))
                referencer.UpdateReference(resource.TranslateUri(refUri).ToString());
            else
                LogNotFoundReference(refUri);
        }
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Referenced Uri not found in tracked resources: {RefUri}")]
    private partial void LogNotFoundReference(Uri refUri);
}
