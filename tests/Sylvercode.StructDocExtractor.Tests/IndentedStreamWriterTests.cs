using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests;

public class IndentedStreamWriterTests_Write
{

    private static IHost GetHost(IndentSpec? indentSpec = null)
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

    [Fact]
    public void MultiLineNoIndentation_NoIndent()
    {
        // Given
        using IHost host = GetHost();
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
        using IHost host = GetHost();
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
        using IHost host = GetHost(new IndentSpec { Type = IndentType.Tab });
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
        using IHost host = GetHost(new IndentSpec { Size = 5 });
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

}
