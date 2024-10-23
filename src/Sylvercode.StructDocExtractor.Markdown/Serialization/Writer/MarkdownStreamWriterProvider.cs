using System.Text;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Markdown.Serialization.Writer;

public class MarkdownStreamWriterProvider(IOptions<MarkdownStreamWriterProvider.Options> options) : ITextWriterProvider<MarkdownStreamWriter>
{
    public class Options
    {
        public IndentSpec IndentSpec { get; set; } = MarkdownStyle.Default.IndentSpec;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public IFormatProvider? FormatProvider { get; set; }
        public StyleCharacter EmphasisCharacter { get; set; } = MarkdownStyle.Default.EmphasisCharacter;
        public StyleCharacter StrongCharacter { get; set; } = MarkdownStyle.Default.StrongCharacter;
        public bool PreferAlternateStyle { get; set; } = MarkdownStyle.Default.PreferAlternateStyle;
    }

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
