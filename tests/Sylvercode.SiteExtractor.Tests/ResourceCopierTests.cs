
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ResourceCopier.Download"/> copying bytes to the data store.</summary>
public class ResourceCopierTests_Download
{
    /// <summary>Creates a default DI host configured with an in-memory source, memory data store, and <see cref="ResourceCopier"/>.</summary>
    /// <param name="isOutputPathAbsolute">When <see langword="true"/>, the output path is treated as an absolute path segment.</param>
    /// <returns>A built <see cref="IHost"/> ready for test use.</returns>
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

    /// <summary>Verifies that an existing resource is copied to the absolute output path in the data store.</summary>
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

    /// <summary>Verifies that an existing resource is copied to the relative output path in the data store.</summary>
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

    /// <summary>Verifies that attempting to download a non-existent resource throws an <see cref="InvalidOperationException"/>.</summary>
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

/// <summary>Tests for <see cref="ResourceCopier.Process"/> returning a finished result after a successful download.</summary>
public class ResourceCopierTests_Process
{
    /// <summary>Verifies that a successful copy produces a finished, non-continuing processor result with a URI translater set.</summary>
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
