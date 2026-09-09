namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Specifies the character used for indentation in the serialization writer.</summary>
public enum IndentType
{
    /// <summary>Indentation uses space characters; the count per level is controlled by <see cref="IndentSpec.Size"/>.</summary>
    Space,

    /// <summary>Indentation uses a single tab character per indent level.</summary>
    Tab
}
