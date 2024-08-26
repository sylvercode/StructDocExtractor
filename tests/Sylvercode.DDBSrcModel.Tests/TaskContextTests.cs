using Sylvercode.DDBSrcModel.Extraction;
using Sylvercode.DDBSrcModel.Factory;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Utils;
using Sylvercode.DDBSrcModel.StructDocStack;
using Sylvercode.DDBSrcModel.Tests.Extraction;
using Sylvercode.DDBSrcModel.Tests.Stubs;

namespace Sylvercode.DDBSrcModel.Tests;

public class TaskContextTests_GetSrcNodeStack
{
    public readonly SrcNodeFactoryProvider<BasicNodeSelectable> DefaultNodeFactoryProvider = new();

    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.ProcessResult(taskResult);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(ISrcNode node, ICollection<string>? childData = null)
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

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        ISrcNodeStack result = context.GetSrcNodeStack();

        // Then
        Assert.Empty(result);
    }

    [Fact]
    public void WithOneNode_ReturnsSingleNode()
    {
        // Given
        var node = new BasicSrcNode();
        ExtractionTask task = NewTask(NewTaskResult(node));

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        ISrcNodeStack result = context.GetSrcNodeStack();

        // Then
        Assert.Single(result, new ISrcNodeStack.Entry(0, node));
    }

    [Fact]
    public void WithTwoNodes_ReturnsTwoNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, [DefaultTaskValue1]));
        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        childTask.ProcessResult(NewTaskResult(childNode));

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        // When
        ISrcNodeStack result = context.GetSrcNodeStack();

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new ISrcNodeStack.Entry(1, childNode), e),
                          e => Assert.Equal(new ISrcNodeStack.Entry(0, parentNode), e));
    }
}

public class TaskContextTests_GetStructDataStack
{
    public readonly SrcNodeFactoryProvider<BasicNodeSelectable> DefaultNodeFactoryProvider = new();

    public const string DefaultTaskValue = nameof(DefaultTaskValue);
    public const string DefaultTaskValue1 = nameof(DefaultTaskValue1);
    public const string DefaultTaskValue2 = nameof(DefaultTaskValue2);
    public const string DefaultSelectableValue1 = nameof(DefaultSelectableValue1);
    public const string DefaultSelectableValue2 = nameof(DefaultSelectableValue2);
    public const string DefaultSelectableValue3 = nameof(DefaultSelectableValue3);

    public static ExtractionTask NewTask(BasicProcessTaskResult? taskResult = null)
    {
        ExtractionTask result = new(DefaultTaskValue);
        if (taskResult is not null)
            result.ProcessResult(taskResult);
        return result;
    }

    public static BasicProcessTaskResult NewTaskResult(ISrcNode node, BasicNodeSelectable? selectable, ICollection<string>? childData = null)
    {
        BasicProcessTaskResult taskResult = new(node)
        {
            DataSelectable = selectable ?? default
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

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeSelectable> result = context.GetStructDataStack();

        // Then
        Assert.Empty(result);
    }

    [Fact]
    public void WithOneNode_ReturnsSingleNode()
    {
        // Given
        BasicSrcNode node = new();
        BasicNodeSelectable selectable = new(BasicSrcNode.DefaultId);
        ExtractionTask task = NewTask(NewTaskResult(node, selectable));

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(task, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeSelectable> result = context.GetStructDataStack();

        // Then
        Assert.Single(result, new IStructDataStack<BasicNodeSelectable>.Entry(0, selectable));
    }

    [Fact]
    public void WithTwoNodes_ReturnsTwoNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        BasicNodeSelectable parentSelectable = new(BasicSrcNode.DefaultId);
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, parentSelectable, [DefaultTaskValue1]));

        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        BasicNodeSelectable childSelectable = new(DefaultSelectableValue1);
        childTask.ProcessResult(NewTaskResult(childNode, childSelectable));

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeSelectable> result = context.GetStructDataStack();

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(1, childSelectable), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(0, parentSelectable), e));
    }

    [Fact]
    public void WithTwoNodesAndExtraNoSelctableInMiddle_ReturnsTwoNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        BasicNodeSelectable parentSelectable = new(BasicSrcNode.DefaultId);
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, parentSelectable, [DefaultTaskValue2]));


        BasicSrcNode extraNode = new(DefaultTaskValue2);
        ExtractionTask extraTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        extraTask.ProcessResult(NewTaskResult(extraNode, null, [DefaultTaskValue1]));

        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = extraTask.ChildrenTaskInfo!.ChildrenTasks[0];
        BasicNodeSelectable childSelectable = new(DefaultSelectableValue1);
        childTask.ProcessResult(NewTaskResult(childNode, childSelectable));

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        // When
        IStructDataStack<BasicNodeSelectable> result = context.GetStructDataStack();

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(1, childSelectable), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(0, parentSelectable), e));
    }

    [Fact]
    public void WithTwoNodesAndTwoExtra_ReturnsFourNodes()
    {
        // Given
        BasicSrcBloc parentNode = new();
        BasicNodeSelectable parentSelectable = new(BasicSrcNode.DefaultId);
        ExtractionTask taskNode = NewTask(NewTaskResult(parentNode, parentSelectable, [DefaultTaskValue1]));

        BasicSrcNode childNode = new(DefaultTaskValue1);
        ExtractionTask childTask = taskNode.ChildrenTaskInfo!.ChildrenTasks[0];
        BasicNodeSelectable childSelectable = new(DefaultSelectableValue1);
        childTask.ProcessResult(NewTaskResult(childNode, childSelectable));

        BaseExtractor<string, BasicNodeSelectable>.TaskContext context = new(childTask, DefaultNodeFactoryProvider);

        BasicNodeSelectable extra1 = new(DefaultSelectableValue2);
        BasicNodeSelectable extra2 = new(DefaultSelectableValue3);

        // When
        IStructDataStack<BasicNodeSelectable> result = context.GetStructDataStack(extra1, extra2);

        // Then
        Assert.Collection(result,
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(3, extra2), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(2, extra1), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(1, childSelectable), e),
                          e => Assert.Equal(new IStructDataStack<BasicNodeSelectable>.Entry(0, parentSelectable), e));
    }
}
