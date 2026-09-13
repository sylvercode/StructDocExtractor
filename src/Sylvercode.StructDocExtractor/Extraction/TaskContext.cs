using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Utils;
using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Typed context wrapping an <see cref="ExtractionTask"/> to provide strongly-typed access to extraction data, node stacks, and the resolved factory provider.</summary>
/// <typeparam name="TExtractionData">The type of source data element being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used to select node factories.</typeparam>
/// <param name="task">The extraction task this context wraps.</param>
/// <param name="defaultNodeFactoryProvider">The default factory provider used when no task-scoped override exists.</param>
public class TaskContext<TExtractionData, TDataDiscriminator>(
    ExtractionTask task,
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider)
{
    /// <summary>Gets the strongly-typed source data for this task.</summary>
    public TExtractionData ExtractionData => (TExtractionData)task.ExtractionData;

    /// <summary>Gets the positional index of this task among its siblings, or <see langword="null"/> for root tasks.</summary>
    public TaskIndex? TaskIndex => task.ParentTaskInfo?.SiblingSubTaskIndex;

    /// <summary>Builds and returns a stack of structural nodes traversed from this task up to the root.</summary>
    /// <returns>A stack of <see cref="IStructDocNode"/> instances ordered from root to current, excluding null results.</returns>
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

    /// <summary>Builds and returns the data-discriminator stack for the current task chain, with optional extra entries prepended.</summary>
    /// <param name="extraDiscriminatorStack">Additional discriminator values to prepend to the top of the stack (e.g., the current task's own discriminator).</param>
    /// <returns>A <see cref="IStructDataStack{TDataDiscriminator}"/> ordered from root to current task.</returns>
    public IStructDataStack<TDataDiscriminator> GetStructDataStack(params IEnumerable<TDataDiscriminator> extraDiscriminatorStack)
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

    /// <summary>Resolves and returns the factory provider in scope for this task, preferring the nearest ancestor task's override over the default.</summary>
    /// <returns>The most-specific <see cref="IStructDocNodeFactoryProvider{TExtractionData, TDataDiscriminator}"/> active for this task.</returns>
    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> GetNodeFactoryProvider()
        => new FactoryProviderStackByTask<TExtractionData, TDataDiscriminator>(task, defaultNodeFactoryProvider).GetActiveSrcNodeFactoryProvider();
}
