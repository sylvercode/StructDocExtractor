using Microsoft.Extensions.Logging;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Configuration options that control extractor behaviour during an extraction pass.</summary>
public struct ExtractorOption()
{
    /// <summary>Gets or sets whether a missing node factory is treated as an error; defaults to <see langword="true"/>.</summary>
    public bool MissingNodeFactoryAsError { get; set; } = true;
    /// <summary>Gets or sets whether the extractor continues processing when an unhandled exception is caught; defaults to <see langword="false"/>.</summary>
    public bool ContinueOnException { get; set; } = false;

    /// <summary>Gets the log level corresponding to the missing-factory policy: <see cref="LogLevel.Error"/> when treated as an error, <see cref="LogLevel.Debug"/> otherwise.</summary>
    public readonly LogLevel MissingNodeFactoryLogLevel => MissingNodeFactoryAsError ? LogLevel.Error : LogLevel.Debug;
    /// <summary>Gets the log level for caught exceptions: <see cref="LogLevel.Critical"/> when <see cref="ContinueOnException"/> is <see langword="false"/>, <see cref="LogLevel.Error"/> otherwise.</summary>
    public readonly LogLevel ExceptionCatchLogLevel => ContinueOnException ? LogLevel.Error : LogLevel.Critical;
}
