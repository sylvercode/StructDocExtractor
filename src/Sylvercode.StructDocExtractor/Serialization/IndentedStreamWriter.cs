using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>
/// <see cref="TextWriter"/> decorator that automatically injects configurable indentation and manages lazy spacing
/// operations (word, line, and paragraph breaks) on behalf of the serialization pipeline.
/// </summary>
/// <remarks>
/// <para>
/// Indentation characters are written at the start of each line based on the current <see cref="IndentLevel"/>
/// and the <see cref="IndentSpec"/> supplied at construction. Spacing operations
/// (<see cref="SpaceOperationType"/>) are deferred until the next character write, allowing serializers to
/// declare spacing intent without knowing whether preceding content already produced the required whitespace.
/// </para>
/// <para>
/// Line-head tokens (see <see cref="PushLineHeadToken"/>) are automatically appended after each newline,
/// supporting Markdown-style block prefixes (e.g., <c>&gt;</c> for block quotes) without extra bookkeeping
/// in individual serializers.
/// </para>
/// <para>Thread safety: the underlying <see cref="Stream"/> is protected by a lock during every write.</para>
/// </remarks>
public class IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding, IFormatProvider? formatProvider, ILogger<IndentedStreamWriter>? logger = null)
    : TextWriter(formatProvider)
{
    /// <summary>Specifies the kind of whitespace transition to perform before or after writing content.</summary>
    public enum SpaceOperationType
    {
        /// <summary>No spacing operation; content is written without additional whitespace.</summary>
        None,

        /// <summary>Ensures a single space between the previous token and the next word.</summary>
        Word,

        /// <summary>Ensures the writer is at the beginning of a new line before the next write.</summary>
        Line,

        /// <summary>Ensures a blank line (paragraph break) precedes the next write.</summary>
        Paragraph,
    }

    private static byte[] BuildIndentBuffer(IndentSpec indentSpec, Encoding encoding)
    {
        if (indentSpec.Type == IndentType.Space)
            return encoding.GetBytes(new string(' ', indentSpec.Size));
        else
            return encoding.GetBytes("\t");
    }

    private readonly Stream _stream = stream;

    private byte[] indentBuffer = BuildIndentBuffer(indentSpec, encoding);

    private int _indentLevel;

    private bool _atLineStart = true;

    private bool _PreviousLineIsEmpty = true;

    private bool _IsAfterSpace = true;

    private SpaceOperationType _pendingOperation = SpaceOperationType.None;

    private readonly LinkedList<string> _LineHeadTokens = new();

    private readonly ILogger<IndentedStreamWriter>? _logger = logger ?? NullLogger<IndentedStreamWriter>.Instance;

    /// <summary>Initializes a new instance of <see cref="IndentedStreamWriter"/> with the specified stream, indent spec, and encoding.</summary>
    /// <param name="stream">The underlying stream to write to.</param>
    /// <param name="indentSpec">The indentation style and size to apply.</param>
    /// <param name="encoding">The character encoding used when writing bytes.</param>
    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding) : this(stream, indentSpec, encoding, null)
    {
    }

    /// <summary>Initializes a new instance of <see cref="IndentedStreamWriter"/> with the specified stream and indent spec, using UTF-8 encoding.</summary>
    /// <param name="stream">The underlying stream to write to.</param>
    /// <param name="indentSpec">The indentation style and size to apply.</param>
    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec) : this(stream, indentSpec, Encoding.UTF8, null)
    {
    }

    /// <summary>Initializes a new instance of <see cref="IndentedStreamWriter"/> with the specified stream, using a default <see cref="IndentSpec"/> and UTF-8 encoding.</summary>
    /// <param name="stream">The underlying stream to write to.</param>
    public IndentedStreamWriter(Stream stream) : this(stream, new IndentSpec())
    {
    }

    /// <summary>Immediately performs the given <paramref name="type"/> spacing operation, resolving any previously pending operation first.</summary>
    /// <param name="type">The spacing operation to execute.</param>
    public void DoSpaceOperation(SpaceOperationType type)
    {
        CheckPendingOperation();
        switch (type)
        {
            case SpaceOperationType.None:
                break;
            case SpaceOperationType.Word:
                StartWord();
                break;
            case SpaceOperationType.Line:
                StartLine();
                break;
            case SpaceOperationType.Paragraph:
                StartParagraph();
                break;
            default:
                throw new InvalidOperationException($"Unknown pending operation type: {type}");
        }
    }

    /// <summary>Ensures the writer is positioned at the start of a new line, writing a newline if the current line is not empty.</summary>
    public void StartLine()
    {
        CheckPendingOperation();
        if (!_atLineStart)
            WriteLine();
    }

    /// <summary>Ensures a blank line precedes the next write, starting a new line and adding an empty line if necessary.</summary>
    public void StartParagraph()
    {
        CheckPendingOperation();
        StartLine();
        if (!_PreviousLineIsEmpty)
            WriteLine();
    }

    /// <summary>Ensures at least one space separates the last written token from the next, writing a space if necessary.</summary>
    public void StartWord()
    {
        CheckPendingOperation();
        if (!_IsAfterSpace)
            Write(' ');
    }

    /// <summary>Schedules a <see cref="SpaceOperationType.Word"/> spacing operation to be resolved before the next character write.</summary>
    public void EnsureEndWordNext() => EnsureSpaceOperation(SpaceOperationType.Word);

    /// <summary>Schedules a <see cref="SpaceOperationType.Line"/> spacing operation to be resolved before the next character write.</summary>
    public void EnsureEndLineNext() => EnsureSpaceOperation(SpaceOperationType.Line);

    /// <summary>Schedules a <see cref="SpaceOperationType.Paragraph"/> spacing operation to be resolved before the next character write.</summary>
    public void EnsureEndParagraphNext() => EnsureSpaceOperation(SpaceOperationType.Paragraph);

    /// <summary>Promotes the pending spacing operation to <paramref name="type"/> if <paramref name="type"/> is of higher precedence.</summary>
    /// <param name="type">The minimum spacing operation to ensure before the next write.</param>
    public void EnsureSpaceOperation(SpaceOperationType type)
    {
        if (_pendingOperation < type)
            _pendingOperation = type;
    }

    /// <summary>Gets the currently pending spacing operation that will be resolved before the next character write.</summary>
    public SpaceOperationType PendingOperation => _pendingOperation;

    /// <summary>Pushes a token string that is prepended to the start of each new line until popped.</summary>
    /// <param name="token">The string to prepend after each newline (e.g., <c>&gt; </c> for Markdown block quotes).</param>
    public void PushLineHeadToken(string token) => _LineHeadTokens.AddLast(token);

    /// <summary>Removes the most recently pushed line-head token.</summary>
    /// <exception cref="InvalidOperationException">Thrown if there are no line-head tokens to pop.</exception>
    public void PopLineHeadToken()
    {
        if (_LineHeadTokens.Count == 0)
            throw new InvalidOperationException("No line head token to pop.");
        CheckPendingOperation();
        _LineHeadTokens.RemoveLast();
    }

    private void CheckPendingOperation()
    {
        if (_pendingOperation == SpaceOperationType.None)
            return;

        (SpaceOperationType type, _pendingOperation) = (_pendingOperation, SpaceOperationType.None);
        DoSpaceOperation(type);
    }

    /// <summary>Gets or sets the current indent depth; setting to zero removes all indentation.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
    public int IndentLevel
    {
        get => field;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Indent level must be non-negative.");
            field = value;
            indentBuffer = BuildIndentBuffer(indentSpec, encoding);
        }
    }

    /// <summary>Increases the indent level by <paramref name="value"/> steps.</summary>
    /// <param name="value">The number of indent levels to add; must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1.</exception>
    public void Indent(int value = 1)
    {
        if (value < 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Indent value must be positive and non-zero.");
        IndentLevel += value;
    }

    /// <summary>Decreases the indent level by <paramref name="value"/> steps.</summary>
    /// <param name="value">The number of indent levels to remove; must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1.</exception>
    public void Unindent(int value = 1)
    {
        if (value < 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Indent value must be positive and non-zero.");
        IndentLevel -= value;
    }

    /// <inheritdoc/>
    public override Encoding Encoding => encoding;

    /// <summary>Writes a single character to the underlying stream, injecting indent characters and resolving pending spacing operations as needed.</summary>
    /// <param name="value">The character to write.</param>
    public override void Write(char value)
    {
        lock (_stream)
        {
            CheckPendingOperation();

            if (value == '\n')
            {
                _IsAfterSpace = true;
                if (_atLineStart)
                    _PreviousLineIsEmpty = true;
                else
                {
                    _PreviousLineIsEmpty = false;
                    _atLineStart = true;
                }
            }
            else if (value == ' ')
            {
                _IsAfterSpace = true;
                if (_atLineStart)
                    return;
            }
            else if (_atLineStart)
            {
                for (int i = 0; i < IndentLevel; i++)
                    _stream.Write(indentBuffer, 0, indentBuffer.Length);

                _atLineStart = false;
                _IsAfterSpace = true;
            }
            else
            {
                _atLineStart = false;
                _IsAfterSpace = false;
            }

            byte[] buffer = Encoding.GetBytes([value]);
            _stream.Write(buffer, 0, buffer.Length);

            if (value == '\n')
            {
                if (_LineHeadTokens.Count > 0)
                {
                    string head = string.Join("", _LineHeadTokens);
                    buffer = Encoding.GetBytes(head);
                    _stream.Write(buffer, 0, buffer.Length);
                    _IsAfterSpace = head[^1] == ' ';
                }
            }
        }
    }

    /// <inheritdoc/>
    public override void Close()
    {
        base.Close();
        lock (_stream)
            _stream.Close();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _stream.Dispose();
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        lock (_stream)
            _stream.Flush();
    }
}
