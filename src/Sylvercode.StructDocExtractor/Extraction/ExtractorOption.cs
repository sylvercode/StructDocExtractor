using Microsoft.Extensions.Logging;

namespace Sylvercode.StructDocExtractor.Extraction;

public struct ExtractorOption()
{
    public bool MissingNodeFactoryAsError { get; set; } = true;
    public bool ContinueOnException { get; set; } = false;

    public readonly LogLevel MissingNodeFactoryLogLevel => MissingNodeFactoryAsError ? LogLevel.Error : LogLevel.Debug;
    public readonly LogLevel ExceptionCatchLogLevel => ContinueOnException ? LogLevel.Error : LogLevel.Critical;
}
