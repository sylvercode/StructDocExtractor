using Sylvercode.StructDocExtractor.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.Extraction;

public class FactoryProviderStackByTask<TExtractionData, TDataDiscriminator>(
    ExtractionTask task,
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider)
    : IStructDocNodeFactoryProviderStack<TExtractionData, TDataDiscriminator>
{
    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultSrcNodeFactoryProvider
        => defaultNodeFactoryProvider;

    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetActiveSrcNodeFactoryProvider()
    {

        for (ExtractionTask? t = task; t is not null; t = t.ParentTaskInfo?.ParentTask)
        {
            if (t.TaskResult is null)
                continue;

            if (t.TaskResult.NodeFactoryProvider is not null)
                return (IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>)t.TaskResult.NodeFactoryProvider;
        }

        return defaultNodeFactoryProvider;
    }
}
