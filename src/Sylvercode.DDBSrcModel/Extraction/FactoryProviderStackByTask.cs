using Sylvercode.DDBSrcModel.Factory;

namespace Sylvercode.DDBSrcModel.Extraction;

public class FactoryProviderStackByTask<DataSelectable>(ExtractionTask task, ISrcNodeFactoryProvider<DataSelectable> defaultNodeFactoryProvider) : ISrcNodeFactoryProviderStack<DataSelectable>
{
    public ISrcNodeFactoryProvider<DataSelectable> DefaulSrcNodeFactoryProvider => defaultNodeFactoryProvider;

    public ISrcNodeFactoryProvider<DataSelectable> GetActiveSrcNodeFactoryProvider()
    {

        for (ExtractionTask? t = task; t is not null; t = t.ParentTaskInfo?.ParentTask)
        {
            if (t.TaskResult is null)
                continue;

            if (t.TaskResult.NodeFactoryProvider is not null)
                return (ISrcNodeFactoryProvider<DataSelectable>)t.TaskResult.NodeFactoryProvider;
        }

        return defaultNodeFactoryProvider;
    }
}
