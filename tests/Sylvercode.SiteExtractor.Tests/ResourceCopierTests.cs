
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceCopierTests_Download
{
    [Fact]
    public void ExistingToAbsolutePath_Copied()
    {
        // Given
        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        var siteExtractorOption = Options.Create(new SiteExtractorOptions()
        {
            SourceAuthority = "memory://example.com",
            SourceBasePath = "input",
            OutputDirectory = "memory://output"
        });
        MemorySiteSource<byte[]> siteSource = new(baseUri: siteExtractorOption.Value.GetSourceBaseUri())
        {
            { sourceUri, [1, 2, 3] }
        };
        MemoryDataStore dataStore = new(siteExtractorOption);

        var resourceCopierOptions = Options.Create(new ResourceCopierOptions()
        {
            IsOutputPathAbsolute = true,
            OutputPath = "store"
        });
        ResourceCopier copier = new(siteSource, dataStore, resourceCopierOptions);

        // When
        copier.Download(sourceUri);

        // Then
        MemoryStream result = Assert.Contains(new Uri("memory://output/store/data1.bin"), dataStore);
        Assert.Equal([1, 2, 3], result.ToArray());
    }

    [Fact]
    public void ExistingToRelativePath_Copied()
    {
        // Given
        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        var siteExtractorOption = Options.Create(new SiteExtractorOptions()
        {
            SourceAuthority = "memory://example.com",
            SourceBasePath = "input",
            OutputDirectory = "memory://output"
        });
        MemorySiteSource<byte[]> siteSource = new(baseUri: siteExtractorOption.Value.GetSourceBaseUri())
        {
            { sourceUri, [1, 2, 3] }
        };
        MemoryDataStore dataStore = new(siteExtractorOption);

        var resourceCopierOptions = Options.Create(new ResourceCopierOptions()
        {
            IsOutputPathAbsolute = false,
            OutputPath = "store"
        });
        ResourceCopier copier = new(siteSource, dataStore, resourceCopierOptions);

        // When
        copier.Download(sourceUri);

        // Then
        MemoryStream result = Assert.Contains(new Uri("memory://output/dir/data1.bin"), dataStore);
        Assert.Equal([1, 2, 3], result.ToArray());
    }

    [Fact]
    public void NotExisting_Throws()
    {
        // Given
        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        var siteExtractorOption = Options.Create(new SiteExtractorOptions()
        {
            SourceAuthority = "memory://example.com",
            SourceBasePath = "input",
            OutputDirectory = "memory://output"
        });
        MemorySiteSource<byte[]> siteSource = new(baseUri: siteExtractorOption.Value.GetSourceBaseUri());
        MemoryDataStore dataStore = new(siteExtractorOption);

        var resourceCopierOptions = Options.Create(new ResourceCopierOptions()
        {
            IsOutputPathAbsolute = true,
            OutputPath = "store"
        });
        ResourceCopier copier = new(siteSource, dataStore, resourceCopierOptions);

        // When
        void act() => copier.Download(sourceUri);

        // Then
        Assert.Throws<InvalidOperationException>(act);
    }
}

public class ResourceCopierTests_Process
{
    [Fact]
    public void Success_FinishedProcess()
    {
        // Given
        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        var siteExtractorOption = Options.Create(new SiteExtractorOptions()
        {
            SourceAuthority = "memory://example.com",
            SourceBasePath = "input",
            OutputDirectory = "memory://output"
        });
        MemorySiteSource<byte[]> siteSource = new(baseUri: siteExtractorOption.Value.GetSourceBaseUri())
        {
            { sourceUri, [1, 2, 3] }
        };
        MemoryDataStore dataStore = new(siteExtractorOption);

        var resourceCopierOptions = Options.Create(new ResourceCopierOptions()
        {
            IsOutputPathAbsolute = true,
            OutputPath = "store"
        });
        ResourceCopier copier = new(siteSource, dataStore, resourceCopierOptions);
        Resource resource = new(sourceUri);

        // When
        IResourceProcessorResult result = copier.Process(resource);

        // Then
        Assert.Same(copier, result.Processor);
        Assert.Same(resource, result.Resource);
        Assert.NotNull(result.ResourceUriTranslaterToSet);
        Assert.False(result.IsUnfinished);
        Assert.Empty(result.GetResourceDependencies());
        Assert.Throws<InvalidOperationException>(() => result.ContinueProcess());
    }
}
