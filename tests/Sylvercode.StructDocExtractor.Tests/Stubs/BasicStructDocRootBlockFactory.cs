using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Fakes;

namespace Sylvercode.StructDocExtractor.Tests.Stubs;

public class BasicStructDocRootBlockFactory : IStructDocNodeFactory<FakeStructDocData, BasicNodeDiscriminator>
{
    public IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> NewNode(FakeStructDocData data)
    {
        BasicSrcRootBlock resultNode = new(data.Id);
        ProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> result = new(data.ResultType, resultNode)
        {
            DataDiscriminator = FakeStructDocDataDiscriminatorProvider.Default.CreateDataDiscriminator(data)
        };
        result.SubTasksExtractionData.AddRange(data.Children);

        return result;
    }
}
