using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Utils;
using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Extraction;

public abstract partial class BaseExtractor<TExtractionData, TDataSelectable> where TExtractionData : notnull
{
    public class TaskContext(ExtractionTask task, ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> defaultNodeFactoryProvider)
    {
        public TExtractionData ExtractionData => (TExtractionData)task.ExtractionData;

        public TaskIndex? TaskIndex => task.ParentTaskInfo?.SiblingSubTaskIndex;

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

        public IStructDataStack<TDataSelectable> GetStructDataStack(params TDataSelectable[] extraSelectableStack)
        {
            ICollection<TDataSelectable> result = [];

            foreach (var selectable in extraSelectableStack.Reverse())
                result.Add(selectable);

            for (ExtractionTask? t = task; t is not null; t = t?.ParentTaskInfo?.ParentTask)
            {
                if (t?.TaskResult is null)
                    continue;
                var eq = EqualityComparer<TDataSelectable>.Default;
                if (!eq.Equals((TDataSelectable?)t.TaskResult.DataSelectable, default))
                    result.Add((TDataSelectable)t.TaskResult.DataSelectable!);
            }
            return new BaseStructDataStack<TDataSelectable>(result.Reverse());
        }

        public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> GetNodeFactoryProvider()
            => new FactoryProviderStackByTask<TExtractionData, TDataSelectable>(task, defaultNodeFactoryProvider).GetActiveSrcNodeFactoryProvider();
    }
}
