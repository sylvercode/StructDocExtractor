using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Tests.Fakes;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class ExtractorTaskSequencerHandlerTests_OnProcessTask
{
    public class MockTaskContext(ExtractionTask task)
        : TaskContext<FakeStructDocData, BasicNodeDiscriminator>(task, BasicStructDocNodeFactoryProvider.Default)
    {
    }
}
