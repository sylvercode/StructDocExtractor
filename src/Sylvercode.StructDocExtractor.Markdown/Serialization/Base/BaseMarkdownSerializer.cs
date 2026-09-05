using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

/// <summary>Base serializer for <see cref="IStructDocNodeHolder{TChild}"/> nodes that writes to a <see cref="MarkdownStreamWriter"/>, providing indented-handler factory helpers to subclasses.</summary>
/// <typeparam name="THtmlNode">The holder node type being serialized.</typeparam>
/// <typeparam name="TChild">The child node type held by <typeparamref name="THtmlNode"/>.</typeparam>
/// <remarks>Bridges <see cref="BaseStructDocNodeHolderSerializer{THtmlNode,TWriter,TChild}"/> to the Markdown output layer; subclasses use the static factory helpers to build indentation-aware handlers.</remarks>
public class BaseMarkdownSerializer<THtmlNode, TChild>(
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>.Handler? handler = null,
    ILogger? logger = null)
    : BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>(holderHandler, handler, logger)
    where THtmlNode : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    /// <summary>Creates a new <see cref="IndentedSerializerHandler{THtmlNode,TWriter}"/> with the specified options.</summary>
    /// <param name="options">The options controlling indentation and spacing behaviour.</param>
    /// <returns>A configured <see cref="IndentedSerializerHandler{THtmlNode,TWriter}"/> instance.</returns>
    protected static IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter> NewIndentedHandler(
        IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>.Options options)
    {
        return new IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>(
            options
        );
    }

    /// <summary>Creates a new <see cref="IndentedSerializerHandler{THtmlNode,TWriter}"/> with the specified spacing operation type.</summary>
    /// <param name="spaceBeforeAfterChild">The spacing operation applied before and after child serialization.</param>
    /// <returns>A configured <see cref="IndentedSerializerHandler{THtmlNode,TWriter}"/> instance.</returns>
    protected static IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter> NewIndentedHandler(IndentedStreamWriter.SpaceOperationType spaceBeforeAfterChild)
    {
        return new IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>(
            new IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>.Options(spaceBeforeAfterChild)
        );
    }
}

/// <summary>Base serializer for <see cref="IStructDocNodeHolder{IStructDocNode}"/> nodes that writes to a <see cref="MarkdownStreamWriter"/>, using <see cref="IStructDocNode"/> as the child type.</summary>
/// <typeparam name="THtmlNode">The holder node type being serialized.</typeparam>
public class BaseMarkdownSerializer<THtmlNode>(
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, IStructDocNode>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, IStructDocNode>.Handler? handler = null, 
    ILogger? logger = null)
    : BaseMarkdownSerializer<THtmlNode, IStructDocNode>(holderHandler, handler, logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{

}
