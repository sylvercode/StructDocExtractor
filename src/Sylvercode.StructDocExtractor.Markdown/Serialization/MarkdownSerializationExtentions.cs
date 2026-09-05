using Sylvercode.StructDocExtractor.Markdown.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for registering all Markdown serializers in a DI container.</summary>
public static class MarkdownSerializerExtentions
{
    /// <summary>Registers all standard Markdown serializers in <paramref name="services"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add serializers to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddMarkdownSerializers(this IServiceCollection services)
    {
        services.AddSerializer<BrSerializer>();
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
