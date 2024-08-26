using System.Collections;
using System.Collections.Immutable;

namespace Sylvercode.DDBSrcModel.Extraction;

public readonly struct TaskIndex : IComparable<TaskIndex>
{
    private readonly ImmutableArray<int> _indexes;

    private TaskIndex(ImmutableArray<int> indexes)
        => _indexes = indexes;

    public TaskIndex(int index = 0) : this([index])
    { }

    public TaskIndex(TaskIndex? parent, int index) : this([.. parent?._indexes ?? [], index])
    { }

    public static implicit operator TaskIndex(int value) => new(value);

    public TaskIndex NewSubTaskIndex(int index)
        => new(this, index);

    public bool HasSubTaskIndex => _indexes.Length > 1;

    public TaskIndex? ParentTaskIndex()
        => HasSubTaskIndex ? new TaskIndex(_indexes[..^2]) : null;

    public TaskIndex Increment()
        => new(ParentTaskIndex(), _indexes[^1] + 1);

    public int CompareTo(TaskIndex other) =>
        ((IStructuralComparable)_indexes).CompareTo(other._indexes, StructuralComparisons.StructuralComparer);

    public override bool Equals(object? obj)
    {
        if (obj is not TaskIndex other)
            return false;

        return ((IStructuralEquatable)_indexes).Equals(other._indexes, StructuralComparisons.StructuralEqualityComparer);
    }

    public override int GetHashCode() =>
        ((IStructuralEquatable)_indexes).GetHashCode(StructuralComparisons.StructuralEqualityComparer);

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
}
