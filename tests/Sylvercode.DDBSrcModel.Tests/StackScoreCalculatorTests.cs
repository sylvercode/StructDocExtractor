using Sylvercode.DDBSrcModel.StructDocStack.Score;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class StackScoreCalculatorTests_Calculate
{
    [Fact]
    public void ForTwoStacklLevelCriteria_ReturnScoreForMatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        NodeScoreCriteriaSet<BasicNodeSelectable> parentCriteria = new(BasicNodeScoreIdSubCriterion.ByDefaultParentId);
        StackScoreCalculator<BasicNodeSelectable> stackScoreCalculator = new([criteria, parentCriteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeSelectable.NewDefaultParent());
        stack.Push(BasicNodeSelectable.NewDefault());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        StackedNodesScore Expected = new(
            new Dictionary<int, NodeScore>() {
                { 0, new NodeScore(new NodeScore.SubScore(BasicNodeScoreIdSubCriterion.ByDefaultParentId.Priority, 1)) },
                { 1, new NodeScore(new NodeScore.SubScore(BasicNodeScoreIdSubCriterion.ByDefaultId.Priority, 1)) }
            }
        );
        Assert.Equal(0, Expected.CompareTo(result));
    }

    [Fact]
    public void ForParentStackLevelCriteria_ReturnScoreForMatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> parentCriteria = new(BasicNodeScoreIdSubCriterion.ByDefaultParentId);
        StackScoreCalculator<BasicNodeSelectable> stackScoreCalculator = new([parentCriteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeSelectable.NewDefaultParent());
        stack.Push(BasicNodeSelectable.NewDefault());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        StackedNodesScore Expected = new(
            new Dictionary<int, NodeScore>() {
                { 0, new NodeScore(new NodeScore.SubScore(BasicNodeScoreIdSubCriterion.ByDefaultParentId.Priority, 1)) }
            }
        );
        Assert.Equal(0, Expected.CompareTo(result));
    }

    [Fact]
    public void ForTwoStacklLevelCriteria_ReturnEmptyForUnmatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        NodeScoreCriteriaSet<BasicNodeSelectable> parentCriteria = new(BasicNodeScoreIdSubCriterion.ByDefaultParentId);
        StackScoreCalculator<BasicNodeSelectable> stackScoreCalculator = new([criteria, parentCriteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeSelectable.NewDefaultParent());
        stack.Push(BasicNodeSelectable.NewWrongId());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void ForIdStackLevelCriteria_ReturnScoreForMatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        StackScoreCalculator<BasicNodeSelectable> stackScoreCalculator = new([criteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeSelectable.NewDefaultParent());
        stack.Push(BasicNodeSelectable.NewDefault());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        StackedNodesScore Expected = new(
            new Dictionary<int, NodeScore>() {
                { 1, new NodeScore(new NodeScore.SubScore(BasicNodeScoreIdSubCriterion.ByDefaultId.Priority, 1)) }
            }
        );
        Assert.Equal(0, Expected.CompareTo(result));
    }

    [Fact]
    public void ForEmptyStackLevelCriteria_ReturnEmpty()
    {
        // Given
        StackScoreCalculator<BasicNodeSelectable> stackScoreCalculator = new([]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeSelectable.NewDefaultParent());
        stack.Push(BasicNodeSelectable.NewDefault());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        Assert.True(result.IsEmpty);
    }
}
