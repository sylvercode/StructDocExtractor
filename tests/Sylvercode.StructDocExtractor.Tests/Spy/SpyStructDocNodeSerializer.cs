using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeSerializer<TData> : BaseStructDocNodeSerializer<TData>, ISpySerializer
    where TData : IStructDocNode
{
    private readonly List<SpyStructDocNodeSerializerEntry> entriesLog;

    public SpyStructDocNodeSerializer(List<SpyStructDocNodeSerializerEntry> entriesLog)
    {
        Hdl = new SpyHandler(this);
        this.entriesLog = entriesLog;
    }

    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    private class SpyHandler(SpyStructDocNodeSerializer<TData> spy) : Handler
    {
        public override void Serialize(TData obj, TextWriter stream, NodeSerializationResult result)
            => spy.Log([obj]);

        public override void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TextWriter stream)
            => spy.Log([node, previousNode]);

        public override void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TextWriter stream)
            => spy.Log([node, nextNode]);
    }
}
