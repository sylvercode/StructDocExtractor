using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class StrongSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<EmphasesSerializer>();
                services.AddSerializer<StrongSerializer>();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Theory]
    [InlineData("Hello,")]
    [InlineData("Hello, ")]
    public void StandaloneStyle(string source)
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write(source);

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add<HtmlStrong>()
                .InitWithContent(c =>
                {
                    c.Add(new PlainTextNode("World"));
                });
        });


        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal($"Hello, **World**", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Word, stringTextWriter.Writer.PendingOperation);
    }

    [Theory]
    [InlineData("Hello,")]
    [InlineData("Hello, ")]
    public void CompositStyle(string source)
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write(source);

        HtmlDiv div = new();
        div.MakeARoot();
        div.InitWithContent(c =>
        {
            c.Add<HtmlStrong>()
                .InitWithContent(c =>
                {
                    c.Add<HtmlEmphases>()
                        .InitWithContent(c =>
                        {
                            c.Add(new PlainTextNode("World"));
                        });
                });
        });


        // When
        serializer.Serialize(stringTextWriter.Writer, div);

        // Then
        Assert.Equal($"Hello, ***World***", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Word, stringTextWriter.Writer.PendingOperation);
    }

}
