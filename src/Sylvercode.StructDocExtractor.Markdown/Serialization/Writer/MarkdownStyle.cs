using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;

public class MarkdownStyle
{
    public static MarkdownStyle Default { get; } = new(
        new() { Type = IndentType.Space, Size = 2 },
        StyleCharacter.Asterisk,
        StyleCharacter.Asterisk,
        false
    );

    public IndentSpec IndentSpec { get; init; }
    public StyleCharacter EmphasisCharacter { get; init; }
    public StyleCharacter StrongCharacter { get; init; }
    public bool PreferAlternateStyle { get; init; }

    [SetsRequiredMembers]
    public MarkdownStyle()
    {
        IndentSpec = Default.IndentSpec;
        EmphasisCharacter = Default.EmphasisCharacter;
        StrongCharacter = Default.StrongCharacter; 
        PreferAlternateStyle = Default.PreferAlternateStyle;
    }

    [SetsRequiredMembers]
    public MarkdownStyle(IndentSpec indentSpec, StyleCharacter emphasisCharacter, StyleCharacter strongCharacter, bool preferAlternateStyle)
    {
        IndentSpec = indentSpec;
        EmphasisCharacter = emphasisCharacter;
        StrongCharacter = strongCharacter;
        PreferAlternateStyle = preferAlternateStyle;
    }
}
