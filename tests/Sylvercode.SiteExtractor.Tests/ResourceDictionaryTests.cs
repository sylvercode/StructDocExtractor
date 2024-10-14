using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceDictionaryTests_Add
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TwoFragmentWithSamePullType_Valid(bool isPullable)
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), isPullable);

        // When
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), isPullable);

        // Then
        Resource resource = Assert.Single(resourceDictionary);
    }

    [Fact]
    public void TwoFragmentWithDifferentPullType_Invalid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), isPullable: true);

        // When
        void action() => resourceDictionary.Add(new Uri("https://example.com#fragment1"), isPullable: false);

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
        ResourceRepository resourceDictionary = [];

        // When
        void action() => _ = resourceDictionary[new Uri("https://example.com")];

        // Then
        Assert.Throws<KeyNotFoundException>(action);
    }

    [Fact]
    public void ResourceExists_Valid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), isPullable: true);

        // When
        Resource resource = resourceDictionary[new Uri("https://example.com")];

        // Then
        Assert.Equal("https://example.com/", resource.Uri.AbsoluteUri);
    }

    [Fact]
    public void ResourceExistsWithFragment_Valid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), isPullable: true);

        // When
        Resource resource = resourceDictionary[new Uri("https://example.com#fragment1")];

        // Then
        Assert.Equal("https://example.com/", resource.Uri.AbsoluteUri);
    }

    [Fact]
    public void ResourceExistsWithOtherFragment_Valid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), isPullable: true);

        // When
        Resource resource = resourceDictionary[new Uri("https://example.com#fragment2")];

        // Then
        Assert.Equal("https://example.com/", resource.Uri.AbsoluteUri);
    }
}

public class ResourceDictionaryTests_Contains
{
    [Fact]
    public void ResourceDoesNotExist_Invalid()
    {
        // Given
        ResourceRepository resourceDictionary = [];

        // When
        bool contains = resourceDictionary.ContainsResourceForUri(new Uri("https://example.com/"));

        // Then
        Assert.False(contains);
    }

    [Fact]
    public void ResourceExists_Valid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com"), isPullable: true);

        // When
        bool contains = resourceDictionary.ContainsResourceForUri(new Uri("https://example.com/"));

        // Then
        Assert.True(contains);
    }

    [Fact]
    public void ResourceExistsWithFragment_Valid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), isPullable: true);

        // When
        bool contains = resourceDictionary.ContainsResourceForUri(new Uri("https://example.com/#fragment1"));

        // Then
        Assert.True(contains);
    }

    [Fact]
    public void ResourceExistsWithOtherFragment_Valid()
    {
        // Given
        ResourceRepository resourceDictionary = [];
        resourceDictionary.Add(new Uri("https://example.com#fragment1"), isPullable: true);

        // When
        bool contains = resourceDictionary.ContainsResourceForUri(new Uri("https://example.com/#fragment2"));

        // Then
        Assert.True(contains);
    }
}
