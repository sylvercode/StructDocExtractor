namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>In-memory adapter that creates and owns a typed <typeparamref name="TWriter"/> over a <see cref="MemoryStream"/>, allowing serialized output to be retrieved as a string.</summary>
/// <typeparam name="TWriter">The specific <see cref="TextWriter"/> subtype to create via <see cref="ITextWriterProvider{TWriter}"/>.</typeparam>
public class StringTextWriter<TWriter> : IDisposable
    where TWriter : TextWriter
{
    private Stream? _stream = new MemoryStream();

    private TWriter? _writer;

    /// <summary>Initializes a new instance of <see cref="StringTextWriter{TWriter}"/> using <paramref name="provider"/> to create the writer.</summary>
    /// <param name="provider">The provider used to create the typed writer over the internal memory stream.</param>
    public StringTextWriter(ITextWriterProvider<TWriter> provider)
    {
        _writer = provider.GetTextWriter(_stream);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases the underlying stream and writer resources.</summary>
    /// <param name="disposing"><see langword="true"/> if called from <see cref="Dispose()"/>; <see langword="false"/> if from the finalizer.</param>
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

    /// <summary>Gets the typed writer that serializers write to.</summary>
    /// <exception cref="ObjectDisposedException">Thrown if this instance has been disposed.</exception>
    public TWriter Writer
    {
        get
        {
            ObjectDisposedException.ThrowIf(_stream == null || _writer == null, this);
            return _writer;
        }
    }

    /// <summary>Flushes the writer and returns all serialized output as a string.</summary>
    /// <returns>The complete serialized content accumulated in the internal memory stream.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if this instance has been disposed.</exception>
    public string GetResult()
    {
        ObjectDisposedException.ThrowIf(_stream == null || _writer == null, this);

        _writer.Flush();
        _stream.Position = 0;
        using StreamReader reader = new(_stream);
        return reader.ReadToEnd();
    }
}
