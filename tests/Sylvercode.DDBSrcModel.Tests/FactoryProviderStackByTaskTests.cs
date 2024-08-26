using Microsoft.VisualBasic;
using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Tests.Extraction;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class FactoryProviderStackByTaskTests_GetActiveSrcNodeFactoryProvider
{
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public readonly SrcNodeFactoryProvider<string, BasicNodeSelectable> DefaultNodeFactoryProvider = new();
    public readonly SrcNodeFactoryProvider<string, BasicNodeSelectable> ANodeFactoryProvider1 = new();
    public readonly SrcNodeFactoryProvider<string, BasicNodeSelectable> ANodeFactoryProvider2 = new();

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.ProcessResult(taskResult);
        return result;
    }

    public BasicProcessTaskResult NewTaskResult(int? providerId = null, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new();
        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        switch (providerId)
        {
            case 1:
                taskResult.NodeFactoryProvider = ANodeFactoryProvider1;
                break;
            case 2:
                taskResult.NodeFactoryProvider = ANodeFactoryProvider2;
                break;
        }
        return taskResult;
    }

    [Fact]
    public void NoActiveProvider_ReturnsDefault()
    {
        // Given
        FactoryProviderStackByTask<string, BasicNodeSelectable> provider =
            new(NewTask(), DefaultNodeFactoryProvider);

        // When
        ISrcNodeFactoryProvider<BasicNodeSelectable> result = provider.GetActiveSrcNodeFactoryProvider();

        // Then
        Assert.Same(DefaultNodeFactoryProvider, result);
    }

    [Fact]
    public void TaskWithActiveProvider_ReturnsThatProvider()
    {
        // Given
        FactoryProviderStackByTask<string, BasicNodeSelectable> provider =
            new(NewTask(NewTaskResult(providerId: 1)), DefaultNodeFactoryProvider);


        // When
        ISrcNodeFactoryProvider<BasicNodeSelectable> result = provider.GetActiveSrcNodeFactoryProvider();

        // Then
        Assert.Same(ANodeFactoryProvider1, result);
    }

    [Fact]
    public void TaskWithActiveProviderInParent_ReturnsThatProvider()
    {
        // Given
        var parentTask = NewTask(NewTaskResult(providerId: 1, [DefaultTaskValue]));
        var childTask = parentTask.ChildrenTaskInfo!.ChildrenTasks[0];
        childTask.ProcessResult(NewTaskResult());
        FactoryProviderStackByTask<string, BasicNodeSelectable> provider =
            new(childTask, DefaultNodeFactoryProvider);


        // When
        ISrcNodeFactoryProvider<BasicNodeSelectable> result = provider.GetActiveSrcNodeFactoryProvider();

        // Then
        Assert.Same(ANodeFactoryProvider1, result);
    }
}
