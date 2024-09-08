using Sylvercode.SiteSource.Resources;

namespace Sylvercode.SiteSource.Tests;

public class ResourceTests_DestinationUri
{
    [Fact]
    public void NoPull_ReturnsOriginal()
    {
        // Given
        Resource resource = new(new Uri("http://example.com"), ResourcePullType.NoPull);

        // When
        Uri destinationUri = resource.DestinationUri;

        // Then
        Assert.Equal(resource.Uri, destinationUri);
    }

    [Fact]
    public void NotPulled_ThrowsInvalidOperationException()
    {
        // Given
        Resource resource = new(new Uri("http://example.com"), ResourcePullType.Extract);

        // When
        void action() => _ = resource.DestinationUri;

        // Then
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Pulled_ReturnsDestination()
    {
        // Given
        Resource resource = new(new Uri("http://example.com"), ResourcePullType.Extract);
        Uri destinationUri = new("http://example.com/destination");
        resource.MarkAsPulled(destinationUri);

        // When
        Uri result = resource.DestinationUri;

        // Then
        Assert.Equal(destinationUri, result);
    }
}
