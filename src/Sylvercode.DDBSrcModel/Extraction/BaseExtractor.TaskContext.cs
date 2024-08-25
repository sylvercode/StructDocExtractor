using Sylvercode.DDBSrcModel.Factory;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Utils;
using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Extraction;

public abstract partial class BaseExtractor<ExtractionData, DataSelectable> where ExtractionData : notnull
{
    public class TaskContext(ExtractionTask tasks, ISrcNodeFactoryProvider<DataSelectable> defaultNodeFactoryProvider)
    {
        public ExtractionData ExtractionData => (ExtractionData)tasks.ExtractionData;

        public ISrcNodeStack GetSrcNodeStack()
        {
            ICollection<ISrcNode> result = [];
            for (ExtractionTask? task = tasks.ParentTaskInfo?.ParnetTask; task is not null; task = task?.ParentTaskInfo?.ParnetTask)
            {
                ISrcNode? node = task?.TaskResult?.SrcNode;
                if (node is not null)
                    result.Add(node);
            }
            return new SrcNodeStack(result.Reverse());
        }

        public IStructDataStack<DataSelectable> GetStructDataStack()
        {
            ICollection<DataSelectable> result = [];
            for (ExtractionTask? task = tasks.ParentTaskInfo?.ParnetTask; task is not null; task = task?.ParentTaskInfo?.ParnetTask)
            {
                if (task?.TaskResult is null)
                    continue;

                if (task.TaskResult.DataSelectable is not null)
                    result.Add((DataSelectable)task.TaskResult.DataSelectable);
            }
            return new BaseStructDataStack<DataSelectable>(result.Reverse());
        }

        public ISrcNodeFactoryProviderStack<DataSelectable> GetNodeFactoryProvider()
            => new FactoryProviderStackByTask<DataSelectable>(tasks, defaultNodeFactoryProvider);
    }
}
