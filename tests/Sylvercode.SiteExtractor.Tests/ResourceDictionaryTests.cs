using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceDictionaryTests_Add
{
    [Fact]
    public void TwoFragmentWithSamePullType_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), ResourcePullType.Extract);

        // When
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);

        // Then
        Resource resource = Assert.Single(resourceDictionary.Values);
        Assert.Collection(resource.Fragments.Order(),
                          f => Assert.Equal("", f),
                          f => Assert.Equal("#fragment1", f));
    }

    [Fact]
    public void TwoFragmentWithDifferentPullType_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), ResourcePullType.Extract);

        // When
        void action() => resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Download);

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }
}

public class ResourceDictionaryTests_GetDestinationFor
{
    [Fact]
    public void ResourceDoesNotExist_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];

        // When
        void action() => resourceDictionary.GetDestinationFor(new Uri("https://example.com"));

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void ResourceExists_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), ResourcePullType.Extract);
        resourceDictionary.Single().Value.MarkAsPulled(new Uri("https://other.example.org"));

        // When
        Uri destination = resourceDictionary.GetDestinationFor(new Uri("https://example.com"));

        // Then
        Assert.Equal("https://other.example.org/", destination.AbsoluteUri);
    }

    [Fact]
    public void ResourceExistsWithFragment_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);
        resourceDictionary.Single().Value.MarkAsPulled(new Uri("https://other.example.org"));

        // When
        Uri destination = resourceDictionary.GetDestinationFor(new Uri("https://example.com#fragment1"));

        // Then
        Assert.Equal("https://other.example.org/#fragment1", destination.AbsoluteUri);
    }

    [Fact]
    public void ResourceExistsWithFragment_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);
        resourceDictionary.Single().Value.MarkAsPulled(new Uri("https://other.example.org"));

        // When
        void action() => resourceDictionary.GetDestinationFor(new Uri("https://other.example.org/#fragment2"));

        // Then
        Assert.Throws<InvalidOperationException>(action);
    }
}

public class ResourceDictionaryTests_Index
{
    [Fact]
    public void ResourceDoesNotExist_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];

        // When
        void action() => _ = resourceDictionary[new Uri("https://example.com")];

        // Then
        Assert.Throws<KeyNotFoundException>(action);
    }

    [Fact]
    public void ResourceExists_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), ResourcePullType.Extract);

        // When
        Resource resource = resourceDictionary[new Uri("https://example.com")];

        // Then
        Assert.Equal("https://example.com/", resource.SourceUri.AbsoluteUri);
    }

    [Fact]
    public void ResourceExistsWithFragment_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);

        // When
        Resource resource = resourceDictionary[new Uri("https://example.com#fragment1")];

        // Then
        Assert.Equal("https://example.com/", resource.SourceUri.AbsoluteUri);
        Assert.Collection(resource.Fragments.Order(),
                          f => Assert.Equal("#fragment1", f));
    }

    [Fact]
    public void ResourceExistsWithFragment_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);

        // When
        void action() => _ = resourceDictionary[new Uri("https://example.com#fragment2")];

        // Then
        Assert.Throws<KeyNotFoundException>(action);
    }
}

public class ResourceDictionaryTests_Contains
{
    [Fact]
    public void ResourceDoesNotExist_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];

        // When
        bool contains = resourceDictionary.ContainsKey(new Uri("https://example.com/"));

        // Then
        Assert.False(contains);
    }

    [Fact]
    public void ResourceExists_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), ResourcePullType.Extract);

        // When
        bool contains = resourceDictionary.ContainsKey(new Uri("https://example.com/"));

        // Then
        Assert.True(contains);
    }

    [Fact]
    public void ResourceExistsWithFragment_Valid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);

        // When
        bool contains = resourceDictionary.ContainsKey(new Uri("https://example.com/#fragment1"));

        // Then
        Assert.True(contains);
    }

    [Fact]
    public void ResourceExistsWithFragment_Invalid()
    {
        // Given
        ResourceDictionary resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), ResourcePullType.Extract);

        // When
        bool contains = resourceDictionary.ContainsKey(new Uri("https://example.com/#fragment2"));

        // Then
        Assert.False(contains);
    }
}
