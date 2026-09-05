using System.Text;
using Microsoft.Extensions.Options;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Creates <see cref="IndentedStreamWriter"/> instances configured from a snapshot of <see cref="Options"/>.</summary>
/// <remarks>
/// Implements <see cref="ITextWriterProvider{IndentedStreamWriter}"/> so it can be injected wherever an
/// <see cref="ITextWriterProvider"/> or a typed provider is needed. Configure encoding and indentation
/// via <see cref="Options"/> and DI; use <see cref="IndentedStreamWriterProviderExtensions.AddIndentedStreamWriterProvider"/> to register.
/// </remarks>
public class IndentedStreamWriterProvider(IOptions<IndentedStreamWriterProvider.Options> options) : ITextWriterProvider<IndentedStreamWriter>
{
    /// <summary>Configuration options for <see cref="IndentedStreamWriterProvider"/>.</summary>
    public class Options
    {
        /// <summary>Gets or sets the indentation specification applied to each created writer.</summary>
        public IndentSpec IndentSpec { get; set; } = new();

        /// <summary>Gets or sets the text encoding used by each created writer.</summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>Gets or sets the format provider passed to each created writer, or <see langword="null"/> to use the default.</summary>
        public IFormatProvider? FormatProvider { get; set; }
    }

    /// <inheritdoc/>
    public IndentedStreamWriter GetTextWriter(Stream stream)
    {
        Options writerOptions = options.Value;
        return new IndentedStreamWriter(stream,
                                        writerOptions.IndentSpec,
                                        writerOptions.Encoding,
                                        writerOptions.FormatProvider);
    }
}
