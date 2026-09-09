using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Model;
namespace Sylvercode.SiteExtractor.Markdown.Tests;

/// <summary>Tests for <see cref="MarkdownReferencerUpdater.UpdateReferencers"/> rewriting Markdown links.</summary>
public class MarkdownReferencerUpdaterTests_UpdateReferencers
{
    /// <summary>Gets the base URI translater used across test cases.</summary>
    public static ResourceUriTranslater BaseTranslater { get; } = ResourceUriTranslater.NewBaseTranslater(
        new Uri("http://example.com/"),
        new Uri("http://test.com/"));

    /// <summary>Local <see cref="IStructDocReferencer"/> implementation used to simulate mutable link nodes in tests.</summary>
    public sealed class ReferencerNode(string reference) : IStructDocReferencer
    {
        /// <summary>Gets or sets the current reference URI string for this node.</summary>
        public string Reference { get; set; } = reference;

        /// <inheritdoc/>
        public IStructDocReferencer.ReferenceType GetReferenceType()
            => IStructDocReferencer.ReferenceType.External;

        /// <inheritdoc/>
        public string GetReference() => Reference;

        /// <inheritdoc/>
        public void UpdateReference(string newReference) => Reference = newReference;
    }

    private static void AssingUriTranlater(ResourceRepository resources)
    {
        foreach (var resource in resources)
            resource.UriTranslater = BaseTranslater;
    }

    /// <summary>Verifies that a fragment-only reference is converted to the Obsidian wiki-link format.</summary>
    [Fact]
    public void FragmentOnly_Untouched()
    {
        // Given
        ReferencerNode referencer = new("#fragment");
        ResourceRepository resources = [];
        Resource referencerResource = resources.Add(new Uri("http://example.com/")).resource;
        AssingUriTranlater(resources);
        MarkdownReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, [referencer], resources);

        // Then
        Assert.Equal("wiki:/#^fragment", referencer.Reference);
    }

    /// <summary>Verifies that a reference to a URI not in the resource repository is left unchanged.</summary>
    [Fact]
    public void NotInRepoReferency_Untouched()
    {
        // Given
        ReferencerNode referencer = new("http://example.com/ref");
        ResourceRepository resources = new(["http://example.com/otherref"]);
        Resource referencerResource = resources.Add(new Uri("http://example.com/referer")).resource;
        AssingUriTranlater(resources);
        MarkdownReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, [referencer], resources);

        // Then
        Assert.Equal("http://example.com/ref", referencer.Reference);
    }

    /// <summary>Verifies that a self-referencing URI with a fragment is converted to a fragment-only wiki-link.</summary>
    [Fact]
    public void SelfReferer_SetTofragmentOnly()
    {
        // Given
        ReferencerNode referencer = new("http://example.com/referer#frag");
        ResourceRepository resources = new(["http://example.com/referer#frag"]);
        Resource referencerResource = resources.Add(new Uri("http://example.com/referer")).resource;
        AssingUriTranlater(resources);
        MarkdownReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, [referencer], resources);

        // Then
        Assert.Equal("wiki:/#^frag", referencer.Reference);
    }

    /// <summary>Verifies that references to unique file names are converted to compact wiki-link format.</summary>
    [Theory]
    [InlineData("http://example.com/ref", "wiki:ref")]
    [InlineData("http://example.com/ref#frag", "wiki:ref#^frag")]
    [InlineData("http://example.com/ref with space", "wiki:ref with space")]
    [InlineData("http://example.com/ref with space#frag", "wiki:ref with space#^frag")]
    [InlineData("http://example.com/folder/ref with space#frag", "wiki:ref with space#^frag")]
    public void ReferenceToUniqueFileName_SetToFileName(string reference, string expected)
    {
        // Given
        ReferencerNode referencer = new(reference);
        ResourceRepository resources = new([reference]);
        for (int i = 1; i <= 5; i++)
            resources.Add(new Uri(reference + i));

        Resource referencerResource = resources.Add(new Uri("http://example.com/referer")).resource;
        AssingUriTranlater(resources);
        MarkdownReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, [referencer], resources);

        // Then
        Assert.Equal(expected, referencer.Reference);
    }

    /// <summary>Verifies that when a file name is not unique across the repository the reference is set to the translated relative URI.</summary>
    [Fact]
    public void ReferenceFileNameNotUnique_SetToTranslatedUri()
    {
        // Given
        ReferencerNode referencer = new("http://example.com/ref");
        ResourceRepository resources = new(["http://example.com/ref", "http://example.com/folder/ref"]);
        Resource referencerResource = resources.Add(new Uri("http://example.com/referer")).resource;
        AssingUriTranlater(resources);
        MarkdownReferencerUpdater referencerUpdater = new();

        // When
        referencerUpdater.UpdateReferencers(referencerResource, [referencer], resources);

        // Then
        Assert.Equal("ref", referencer.Reference);
    }
}
