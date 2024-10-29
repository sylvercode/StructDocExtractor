using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class ListSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<ListSerializer>();
                services.AddSerializer<ListItemSerializer>();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void SerializeSingleList()
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
            c.Add<HtmlList>()
                .InitWithContent(c =>
                {
                    c.Add<HtmlListItem>()
                       .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("bird?"));
                        });
                    c.Add<HtmlListItem>()
                       .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("plane?"));
                        });
                    c.Add<HtmlListItem>()
                       .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("Superman!"));
                        });
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal("It's a\n\n- bird?\n- plane?\n- Superman!", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeEmbededList()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Main list");

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add<HtmlList>()
                .InitWithContent(c =>
                {
                    c.Add<HtmlListItem>()
                       .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("item1"));
                            c.Add<HtmlList>()
                                .InitWithContent(c =>
                                {
                                    c.Add<HtmlListItem>()
                                       .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("subitem1"));
                                        });
                                    c.Add<HtmlListItem>()
                                       .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("subitem2"));
                                        });
                                });
                        });
                    c.Add<HtmlListItem>()
                       .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("item2"));
                        });
                    c.Add<HtmlListItem>()
                       .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("item3"));
                            c.Add<HtmlList>()
                                .InitWithContent(c =>
                                {
                                    c.Add<HtmlListItem>()
                                       .InitWithContent(c =>
                                        {
                                            c.Add(new PlainTextNode("subitem3"));
                                        });
                                });
                        });
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal(
            "Main list\n\n- item1\n  - subitem1\n  - subitem2\n- item2\n- item3\n  - subitem3",
            stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }
}
