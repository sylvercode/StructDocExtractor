using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MarkdownSerializerExtentions
{
    public static IServiceCollection AddMarkdownSerializers(this IServiceCollection services)
    {
        services.AddSerializer<HtmlDiv, DivSerializer>();
        services.AddSerializer<HtmlEmphases, EmphasesSerializer>();
        services.AddSerializer<HtmlFigCaption, FigCaptionSerializer>();
        services.AddSerializer<HtmlFigure, FigureSerializer>();
        services.AddSerializer<HtmlHeading, HeadingSerializer>();
        services.AddSerializer<HtmlImg, ImageSerializer>();
        services.AddSerializer<HtmlAnchor, LinkSerializer>();
        services.AddSerializer<HtmlListItem, ListItemSerializer>();
        services.AddSerializer<HtmlList, ListSerializer>();
        services.AddSerializer<HtmlParagraph, ParagraphSerializer>();
        services.AddSerializer<PlainTextNode, PlainTextSerializer>();
        services.AddSerializer<HtmlStrong, StrongSerializer>();
        services.AddSerializer<HtmlTableBody, TableBodySerializer>();
        services.AddSerializer<HtmlTableFooter, TableFooterSerializer>();
        services.AddSerializer<HtmlTableHeader, TableHeaderSerializer>();
        services.AddSerializer<HtmlTableRowData, TableRowDataSerializer>();
        services.AddSerializer<HtmlTableRowHeader, TableRowHeaderSerializer>();
        services.AddSerializer<HtmlTableRow, TableRowSerializer>();
        services.AddSerializer<HtmlTable, TableSerializer>();
        return services;
    }
}
