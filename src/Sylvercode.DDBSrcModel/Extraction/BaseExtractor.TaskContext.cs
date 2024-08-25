using Sylvercode.DDBSrcModel.Factory;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Utils;
using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Extraction;

public abstract partial class BaseExtractor<ExtractionData, DataSelectable> where ExtractionData : notnull
{
    public class TaskContext(ExtractionTask task, ISrcNodeFactoryProvider<DataSelectable> defaultNodeFactoryProvider)
    {
        public ExtractionData ExtractionData => (ExtractionData)task.ExtractionData;

        public ISrcNodeStack GetSrcNodeStack()
        {
            ICollection<ISrcNode> result = [];
            for (ExtractionTask? t = task; t is not null; t = t?.ParentTaskInfo?.ParentTask)
            {
                ISrcNode? node = t?.TaskResult?.SrcNode;
                if (node is not null)
                    result.Add(node);
            }
            return new SrcNodeStack(result.Reverse());
        }

        public IStructDataStack<DataSelectable> GetStructDataStack()
        {
            ICollection<DataSelectable> result = [];
            for (ExtractionTask? t = task.ParentTaskInfo?.ParentTask; t is not null; t = t?.ParentTaskInfo?.ParentTask)
            {
                if (t?.TaskResult is null)
                    continue;

                if (t.TaskResult.DataSelectable is not null)
                    result.Add((DataSelectable)t.TaskResult.DataSelectable);
            }
            return new BaseStructDataStack<DataSelectable>(result.Reverse());
        }

        public ISrcNodeFactoryProviderStack<DataSelectable> GetNodeFactoryProvider()
            => new FactoryProviderStackByTask<DataSelectable>(task, defaultNodeFactoryProvider);
    }
}
