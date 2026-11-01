using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class CalloutSerializerTests
{
    private static IHost GetHost(
    IReadOnlyDictionary<Type, CalloutSerializer.CalloutType> calloutTypeMap)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.ConfigureSerializer(new CalloutSerializer(calloutTypeMap: calloutTypeMap));
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    [Theory]
    [InlineData("", "> [!info]\n> Hello\n> \n> World")]
    [InlineData("37066f", "> [!info] Info\n> Hello\n> \n> World\n\n^37066f")]
    public void SerializeText(string id, string expectedCallout)
    {
        // Given
        IHost host = GetHost(new Dictionary<Type, CalloutSerializer.CalloutType>
        {
            { typeof(HtmlParagraph), CalloutSerializer.CalloutType.info }
        });
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> stringTextWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();
        string aLongTimeAgo = "A long time ago...";
        stringTextWriter.Writer.Write(aLongTimeAgo);

        HtmlParagraph paragraph = new() { Id = id };
        paragraph.InitWithContent(c =>
        {
            c.Add(new PlainTextNode("Hello\n\nWorld"));
        });
        HtmlDiv mainDiv = new();
        mainDiv.MakeARoot();
        mainDiv.InitWithContent(c =>
        {
            c.Add(paragraph);
        });

        // When
        serializer.Serialize(stringTextWriter.Writer, mainDiv);

        // Then
        Assert.Equal(IndentedStreamWriter.SpaceOperationType.Paragraph, stringTextWriter.Writer.PendingOperation);
        stringTextWriter.Writer.Write("Following paragraph.");
        Assert.Equal($"{aLongTimeAgo}\n\n{expectedCallout}\n\nFollowing paragraph.", stringTextWriter.GetResult());
    }
}
