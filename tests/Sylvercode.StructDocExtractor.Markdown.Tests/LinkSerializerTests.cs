
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class LinkSerializerTests
{
    private static IHost GetHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.AddSerializer<LinkSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Fact]
    public void SerializeFragmentOnlyLink()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("some text");

        HtmlAnchor anchor = new("wiki:/#^fragment");
        anchor.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("link text"));
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, anchor);

        // Then
        Assert.Equal("some text[[#^fragment|link text]]", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.None, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeExternalLink()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("some text");

        HtmlAnchor anchor = new("https://example.com/source/page%20with%20space.html?key1=value&keu2=value#frag");
        anchor.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("link text"));
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, anchor);

        // Then
        Assert.Equal("some text[link text](https://example.com/source/page%20with%20space.html?key1=value&keu2=value#frag)", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.None, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeInternalLink()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("some text");

        HtmlAnchor anchor = new("dir/page%20with%20space.md");
        anchor.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("link text"));
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, anchor);

        // Then
        Assert.Equal("some text[link text](dir/page%20with%20space.md)", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.None, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeWikiLink()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("some text");

        HtmlAnchor anchor = new("wiki:ref with space");
        anchor.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("ref with space"));
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, anchor);

        // Then
        Assert.Equal("some text[[ref with space]]", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.None, stringTextWriter.Writer.PendingOperation);
    }

    [Fact]
    public void SerializeWikiLinkWithCustomTest()
    {
        // Given
        IHost host = GetHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        stringTextWriter.Writer.Write("some text");

        HtmlAnchor anchor = new("wiki:ref with space");
        anchor.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("link text"));
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, anchor);

        // Then
        Assert.Equal("some text[[ref with space|link text]]", stringTextWriter.GetResult());
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.None, stringTextWriter.Writer.PendingOperation);
    }
}
