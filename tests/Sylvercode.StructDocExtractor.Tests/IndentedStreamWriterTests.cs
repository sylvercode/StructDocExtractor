using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests;

public static class IndentedStreamWriterTestsSetup
{
    public static IHost GetHost(IndentSpec? indentSpec = null)
    {
        return Host.CreateDefaultBuilder()
                   .ConfigureServices((_, services) =>
                   {
                       services.AddIndentedStreamWriterProvider(indentSpec is null
                            ? null :
                            (options) =>
                            {
                                options.IndentSpec = indentSpec;
                            });
                   })
                   .Build();
    }
}

public class IndentedStreamWriterTests_Write
{

    [Fact]
    public void MultiLineNoIndentation_NoIndent()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = provider.GetTextWriter(stream);

        // When
        writer.Write("Hello, \nWorld!");
        writer.Write("!");
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine("Goodbye, ");
        writer.WriteLine("My friend!");
        writer.Flush();

        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal("Hello, \nWorld!!\n\nGoodbye, \nMy friend!\n",
                     reader.ReadToEnd());
    }

    [Fact]
    public void MultiLineWithDefaultSpaceIndentation()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);

        // When
        writer.Indent();
        writer.Write("Hello, \nWorld!");
        writer.Write("!");
        writer.WriteLine();
        writer.WriteLine();
        writer.Unindent();
        writer.WriteLine("Goodbye, ");
        writer.WriteLine("My friend!");
        writer.Flush();

        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal("    Hello, \n    World!!\n\nGoodbye, \nMy friend!\n",
                     reader.ReadToEnd());
    }

    [Fact]
    public void MultiLineWithTabsIndentation()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost(new IndentSpec { Type = IndentType.Tab });
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);

        // When
        writer.Indent();
        writer.Write("Hello, \nWorld!");
        writer.Write("!");
        writer.WriteLine();
        writer.WriteLine();
        writer.Unindent();
        writer.WriteLine("Goodbye, ");
        writer.WriteLine("My friend!");
        writer.Flush();

        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal("\tHello, \n\tWorld!!\n\nGoodbye, \nMy friend!\n",
                     reader.ReadToEnd());
    }

    [Fact]
    public void MultiLineWithDouble5SpacesIndentation()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost(new IndentSpec { Size = 5 });
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);

        // When
        writer.Indent(2);
        writer.Write("Hello, \nWorld!");
        writer.Write("!");
        writer.WriteLine();
        writer.WriteLine();
        writer.Unindent();
        writer.WriteLine("Goodbye, ");
        writer.WriteLine("My friend!");
        writer.Flush();

        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal("          Hello, \n          World!!\n\n     Goodbye, \n     My friend!\n",
                     reader.ReadToEnd());
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("", true)]
    [InlineData("Hello\n", false)]
    [InlineData("Hello\n", true)]
    public void NoSpaceAddedToLineStart(string message, bool withIndent)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.Write(" ");
        writer.Flush();

        // Then
        string indent = withIndent && message.Length != 0 ? "    " : "";
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal(indent + message, reader.ReadToEnd());
    }

}

public class IndentedStreamWriterTests_StartLine
{
    [Theory]
    [InlineData("Hello,", false)]
    [InlineData("Hello,", true)]
    [InlineData("Hello,\n", false)]
    [InlineData("Hello,\n", true)]
    public void NoLineAtStartOrOne_OnlyOneLineAdded(string message, bool withIndent)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartLine();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal($"{indent}Hello,\n{indent}World!", reader.ReadToEnd());
    }

    [Theory]
    [InlineData("Hello,\n\n", false)]
    [InlineData("Hello,\n\n", true)]
    [InlineData("Hello,\n\n\n", false)]
    [InlineData("Hello,\n\n\n", true)]
    public void MoreThanOneLineAtStart_NoneAdded(string message, bool withIndent)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartLine();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal($"{indent}{message}{indent}World!", reader.ReadToEnd());
    }
}

public class IndentedStreamWriterTests_StartParagraph
{
    [Theory]
    [InlineData("Hello,", false)]
    [InlineData("Hello,", true)]
    [InlineData("Hello,\n", false)]
    [InlineData("Hello,\n", true)]
    [InlineData("Hello,\n\n", false)]
    [InlineData("Hello,\n\n", true)]
    public void LessThanThreeLine_ResultOnePargraph(string message, bool withIndent)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartParagraph();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal($"{indent}Hello,\n\n{indent}World!", reader.ReadToEnd());
    }

    [Theory]
    [InlineData("Hello,\n\n\n", false)]
    [InlineData("Hello,\n\n\n", true)]
    [InlineData("Hello,\n\n\n\n", false)]
    [InlineData("Hello,\n\n\n\n", true)]
    public void LessThanThreeLine_NoneAdded(string message, bool withIndent)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartParagraph();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal($"{indent}{message}{indent}World!", reader.ReadToEnd());
    }
}

public class IndentedStreamWriterTests_StartWord
{
    [Fact]
    public void SpaceNeeded_ResultSpaceAdded()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);

        // When
        writer.StartWord();
        writer.Write("Hello,");
        writer.StartWord();
        writer.Write("World!");
        writer.Flush();

        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal($"Hello, World!", reader.ReadToEnd());
    }

    [Theory]
    [InlineData("Hello,\n")]
    [InlineData("Hello, ")]
    [InlineData("Hello,  ")]
    public void NoSpaceNeeded_NoneAdded(string message)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        var provider = host.Services.GetRequiredService<ITextWriterProvider>();
        using var stream = new MemoryStream();
        using var writer = (IndentedStreamWriter)provider.GetTextWriter(stream);

        // When
        writer.Write(message);
        writer.StartWord();
        writer.Write("World!");
        writer.Flush();

        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        Assert.Equal($"{message}World!", reader.ReadToEnd());
    }
}
