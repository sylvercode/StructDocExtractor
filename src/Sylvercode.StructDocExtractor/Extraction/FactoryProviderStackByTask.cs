using Sylvercode.StructDocExtractor.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Builds the factory-provider stack by walking an <see cref="ExtractionTask"/> ancestor chain to find the nearest task-scoped provider override.</summary>
/// <typeparam name="TExtractionData">The type of source data element being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used for factory selection.</typeparam>
/// <param name="task">The current extraction task whose ancestor chain is searched for provider overrides.</param>
/// <param name="defaultNodeFactoryProvider">The fallback provider used when no ancestor task has set an override.</param>
public class FactoryProviderStackByTask<TExtractionData, TDataDiscriminator>(
    ExtractionTask task,
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider)
    : IStructDocNodeFactoryProviderStack<TExtractionData, TDataDiscriminator>
{
    /// <summary>Gets the default factory provider that serves as the fallback when no task-scoped override is found.</summary>
    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultSrcNodeFactoryProvider
        => defaultNodeFactoryProvider;

    /// <summary>Walks the task chain from the current task upward and returns the first scoped provider override found, or the default if none exists.</summary>
    /// <returns>The most-specific <see cref="IStructDocNodeFactoryProvider{TExtractionData, TDataDiscriminator}"/> active for this task.</returns>
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
