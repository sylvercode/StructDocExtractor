using Sylvercode.SiteSource.Resources;

namespace Sylvercode.SiteSource.Tests;

public class ResourceTests_DestinationUri
{
    [Fact]
    public void NoPull_ReturnsOriginal()
    {
        // Given
        Resource resource = new(ResourcePullType.NoPull, new Uri("http://example.com"));

        // When
        Uri destinationUri = resource.DestinationUri;

        // Then
        Assert.Equal(resource.SourceUri, destinationUri);
    }

    [Fact]
    public void NotPulled_ThrowsInvalidOperationException()
    {
        // Given
        Resource resource = new(ResourcePullType.Extract, new Uri("http://example.com"));

        // When
        void action() => _ = resource.DestinationUri;

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Pulled_ReturnsDestination()
    {
        // Given
        Resource resource = new(ResourcePullType.Extract, new Uri("http://example.com"));
        Uri destinationUri = new("http://example.com/destination");
        resource.MarkAsPulled(destinationUri);

        // When
        Uri result = resource.DestinationUri;

        // Then
        Assert.Equal(destinationUri, result);
    }

    [Fact]
    public void WithFragment_ThrowsInvalidOperationException()
    {
        // Given
        Resource resource = new(ResourcePullType.Extract, new Uri("http://example.com"), "#fragment");
        resource.AddFragment("");

        // When
        void action() => _ = resource.DestinationUri;

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }
}

public class ResourceTests_GetDestinationFor
{
    [Fact]
    public void UriWithUnknownFragment_ThrowException()
    {
        // Given
        Resource resource = new(ResourcePullType.Extract, new Uri("http://example.com"));
        resource.MarkAsPulled(new Uri("http://other.example.com"));

        // When
        void action() => resource.GetDestinationFor(new Uri("http://example.com#fragment"));

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void UriWithKnownFragment_ReturnNewUri()
    {
        // Given
        Resource resource = new(ResourcePullType.Extract, new Uri("http://example.com"), "#fragment1");
        resource.AddFragment("#fragment2");
        resource.MarkAsPulled(new Uri("http://other.example.com/destination"));

        // When
        Uri result = resource.GetDestinationFor(new Uri("http://example.com#fragment2"));

        // Then
        Assert.Equal(new Uri("http://other.example.com/destination#fragment2"), result);

    }
}

public class ResourceTests_MarkAsPulled
{
    [Fact]
    public void WithFragment_ThrowsInvalidOperationException()
    {
        // Given
        Resource resource = new(ResourcePullType.Extract, new Uri("http://example.com"), "#fragment");

        // When
        void action() => resource.MarkAsPulled(new Uri("http://example.com/destination#fragment"));

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }
}
