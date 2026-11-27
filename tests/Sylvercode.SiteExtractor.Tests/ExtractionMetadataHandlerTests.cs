using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Mocks;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ExtractionMetadataHandler"/> applying processor metadata.</summary>
public class ExtractionMetadataHandlerTests_OnNext
{
    /// <summary>Verifies that metadata from a processed resource is copied to the resource.</summary>
    [Fact]
    public void ResourceProcessed_CopiesMetadata()
    {
        ResourceProcessorMock processor = new();
        Resource resource = new(new Uri("https://example.com/page"), isPullable: true);
        ResourceProcessorResultMock result = new(processor, resource);
        result.NewMetadata.AddMetadata("title", "A title");
        ExtractionMetadataHandler handler = new();

        handler.OnNext(new ResourceProcessed(resource, result));

        Assert.Equal("A title", resource.Metadata.GetStrValue("title"));
    }

    /// <summary>Verifies that unrelated events do not alter resource metadata.</summary>
    [Fact]
    public void ExtractionStarted_DoesNothing()
    {
        ExtractionMetadataHandler handler = new();

        handler.OnNext(new ExtractionStarted(new Uri("https://example.com/page")));
    }
}
