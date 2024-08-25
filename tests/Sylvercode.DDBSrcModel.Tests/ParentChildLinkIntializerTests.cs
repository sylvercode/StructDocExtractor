using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Base;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Tests;

public class ParentChildLinkIntializer_InitializeParentChildLinkShould
{
    [Fact]
    public void Link_OneParent_WithOneChild()
    {
        // Given
        SrcRootBlock root = new();
        SrcParagraph paragraph = new();

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
        SrcRootBlock root = new();
        SrcParagraph paragraph1 = new();
        SrcParagraph paragraph2 = new();

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
        SrcTOC toc = new();
        SrcTOCSection? section1 = null;
        SrcTOCSectionEntry? entry1_1 = null;
        string entry1_1_Text = nameof(entry1_1_Text);
        string entry1_1_Href = nameof(entry1_1_Href);
        SrcTOCSectionEntry? entry1_2 = null;
        string entry1_2_Text = nameof(entry1_2_Text);
        string entry1_2_Href = nameof(entry1_2_Href);
        SrcTOCSection? section2 = null;
        SrcTOCSectionTitle? title2_1 = null;
        string entry2_1_Text = nameof(entry2_1_Text);
        SrcTOCSectionEntry? entry2_1_1 = null;
        string entry2_1_1_Text = nameof(entry2_1_1_Text);
        string entry2_1_1_Href = nameof(entry2_1_1_Href);
        SrcTOCSectionEntry? entry2_2 = null;
        string entry2_2_Text = nameof(entry2_2_Text);
        string entry2_2_Href = nameof(entry2_2_Href);

        // When
        toc.InitWithContent(content =>
        {
            section1 = content.Add(new SrcTOCSection());
            section1.InitWithContent(content =>
            {
                entry1_1 = content.Add(new SrcTOCSectionEntry(entry1_1_Text, entry1_1_Href));
                entry1_2 = content.Add(new SrcTOCSectionEntry(entry1_2_Text, entry1_2_Href));
            });

            section2 = content.Add(new SrcTOCSection());
            section2.InitWithContent(content =>
            {
                title2_1 = content.Add(new SrcTOCSectionTitle(entry2_1_Text));
                entry2_1_1 = content.Add(new SrcTOCSectionEntry(entry2_1_1_Text, entry2_1_1_Href));
                entry2_2 = content.Add(new SrcTOCSectionEntry(entry2_2_Text, entry2_2_Href));
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
