using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.Tests.Fakes;
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

        public ExtractionContext()
        {
            RootSerializer = new SpyStructDocNodeHolderSerializer<BasicSrcRootBlock, IStructDocNode>(EntriesLog);
            BlockSerializer = new SpyStructDocNodeHolderSerializer<BasicSrcBloc, IStructDocNode>(EntriesLog);
            NodeSerializer = new SpyStructDocNodeSerializer<BasicSrcNode>(EntriesLog);
            SerializerProvider = new SerializerProvider(){
                {typeof(BasicSrcRootBlock), RootSerializer},
                {typeof(BasicSrcBloc), BlockSerializer},
                {typeof(BasicSrcNode), NodeSerializer},
            };
        }

        public StructDocSerializerExecutor NewExecutor(object rootData)
            => new(_streamWriter, rootData, SerializerProvider);

        public void Dispose()
        {
            _streamWriter.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    [Fact]
    public void SingleRoot_One()
    {
        // Given
        ExtractionContext context = new();
        BasicSrcRootBlock root = new();
        StructDocSerializerExecutor executor = context.NewExecutor(root);

        // When
        executor.ExecuteTasks();

        // Then
        List<SpyStructDocNodeSerializerEntry> ExpectedEntriesLog = [];
        ExpectedEntriesLog.AddEntry(context.RootSerializer, OnNoChildSerialize, root);
        Assert.Collection(ExpectedEntriesLog, context.EntriesLog.AsAsserter());
    }
}
