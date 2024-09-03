using Sylvercode.StructDocExtractor.StructDataStack.Score;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class NodeScoreCriteriaSet_CalculateScore
{
    [Theory]
    [MemberData(nameof(NoCriterionSetSata))]
    public void NoCriteriaSet_ReturnEmpty(BasicNodeDiscriminator? node)
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator?> criteria = new();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.True(score.IsEmpty);
    }

    public static IEnumerable<object?[]> NoCriterionSetSata()
    {
        yield return [BasicNodeDiscriminator.NewDefault()];

        yield return [BasicNodeDiscriminator.NewWrongId()];

        yield return [null];
    }

    [Fact]
    public void DefaultIdOnlyCriteriaSet_ReturnIdPriorityScoreWithSameIdNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewDefault();

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
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new(BasicNodeScoreIdSubCriterion.ByDefaultId);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewWrongId();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.True(score.IsEmpty);
    }

    [Fact]
    public void DefaultIdAndDataCriteriaSet_ReturnEmptyScoreWithSameDataOnlyNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new([BasicNodeScoreIdSubCriterion.ByDefaultId, BasicNodeScoreDataSubCriterion.Instance]);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewWrongId();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.True(score.IsEmpty);
    }

    [Fact]
    public void DefaultIdAndDataCriteriaSet_ReturnBothPriorityScoreWithDefaultNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new([BasicNodeScoreIdSubCriterion.ByDefaultId, BasicNodeScoreDataSubCriterion.Instance]);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewDefault();

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
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new([[BasicNodeScoreIdSubCriterion.ByDefaultId], [BasicNodeScoreDataSubCriterion.Instance]]);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewDefault();

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
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new([[BasicNodeScoreIdSubCriterion.ByDefaultId], [BasicNodeScoreDataSubCriterion.Instance]]);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewWrongId();

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
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new([[BasicNodeScoreIdSubCriterion.ByDefaultId], [BasicNodeScoreDataSubCriterion.Instance]]);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewWrongIdAndData();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.Empty(score);
    }

    [Fact]
    public void DefaultIdAndDataCriteriaSet_ReturnEmptyScoreWithWrongIdAndDataNode()
    {
        // Given
        NodeScoreCriteriaSet<BasicNodeDiscriminator> criteria = new([BasicNodeScoreIdSubCriterion.ByDefaultId, BasicNodeScoreDataSubCriterion.Instance]);
        BasicNodeDiscriminator node = BasicNodeDiscriminator.NewWrongIdAndData();

        // When
        NodeScore score = criteria.CalculateScore(node);

        // Then
        Assert.Empty(score);
    }
}
