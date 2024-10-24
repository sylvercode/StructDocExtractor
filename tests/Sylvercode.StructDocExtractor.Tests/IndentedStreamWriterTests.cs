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
                       services.AddSingleton<StringTextWriter<IndentedStreamWriter>>();
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
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.Write("Hello, \nWorld!");
        writer.Write("!");
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine("Goodbye, ");
        writer.WriteLine("My friend!");
        writer.Flush();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("Hello, \nWorld!!\n\nGoodbye, \nMy friend!\n",
                     result);
    }

    [Fact]
    public void MultiLineWithDefaultSpaceIndentation()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;

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
        string result = stringTextWriter.GetResult();
        Assert.Equal("    Hello, \n    World!!\n\nGoodbye, \nMy friend!\n",
                     result);
    }

    [Fact]
    public void MultiLineWithTabsIndentation()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost(new IndentSpec { Type = IndentType.Tab });
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;

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
        string result = stringTextWriter.GetResult();
        Assert.Equal("\tHello, \n\tWorld!!\n\nGoodbye, \nMy friend!\n",
                     result);
    }

    [Fact]
    public void MultiLineWithDouble5SpacesIndentation()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost(new IndentSpec { Size = 5 });
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;

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
        string result = stringTextWriter.GetResult();
        Assert.Equal("          Hello, \n          World!!\n\n     Goodbye, \n     My friend!\n",
                     result);
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
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.Write(" ");
        writer.Flush();

        // Then
        string indent = withIndent && message.Length != 0 ? "    " : "";
        string result = stringTextWriter.GetResult();
        Assert.Equal(indent + message, result);
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
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartLine();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        string result = stringTextWriter.GetResult();
        Assert.Equal($"{indent}Hello,\n{indent}World!", result);
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
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartLine();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        string result = stringTextWriter.GetResult();
        Assert.Equal($"{indent}{message}{indent}World!", result);
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
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartParagraph();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        string result = stringTextWriter.GetResult();
        Assert.Equal($"{indent}Hello,\n\n{indent}World!", result);
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
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        if (withIndent)
            writer.Indent();

        // When
        writer.Write(message);
        writer.StartParagraph();
        writer.Write("World!");
        writer.Flush();

        // Then
        string indent = withIndent ? "    " : "";
        string result = stringTextWriter.GetResult();
        Assert.Equal($"{indent}{message}{indent}World!", result);
    }
}

public class IndentedStreamWriterTests_StartWord
{
    [Fact]
    public void SpaceNeeded_ResultSpaceAdded()
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.StartWord();
        writer.Write("Hello,");
        writer.StartWord();
        writer.Write("World!");
        writer.Flush();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal($"Hello, World!", result);
    }

    [Theory]
    [InlineData("Hello,\n")]
    [InlineData("Hello, ")]
    [InlineData("Hello,  ")]
    public void NoSpaceNeeded_NoneAdded(string message)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;

        // When
        writer.Write(message);
        writer.StartWord();
        writer.Write("World!");
        writer.Flush();

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal($"{message}World!", result);
    }
}

public class IndentedStreamWriterTests_EnsureEndXNext
{
    public enum OperationType
    {
        Word,
        Line,
        Paragraph,
    }
    private static void EnsureEndXNextAndLess(IndentedStreamWriter writer, OperationType operation)
    {
        if (operation >= OperationType.Word)
            writer.EnsureEndWordNext();
        if (operation >= OperationType.Line)
            writer.EnsureEndLineNext();
        if (operation >= OperationType.Paragraph)
            writer.EnsureEndParagraphNext();
    }

    [Theory]
    [InlineData(OperationType.Word, "Hello, World!")]
    [InlineData(OperationType.Line, "Hello,\nWorld!")]
    [InlineData(OperationType.Paragraph, "Hello,\n\nWorld!")]
    public void WordFollowedByText(OperationType operation, string expected)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        writer.Write("Hello,");

        // When
        EnsureEndXNextAndLess(writer, operation);
        EnsureEndXNextAndLess(writer, operation);
        writer.Write("World!");

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(OperationType.Word)]
    [InlineData(OperationType.Line)]
    [InlineData(OperationType.Paragraph)]
    public void WordFollowedByNothing(OperationType operation)
    {
        // Given
        using IHost host = IndentedStreamWriterTestsSetup.GetHost();
        StringTextWriter<IndentedStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<IndentedStreamWriter>>();
        IndentedStreamWriter writer = stringTextWriter.Writer;
        writer.Write("Hello,");

        // When
        EnsureEndXNextAndLess(writer, operation);
        EnsureEndXNextAndLess(writer, operation);

        // Then
        string result = stringTextWriter.GetResult();
        Assert.Equal("Hello,", result);
    }

}
