using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Fakes;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicStructDocNodeFactory : IStructDocNodeFactory<FakeStructDocData, BasicNodeDiscriminator>
{
    public IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> NewNode(FakeStructDocData data)
    {
        BasicSrcNode resultNode = new(data.Id, data.Data);
        ProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> result = new(data.ResultType, resultNode)
        {
            DataDiscriminator = FakeStructDocDataDiscriminatorProvider.Default.CreateDataDiscriminator(data)
        };
        result.Metadatas.CopyMetadataFrom(data.Metadatas);

        return result;
    }
}
