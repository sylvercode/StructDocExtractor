using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Fakes;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ResourcesTracker.AddResource"/> discovery and deduplication.</summary>
public class ResourcesTrackerTests_AddResource
{
    private static ExtractionTask AsTask(UriNode uri)
    {
        ExtractionTask result = new(uri.Uri);
        result.SetResult(new BasicProcessTaskResult(uri));
        return result;
    }

    /// <summary>Verifies that adding a new URI registers it in the resource tracker with the correct pullability.</summary>
    [Fact]
    public void NewUriResult_UriAdded()
    {
        // Arrange
        ResourcesTracker resourcesTracker = new(new FakeResourceProcessoProvider(returnsProcessor: false));
        resourcesTracker.AddResource(new Uri("http://example.com"));
        Uri uri = new("http://other.example.com");

        // Act
        resourcesTracker.AddResource(uri);

        // Assert
        Resource entry = Assert.Contains(uri, resourcesTracker.Resources.AsDictionary());
        Assert.False(entry.State.IsPullable);
    }

    /// <summary>Verifies that adding an already-tracked URI is silently ignored without creating a duplicate entry.</summary>
    [Fact]
    public void ExistingUriResult_UriIgnored()
    {
        // Arrange
        ResourcesTracker resourcesTracker = new(new FakeResourceProcessoProvider(returnsProcessor: true));
        resourcesTracker.AddResource(new Uri("http://example.com"));
        Uri uri = new("http://example.com");

        // Act
        resourcesTracker.AddResource(uri);

        // Assert
        Resource entry = Assert.Contains(uri, resourcesTracker.Resources.AsDictionary());
        Assert.True(entry.State.IsPullable);
    }
}
