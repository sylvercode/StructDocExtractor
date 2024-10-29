using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class ImageSerializerTest
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<ImageSerializer>();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void SerializeImage()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("Hello");

        HtmlParagraph paragraph = new("Hello");
        paragraph.MakeARoot();
        paragraph.InitWithContent(c =>
        {
            c.Add(new HtmlImg("world.png"));
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, paragraph);

        // Then
        Assert.Equal("Hello ![](world.png)", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Word, stringTextWriter.Writer.PendingOperation);
    }
}
