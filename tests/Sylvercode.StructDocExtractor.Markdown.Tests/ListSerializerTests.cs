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

    [Theory]
    [InlineData("", "", "")]
    [InlineData("37066f", "", "\n\n^37066f")]
    [InlineData("", "37006f", "")]
    [InlineData("37066f", "37006f", "\n\n^37066f")]
    public void SerializeSingleList(string listId, string itemId, string expectedListId)
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("It's a");

        HtmlList list = new() { Id = listId };
        HtmlListItem firstItem = new() { Id = itemId };
        firstItem.InitWithContent(c => c.Add(new PlainTextNode("bird?")));
        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add(list);
            list.InitWithContent(c =>
                {
                    c.Add(firstItem);
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
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
        stringTextWriter.Writer.Write("Following paragraph.");
        string expectedItemId = string.IsNullOrEmpty(itemId) ? "" : $"\n  ^{itemId}";
        Assert.Equal($"It's a\n\n- bird?{expectedItemId}\n- plane?\n- Superman!{expectedListId}\n\nFollowing paragraph.", stringTextWriter.GetResult());
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
