using System.Collections;
using System.Collections.Immutable;

namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Represents the computed score for a discriminator node against a <see cref="NodeScoreCriteriaSet{TDiscriminator}"/>, composed of prioritised sub-scores.</summary>
/// <remarks>Scores are compared lexicographically by descending priority, matching the BCL convention for compound keys. An empty score compares less than any non-empty score.</remarks>
public class NodeScore(IEnumerable<NodeScore.SubScore> subScores) : IComparable<NodeScore>, IEnumerable<NodeScore.SubScore>
{
    /// <summary>Represents a single weighted component of a <see cref="NodeScore"/>, pairing a priority level with a match count.</summary>
    public readonly struct SubScore(int priority, int value) : IComparable<SubScore>
    {
        /// <summary>Gets the relative importance of this sub-score; higher values are compared first.</summary>
        public int Priority { get; } = priority;

        /// <summary>Gets the match count produced by the corresponding criterion.</summary>
        public int Value { get; } = value;

        /// <inheritdoc/>
        public int CompareTo(SubScore other)
        {
            int priorityDiff = Priority - other.Priority;
            return priorityDiff != 0 ? priorityDiff
                                     : Value - other.Value;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not SubScore other)
                return false;

            return other.Priority == Priority && other.Value == Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => HashCode.Combine(Priority, Value);

        /// <summary>Returns <see langword="true"/> if both sub-scores have equal priority and value.</summary>
        public static bool operator ==(SubScore left, SubScore right)
            => left.Equals(right);

        /// <summary>Returns <see langword="true"/> if the sub-scores differ in priority or value.</summary>
        public static bool operator !=(SubScore left, SubScore right)
            => !(left == right);

        /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks lower than <paramref name="right"/>.</summary>
        public static bool operator <(SubScore left, SubScore right)
            => left.CompareTo(right) < 0;

        /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks lower than or equal to <paramref name="right"/>.</summary>
        public static bool operator <=(SubScore left, SubScore right)
            => left.CompareTo(right) <= 0;

        /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks higher than <paramref name="right"/>.</summary>
        public static bool operator >(SubScore left, SubScore right)
            => left.CompareTo(right) > 0;

        /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks higher than or equal to <paramref name="right"/>.</summary>
        public static bool operator >=(SubScore left, SubScore right)
            => left.CompareTo(right) >= 0;
    }

    private readonly ImmutableList<SubScore> m_SubScore = [.. subScores.DistinctBy(s => s.Priority).OrderByDescending(s => s.Priority)];

    /// <summary>Initializes a new instance of <see cref="NodeScore"/> with a single sub-score.</summary>
    /// <param name="subScore">The single sub-score component.</param>
    public NodeScore(SubScore subScore) : this([subScore])
    {

    }

    /// <summary>Initializes a new empty <see cref="NodeScore"/> representing no match.</summary>
    public NodeScore() : this([])
    {

    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public IEnumerator<SubScore> GetEnumerator() => m_SubScore.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => m_SubScore.GetEnumerator();

    /// <summary>Gets whether this score carries no sub-scores or all sub-scores have a value of zero.</summary>
    public bool IsEmpty => m_SubScore.IsEmpty || subScores.All(s => s.Value == 0);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;


        if (obj is not NodeScore other)
            return false;

        return m_SubScore.SequenceEqual(other.m_SubScore);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var subScore in m_SubScore)
        {
            hash = hash * 31 + subScore.GetHashCode();
        }

        return hash;
    }

    /// <summary>Returns <see langword="true"/> if both scores are equal.</summary>
    public static bool operator ==(NodeScore left, NodeScore right)
        => left.Equals(right);

    /// <summary>Returns <see langword="true"/> if the scores differ.</summary>
    public static bool operator !=(NodeScore left, NodeScore right)
        => !(left == right);

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks lower than <paramref name="right"/>.</summary>
    public static bool operator <(NodeScore left, NodeScore right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks lower than or equal to <paramref name="right"/>.</summary>
    public static bool operator <=(NodeScore left, NodeScore right)
        => left is null || left.CompareTo(right) <= 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks higher than <paramref name="right"/>.</summary>
    public static bool operator >(NodeScore left, NodeScore right)
        => left is not null && left.CompareTo(right) > 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks higher than or equal to <paramref name="right"/>.</summary>
    public static bool operator >=(NodeScore left, NodeScore right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}
