using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Fakes;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicStructDocBlockFactory : IStructDocNodeFactory<FakeStructDocData, BasicNodeDiscriminator>
{
    public IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> NewNode(BasicNodeDiscriminator discriminator, FakeStructDocData data)
    {
        BasicSrcBloc resultNode = new(data.Id);
        ProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> result = new(data.ResultType, resultNode)
        {
            DataDiscriminator = FakeStructDocDataDiscriminatorProvider.Default.CreateDataDiscriminator(data)
        };
        result.Metadatas.CopyMetadataFrom(data.Metadatas);
        result.SubTasksExtractionData.AddRange(data.Children);

        return result;
    }
}
