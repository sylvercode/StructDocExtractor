using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization;

[method: SetsRequiredMembers]
public class MarkdownStyle()
{
    public IndentSpec IndentSpec { get; init; } = new() { Type = IndentType.Space, Size = 2 };
    public StyleCharacter EmphasisCharacter { get; init; } = StyleCharacter.Asterisk;
    public StyleCharacter StrongCharacter { get; init; } = StyleCharacter.Asterisk;
    public bool PreferAlternateStyle { get; init; } = false;
}
