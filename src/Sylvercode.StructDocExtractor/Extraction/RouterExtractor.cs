using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Sylvercode.StructDocExtractor.Extraction;

public class RouterExtractor<TExtractionData>(RouterExtractorList<TExtractionData> extractors, IOptions<RouterExtractor<TExtractionData>.RouterExtractorOptions> options) : IExtractor<TExtractionData>
{
    public class RouterExtractorOptions
    {
        public bool NoExtractorAsError { get; set; }
    }
    public RouterExtractorList<TExtractionData> Extractors { get; } = extractors;

    public ExtractionResult Extract([DisallowNull] TExtractionData data, IObserver<ExtractionTask>? observer = null)
    {
        var selectionResult = Extractors.Select(data);
        if (selectionResult is null)
            return new ExtractionResult(options.Value.NoExtractorAsError ? TaskResultType.Error : TaskResultType.Skipped);

        return selectionResult.Extractor.Extract(selectionResult.RootData, observer);
    }
}
