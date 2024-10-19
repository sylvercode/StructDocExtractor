using System.Text;

namespace Sylvercode.StructDocExtractor.Serialization;

public class IndentedStreamWriter(Stream stream, IndentedStreamWriter.IndentSpec indentSpec, Encoding encoding, IFormatProvider? formatProvider)
    : TextWriter(formatProvider)
{
    public struct IndentSpec()
    {
        public bool IsSpaceIndent { get; set; } = true;
        public int IndentSize { get; set; } = 4;
    }

    private static byte[] BuildIndentBuffer(IndentSpec indentSpec, Encoding encoding)
    {
        if (indentSpec.IsSpaceIndent)
            return encoding.GetBytes(new string(' ', indentSpec.IndentSize));
        else
            return encoding.GetBytes("\t");
    }

    private readonly Stream _stream = stream;

    private byte[] indentBuffer = BuildIndentBuffer(indentSpec, encoding);

    private int _indentLevel;

    private bool _atLineStart = true;

    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec, Encoding encoding) : this(stream, indentSpec, encoding, null)
    {
    }

    public IndentedStreamWriter(Stream stream, IndentSpec indentSpec) : this(stream, indentSpec, Encoding.UTF8, null)
    {
    }

    public IndentedStreamWriter(Stream stream) : this(stream, new IndentSpec { IsSpaceIndent = true, IndentSize = 4 })
    {
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

    public override Encoding Encoding => encoding;

    public override void Write(char value)
    {
        lock (_stream)
        {
            if (value == '\n')
                _atLineStart = true;
            else if (_atLineStart)
            {
                for (int i = 0; i < IndentLevel; i++)
                    _stream.Write(indentBuffer, 0, indentBuffer.Length);

                _atLineStart = false;
            }

            byte[] buffer = Encoding.GetBytes([value]);
            _stream.Write(buffer, 0, buffer.Length);
        }
    }
}
