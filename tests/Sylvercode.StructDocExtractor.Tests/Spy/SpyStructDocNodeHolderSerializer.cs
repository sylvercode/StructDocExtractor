using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeHolderSerializer<TData, TChild>(List<SpyStructDocNodeSerializerEntry> entriesLog) : BaseStructDocNodeHolderSerializer<TData, TChild>, ISpySerializer
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    public override void Serialize(TData obj, TextWriter stream)
        => this.Log([obj]);

    public override void OnBeforeChildSerialize(TData node, TData? previousNode, TextWriter stream)
        => this.Log([node, previousNode]);

    public override void OnAfterChildSerialize(TData node, TData? nextNode, TextWriter stream)
        => this.Log([node, nextNode]);

    protected override void OnBeforeFirstChildSerialize(TData parent, TChild nextChild, TextWriter stream)
        => this.Log([parent, nextChild]);

    protected override void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TextWriter stream)
        => this.Log([parent, previousChild, nextChild]);

    protected override void OnAfterLastChildSerialize(TData parent, TChild previousChild, TextWriter stream)
        => this.Log([parent, previousChild]);

    protected override void OnNoChildSerialize(TData parent, TextWriter stream)
        => this.Log([parent]);
}
