namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

/// <summary>Aggregated score across all entries in a discriminator stack, keyed by depth, used to rank candidate extraction paths against one another.</summary>
/// <remarks>Entries are compared from highest depth to lowest; the first differing depth-score pair determines the ordering.</remarks>
public class StackedNodesScore(IDictionary<int, NodeScore> stackedNodesScore) : IComparable<StackedNodesScore>
{
    private sealed class ReverseComparer : IComparer<int>
    {
        public int Compare(int x, int y) => y - x;
    }
    private static readonly ReverseComparer _reverseComparer = new();

    /// <summary>Initializes a new instance of <see cref="StackedNodesScore"/> with a single depth/score pair.</summary>
    /// <param name="depth">The stack depth this score corresponds to.</param>
    /// <param name="score">The node score at that depth.</param>
    public StackedNodesScore(int depth, NodeScore score) : this(new Dictionary<int, NodeScore>() { { depth, score } })
    {

    }

    /// <summary>Initializes a new empty <see cref="StackedNodesScore"/> representing no match.</summary>
    public StackedNodesScore() : this(new Dictionary<int, NodeScore>())
    {
    }

    private readonly SortedDictionary<int, NodeScore> _stackedNodesScore = new(stackedNodesScore, _reverseComparer);

    /// <summary>Gets whether this score contains no depth/score entries.</summary>
    public bool IsEmpty => _stackedNodesScore.Count == 0;

    /// <inheritdoc/>
    public int CompareTo(StackedNodesScore? other)
    {
        if (other is null)
            return IsEmpty ? 0 : 1;

        var criteriaIt = _stackedNodesScore.GetEnumerator();
        var otherIt = other._stackedNodesScore.GetEnumerator();
        while (criteriaIt.MoveNext() && otherIt.MoveNext())
        {
            var keyDiff = criteriaIt.Current.Key - otherIt.Current.Key;
            if (keyDiff != 0)
                return keyDiff;

            int scoreDiff = criteriaIt.Current.Value.CompareTo(otherIt.Current.Value);
            if (scoreDiff != 0)
                return scoreDiff;
        }

        return _stackedNodesScore.Count - other._stackedNodesScore.Count;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not StackedNodesScore other)
            return false;

        return CompareTo(other) == 0;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => _stackedNodesScore.GetHashCode();

    /// <summary>Returns <see langword="true"/> if both stacked scores are equal.</summary>
    public static bool operator ==(StackedNodesScore left, StackedNodesScore right)
        => left is null ? right is null : left.CompareTo(right) == 0;

    /// <summary>Returns <see langword="true"/> if the stacked scores differ.</summary>
    public static bool operator !=(StackedNodesScore left, StackedNodesScore right)
        => !(left == right);

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks lower than <paramref name="right"/>.</summary>
    public static bool operator <(StackedNodesScore left, StackedNodesScore right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks lower than or equal to <paramref name="right"/>.</summary>
    public static bool operator <=(StackedNodesScore left, StackedNodesScore right)
        => left is null || left.CompareTo(right) <= 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks higher than <paramref name="right"/>.</summary>
    public static bool operator >(StackedNodesScore left, StackedNodesScore right)
        => left is not null && left.CompareTo(right) > 0;

    /// <summary>Returns <see langword="true"/> if <paramref name="left"/> ranks higher than or equal to <paramref name="right"/>.</summary>
    public static bool operator >=(StackedNodesScore left, StackedNodesScore right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}
