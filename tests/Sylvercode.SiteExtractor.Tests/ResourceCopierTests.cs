
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceCopierTests_Download
{
    public static IHost GetDefaultHost(bool isOutputPathAbsolute)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddOptions<SiteExtractorOptions>().Configure((options) =>
            {
                options.SourceAuthority = "memory://example.com";
                options.SourceBasePath = "input";
                options.OutputDirectory = "memory://output";
            });

            services.AddOptions<MemorySiteSource<byte[]>.Options>().Configure<IOptions<SiteExtractorOptions>>(
                (option, siteOptions) => option.BaseUri = siteOptions.Value.GetSourceBaseUri());
            services.AddMemorySiteSource<byte[]>();

            services.AddMemoryDataStore();

            services.AddOptions<ResourceCopierOptions>().Configure((options) =>
            {
                options.IsOutputPathAbsolute = isOutputPathAbsolute;
                options.OutputPath = "store";
            });

            services.AddSingleton<ResourceCopier>();
        });

        return builder.Build();
    }

    [Fact]
    public void ExistingToAbsolutePath_Copied()
    {
        // Given
        IHost host = GetDefaultHost(isOutputPathAbsolute: true);
        var source = host.Services.GetRequiredService<MemorySiteSource<byte[]>>();

        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        byte[] data = [1, 2, 3];
        source.Add(sourceUri, data);

        var copier = host.Services.GetRequiredService<ResourceCopier>();

        // When
        copier.Download(sourceUri);

        // Then
        var dataStore = host.Services.GetRequiredService<MemoryDataStore>();
        MemoryStream result = Assert.Contains(new Uri("memory://output/store/data1.bin"), dataStore);
        Assert.Equal(data, result.ToArray());
    }

    [Fact]
    public void ExistingToRelativePath_Copied()
    {
        // Given
        IHost host = GetDefaultHost(isOutputPathAbsolute: false);
        var source = host.Services.GetRequiredService<MemorySiteSource<byte[]>>();

        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        byte[] data = [1, 2, 3];
        source.Add(sourceUri, data);

        var copier = host.Services.GetRequiredService<ResourceCopier>();

        // When
        copier.Download(sourceUri);

        // Then
        var dataStore = host.Services.GetRequiredService<MemoryDataStore>();
        MemoryStream result = Assert.Contains(new Uri("memory://output/dir/store/data1.bin"), dataStore);
        Assert.Equal([1, 2, 3], result.ToArray());
    }

    [Fact]
    public void NotExisting_Throws()
    {
        // Given
        IHost host = GetDefaultHost(isOutputPathAbsolute: true);
        var copier = host.Services.GetRequiredService<ResourceCopier>();

        // When
        void act() => copier.Download(new Uri("memory://example.com/input/dir/data1.bin"));

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

        IHost host = ResourceCopierTests_Download.GetDefaultHost(isOutputPathAbsolute: true);
        var source = host.Services.GetRequiredService<MemorySiteSource<byte[]>>();

        Uri sourceUri = new("memory://example.com/input/dir/data1.bin");
        byte[] data = [1, 2, 3];
        source.Add(sourceUri, data);

        var copier = host.Services.GetRequiredService<ResourceCopier>();
        Resource resource = new(sourceUri);

        // When
        IResourceProcessorResult result = copier.Process(resource, new ResourceRepository());

        // Then
        Assert.Same(copier, result.Processor);
        Assert.Same(resource, result.Resource);
        Assert.NotNull(result.ResourceUriTranslaterToSet);
        Assert.False(result.IsUnfinished);
        Assert.Empty(result.GetResourceDependencies());
        Assert.Throws<InvalidOperationException>(() => result.ContinueProcess());
    }
}
