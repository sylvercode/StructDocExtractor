using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class StructDocSerializerMock(MockCallTracker tracker) : IStructDocSerializer
{
    public void Serialize(TextWriter stream, IStructDocNode rootData)
    {
        tracker.TrackCall(this, nameof(Serialize), [stream, rootData]);
    }
}
