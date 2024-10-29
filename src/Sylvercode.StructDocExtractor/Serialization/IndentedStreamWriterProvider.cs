using System.Text;
using Microsoft.Extensions.Options;

namespace Sylvercode.StructDocExtractor.Serialization;

public class IndentedStreamWriterProvider(IOptions<IndentedStreamWriterProvider.Options> options) : ITextWriterProvider<IndentedStreamWriter>
{
    public class Options
    {
        public IndentSpec IndentSpec { get; set; } = new();
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public IFormatProvider? FormatProvider { get; set; }
    }

    public IndentedStreamWriter GetTextWriter(Stream stream)
    {
        Options writerOptions = options.Value;
        return new IndentedStreamWriter(stream,
                                        writerOptions.IndentSpec,
                                        writerOptions.Encoding,
                                        writerOptions.FormatProvider);
    }
}
