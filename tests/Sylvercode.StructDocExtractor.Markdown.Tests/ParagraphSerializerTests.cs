using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class ParagraphSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void RootParagraph_DoNotStartParagraph()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello ");

        HtmlParagraph para = new();
        para.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("World"));
        });
        para.MakeARoot();

        // When
        serializer.Serialize(stringTextWriter.Writer, para);

        // Then
        Assert.Equal("Hello World", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.None, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void NotRootParagraph_StartParagraph()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello");

        HtmlParagraph para = new();
        para.InitWithContent(c =>
        {
            c.Add<HtmlParagraph>()
                .InitWithContent(c =>
                {
                    c.Add(new PlainTextNode("World"));
                });
        });
        para.MakeARoot();

        // When
        serializer.Serialize(stringTextWriter.Writer, para);

        // Then
        Assert.Equal("Hello\n\nWorld", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }
}
