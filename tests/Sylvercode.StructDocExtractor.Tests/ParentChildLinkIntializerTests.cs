using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Init;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Tests;

public class ParentChildLinkInitializer_InitializeParentChildLinkShould
{
    [Fact]
    public void Link_OneParent_WithOneChild()
    {
        // Given
        StructDocRootBlock root = new();
        HtmlParagraph paragraph = new();

        // When
        root.InitWithContent(paragraph);

        // Then
        Assert.Equal([paragraph], root.Content);
        Assert.Same(paragraph.Parent, root);
    }

    [Fact]
    public void Link_OneParent_WithTwoChild()
    {
        // Given
        StructDocRootBlock root = new();
        HtmlParagraph paragraph1 = new();
        HtmlParagraph paragraph2 = new();

        // When
        root.InitWithContent(paragraph1, paragraph2);

        // Then
        Assert.Equal([paragraph1, paragraph2], root.Content);
        Assert.Same(paragraph1.Parent, root);
        Assert.Same(paragraph2.Parent, root);
    }
}
