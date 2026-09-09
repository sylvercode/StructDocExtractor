using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ResourceRepository.Add"/> deduplication and indexing logic.</summary>
public class ResourceDictionaryTests_Add
{
    /// <summary>Verifies that fragment URIs sharing the same base are collapsed into a single repository entry.</summary>
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

    /// <summary>Verifies that adding the same base URI with conflicting pullability throws an <see cref="InvalidOperationException"/>.</summary>
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

/// <summary>Tests for <see cref="ResourceRepository"/> index lookup by URI.</summary>
public class ResourceDictionaryTests_Index
{
    /// <summary>Verifies that indexing a URI not in the repository throws a <see cref="KeyNotFoundException"/>.</summary>
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

    /// <summary>Verifies that a plain URI resolves to its stored resource.</summary>
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

    /// <summary>Verifies that a fragment URI resolves to the base resource entry.</summary>
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

    /// <summary>Verifies that any fragment of a registered base URI resolves to the same resource entry.</summary>
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

/// <summary>Tests for <see cref="ResourceRepository.ContainsResourceForUri"/> membership checks.</summary>
public class ResourceDictionaryTests_Contains
{
    /// <summary>Verifies that querying a URI not in the repository returns <see langword="false"/>.</summary>
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

    /// <summary>Verifies that querying a registered URI returns <see langword="true"/>.</summary>
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

    /// <summary>Verifies that querying with a fragment of a registered URI returns <see langword="true"/>.</summary>
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

    /// <summary>Verifies that querying with a different fragment of the same base URI returns <see langword="true"/>.</summary>
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
