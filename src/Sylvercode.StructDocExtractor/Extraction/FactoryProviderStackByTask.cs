using Sylvercode.StructDocExtractor.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.Extraction;

public class FactoryProviderStackByTask<TExtractionData, TDataDiscriminator>(
    ExtractionTask task,
    ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider)
    : ISrcNodeFactoryProviderStack<TExtractionData, TDataDiscriminator>
{
    public ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultSrcNodeFactoryProvider
        => defaultNodeFactoryProvider;

    public ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetActiveSrcNodeFactoryProvider()
    {

        for (ExtractionTask? t = task; t is not null; t = t.ParentTaskInfo?.ParentTask)
        {
            if (t.TaskResult is null)
                continue;

            if (t.TaskResult.NodeFactoryProvider is not null)
                return (ISrcNodeFactoryProvider<TExtractionData, TDataDiscriminator>)t.TaskResult.NodeFactoryProvider;
        }

        return defaultNodeFactoryProvider;
    }
}
