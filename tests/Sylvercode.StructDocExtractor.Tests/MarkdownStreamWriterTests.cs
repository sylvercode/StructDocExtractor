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
                       services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
                   })
                   .Build();
    }

    [Fact]
    public void PushEmphasisDefault_SingleAsterisk()
    {
        // Given
        using IHost host = GetHost();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        MarkdownStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.PushEmphasis();
        writer.Write("Hello, World!");
        writer.PopEmphasis();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("*Hello, World!*", result);
    }

    [Fact]
    public void PushStrongDefault_DoubleAsterisk()
    {
        // Given
        using IHost host = GetHost();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        MarkdownStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.PushStrong();
        writer.Write("Hello, World!");
        writer.PopStrong();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("**Hello, World!**", result);
    }

    [Fact]
    public void PushStrongEmphasisDefault_TripleAsterisk()
    {
        // Given
        using IHost host = GetHost();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        MarkdownStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.PushStrong();
        writer.PushEmphasis();
        writer.WriteLine("Hello, ");
        writer.PopEmphasis();
        writer.Write("World!");
        writer.PopStrong();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("***Hello, \n*World!**", result);
    }

    [Fact]
    public void PushStrongEmphasisAlternate_DoubleAsteriskSingleUnderScore()
    {
        // Given
        using IHost host = GetHost(new MarkdownStyle() { PreferAlternateStyle = true });
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        MarkdownStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.PushStrong();
        writer.PushEmphasis();
        writer.WriteLine("Hello, ");
        writer.Indent();
        writer.PopEmphasis();
        writer.Write("World!");
        writer.PopStrong();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("**_Hello, \n  _World!**", result);
    }

    [Fact]
    public void PushStrongEmphasisUnderscoresAlternate_SingleUnderscoreDoubleAsterisk()
    {
        // Given
        using IHost host = GetHost(new MarkdownStyle() { PreferAlternateStyle = true, EmphasisCharacter = StyleCharacter.Underscore, StrongCharacter = StyleCharacter.Underscore });
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        MarkdownStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.PushEmphasis();
        writer.WriteLine("Hello, ");
        writer.Indent();
        writer.PushStrong();
        writer.Write("World!");
        writer.PopStrong();
        writer.PopEmphasis();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("_Hello, \n  **World!**_", result);
    }
}
