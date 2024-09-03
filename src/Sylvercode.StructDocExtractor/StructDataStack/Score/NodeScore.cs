using System.Collections;
using System.Collections.Immutable;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class NodeScore(IEnumerable<NodeScore.SubScore> subScores) : IComparable<NodeScore>, IEnumerable<NodeScore.SubScore>
{
    public readonly struct SubScore(int priority, int value) : IComparable<SubScore>
    {
        public int Priority { get; } = priority;
        public int Value { get; } = value;

        public int CompareTo(SubScore other)
        {
            int priorityDiff = Priority - other.Priority;
            return priorityDiff != 0 ? priorityDiff
                                     : Value - other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SubScore other)
                return false;

            return other.Priority == Priority && other.Value == Value;
        }

        public override int GetHashCode()
            => HashCode.Combine(Priority, Value);

        public static bool operator ==(SubScore left, SubScore right)
            => left.Equals(right);

        public static bool operator !=(SubScore left, SubScore right)
            => !(left == right);

        public static bool operator <(SubScore left, SubScore right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(SubScore left, SubScore right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(SubScore left, SubScore right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(SubScore left, SubScore right)
            => left.CompareTo(right) >= 0;
    }

    private readonly ImmutableList<SubScore> m_SubScore = [.. subScores.DistinctBy(s => s.Priority).OrderByDescending(s => s.Priority)];

    public NodeScore(SubScore subScore) : this([subScore])
    {

    }

    public NodeScore() : this([])
    {

    }

    public int CompareTo(NodeScore? other)
    {
        if (other is null)
            return IsEmpty ? 0 : 1;

        var enumerator = GetEnumerator();
        var otherEnumerator = other.GetEnumerator();

        while (enumerator.MoveNext() && otherEnumerator.MoveNext())
        {
            int subScoreDiff = enumerator.Current.CompareTo(otherEnumerator.Current);
            if (subScoreDiff != 0)
                return subScoreDiff;
        }

        return m_SubScore.Count - other.m_SubScore.Count;
    }

    public IEnumerator<SubScore> GetEnumerator() => m_SubScore.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => m_SubScore.GetEnumerator();

    public bool IsEmpty => m_SubScore.IsEmpty || subScores.All(s => s.Value == 0);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;


        if (obj is not NodeScore other)
            return false;

        return m_SubScore.SequenceEqual(other.m_SubScore);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var subScore in m_SubScore)
        {
            hash = hash * 31 + subScore.GetHashCode();
        }

        return hash;
    }

    public static bool operator ==(NodeScore left, NodeScore right)
        => left.Equals(right);

    public static bool operator !=(NodeScore left, NodeScore right)
        => !(left == right);

    public static bool operator <(NodeScore left, NodeScore right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(NodeScore left, NodeScore right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(NodeScore left, NodeScore right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(NodeScore left, NodeScore right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}
