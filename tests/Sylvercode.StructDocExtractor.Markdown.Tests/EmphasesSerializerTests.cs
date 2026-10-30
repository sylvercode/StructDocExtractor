using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class EmphasesSerializerTests
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
    [InlineData(", after.", ", after.")]
    [InlineData(".", ".")]
    [InlineData("!", "!")]
    [InlineData("?", "?")]
    [InlineData("; after.", "; after.")]
    [InlineData(": after.", ": after.")]
    [InlineData(")", ")")]
    [InlineData(" after.", " after.")]
    [InlineData("after.", " after.")]
    public void Emphasis_PreservesFollowingPunctuationAndWhitespace(string followingText, string expectedSuffix)
    {
        using IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> textWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        HtmlParagraph paragraph = new();
        paragraph.MakeARoot();
        paragraph.InitWithContent(content =>
        {
            content.Add(new HtmlEmphases()).InitWithContent(emphasis =>
            {
                emphasis.Add(new PlainTextNode("World"));
            });
            content.Add(new PlainTextNode(followingText));
        });

        serializer.Serialize(textWriter.Writer, paragraph);

        Assert.Equal($"*World*{expectedSuffix}", textWriter.GetResult());
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

        HtmlParagraph para = new();
        para.MakeARoot();
        para.InitWithContent(c =>
        {
            c.Add<HtmlEmphases>()
                .InitWithContent(c =>
                {
                    c.Add(new PlainTextNode("World"));
                });
        });


        // When
        serializer.Serialize(stringTextWriter.Writer, para);

        // Then
        Assert.Equal($"Hello, *World*", stringTextWriter.GetResult());
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

        HtmlParagraph para = new();
        para.MakeARoot();
        para.InitWithContent(c =>
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
        serializer.Serialize(stringTextWriter.Writer, para);

        // Then
        Assert.Equal($"Hello, ***World***", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Word, stringTextWriter.Writer.PendingOperation);
    }

}
