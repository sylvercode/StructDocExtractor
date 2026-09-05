using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Default implementation of <see cref="IReferencerUpdater"/> that rewrites each reference URI to its translated output path by looking it up in the resource repository.</summary>
/// <remarks>
/// For each <see cref="IStructDocReferencer"/> node, the updater resolves the resource from the repository
/// and calls <see cref="Resource.TranslateUri"/> to obtain the final output URI.  References that cannot
/// be resolved are logged at <see cref="LogLevel.Debug"/> and left unchanged.
/// </remarks>
public partial class ReferencerUpdater(ILogger<ReferencerUpdater>? logger = null) : IReferencerUpdater
{
    private readonly ILogger<ReferencerUpdater> _logger = logger ?? NullLogger<ReferencerUpdater>.Instance;

    /// <inheritdoc/>
    public void UpdateReferencers(Resource referencerResource,
                                  List<IStructDocReferencer> referencers,
                                  IReadOnlyResourceRepository resourceRepository)
    {
        foreach (var referencer in referencers)
        {
            Uri refUri = new(referencer.GetReference());
            if (resourceRepository.TryGetResourceForUri(refUri, out var resource))
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
