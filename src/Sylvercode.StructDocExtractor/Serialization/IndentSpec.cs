using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Specifies the indent type and character count used by <see cref="IndentedStreamWriter"/>.</summary>
[method: SetsRequiredMembers]
public class IndentSpec()
{
    /// <summary>Gets the character type used for indentation.</summary>
    public required IndentType Type { get; init; } = IndentType.Space;

    /// <summary>Gets the number of indent characters (or tab stops) per indent level.</summary>
    public required int Size { get; init; } = 4;
}
