using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.Tests.Fakes;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicStructDocNodeFactoryProvider : IStructDocNodeFactoryProvider<FakeStructDocData, BasicNodeDiscriminator>
{
    public static BasicStructDocNodeFactoryProvider Default { get; } = new();

    private readonly BasicStructDocRootBlockFactory _rootFactory = new();
    private readonly BasicStructDocBlockFactory _blockFactory = new();
    private readonly BasicStructDocNodeFactory _nodeFactory = new();

    public IStructDocNodeFactory<FakeStructDocData, BasicNodeDiscriminator>? GetFactoryForStack(IStructDataStack<BasicNodeDiscriminator> stackEntries)
    {
        if (stackEntries.Count == 1)
            return _rootFactory;

        if (stackEntries.Peek().NodeDiscriminator.Data.Length > 0)
            return _blockFactory;

        return _nodeFactory;
    }
}
