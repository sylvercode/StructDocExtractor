using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests;

public class MarkdownStreamWriterTests_PushStyle
{
    private static IHost GetHost(MarkdownStyle? style = null)
    {
        return Host.CreateDefaultBuilder()
                   .ConfigureServices((context, services) =>
                   {
                       services.AddMarkdownStreamWriterProvider(style is null
                            ? null
                            : option =>
                                {
                                    option.IndentSpec = style.IndentSpec;
                                    option.EmphasisCharacter = style.EmphasisCharacter;
                                    option.StrongCharacter = style.StrongCharacter;
                                    option.PreferAlternateStyle = style.PreferAlternateStyle;
                                }
                       );
                   })
                   .Build();
    }

    [Fact]
    public void PushEmphasisDefault_SingleAsterisk()
    {
        // Given
        using IHost host = GetHost();
        ITextWriterProvider writerProvider = host.Services.GetRequiredService<ITextWriterProvider>();
        using MemoryStream stream = new();
        using MarkdownStreamWriter writer = (MarkdownStreamWriter)writerProvider.GetTextWriter(stream);

        // When
        writer.PushEmphasis();
        writer.Write("Hello, World!");
        writer.PopEmphasis();

        writer.Flush();
        stream.Position = 0;

        using StreamReader reader = new(stream);
        string result = reader.ReadToEnd();

        // Then
        Assert.Equal("*Hello, World!*", result);
    }

    [Fact]
    public void PushStrongDefault_DoubleAsterisk()
    {
        // Given
        using IHost host = GetHost();
        ITextWriterProvider writerProvider = host.Services.GetRequiredService<ITextWriterProvider>();
        using MemoryStream stream = new();
        using MarkdownStreamWriter writer = (MarkdownStreamWriter)writerProvider.GetTextWriter(stream);

        // When
        writer.PushStrong();
        writer.Write("Hello, World!");
        writer.PopStrong();

        writer.Flush();
        stream.Position = 0;

        using StreamReader reader = new(stream);
        string result = reader.ReadToEnd();

        // Then
        Assert.Equal("**Hello, World!**", result);
    }

    [Fact]
    public void PushStrongEmphasisDefault_TripleAsterisk()
    {
        // Given
        using IHost host = GetHost();
        ITextWriterProvider writerProvider = host.Services.GetRequiredService<ITextWriterProvider>();
        using MemoryStream stream = new();
        using MarkdownStreamWriter writer = (MarkdownStreamWriter)writerProvider.GetTextWriter(stream);

        // When
        writer.PushStrong();
        writer.PushEmphasis();
        writer.WriteLine("Hello, ");
        writer.PopEmphasis();
        writer.Write("World!");
        writer.PopStrong();

        writer.Flush();
        stream.Position = 0;

        using StreamReader reader = new(stream);
        string result = reader.ReadToEnd();

        // Then
        Assert.Equal("***Hello, \n*World!**", result);
    }

    [Fact]
    public void PushStrongEmphasisAlternate_DoubleAsteriskSingleUnderScore()
    {
        // Given
        using IHost host = GetHost(new MarkdownStyle() { PreferAlternateStyle = true });
        ITextWriterProvider writerProvider = host.Services.GetRequiredService<ITextWriterProvider>();
        using MemoryStream stream = new();
        using MarkdownStreamWriter writer = (MarkdownStreamWriter)writerProvider.GetTextWriter(stream);

        // When
        writer.PushStrong();
        writer.PushEmphasis();
        writer.WriteLine("Hello, ");
        writer.Indent();
        writer.PopEmphasis();
        writer.Write("World!");
        writer.PopStrong();

        writer.Flush();
        stream.Position = 0;

        using StreamReader reader = new(stream);
        string result = reader.ReadToEnd();

        // Then
        Assert.Equal("**_Hello, \n  _World!**", result);
    }

    [Fact]
    public void PushStrongEmphasisUnderscoresAlternate_SingleUnderscoreDoubleAsterisk()
    {
        // Given
        using IHost host = GetHost(new MarkdownStyle() { PreferAlternateStyle = true, EmphasisCharacter = StyleCharacter.Underscore, StrongCharacter = StyleCharacter.Underscore });
        ITextWriterProvider writerProvider = host.Services.GetRequiredService<ITextWriterProvider>();
        using MemoryStream stream = new();
        using MarkdownStreamWriter writer = (MarkdownStreamWriter)writerProvider.GetTextWriter(stream);

        // When
        writer.PushEmphasis();
        writer.WriteLine("Hello, ");
        writer.Indent();
        writer.PushStrong();
        writer.Write("World!");
        writer.PopStrong();
        writer.PopEmphasis();

        writer.Flush();
        stream.Position = 0;

        using StreamReader reader = new(stream);
        string result = reader.ReadToEnd();

        // Then
        Assert.Equal("_Hello, \n  **World!**_", result);
    }
}
