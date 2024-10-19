using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeHolderSerializer<TData, TChild>(List<SpyStructDocNodeSerializerEntry> entriesLog, bool SkipContent = false) : BaseStructDocNodeHolderSerializer<TData, TChild>, ISpySerializer
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    protected override void Serialize(TData obj, TextWriter stream, NodeSerializationResult result)
    {
        this.Log([obj]);
        result.ContentSerialized = SkipContent;
    }

    protected override void OnBeforeChildSerialize(TData node, TData? previousNode, TextWriter stream)
        => this.Log([node, previousNode]);

    protected override void OnAfterChildSerialize(TData node, TData? nextNode, TextWriter stream)
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
