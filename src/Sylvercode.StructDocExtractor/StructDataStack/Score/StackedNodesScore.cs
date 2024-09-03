namespace Sylvercode.StructDocExtractor.StructDataStack.Score;

public class StackedNodesScore(IDictionary<int, NodeScore> stackedNodesScore) : IComparable<StackedNodesScore>
{
    private sealed class ReverseComparer : IComparer<int>
    {
        public int Compare(int x, int y) => y - x;
    }
    private static readonly ReverseComparer _reverseComparer = new();

    public StackedNodesScore(int depth, NodeScore score) : this(new Dictionary<int, NodeScore>() { { depth, score } })
    {

    }

    public StackedNodesScore() : this(new Dictionary<int, NodeScore>())
    {
    }

    private readonly SortedDictionary<int, NodeScore> _stackedNodesScore = new(stackedNodesScore, _reverseComparer);

    public bool IsEmpty => _stackedNodesScore.Count == 0;

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
                return -keyDiff;

            int scoreDiff = criteriaIt.Current.Value.CompareTo(otherIt.Current.Value);
            if (scoreDiff != 0)
                return scoreDiff;
        }

        return _stackedNodesScore.Count - other._stackedNodesScore.Count;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not StackedNodesScore other)
            return false;

        return CompareTo(other) == 0;
    }

    public override int GetHashCode()
        => _stackedNodesScore.GetHashCode();

    public static bool operator ==(StackedNodesScore left, StackedNodesScore right)
        => left is null ? right is null : left.CompareTo(right) == 0;

    public static bool operator !=(StackedNodesScore left, StackedNodesScore right)
        => !(left == right);

    public static bool operator <(StackedNodesScore left, StackedNodesScore right)
        => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(StackedNodesScore left, StackedNodesScore right)
        => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(StackedNodesScore left, StackedNodesScore right)
        => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(StackedNodesScore left, StackedNodesScore right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}
