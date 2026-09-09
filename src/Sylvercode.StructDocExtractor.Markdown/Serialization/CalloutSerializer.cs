using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

/// <summary>Serializer that emits block-level callout/aside nodes as Obsidian-flavoured Markdown callout blocks using <c>&gt; [!type]</c> syntax.</summary>
/// <remarks>The callout type can be mapped per node type via the optional <c>calloutTypeMap</c> constructor parameter; unmapped types fall back to <paramref name="calloutType"/>.</remarks>
public class CalloutSerializer(
    IReadOnlyDictionary<Type, CalloutSerializer.CalloutType>? calloutTypeMap = null,
    CalloutSerializer.CalloutType calloutType = CalloutSerializer.CalloutType.info,
    ILogger<CalloutSerializer>? logger = null)
    : BaseMarkdownSerializer<IStructDocNodeHolder<IStructDocNode>>(
        handler: NewIndentedHandler(IndentedStreamWriter.SpaceOperationType.Paragraph),
        holderHandler: new CalloutHolderHandler(calloutTypeMap, calloutType),
        logger: logger),
    IStructDocNodeSerializerServiceInit
{
    /// <summary>Defines the recognised Obsidian callout type labels.</summary>
    public enum CalloutType
    {
        /// <summary>General informational callout.</summary>
        info,
        /// <summary>Note callout for supplementary annotations.</summary>
        note,
        /// <summary>Summary callout for condensed overviews.</summary>
        summary,
        /// <summary>Todo callout for action items.</summary>
        todo,
        /// <summary>Tip callout for helpful hints.</summary>
        tip,
        /// <summary>Success callout indicating a positive outcome.</summary>
        success,
        /// <summary>Question callout for open queries.</summary>
        question,
        /// <summary>Warning callout for cautionary notes.</summary>
        warning,
        /// <summary>Failure callout indicating a negative outcome.</summary>
        failure,
        /// <summary>Danger callout for critical hazards.</summary>
        danger,
        /// <summary>Bug callout for known defects.</summary>
        bug,
        /// <summary>Example callout for illustrative content.</summary>
        example,
        /// <summary>Quote callout for block quotations.</summary>
        quote,
    }

    /// <summary>Holder handler that writes the callout header line and manages the block-quote line prefix.</summary>
    public class CalloutHolderHandler(
        IReadOnlyDictionary<Type, CalloutType>? calloutTypeMap,
        CalloutType defaultCalloutType) : HolderHandler
    {
        /// <inheritdoc/>
        public override void OnBeforeFirstChildSerialize(IStructDocNodeHolder<IStructDocNode> parent, IStructDocNode firstChild, MarkdownStreamWriter stream)
        {
            stream.PushLineHeadToken($"> ");

            CalloutType calloutType = calloutTypeMap?.GetValueOrDefault(parent.GetType(), defaultCalloutType) ?? defaultCalloutType;
            string calloutText = Enum.GetName(calloutType)!;
            stream.WriteLine($"> [!{calloutText}]");
        }

        /// <inheritdoc/>
        public override void OnAfterLastChildSerialize(IStructDocNodeHolder<IStructDocNode> parent, IStructDocNode lastChild, MarkdownStreamWriter stream)
        {
            stream.PopLineHeadToken();
        }
    }

    /// <inheritdoc/>
    IEnumerable<Type> IStructDocNodeSerializerServiceInit.GetDefaultSeriazableType()
        => calloutTypeMap?.Keys ?? [];
}
