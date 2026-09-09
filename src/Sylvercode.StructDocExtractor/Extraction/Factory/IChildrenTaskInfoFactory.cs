using Sylvercode.StructDocExtractor.Extraction.TaskInfo;

namespace Sylvercode.StructDocExtractor.Extraction.Factory;

/// <summary>Defines the contract for creating an <see cref="IChildrenTaskInfo"/> that manages child extraction tasks spawned from a completed parent task.</summary>
public interface IChildrenTaskInfoFactory
{
    /// <summary>Creates a new children-task descriptor for <paramref name="task"/> using the supplied child source data.</summary>
    /// <param name="task">The completed extraction task whose result spawns the child tasks.</param>
    /// <param name="childrenData">The collection of child source data items to dispatch; may be <see langword="null"/> for zero children.</param>
    /// <returns>An <see cref="IChildrenTaskInfo"/> that tracks the resulting child extraction tasks.</returns>
    IChildrenTaskInfo NewChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData);
}
