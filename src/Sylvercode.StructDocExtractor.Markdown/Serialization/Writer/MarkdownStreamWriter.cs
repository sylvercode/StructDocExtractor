using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;

/// <summary>Specialized <see cref="IndentedStreamWriter"/> that outputs formatted Markdown text with stack-based inline style management and list nesting awareness.</summary>
/// <remarks>
/// Extends <see cref="IndentedStreamWriter"/> with a push/pop style stack for emphasis and strong markers, enforcing strict nesting symmetry and optionally alternating delimiter characters between <see cref="StyleCharacter.Asterisk"/> and <see cref="StyleCharacter.Underscore"/> when styles are nested.
/// List depth is tracked via an integer counter, enabling context-aware formatting for nested lists.
/// </remarks>
public partial class MarkdownStreamWriter(
    Stream stream,
    MarkdownStyle style,
    Encoding encoding,
    IFormatProvider? formatProvider,
    ILogger<MarkdownStreamWriter>? logger = null)
    : IndentedStreamWriter(stream, style.IndentSpec, encoding, formatProvider, logger)
{
    /// <summary>Defines the possible inline style states managed by the writer's style stack.</summary>
    public enum StyleState
    {
        /// <summary>No active inline style.</summary>
        None,
        /// <summary>Emphasis (italic) style is active.</summary>
        Emphasis,
        /// <summary>Strong (bold) style is active.</summary>
        Strong
    }

    private sealed class StyleStackEntry
    {
        public StyleState State { get; set; }
        public StyleCharacter Character { get; set; }
    }

    private readonly Stack<StyleStackEntry> _styleStack = new();

    private int _listCounter;

    private readonly ILogger<MarkdownStreamWriter> _logger = logger ?? NullLogger<MarkdownStreamWriter>.Instance;

    /// <summary>Initializes a new instance of <see cref="MarkdownStreamWriter"/> with default style settings and UTF-8 encoding.</summary>
    /// <param name="stream">The stream to write Markdown output to.</param>
    public MarkdownStreamWriter(Stream stream) : this(stream, new MarkdownStyle())
    {
    }

    /// <summary>Initializes a new instance of <see cref="MarkdownStreamWriter"/> with the specified style and UTF-8 encoding.</summary>
    /// <param name="stream">The stream to write Markdown output to.</param>
    /// <param name="style">The Markdown style configuration.</param>
    public MarkdownStreamWriter(Stream stream, MarkdownStyle style) : this(stream, style, Encoding.UTF8)
    {
    }

    /// <summary>Initializes a new instance of <see cref="MarkdownStreamWriter"/> with the specified style and encoding.</summary>
    /// <param name="stream">The stream to write Markdown output to.</param>
    /// <param name="style">The Markdown style configuration.</param>
    /// <param name="encoding">The character encoding to use.</param>
    public MarkdownStreamWriter(Stream stream, MarkdownStyle style, Encoding encoding) : this(stream, style, encoding, null)
    {
    }

    /// <summary>Gets the <see cref="MarkdownStyle"/> that governs delimiter selection and indentation for this writer.</summary>
    public MarkdownStyle Style { get; } = style;

    /// <summary>Gets a value indicating whether the writer is currently inside at least one list.</summary>
    public bool IsInList => _listCounter > 0;

    /// <summary>Gets a value indicating whether the writer is currently nested inside two or more lists.</summary>
    public bool IsInSubList => _listCounter > 1;

    /// <summary>Opens an emphasis (italic) inline style marker at the current position.</summary>
    public void PushEmphasis() => PushStyle(StyleState.Emphasis);

    /// <summary>Opens a strong (bold) inline style marker at the current position.</summary>
    public void PushStrong() => PushStyle(StyleState.Strong);

    /// <summary>Closes the current emphasis (italic) inline style marker.</summary>
    public void PopEmphasis() => PopStyle(StyleState.Emphasis);

    /// <summary>Closes the current strong (bold) inline style marker.</summary>
    public void PopStrong() => PopStyle(StyleState.Strong);

    /// <summary>Increments the active list nesting counter.</summary>
    public void AddListCount() => _listCounter++;

    /// <summary>Decrements the active list nesting counter.</summary>
    /// <exception cref="InvalidOperationException">The list counter is already zero.</exception>
    public void RemoveListCount()
    {
        if (_listCounter == 0)
            throw new InvalidOperationException("Cannot remove list count when it is already 0.");

        _listCounter--;
    }


    /// <summary>Pushes <paramref name="state"/> onto the style stack and writes its opening delimiter character.</summary>
    /// <param name="state">The inline style state to activate.</param>
    /// <exception cref="InvalidOperationException"><paramref name="state"/> is invalid or an alternate delimiter cannot be resolved.</exception>
    public void PushStyle(StyleState state)
    {
        if (_styleStack.Any((i) => i.State == state))
            LogAleradyActiveStyle(state);

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

    /// <summary>Pops <paramref name="state"/> from the style stack and writes its closing delimiter character.</summary>
    /// <param name="state">The inline style state to deactivate; must match the top of the stack.</param>
    /// <exception cref="InvalidOperationException">The top of the stack does not match <paramref name="state"/>.</exception>
    public void PopStyle(StyleState state)
    {
        if (_styleStack.Peek().State != state)
            throw new InvalidOperationException("Cannot pop inactive style.");

        StyleCharacter styleCharacter = _styleStack.Pop().Character;
        char characterToWrite = GetCharacterToWrite(styleCharacter);

        WriteCharacterForStyle(state, characterToWrite);
    }

    private static char GetCharacterToWrite(StyleCharacter styleCharacter) => styleCharacter switch
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

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "The style {StyleState} is already active.")]
    private partial void LogAleradyActiveStyle(StyleState styleState);

}
