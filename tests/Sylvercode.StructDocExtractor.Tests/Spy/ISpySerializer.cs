using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public interface ISpySerializer : ISerializer
{
    List<SpyStructDocNodeSerializerEntry> EntriesLog { get; }
}
