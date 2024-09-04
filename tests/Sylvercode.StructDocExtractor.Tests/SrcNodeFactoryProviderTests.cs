using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StructDataStack.Score;
using Sylvercode.StructDocExtractor.Tests.Mocks;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class SrcNodeFactoryProvider_GetFactoryForStack
{
    public class FakeFactory(string name) : IStructDocNodeFactory<string, BasicNodeDiscriminator>
    {
        public const string Factory1Name = nameof(Factory1Name);
        public const string Factory2Name = nameof(Factory2Name);

        public string Name => name;

        public static readonly FakeFactory FakeFactory1 = new(Factory1Name);
        public static readonly FakeFactory FakeFactory2 = new(Factory2Name);

        public IProcessTaskResult<string, BasicNodeDiscriminator> NewNode(string data)
        {
            throw new NotImplementedException();
        }
    }

    public static StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> NewProvider(StackedNodesScore? FactoryOneScore = null, StackedNodesScore? FactoryTwoScore = null)
    {
        StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> provider = new();
        provider.AddFactory(FakeFactory.FakeFactory1, new StackScoreCalculatorMock(FactoryOneScore ?? new()));
        provider.AddFactory(FakeFactory.FakeFactory2, new StackScoreCalculatorMock(FactoryTwoScore ?? new()));
        return provider;
    }

    public static BasicNodeStructDataStack StructDataStack => new();

    [Fact]
    public void WithNoMatch_ReturnNull()
    {
        // Given
        StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> provider = NewProvider();

        // When
        var result = provider.GetFactoryForStack(StructDataStack);

        // Then
        Assert.Null(result);
    }

    [Fact]
    public void WithFactoryOneMatch_ReturnFactoryOne()
    {
        // Given
        StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> provider = NewProvider(FactoryOneScore: new(1, new(new NodeScore.SubScore(1, 2))));

        // When
        var result = provider.GetFactoryForStack(StructDataStack);

        // Then
        Assert.Same(FakeFactory.FakeFactory1, result);
    }

    [Fact]
    public void WithFactoryTwoBestMatch_ReturnFactoryTwo()
    {
        // Given
        StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> provider = NewProvider(FactoryOneScore: new(1, new(new NodeScore.SubScore(1, 2))),
                                                                           FactoryTwoScore: new(0, new(new NodeScore.SubScore(1, 2))));

        // When
        var result = provider.GetFactoryForStack(StructDataStack);

        // Then
        Assert.Same(FakeFactory.FakeFactory2, result);
    }
}
