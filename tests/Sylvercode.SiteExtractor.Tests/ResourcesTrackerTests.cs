using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Fakes;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Tests.Stubs;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourcesTrackerTests_OnTaskResult
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
        ResourcesTracker resourcesTracker = new(new FakeResourceProcessoProvider(ResourcePullType.NoPull), new BasicResourcesUriRetriver());
        resourcesTracker.AddResource(new Uri("http://example.com"));
        UriNode uri = new("http://other.example.com");
        ExtractionTask task = AsTask(uri);

        // Act
        resourcesTracker.OnNext(task);

        // Assert
        Resource entry = Assert.Contains(uri.Uri, resourcesTracker.Resources);
        Assert.Equal(ResourcePullType.NoPull, entry.State.PullType);
    }

    [Fact]
    public void ExistingUriResult_UriIgnored()
    {
        // Arrange
        ResourcesTracker resourcesTracker = new(new FakeResourceProcessoProvider(ResourcePullType.Extract), new BasicResourcesUriRetriver());
        resourcesTracker.AddResource(new Uri("http://example.com"));
        UriNode uri = new("http://example.com");
        ExtractionTask task = AsTask(uri);

        // Act
        resourcesTracker.OnNext(task);

        // Assert
        Resource entry = Assert.Contains(uri.Uri, resourcesTracker.Resources);
        Assert.Equal(ResourcePullType.Extract, entry.State.PullType);
    }
}
