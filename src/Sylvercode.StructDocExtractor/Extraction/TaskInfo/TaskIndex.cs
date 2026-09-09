using System.Collections;
using System.Collections.Immutable;

namespace Sylvercode.StructDocExtractor.Extraction.TaskInfo;

/// <summary>Immutable hierarchical index that identifies a task's position within the extraction task tree using a sequence of integer segments (e.g., <c>0.1.2</c> for a third-level subtask).</summary>
/// <remarks>
/// Internally backed by an <see cref="ImmutableArray{T}">ImmutableArray&lt;int&gt;</see>, providing value equality,
/// structural comparison, and ordering semantics compatible with <see cref="IStructuralComparable"/> and
/// <see cref="IStructuralEquatable"/>. Composite indices are constructed via the <c>TaskIndex(TaskIndex?, TaskIndex)</c>
/// constructor; the <see cref="Increment"/> method advances the last segment to produce the next sibling index.
/// Root tasks use a single-segment index (e.g., <c>0</c>); sub-task indices carry additional segments appended by their parent.
/// </remarks>
public readonly struct TaskIndex : IComparable<TaskIndex>, IReadOnlyList<int>, IStructuralComparable, IStructuralEquatable, IEquatable<TaskIndex>
{
    private readonly ImmutableArray<int> _indexes;

    private TaskIndex(ImmutableArray<int> indexes)
        => _indexes = indexes;

    /// <summary>Initializes a new root <see cref="TaskIndex"/> at position zero (<c>0</c>).</summary>
    public TaskIndex() : this([0])
    { }

    /// <summary>Initializes a new single-segment <see cref="TaskIndex"/> at the specified position.</summary>
    /// <param name="index">The zero-based integer position of this task among its siblings.</param>
    public TaskIndex(int index) : this([index])
    { }

    /// <summary>Initializes a new <see cref="TaskIndex"/> as the first sub-task of <paramref name="parent"/> (segment <c>0</c>).</summary>
    /// <param name="parent">The parent index whose path is prepended.</param>
    public TaskIndex(TaskIndex parent) : this(parent, 0)
    { }

    /// <summary>Initializes a composite <see cref="TaskIndex"/> by concatenating <paramref name="parent"/>'s segments with <paramref name="task"/>'s segments.</summary>
    /// <param name="parent">The parent index segments; may be <see langword="null"/> to produce a root-relative index.</param>
    /// <param name="task">The child index segments to append.</param>
    public TaskIndex(TaskIndex? parent, TaskIndex task)
        : this([.. parent?._indexes ?? [], .. task._indexes])
    { }

    /// <summary>Implicitly converts an <see cref="int"/> to a single-segment <see cref="TaskIndex"/>.</summary>
    /// <param name="value">The integer position.</param>
    public static implicit operator TaskIndex(int value) => new(value);

    /// <summary>Returns a new sub-task <see cref="TaskIndex"/> rooted at this index with the specified sub-task position.</summary>
    /// <param name="index">The zero-based position of the sub-task; defaults to <c>0</c>.</param>
    /// <returns>A new composite <see cref="TaskIndex"/> one level deeper than this index.</returns>
    public TaskIndex NewSubTaskIndex(int index = 0)
        => new(this, index);

    /// <summary>Gets a value indicating whether this index has more than one segment, identifying it as a sub-task index.</summary>
    public bool IsSubTaskIndex => _indexes.Length > 1;

    /// <inheritdoc/>
    int IReadOnlyCollection<int>.Count => _indexes.Length;

    /// <summary>Gets the integer segment at <paramref name="index"/> within this hierarchical path.</summary>
    /// <param name="index">The zero-based position of the segment to retrieve.</param>
    public int this[int index] => _indexes[index];

    /// <summary>Returns the parent <see cref="TaskIndex"/> by removing the last segment.</summary>
    /// <returns>The parent index with one fewer segment.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this index is not a sub-task index (has only one segment).</exception>
    public TaskIndex ParentTaskIndex()
        => IsSubTaskIndex ? new TaskIndex(_indexes[..^1])
                          : throw new InvalidOperationException("Index is not as sub task index.");

    /// <summary>Returns a new <see cref="TaskIndex"/> with the last segment incremented by one, advancing to the next sibling position.</summary>
    /// <returns>The next sibling <see cref="TaskIndex"/>.</returns>
    public TaskIndex Increment()
        => new([.. _indexes[..^1], (_indexes[^1] + 1)]);

    /// <inheritdoc/>
    public int CompareTo(TaskIndex other) =>
        ((IStructuralComparable)_indexes).CompareTo(other._indexes, StructuralComparisons.StructuralComparer);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => (obj is TaskIndex other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        ((IStructuralEquatable)_indexes).GetHashCode(StructuralComparisons.StructuralEqualityComparer);

    /// <inheritdoc/>
    public IEnumerator<int> GetEnumerator()
        => ((IEnumerable<int>)_indexes).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_indexes).GetEnumerator();

    /// <inheritdoc/>
    public int CompareTo(object? other, IComparer comparer)
        => ((IStructuralComparable)_indexes).CompareTo(other, comparer);

    /// <inheritdoc/>
    public bool Equals(object? other, IEqualityComparer comparer)
        => ((IStructuralEquatable)_indexes).Equals(other, comparer);

    /// <inheritdoc/>
    public int GetHashCode(IEqualityComparer comparer)
        => ((IStructuralEquatable)_indexes).GetHashCode(comparer);

    /// <summary>Determines whether this index equals <paramref name="other"/> using structural equality of the underlying segments.</summary>
    /// <param name="other">The <see cref="TaskIndex"/> to compare to.</param>
    /// <returns><see langword="true"/> if both indices have identical segments; otherwise <see langword="false"/>.</returns>
    public bool Equals(TaskIndex other)
        => ((IStructuralEquatable)_indexes).Equals(other._indexes, StructuralComparisons.StructuralEqualityComparer);

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are structurally equal.</summary>
    public static bool operator ==(TaskIndex left, TaskIndex right)
        => left.Equals(right);

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not structurally equal.</summary>
    public static bool operator !=(TaskIndex left, TaskIndex right)
        => !(left == right);

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> is ordered before <paramref name="right"/>.</summary>
    public static bool operator <(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) < 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> is ordered before or equal to <paramref name="right"/>.</summary>
    public static bool operator <=(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) <= 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> is ordered after <paramref name="right"/>.</summary>
    public static bool operator >(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) > 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> is ordered after or equal to <paramref name="right"/>.</summary>
    public static bool operator >=(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) >= 0;

    /// <summary>Returns a dot-separated string representation of this index (e.g., <c>"0.1.2"</c>).</summary>
    /// <returns>A dot-separated string of the integer segments.</returns>
    public override string ToString()
        => string.Join(".", _indexes);
}
