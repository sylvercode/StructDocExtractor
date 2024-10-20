using System.Text;

namespace Sylvercode.StructDocExtractor.Serialization;

public class IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding, IFormatProvider? formatProvider)
    : TextWriter(formatProvider)
{

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

    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding) : this(stream, indentSpec, encoding, null)
    {
    }

    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec) : this(stream, indentSpec, Encoding.UTF8, null)
    {
    }

    public IndentedStreamWriter(Stream stream) : this(stream, new IndentSpec())
    {
    }

    public void StartLine()
    {
        if (!_atLineStart)
            WriteLine();
    }

    public void StartParagraph()
    {
        StartLine();
        if (!_PreviousLineIsEmpty)
            WriteLine();
    }

    public void StartWord()
    {
        if (!_IsAfterSpace)
            Write(' ');
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

            byte[] buffer = Encoding.GetBytes([value]);
            _stream.Write(buffer, 0, buffer.Length);
        }
    }
}
