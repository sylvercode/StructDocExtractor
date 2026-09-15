using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.StructDocExtractor.AngleSharp;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Markdown.Serialization;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Markdown.Tests;

public class BodyToMarkdownRegressionTests
{
    private static IHost GetSerializerHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddMarkdownStreamWriterProvider();
                services.AddStructDocSerializer();
                services.ConfigureSerializer(new CalloutSerializer(new Dictionary<Type, CalloutSerializer.CalloutType>
                {
                    { typeof(HtmlAside), CalloutSerializer.CalloutType.quote }
                }));
                services.AddSerializer<HeadingSerializer>();
                services.AddSerializer<ParagraphSerializer>();
                services.AddSerializer<PlainTextSerializer>();
                services.AddSerializer<StrongSerializer>();
                services.AddSerializer<EmphasesSerializer>();
                services.AddSerializer<AnchorSerializer>();
                services.AddSingleton<StringTextWriter<MarkdownStreamWriter>>();
            }).Build();
    }

    private static string GetFixturePath(string fileName)
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);
        while (current is not null)
        {
            string solutionFile = Path.Combine(current.FullName, "StructDocExtractor.sln");
            if (File.Exists(solutionFile))
                return Path.Combine(current.FullName, "tests", "Sylvercode.StructDocExtractor.Markdown.Tests", "files", fileName);

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate StructDocExtractor repository root from test runtime directory.");
    }

    private static IElement ParseBodyFromFixture(string fileName)
    {
        string html = File.ReadAllText(GetFixturePath(fileName));
        HtmlParser parser = new();
        var document = parser.ParseDocument(html);

        return document.Body ?? throw new InvalidOperationException("Fixture does not contain a body element.");
    }

    [Fact]
    public void BodyFixture_SerializesMarkdown_HeadingIsNotInsideBlockquote()
    {
        // Given
        IElement body = ParseBodyFromFixture("test1.html");
        Extractor<INode, HtmlNodeDiscriminator> extractor = new(
            new AngleSharpNodeFactoryProvider(selector: null),
            dataDiscriminatorFactory: new HtmlNodeDiscriminatorFactory());

        IHost host = GetSerializerHost();
        IStructDocSerializer serializer = host.Services.GetRequiredService<IStructDocSerializer>();
        StringTextWriter<MarkdownStreamWriter> textWriter = host.Services.GetRequiredService<StringTextWriter<MarkdownStreamWriter>>();

        // When
        List<IStructDocNode> topNodes = [];
        foreach (INode bodyChild in body.ChildNodes)
        {
            ExtractionResult extraction = extractor.Extract(bodyChild);
            topNodes.AddRange(extraction.StructDocNodes);
        }

        Assert.NotEmpty(topNodes);

        foreach (IStructDocNode node in topNodes)
        {
            serializer.Serialize(textWriter.Writer, node);
        }

        string markdown = textWriter.GetResult();
        Console.WriteLine(markdown);

        // Then
        Assert.Contains("Lorem Ipsum", markdown);
        Assert.DoesNotContain("\n> ### Lorem Ipsum", markdown);
        Assert.Contains("\n\n### Lorem Ipsum", markdown);
    }
}
