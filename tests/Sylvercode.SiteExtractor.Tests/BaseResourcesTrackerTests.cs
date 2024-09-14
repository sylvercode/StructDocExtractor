using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Tests.Fakes;
using Sylvercode.SiteExtractor.Tests.Stubs;

namespace Sylvercode.SiteExtractor.Tests;

public class BaseResourcesTrackerTests_OnTaskResult
{
    [Fact]
    public void NewUriResult_UriAdded()
    {
        // Arrange
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("http://example.com"), ResourcePullType.NoPull);
        FakeResourcePullConfig pullConfig = new();
        BasicResourcesTracker resourcesTracker = new(resourceDictionary, pullConfig);
        Uri uri = new("http://other.example.com");

        // Act
        resourcesTracker.OnTaskResult(uri, EventArgs.Empty);

        // Assert
        Resource entry = Assert.Contains(uri, resourceDictionary);
        Assert.Equal(ResourcePullType.NoPull, entry.State.PullType);
    }
    [Fact]
    public void ExistingUriResult_UriIgnored()
    {
        // Arrange
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("http://example.com"), ResourcePullType.Extract);
        FakeResourcePullConfig pullConfig = new();
        BasicResourcesTracker resourcesTracker = new(resourceDictionary, pullConfig);
        Uri uri = new("http://example.com");

        // Act
        resourcesTracker.OnTaskResult(uri, EventArgs.Empty);

        // Assert
        Resource entry = Assert.Contains(uri, resourceDictionary);
        Assert.Equal(ResourcePullType.Extract, entry.State.PullType);
    }
}
