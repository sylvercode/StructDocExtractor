Build, test, and lint

- Build solution: dotnet build StructDocExtractor.sln
- Build single project: dotnet build src\Path\ToProject.csproj
- Run all tests: dotnet test StructDocExtractor.sln
- Run a single project tests: dotnet test tests\<Project>.Tests\<Project>.Tests.csproj
- Run a single test by fully-qualified name: dotnet test --filter "FullyQualifiedName=MyNamespace.MyTests.MyTestMethod"
- Run a single test by method name: dotnet test --filter "Name=MyTestMethod"
- Collect code coverage (coverlet): dotnet test --collect:"XPlat Code Coverage"
- Format / basic linting: use .editorconfig + compiler analyzers (AnalysisMode=Recommended). Optional: install dotnet-format and run: dotnet tool install -g dotnet-format && dotnet format StructDocExtractor.sln

VS Code tasks

- Predefined tasks: build/test/publish/watch in .vscode/tasks.json — run via Terminal → Run Task or Ctrl+Shift+B for build.

High-level architecture

- Solution: StructDocExtractor.sln contains core libraries and test projects.
- Core libraries:
  - src/Sylvercode.StructDocExtractor: core extraction model, serialization, StructDoc node model and serializer providers.
  - src/Sylvercode.SiteExtractor: site downloading, resource tracking, stores, and abstractions for site sources and processors.
- Implementations:
  - *AngleSharp* projects implement HTML parsing via AngleSharp.
  - *Markdown* projects implement Markdown-specific serializers/serializers and referencer updaters.
  - *StdHtml* provides a standard HTML model and related scoring/stacking.
- Patterns:
  - Factory/Provider/Extractor interfaces (I*): open for DI and extension (IStructDocNodeFactory, IResourceProcessor, IExtractor, ISerializerProvider).
  - Serialization pipeline implemented via serializer tasks/executors.
  - Scoring/Stack: StructDataStack contains stack-score calculators and matchers used to choose structural data nodes.
- Target framework: net8.0. Projects enable ImplicitUsings and Nullable.

Key conventions

- File-scoped namespaces are preferred (see .editorconfig: csharp_style_namespace_declarations = file_scoped).
- Project names and namespaces mirror folder structure: src/<ProjectName>/<ProjectName>.csproj and corresponding namespaces.
- Analyzer config: projects set <AnalysisMode>Recommended</AnalysisMode> so compiler diagnostics are useful; treat warnings as guidance.
- Naming patterns: *Provider, *Factory, *Extractor, *Serializer, *Options, *Extensions are common and imply extension points.
- Tests: xUnit + Microsoft.NET.Test.Sdk are used across tests; test projects set <IsTestProject>true</IsTestProject>.

Other AI assistant configs

- No CLAUDE.md, AGENTS.md, AIDER_CONVENTIONS.md, .cursorrules, .clinerules, or similar files were found.

Notes for Copilot sessions

- Start by opening the solution file (StructDocExtractor.sln) and tests/ to run a focused failing test when changing behavior.
- Prefer reading provider/factory interfaces first to understand extension points before diving into concrete implementations.
