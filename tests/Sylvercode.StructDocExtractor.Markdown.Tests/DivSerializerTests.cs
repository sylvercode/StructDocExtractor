using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class DivSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<HtmlDiv, DivSerializer>();
                services.AddSerializer<PlainTextNode, PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void RootDiv_DoNotStartParagraph()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello ");

        HtmlDiv div = new();
        div.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("World"));
        });
        div.MakeARoot();

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal("Hello World", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.PedingOperationType.None, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void NotRootDiv_StartParagraph()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello");

        HtmlDiv div = new();
        div.InitWithContent(c =>
        {
            c.Add<HtmlDiv>()
                .InitWithContent(c =>
                {
                    c.Add(new PlainTextNode("World"));
                });
        });
        div.MakeARoot();

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal("Hello\n\nWorld", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.PedingOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }
}
