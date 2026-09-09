namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Contract for creating a <see cref="TextWriter"/> adapter over a raw <see cref="Stream"/>.</summary>
public interface ITextWriterProvider
{
    /// <summary>Returns a <see cref="TextWriter"/> that writes to the given <paramref name="stream"/>.</summary>
    /// <param name="stream">The underlying stream to write to.</param>
    /// <returns>A <see cref="TextWriter"/> wrapping <paramref name="stream"/>.</returns>
    TextWriter GetTextWriter(Stream stream);
}

/// <summary>Typed provider contract that produces a specific <typeparamref name="TWriter"/> subtype.</summary>
/// <typeparam name="TWriter">The concrete <see cref="TextWriter"/> type this provider creates.</typeparam>
public interface ITextWriterProvider<TWriter> : ITextWriterProvider
    where TWriter : TextWriter
{
    /// <summary>Returns a typed <typeparamref name="TWriter"/> wrapping the given <paramref name="stream"/>.</summary>
    /// <param name="stream">The underlying stream to write to.</param>
    /// <returns>A <typeparamref name="TWriter"/> wrapping <paramref name="stream"/>.</returns>
    new TWriter GetTextWriter(Stream stream);
    TextWriter ITextWriterProvider.GetTextWriter(Stream stream) => GetTextWriter(stream);
}
