using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

[method: SetsRequiredMembers]
public class MarkdownStyle()
{
    public static MarkdownStyle Default { get; } = new()
    {
        IndentSpec = new() { Type = IndentType.Space, Size = 2 },
        EmphasisCharacter = StyleCharacter.Asterisk,
        StrongCharacter = StyleCharacter.Asterisk,
        PreferAlternateStyle = false
    };
    
    public IndentSpec IndentSpec { get; init; } = Default.IndentSpec;
    public StyleCharacter EmphasisCharacter { get; init; } = Default.EmphasisCharacter;
    public StyleCharacter StrongCharacter { get; init; } = Default.StrongCharacter;
    public bool PreferAlternateStyle { get; init; } = Default.PreferAlternateStyle;
}
