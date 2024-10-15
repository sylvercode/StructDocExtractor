using Sylvercode.SiteExtractor.Resources;
using Sylvercode.StructDocExtractor.Model;
namespace Sylvercode.SiteExtractor.Markdown.Tests;

public class MarkdownReferencerUpdaterTests_UpdateReferencers
{
    public static ResourceUriTranslater BaseTranslater { get; } = ResourceUriTranslater.NewBaseTranslater(
        new Uri("http://example.com/"),
        new Uri("http://test.com/"));
    public sealed class ReferencerNode(string reference) : IStructDocReferencer
    {
        public string Reference { get; set; } = reference;

        public string GetReference() => Reference;

        public void UpdateReference(string newReference) => Reference = newReference;
    }

    private static void AssingUriTranlater(ResourceRepository resources)
    {
        foreach (var resource in resources)
            resource.UriTranslater = BaseTranslater;
    }

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
        Assert.Equal("#fragment", referencer.Reference);
    }

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
        Assert.Equal("#frag", referencer.Reference);
    }

    [Theory]
    [InlineData("http://example.com/ref", "ref")]
    [InlineData("http://example.com/ref#frag", "ref#frag")]
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
        Assert.Equal("http://test.com/ref", referencer.Reference);
    }
}
