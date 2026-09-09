using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ReferencerUpdater.UpdateReferencers"/> rewriting embedded URIs.</summary>
public class ReferencerUpdaterTests_UpdateReferencers
{
    /// <summary>Verifies that referencers whose target URIs are in the tracked resource repository are updated to translated URIs.</summary>
    [Fact]
    public void WithReferencersFoundInTrackedResources_ReferencersUpdated()
    {
        // Given
        Resource referencerResource = new(new Uri("https://example.com/referencer"));
        List<IStructDocReferencer> referencers = [
            new UriReferenceNode("https://example.com/referencer1", "https://example.com/ref1"),
            new UriReferenceNode("https://example.com/referencer2", "https://example.com/ref2#frag")
        ];
        ResourceRepository resourceRepository = new(["https://example.com/ref1", "https://example.com/ref2"]);
        ResourceUriTranslater uriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"));
        foreach (var resource in resourceRepository)
            resource.UriTranslater = uriTranslater;

        ReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, referencers, resourceRepository);

        // Then
        Assert.Equal("https://test.com/ref1", referencers[0].GetReference());
        Assert.Equal("https://test.com/ref2#frag", referencers[1].GetReference());
    }

    /// <summary>Verifies that referencers whose target URIs are not in the repository are left unchanged.</summary>
    [Fact]
    public void WithReferencersNotFoundInTrackedResources_ReferencersUntouch()
    {
        // Given
        Resource referencerResource = new(new Uri("https://example.com/referencer"));
        List<IStructDocReferencer> referencers = [
            new UriReferenceNode("https://example.com/referencer1", "https://example.com/ref1"),
            new UriReferenceNode("https://example.com/referencer2", "https://example.com/ref2#frag")
        ];
        ResourceRepository resourceRepository = new(["https://example.com/ref3", "https://example.com/ref4"]);
        ResourceUriTranslater uriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"));
        foreach (var resource in resourceRepository)
            resource.UriTranslater = uriTranslater;

        ReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, referencers, resourceRepository);

        // Then
        Assert.Equal("https://example.com/ref1", referencers[0].GetReference());
        Assert.Equal("https://example.com/ref2#frag", referencers[1].GetReference());
    }
}
