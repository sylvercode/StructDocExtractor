using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests.Fakes;

public class FakeStructDocDataDiscriminatorProvider : IDataDiscriminatorFactory<FakeStructDocData, BasicNodeDiscriminator>
{
    public static FakeStructDocDataDiscriminatorProvider Default => new();

    public BasicNodeDiscriminator CreateDataDiscriminator(FakeStructDocData data)
        => new(data.Id, data.Children.Select(x => x.Id).ToArray());
}
