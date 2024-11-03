using Sylvercode.StructDocExtractor.Markdown.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MarkdownSerializerExtentions
{
    public static IServiceCollection AddMarkdownSerializers(this IServiceCollection services)
    {
        services.AddSerializer<ParagraphSerializer>();
        services.AddSerializer<EmphasesSerializer>();
        services.AddSerializer<FigCaptionSerializer>();
        services.AddSerializer<HeadingSerializer>();
        services.AddSerializer<ImageSerializer>();
        services.AddSerializer<AnchorSerializer>();
        services.AddSerializer<ListItemSerializer>();
        services.AddSerializer<ListSerializer>();
        services.AddSerializer<ParagraphSerializer>();
        services.AddSerializer<PlainTextSerializer>();
        services.AddSerializer<StrongSerializer>();
        services.AddSerializer<TableBodySerializer>();
        services.AddSerializer<TableFooterSerializer>();
        services.AddSerializer<TableHeaderSerializer>();
        services.AddSerializer<TableRowDataSerializer>();
        services.AddSerializer<TableRowHeaderSerializer>();
        services.AddSerializer<TableRowSerializer>();
        services.AddSerializer<TableSerializer>();
        return services;
    }
}
