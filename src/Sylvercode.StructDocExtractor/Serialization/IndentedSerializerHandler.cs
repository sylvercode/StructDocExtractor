using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class IndentedSerializerHandler<TData, TWriter>(
    IndentedSerializerHandler<TData, TWriter>.Options options)
    : BaseStructDocNodeSerializer<TData, TWriter>.Handler
        where TData : IStructDocNode
        where TWriter : IndentedStreamWriter
{
    public class Options(IndentedStreamWriter.SpaceOperationType spaceBeforeAfterChild = IndentedStreamWriter.SpaceOperationType.None)
    {
        public bool SpaceOperationOnlyOnNoneRoot { get; set; } = true;
        public IndentedStreamWriter.SpaceOperationType SpaceBeforeChild { get; set; } = spaceBeforeAfterChild;
        public IndentedStreamWriter.SpaceOperationType SpaceAfterChild { get; set; } = spaceBeforeAfterChild;

        public bool MustDoSpaceOperation(TData node)
            => !SpaceOperationOnlyOnNoneRoot || !node.IsRoot;
    }

    private readonly Options _options = options;

    public override void OnBeforeChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream)
    {
        if (_options.MustDoSpaceOperation(node))
            stream.DoSpaceOperation(_options.SpaceBeforeChild);
    }

    public override void OnAfterChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream)
    {
        if (_options.MustDoSpaceOperation(node))
            stream.EnsureSpaceOperation(_options.SpaceAfterChild);
    }
}
