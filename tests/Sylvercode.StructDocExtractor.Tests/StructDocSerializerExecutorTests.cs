using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.Tests.Spy;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.StructDocExtractor.Tests;

public class StructDocSerializerExecutorTests
{
    private const SpyStructDocNodeSerializerEntry.Methods Serialize = SpyStructDocNodeSerializerEntry.Methods.Serialize;
    private const SpyStructDocNodeSerializerEntry.Methods OnBeforeChildSerialize = SpyStructDocNodeSerializerEntry.Methods.OnBeforeChildSerialize;
    private const SpyStructDocNodeSerializerEntry.Methods OnAfterChildSerialize = SpyStructDocNodeSerializerEntry.Methods.OnAfterChildSerialize;
    private const SpyStructDocNodeSerializerEntry.Methods OnBeforeFirstChildSerialize = SpyStructDocNodeSerializerEntry.Methods.OnBeforeFirstChildSerialize;
    private const SpyStructDocNodeSerializerEntry.Methods OnBetweenSiblingSerialize = SpyStructDocNodeSerializerEntry.Methods.OnBetweenSiblingSerialize;
    private const SpyStructDocNodeSerializerEntry.Methods OnAfterLastChildSerialize = SpyStructDocNodeSerializerEntry.Methods.OnAfterLastChildSerialize;
    private const SpyStructDocNodeSerializerEntry.Methods OnNoChildSerialize = SpyStructDocNodeSerializerEntry.Methods.OnNoChildSerialize;

    public class ExtractionContext : IDisposable
    {
        private readonly StreamWriter _streamWriter = new(Stream.Null);
        public List<SpyStructDocNodeSerializerEntry> EntriesLog { get; } = [];
        public ISpySerializer RootSerializer { get; }
        public ISpySerializer BlockSerializer { get; }
        public ISpySerializer NodeSerializer { get; }
        public SerializerProvider SerializerProvider { get; }

        public ExtractionContext(bool SkipBlockConetntSerialization = false)
        {
            RootSerializer = new SpyStructDocNodeHolderSerializer<BasicSrcRootBlock, IStructDocNode>(EntriesLog);
            BlockSerializer = new SpyStructDocNodeHolderSerializer<BasicSrcBloc, IStructDocNode>(EntriesLog, SkipBlockConetntSerialization);
            NodeSerializer = new SpyStructDocNodeSerializer<BasicSrcNode>(EntriesLog);
            SerializerProvider = new SerializerProvider(){
                {typeof(BasicSrcRootBlock), RootSerializer},
                {typeof(BasicSrcBloc), BlockSerializer},
                {typeof(BasicSrcNode), NodeSerializer},
            };
        }

        public StructDocSerializerExecutor NewExecutor(IStructDocNode rootData)
            => new(_streamWriter, rootData, SerializerProvider);

        public void Dispose()
        {
            _streamWriter.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    [Fact]
    public void SingleRoot_OneNoChildEntry()
    {
        // Given
        ExtractionContext context = new();
        BasicSrcRootBlock root = new(nameof(root));
        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, Serialize, root);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnNoChildSerialize, root);
        Assert.Collection(context.EntriesLog, ExpectedEntriesLog.AsAsserter());
    }

    [Fact]
    public void SingleNodeInRoot_AfterBefore()
    {
        // Given
        ExtractionContext context = new();
        BasicSrcRootBlock root = new(nameof(root));
        var rootInit = root.NewParentChildLinkInitializer();
        BasicSrcNode node = new(nameof(node));
        rootInit.AddChild(node);
        rootInit.InitializeParentChildLink();

        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, Serialize, root);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBeforeFirstChildSerialize, root, node);
        ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, node, null);
        ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, node);
        ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, node, null);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnAfterLastChildSerialize, root, node);
        Assert.Collection(context.EntriesLog, ExpectedEntriesLog.AsAsserter());
    }

    [Fact]
    public void TwoNodeInRoot_AfterBeforeBetween()
    {
        // Given
        ExtractionContext context = new();
        BasicSrcRootBlock root = new(nameof(root));
        var rootInit = root.NewParentChildLinkInitializer();
        BasicSrcNode node1 = new(nameof(node1));
        rootInit.AddChild(node1);
        BasicSrcNode node2 = new(nameof(node2));
        rootInit.AddChild(node2);
        rootInit.InitializeParentChildLink();

        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, Serialize, root);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBeforeFirstChildSerialize, root, node1);
        {
            ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, node1, null);
            ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, node1);
            ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, node1, node2);
        }

        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBetweenSiblingSerialize, root, node1, node2);

        {
            ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, node2, node1);
            ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, node2);
            ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, node2, null);
        }
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnAfterLastChildSerialize, root, node2);
        Assert.Collection(ExpectedEntriesLog, context.EntriesLog.AsAsserter());
    }

    [Fact]
    public void TwoBlockHavingTwoNodeInRoot()
    {
        // Given
        ExtractionContext context = new();
        BasicSrcRootBlock root = new(nameof(root));
        var rootInit = root.NewParentChildLinkInitializer();
        BasicSrcBloc blockA = new(nameof(blockA));
        rootInit.AddChild(blockA);
        var blockAInit = blockA.NewParentChildLinkInitializer();
        BasicSrcNode nodeA1 = new(nameof(nodeA1));
        blockAInit.AddChild(nodeA1);
        BasicSrcNode nodeA2 = new(nameof(nodeA2));
        blockAInit.AddChild(nodeA2);
        blockAInit.InitializeParentChildLink();
        BasicSrcBloc blockB = new(nameof(blockB));
        rootInit.AddChild(blockB);
        var blockBInit = blockB.NewParentChildLinkInitializer();
        BasicSrcNode nodeB1 = new(nameof(nodeB1));
        blockBInit.AddChild(nodeB1);
        BasicSrcNode nodeB2 = new(nameof(nodeB2));
        blockBInit.AddChild(nodeB2);
        blockBInit.InitializeParentChildLink();
        rootInit.InitializeParentChildLink();

        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, Serialize, root);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBeforeFirstChildSerialize, root, blockA);
        {
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeChildSerialize, blockA, null);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, Serialize, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeFirstChildSerialize, blockA, nodeA1);
            {
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, nodeA1, null);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, nodeA1);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, nodeA1, nodeA2);
            }
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBetweenSiblingSerialize, blockA, nodeA1, nodeA2);
            {
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, nodeA2, nodeA1);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, nodeA2);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, nodeA2, null);
            }
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterLastChildSerialize, blockA, nodeA2);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterChildSerialize, blockA, blockB);
        }

        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBetweenSiblingSerialize, root, blockA, blockB);

        {
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeChildSerialize, blockB, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, Serialize, blockB);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeFirstChildSerialize, blockB, nodeB1);
            {
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, nodeB1, null);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, nodeB1);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, nodeB1, nodeB2);
            }
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBetweenSiblingSerialize, blockB, nodeB1, nodeB2);
            {
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnBeforeChildSerialize, nodeB2, nodeB1);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, Serialize, nodeB2);
                ExpectedEntriesLog.AddEntry(context.NodeSerializer, OnAfterChildSerialize, nodeB2, null);
            }
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterLastChildSerialize, blockB, nodeB2);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterChildSerialize, blockB, null);
        }
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnAfterLastChildSerialize, root, blockB);

        Assert.Collection(ExpectedEntriesLog, context.EntriesLog.AsAsserter());
    }

    [Fact]
    public void TwoEmptyBlockHavingInRoot()
    {
        // Given
        ExtractionContext context = new();
        BasicSrcRootBlock root = new(nameof(root));
        var rootInit = root.NewParentChildLinkInitializer();
        BasicSrcBloc blockA = new(nameof(blockA));
        rootInit.AddChild(blockA);
        BasicSrcBloc blockB = new(nameof(blockB));
        rootInit.AddChild(blockB);
        rootInit.InitializeParentChildLink();

        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, Serialize, root);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBeforeFirstChildSerialize, root, blockA);
        {
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeChildSerialize, blockA, null);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, Serialize, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnNoChildSerialize, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterChildSerialize, blockA, blockB);
        }

        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBetweenSiblingSerialize, root, blockA, blockB);

        {
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeChildSerialize, blockB, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, Serialize, blockB);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnNoChildSerialize, blockB);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterChildSerialize, blockB, null);
        }
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnAfterLastChildSerialize, root, blockB);
        Assert.Collection(ExpectedEntriesLog, context.EntriesLog.AsAsserter());
    }

    [Fact]
    public void TwoBlockHavingTwoNodeInRoot_WithSkipBlockContentSerialization()
    {
        // Given
        ExtractionContext context = new(SkipBlockConetntSerialization: true);
        BasicSrcRootBlock root = new(nameof(root));
        var rootInit = root.NewParentChildLinkInitializer();
        BasicSrcBloc blockA = new(nameof(blockA));
        rootInit.AddChild(blockA);
        var blockAInit = blockA.NewParentChildLinkInitializer();
        BasicSrcNode nodeA1 = new(nameof(nodeA1));
        blockAInit.AddChild(nodeA1);
        BasicSrcNode nodeA2 = new(nameof(nodeA2));
        blockAInit.AddChild(nodeA2);
        blockAInit.InitializeParentChildLink();
        BasicSrcBloc blockB = new(nameof(blockB));
        rootInit.AddChild(blockB);
        var blockBInit = blockB.NewParentChildLinkInitializer();
        BasicSrcNode nodeB1 = new(nameof(nodeB1));
        blockBInit.AddChild(nodeB1);
        BasicSrcNode nodeB2 = new(nameof(nodeB2));
        blockBInit.AddChild(nodeB2);
        blockBInit.InitializeParentChildLink();
        rootInit.InitializeParentChildLink();

        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, Serialize, root);
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBeforeFirstChildSerialize, root, blockA);
        {
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeChildSerialize, blockA, null);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, Serialize, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterChildSerialize, blockA, blockB);
        }

        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnBetweenSiblingSerialize, root, blockA, blockB);

        {
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnBeforeChildSerialize, blockB, blockA);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, Serialize, blockB);
            ExpectedEntriesLog.AddEntry(context.BlockSerializer, OnAfterChildSerialize, blockB, null);
        }
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnAfterLastChildSerialize, root, blockB);

        Assert.Collection(ExpectedEntriesLog, context.EntriesLog.AsAsserter());
    }
}
