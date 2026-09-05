using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Processor result carrying the extracted document nodes, discovered referencer nodes, and resource repository context needed to complete serialization in a subsequent pipeline step.</summary>
/// <remarks>
/// This result is always unfinished (<see cref="BaseResourceProcessorResult.IsUnfinished"/> is <see langword="true"/>).
/// The pipeline calls <see cref="ContinueProcess"/> — which delegates to
/// <see cref="ResourceDataExtractor{TExtractionData}.ContinueExtraction"/> — to update URI references and serialize
/// the extracted nodes.  <see cref="GetResourceDependencies"/> returns the URIs of all referenced nodes so they can
/// be enqueued for further processing.
/// </remarks>
/// <typeparam name="TExtractionData">The raw data type provided by the site source during extraction.</typeparam>
public class DataExtractedProcessorResult<TExtractionData>(
    ResourceDataExtractor<TExtractionData> processor,
    Resource resource,
    IReadOnlyResourceRepository resourceRepository,
    ExtractionResult result,
    IResourceUriTranslater? resourceUriTranslaterToSet,
    List<IStructDocReferencer> referencers,
    MetadataDictionary newMetadatas)
    : BaseResourceProcessorResult(
        processor,
        resource,
        resourceUriTranslaterToSet,
        isUnfinished: true,
        newMetadatas)
{
    /// <summary>Gets the read-only resource repository used to resolve reference URIs during post-processing.</summary>
    public IReadOnlyResourceRepository ResourceRepository { get; } = resourceRepository;

    /// <summary>Gets the extraction result containing the structural document nodes produced from the resource.</summary>
    public ExtractionResult Result => result;

    /// <summary>Gets the list of referencer nodes whose URIs should be rewritten during post-processing.</summary>
    public List<IStructDocReferencer> Referencers { get; } = referencers;

    /// <inheritdoc/>
    public override IResourceProcessorResult ContinueProcess() 
        => processor.ContinueExtraction(this);

    /// <inheritdoc/>
    public override IEnumerable<Uri> GetResourceDependencies() 
        => Referencers.Select(r => new Uri(r.GetReference()));
}
