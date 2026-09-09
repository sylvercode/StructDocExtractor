using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Implementation of <see cref="IExtractor{TExtractionData}"/> that routes each extraction request to the first matching sub-extractor in a prioritised <see cref="RouterExtractorList{TExtractionData}"/>.</summary>
/// <typeparam name="TExtractionData">The type of source data element to route and extract.</typeparam>
public class RouterExtractor<TExtractionData>(
    RouterExtractorList<TExtractionData> extractors,
    IOptions<RouterExtractor<TExtractionData>.RouterExtractorOptions> options) : IExtractor<TExtractionData>
{
    /// <summary>Configuration options for <see cref="RouterExtractor{TExtractionData}"/>.</summary>
    public class RouterExtractorOptions
    {
        /// <summary>Gets or sets whether an unmatched extraction attempt is treated as an error; defaults to <see langword="false"/>.</summary>
        public bool NoExtractorAsError { get; set; }
    }
    /// <summary>Gets the ordered list of extractor entries consulted when routing a source element.</summary>
    public RouterExtractorList<TExtractionData> Extractors { get; } = extractors;

    /// <inheritdoc/>
    public ExtractionResult Extract([DisallowNull] TExtractionData data, IObserver<ExtractionTask>? observer = null)
    {
        RouterExtractorList<TExtractionData>.SelectionResult? selectionResult = Extractors.Select(data);
        if (selectionResult is null)
            return new ExtractionResult(options.Value.NoExtractorAsError ? TaskResultType.Error : TaskResultType.Skipped);

        ExtractionResult result = selectionResult.Extractor.Extract(selectionResult.RootData, observer);
        result.Metadatas.CopyMetadataFrom(selectionResult.Metadatas, newOnly: true);
        return result;
    }
}
