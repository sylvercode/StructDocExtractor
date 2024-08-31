using Microsoft.Extensions.Logging;

namespace Sylvercode.DDBSrcModel.Extraction;

public struct BaseExtractorOption()
{
    public bool MissingNodeFactoryAsError = true;
    public readonly LogLevel MissingNodeFactoryLogLevel => MissingNodeFactoryAsError ? LogLevel.Error : LogLevel.Debug;
}
