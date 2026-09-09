using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;

/// <summary>Carries configuration for <see cref="MarkdownStreamWriter"/> controlling indentation, inline style delimiter characters, and alternate-style nesting preferences.</summary>
public class MarkdownStyle
{
    /// <summary>Gets the default <see cref="MarkdownStyle"/> using two-space indentation, asterisk delimiters, and no alternate-style preference.</summary>
    public static MarkdownStyle Default { get; } = new(
        new() { Type = IndentType.Space, Size = 2 },
        StyleCharacter.Asterisk,
        StyleCharacter.Asterisk,
        false
    );

    /// <summary>Gets the indentation specification applied to the underlying <see cref="IndentedStreamWriter"/>.</summary>
    public IndentSpec IndentSpec { get; init; }
    /// <summary>Gets the delimiter character used for emphasis (italic) markers.</summary>
    public StyleCharacter EmphasisCharacter { get; init; }
    /// <summary>Gets the delimiter character used for strong (bold) markers.</summary>
    public StyleCharacter StrongCharacter { get; init; }
    /// <summary>Gets a value indicating whether an alternate delimiter character is preferred when inline styles are nested.</summary>
    public bool PreferAlternateStyle { get; init; }

    /// <summary>Initializes a new instance of <see cref="MarkdownStyle"/> with default settings.</summary>
    [SetsRequiredMembers]
    public MarkdownStyle()
    {
        IndentSpec = Default.IndentSpec;
        EmphasisCharacter = Default.EmphasisCharacter;
        StrongCharacter = Default.StrongCharacter; 
        PreferAlternateStyle = Default.PreferAlternateStyle;
    }

    /// <summary>Initializes a new instance of <see cref="MarkdownStyle"/> with the specified settings.</summary>
    /// <param name="indentSpec">The indentation specification for the writer.</param>
    /// <param name="emphasisCharacter">The delimiter character for emphasis markers.</param>
    /// <param name="strongCharacter">The delimiter character for strong markers.</param>
    /// <param name="preferAlternateStyle">Whether to prefer alternate delimiter characters when styles are nested.</param>
    [SetsRequiredMembers]
    public MarkdownStyle(IndentSpec indentSpec, StyleCharacter emphasisCharacter, StyleCharacter strongCharacter, bool preferAlternateStyle)
    {
        IndentSpec = indentSpec;
        EmphasisCharacter = emphasisCharacter;
        StrongCharacter = strongCharacter;
        PreferAlternateStyle = preferAlternateStyle;
    }
}
