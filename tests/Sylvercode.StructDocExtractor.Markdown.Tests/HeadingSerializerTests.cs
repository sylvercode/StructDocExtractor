using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class HeadingSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<HeadingSerializer>();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void SerializeHeading()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello");

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add(new HtmlHeading(2))
                .InitWithContent(c =>
                {
                    c.Add(new PlainTextNode("World!"));
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal("Hello\n\n## World!", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeHeadingWithAnchor()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello\n\n");

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add(new HtmlHeading(1, "anchor"))
                .InitWithContent(c =>
                {
                    c.Add(new PlainTextNode("World!"));
                });
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal("Hello\n\n# World! ^anchor", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
    }
}
