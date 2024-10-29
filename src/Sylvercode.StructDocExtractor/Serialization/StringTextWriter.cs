namespace Sylvercode.StructDocExtractor.Serialization;

public class StringTextWriter<TWriter> : IDisposable
    where TWriter : TextWriter
{
    private Stream? _stream = new MemoryStream();

    private TWriter? _writer;

    public StringTextWriter(ITextWriterProvider<TWriter> provider)
    {
        _writer = provider.GetTextWriter(_stream);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }
    }

    public TWriter Writer
    {
        get
        {
            ObjectDisposedException.ThrowIf(_stream == null || _writer == null, this);
            return _writer;
        }
    }

    public string GetResult()
    {
        ObjectDisposedException.ThrowIf(_stream == null || _writer == null, this);

        _writer.Flush();
        _stream.Position = 0;
        using StreamReader reader = new(_stream);
        return reader.ReadToEnd();
    }
}
