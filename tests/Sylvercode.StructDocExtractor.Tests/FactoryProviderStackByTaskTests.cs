using Microsoft.VisualBasic;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class FactoryProviderStackByTaskTests_GetActiveSrcNodeFactoryProvider
{
    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public readonly SrcNodeFactoryProvider<string, BasicNodeDiscriminator> DefaultNodeFactoryProvider = new();
    public readonly SrcNodeFactoryProvider<string, BasicNodeDiscriminator> ANodeFactoryProvider1 = new();
    public readonly SrcNodeFactoryProvider<string, BasicNodeDiscriminator> ANodeFactoryProvider2 = new();

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.SetResult(taskResult, ChildrenTaskInfoFactory.Default);
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
        FactoryProviderStackByTask<string, BasicNodeDiscriminator> provider =
            new(NewTask(), DefaultNodeFactoryProvider);

        // When
        ISrcNodeFactoryProvider<string, BasicNodeDiscriminator> result = provider.GetActiveSrcNodeFactoryProvider();

        // Then
        Assert.Same(DefaultNodeFactoryProvider, result);
    }

    [Fact]
    public void TaskWithActiveProvider_ReturnsThatProvider()
    {
        // Given
        FactoryProviderStackByTask<string, BasicNodeDiscriminator> provider =
            new(NewTask(NewTaskResult(providerId: 1)), DefaultNodeFactoryProvider);


        // When
        ISrcNodeFactoryProvider<string, BasicNodeDiscriminator> result = provider.GetActiveSrcNodeFactoryProvider();

        // Then
        Assert.Same(ANodeFactoryProvider1, result);
    }

    [Fact]
    public void TaskWithActiveProviderInParent_ReturnsThatProvider()
    {
        // Given
        var parentTask = NewTask(NewTaskResult(providerId: 1, [DefaultTaskValue]));
        var childTask = parentTask.ChildrenTaskInfo!.ChildrenTasks[0];
        childTask.SetResult(NewTaskResult(), ChildrenTaskInfoFactory.Default);
        FactoryProviderStackByTask<string, BasicNodeDiscriminator> provider =
            new(childTask, DefaultNodeFactoryProvider);


        // When
        ISrcNodeFactoryProvider<string, BasicNodeDiscriminator> result = provider.GetActiveSrcNodeFactoryProvider();

        // Then
        Assert.Same(ANodeFactoryProvider1, result);
    }
}
