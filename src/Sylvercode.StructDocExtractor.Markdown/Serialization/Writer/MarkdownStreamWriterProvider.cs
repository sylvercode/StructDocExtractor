using System.Text;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;

/// <summary>Implementation of <see cref="ITextWriterProvider{TWriter}"/> that creates <see cref="MarkdownStreamWriter"/> instances configured from DI options.</summary>
public class MarkdownStreamWriterProvider(IOptions<MarkdownStreamWriterProvider.Options> options) : ITextWriterProvider<MarkdownStreamWriter>
{
    /// <summary>Configuration options for <see cref="MarkdownStreamWriterProvider"/>.</summary>
    public class Options
    {
        /// <summary>Gets or sets the indentation specification for the Markdown writer.</summary>
        public IndentSpec IndentSpec { get; set; } = MarkdownStyle.Default.IndentSpec;
        /// <summary>Gets or sets the character encoding used by the writer.</summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>Gets or sets the format provider passed to the writer, or <see langword="null"/> for the current culture.</summary>
        public IFormatProvider? FormatProvider { get; set; }
        /// <summary>Gets or sets the delimiter character used for emphasis (italic) markers.</summary>
        public StyleCharacter EmphasisCharacter { get; set; } = MarkdownStyle.Default.EmphasisCharacter;
        /// <summary>Gets or sets the delimiter character used for strong (bold) markers.</summary>
        public StyleCharacter StrongCharacter { get; set; } = MarkdownStyle.Default.StrongCharacter;
        /// <summary>Gets or sets a value indicating whether alternate delimiter characters are preferred for nested inline styles.</summary>
        public bool PreferAlternateStyle { get; set; } = MarkdownStyle.Default.PreferAlternateStyle;
    }

    /// <summary>Creates and returns a new <see cref="MarkdownStreamWriter"/> for the specified stream using the configured options.</summary>
    /// <param name="stream">The stream to write Markdown output to.</param>
    /// <returns>A configured <see cref="MarkdownStreamWriter"/> instance.</returns>
    public MarkdownStreamWriter GetTextWriter(Stream stream)
    {
        Options writerOptions = options.Value;
        MarkdownStyle style = new()
        {
            IndentSpec = writerOptions.IndentSpec,
            EmphasisCharacter = writerOptions.EmphasisCharacter,
            StrongCharacter = writerOptions.StrongCharacter,
            PreferAlternateStyle = writerOptions.PreferAlternateStyle
        };
        return new MarkdownStreamWriter(stream,
                                        style,
                                        writerOptions.Encoding,
                                        writerOptions.FormatProvider);
    }
}
