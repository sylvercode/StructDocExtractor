using Microsoft.Extensions.Logging;

namespace Sylvercode.DDBSrcModel.Extraction;

public struct BaseExtractorOption()
{
    public bool MissingNodeFactoryAsError = true;
    public bool ContinueOnException = false;

    public readonly LogLevel MissingNodeFactoryLogLevel => MissingNodeFactoryAsError ? LogLevel.Error : LogLevel.Debug;
    public readonly LogLevel ExceptionCatchLogLevel => ContinueOnException ? LogLevel.Error : LogLevel.Critical;
}
