using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Fakes;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourcesTrackerTests_AddResource
{
    private static ExtractionTask AsTask(UriNode uri)
    {
        ExtractionTask result = new(uri.Uri);
        result.SetResult(new BasicProcessTaskResult(uri), ChildrenTaskInfoFactory.Default);
        return result;
    }

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
        Resource entry = Assert.Contains(uri, resourcesTracker.Resources);
        Assert.False(entry.State.IsPullable);
    }

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
        Resource entry = Assert.Contains(uri, resourcesTracker.Resources);
        Assert.True(entry.State.IsPullable);
    }
}
