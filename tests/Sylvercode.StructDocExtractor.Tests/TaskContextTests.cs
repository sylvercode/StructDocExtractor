using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Utils;
using Sylvercode.StructDocExtractor.StructDataStack;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class TaskContextTests_GetSrcNodeStack
{
    public readonly StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> DefaultNodeFactoryProvider = new();

    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.SetResult(taskResult, ChildrenTaskInfoFactory.Default);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(IStructDocNode node, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new(node);
        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        return taskResult;
    }

    [Fact]
    public void WithNoNode_ReturnsEmpty()
    {
        // Given
        ExtractionTask task = NewTask();

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        IStructDocNodeStack result = context.GetSrcNodeStack();

        // Then
        Assert.Empty(result);
    }

    [Fact]
    public void WithOneNode_ReturnsSingleNode()
    {
        // Given
        var node = new BasicSrcNode();
        ExtractionTask task = NewTask(NewTaskResult(node));

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        IStructDocNodeStack result = context.GetSrcNodeStack();

        // Then
        Assert.Single(result, new IStructDocNodeStack.Entry(0, node));
    }

    [Fact]
    public void WithTwoNodes_ReturnsTwoNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, [DefaultTaskValue1]));
        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        childTask.SetResult(NewTaskResult(childNode), ChildrenTaskInfoFactory.Default);

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        // When
        IStructDocNodeStack result = context.GetSrcNodeStack();

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDocNodeStack.Entry(1, childNode), e),
                          e => Assert.Equal(new IStructDocNodeStack.Entry(0, parentNode), e));
    }
}

public class TaskContextTests_GetStructDataStack
{
    public readonly StructDocNodeFactoryProvider<string, BasicNodeDiscriminator> DefaultNodeFactoryProvider = new();

    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);
    public const string DefaultDiscriminatorValue1 = nameof(DefaultDiscriminatorValue1);
    public const string DefaultDiscriminatorValue2 = nameof(DefaultDiscriminatorValue2);
    public const string DefaultDiscriminatorValue3 = nameof(DefaultDiscriminatorValue3);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.SetResult(taskResult, ChildrenTaskInfoFactory.Default);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(IStructDocNode node, BasicNodeDiscriminator? discriminator, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new(node)
        {
            DataDiscriminator = discriminator ?? default
        };

        if (childData is not null)
            taskResult.SubTasksExtractionData.AddRange(childData);
        return taskResult;
    }

    [Fact]
    public void WithNoNode_ReturnsEmpty()
    {
        // Given
        ExtractionTask task = NewTask();

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeDiscriminator> result = context.GetStructDataStack();

        // Then
        Assert.Empty(result);
    }

    [Fact]
    public void WithOneNode_ReturnsSingleNode()
    {
        // Given
        BasicSrcNode node = new();
        BasicNodeDiscriminator discriminator = new(BasicSrcNode.DefaultId);
        ExtractionTask task = NewTask(NewTaskResult(node, discriminator));

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeDiscriminator> result = context.GetStructDataStack();

        // Then
        Assert.Single(result, new IStructDataStack<BasicNodeDiscriminator>.Entry(0, discriminator));
    }

    [Fact]
    public void WithTwoNodes_ReturnsTwoNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        BasicNodeDiscriminator parentDiscriminator = new(BasicSrcNode.DefaultId);
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, parentDiscriminator, [DefaultTaskValue1]));

        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        BasicNodeDiscriminator childDiscriminator = new(DefaultDiscriminatorValue1);
        childTask.SetResult(NewTaskResult(childNode, childDiscriminator), ChildrenTaskInfoFactory.Default);

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeDiscriminator> result = context.GetStructDataStack();

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(1, childDiscriminator), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(0, parentDiscriminator), e));
    }

    [Fact]
    public void WithTwoNodesAndExtraNoDiscriminatorInMiddle_ReturnsTwoNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        BasicNodeDiscriminator parentDiscriminator = new(BasicSrcNode.DefaultId);
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, parentDiscriminator, [DefaultTaskValue2]));


        BasicSrcNode extraNode = new(DefaultTaskValue2);
        ExtractionTask extraTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        extraTask.SetResult(NewTaskResult(extraNode, null, [DefaultTaskValue1]), ChildrenTaskInfoFactory.Default);

        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = extraTask.ChildrenTaskInfo!.ChildrenTasks[0];
        BasicNodeDiscriminator childDiscriminator = new(DefaultDiscriminatorValue1);
        childTask.SetResult(NewTaskResult(childNode, childDiscriminator), ChildrenTaskInfoFactory.Default);

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeDiscriminator> result = context.GetStructDataStack();

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(1, childDiscriminator), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(0, parentDiscriminator), e));
    }

    [Fact]
    public void WithTwoNodesAndTwoExtra_ReturnsFourNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        BasicNodeDiscriminator parentDiscriminator = new(BasicSrcNode.DefaultId);
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, parentDiscriminator, [DefaultTaskValue1]));

        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        BasicNodeDiscriminator childDiscriminator = new(DefaultDiscriminatorValue1);
        childTask.SetResult(NewTaskResult(childNode, childDiscriminator), ChildrenTaskInfoFactory.Default);

        BaseExtractor<string, BasicNodeDiscriminator>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        BasicNodeDiscriminator extra1 = new(DefaultDiscriminatorValue2);
        BasicNodeDiscriminator extra2 = new(DefaultDiscriminatorValue3);

        // When
        IStructDataStack<BasicNodeDiscriminator> result = context.GetStructDataStack(extra1, extra2);

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(3, extra2), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(2, extra1), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(1, childDiscriminator), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeDiscriminator>.Entry(0, parentDiscriminator), e));
    }
}
