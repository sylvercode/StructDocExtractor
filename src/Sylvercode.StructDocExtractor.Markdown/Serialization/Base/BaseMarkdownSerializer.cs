using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Base;

public class BaseMarkdownSerializer<THtmlNode, TChild>(ILogger? logger = null)
: BaseStructDocNodeHolderSerializer<THtmlNode, MarkdownStreamWriter, TChild>(logger)
    where THtmlNode : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{

}

public class BaseMarkdownSerializer<THtmlNode>(ILogger? logger = null)
    : BaseMarkdownSerializer<THtmlNode, IStructDocNode>(logger)
    where THtmlNode : IStructDocNodeHolder<IStructDocNode>
{

}
