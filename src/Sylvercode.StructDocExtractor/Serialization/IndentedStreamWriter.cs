using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.StructDocExtractor.Serialization;

public class IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding, IFormatProvider? formatProvider, ILogger<IndentedStreamWriter>? logger = null)
    : TextWriter(formatProvider)
{
    public enum SpaceOperationType
    {
        None,
        Word,
        Line,
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

    private readonly ILogger<IndentedStreamWriter>? _logger = logger ?? NullLogger<IndentedStreamWriter>.Instance;

    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding) : this(stream, indentSpec, encoding, null)
    {
    }

    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec) : this(stream, indentSpec, Encoding.UTF8, null)
    {
    }

    public IndentedStreamWriter(Stream stream) : this(stream, new IndentSpec())
    {
    }

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

    public void StartLine()
    {
        CheckPendingOperation();
        if (!_atLineStart)
            WriteLine();
    }

    public void StartParagraph()
    {
        CheckPendingOperation();
        StartLine();
        if (!_PreviousLineIsEmpty)
            WriteLine();
    }

    public void StartWord()
    {
        CheckPendingOperation();
        if (!_IsAfterSpace)
            Write(' ');
    }

    public void EnsureEndWordNext() => EnsureSpaceOperation(SpaceOperationType.Word);

    public void EnsureEndLineNext() => EnsureSpaceOperation(SpaceOperationType.Line);

    public void EnsureEndParagraphNext() => EnsureSpaceOperation(SpaceOperationType.Paragraph);

    public void EnsureSpaceOperation(SpaceOperationType type)
    {
        if (_pendingOperation < type)
            _pendingOperation = type;
    }

    public SpaceOperationType PendingOperation => _pendingOperation;

    private void CheckPendingOperation()
    {
        if (_pendingOperation == SpaceOperationType.None)
            return;

        (SpaceOperationType type, _pendingOperation) = (_pendingOperation, SpaceOperationType.None);
        DoSpaceOperation(type);
    }

    public int IndentLevel
    {
        get => _indentLevel;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Indent level must be non-negative.");
            _indentLevel = value;
            indentBuffer = BuildIndentBuffer(indentSpec, encoding);
        }
    }

    public void Indent(int value = 1)
    {
        if (value < 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Indent value must be positive and non-zero.");
        IndentLevel += value;
    }

    public void Unindent(int value = 1)
    {
        if (value < 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Indent value must be positive and non-zero.");
        IndentLevel -= value;
    }

    public override Encoding Encoding => encoding;

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
        }
    }

    public override void Close()
    {
        base.Close();
        lock (_stream)
            _stream.Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _stream.Dispose();
        base.Dispose(disposing);
    }

    public override void Flush()
    {
        lock (_stream)
            _stream.Flush();
    }
}
