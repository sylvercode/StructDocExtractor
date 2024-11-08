using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

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
    public enum CalloutType
    {
        info,
        note,
        summary,
        todo,
        tip,
        success,
        question,
        warning,
        failure,
        danger,
        bug,
        example,
        quote,
    }

    public class CalloutHolderHandler(
        IReadOnlyDictionary<Type, CalloutType>? calloutTypeMap,
        CalloutType defaultCalloutType) : HolderHandler
    {

        public override void OnBeforeFirstChildSerialize(IStructDocNodeHolder<IStructDocNode> parent, IStructDocNode firstChild, MarkdownStreamWriter stream)
        {
            stream.PushLineHeadToken($"> ");

            CalloutType calloutType = calloutTypeMap?.GetValueOrDefault(parent.GetType(), defaultCalloutType) ?? defaultCalloutType;
            string calloutText = Enum.GetName(calloutType)!;
            stream.WriteLine($"> [!{calloutText}]");
        }

        public override void OnAfterLastChildSerialize(IStructDocNodeHolder<IStructDocNode> parent, IStructDocNode lastChild, MarkdownStreamWriter stream)
        {
            stream.PopLineHeadToken();
        }
    }

    IEnumerable<Type> IStructDocNodeSerializerServiceInit.GetDefaultSeriazableType()
        => calloutTypeMap?.Keys ?? [];
}
