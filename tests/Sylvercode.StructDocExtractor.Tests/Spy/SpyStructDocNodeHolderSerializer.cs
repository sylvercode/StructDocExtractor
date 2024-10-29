using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeHolderSerializer<TData, TChild> : BaseStructDocNodeHolderSerializer<TData, TChild>,
    ISpySerializer
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    private readonly List<SpyStructDocNodeSerializerEntry> entriesLog;

    public SpyStructDocNodeHolderSerializer(
        List<SpyStructDocNodeSerializerEntry> entriesLog,
        bool SkipContent = false) : base()
    {
        HolderHdl = new SpyHolderHandler(this);
        Hdl = new SpyHandler(this, SkipContent);
        this.entriesLog = entriesLog;
    }

    public List<SpyStructDocNodeSerializerEntry> EntriesLog => entriesLog;

    private class SpyHandler(SpyStructDocNodeHolderSerializer<TData, TChild> spy, bool SkipContent) : Handler
    {
        public override void Serialize(TData obj, TextWriter stream, NodeSerializationResult result)
        {
            spy.Log([obj]);
            result.ContentSerialized = SkipContent;
        }

        public override void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TextWriter stream)
            => spy.Log([node, previousNode]);

        public override void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TextWriter stream)
            => spy.Log([node, nextNode]);
    }

    private class SpyHolderHandler(SpyStructDocNodeHolderSerializer<TData, TChild> spy) : HolderHandler
    {
        public override void OnBeforeFirstChildSerialize(TData parent, TChild nextChild, TextWriter stream)
        => spy.Log([parent, nextChild]);

        public override void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TextWriter stream)
            => spy.Log([parent, previousChild, nextChild]);

        public override void OnAfterLastChildSerialize(TData parent, TChild previousChild, TextWriter stream)
            => spy.Log([parent, previousChild]);

        public override void OnNoChildSerialize(TData parent, TextWriter stream)
            => spy.Log([parent]);
    }
}
