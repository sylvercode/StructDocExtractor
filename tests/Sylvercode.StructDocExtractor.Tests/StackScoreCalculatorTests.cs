using Sylvercode.StructDocExtractor.StructDataStack.Score;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class StackScoreCalculatorTests_Calculate
{
    [Fact]
    public void ForTwoStacklLevelCriteria_ReturnScoreForMatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        NodeScoreCriteriaSet<BasicNodeDiscriminator> parentCriteria = new(BasicNodeScoreIdSubCriterion.ByDefaultParentId);
        StackScoreCalculator<BasicNodeDiscriminator> stackScoreCalculator = new([criteria, parentCriteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeDiscriminator.NewDefaultParent());
        stack.Push(BasicNodeDiscriminator.NewDefault());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        StackedNodesScore Expected = new(
            new Dictionary<int, NodeScore> {
                { 0, new NodeScore(new NodeScore.SubScore(BasicNodeScoreIdSubCriterion.ByDefaultParentId.Priority, 1)) },
                { 1, new NodeScore(new NodeScore.SubScore(BasicNodeScoreIdSubCriterion.ByDefaultId.Priority, 1)) }
            }
        );
        Assert.Equal(0, Expected.CompareTo(result));
    }

    [Fact]
    public void ForTwoStacklLevelCriteria_ReturnEmptyForUnmatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        NodeScoreCriteriaSet<BasicNodeDiscriminator> parentCriteria = new(BasicNodeScoreIdSubCriterion.ByDefaultParentId);
        StackScoreCalculator<BasicNodeDiscriminator> stackScoreCalculator = new([criteria, parentCriteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeDiscriminator.NewDefaultParent());
        stack.Push(BasicNodeDiscriminator.NewWrongId());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void ForIdStackLevelCriteria_ReturnScoreForMatchingStack()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        StackScoreCalculator<BasicNodeDiscriminator> stackScoreCalculator = new([criteria]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeDiscriminator.NewDefaultParent());
        stack.Push(BasicNodeDiscriminator.NewDefault());


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
        StackScoreCalculator<BasicNodeDiscriminator> stackScoreCalculator = new([]);

        BasicNodeStructDataStack stack = new();
        stack.Push(BasicNodeDiscriminator.NewDefaultParent());
        stack.Push(BasicNodeDiscriminator.NewDefault());


        // When
        StackedNodesScore result = stackScoreCalculator.Calculate(stack);

        // Then
        Assert.True(result.IsEmpty);
    }
}
