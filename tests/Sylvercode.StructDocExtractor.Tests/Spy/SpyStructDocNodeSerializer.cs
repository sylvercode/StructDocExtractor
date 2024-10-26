using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeSerializer<TData>(List<SpyStructDocNodeSerializerEntry> entriesLog) : BaseStructDocNodeSerializer<TData>, ISpySerializer
    where TData : IStructDocNode
{
    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    protected override void Serialize(TData obj, TextWriter stream, NodeSerializationResult result)
        => this.Log([obj]);

    protected override void OnBeforeChildSerialize(TData node, IStructDocNode? previousNode, TextWriter stream)
        => this.Log([node, previousNode]);

    protected override void OnAfterChildSerialize(TData node, IStructDocNode? nextNode, TextWriter stream)
        => this.Log([node, nextNode]);
}
