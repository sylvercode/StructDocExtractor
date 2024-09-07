using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeHolderSerializer<TData, TChild>(List<SpyStructDocNodeSerializerEntry> entriesLog) : BaseStructDocNodeHolderSerializer<TData, TChild>, ISpySerializer
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    protected override void OnBeforeFirstChildSerialize(TData parent, TChild nextChild, StreamWriter stream)
        => this.Log([parent, nextChild]);

    protected override void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, StreamWriter stream)
        => this.Log([parent, previousChild]);

    protected override void OnAfterLastChildSerialize(TData parent, TChild previousChild, StreamWriter stream)
        => this.Log([parent, previousChild]);

    protected override void OnNoChildSerialize(TData parent, StreamWriter stream)
        => this.Log([parent]);
}
