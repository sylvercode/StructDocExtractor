using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Extraction;

public interface IExtractor<TExtractionData>
{
    ExtractionResult Extract([DisallowNull] TExtractionData data, IObserver<ExtractionTask>? observer = null);
}
