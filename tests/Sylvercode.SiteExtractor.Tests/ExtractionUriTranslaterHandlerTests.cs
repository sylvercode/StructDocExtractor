using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ExtractionUriTranslaterHandler"/> applying output translations.</summary>
public class ExtractionUriTranslaterHandlerTests_OnNext
{
    /// <summary>Verifies that a translator returned by a processor is assigned to the resource.</summary>
    [Fact]
    public void ResourceProcessed_SetsUriTranslater()
    {
        ResourceProcessorMock processor = new();
        Resource resource = new(new Uri("https://example.com/page"), isPullable: true);
        FixedTranslater translater = new();
        ResourceProcessorResultMock result = new(processor, resource)
        {
            ResourceUriTranslaterToSet = translater,
        };
        ExtractionUriTranslaterHandler handler =
            new(new RemoveAllUriQueryIdentityFilter());

        handler.OnNext(new ResourceProcessed(resource, result));

        Assert.Equal(new Uri("output.md", UriKind.Relative), resource.TranslatedResourceUri);
    }

    /// <summary>Verifies that a result without a translator leaves the resource translation unchanged.</summary>
    [Fact]
    public void ResourceProcessedWithoutTranslater_DoesNothing()
    {
        ResourceProcessorMock processor = new();
        Uri sourceUri = new("https://example.com/page");
        Resource resource = new(sourceUri, isPullable: true);
        ResourceProcessorResultMock result = new(processor, resource);
        ExtractionUriTranslaterHandler handler =
            new(new RemoveAllUriQueryIdentityFilter());

        handler.OnNext(new ResourceProcessed(resource, result));

        Assert.Equal(sourceUri, resource.TranslatedResourceUri);
    }

    [Fact]
    public void ResourceProcessed_RemovesQueryBeforeTranslation()
    {
        ResourceProcessorMock processor = new();
        Resource resource = new(
            new Uri("https://example.com/page?lang=en"),
            isPullable: true);
        RecordingTranslater translater = new();
        ResourceProcessorResultMock result = new(processor, resource)
        {
            ResourceUriTranslaterToSet = translater,
        };
        ExtractionUriTranslaterHandler handler =
            new(new RemoveAllUriQueryIdentityFilter());

        handler.OnNext(new ResourceProcessed(resource, result));
        resource.TranslateUri();

        Assert.Equal("https://example.com/page", translater.SourceUri.AbsoluteUri);
    }

    private sealed class FixedTranslater : IResourceUriTranslater
    {
        public Uri Translate(Resource resource, Uri uri)
            => new("output.md", UriKind.Relative);
    }

    private sealed class RecordingTranslater : IResourceUriTranslater
    {
        public Uri SourceUri { get; private set; } = null!;

        public Uri Translate(Resource resource, Uri uri)
        {
            SourceUri = uri;
            return new("output.md", UriKind.Relative);
        }
    }
}
