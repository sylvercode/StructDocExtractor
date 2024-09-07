using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeSerializer<TData>(List<SpyStructDocNodeSerializerEntry> entriesLog) : BaseStructDocNodeSerializer<TData>, ISpySerializer
    where TData : IStructDocNode
{
    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    public override void Serialize(TData obj, StreamWriter stream)
        => this.Log([obj]);

    public override void OnBeforeChildSerialize(TData node, TData? previousNode, StreamWriter stream)
        => this.Log([node, previousNode]);

    public override void OnAfterChildSerialize(TData node, TData? nextNode, StreamWriter stream)
        => this.Log([node, nextNode]);
}
