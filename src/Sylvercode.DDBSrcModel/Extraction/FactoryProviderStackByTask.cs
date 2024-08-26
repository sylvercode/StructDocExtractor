using Sylvercode.DDBSrcModel.Extraction.Factory;

namespace Sylvercode.DDBSrcModel.Extraction;

public class FactoryProviderStackByTask<TExtractionData, TDataSelectable>(
    ExtractionTask task,
    ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> defaultNodeFactoryProvider)
    : ISrcNodeFactoryProviderStack<TExtractionData, TDataSelectable>
{
    public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> DefaulSrcNodeFactoryProvider
        => defaultNodeFactoryProvider;

    public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> GetActiveSrcNodeFactoryProvider()
    {

        for (ExtractionTask? t = task; t is not null; t = t.ParentTaskInfo?.ParentTask)
        {
            if (t.TaskResult is null)
                continue;

            if (t.TaskResult.NodeFactoryProvider is not null)
                return (ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>)t.TaskResult.NodeFactoryProvider;
        }

        return defaultNodeFactoryProvider;
    }
}
