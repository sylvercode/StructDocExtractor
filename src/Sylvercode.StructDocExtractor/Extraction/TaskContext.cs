using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Utils;
using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction;

public class TaskContext<TExtractionData, TDataDiscriminator>(ExtractionTask task, IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider)
{
    public TExtractionData ExtractionData => (TExtractionData)task.ExtractionData;

    public TaskIndex? TaskIndex => task.ParentTaskInfo?.SiblingSubTaskIndex;

    public IStructDocNodeStack GetSrcNodeStack()
    {
        ICollection<IStructDocNode> result = [];
        for (ExtractionTask? t = task; t is not null; t = t?.ParentTaskInfo?.ParentTask)
        {
            IStructDocNode? node = t?.TaskResult?.SrcNode;
            if (node is not null)
                result.Add(node);
        }
        return new StructDocNodeStack(result.Reverse());
    }

    public IStructDataStack<TDataDiscriminator> GetStructDataStack(params TDataDiscriminator[] extraDiscriminatorStack)
    {
        ICollection<TDataDiscriminator> result = [];

        foreach (var discriminator in extraDiscriminatorStack.Reverse())
            result.Add(discriminator);

        for (ExtractionTask? t = task; t is not null; t = t?.ParentTaskInfo?.ParentTask)
        {
            if (t?.TaskResult is null)
                continue;
            var eq = EqualityComparer<TDataDiscriminator>.Default;
            if (!eq.Equals((TDataDiscriminator?)t.TaskResult.DataDiscriminator, default))
                result.Add((TDataDiscriminator)t.TaskResult.DataDiscriminator!);
        }
        return new BaseStructDataStack<TDataDiscriminator>(result.Reverse());
    }

    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetNodeFactoryProvider()
        => new FactoryProviderStackByTask<TExtractionData, TDataDiscriminator>(task, defaultNodeFactoryProvider).GetActiveSrcNodeFactoryProvider();
}
