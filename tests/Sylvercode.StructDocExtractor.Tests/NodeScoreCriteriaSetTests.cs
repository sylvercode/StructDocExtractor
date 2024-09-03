using Sylvercode.StructDocExtractor.StructDataStack.Score;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class NodeScoreCriteriaSet_CalculateScore
{
    [Theory]
    [MemberData(nameof(NoCriterionSetSata))]
    public void NoCriteriaSet_ReturnEmpty(BasicNodeSelectable? node)
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable?> criteria = new();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.True(score.IsEmpty);
    }

    public static IEnumerable<object?[]> NoCriterionSetSata()
    {
        yield return [BasicNodeSelectable.NewDefault()];

        yield return [BasicNodeSelectable.NewWrongId()];

        yield return [null];
    }

    [Fact]
    public void DefaultIdOnlyCriteriaSet_ReturnIdPriorityScoreWithSameIdNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        BasicNodeSelectable node = BasicNodeSelectable.NewDefault();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        NodeScore.SubScore subScore = Assert.Single(score);
        Assert.Equal(BasicNodeScoreIdSubCriterion.ByDefaultId.Priority, subScore.Priority);
    }

    [Fact]
    public void DefaultIdOnlyCriteriaSet_ReturnEmptyScoreWithWrongIdNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        BasicNodeSelectable node = BasicNodeSelectable.NewWrongId();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.True(score.IsEmpty);
    }

    [Fact]
    public void DefaultIdAndDataCriteriaSet_ReturnEmptyScoreWithSameDataOnlyNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new([BasicNodeScoreIdSubCriterion.ByDefaultId, BasicNodeScoreDataSubCriterion.Instance]);
        BasicNodeSelectable node = BasicNodeSelectable.NewWrongId();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.True(score.IsEmpty);
    }

    [Fact]
    public void DefaultIdAndDataCriteriaSet_ReturnBothPriorityScoreWithDefaultNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new([BasicNodeScoreIdSubCriterion.ByDefaultId, BasicNodeScoreDataSubCriterion.Instance]);
        BasicNodeSelectable node = BasicNodeSelectable.NewDefault();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.Equal(2, score.Count());
        Assert.Equal(BasicNodeScoreIdSubCriterion.ByDefaultId.Priority, score.First().Priority);
        Assert.Equal(BasicNodeScoreDataSubCriterion.Instance.Priority, score.Last().Priority);
    }

    [Fact]
    public void DefaultIdAndDataSeparatedCriteriaSet_ReturnIdPriorityScoreWithDefaultNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new([[BasicNodeScoreIdSubCriterion.ByDefaultId], [BasicNodeScoreDataSubCriterion.Instance]]);
        BasicNodeSelectable node = BasicNodeSelectable.NewDefault();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        NodeScore.SubScore subScore = Assert.Single(score);
        Assert.Equal(BasicNodeScoreIdSubCriterion.ByDefaultId.Priority, subScore.Priority);
    }

    [Fact]
    public void DefaultIdAndDataSeparatedCriteriaSet_ReturnDataPriorityScoreWithWrongIdNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new([[BasicNodeScoreIdSubCriterion.ByDefaultId], [BasicNodeScoreDataSubCriterion.Instance]]);
        BasicNodeSelectable node = BasicNodeSelectable.NewWrongId();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        NodeScore.SubScore subScore = Assert.Single(score);
        Assert.Equal(BasicNodeScoreDataSubCriterion.Instance.Priority, subScore.Priority);
    }

    [Fact]
    public void DefaultIdAndDataSeparatedCriteriaSet_ReturnEmptyScoreWithWrongIdAndDataNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new([[BasicNodeScoreIdSubCriterion.ByDefaultId], [BasicNodeScoreDataSubCriterion.Instance]]);
        BasicNodeSelectable node = BasicNodeSelectable.NewWrongIdAndData();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.Empty(score);
    }

    [Fact]
    public void DefaultIdAndDataCriteriaSet_ReturnEmptyScoreWithWrongIdAndDataNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeSelectable> criteria = new([BasicNodeScoreIdSubCriterion.ByDefaultId, BasicNodeScoreDataSubCriterion.Instance]);
        BasicNodeSelectable node = BasicNodeSelectable.NewWrongIdAndData();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.Empty(score);
    }
}
