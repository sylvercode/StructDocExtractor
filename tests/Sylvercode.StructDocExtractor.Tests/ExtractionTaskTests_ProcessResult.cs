using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public partial class ExtractionTaskTests_ProcessResult
{
    public class EventLogger
    {
        public bool EventRaised { get; private set; } = false;

        public void OnEventRaised(object? task, EventArgs args)
        {
            EventRaised = true;
        }
    }

    public const string TaskData = nameof(TaskData);
    public readonly BasicNodeDiscriminator DataDiscriminatorValue = new();
    public readonly StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> NodeFactoryProviderValue = new();

    [Fact]
    public void WithNoChildResult_ResultIsSetAnEventSend()
    {
        // Given
        ExtractionTask task = new(TaskData);
        EventLogger eventLogger = new();
        task.ResultSet += eventLogger.OnEventRaised;
        BasicProcessTaskResult processResult = new(new BasicSrcNode())
        {
            DataDiscriminator = DataDiscriminatorValue,
            NodeFactoryProvider = NodeFactoryProviderValue
        };

        // When
        task.SetResult(processResult, ChildrenTaskInfoFactory.Default);

        // Then
        Assert.NotNull(task.TaskResult);
        Assert.NotNull(task.TaskResult.SrcNode);
        Assert.Equal(BasicSrcNode.DefaultId, task.TaskResult.SrcNode.Id);
        Assert.Equal(task.TaskResult.DataDiscriminator, DataDiscriminatorValue);
        Assert.Same(task.TaskResult.NodeFactoryProvider, NodeFactoryProviderValue);
        Assert.NotNull(task.ChildrenTaskInfo);
        Assert.Empty(task.ChildrenTaskInfo.ChildrenTasks);
        Assert.True(eventLogger.EventRaised);
    }

    [Fact]
    public void WithChildResult_DataIsAddedToInfo()
    {
        // Given
        ExtractionTask task = new(TaskData);
        BasicProcessTaskResult processResult = new();
        processResult.SubTasksExtractionData.Add("Data1");
        processResult.SubTasksExtractionData.Add("Data2");

        // When
        task.SetResult(processResult, ChildrenTaskInfoFactory.Default);

        // Then
        Assert.NotNull(task.ChildrenTaskInfo);
        Assert.Equal([new("Data1"), new("Data2")], task.ChildrenTaskInfo.ChildrenTasks,
                     (ExtractionTask e, ExtractionTask a) => e.ExtractionData.Equals(a.ExtractionData));
    }

}
