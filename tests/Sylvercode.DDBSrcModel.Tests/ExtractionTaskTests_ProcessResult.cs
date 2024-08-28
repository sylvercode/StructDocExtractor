using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Tests.Extraction;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public partial class ExtractionTaskTests_ProcessResult
{
    public class EventLogger
    {
        public bool EventRaised { get; private set; } = false;

        public void OnEventRaised(ExtractionTask task)
        {
            EventRaised = true;
        }
    }

    public const string TaskData = nameof(TaskData);
    public readonly BasicNodeSelectable DataSelectableValue = new();
    public readonly SrcNodeFactoryProvider<string, BasicNodeSelectable> NodeFactoryProviderValue = new();

    [Fact]
    public void WithNoChildResult_ResulIsSetAnEventSend()
    {
        // Given
        ExtractionTask task = new(TaskData);
        EventLogger eventLogger = new();
        task.ResultSetted += eventLogger.OnEventRaised;
        BasicProcessTaskResult processResult = new(new BasicSrcNode())
        {
            DataSelectable = DataSelectableValue,
            NodeFactoryProvider = NodeFactoryProviderValue
        };

        // When
        task.ProcessResult(processResult, ChildrenTaskInfoFactory.Default);

        // Then
        Assert.NotNull(task.TaskResult);
        Assert.NotNull(task.TaskResult.SrcNode);
        Assert.Equal(BasicSrcNode.DefaultId, task.TaskResult.SrcNode.Id);
        Assert.Equal(task.TaskResult.DataSelectable, DataSelectableValue);
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
        task.ProcessResult(processResult, ChildrenTaskInfoFactory.Default);

        // Then
        Assert.NotNull(task.ChildrenTaskInfo);
        Assert.Equal([new("Data1"), new("Data2")], task.ChildrenTaskInfo.ChildrenTasks,
                     (ExtractionTask e, ExtractionTask a) => e.ExtractionData.Equals(a.ExtractionData));
    }

}
