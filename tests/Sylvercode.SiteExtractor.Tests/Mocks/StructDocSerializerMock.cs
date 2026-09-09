using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

/// <summary>Mock <see cref="IStructDocSerializer"/> returning stub serialisation strings.</summary>
public class StructDocSerializerMock(MockCallTracker tracker) : IStructDocSerializer
{
    /// <inheritdoc/>
    public ITextWriterProvider? TextWriterProvider => null;

    /// <summary>Records the <see cref="Serialize"/> call via the tracker without writing any output.</summary>
    /// <param name="stream">The target <see cref="TextWriter"/> (unused by this mock).</param>
    /// <param name="rootData">The root node passed to the serializer.</param>
    public void Serialize(TextWriter stream, IStructDocNode rootData)
    {
        tracker.TrackCall(this, nameof(Serialize), [stream, rootData]);
    }
}
