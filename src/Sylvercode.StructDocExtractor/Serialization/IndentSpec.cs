using System.Diagnostics.CodeAnalysis;

namespace Sylvercode.StructDocExtractor.Serialization;

[method: SetsRequiredMembers]
public class IndentSpec()
{

    public required IndentType Type { get; init; } = IndentType.Space;
    public required int Size { get; init; } = 4;
}
