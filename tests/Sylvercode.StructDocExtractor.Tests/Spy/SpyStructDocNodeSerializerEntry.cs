using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.StructDocExtractor.Tests.Spy;

public class SpyStructDocNodeSerializerEntry(ISerializer serializer, string methodName, IEnumerable<string?> parameters)
{
    public ISerializer Serializer { get; } = serializer;
    public string MethodName { get; } = methodName;
    public List<string?> ParamsId { get; } = [.. parameters];

    public void Assert(SpyStructDocNodeSerializerEntry expected)
    {
        Xunit.Assert.Equal(expected.Serializer, this.Serializer);
        Xunit.Assert.Equal(expected.MethodName, this.MethodName);
        Xunit.Assert.Equal(expected.ParamsId, this.ParamsId);
    }
}
