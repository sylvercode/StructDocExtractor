using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests.Fakes;

public class FakeStructDocData
{
    public FakeStructDocData? Parent { get; set; }
    public string Id { get; set; } = "";
    public string Data { get; set; } = "";
    public List<FakeStructDocData> Children { get; } = [];
    public TaskResultType ResultType { get; set; } = TaskResultType.Success;

    public ExtractionTask ToTask(bool AsProcessed = false)
    {
        ParentTaskInfo? parentTaskInfo = null;
        if (Parent is not null)
        {
            TaskIndex index = Parent.Children.IndexOf(this);
            parentTaskInfo = new ParentTaskInfo(Parent.ToTask(true), index);
        }
        ExtractionTask result = new(this, parentTaskInfo);

        if (AsProcessed)
        {
            BasicNodeDiscriminator discriminator = FakeStructDocDataDiscriminatorProvider.Default.CreateDataDiscriminator(this);
            IProcessTaskResult<FakeStructDocData, BasicNodeDiscriminator> TaskResult =
                BasicStructDocNodeFactoryProvider.Default.GetFactoryForStack(
                    new BasicNodeStructDataStack(discriminator))!.NewNode(this);
            result.SetResult(TaskResult, ChildrenTaskInfoFactory.Default);
        }

        return result;
    }
}
