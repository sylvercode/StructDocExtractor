using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.StructDocStack.Score;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class SrcNodeFactoryProvider_GetFactoryForStack
{
    public class FakeFactory(string name) : ISrcNodeFactory<string, BasicNodeSelectable>
    {
        public const string Factory1Name = nameof(Factory1Name);
        public const string Factory2Name = nameof(Factory2Name);

        public string Name => name;

        public static readonly FakeFactory FakeFactory1 = new(Factory1Name);
        public static readonly FakeFactory FakeFactory2 = new(Factory2Name);

        public IProcessTaskResult<string, BasicNodeSelectable> NewNode(string data)
        {
            throw new NotImplementedException();
        }
    }

    public static SrcNodeFactoryProvider<string, BasicNodeSelectable> NewProvider(StackedNodesScore? FactoryOneScore = null, StackedNodesScore? FactoryTwoScore = null)
    {
        SrcNodeFactoryProvider<string, BasicNodeSelectable> provider = new();
        provider.AddFactory(FakeFactory.FakeFactory1, new StackScoreCalculatorMock(FactoryOneScore ?? new()));
        provider.AddFactory(FakeFactory.FakeFactory2, new StackScoreCalculatorMock(FactoryTwoScore ?? new()));
        return provider;
    }

    public static BasicNodeStructDataStack StructDataStack => new();

    [Fact]
    public void WithNoMatch_ReturnNull()
    {
        // Given
        SrcNodeFactoryProvider<string, BasicNodeSelectable> provider = NewProvider();

        // When
        var result = provider.GetFactoryForStack(StructDataStack);

        // Then
        Assert.Null(result);
    }

    [Fact]
    public void WithFactoryOneMatch_ReturnFactoryOne()
    {
        // Given
        SrcNodeFactoryProvider<string, BasicNodeSelectable> provider = NewProvider(FactoryOneScore: new(1, new(new NodeScore.SubScore(1, 2))));

        // When
        var result = provider.GetFactoryForStack(StructDataStack);

        // Then
        Assert.Same(FakeFactory.FakeFactory1, result);
    }

    [Fact]
    public void WithFactoryTwoBestMatch_ReturnFactoryTwo()
    {
        // Given
        SrcNodeFactoryProvider<string, BasicNodeSelectable> provider = NewProvider(FactoryOneScore: new(1, new(new NodeScore.SubScore(1, 2))),
                                                                           FactoryTwoScore: new(0, new(new NodeScore.SubScore(1, 2))));

        // When
        var result = provider.GetFactoryForStack(StructDataStack);

        // Then
        Assert.Same(FakeFactory.FakeFactory2, result);
    }
}
