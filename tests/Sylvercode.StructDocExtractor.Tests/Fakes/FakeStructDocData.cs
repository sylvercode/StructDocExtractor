using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.TaskInfo;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests.Fakes;

public class FakeStructDocData(string? id = null, string? data = null)
{
    public static FakeStructDocDataBuilder New(string? id = null, string? data = null)
        => new(new FakeStructDocData(id, data));
    public FakeStructDocData? Parent { get; set; }
    public string Id { get; set; } = id ?? "";
    public string Data { get; set; } = data ?? "";
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
            result.SetResult(TaskResult);
        }

        return result;
    }
}
