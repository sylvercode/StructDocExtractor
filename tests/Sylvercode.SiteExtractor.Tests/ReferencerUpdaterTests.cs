using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.Tests;

public class ReferencerUpdaterTests_UpdateReferencers
{
    [Fact]
    public void WithReferencersFoundInTrackedResources_ReferencersUpdated()
    {
        // Given
        List<IStructDocReferencer> referencers = [
            new UriReferenceNode("https://example.com/referencer1", "https://example.com/ref1"),
            new UriReferenceNode("https://example.com/referencer2", "https://example.com/ref2#frag")
        ];
        IReadOnlyDictionary<Uri, Resource> trackedResources = new Dictionary<Uri, Resource>
        {
            { new Uri("https://example.com/ref1"), new Resource(new Uri("https://example.com/ref1")) },
            { new Uri("https://example.com/ref2"), new Resource(new Uri("https://example.com/ref2")) }
        };
        ResourceUriTranslater uriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"));
        foreach (var resource in trackedResources)
            resource.Value.UriTranslater = uriTranslater;

        ReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencers, trackedResources);

        // Then
        Assert.Equal("https://test.com/ref1", referencers[0].GetReference());
        Assert.Equal("https://test.com/ref2#frag", referencers[1].GetReference());
    }

    [Fact]
    public void WithReferencersNotFoundInTrackedResources_ReferencersUntouch()
    {
        // Given
        List<IStructDocReferencer> referencers = [
            new UriReferenceNode("https://example.com/referencer1", "https://example.com/ref1"),
            new UriReferenceNode("https://example.com/referencer2", "https://example.com/ref2#frag")
        ];
        IReadOnlyDictionary<Uri, Resource> trackedResources = new Dictionary<Uri, Resource>
        {
            { new Uri("https://example.com/ref3"), new Resource(new Uri("https://example.com/ref3")) },
            { new Uri("https://example.com/ref4"), new Resource(new Uri("https://example.com/ref4")) }
        };
        ResourceUriTranslater uriTranslater = ResourceUriTranslater.NewBaseTranslater(new Uri("https://example.com/"), new Uri("https://test.com/"));
        foreach (var resource in trackedResources)
            resource.Value.UriTranslater = uriTranslater;

        ReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencers, trackedResources);

        // Then
        Assert.Equal("https://example.com/ref1", referencers[0].GetReference());
        Assert.Equal("https://example.com/ref2#frag", referencers[1].GetReference());
    }
}
