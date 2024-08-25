using System.Collections;
using System.Collections.Immutable;

namespace Sylvercode.DDBSrcModel.StructDocStack.Score;

public class NodeScore(IEnumerable<NodeScore.SubScore> subScores) : IComparable<NodeScore>, IEnumerable<NodeScore.SubScore>
{
    public readonly struct SubScore(int priority, int value) : IComparable<SubScore>
    {
        public readonly int Priority = priority;
        public readonly int Value = value;

        public int CompareTo(SubScore other)
        {
            int priorityDiff = Priority - other.Priority;
            return priorityDiff != 0 ? priorityDiff
                                     : Value - other.Value;
        }
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
}
