using System.Text;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;

public class MarkdownStreamWriter(
    Stream stream,
    MarkdownStyle style,
    Encoding encoding,
    IFormatProvider? formatProvider)
    : IndentedStreamWriter(stream, style.IndentSpec, encoding, formatProvider)
{
    private enum StyleState
    {
        None,
        Emphasis,
        Strong
    }

    private class StyleStackEntry
    {
        public StyleState State { get; set; }
        public StyleCharacter Character { get; set; }
    }

    private readonly Stack<StyleStackEntry> _styleStack = new();

    public MarkdownStreamWriter(Stream stream) : this(stream, new MarkdownStyle())
    {
    }

    public MarkdownStreamWriter(Stream stream, MarkdownStyle style) : this(stream, style, Encoding.UTF8)
    {
    }

    public MarkdownStreamWriter(Stream stream, MarkdownStyle style, Encoding encoding) : this(stream, style, encoding, null)
    {
    }

    public MarkdownStyle Style { get; } = style;

    public void PushEmphasis() => PushStyle(StyleState.Emphasis);

    public void PushStrong() => PushStyle(StyleState.Strong);

    public void PopEnmphasis() => PopStyle(StyleState.Emphasis);

    public void PopStrong() => PopStyle(StyleState.Strong);

    private void PushStyle(StyleState state)
    {
        if (_styleStack.Any((i) => i.State == state))
            throw new InvalidOperationException("Cannot push already active style.");

        StyleCharacter styleCharacter;
        if (Style.PreferAlternateStyle && _styleStack.Count != 0)
        {
            styleCharacter = _styleStack.Peek().Character switch
            {
                StyleCharacter.Asterisk => StyleCharacter.Underscore,
                StyleCharacter.Underscore => StyleCharacter.Asterisk,
                _ => throw new InvalidOperationException("Invalid style character.")
            };
        }
        else
        {
            styleCharacter = state switch
            {
                StyleState.Emphasis => Style.EmphasisCharacter,
                StyleState.Strong => Style.StrongCharacter,
                _ => throw new InvalidOperationException("Invalid style state.")
            };
        }

        _styleStack.Push(new StyleStackEntry { State = state, Character = styleCharacter });

        char characterToWrite = GetCharacterToWrite(styleCharacter);

        WriteCharacterForStyle(state, characterToWrite);
    }

    private void PopStyle(StyleState state)
    {
        if (_styleStack.Peek().State != state)
            throw new InvalidOperationException("Cannot pop inactive style.");

        StyleCharacter styleCharacter = _styleStack.Pop().Character;
        char characterToWrite = GetCharacterToWrite(styleCharacter);

        WriteCharacterForStyle(state, characterToWrite);
    }

    private char GetCharacterToWrite(StyleCharacter styleCharacter) => styleCharacter switch
    {
        StyleCharacter.Asterisk => '*',
        StyleCharacter.Underscore => '_',
        _ => throw new InvalidOperationException("Invalid style character.")
    };

    private void WriteCharacterForStyle(StyleState styleState, char characterToWrite)
    {
        Write(characterToWrite);
        if (styleState == StyleState.Strong)
            Write(characterToWrite);
    }
}
