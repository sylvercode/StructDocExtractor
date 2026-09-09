# StructDocExtractor

StructDocExtractor is a .NET 8 framework for extracting structured content from websites and
serializing it into another format such as Markdown. It converts source documents (HTML/DOM)
into a strongly‑typed node graph, then walks that graph to produce clean output — and can crawl
an entire site, downloading assets and rewriting links along the way.

- **New here?** Read [ARCHITECTURE.md](./ARCHITECTURE.md) for the concepts and diagrams.
- **Looking for a specific type?** The [ARCHITECTURE.md appendix](./ARCHITECTURE.md#appendix--type-reference-by-namespace) maps every namespace to its notable types.

This README is a hands‑on guide to **using and extending the framework**. Everything below uses
**hypothetical** examples (no real sites). Two paths are covered:

1. **Generic HTML → Markdown** — works out of the box with the built‑in integrations; you
   write almost no code.
2. **Domain‑specific customization** — add your own node types, factories, and serializers
   only when you need semantic structure the built‑ins cannot express.

---

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) or later.

## Build & test

```bash
# Build the whole framework
dotnet build StructDocExtractor.sln

# Run all tests
dotnet test StructDocExtractor.sln

# Build or test a single project
dotnet build src/Sylvercode.StructDocExtractor/Sylvercode.StructDocExtractor.csproj
dotnet test  tests/Sylvercode.StructDocExtractor.Tests/Sylvercode.StructDocExtractor.Tests.csproj

# Run a single test by fully‑qualified name
dotnet test --filter "FullyQualifiedName=MyNamespace.MyTests.MyTestMethod"
```

---

## Built‑in HTML → Markdown pipeline

The framework ships everything needed to convert standard HTML to Markdown without writing a
single custom node or serializer:

| Layer | Package | What it provides |
|---|---|---|
| **Node model** | `StructDocExtractor.StdHtml` | Typed nodes for every common HTML element — headings, paragraphs, lists, tables, anchors, images, aside, figure, emphasis, strong, … |
| **DOM → nodes** | `StructDocExtractor.AngleSharp` | One factory per HTML element kind (plus `AngleSharpNodeFactoryProvider` that registers them all with `addDefaultFactory: true`). |
| **Nodes → Markdown** | `StructDocExtractor.Markdown` | One Markdown serializer per StdHtml node type. `AddMarkdownSerializers()` registers them all. |
| **Crawl + rewrite** | `SiteExtractor.AngleSharp` + `SiteExtractor.Markdown` | `AddAngleSharpSiteExtractor()` + `AddMarkdownSerialization()` wire up the full download, asset‑copy, and link‑rewrite pipeline. |

To scope extraction to a region of the page, subclass `AngleSharpNodeFactoryProvider` with a
CSS selector. `addDefaultFactory: true` (the default) registers all built‑in HTML factories
automatically:

```csharp
// The only custom class you need for a generic scrape.
// All standard HTML factories are included via addDefaultFactory: true (the default).
public class ArticleProvider() : AngleSharpNodeFactoryProvider(".article-body") { }
```

That one class, together with the DI chain below, is a fully working HTML → Markdown extractor.

---

## Quick start — generic HTML to Markdown

### CLI project references

```xml
<ItemGroup>
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.SiteExtractor\Sylvercode.SiteExtractor.csproj" />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.SiteExtractor.AngleSharp\Sylvercode.SiteExtractor.AngleSharp.csproj" />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.SiteExtractor.Markdown\Sylvercode.SiteExtractor.Markdown.csproj" />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.StructDocExtractor\Sylvercode.StructDocExtractor.csproj" />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.StructDocExtractor.AngleSharp\Sylvercode.StructDocExtractor.AngleSharp.csproj" />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.StructDocExtractor.Markdown\Sylvercode.StructDocExtractor.Markdown.csproj" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
</ItemGroup>
```

### `Program.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor;

IHostBuilder builder = Host.CreateDefaultBuilder();

builder.ConfigureAppConfiguration((ctx, config) =>
{
    config.AddCommandLine(args, new Dictionary<string, string>
    {
        ["-output"]   = $"{SiteExtractorOptions.SiteExtractor}:{nameof(SiteExtractorOptions.OutputDirectory)}",
        ["-sitePath"] = $"{SiteExtractorOptions.SiteExtractor}:{nameof(SiteExtractorOptions.SourceBasePath)}",
    });
    config.AddEnvironmentVariables("MYAPP_");
});

builder.ConfigureServices((ctx, services) =>
{
    services.AddOptions<SiteExtractorOptions>()
            .Bind(ctx.Configuration.GetSection(SiteExtractorOptions.SiteExtractor));

    services.AddHostedService<Worker>()
            .AddAngleSharpSiteExtractor()   // web source + HTML filter + asset downloader
            .AddAngleExtractorFor<ArticleProvider>()  // scope + all built-in HTML factories
            .AddMarkdownSerializers()        // built-in Markdown serializers for every StdHtml node
            .AddMarkdownSerialization()      // post-extraction link rewriting
            .AddDirectoryDataStore();        // write .md files to disk
});

IHost host = builder.Build();
host.Run();
```

### `Worker.cs`

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor;

public class Worker(
    IHostApplicationLifetime host,
    IOptions<SiteExtractorOptions> options,
    ISiteExtractor siteExtractor) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        siteExtractor.Extract(options.Value.GetSourceBaseUri());
        host.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

### `appsettings.json`

```json
{
  "SiteExtractor": {
    "SourceAuthority": "https://example.com",
    "SourceBasePath": "/articles",
    "OutputDirectory": "./output"
  }
}
```

### Run

```bash
dotnet run -- -sitePath /articles -output ./output
```

That is the complete setup. No custom nodes, no custom factories, no custom serializers.

---

## Domain‑specific customization

The three moving parts below are **only needed when the built‑in HTML model is not expressive
enough** for your use case — for example, when you want a typed `RecipeRoot` node so your
serializer can emit a custom frontmatter header, or when a specific aside class should become
a Markdown callout block rather than a generic aside.

### The three moving parts

| Concern | You write… | Base type |
|---|---|---|
| **Model** — the semantic shape | Domain node classes | `BaseStructDocRootBlock<T>`, `BaseStructDocBlock<TParent,TChild>`, `BaseStructDocNode<TParent>` |
| **Extractor** — how to recognise it | Node factories + a `AngleSharpNodeFactoryProvider` subclass | `BaseAngleNodeFactory`, `AngleSharpNodeFactoryProvider` |
| **Serializer** — how to write it | One `BaseMarkdownSerializer<T>` per node type | `BaseMarkdownSerializer<T>` |

A typical domain library layout:

```
Acme.DocExtractor/
├── Model/          # domain node classes
├── Extractor/      # factories + factory provider + AddAcmeExtractor()
└── Serialization/  # serializers + AddAcmeSerializer()
```

### Defining domain Model nodes

Model nodes encode **parent/child type constraints** through the generic base classes, so the
tree is type‑safe at compile time.

```csharp
// Model/RecipeRoot.cs
using Sylvercode.StructDocExtractor.Model.Base;

namespace Acme.DocExtractor.Model;

public class RecipeRoot(string title) : BaseStructDocRootBlock<RecipeSection>(id: string.Empty)
{
    public string Title { get; } = title;
}
```

```csharp
// Model/RecipeSection.cs
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Acme.DocExtractor.Model;

// Implement IStructDocReferencer so the site pipeline rewrites its href.
public class RecipeSection(string text, string href)
    : BaseStructDocBlock<RecipeRoot, RecipeEntry>(id: ""), IStructDocReferencer
{
    public string Text { get; } = text;
    public string Href { get; private set; } = href;

    public IStructDocReferencer.ReferenceType GetReferenceType()
        => IStructDocReferencer.ReferenceType.External;
    public string GetReference() => Href;
    public void UpdateReference(string newReference) => Href = newReference;
}
```

You can also subclass an existing StdHtml node (e.g. extend `HtmlAside` for a typed callout)
without defining a full tree.

### Writing a factory and factory provider

A **factory** matches a source element via a score selector and builds a node. Override
`BuildFromElement` and set `DefaultSelector` using `HtmlScoreCriteriaSetsBuilder`.

```csharp
// Extractor/RecipeRootNodeFactory.cs
using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;
using Sylvercode.StructDocExtractor.StructDataStack.Score;
using Acme.DocExtractor.Model;

namespace Acme.DocExtractor.Extractor;

public class RecipeRootNodeFactory : BaseAngleNodeFactory
{
    public const string StyleClass = "recipe-root";

    public static List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>> Selector { get; } =
        new HtmlScoreCriteriaSetsBuilder().WithStyleClass(StyleClass).BuildSets();

    public override List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector => Selector;

    protected override bool BuildFromElement(AngleProcessTaskResultBuilder resultBuilder, IElement element)
    {
        string title = element.QuerySelector("h1")?.TextContent.Trim() ?? string.Empty;
        resultBuilder.WithNode(new RecipeRoot(title));
        resultBuilder.WithSubTaskByAll($".{RecipeSectionNodeFactory.StyleClass}");
        return true;
    }
}
```

A **factory provider** groups factories for a page region. Passing `addDefaultFactory: false`
keeps only your custom factories; use `true` (or omit it) to also include all built‑in HTML
factories alongside yours.

```csharp
// Extractor/RecipeNodeFactoryProvider.cs
using Sylvercode.StructDocExtractor.AngleSharp;

namespace Acme.DocExtractor.Extractor;

public class RecipeNodeFactoryProvider : AngleSharpNodeFactoryProvider
{
    // addDefaultFactory: false — only our custom factories; the rest of the page
    // is handled by a second ArticleProvider registered via AddAngleExtractorFor.
    public RecipeNodeFactoryProvider()
        : base($".{RecipeRootNodeFactory.StyleClass}", addDefaultFactory: false)
    {
        AddFactory(new RecipeRootNodeFactory());
        AddFactory(new RecipeSectionNodeFactory());
        AddFactory(new RecipeEntryNodeFactory());
    }
}
```

### Writing serializers

One serializer per custom node type, extending `BaseMarkdownSerializer<TNode>`:

```csharp
// Serialization/RecipeRootNodeSerializer.cs
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Base;
using Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;
using Sylvercode.StructDocExtractor.Serialization;
using Acme.DocExtractor.Model;

namespace Acme.DocExtractor.Serialization;

public class RecipeRootNodeSerializer(ILogger<RecipeRootNodeSerializer>? logger = null)
    : BaseMarkdownSerializer<RecipeRoot>(handler: new RootHandler(), logger: logger)
{
    private sealed class RootHandler()
        : IndentedSerializerHandler<RecipeRoot, MarkdownStreamWriter>(
            new Options(IndentedStreamWriter.SpaceOperationType.Paragraph))
    {
        public override void Serialize(RecipeRoot obj, MarkdownStreamWriter stream, NodeSerializationResult result)
        {
            stream.WriteLine($"# {obj.Title}");
        }
    }
}
```

### DI extension methods

Expose two extension methods (by convention in the `Microsoft.Extensions.DependencyInjection`
namespace) so the CLI can wire everything up:

```csharp
// Extractor/AcmeExtractorExtensions.cs
namespace Microsoft.Extensions.DependencyInjection;
using Acme.DocExtractor.Extractor;

public static class AcmeExtractorExtensions
{
    public static IServiceCollection AddAcmeExtractor(this IServiceCollection services)
    {
        services.AddAngleExtractorFor<RecipeNodeFactoryProvider>();
        return services;
    }
}
```

```csharp
// Serialization/AcmeSerializerExtensions.cs
namespace Microsoft.Extensions.DependencyInjection;
using Acme.DocExtractor.Model;
using Acme.DocExtractor.Serialization;

public static class AcmeSerializerExtensions
{
    public static IServiceCollection AddAcmeSerializer(this IServiceCollection services)
    {
        services.AddMarkdownSerializers();                                         // built-ins
        services.AddSerializer<RecipeRoot, RecipeRootNodeSerializer>();           // custom
        services.AddSerializer<RecipeSection, RecipeSectionNodeSerializer>();
        services.AddSerializer<RecipeEntry, RecipeEntryNodeSerializer>();
        return services;
    }
}
```

### Wiring the customized CLI

The consumer library references the same framework packages, plus its own project:

```xml
<!-- Acme.DocExtractor.csproj (library) -->
<ItemGroup>
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.StructDocExtractor\..." />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.StructDocExtractor.StdHtml\..." />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.StructDocExtractor.AngleSharp\..." />
  <ProjectReference Include="..\..\references\StructDocExtractor\src\Sylvercode.SiteExtractor.Markdown\..." />
</ItemGroup>
```

In `Program.cs`, replace the generic registrations with your domain ones:

```csharp
services.AddHostedService<Worker>()
        .AddAngleSharpSiteExtractor()
        .AddAcmeExtractor()          // replaces AddAngleExtractorFor<ArticleProvider>()
        .AddMarkdownSerialization()
        .AddAcmeSerializer()         // replaces AddMarkdownSerializers()
        .AddDirectoryDataStore();
```

---

## Configuration precedence

From highest to lowest priority:

1. Command‑line arguments (mapped in `Program.cs`)
2. Environment variables (prefixed, e.g. `MYAPP_`)
3. `appsettings.{Environment}.json`
4. `appsettings.json`
5. Option defaults in code

---

## Where to go next

- **[ARCHITECTURE.md](./ARCHITECTURE.md)** — the design, pipelines, and diagrams behind the
  APIs used above, including a per‑namespace type reference in its appendix.
