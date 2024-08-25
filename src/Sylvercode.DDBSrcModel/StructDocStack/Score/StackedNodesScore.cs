namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public class StackedNodesScore(IDictionary<int, NodeScore> starckedNodesScore) : IComparable<StackedNodesScore>
{
    private class ReverseComparer : IComparer<int>
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

    private readonly SortedDictionary<int, NodeScore> _starckedNodesScore = new(starckedNodesScore, _reverseComparer);

    public bool IsEmpty => _starckedNodesScore.Count == 0;

    public int CompareTo(StackedNodesScore? other)
    {
        if (other is null)
            return IsEmpty ? 0 : 1;

        var criteriaIt = _starckedNodesScore.GetEnumerator();
        var otherIt = other._starckedNodesScore.GetEnumerator();
        while (criteriaIt.MoveNext() && otherIt.MoveNext())
        {
            var keyDiff = criteriaIt.Current.Key - otherIt.Current.Key;
            if (keyDiff != 0)
                return -keyDiff;

            int scoreDiff = criteriaIt.Current.Value.CompareTo(otherIt.Current.Value);
            if (scoreDiff != 0)
                return scoreDiff;
        }

        return _starckedNodesScore.Count - other._starckedNodesScore.Count;
    }
}
