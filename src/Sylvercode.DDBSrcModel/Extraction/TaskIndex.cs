using System.Collections;
using System.Collections.Immutable;

namespace Sylvercode.DDBSrcModel.Extraction;

public readonly struct TaskIndex : IComparable<TaskIndex>, IReadOnlyList<int>, IStructuralComparable, IStructuralEquatable, IEquatable<TaskIndex>
{
    private readonly ImmutableArray<int> _indexes;

    private TaskIndex(ImmutableArray<int> indexes)
        => _indexes = indexes;

    public TaskIndex() : this([0])
    { }

    public TaskIndex(int index) : this([index])
    { }

    public TaskIndex(TaskIndex parent) : this(parent, 0)
    { }

    public TaskIndex(TaskIndex? parent, TaskIndex task)
        : this([.. parent?._indexes ?? [], .. task._indexes])
    { }

    public static implicit operator TaskIndex(int value) => new(value);

    public TaskIndex NewSubTaskIndex(int index = 0)
        => new(this, index);

    public bool IsSubTaskIndex => _indexes.Length > 1;

    int IReadOnlyCollection<int>.Count => _indexes.Length;

    public int this[int index] => _indexes[index];

    public TaskIndex ParentTaskIndex()
        => IsSubTaskIndex ? new TaskIndex(_indexes[..^1])
                          : throw new InvalidOperationException("Index is not as sub task index.");

    public TaskIndex Increment()
        => new([.. _indexes[..^1], (_indexes[^1] + 1)]);

    public int CompareTo(TaskIndex other) =>
        ((IStructuralComparable)_indexes).CompareTo(other._indexes, StructuralComparisons.StructuralComparer);

    public override bool Equals(object? obj)
        => (obj is TaskIndex other) && Equals(other);

    public override int GetHashCode() =>
        ((IStructuralEquatable)_indexes).GetHashCode(StructuralComparisons.StructuralEqualityComparer);

    public IEnumerator<int> GetEnumerator()
        => ((IEnumerable<int>)_indexes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_indexes).GetEnumerator();

    public int CompareTo(object? other, IComparer comparer)
        => ((IStructuralComparable)_indexes).CompareTo(other, comparer);

    public bool Equals(object? other, IEqualityComparer comparer)
        => ((IStructuralEquatable)_indexes).Equals(other, comparer);

    public int GetHashCode(IEqualityComparer comparer)
        => ((IStructuralEquatable)_indexes).GetHashCode(comparer);

    public bool Equals(TaskIndex other)
        => ((IStructuralEquatable)_indexes).Equals(other._indexes, StructuralComparisons.StructuralEqualityComparer);

    public static bool operator ==(TaskIndex left, TaskIndex right)
        => left.Equals(right);

    public static bool operator !=(TaskIndex left, TaskIndex right)
        => !(left == right);

    public static bool operator <(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(TaskIndex left, TaskIndex right)
        => left.CompareTo(right) >= 0;

    public override string ToString()
        => string.Join(".", _indexes);
}
