using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class TableSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<TableSerializer>();
                services.AddSerializer<TableRowSerializer>();
                services.AddSerializer<TableRowHeaderSerializer>();
                services.AddSerializer<TableRowDataSerializer>();
                services.AddSerializer<TableHeaderSerializer>();
                services.AddSerializer<TableBodySerializer>();
                services.AddSerializer<TableFooterSerializer>();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void SerializeTableHeadBodyFooter()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("It's a");

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add<HtmlTable>()
                .InitWithContent(c =>
                {
                    c.Add<HtmlTableHeader>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRow>()
                                .InitWithContent(c =>
                                {
                                    c.Add<HtmlTableRowHeader>()
                                        .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("Header1"));
                                        });
                                    c.Add<HtmlTableRowHeader>()
                                        .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("Header2"));
                                        });
                                });
                        });
                    c.Add<HtmlTableBody>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRow>()
                                .InitWithContent(c =>
                                {
                                    c.Add<HtmlTableRowData>()
                                        .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("Data1"));
                                        });
                                    c.Add<HtmlTableRowData>()
                                        .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("Data2"));
                                        });
                                });
                        });
                    c.Add<HtmlTableFooter>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRow>()
                                .InitWithContent(c =>
                                {
                                    c.Add<HtmlTableRowData>()
                                        .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("Footer1"));
                                        });
                                    c.Add<HtmlTableRowData>()
                                        .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("Footer2"));
                                        });
                                });
                        });
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal(
            "It's a\n\n| Header1 | Header2 |\n|-|-|\n| Data1 | Data2 |\n| Footer1 | Footer2 |",
            stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeTableHeadBody()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("It's a");

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add<HtmlTable>()
                .InitWithContent(c =>
                {
                    c.Add<HtmlTableRow>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRowHeader>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Header1"));
                                });
                            c.Add<HtmlTableRowHeader>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Header2"));
                                });
                        });
                    c.Add<HtmlTableRow>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data1"));
                                });
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data2"));
                                });
                        });
                    c.Add<HtmlTableRow>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data3"));
                                });
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data4"));
                                });
                        });
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal(
            "It's a\n\n| Header1 | Header2 |\n|-|-|\n| Data1 | Data2 |\n| Data3 | Data4 |",
            stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeTableNoHeader()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("It's a");

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add<HtmlTable>()
                .InitWithContent(c =>
                {
                    c.Add<HtmlTableRow>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data1"));
                                });
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data2"));
                                });
                        });
                    c.Add<HtmlTableRow>()
                        .InitWithContent(c =>
                        {
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data3"));
                                });
                            c.Add<HtmlTableRowData>()
                                .InitWithContent(c =>
                                {
                                    c.Add(new PlainTextNode("Data4"));
                                });
                        });
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal(
            "It's a\n\n| | |\n|-|-|\n| Data1 | Data2 |\n| Data3 | Data4 |",
            stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }
}
