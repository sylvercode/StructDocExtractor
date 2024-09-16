using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Extraction;

public interface IExtractor<TExtractionData> : IObservable<ExtractionTask>
{
    ExtractionResult Extract([DisallowNull] TExtractionData data);
}
