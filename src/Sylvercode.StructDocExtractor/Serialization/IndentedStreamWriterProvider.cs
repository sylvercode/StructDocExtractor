using System.Text;
using Microsoft.Extensions.Options;

namespace Sylvercode.StructDocExtractor.Serialization;

public class IndentedStreamWriterProvider(IOptions<IndentedStreamWriterProvider.Options> options) : ITextWriterProvider
{
    public class Options
    {
        public IndentedStreamWriter.IndentSpec IndentSpec { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public IFormatProvider? FormatProvider { get; set; }
    }

    public TextWriter GetTextWriter(Stream stream)
    {
        Options writerOptions = options.Value;
        return new IndentedStreamWriter(stream,
                                        writerOptions.IndentSpec,
                                        writerOptions.Encoding,
                                        writerOptions.FormatProvider);
    }
}
