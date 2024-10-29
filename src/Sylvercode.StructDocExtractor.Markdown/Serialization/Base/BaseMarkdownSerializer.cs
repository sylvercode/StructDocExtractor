using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public class BaseMarkdownSerializer<THtmlNode, TChild>(
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>.Handler? handler = null,
    ILogger? logger = null)
    : BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>(holderHandler, handler, logger)
    where THtmlNode : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
    
    protected static IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter> NewIndentedHandler(
        IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>.Options options)
    {
        return new IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>(
            options
        );
    }

    protected static IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter> NewIndentedHandler(IndentedStreamWriter.SpaceOperationType spaceBeforeAfterChild)
    {
        return new IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>(
            new IndentedSerializerHandler<THtmlNode, MarkdownStreamWriter>.Options(spaceBeforeAfterChild)
        );
    }
}

public class BaseMarkdownSerializer<THtmlNode>(
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, IStructDocNode>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, IStructDocNode>.Handler? handler = null, 
    ILogger? logger = null)
    : BaseMarkdownSerializer<THtmlNode, IStructDocNode>(holderHandler, handler, logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{

}
