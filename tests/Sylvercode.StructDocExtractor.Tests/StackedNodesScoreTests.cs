using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.Tests;

public class StackedNodesScoreTests_CompareTo
{
    private static NodeScore NewNodeScore(int priority, int value)
        => new(new NodeScore.SubScore(priority, value));

    [Theory]
    [MemberData(nameof(LeftStackPrioritizedData))]
    public void LeftStackPrioritized_ReturnPositive(
        IDictionary<int, NodeScore> leftStack,
        IDictionary<int, NodeScore>? rightStack)
    {
        // Given
        StackedNodesScore left = new(leftStack);
        StackedNodesScore? right = (rightStack is not null) ? new(rightStack) : null;
        // When
        int result = left.CompareTo(right);

        // Then
        Assert.True(int.IsPositive(result));
    }

    public static IEnumerable<object?[]> LeftStackPrioritizedData()
    {
        yield return [
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 2) } },
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 1) } },
        ];

        yield return [
            new Dictionary<int, NodeScore> { { 2, NewNodeScore(1, 2) } },
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 1) } },
        ];

        yield return [
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) },
                { 2, NewNodeScore(1, 2) },
                { 4, NewNodeScore(1, 2) },
            },
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) },
                { 2, NewNodeScore(1, 2) },
                { 3, NewNodeScore(1, 2) },
            },
        ];

        yield return [
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) }
            },
            new Dictionary<int, NodeScore> {
            }
        ];

        yield return [
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) }
            },
            null
        ];
    }

    [Theory]
    [MemberData(nameof(RightStackPrioritizedData))]
    public void RightStackPrioritized_ReturnNegative(IDictionary<int, NodeScore> leftStack, IDictionary<int, NodeScore> rightStack)
    {
        // Given
        StackedNodesScore left = new(leftStack);
        StackedNodesScore? right = new(rightStack);
        // When
        int result = left.CompareTo(right);

        // Then
        Assert.True(int.IsNegative(result));
    }

    public static IEnumerable<object[]> RightStackPrioritizedData()
    {
        yield return [
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 1) } },
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 2) } },
        ];

        yield return [
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 1) } },
            new Dictionary<int, NodeScore> { { 2, NewNodeScore(1, 2) } },
        ];

        yield return [
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) },
                { 2, NewNodeScore(1, 2) },
                { 3, NewNodeScore(1, 2) }
            },
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) },
                { 2, NewNodeScore(1, 2) },
                { 4, NewNodeScore(1, 2) },
            },
        ];

        yield return [
            new Dictionary<int, NodeScore> {
            },
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) }
            }
        ];
    }

    [Theory]
    [MemberData(nameof(BothStackPrioritizedData))]
    public void BothStackPrioritized_ReturnZero(IDictionary<int, NodeScore> leftStack, IDictionary<int, NodeScore>? rightStack)
    {
        // Given
        StackedNodesScore left = new(leftStack);
        StackedNodesScore? right = (rightStack is not null) ? new(rightStack) : null;
        // When
        int result = left.CompareTo(right);

        // Then
        Assert.Equal(0, result);
    }

    public static IEnumerable<object?[]> BothStackPrioritizedData()
    {
        yield return [
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 2) } },
            new Dictionary<int, NodeScore> { { 1, NewNodeScore(1, 2) } }
        ];

        yield return [
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) },
                { 2, NewNodeScore(1, 2) },
                { 3, NewNodeScore(1, 2) }
            },
            new Dictionary<int, NodeScore> {
                { 1, NewNodeScore(1, 2) },
                { 2, NewNodeScore(1, 2) },
                { 3, NewNodeScore(1, 2) }
            }
        ];

        yield return [
            new Dictionary<int, NodeScore>() {
            },
            new Dictionary<int, NodeScore>() {
            }
        ];

        yield return [
            new Dictionary<int, NodeScore>() {
            },
            null
        ];
    }
}
