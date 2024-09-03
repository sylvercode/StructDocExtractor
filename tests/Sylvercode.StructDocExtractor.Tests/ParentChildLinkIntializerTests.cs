using Sylvercode.StructDocExtractor.DnDBeyond.Model;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;
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


    [Fact]
    public void Link_TOC_WITH_TOCSection()
    {
        // Given
        DDBTOC toc = new();
        DDBTOCSection? section1 = null;
        DDBTOCSectionEntry? entry1_1 = null;
        string entry1_1_Text = nameof(entry1_1_Text);
        string entry1_1_Href = nameof(entry1_1_Href);
        DDBTOCSectionEntry? entry1_2 = null;
        string entry1_2_Text = nameof(entry1_2_Text);
        string entry1_2_Href = nameof(entry1_2_Href);
        DDBTOCSection? section2 = null;
        DDBTOCSectionTitle? title2_1 = null;
        string entry2_1_Text = nameof(entry2_1_Text);
        DDBTOCSectionEntry? entry2_1_1 = null;
        string entry2_1_1_Text = nameof(entry2_1_1_Text);
        string entry2_1_1_Href = nameof(entry2_1_1_Href);
        DDBTOCSectionEntry? entry2_2 = null;
        string entry2_2_Text = nameof(entry2_2_Text);
        string entry2_2_Href = nameof(entry2_2_Href);

        // When
        toc.InitWithContent(content =>
        {
            section1 = content.Add(new DDBTOCSection());
            section1.InitWithContent(content =>
            {
                entry1_1 = content.Add(new DDBTOCSectionEntry(entry1_1_Text, entry1_1_Href));
                entry1_2 = content.Add(new DDBTOCSectionEntry(entry1_2_Text, entry1_2_Href));
            });

            section2 = content.Add(new DDBTOCSection());
            section2.InitWithContent(content =>
            {
                title2_1 = content.Add(new DDBTOCSectionTitle(entry2_1_Text));
                entry2_1_1 = content.Add(new DDBTOCSectionEntry(entry2_1_1_Text, entry2_1_1_Href));
                entry2_2 = content.Add(new DDBTOCSectionEntry(entry2_2_Text, entry2_2_Href));
            });
        });

        // Then
        Assert.NotNull(section1);
        Assert.NotNull(section2);
        Assert.Equal([section1, section2], toc.Content);
        Assert.Same(section1.Parent, toc);
        Assert.Same(section2.Parent, toc);

        Assert.NotNull(entry1_1);
        Assert.NotNull(entry1_2);
        Assert.Equal([entry1_1, entry1_2], section1.Content);
        Assert.Same(entry1_1.Parent, section1);
        Assert.Same(entry1_2.Parent, section1);

        Assert.NotNull(title2_1);
        Assert.NotNull(entry2_1_1);
        Assert.NotNull(entry2_2);
        Assert.Equal([title2_1, entry2_1_1, entry2_2], section2.Content);
        Assert.Same(title2_1.Parent, section2);
        Assert.Same(entry2_1_1.Parent, section2);
        Assert.Same(entry2_2.Parent, section2);
    }
}
